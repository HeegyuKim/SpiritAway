using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;
using 행방불명.Framework;
using 행방불명.Framework.UI;
using IrrKlang;


namespace 행방불명.Game.Process
{
	public class DialogProcess
		: IProcess
	{
		Program app;
		ScriptView scriptView;
		Talking talking;
		VoiceControl voice;
		Mouse mouse;
		ISound currSound;
		float nextScriptDelta = 0;


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
			scriptView.Script = talking.Current;
			currSound =  app.Play2D(talking.Current.Sfx);

			Console.WriteLine("<Script> " + talking.Current.Sfx);
			Console.WriteLine(talking.Current.TargetText);

		}

		public void Update(float delta)
		{
			nextScriptDelta += delta;


			if (app.KeySpace && nextScriptDelta > 0.3f)
			{
				nextScriptDelta = 0;

				// 음성인식아니면 걍 넘어가고
				if (!talking.HasVoice)
				{
					NextScript();
				}
				// 맞으면 취소하고 다음스크립트로
				else
				{
					voice.Cancle();
					NextScript();
				}
				return;
			}


			if (mouse[0] && !pressed)
			{
				pressed = true;
			}
			else if(!mouse[0] && pressed)
			{
				pressed = false;

				// 음성인식아니면 걍 넘어가고
				if (!talking.HasVoice)
				{
					NextScript();
				}
				// 맞으면 취소하고 다음스크립트로
				else
				{
					voice.Cancle();
					NextScript();
				}
				return;
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
			if (currSound != null)
				currSound.Stop();

			if (talking.HasNext)
			{
				talking.Next();
				scriptView.Script = talking.Current;
				currSound = app.Play2D(talking.Current.Sfx);
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
