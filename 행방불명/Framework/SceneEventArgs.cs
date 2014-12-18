using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	public class SceneEventArgs
	{
		public Scene Scene { get; set; }
		public float Delta { get; set; }

		public SceneEventArgs(Scene scene, float delta)
		{
			Scene = scene;
			Delta = delta;
		}
	}
}
