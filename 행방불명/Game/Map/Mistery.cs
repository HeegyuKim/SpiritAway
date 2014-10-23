using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Mistery
		: Waypoint
	{
		string _real = "";

		public string Real { get { return _real; } }


		public Mistery(
			string id,
			float x,
			float y,
			string real
			) : base (
				id,
				x,
				y
				)
		{
			_real = real;
		}
	}
}
