using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
	public class DelayProcess : IProcess
	{
		float delayTime = 0, delayDelta = 0;

		public DelayProcess(float delayTime)
		{
			this.delayTime = delayTime;
		}


		public void Start()
		{
			delayDelta = 0;
		}

		public void Update(float delta)
		{
			delayDelta += delta;
		}

		public void End()
		{
		}

		public bool IsEnded()
		{
			return delayTime <= delayDelta;
		}
	}
}
