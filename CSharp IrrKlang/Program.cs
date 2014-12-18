using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;

namespace CSharp_IrrKlang
{


	class Program
	{
		static void Main(string[] args)
		{
			ISoundEngine engine = new ISoundEngine();
			engine.SetListenerPosition(
				new Vector3D(0, 0, 0),
				new Vector3D(1, 0, 0),
				new Vector3D(0, 0, 0),
				new Vector3D(0, 1, 0)
				);

			var source = engine.AddSoundSourceFromFile("sound.mp3");
			Console.WriteLine("Maximum distance: " + source.DefaultMaxDistance);
			Console.WriteLine("Minimum distance: " + source.DefaultMinDistance);

			engine.Play3D("sound.mp3", 0, 0, 5);

			Console.WriteLine("아무 키나 누르면 끝납니다.");
			Console.ReadKey();
		}
	}
}
