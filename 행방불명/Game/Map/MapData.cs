using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace 행방불명.Game.Map
{
	public class Link
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;

		[JsonProperty(PropertyName = "name")]
		public string Name;

		[JsonProperty(PropertyName = "required")]
		public string Required;
	}

	public class Script
	{
		[JsonProperty(PropertyName = "target_name")]
		public string TargetName;

		[JsonProperty(PropertyName = "target_text")]
		public string TargetText;

		[JsonProperty(PropertyName = "player_text")]
		public string PlayerText;

		[JsonProperty(PropertyName = "sfx")]
		public string Sfx;

		public Script() { }

		public Script(string targetName, string targetText, string playerText)
		{
			TargetName = targetName;
			TargetText = targetText;
			PlayerText = playerText;
			Sfx = null;
		}

		public Script(string targetName, string targetText, string playerText,
			string sfx)
		{
			TargetName = targetName;
			TargetText = targetText;
			PlayerText = playerText;
			Sfx = sfx;
		}
	}


	public class Waypoint
	{
		//
		//
		// Property for Every Waypoint
		[JsonProperty(PropertyName="id")]
		public string Id;
		[JsonProperty(PropertyName = "type")]
		public string Type;
		[JsonProperty(PropertyName = "x")]
		public float X;
		[JsonProperty(PropertyName = "y")]
		public float Y;
		[JsonProperty(PropertyName = "scripts")]
		public List<Script> Scripts;

		// Property for Waypoint-Tutorial
		[JsonProperty(PropertyName = "next")]
		public string Next;
		[JsonProperty(PropertyName = "sfx")]
		public string Sfx;

		// Property for Crossroad
		[JsonProperty(PropertyName = "links")]
		public List<Link> Links;

		// Property for Curve
		[JsonProperty(PropertyName = "link1")]
		public string Link1;
		[JsonProperty(PropertyName = "link2")]
		public string Link2;


		// Property for people
		[JsonProperty(PropertyName = "obtained")]
		public int NumObtained;
		[JsonProperty(PropertyName = "patients")]
		public int NumPatients;


		// Property for Portal
		[JsonProperty(PropertyName = "value")]
		public string Value;


		public bool Used;

		public Waypoint()
		{
			Used = false;
		}
	}

	public class Area
	{

		[JsonProperty(PropertyName = "x")]
		public float X;
		[JsonProperty(PropertyName = "y")]
		public float Y;
		[JsonProperty(PropertyName = "radius")]
		public float Radius;
	}

	public class Mistery
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "bomb")]
		public bool Bomb;
		[JsonProperty(PropertyName = "x")]
		public float X;
		[JsonProperty(PropertyName = "y")]
		public float Y;

		[JsonProperty(PropertyName = "detects")]
		public List<Area> detects;
		[JsonProperty(PropertyName = "damages")]
		public List<Area> damages;

		[JsonProperty(PropertyName = "time")]
		public float Time;

		public float Delta;
		public bool Fired, Checked, Known, Ignored;

		Mistery()
		{
			Delta = 0;
			Fired = false;
			Known = false;
			Checked = false;
			Ignored = false;
		}

		public bool IsDetected(float x, float y)
		{
			
			foreach (var det in detects)
			{
				float dx = x - det.X,
						dy = y - det.Y;
				float len2 = dx * dx + dy * dy;

				if (len2 < det.Radius * det.Radius)
					return true;
			}
			return false;
		}

		public bool IsDamaged(float x, float y)
		{

			foreach (var det in damages)
			{
				float dx = x - det.X,
						dy = y - det.Y;
				float len2 = dx * dx + dy * dy;

				if (len2 < det.Radius * det.Radius)
					return true;
			}
			return false;
		}
	}

	public class Survivor
	{
		[JsonProperty(PropertyName = "id")]
		public string Id;
		[JsonProperty(PropertyName = "obtained")]
		public int NumObtained;
		[JsonProperty(PropertyName = "patients")]
		public int NumPatients;
		[JsonProperty(PropertyName = "hp")]
		public int Hp;
		[JsonProperty(PropertyName = "detect")]
		public float DetectRadius;
		[JsonProperty(PropertyName = "relief")]
		public string Relief;
		[JsonProperty(PropertyName = "thanks")]
		public string Thanks;

		[JsonProperty(PropertyName = "x")]
		public float X;
		[JsonProperty(PropertyName = "y")]
		public float Y;


		public float SX = 0, 
					SY = 0,
					LoopDelta = 0,
					LoopCycle = 0;
		public bool IsPlaying = false;
	};

	public class Event
	{
		List<Script> Scripts;
		
		List<Script> AgreeScripts;

		string AgreeLink;

		List<Script> DisagreeScripts;

		string TargetName;

		string TargetText;

		string Agree;

		string Disagree;
	}

	public class MapData
	{
		[JsonProperty(PropertyName = "stage")]
		public string Stage;
		[JsonProperty(PropertyName = "player_position")]
		public string PlayerPosition;
		[JsonProperty(PropertyName = "esc_to_skip")]
		public bool escToSkip;
		[JsonProperty(PropertyName = "waypoints")]
		public List<Waypoint> Waypoints;
		[JsonProperty(PropertyName = "misteries")]
		public List<Mistery> Misteries;
		[JsonProperty(PropertyName = "survivors")]
		public List<Survivor> Survivors;
		[JsonProperty(PropertyName = "choices")]
		public List<string> Choices;


		public Waypoint GetWaypoint(string id)
		{
			var found = from waypoint in Waypoints
						   where waypoint.Id.Equals(id)
						   select waypoint;
			if (found.Count() > 0)
				return found.First();
			else return null;
		}

		public void ConvertEncoding()
		{
			string src = "ISO-8859-1",
					dest = "u";

			foreach (var waypoint in Waypoints)
			{
				if(waypoint.Scripts != null)
					foreach (var script in waypoint.Scripts)
					{
						script.TargetText = StringEncodingConvert(script.TargetText, src, dest);
						script.TargetName = StringEncodingConvert(script.TargetName, src, dest);
						script.PlayerText = StringEncodingConvert(script.PlayerText, src, dest);
					}
				
			}
		}


		public static String StringEncodingConvert(String strText, String strSrcEncoding, String strDestEncoding)
		{
			if (strText == null) return null;

			System.Text.Encoding srcEnc = System.Text.Encoding.GetEncoding(strSrcEncoding);
			System.Text.Encoding destEnc = System.Text.Encoding.GetEncoding(strDestEncoding);
			byte[] bData = srcEnc.GetBytes(strText);
			byte[] bResult = System.Text.Encoding.Convert(srcEnc, destEnc, bData);
			return destEnc.GetString(bResult);
		}
		
		public bool IsTutorial()
		{
			return Stage != null && Stage.Equals("tutorial");
		}
		public bool IsB1()
		{
			return Stage != null && Stage.Equals("B1");
		}
		public bool IsSkippableOnESC()
		{
			return escToSkip;
		}
	}
}
