using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;
using 행방불명.Game;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace 행방불명.Framework.UI
{
	public class MapView
		: View
	{
		Program app;
		GameStage stage;
		Player player;
		Mouse mouse;
		bool showMap, pressed;


		SolidColorBrush brush;
		Bitmap button, map, ping;
		Ellipse playerPos;
		TextFormat format;
		Matrix3x2 matrix;
		float scaleRate;


		public MapView(GameStage stage)
		{
			this.app = stage.App;
			this.stage = stage;
			this.mouse = app.Mouse;
			this.player = stage.Player;

			playerPos = new Ellipse(new Vector2(), 30, 30);
			brush = new SolidColorBrush(app.Graphics2D.RenderTarget, new Color4(1, 0, 0, 1));
			button = app.Media.BitmapDic["map_icon"];
			map = app.Media.BitmapDic["map"];
			ping = app.Media.BitmapDic["ping_danger"];
			format = app.Media.FormatDic["default"];
			Rect = new RectangleF(0, 400, button.Size.Width, button.Size.Height);

			Draw += DrawButton;
			Update += CheckClick;
		}



		private void CheckClick(float delta)
		{
			scaleRate += delta;
			if (scaleRate > 1)
				scaleRate = 0;

			if (!pressed && mouse[0])
				pressed = true;
			if (pressed && !mouse[0])
			{
				pressed = false;
				if(showMap)
					showMap = !showMap;
				else if (Rect.Contains(mouse.X, mouse.Y))
					showMap = !showMap;
			}
		}

		private void DrawButton()
		{
			app.Graphics2D.RenderTarget.DrawBitmap(
				button,
				Rect,
				1,
				BitmapInterpolationMode.Linear
				);

			if (showMap)
				DrawMap();
		}

		
		
		
		private void DrawMap()
		{
			var w = stage.GameView.MapWidth;
			var h = stage.GameView.MapHeight;
			var mw = map.PixelSize.Width;
			var mh = map.PixelSize.Height;
			var pos = player.CurrentPosition;


			var mx = (app.Width - mw) / 2;
			var my = (app.Height - mh) / 2;

			var px = pos.X / w * mw * 0.975f;
			var py = pos.Y / h * mh * 0.975f;

			app.Graphics2D.Draw(
				map,
				mx,
				my
				);

			playerPos.Point = new Vector2(mx + px, my + py);

			float scale = (float)Math.Pow(Math.Sin(scaleRate * 6.243f) / 7 + 1, 2);

			Matrix3x2.Scaling(scale, scale, ref playerPos.Point, out matrix);
			app.Graphics2D.RenderTarget.Transform = matrix;
			app.Graphics2D.DrawCenter(
				ping,
				mx + px,
				my + py
				);
			app.Graphics2D.RenderTarget.Transform = Matrix3x2.Identity;


			/*

			app.Graphics2D.RenderTarget.DrawEllipse(
				playerPos,
				brush
				);

			app.Graphics2D.RenderTarget.DrawText(
				String.Format(
					"{0}, {1}\n"+
					"{2}, {3}\n"+
					"{4}, {5}", 
					playerPos.Point.X, playerPos.Point.Y,
					pos.X, pos.Y,
					w, h
					),
				format,
				app.RectF,
				brush
				);*/
		}
	}
}
