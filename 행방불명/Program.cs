using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using 행방불명.Game;
using 행방불명.Framework.UI;
using SharpDX.Toolkit;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.DXGI;
using IrrKlang;

namespace 행방불명
{
	/*
	 * Program 클래스는 Main 정적 메서드가 구현되어 있고
	 * 그래픽 객체들을 갖고 있고
	 * 스테이지들을 담당하고
	 * FPS를 관리함.
	 */
	public class Program
		: IDisposable
	{
		private Form mForm;
		private Graphics2D g2d;
		private Graphics3D g3d;
		private ISoundEngine engine;
		private Mouse mouse;
		private VoiceControl control;
		private Config config;



		public Graphics2D Graphics2D { get { return g2d; } }
		public Graphics3D Graphics3D { get { return g3d; } }
		public Form Form { get { return mForm; } }
		public ISoundEngine Sound { get { return engine; } }
		public Container Container { get; set; }
		public Mouse Mouse { get { return mouse; } }
		public int Width { get { return mForm.Width; } }
		public int Height { get { return mForm.Height; } }
		public VoiceControl VoiceControl { get { return control; } }
		public Config Config { get { return config; } }
		public SharpDX.RectangleF RectF { get; private set; }
		public Media Media { get; private set; }
		public readonly float SoundDistanceRate = 100;

		bool[] keyState = new bool[256];
		public bool IsKeyDown(int value)
		{
			if (value < 256)
				return keyState[value];
			else
				return false;
		}

		public Program()
		{
			mForm = new Form();
			mForm.Text = "행방불명";
			mForm.ClientSize = new Size(1024, 768);
			mForm.MaximizeBox = false;
			mForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			mForm.KeyDown += (object sender, KeyEventArgs args) =>
			{
				if (args.KeyValue < 256)
					keyState[args.KeyValue] = true;
			};
			mForm.KeyUp += (object sender, KeyEventArgs args) =>
			{
				if (args.KeyValue < 256)
					keyState[args.KeyValue] = false;
			};

			g3d = new Graphics3D(mForm.Handle, mForm.ClientSize.Width, mForm.ClientSize.Height, false);
			g2d = new Graphics2D(g3d.SwapChain.GetBackBuffer<Surface>(0));
			engine = new ISoundEngine();
			control = new VoiceControl();
			mouse = new Mouse(mForm);
			config = new Config("res/config.json");
			RectF = new SharpDX.RectangleF(0, 0, Width, Height);
			Media = new Media(this, "res/media.json");

			mCurrStage = new StartStage(this);
			//mCurrStage = new GameStage(this, "res/B1.json");
			//mCurrStage = new GameStage(this, "res/B1.json", null);
			mCurrStage.Start();

			mLastTime = DateTime.Now;

			mTimer = new Timer();
			mTimer.Interval = 16;
			mTimer.Tick += OnUpdate;
			mTimer.Start();


			mForm.HandleDestroyed += delegate(object sender, EventArgs args)
			{
				config.save();
			};

			if (!config.SoundEnabled)
				Sound.SoundVolume = 0;
		}
		
		private Stage mCurrStage;
		private Timer mTimer;
		private DateTime mLastTime;


		private void OnUpdate(object sender, EventArgs args)
		{
			var currTime = DateTime.Now;
			var delta = currTime - mLastTime;
			mLastTime = currTime;

			var stage = mCurrStage;

			stage.Update((float)delta.TotalSeconds);
			stage.Draw();

			if (stage.IsEnded())
			{
				mCurrStage = stage.getNextStage();
				if (mCurrStage == null)
					mTimer.Stop();
				else
					mCurrStage.Start();

				var obj = stage as IDisposable;
				if (obj != null)
					obj.Dispose();
			}
		}

		~Program()
		{
			Dispose(false);
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		protected void Dispose(bool managed)
		{
			if (managed)
			{
				mForm.Close();
				if (mTimer.Enabled)
					mTimer.Dispose();

				config.save();
			}
		}

		public ISound Play2D(string alias, bool looped = false)
		{
			if (alias == null) return null;

			var sound = Sound.Play2D(alias, looped, true);
			if (sound == null)
			{
				Console.Write("{0} 사운드를 찾을 수 없습니다.", alias);
				return null;
			}

			sound.Paused = false;
			return sound;
		}

		public void Play3D(string alias, float x, float y, bool looped = false)
		{
			if (alias == null) return;

			Sound.Play3D(
				alias,
				x / SoundDistanceRate,
				y / SoundDistanceRate,
				0,
				looped
				);

		}


		[STAThread]
		public static void Main(string []args) 
		{
			Application.Run(new Program().Form);
		}
	}
}
