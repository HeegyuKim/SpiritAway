using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

namespace 행방불명.Framework
{
	class Media
	{
		private Dictionary<string, object> mDic = new Dictionary<string, object>();

		public Media(String mediaUri)
		{
			var obj = JObject.Parse(File.ReadAllText(mediaUri));
			var res = obj.Property("res").Value as JArray;

			if (res == null)
				throw new MediaLoadingException(mediaUri + " JSON 형식의 파일에서 res 속성을 찾을 수 없습니다.");

			foreach(JObject item in res.Children<JObject>())
			{
				string type = item.Property("type").Value.ToString();
				Console.WriteLine(type + " type items here!");
				
			}
		}

		public object this[string name] {
			get {
				return mDic[name];
			}
			set {
				mDic[name] = value;
			}
		}


	}
}
