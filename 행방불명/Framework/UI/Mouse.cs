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

		public Mouse(Program app)
		{
			this.app = app;
            IsUsed = false;
			var form = app.Form;

			if (app.Config.Fullscreen)
			{
				Screen screen = Screen.FromControl(form);
				adjX = (float)app.Width / screen.WorkingArea.Width;
				adjY = (float)app.Height / screen.WorkingArea.Height;
			}
			else
			{
				adjX = adjY = 1;
			}

			form.MouseDown += (object sender, MouseEventArgs args) =>
			{
				int buttons = ButtonToInt(args.Button);
				if(buttons == -1)
					return;

				_x = args.X;
				_y = args.Y;
				_pressed[buttons] = true;
                IsUsed = false;
				Adjust();
			};

			form.MouseUp += (object sender, MouseEventArgs args) =>
			{
				int buttons = ButtonToInt(args.Button);
				if(buttons == -1)
					return;

				_x = args.X;
                _y = args.Y;
                IsUsed = false;
				_pressed[buttons] = false;
				Adjust();
			};
			form.MouseMove  += (object sender, MouseEventArgs args) =>
			{
				_x = args.X;
				_y = args.Y;
				Adjust();
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

		Program app;
		bool []_pressed = new bool[3];
		bool _mouseIn = false;
		float _x, _y;
		float adjX, adjY;

		private void Adjust()
		{
			_x *= adjX;
			_y *= adjY;
		}

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

        public bool IsUsed { get; set; }
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
