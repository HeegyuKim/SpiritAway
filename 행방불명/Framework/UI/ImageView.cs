using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;



namespace 행방불명.Framework.UI
{
	public class ImageView
		: View
	{
		Bitmap _bitmap;
		Graphics2D _g2d;

		public Bitmap Bitmap { get; set; }
		public float Opacity { get; set; }
		public BitmapInterpolationMode InterpolationMode { get; set; }
		public ImageView(Graphics2D g2d)
		{
			_bitmap = null;
			_g2d = g2d;
			Opacity = 1;
			InterpolationMode = BitmapInterpolationMode.Linear;
		}
	
		
		public ImageView(Graphics2D g2d, Bitmap bitmap)
			: this(g2d)
		{
			_bitmap = bitmap;
            Rect = new SharpDX.RectangleF (0, 0, bitmap.Size.Width, bitmap.Size.Height);
		}


		public void DrawBitmap()
		{
			if (_bitmap == null) return;


			_g2d.RenderTarget.DrawBitmap(
				_bitmap,
				Rect,
				Opacity,
				InterpolationMode
				);
		}
	}
}
