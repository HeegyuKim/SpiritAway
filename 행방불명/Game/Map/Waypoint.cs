using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Waypoint
	{
		float _x, _y;
		string _id;


		public string ID { get { return _id; } }
		public float X { get { return _x; } }
		public float Y { get { return _y; } }



		public Waypoint(string id, float x, float y)
		{
			_x = x;
			_y = y;
			_id = id;
		}


		public bool IsEqualID(Waypoint that)
		{
			return _id.Equals(that._id);
		}

	}
}
