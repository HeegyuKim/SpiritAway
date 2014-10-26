using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;


namespace 행방불명.Framework.UI
{
	class ImageButton
		: ImageView
	{
		Bitmap _hover, _button, _press;
		Mouse mouse;

		public Bitmap HoverBitmap 
		{
			get 
			{
				return _hover;
			}
			set
			{
				_hover = value;
			}
		}

		public Bitmap ButtonBitmap 
		{ 
			get
			{
				return _button;
			}
			set
			{
				_button = base.Bitmap = value;
			}
		}
		
		public Bitmap PressBitmap 
		{
			get
			{
				return _press;
			}
			set
			{
				_press = value;
			}
		}


		public ImageButton(Graphics2D g2d, Mouse mouse)
			: base(g2d)
		{
			this.mouse = mouse;

			Update += UpdateButton;
			Update += CheckMouseInput;
		}


		
		private void UpdateButton(float delta)
		{
			if (!Rect.Contains(mouse.X, mouse.Y))
			{
				Bitmap = _button;
				return;
			}

			if (mouse[0])
				Bitmap = _press;
			else
				Bitmap = _hover;
		}


		bool pressed = false;

		public delegate void ClickHandler(float x, float y);
		public event ClickHandler Click;

		private void CheckMouseInput(float delta)
		{
			if (!Rect.Contains(mouse.X, mouse.Y))
				return;
			

			if (pressed && !mouse[0])
			{
				Click(mouse.X - X, mouse.Y - Y);
			}
			else if (mouse[0])
			{
				pressed = true;
			}
		}

		
	}
}
