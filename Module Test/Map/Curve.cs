using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Curve
		: Waypoint
	{
		Link _1, _2;

		
		public Link Link1 { get { return _1; } }
		public Link Link2 { get { return _2; } }


		public Curve(
			string id,
			float x,
			float y,
			Link link1,
			Link link2
			)
			: base(
				  id,
				  x,
				  y)
		{
			_1 = link1;
			_2 = link2;
		}
	}
}
