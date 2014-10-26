using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace 행방불명.Framework.UI
{
	public class View
	{

		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public RectangleF Rect
		{
			get
			{
				return new RectangleF(X, Y, Width, Height);
			}
			set
			{
				X = value.Left;
				Y = value.Top;
				Width = value.Width;
				Height = value.Height;
			}
		}


		public View()
		{
			X = Y = Width = Height = 0;
		}


		public View(float x, float y, float width, float height)
			: this()
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}


		public delegate void DrawHandler();
		public delegate void UpdateHandler(float delta);

		public event DrawHandler Draw;
		public event UpdateHandler Update;

		
		public void OnUpdate(float delta)
		{
			if(Update != null)
				Update(delta);
		}

		public void OnDraw()
		{
			if(Draw != null)
				Draw();

		}

	}
}
