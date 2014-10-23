using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class People
		: Waypoint
	{
		string _soundName;
		int _numHealthy;
		int _numHurt;

		public People(
			string id,
			float x,
			float y,
			string soundName,
			int numHealthy,
			int numHurt
			)
			: base(
				id,
				x,
				y)
		{
			_soundName = soundName;
			_numHealthy = numHealthy;
			_numHurt = numHurt;
		}


	}
}
