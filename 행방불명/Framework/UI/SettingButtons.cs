using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using 행방불명.Game;


namespace 행방불명.Framework.UI
{
	public class SettingButtons
		: View
	{
		Program app;
		Mouse mouse;
		Config config;
		
		RectangleF exitRect, soundRect;
		Bitmap exit, soundOn, soundOff;


		public SettingButtons(Program app)
		{
			this.app = app;
			this.mouse = app.Mouse;
			this.config = app.Config;


			exit = app.Media.BitmapDic["exit"];
			soundOn = app.Media.BitmapDic["sound_on"];
			soundOff = app.Media.BitmapDic["sound_off"];


			exitRect = new SharpDX.RectangleF(
				0, 0,
				exit.Size.Width, exit.Size.Height
				);
			soundRect = new SharpDX.RectangleF(
				exit.Size.Width, 0,
				soundOff.Size.Width, soundOff.Size.Height
				);
			Rect = new RectangleF(
				0, 0,
				exit.Size.Width + soundOff.Size.Width, exit.Size.Height
				);

			Draw += DrawButtons;
			Update += CheckMouseInput;
		}


		private void DrawButtons()
		{
			app.Graphics2D.Draw(exit, 0, 0);
			if (config.SoundEnabled)
			{
				app.Graphics2D.Draw(soundOn, soundRect.Left, 0);
			}
			else
			{
				app.Graphics2D.Draw(soundOff, soundRect.Left, 0);
			}
		}

		bool pressed = false, questioned = false;

		private void CheckMouseInput(float delta)
		{
			if (!Rect.Contains(mouse.X, mouse.Y))
				return;

			if (pressed && !mouse[0])
			{
				if (exitRect.Contains(mouse.X, mouse.Y) && !questioned)
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
					questioned = false;
				}
				else
				{
					config.SoundEnabled = !config.SoundEnabled;
					if (config.SoundEnabled)
						app.Sound.SoundVolume = 1;
					else
						app.Sound.SoundVolume = 0;
				}
				pressed = false;

			}
			else if(mouse[0])
			{
				pressed = true;
			}
		}
	}
}
