using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework.UI;
using 행방불명.Game.Map;
using 행방불명.Game;


namespace 행방불명.Game.Process
{
	public class SelectProcess
		: IProcess
	{
		VoiceControl voice;
		ScriptView scriptView;
		Player player;
		MapData map;
		List<Link> links;


		public SelectProcess(
			VoiceControl voice, 
			ScriptView scriptView,
			Player player, 
			MapData map,
			List<Link> links
			)
		{
			this.voice = voice;
			this.scriptView = scriptView;
			this.player = player;
			this.map = map;
			this.links = links;
		}


		public void Start()
		{
			StringBuilder builder = new StringBuilder();
			int i = 0;

			foreach (var link in links)
			{
				builder.Append(link.Name);

				++i;
				if (i != links.Count)
					builder.Append(" 혹은 ");
			}


			scriptView.Script = new Script(
				"이대원",
				"어디로 갈까요",
				builder.ToString()
				);
			voice.Recognize();
		}

		public void End()
		{
			scriptView.Script = null;
		}


		bool ended = false;

		public void Update(float delta)
		{
			if (voice.isSuccess)
			{
				foreach (var link in links)
				{
					if (link.Name.Equals(voice.Text))
					{
						player.StartTo(map.GetWaypoint(link.Id));
						ended = true;
					}
				}
			}
			else if (!voice.IsRecognizing)
				voice.Recognize();
		}


		public bool IsEnded()
		{
			return ended;
		}
	}
}
