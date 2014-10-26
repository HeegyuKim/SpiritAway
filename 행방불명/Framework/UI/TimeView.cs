using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;


namespace 행방불명.Framework.UI
{
	public class TimeView
		: View
	{
		Program app;
		Bitmap clock;
		SolidColorBrush brush;
		TextFormat format;
		TextLayout layout;
		Vector2 origin;


		public bool Enabled { get; set; }
		public float Time { get; set; }

		public TimeView(Program app)
		{
			this.app = app;
			Time = 0;
			Enabled = true;
			clock = app.Media.BitmapDic["time_ui"];
			format = app.Media.FormatDic["default32"];
			brush = new SolidColorBrush(app.Graphics2D.RenderTarget, new Color4(1, 0, 0, 1));
			origin = new Vector2((app.Width - clock.Size.Width) / 2.0f, -14);

			Draw += DrawTime;
			Update += UpdateTime;
		}

		private void UpdateTime(float delta)
		{
			if (Enabled)
			{
				Time += delta;

				int time = (int)(Time);
				int minutes = time / 60;
				int seconds = time % 60;

				Utilities.Dispose(ref layout);

				StringBuilder text = new StringBuilder();
				if (minutes < 10)
					text.Append('0');
				text.Append(minutes);

				text.Append(" : ");

				if (seconds < 10)
					text.Append('0');
				text.Append(seconds);

				layout = new TextLayout(
					app.Graphics2D.DWriteFactory,
					text.ToString(),
					format,
					clock.Size.Width,
					clock.Size.Height
					);
				layout.TextAlignment = TextAlignment.Center;
				layout.ParagraphAlignment = ParagraphAlignment.Center;
			}
		}

		
		private void DrawTime()
		{
			var g2d = app.Graphics2D;
			g2d.Draw(clock, origin.X, origin.Y);
			g2d.RenderTarget.DrawTextLayout(origin, layout, brush);
		}
	}
}
