using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;



namespace 행방불명.Game.Map
{
	public class GameMap
	{
		List<Waypoint> _waypointList;

		public List<Waypoint> Waypoints { get { return _waypointList; } }



		public GameMap(string filename)
		{
			// 파일에서 읽어옴
			var text = File.ReadAllText(filename);
			var json = new JObject(text);

			// 리더를 생성해서 파싱함
			_waypointList = new List<Waypoint>();
			var reader = new WaypointReader(_waypointList);
			reader.Load(json);
		}


		// id가 맞는 웨이포인트를 찾아서 반환함.
		public Waypoint Find(string id)
		{
			var result = from waypoint in Waypoints
				   where waypoint.ID.Equals(id)
				   select waypoint;
			return result.First<Waypoint>();
		}
	}
}
