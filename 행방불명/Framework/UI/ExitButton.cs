using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;


namespace 행방불명.Framework.UI
{
	public class ExitButton
		: View
	{
		Program app;
		Mouse mouse;
		SolidColorBrush brush;

		public ExitButton(Program app)
		{
			this.app = app;
			this.mouse = app.Mouse;

			Rect = new SharpDX.RectangleF(
				0, 0,
				50, 50
				);
			brush = new SolidColorBrush(
				app.Graphics2D.RenderTarget, 
				new Color4(1, 1, 1, 1)
				);

			Draw += DrawExitButton;
			Update += CheckMouseInput;
		}

		~ExitButton()
		{
			Utilities.Dispose(ref brush);
		}

		private void DrawExitButton()
		{
			var rt = app.Graphics2D.RenderTarget;

			rt.DrawRectangle(Rect, brush, 5);
		}

		bool pressed = false, questioned = false;

		private void CheckMouseInput(float delta)
		{
			if (!Rect.Contains(mouse.X, mouse.Y))
				return;

			if (questioned) return;

			if (pressed && !mouse[0])
			{
				questioned = true;

				// EXIT 버튼 눌림
				var result = MessageBox.Show(
					app.Form,
					"정말로 종료하실건가요?", 
					"종료", 
					MessageBoxButtons.YesNo
					);
				if (result == DialogResult.Yes)
				{
					app.Dispose();
				}
				pressed = false;
				questioned = false;
			}
			else if(mouse[0])
			{
				pressed = true;
			}
		}
	}
}
