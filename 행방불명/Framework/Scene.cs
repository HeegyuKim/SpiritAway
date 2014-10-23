using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	public class Scene
	{
		LinkedList<SceneEntity> mSceneList;

		public LinkedList<SceneEntity> List
		{
			get
			{
				return mSceneList;
			}
		}



		public Scene()
		{
			mSceneList = new LinkedList<SceneEntity>();
		}


		float delta;

		public void Start(SceneEntity entity)
		{
			mSceneList.AddLast(entity);
			entity.OnStart(this);
			delta = 0;
		}



		public void Update(float delta)
		{
			this.delta += delta;
			var curr = CurrentScene;

			curr.OnUpdate(
				this,
				delta
				);

			if (delta > curr.Time)
			{
				curr.OnEnd(this);
				mSceneList.RemoveFirst();
			}
		}


		public void Stop()
		{
			CurrentScene.OnEnd(this);
			mSceneList.RemoveFirst();
		}


		public SceneEntity CurrentScene
		{
			get
			{
				if (mSceneList.Count == 0)
					return null;
				else
					return mSceneList.First.Value;
			}
		}


	}
}
