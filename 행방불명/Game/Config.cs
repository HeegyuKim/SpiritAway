﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;



namespace 행방불명.Game
{
	public struct Rank
	{
		public string Level;			// 등급(S, A, B, C, ...)
		public float Time;				// 소요시간
		public int NumObtainedPeople;	// 구한사람수
		public int Score;
	}

	public class Config
	{
		string filename;
		List<Rank> rankList = new List<Rank>();
		bool soundEnabled;


		public bool Fullscreen { get; set; }

		public List<Rank> RankList { get { return rankList; } }
		public bool SoundEnabled 
		{
			get 
			{
				return soundEnabled;
			}
			set
			{
				soundEnabled = value;
			}
		}
		public string Filename { get { return filename; } }



		public Config(string filename)
		{
			this.filename = filename;
			try
			{
				JObject obj = JObject.Parse(File.ReadAllText(filename));

				soundEnabled = obj["sound_enabled"].Value<bool>();
				Fullscreen = obj["fullscreen"].Value<bool>();
				var ranks = obj["ranking"].Value<JArray>();

				foreach (JObject rank in ranks.Values<JObject>())
					addRank(rank);
			}
			catch (FileNotFoundException)
			{
				filename = "config.data";
				soundEnabled = true;
			}
		}



		private void addRank(JObject obj)
		{
			var rank = new Rank()
			{
				Level = obj["level"].Value<string>(),
				Time = obj["time"].Value<float>(),
				NumObtainedPeople = obj["people"].Value<int>(),
				Score = obj["score"].Value<int>()
			};
			rankList.Add(rank);
		}

		private int compareRank(Rank a, Rank b)
		{
			return b.Score - a.Score;
		}

		public void sort()
		{
			rankList.Sort(compareRank);
		}

		public void save()
		{
			var obj = new JObject();

			JArray ranking = new JArray();
			sort();
			int i = 0;
			foreach (Rank rank in rankList)
			{
				JObject rankObj = new JObject();
				rankObj.Add("level", rank.Level);
				rankObj.Add("score", rank.Score);
				rankObj.Add("time", rank.Time);
				rankObj.Add("people", rank.NumObtainedPeople);

				ranking.Add(rankObj);

				if (++i == 3) break;
			}

			obj.Add("sound_enabled", soundEnabled);
			obj.Add("fullscreen", Fullscreen);
			obj.Add("ranking", ranking);

			string text = obj.ToString();
			Console.WriteLine("설정 저장됨...");

			File.WriteAllText(filename, text);
		}
	}
}
