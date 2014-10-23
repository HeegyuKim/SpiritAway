using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;



namespace 행방불명.Game.Entities
{
	public class SoundEntity 
		: Framework.SceneEntity
	{
		ISound _sound;


		public SoundEntity(ISoundEngine engine, string filename)
			: this(engine.Play2D(filename, false, true))
		{
		}


		public SoundEntity(ISound sound)
			: base(sound.PlayLength / 1000.0f)
		{
			_sound = sound;

			Start += (object sender) =>
			{
				_sound.Paused = false;
			};
			End += (object sender) =>
			{
				_sound.Stop();
			};
		}

		
	}
}
