using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;


namespace 행방불명.Game.Process
{
	public class DynamicSelectionProcess : IProcess
	{
		public delegate IProcess OnSelect(string value);

		GameStage stage;
		OnSelect select;
		bool ended;
		string[] args;
		Script script;


		public DynamicSelectionProcess (
			GameStage stage,
			OnSelect select,
			Script script,
			params string []args
			)
		{
			this.stage = stage;
			this.select = select;
			this.script = script;
			this.args = args;
		}

		public void Start()
		{
			ended = false;
			StringBuilder builder = new StringBuilder();
			int i = 0;

			foreach (var arg in args)
			{
				builder.Append(arg);

				++i;
				if (i > 0 && i % 2 == 0)
					builder.Append("\n\t");
				else if (i != args.Length)
					builder.Append(",\t\t");
			}
			script.PlayerText = builder.ToString();

			stage.ScriptView.Script = script;

			stage.App.Play2D(script.Sfx);
		}


		public void Update(float delta)
		{
			var app = stage.App;
			var voice = app.VoiceControl;

			if (voice.isSuccess)
			{
				foreach (var arg in args)
				{
					if (arg.Equals(voice.Text))
					{
						stage.Processes.Add(
							select(arg)
							);
						ended = true;
						break;
					}
				}
				if (!ended)
				{
					voice.Recognize();
				}
			}
			else if (!voice.IsRecognizing)
				voice.Recognize();
		}



		public void End()
		{
			stage.ScriptView.Script = null;
		}

		public bool IsEnded()
		{
			return ended;
		}
	}
}
