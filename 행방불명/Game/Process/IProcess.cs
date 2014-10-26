using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
	public interface IProcess
	{
		void Start();
		void Update(float delta);
		void End();
		bool IsEnded();
	}
}
