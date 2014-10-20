using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{

	public class Stage
	{

		public delegate void UpdateHandler(float delta);
		public delegate void DrawHandler();

		public event UpdateHandler OnUpdate;
		public event DrawHandler OnDraw;

		public Stage NextStage { get; set; }
		public bool HasNext { get; set; }

	}
}
