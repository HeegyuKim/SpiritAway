using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX;

namespace 행방불명.Framework
{
	public class Graphics3D
	{
		private SwapChain mSwapChain;
		private SharpDX.Direct3D11.Device mDevice;
		private DeviceContext mContext;



		public SwapChain SwapChain { get { return mSwapChain; } }
		public SharpDX.Direct3D11.Device Device { get { return mDevice; }}
		public DeviceContext DeviceContext { get { return mContext;}}



		// Create Direct3D11 device from window
		public Graphics3D(IntPtr handle, int width, int height, bool fullscreen)
		{
			var desc = new SwapChainDescription()
			{
				BufferCount = 1,
				Flags = SwapChainFlags.None,
				IsWindowed = !false,
				ModeDescription = new ModeDescription()
				{
					Format = Format.R8G8B8A8_UNorm,
					Width = width,
					Height = height,
					RefreshRate = new Rational()
					{
						Denominator = 1,
						Numerator = 60
					}
				},
				OutputHandle = handle,
				SampleDescription = new SampleDescription()
				{
					Count = 1,
					Quality = 0
				},
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};

			SharpDX.Direct3D11.Device.CreateWithSwapChain (
				DriverType.Hardware, 
				DeviceCreationFlags.BgraSupport, 
				desc, 
				out mDevice,
				out mSwapChain
				);

			mContext = mDevice.ImmediateContext;
		}

		~Graphics3D()
		{
			Utilities.Dispose(ref mSwapChain);
			Utilities.Dispose(ref mContext);
			Utilities.Dispose(ref mDevice);
		}

		public void Present()
		{
			mSwapChain.Present(0, PresentFlags.None);
		}
	}
}
