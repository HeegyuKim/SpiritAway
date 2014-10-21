using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Game;
using SharpDX.Toolkit;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.DXGI;

namespace 행방불명
{
	/*
	 * Program 클래스는 Main 정적 메서드가 구현되어 있고
	 * 그래픽 객체들을 갖고 있고
	 * 스테이지들을 담당하고
	 * FPS를 관리함.
	 */
	public class Program
	{
		private Graphics2D g2d;
		private Graphics3D g3d;
		private Form mForm;


		public Graphics2D Graphics2D { get { return g2d; } }
		public Graphics3D Graphics3D { get { return g3d; } }
		public Form Form { get { return mForm; } }


		public Program()
		{
			mForm = new Form();
			mForm.Text = "행방불명";
			mForm.ClientSize = new Size(800, 600);
			mForm.MaximizeBox = false;
			mForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

			g3d = new Graphics3D(mForm.Handle, mForm.ClientSize.Width, mForm.ClientSize.Height, false);
			g2d = new Graphics2D(g3d.SwapChain.GetBackBuffer<Surface>(0));

			mCurrStage = new StartStage();

			mTimer = new Timer();
			mTimer.Interval = 16;
			mTimer.Tick += OnUpdate;
			mTimer.Start();
		}
		
		~Program()
		{
			if(mTimer.Enabled)
				mTimer.Stop();
		}

		private Stage mCurrStage;
		private Timer mTimer;

		private void OnUpdate(object sender, EventArgs args)
		{
			var stage = mCurrStage;

			stage.Update(16.6f);
			stage.Draw(this);

			if (stage.IsEnded())
			{
				mCurrStage = stage.getNextStage();
				if (mCurrStage == null)
					mTimer.Stop();

				var obj = stage as IDisposable;
				if (obj != null)
					obj.Dispose();
			}
		}
		
		

		[STAThread]
		public static void Main(string []args) 
		{
			Application.Run(new Program().Form);
		}
	}
}
