using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Framework;
using SharpDX;

namespace 행방불명.Game
{
	public class StartStage : Stage
	{

		public StartStage()
		{

		}

		~StartStage()
		{

		}

		public void Draw(Program app)
		{
			var g2d = app.Graphics2D;
			var rt = g2d.RenderTarget;

			rt.BeginDraw();
			rt.Clear(new Color4(1, 1, 1, 1));


			rt.EndDraw();
			app.Graphics3D.Present();
		}

		public void Update(float delta)
		{

		}

		public Stage getNextStage()
		{
			return null;
		}

		public bool IsEnded()
		{
			return false;
		}
	}
}
