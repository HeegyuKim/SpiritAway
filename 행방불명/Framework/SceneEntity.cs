using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	public class SceneEntity
	{
		private float mTime;

		
		public float Time
		{
			get
			{
				return mTime;
			}
			set
			{
				mTime = value;
			}
		}
		
		
		public SceneEntity(float time)
		{
			mTime = time;
		}

		
		public delegate void SceneHandler(object sender);
		public delegate void UpdateHandler(object sender, SceneEventArgs args);

		public event SceneHandler
				Start,
				End;

		public event UpdateHandler Update;

		public void OnStart(Scene scene)
		{
			Start(this);
		}
		public void OnEnd(Scene scene)
		{
			End(this);
		}
		public void OnUpdate(Scene scene, float delta)
		{
			Update(this, new SceneEventArgs(scene, delta));
		}

	}
}
