using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace 행방불명.Framework.UI
{
	public class Mouse
	{

		public Mouse(Form form)
		{
			form.MouseDown += (object sender, MouseEventArgs args) =>
			{
				int buttons = ButtonToInt(args.Button);
				if(buttons == -1)
					return;

				_x = args.X;
				_y = args.Y;
				_pressed[buttons] = true;
			};

			form.MouseUp += (object sender, MouseEventArgs args) =>
			{
				int buttons = ButtonToInt(args.Button);
				if(buttons == -1)
					return;

				_x = args.X;
				_y = args.Y;
				_pressed[buttons] = false;
			};
			form.MouseMove  += (object sender, MouseEventArgs args) =>
			{
				_x = args.X;
				_y = args.Y;
			};
			form.MouseEnter += (object sender, EventArgs args) =>
			{
				_mouseIn = true;
			};
			form.MouseLeave += (object sender, EventArgs args) =>
			{
				_mouseIn = false;
			};

		}

		private int ButtonToInt(MouseButtons buttons)
		{
			switch (buttons)
			{
				case MouseButtons.Left:
					return 0;
				case MouseButtons.Middle:
					return 1;
				case MouseButtons.Right:
					return 2;
				default:
					return -1;
			}
		}

		bool []_pressed = new bool[3];
		bool _mouseIn = false;
		float _x, _y;



		public bool this[int button]
		{
			get
			{
				return _pressed[button];
			}
			set
			{
				_pressed[button] = value;
			}
		}

		public bool IsInside
		{
			get
			{
				return _mouseIn;
			}
		}

		public float X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public float Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}
	}
}
