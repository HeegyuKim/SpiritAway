using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using SharpDX.DirectWrite;
using SharpDX;

namespace 행방불명.Framework
{
	public class Graphics2D
	{
		//
		// fields

		private SharpDX.Direct2D1.Factory mFac2d;
		private SharpDX.WIC.ImagingFactory mImgFac;
		private SharpDX.DirectWrite.Factory mFacDw;
		private RenderTarget mRt;

		//
		// properties

		public RenderTarget RenderTarget { get{ return mRt; }}
		public SharpDX.Direct2D1.Factory D2DFactory { get { return mFac2d; } }
		public SharpDX.WIC.ImagingFactory ImagingFactory { get { return mImgFac; } }
		public SharpDX.DirectWrite.Factory DWriteFactory { get { return mFacDw; } }

		private Graphics2D()
		{
			mFac2d = new SharpDX.Direct2D1.Factory();
			mImgFac = new SharpDX.WIC.ImagingFactory();
			mFacDw = new SharpDX.DirectWrite.Factory();
		}

		//
		// constructor by DXGI surface
		public Graphics2D(Surface surface)
			: this()
		{
			var rtp = new RenderTargetProperties()
			{
				DpiX = mFac2d.DesktopDpi.Width,
				DpiY = mFac2d.DesktopDpi.Height,
				PixelFormat = new SharpDX.Direct2D1.PixelFormat()
				{
					AlphaMode = AlphaMode.Premultiplied,
					Format = surface.Description.Format
				},
				Type = RenderTargetType.Default,
				Usage = RenderTargetUsage.None
			};

			mRt = new RenderTarget(mFac2d, surface, rtp);
		}

		//
		// constructor by window handle
		public Graphics2D(IntPtr handle, int width, int height)
			: this()
		{
			var rtp = new RenderTargetProperties()
			{
				DpiX = mFac2d.DesktopDpi.Width,
				DpiY = mFac2d.DesktopDpi.Height,
			};
			var hp = new HwndRenderTargetProperties()
			{
				Hwnd = handle,
				PixelSize = new Size2(width, height)
			};

			mRt = new WindowRenderTarget(mFac2d, rtp, hp);
		}

		// destructor
		~Graphics2D()
		{
			Utilities.Dispose(ref mRt);

			Utilities.Dispose(ref mImgFac);
			Utilities.Dispose(ref mFacDw);
			Utilities.Dispose(ref mFac2d);
		}

		// Load Direct2D Bitmap from filename
		public SharpDX.Direct2D1.Bitmap LoadBitmap(string filename)
		{
			var decoder = new BitmapDecoder(mImgFac, filename, DecodeOptions.CacheOnDemand);
			var frame = decoder.GetFrame(0);
			var converter = new FormatConverter(mImgFac);

			converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);
			var bitmap = SharpDX.Direct2D1.Bitmap.FromWicBitmap(mRt, converter);

			Utilities.Dispose(ref decoder);
			Utilities.Dispose(ref frame);
			Utilities.Dispose(ref converter);

			return bitmap;
		}

		// Create DWrite TextFormat
		public TextFormat TextFormat(string familyName, float size)
		{
			return new TextFormat(mFacDw, familyName, size);
		}

		RectangleF rect = new RectangleF();

		public void Draw(SharpDX.Direct2D1.Bitmap bitmap, float x, float y,
			float opacity = 1, 
			SharpDX.Direct2D1.BitmapInterpolationMode mode = SharpDX.Direct2D1.BitmapInterpolationMode.Linear)
		{
			rect.X = x;
			rect.Y = y;
			rect.Size = bitmap.Size;
			mRt.DrawBitmap(
				bitmap,
				rect,
				opacity,
				mode
				);
		}
	}
}
