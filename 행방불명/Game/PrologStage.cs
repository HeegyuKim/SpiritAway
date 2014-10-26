using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Framework.UI;
using 행방불명.Game.Map;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;


namespace 행방불명.Game
{
	public class PrologStage
		: Stage
	{
		Program app;
		Script script1,
				script2;
		bool isScript2 = false;


		public PrologStage(Program app)
		{
			Console.WriteLine("Prolog Stage start!");

			this.app = app;
		}

		public void Start()
		{
			app.VoiceControl.Reset();

			container = new Container(app);

			var media = app.Media;
			defaultFormat = media.FormatDic["default"];
			minister = media.BitmapDic["minister"];

			InitUI();

		}

		private void InitUI()
		{
			btnExit = new ExitButton(app);
			scriptView = new ScriptView(app);
			
			script1 = new Script(
				"장관",
				"김대장, 코엑스에서 테러가 일어나 미처 대피하지 못한 사람들과 우리 대원들이 갇혀 있네,",
				""
				);
			script2 = new Script(
				"장관",
				"대원들과 연락하여 사람들을 구출해 내게나, 외부 출구가 무너져서 지하내부의 상황을 전혀 알 수가 없어.",
				"옙"
				);
			scriptView.Script = script1;

			app.VoiceControl.Load(new string[][]{
				new string[]{"옙"}
			});

			container.Views.Add(btnExit);
			container.Views.Add(scriptView);
		}



		Color4 bg = new Color4(0, 0, 0, 1);
		Bitmap minister;
		TextFormat defaultFormat;

		//
		//
		// GUI ZONE~~~
		Container container;
		ExitButton btnExit;
		ScriptView scriptView;






		public void Draw()
		{
			var g2d = app.Graphics2D;
			var rt = g2d.RenderTarget;

			rt.BeginDraw();
			rt.Clear(bg);
			g2d.Draw(
				minister,
				(app.Width - minister.Size.Width) / 2,
				(app.Height - minister.Size.Height) / 2
				);

			container.Draw();
			
			rt.EndDraw();

			app.Graphics3D.Present();
		}


		bool pressed = false, yep = false;
		public void Update(float delta)
		{
			container.Update(delta);

			var mouse = app.Mouse;
			var voice = app.VoiceControl;

			if (mouse[0])
			{
				pressed = true;
				scriptView.Script = script2;
				isScript2 = true;
				return;
			}

			if (yep) return;

			if (voice.isSuccess && voice.Text.Equals("옙"))
			{
				yep = true;
				Console.Write("다음 스테이지로 ㄱㄱ");
			}
			else if (pressed && !mouse[0] && isScript2)
			{
				voice.Recognize();
			}
		}




		public Stage getNextStage()
		{
			return new GameStage(app, "res/tutorial.data", new StartStage(app));
		}

		public bool IsEnded()
		{
			return yep;
		}
	}
}
