using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Framework.UI;
using SharpDX;

using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using IrrKlang;


namespace 행방불명.Game
{
	public class StartStage : Stage
	{
		Program _app;
		Container container;
		VoiceControl voice;
		ISound bgm;
		Bitmap bg;

		public StartStage(Program app)
		{
			_app = app;
			container = new Container(app);
		}


		public void Start()
		{

			mFont = _app.Media.FormatDic["default32"];
			mFontSmall = _app.Media.FormatDic["default"];
			mBrush = new SolidColorBrush(_app.Graphics2D.RenderTarget, new Color4(1, 1, 1, 1));

			InitUI();

			voice = _app.VoiceControl;
			voice.Load(new string[][]{
				new string[]{ "시작" }
			});

			bg = _app.Media.BitmapDic["start_bg"];
			bgm = _app.Sound.Play2D("start_stage_bgm", true, true);
			if (bgm != null)
				bgm.Paused = false;

		}

		private void InitUI()
		{
			var app = _app;
			var cx = app.Width / 2;
			var cy = app.Height / 2;





			/////////////////////////////////
			/////////////////////////////////
			int rankingCount = app.Config.RankList.Count;

			var grayBrush = new SolidColorBrush(
				_app.Graphics2D.RenderTarget, 
				new Color4(0.9f, 0.9f, 0.9f, 1)
				);

			var rankView = new TextView(_app.Graphics2D, mFontSmall, grayBrush);
			var timeView = new TextView(_app.Graphics2D, mFontSmall, grayBrush);
			var peopleView = new TextView(_app.Graphics2D, mFontSmall, grayBrush);
			var levelView = new TextView(_app.Graphics2D, mFontSmall, grayBrush);

			float x = cx / 2.0f;
			float y = cy * 0.8f;
			float dx = cx / 4.0f;
			float dy = cy;
			float w = dx;
			float hx = _app.Width / 2;

			rankView.Rect = new RectangleF(hx - 250, y, w, dy); 
			timeView.Rect = new RectangleF(hx - 140, y, w, dy);
			peopleView.Rect = new RectangleF(hx - 30, y, w, dy);
			levelView.Rect = new RectangleF(hx + 80, y, w, dy);
						
			for (int i = 0; i < rankingCount; ++i)
			{
				var rank = app.Config.RankList[i];
				rankView.Text += "\n\n" + (i + 1) + "위";
				timeView.Text += "\n\n" + rank.Time.ToString("###.0") + "분";
				peopleView.Text += "\n\n" + rank.NumObtainedPeople + "명";
				levelView.Text += "\n\n" + rank.Level;
			}

			rankView.TextAlignment = TextAlignment.Center;
			timeView.TextAlignment = TextAlignment.Center;
			peopleView.TextAlignment = TextAlignment.Center;
			levelView.TextAlignment = TextAlignment.Center;
			

			container.Views.Add(rankView);
			container.Views.Add(timeView);
			container.Views.Add(peopleView);
			container.Views.Add(levelView);
		}


		~StartStage()
		{
		}


		TextFormat mFont, mFontSmall;
		SolidColorBrush mBrush;

		bool fadingIn = false;
		float fadingInRate = 0.5f;
		float opacityRate = 1.0f;
		float fadingInDelta = 0;
		Color4 curtain = new Color4(0, 0, 0, 0.5f);

		public void Draw()
		{
			var g2d = _app.Graphics2D;
			var rt = g2d.RenderTarget;

			rt.BeginDraw();
			rt.DrawBitmap(bg, _app.RectF, 1, BitmapInterpolationMode.Linear);
			container.Draw();

			rt.EndDraw();
			_app.Graphics3D.Present();
		}


		public void Update(float delta)
		{
			if (fadingIn)
			{
				if (fadingInDelta > 1)
					return;

				fadingInDelta += delta;
				fadingInRate -= delta / 2;
				opacityRate -= delta;
				mBrush.Opacity = opacityRate;
				return;
			}

			container.Update(delta);

			if(voice.isSuccess && voice.Text.Equals("시작"))
			{
				Console.WriteLine("시자악~");
				fadingIn = true;
			}
			else if (!voice.IsRecognizing)
			{
				voice.Recognize();
			}
		}



		public Stage getNextStage()
		{
			return new PrologStage(_app);
		}

		public bool IsEnded()
		{
			return fadingInDelta > 1;
		}
	}
}
