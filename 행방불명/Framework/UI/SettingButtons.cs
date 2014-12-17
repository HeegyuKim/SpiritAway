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

		float margin = 30, lastChangedDelta = 0, queryDelta = 0;
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
				exit.Size.Width + margin, 0,
				soundOff.Size.Width, soundOff.Size.Height
				);
			Rect = new RectangleF(
				0, 0,
				margin + exit.Size.Width + soundOff.Size.Width, exit.Size.Height
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

		private void QueryQuit()
		{
			if (questioned || queryDelta < 0.5f)
				return;

			if (app.Config.Fullscreen)
				app.Form.Close();
			else
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
					app.Form.Close();
				}

				app.KeyF1 = false;
				queryDelta = 0;
				questioned = false;
			}
		}

		private void OnOffSound()
		{
			lastChangedDelta = 0;
			config.SoundEnabled = !config.SoundEnabled;
			if (config.SoundEnabled)
				app.Sound.SoundVolume = 1;
			else
				app.Sound.SoundVolume = 0;
		}

		private void CheckMouseInput(float delta)
		{
			lastChangedDelta += delta;
			queryDelta += delta;

			if (app.KeyF1)
			{
				QueryQuit();
			}
			if (app.KeyF2 && lastChangedDelta > 0.15f)
			{
				OnOffSound();
			}

            if (!Rect.Contains(mouse.X, mouse.Y))
                return;
            if (mouse.IsUsed) return;

			bool clicked = pressed && !mouse[0];
			if (mouse[0])
			{
                pressed = true;
                mouse.IsUsed = true;
			}
			else
				pressed = false;


			
			if (clicked && exitRect.Contains(mouse.X, mouse.Y))
			{
				QueryQuit();
			}
			
			if(clicked && soundRect.Contains(mouse.X, mouse.Y))
			{
				OnOffSound();
			}

		}
	}
}
