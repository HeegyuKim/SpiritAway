using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
	public class EmptyProcess : IProcess
	{
		~EmptyProcess()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void Update(float delta)
		{
		}

		public virtual void End()
		{
		}

		public virtual bool IsEnded()
		{
			return true;
		}
	}
}
