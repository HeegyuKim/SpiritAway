using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace 행방불명.Game.Map
{
	public class GameMap
	{
		List<Waypoint> waypoints;
		List<Mistery> misteries;

		public List<Waypoint> Waypoints { get { return waypoints; } }
		public List<Mistery> Misteries { get { return misteries; } }



		public GameMap(string filename)
		{
			// 파일에서 읽어옴
			var text = File.ReadAllText(filename);

			MapData data = JsonConvert.DeserializeObject<MapData>(text);

		}


		// id가 맞는 웨이포인트를 찾아서 반환함.
		public Waypoint Find(string id)
		{
			var result = from waypoint in Waypoints
				   where waypoint.Id.Equals(id)
				   select waypoint;
			return result.First<Waypoint>();
		}
	}
}
