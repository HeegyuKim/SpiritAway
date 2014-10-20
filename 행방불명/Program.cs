using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using SharpDX.Toolkit;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.DXGI;

namespace 행방불명
{
	class Program : Form
	{
		Graphics2D g2d;
		Graphics3D g3d;

		SharpDX.Direct2D1.Bitmap bitmap;

		public Graphics2D Graphics2D { get { return g2d; } }
		public Graphics3D Graphics3D { get { return g3d; } }

		public Program()
		{
			Text = "행방불명";
			ClientSize = new Size(800, 600);
			MaximizeBox = false;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

			g3d = new Graphics3D(Handle, ClientSize.Width, ClientSize.Height, false);
			g2d = new Graphics2D(g3d.SwapChain.GetBackBuffer<Surface>(0));

			g3d.SwapChain.SetFullscreenState(true, null);

			bitmap = g2d.LoadBitmap("pic_test.png");

			Paint += OnDraw;
		}

		private void OnDraw(object sender, PaintEventArgs args)
		{
			var rt = g2d.RenderTarget;
			rt.BeginDraw();
			rt.Clear(new SharpDX.Color4(1, 0.5f, 0.5f, 1));
			rt.DrawBitmap(
				bitmap, 
				new SharpDX.RectangleF(0, 0, 500, 700),
				1.0f, 
				SharpDX.Direct2D1.BitmapInterpolationMode.Linear
				);
			rt.EndDraw();

			g3d.SwapChain.Present(0, PresentFlags.None);
		}


		[STAThread]
		public static void Main(string []args) 
		{
			Application.Run(new Program());
		}
	}
}
