﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

using 행방불명.Game.Map;



namespace 행방불명.Framework.UI
{
	public class ScriptView
		: View
	{
		Program app;

		TextFormat format;
		TextLayout targetText, playerText, unknownText;
        bool hasPlayerText;
		Script script;

		public Script Script 
		{ 
			get
			{
				return script;
			}
			set
			{
				if (value == null)
				{
					Utilities.Dispose(ref targetText);
					Utilities.Dispose(ref playerText);
					return;
				}
				string targetTextString = 
					value.TargetName + "\n" + value.TargetText;

				Utilities.Dispose(ref targetText);

				targetText = new TextLayout(
					app.Graphics2D.DWriteFactory,
					targetTextString,
					format,
					Width - 75,
					Height
					);

				if (value.TargetName != null)
				{
					var range = new TextRange(0, value.TargetName.Length);
					targetText.SetFontSize(format.FontSize * 1.3f, range);
					targetText.SetFontWeight(FontWeight.Bold, range);
				}

				Utilities.Dispose(ref playerText);
				if (value.PlayerText != null && value.PlayerText.Length != 0)
				{
					playerText = new TextLayout(
						app.Graphics2D.DWriteFactory,
						"말하세요> " + value.PlayerText,
						format,
						Width - 175,
						100
						);
					playerText.SetFontSize(
						format.FontSize * 0.8f,
						new TextRange(0, 6)
						);
                    hasPlayerText = true;
				}
                else
                {
                    hasPlayerText = false;
                    playerText = new TextLayout(
                        app.Graphics2D.DWriteFactory,
                        "클릭 혹은 스페이스바를 누르세요",
                        format,
                        Width - 175,
                        100
                        );
                    Unknown = null;
                }
				script = value;
			}
		}

        float unknownChangedDelta = 0;

        public String Unknown
        {
            set
            {
                if(unknownText != null)
                {
                    Utilities.Dispose(ref unknownText);
                }

                if (value == null || value.Length <= 0 || !hasPlayerText)
                {
                    unknownChangedDelta = 0;
                    return;
                }

                unknownText = new TextLayout(
                    app.Graphics2D.DWriteFactory,
                    value,
                    format,
                    Width,
                    50
                    );
                unknownText.TextAlignment = TextAlignment.Trailing;
                unknownChangedDelta = 0;
            }
        }
		public void Clear()
		{
			script.TargetName = 
				script.TargetText = 
				script.PlayerText = "";
		}

		SolidColorBrush brush;
		Color4 textColor;
		Bitmap bg, playerThumbnail;


		public ScriptView(Program app)
		{
			this.app = app;
			

			Rect = new RectangleF(
				150, app.Height - 200,
				750, 200
				);


			var media = app.Media;
			format = media.FormatDic["default18"];

			bg = media.BitmapDic["dialog_bg"];
			playerThumbnail = media.BitmapDic["character_thumbnail"];

			textColor = new Color4(0, 0, 0, 1);

			brush = new SolidColorBrush(
				app.Graphics2D.RenderTarget, 
				textColor
				);
			Draw += DrawText;
            Update += UpdateUnknown;
		}

        private void UpdateUnknown(float dt)
        {
            if(unknownText != null)
            {
                unknownChangedDelta += dt;
                if (unknownChangedDelta > 2)
                    Unknown = "";
            }
        }

		private void DrawText()
		{
			var g2d = app.Graphics2D;
			var rt = app.Graphics2D.RenderTarget;

			float bgY = app.Height - 230;
			g2d.Draw(bg, 150, app.Height - bg.Size.Height);
			g2d.Draw(playerThumbnail, 0, app.Height - playerThumbnail.Size.Height);

			brush.Color = textColor;
			var pos = new Vector2(
				playerThumbnail.Size.Width * 0.7f, 
				bgY + 30);

			if(targetText != null)
				rt.DrawTextLayout(pos, targetText, brush);

			pos.X += 100;
			pos.Y += 90;
			if (playerText != null)
				rt.DrawTextLayout(pos, playerText, brush);

            if (unknownText != null)
            {
                pos.X = 0;
                pos.Y = app.Height - unknownText.MaxHeight;
                rt.DrawTextLayout(pos, unknownText, brush);
            }
		}
	}
}
