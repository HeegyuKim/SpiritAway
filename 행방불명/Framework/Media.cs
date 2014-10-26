using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Newtonsoft.Json.Linq;


namespace 행방불명.Framework
{

	public class Media
	{
		Program app;
		Dictionary<string, Bitmap> bitmapDic;
		Dictionary<string, TextFormat> formatDic;

		
		public Dictionary<string, Bitmap> BitmapDic { get { return bitmapDic; } }
		public Dictionary<string, TextFormat> FormatDic { get { return formatDic; } }


		public Media(Program app, String mediaUri)
		{
			this.app = app;

			bitmapDic = new Dictionary<string, Bitmap>();
			formatDic = new Dictionary<string, TextFormat>();

			JObject mediaObj = JObject.Parse(File.ReadAllText(mediaUri));
			JArray formatObjArr = mediaObj["fonts"].Value<JArray>();
			JObject bitmapObj= mediaObj["bitmaps"].Value<JObject>();
			JObject soundObj= mediaObj["sounds"].Value<JObject>();

			foreachJson(formatObjArr, AddFormat);

			foreach (var pair in bitmapObj)
			{
				bitmapDic.Add(
					pair.Key, 
					app.Graphics2D.LoadBitmap(pair.Value.Value<string>())
					);
			}
			foreach (var pair in soundObj)
			{
				var filename = pair.Value.Value<string>();
				var source = app.Sound.AddSoundSourceFromFile(filename);
				app.Sound.AddSoundSourceAlias(source, pair.Key);
			}
		}
		
		private delegate void ParseFunc(JObject obj);
		private void foreachJson(JArray arr, ParseFunc func)
		{
			foreach (var obj in arr.Values<JObject>())
				func(obj);
		}

		private void AddFormat(JObject format)
		{
			string name = format["name"].Value<string>();
			string familyName = format["family_name"].Value<string>();
			float size = format["size"].Value<float>();

			var textFormat = new TextFormat (
				app.Graphics2D.DWriteFactory,
				familyName,
				null,
				FontWeight.Normal,
				FontStyle.Normal,
				FontStretch.Normal,
				size,
				"ko-kr"
				);

			formatDic.Add(name, textFormat);
		}

		~Media()
		{
			foreach (var pair in bitmapDic)
			{
				var bitmap = pair.Value;
				Utilities.Dispose(ref bitmap);
			}
			foreach (var pair in formatDic)
			{
				var format = pair.Value;
				Utilities.Dispose(ref format);
			}

			bitmapDic.Clear();
			formatDic.Clear();
		}

	}
}
