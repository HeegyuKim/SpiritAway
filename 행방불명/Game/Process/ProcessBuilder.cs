using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;
using 행방불명.Game;

namespace 행방불명.Game.Process
{
	public class ProcessBuilder
	{
		GameStage stage;
		Program app;
		
		
		public ProcessBuilder(Program app, GameStage stage)
		{
			this.app = app;
			this.stage = stage;
		}

		List<IProcess> processes;
		Waypoint arrived;


		public void AddProcess(List<IProcess> processes, Waypoint arrived)
		{
			this.processes = processes;
			this.arrived = arrived;

			Console.WriteLine("Arrived to {0}: {1} ({2}, {3})",
				arrived.Id,
				arrived.Type,
				arrived.X,
				arrived.Y
				);

			SavePeople();
			AddScriptIfExists();
			ProcessLinks();

			arrived.Used = true;

			if(processes.Count > 0)
				processes.First().Start();
		}

		private void ProcessLinks()
		{
			if (arrived.Links != null)
			{
				processes.Add(
				  new SelectProcess(
					  stage,
					  arrived.Links
					  )
				  );
			}
			else if (arrived.Link1 != null && arrived.Link2 != null)
			{
				var moveFrom = stage.Player.MoveFrom;
				string link;

				if (moveFrom.Id.Equals(arrived.Link1))
					link = arrived.Link2;
				else
					link = arrived.Link1;

				var process = new StartProcess(stage.Player, stage.Map.GetWaypoint(link));
				processes.Add(process);
			}
			else if (arrived.Next != null)
			{
				processes.Add(
					new StartProcess(
						stage,
						arrived.Next
						)
					);
			}

			switch(arrived.Type)
			{
				case "medical_kit":
					if (!arrived.Used)
					{
						Console.WriteLine("응급상자 겟!");
						stage.Player.NumMedicalKits++;
					}
					break;
				case "hammer":
					if (!arrived.Used)
					{
						Console.WriteLine("망치 겟!");
						stage.Player.HasHammer = true;
					}
					break;
				case "key":
					if (!arrived.Used)
					{
						Console.WriteLine("열쇠 겟!");
						stage.Player.HasKey = true;
					}
					break;
			}
		}

		private void SavePeople()
		{
			if (arrived.Used) return;
			if (arrived.NumObtained == 0) return;

			var player = stage.Player;
			player.NumObtained += arrived.NumObtained;
			player.NumPatients += arrived.NumPatients;

			List<Script> scripts = new List<Script>();
			scripts.Add(
				new Script(
					"이대원",
					"생존자가 있습니다!",
					null
					)
				);

			processes.Add(
				new DialogProcess(
					app,
					stage.ScriptView,
					new Talking(scripts)
					)
				);
		}

		private void AddScriptIfExists()
		{
			if (arrived.Scripts == null) return;

			processes.Add(
				new DialogProcess(
					app,
					stage.ScriptView,
					new Talking(arrived.Scripts)
					)
				);

		}
	}
}
