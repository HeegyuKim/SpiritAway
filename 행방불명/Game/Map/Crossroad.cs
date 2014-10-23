using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;


namespace 행방불명.Game.Map
{
	public class Crossroad
		: Waypoint
	{
		List<Link> _links = new List<Link>();
		string _soundName;
		

		public List<Link> Links { get { return _links; } }
		public string SoundName { get { return _soundName; } }


		public Crossroad(
			string id,
			float x,
			float y,
			string soundName
			)
			: base(id, x, y)
		{
			_soundName = soundName;
		}

	}
}
