﻿using System;
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
		bool arriveOnEnd = true;

		public bool ArriveOnEnd { get { return arriveOnEnd; } }


		public void AddProcess(
			List<IProcess> processes, 
			Waypoint arrived, 
			bool arriveOnEnd = true)
		{
			this.arriveOnEnd = arriveOnEnd;
			this.processes = processes;
			this.arrived = arrived;


			SavePeople();
			if(arriveOnEnd)
				AddScriptIfExists();
			ProcessType();
			ProcessLinks();

			arrived.Used = true;

			if(processes.Count > 0)
				processes.First().Start();
		}

		private void ProcessLinks()
		{
			if (!arrived.Used && arrived.Type.Equals("gas_valve")) return;

            if(arrived.Thanks != null && 
                (!arrived.Thanks.Paused && !arrived.Thanks.Finished))
            {
                arrived.Thanks.Stop();
                arrived.Thanks = null;
            }

			if (arrived.Links != null)
			{
                if(stage.Player.RunningAway)
                {
                    Waypoint way = stage.Map.GetWaypoint(
                        arrived.Links[app.Random.Next(arrived.Links.Count)].Id
                        );

                    processes.Add(
                        new StartProcess (
                            stage,
                            way
                            )
                        );
                }
                else
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

				var process = new StartProcess(stage, stage.Map.GetWaypoint(link));
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

		}

		private void ProcessType()
		{

			switch (arrived.Type)
			{
                case "locking_gas_valve":
                    stage.Player.LockGasValve = true;
                    break;

                case "blocked":
                    if (arrived.Used) 
                        break;

                    {
                        var obj = new GameObject(
                            arrived.X,
                            arrived.Y,
                            arrived.Id,
                            app.Media.BitmapDic["locked"]
                            );
                        stage.GameObjects.Add(obj);
                    }
                    break;
				case "gas_valve":
					if (arrived.Used) 
                        break;

					{
						var gas = new Script(
							"요리사",
							"주변에 가스벨브가 열렸는지 확인해야 합니다.",
							null,
							"chef_check_gas_please"
							);

						var links = new List<Link>(arrived.Links);
						links.Add(new Link()
						{
							Id = "kitchen_gas_dangerous",
							Name = "가스벨브를 잠궈",
							Required = null
						});
						var select = new SelectProcess(
							stage,
							gas,
							links
							);

						processes.Add(select);
					}
					break;
				case "key":
                    {
                        app.Sound.Play2D("notification");

						if (arrived.Used) break;

						stage.Player.HasKey = true;

						var talking = new Talking(
							new Script(
								"경비 아저씨",
								"나가도 되겠습니까?.",
								null,
								"keyman_script1"
								),
							new Script(
								"이대원",
								"예, 비상구 열쇠가 필요합니다.",
								null,
                                "need_key"
								),
							new Script(
								"경비 아저씨",
								"여기 있소, 어서 나갑시다.",
								null,
								"keyman_script2"
								)
							);

						processes.Add(
							new DialogProcess(
									app,
									stage.ScriptView,
									talking
								)
							);

						break;
					}
				case "medical_kit":
					if (!arrived.Used && !stage.Map.IsTutorial() )
					{
                        app.Sound.Play2D("notification");

						List<Script> scripts = new List<Script>();
						scripts.Add(
							new Script(
								"이대원",
								"의료상자가 있습니다! 이걸로 부상자분들을 치료할 수 있겠습니다.",
								null,
								"find_medical_kit"
								)
							);

						stage.Player.NumMedicalKits++;
						if (stage.Player.NumPatients > 0)
						{
							stage.Player.Cure(1);
							Console.WriteLine("응급상자 얻었으나 사용, 현재 {0}개", stage.Player.NumMedicalKits);

							scripts.Add(
								new Script(
									"이대원",
									"부상자분들을 치료해드렸습니다.",
									null,
									"cure_patients#" + (app.Random.Next(3) + 1)
									)
								);
						}
						else
						{
							Console.WriteLine("응급상자 얻음, 현재 {0}개", stage.Player.NumMedicalKits);
						}
						var talking = new Talking(scripts);

						processes.Add(
							new DialogProcess(
									app,
									stage.ScriptView,
									talking
								)
							);
					}
					break;
				case "hammer":
					if (!arrived.Used)
                    {
                        app.Sound.Play2D("notification");

						stage.Player.HasHammer = true;

						var talking = new Talking(
							new Script(
								"이대원",
								"망치를 찾았습니다.",
								null,
								"find_hammer"
								)
							);

						processes.Add(
							new DialogProcess(
									app,
									stage.ScriptView,
									talking
								)
							);
					}
					break;
			}
		}
		private void SavePeople()
		{
			if (arrived.Used) return;
			if (arrived.NumObtained == 0) return;

			var player = stage.Player;
			player.SavePeople(arrived.NumObtained, arrived.NumPatients);


			List<Script> scripts = new List<Script>();
			
			scripts.Add(
				new Script(
					"이대원",
					"생존자가 있습니다!",
					null,
					"find_survivors#" + (app.Random.Next(2) + 1)
					)
				);

			if(arrived.NumPatients > 0)
			{
				scripts.Add(
					new Script(
						"이대원",
						"부상당했습니다",
						null,
						"hurt#" + (app.Random.Next(2) + 1)
						)
					);
			}

			if (player.NumMedicalKits > 0)
			{
				int numPatients = player.NumPatients;
				player.Cure(Math.Min(numPatients, player.NumMedicalKits));


				scripts.Add(
					new Script(
						"이대원",
						"부상자분들을 치료해드렸습니다.",
						null,
						"cure_patients#" + (app.Random.Next(3) + 1)
						)
					);
			}

			processes.Add(
				new DialogProcess(
					app,
					stage.ScriptView,
					new Talking(scripts)
					)
				);

            processes.Add(
                new RemoveSurvivorIconProcess (
                    stage,
                    arrived.Id
                    )
                );
		}

		private void AddScriptIfExists()
		{
			if (arrived.Scripts == null) return;
			else if (arrived.Used && arrived.Type.Equals("people")) return;
			else if (arrived.Used && arrived.Type.Equals("hidden_route")) return;

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
