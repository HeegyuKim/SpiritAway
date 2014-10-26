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

			AddScriptIfExists();

			switch (arrived.Type)
			{
				case "crossroad":

					processes.Add(
						new SelectProcess(
							app.VoiceControl,
							stage.ScriptView,
							stage.Player,
							stage.Map,
							arrived.Links
							)
						);


					break;

				case "waypoint":

					if(arrived.Next != null)
						processes.Add(
							new StartProcess(
								stage,
								arrived.Next
								)
							);

					break;

				case "medical_kit":

					processes.Add(
						new StartProcess (
							stage,
							arrived.Next
							)
						);

					break;

				default:
					processes.Clear();
					Console.WriteLine("Unknown type waypoint. " + arrived.Type);
					break;
			}

			if(processes.Count > 0)
				processes.First().Start();
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
