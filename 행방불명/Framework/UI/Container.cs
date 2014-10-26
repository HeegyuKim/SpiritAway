using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpDX.Direct2D1;

namespace 행방불명.Framework.UI
{
	public class Container
	{
		Program app;
		List<View> _viewList = new List<View>();


		public List<View> Views { get { return _viewList; } }



		public Container(Program app)
		{
			this.app = app;
			brush = new SolidColorBrush(
				app.Graphics2D.RenderTarget,
				new SharpDX.Color4(1, 1, 1, 1)
				);
		}



		public void Update(float delta)
		{
			foreach (View view in _viewList)
			{
				view.OnUpdate(delta);
			}
		}



		public void Draw()
		{
			foreach (View view in _viewList)
			{
				view.OnDraw();
				DrawOutline(view);
			}
		}

		Brush brush;

		[Conditional("DEBUG")]
		private void DrawOutline(View view)
		{
			app.Graphics2D.RenderTarget.DrawRectangle(
				view.Rect,
				brush
				);
		}
	}
}
