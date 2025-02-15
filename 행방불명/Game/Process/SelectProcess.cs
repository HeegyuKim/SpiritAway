﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework.UI;
using 행방불명.Game.Map;
using 행방불명.Game;
using System.Windows.Forms;

namespace 행방불명.Game.Process
{
	public class SelectProcess
		: IProcess
	{
		VoiceControl voice;
		GameStage stage;
		Script script;
		ScriptView scriptView;
		Player player;
		MapData map;
		List<Link> links;
		bool ended;

		public SelectProcess(
			GameStage stage,
			List<Link> links
			)
		{
			this.stage = stage;
			this.voice = stage.App.VoiceControl;
			this.scriptView = stage.ScriptView;
			this.player = stage.Player;
			this.map = stage.Map;
			this.links = links;
			this.script = null;
		}

		public SelectProcess(
			GameStage stage,
			Script script,
			List<Link> links
			)
			:  this(stage, links)
		{
			this.script = script;
		}


		public void Start()
		{
			ended = false;
			StringBuilder builder = new StringBuilder();
			int i = 0;

			foreach (var link in links)
			{
				if (!validateLink(link))
					continue;
					
				builder.Append(link.Name);

				++i;
				if (i > 0 && i % 2 == 0)
					builder.Append("\n\t");
				else if (i != links.Count)
					builder.Append(",\t\t");
			}

			if (script == null)
			{

				scriptView.Script = new Script(
					"이대원",
					"어디로 갈까요",
					builder.ToString()
					);
                if(stage.App.Random.Next(3) == 0)
				    stage.App.PlayRandom2D("ask", 3);
				voice.Recognize();
			}
			else
			{
				script.PlayerText = builder.ToString();
				scriptView.Script = script;
				stage.App.Play2D(script.Sfx);
				voice.Recognize();
			}
		}

		public void End()
		{
			scriptView.Script = null;
		}

		private bool validateLink(Link link)
		{
			String required = link.Required;
			if (required == null)
				return true;

			if (required.Equals("hidden_route"))
			{
				Waypoint waypoint = map.GetWaypoint("iweb_staff");
				if (waypoint != null && !waypoint.Used)
					return false;
			}
			return true;
		}
		private void SelectLinkAt(int index)
		{
			if (index >= links.Count) return;

			SelectLink(links[index]);

			if (voice.IsRecognizing)
				voice.Cancle();
		}

		public void Update(float delta)
		{
			var app = stage.App;
			
			if (voice.isSuccess)
			{
                if (map.IsB1() && voice.Text != null)
                {
                    switch(voice.Text)
                    {
                        case "여러분 치트키는 쓰지 맙시다":
                        {
                            var link = new Link();
                            link.Id = "keyroom";
                            SelectLink(link);
                            scriptView.Unknown = null;
                            voice.Text = "";
                            return;
                        }
                        case "집가서 라면 먹자":
                        {
                            var link = new Link();
                            link.Id = "lecture_room_door";
                            SelectLink(link);
                            scriptView.Unknown = null;
                            voice.Text = "";
                            return;
                        }
                    }
                }
				foreach (var link in links)
				{
					if (link.Name.Equals(voice.Text) && validateLink(link))
					{
						SelectLink(link);
                        scriptView.Unknown = null;
                        voice.Text = "";
						break;
					}
				}
				if (!ended)
                {
                    Console.WriteLine("ㄹㄹ");
                    scriptView.Unknown = voice.Text + " -?";
					voice.Recognize();
				}
			}
            else if (!voice.IsRecognizing)
            {
                Console.WriteLine("ㄹㄹ");
                scriptView.Unknown = voice.Text + " -?";
                voice.Recognize();
            }
		}

		private void SelectLink(Link link)
		{
			if (link.Required != null)
			{
				if (link.Required.Equals("key") && !player.HasKey)
				{
					var scripts = new List<Script>();
					scripts.Add(
						new Script(
							"이대원",
							"문이 잠겨있어서 지나갈 수 없습니다.",
							null,
							"locked"
							)
						);

					ended = true;
					var ps = stage.Processes;
					ps.Add(
						new DialogProcess(
							stage.App,
							stage.ScriptView,
							new Talking(scripts)
							)
						);
					ps.Add(this);

					var locked = stage.Map.GetWaypoint(link.Id);

					stage.GameObjects.Add(
						new GameObject(
                            locked.X,
                            locked.Y,
                            locked.Id,
							stage.App.Media.BitmapDic["locked"]
							)
						);
					Console.WriteLine("잠긴 문이 게임오브젝트에 추가됨!");
					return;
				}
				else
				{
					var removed = from obj in stage.GameObjects
								  where obj.key.Equals(link.Id)
								  select obj;
					if (removed.Count() > 0)
					{
						stage.GameObjects.Remove(removed.First());
						Console.WriteLine("잠긴 문이 게임오브젝트에서 제거됨");
					}
				}
				if (link.Required.Equals("hammer") && !player.HasHammer)
				{
					var scripts = new List<Script>();
					scripts.Add(
						new Script(
							"이대원",
							"길이 막혀있습니다.",
							null,
							"blocked"
							)
						);

					ended = true;
					var ps = stage.Processes;
					ps.Add(
						new DialogProcess(
							stage.App,
							stage.ScriptView,
							new Talking(scripts)
							)
						);
					ps.Add(this);

					var curr = player.MoveTo;

					stage.GameObjects.Add(
						new GameObject(
							curr.X,
							curr.Y,
							curr.Id,
							stage.App.Media.BitmapDic["blocked"]
							)
						);

					Console.WriteLine("막힌 길이 게임오브젝트에 추가됨!");
					return;
				}
				else if (link.Required.Equals("hammer") && player.HasHammer)
				{
					Waypoint waypoint = map.GetWaypoint(link.Id);

					var ps = stage.Processes;
					if (!waypoint.Used)
					{
						var scripts = new List<Script>();
						scripts.Add(
							new Script(
								"이대원",
								"길이 막혀있습니다.",
								"망치로 부숴",
								"blocked"
								)
							);
						scripts.Add(
							new Script(
								"",
								"쾅!",
								null,
								"crash"
								)
							);

						ended = true;
						ps.Add(
							new DialogProcess(
								stage.App,
								stage.ScriptView,
								new Talking(scripts)
								)
							);
						ps.Add(
							new StartProcess(
								stage,
								link.Id
								)
							);


						var removed = from obj in stage.GameObjects
									  where obj.key.Equals(player.MoveTo.Id)
									  select obj;
						if (removed.Count() > 0)
						{
							stage.GameObjects.Remove(removed.First());
							Console.WriteLine("막힌 길이 게임오브젝트에서 제거됨");
						}

						return;
					}
					
				}
			}

			Console.WriteLine("select link " + link.Id);
			player.StartTo(map.GetWaypoint(link.Id));
			ended = true;
            scriptView.Unknown = null;
		}


		public bool IsEnded()
		{
			return ended;
		}
	}
}
