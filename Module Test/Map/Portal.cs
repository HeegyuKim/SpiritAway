using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Portal
		: Waypoint
	{
		string _value;

		
		public string Value { get { return _value; } }


		public Portal (
			string id,
			float x,
			float y,
			string value
			)
			: base(id, x, y)
		{
			_value = value;
		}
	}
}
