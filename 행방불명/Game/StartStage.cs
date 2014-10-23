using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Game.Entities;
using SharpDX;

using SharpDX.DirectWrite;
using SharpDX.Direct2D1;


namespace 행방불명.Game
{
	public class StartStage : Stage
	{
		Program _app;
		Scene _scene;

		public StartStage(Program app)
		{
			_scene = new Scene();
			_app = app;

			mFont = app.Graphics2D.TextFormat("돋움", 32);
			mBrush = new SolidColorBrush(app.Graphics2D.RenderTarget, new Color4(0, 0, 0, 1));


			_scene.Start(new SoundEntity(app.Sound.Play2D(
		}

		~StartStage()
		{
			
		}


		float delta = 0;
		TextFormat mFont;
		SolidColorBrush mBrush;


		public void Draw()
		{
			var g2d = _app.Graphics2D;
			var rt = g2d.RenderTarget;

			rt.BeginDraw();
			rt.Clear(new Color4(1, 1, 1, 1));

			var text = String.Format(
				"DELTA: {0}",
				delta
				);
			rt.DrawText(text, mFont, new RectangleF(0, 0, 500, 500), mBrush);

			rt.EndDraw();
			_app.Graphics3D.Present();
		}


		public void Update(float delta)
		{
			this.delta = delta;
		}



		public Stage getNextStage()
		{
			return null;
		}

		public bool IsEnded()
		{
			return false;
		}
	}
}
