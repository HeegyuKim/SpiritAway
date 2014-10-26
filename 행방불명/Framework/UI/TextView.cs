using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;



namespace 행방불명.Framework.UI
{
	public class TextView
		: View
	{
		Graphics2D _g2d;
		TextLayout _layout;
		TextFormat _format;
		Brush _brush;
		string _text;

		public TextView(Graphics2D g2d, TextFormat format, Brush brush)
			: base()
		{
			_g2d = g2d;
			_format = format;
			_brush = brush;

			Draw += DrawText;
		}

		~TextView()
		{
			SharpDX.Utilities.Dispose(ref _layout);
		}

		private void DrawText()
		{
			if (_layout == null) return;

			_g2d.RenderTarget.DrawTextLayout(
				new SharpDX.Vector2(X, Y),
				_layout,
				_brush
				);
		}
		public TextAlignment TextAlignment
		{
			get
			{
				return _layout.TextAlignment;
			}
			set
			{
				_layout.TextAlignment = value;
			}
		}
		public TextFormat Format
		{
			get
			{
				return _format;
			}
			set
			{
				_format = value;
			}
		}
		public TextLayout Layout
		{
			get
			{
				return _layout;
			}
			set
			{
				_layout = value;
			}
		}
		public Brush Brush
		{
			get
			{
				return _brush;
			}
			set
			{
				_brush = value;
			}
		}
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				_layout = new TextLayout(_g2d.DWriteFactory, value, _format, Width, Height);
			}
		}
	}
}
