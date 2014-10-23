using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;



namespace 행방불명.Game.Map
{
	public class WaypointReader
	{
		List<Waypoint> _waypointList;

		//
		// 
		public WaypointReader(List<Waypoint> waypointList)
		{
			_waypointList = waypointList;
			
		}

		public void Load(JObject obj)
		{
			var waypointArr = obj["waypoints"].Value<JArray>();

			LoadWaypoints(waypointArr);
		}


		// waypoint 객체 배열들로부터
		// waypoint 객체를 생성해 리스트에 추가
		private void LoadWaypoints(JArray array)
		{
			foreach (JObject obj in array.Values<JObject>())
			{
				var waypoint = MakeWaypointFrom(obj);

				_waypointList.Add(waypoint);
			}
		}


		//
		// waypoint 객체로부터 웨이포인트를 생성
		private Waypoint MakeWaypointFrom(JObject obj)
		{
			// 기본적인 타입 값들을 얻어옴
			var type = obj["type"].Value<string>();
			string id = obj["id"].Value<string>();
			float x = obj["x"].Value<float>();
			float y = obj["y"].Value<float>();
			
			Waypoint waypoint = null;

			switch (type)
			{
				// 교차로가 추가됨
				case "crossroad":

					var crossroad = new Crossroad(
						id,
						x,
						y,
						obj["sfx"].Value<string>()
						);

					// 연결된 곳들을 찾아서 추가함.
					var links = obj["links"].Values<JArray>();
					foreach (var linkObj in links.Values<JObject>())
						crossroad.Links.Add(MakeLink(linkObj));

					break;

				// 포탈이 추가됨
				case "portal":
					waypoint = new Portal (
						id,
						x,
						y,
						obj["value"].Value<string>()
						);
				
					break;
					
				// 사람들이 추가됨.
				case "people":
					waypoint = new People(
						id,
						x,
						y,
						obj["sfx"].Value<string>(),
						obj["num_healthy"].Value<int>(),
						obj["num_hurt"].Value<int>()
						);

					break;


				// 그냥 꺾는 곳.
				case "curve":
					waypoint = new Curve(
						id,
						x,
						y,
						MakeLink(obj["link1"].Value<JObject>()),
						MakeLink(obj["link2"].Value<JObject>())
						);

					break;

				// 알 수 없는 물체
				case "mistery":
					waypoint = new Mistery(
						id,
						x,
						y,
						obj["real"].Value<string>()
						);

					break;

				default:
					throw new Exception("'" + type + "' 라는 알 수 없는 타입의 웨이포인트 입니다.");
			};

			return waypoint;
		}

		private Link MakeLink(JObject obj)
		{
			return new Link (
				obj["id"].Value<string>(),
				obj["name"].Value<string>()
				);
		}
	}
}
