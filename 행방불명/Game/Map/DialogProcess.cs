using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Process;
using 행방불명.Framework;
using 행방불명.Framework.UI;

namespace 행방불명.Game.Map
{
	public class DialogProcess
		: IProcess
	{
		Program app;
		ScriptView scriptView;
		Talking talking;
		VoiceControl voice;
		Mouse mouse;


		public DialogProcess(
			Program app,
			ScriptView scriptView,
			Talking talking
			)
		{
			this.app = app;
			this.scriptView = scriptView;
			this.talking = talking;
			this.mouse = app.Mouse;
			this.voice = app.VoiceControl;
		}

		bool pressed = false, ended = false;

		public void Start()
		{
			Console.WriteLine("DialogProcess started.");
			scriptView.Script = talking.Current;
		}

		public void Update(float delta)
		{
			if (mouse[0] && !pressed)
			{
				pressed = true;
			}
			else if(!mouse[0] && pressed)
			{
				pressed = false;

				// 음성인식 사용이 아니면 걍 클릭했을 때 넘어간다
				if (!talking.HasVoice)
					NextScript();
			}

			// 음성인식 사용해야 하는 부분일 경우에..
			if (voice.isSuccess && voice.Text.Equals(talking.Current.PlayerText))
				NextScript();

			// 대화가 필요한데 인식중이지 않으면 인식해야지
			else if (talking.HasVoice && !voice.IsRecognizing)
				voice.Recognize();
		}

		public void End()
		{
			scriptView.Script = null;
		}

		private void NextScript()
		{
			if (talking.HasNext)
			{
				talking.Next();
				scriptView.Script = talking.Current;
			}
			else
			{
				ended = true;
			}
		}

		public bool IsEnded()
		{
			return ended;
		}
	}
}
