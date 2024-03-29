﻿using System;
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
using IrrKlang;



namespace 행방불명.Game
{
	public class PrologStage
		: Stage
	{
		Program app;
		Script script1,
				script2;
        ISound sfx1, sfx2;
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
			btnExit = new SettingButtons(app);
			scriptView = new ScriptView(app);
			
			script1 = new Script(
				"장관",
				"김대장, 코엑스에서 테러가 일어나 미처 대피하지 못한 사람들과 우리 대원들이 갇혀 있네,",
                null,
				"minister1"
				);
			script2 = new Script(
				"장관",
				"대원들과 연락하여 사람들을 구출해 내게나, 외부 출구가 무너져서 지하내부의 상황을 전혀 알 수가 없어.",
				"옙",
                "minister2"
				);
			scriptView.Script = script1;

			app.VoiceControl.Load(new string[][]{
				new string[]{"옙"}
			});


            escToSkipView = new ImageView(app.Graphics2D, app.Media.BitmapDic["esc_to_skip"]);
            escToSkipView.X = app.Width - escToSkipView.Width - 30;
            escToSkipView.Y = 30;
            escToSkipView.Draw += escToSkipView.DrawBitmap;

            container.Views.Add(scriptView);
            container.Views.Add(btnExit);
            container.Views.Add(escToSkipView);
		}



		Color4 bg = new Color4(0, 0, 0, 1);
		Bitmap minister;
		TextFormat defaultFormat;

		//
		//
		// GUI ZONE~~~
		Container container;
		SettingButtons btnExit;
		ScriptView scriptView;
        ImageView escToSkipView;






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

            if(sfx1 == null)
            {
                sfx1 = app.Play2D(script1.Sfx);
            }

			if (app.KeyESC)
			{
                yep = true;
                if(sfx1 != null) sfx1.Stop();
                if(sfx2 != null) sfx2.Stop();
				return;
			}
			var mouse = app.Mouse;
			var voice = app.VoiceControl;

            if (!isScript2 && (app.KeySpace || (mouse[0] && scriptView.Rect.Contains(mouse.X, mouse.Y))))
			{
				pressed = true;
				scriptView.Script = script2;
				isScript2 = true;
                sfx2 = app.Play2D(script2.Sfx);
                sfx1.Stop();

				return;
			}

			if (yep) return;

			if (voice.isSuccess && voice.Text.Equals("옙"))
			{
				yep = true;
                voice.Text = "";
                sfx2.Stop();
				Console.Write("다음 스테이지로 ㄱㄱ");
			}
			else if (pressed && !mouse[0] && isScript2)
			{
				voice.Recognize();
			}
		}




		public Stage getNextStage()
		{
			return new GameStage(app, "res/tutorial.json", new GameStageOptions()
				{
					HasCountUI = false,
					HasMapUI = false,
					HasTimeUI = false,
					Player = null,
				});
		}

		public bool IsEnded()
		{
			return yep;
		}
	}
}
