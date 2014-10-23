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
			var sound = engine.Play2D("sound.mp3", false, true);

			sound.Paused = false;

			Console.WriteLine("아무 키나 누르면 끝납니다.");
			Console.ReadKey();

			sound.Dispose();
		}
	}
}
