using System;
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
		}


		public void Start()
		{
			ended = false;
			StringBuilder builder = new StringBuilder();
			int i = 0;

			foreach (var link in links)
			{
				builder.Append(link.Name);

				++i;
				if (i > 0 && i % 2 == 0)
					builder.Append("\n\t");
				else if (i != links.Count)
					builder.Append(",\t\t");
			}


			scriptView.Script = new Script(
				"이대원",
				"어디로 갈까요",
				builder.ToString()
				);
			stage.App.Play2D("ask");
			voice.Recognize();
		}

		public void End()
		{
			scriptView.Script = null;
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
				foreach (var link in links)
				{
					if (link.Name.Equals(voice.Text))
					{
						SelectLink(link);
						break;
					}
				}
				if (!ended)
					voice.Recognize();
			}
			else if (!voice.IsRecognizing)
				voice.Recognize();
		}

		private void SelectLink(Link link)
		{
			if (link.Required != null)
			{
				if (link.Required.Equals("key") && !player.HasKey)
				{
					Console.WriteLine(link.Name + " 라고 하셨지만 열쇠가 없군요.");

					var scripts = new List<Script>();
					scripts.Add(
						new Script(
							"이대원",
							"문이 잠겨있어서 지나갈 수 없습니다.",
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

					return;
				}
				if (link.Required.Equals("hammer") && !player.HasHammer)
				{
					Console.WriteLine(link.Name + " 라고 하셨지만 망치가 없군요.");
					var scripts = new List<Script>();
					scripts.Add(
						new Script(
							"이대원",
							"길이 막혀있어서 지나갈 수 없습니다.",
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

					return;
				}
				else if (link.Required.Equals("hammer") && player.HasHammer)
				{
					Console.WriteLine(link.Name + " 망치로 꽝!");
					Waypoint waypoint = map.GetWaypoint(link.Id);

					var ps = stage.Processes;
					if (waypoint.Used)
					{
						var scripts = new List<Script>();
						scripts.Add(
							new Script(
								"이대원",
								"망치로 문을 부수겠습니다.",
								null
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
					}

					ps.Add (
						new StartProcess(
							stage,
							link.Id
							)
						);
				}
			}

			player.StartTo(map.GetWaypoint(link.Id));
			ended = true;
		}


		public bool IsEnded()
		{
			return ended;
		}
	}
}
