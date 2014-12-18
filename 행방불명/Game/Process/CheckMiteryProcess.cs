using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;

namespace 행방불명.Game.Process
{
	public class CheckMiteryProcess
		: EmptyProcess
	{
		Mistery mis;

		public CheckMiteryProcess(Mistery mis)
		{
			this.mis = mis;
		}

		public override void End()
		{
			mis.Checked = true;
		}
	}
}
