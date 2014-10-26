using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;


namespace 행방불명.Framework.UI
{
	public class GameView
		: View
	{
		Program app;
		Vector2 viewport;
		Bitmap bg;
		RectangleF dest, src;

		
		public Vector2 Center
		{
			get
			{
				return new Vector2(
					viewport.X + app.Width / 2,
					viewport.Y + app.Height / 2
					);
			}
			set
			{
				viewport.X = value.X - app.Width / 2;
				viewport.Y = value.Y - app.Height / 2;
			}
		}


		public Vector2 Viewport
		{
			get
			{
				return viewport;
			}
			set
			{
				viewport = value;
			}
		}


		public GameView(Program app)
		{
			this.app = app;

			viewport = new Vector2(0, 0);
			dest = new RectangleF();
			src = new RectangleF();

			bg = app.Media.BitmapDic["game_map"];

			Draw += DrawGame;
		}

		
		private void DrawGame()
		{
			var rt = app.Graphics2D.RenderTarget;
			
			var vw = app.Width;
			var vh = app.Height;
			var bw = bg.Size.Width;
			var bh = bg.Size.Height;

			var x = viewport.X;
			var y = viewport.Y;
			var dx = bw - x;
			var dy = bh - y;


			if (x < 0) // 화면의 왼쪽 부분이 남음
			{
				dest.X = -x;
				dest.Width = vw + x;
				src.X = 0;
				src.Width = dest.Width;
			}
			else if (dx < vw)	// 화면의 오른쪽 부분이 남음
			{
				dest.X = 0;
				dest.Width = dx;
				src.X = x;
				src.Width = dx;
			}
			else // 화면을 가득 채우더라
			{
				dest.X = 0;
				dest.Width = vw;
				src.X = x;
				src.Width = vw;
			}

			if (y < 0) // 화면의 위가 남음
			{
				dest.Y = -y;
				dest.Height = vh + y;
				src.Y = 0;
				src.Height = dest.Height;
			}
			else if (dy < vh) // 화면의 아래가 남음
			{
				dest.Y = 0;
				dest.Height = dy;
				src.Y = y;
				src.Height = dy;
			}
			else
			{
				dest.Y = 0;
				dest.Height = vh;
				src.Y = y;
				src.Height = vh;
			}



			rt.DrawBitmap(bg, dest, 1, BitmapInterpolationMode.Linear, src);
		}

	}
}
