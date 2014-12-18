using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	class MediaSource
	{
		
		public delegate object LoadHandler();
		public delegate void ReleaseHandler(MediaSource source);


		public bool IsLoaded { get; private set; }
		public object Source { get; private set; }

		LoadHandler OnLoad;
		ReleaseHandler OnRelease;


		public MediaSource(LoadHandler load, ReleaseHandler release)
		{
			this.OnLoad = load;
			this.OnRelease = release;
			IsLoaded = false;
			Source = null;
		}


		public void Load()
		{
			Source = OnLoad();
			IsLoaded = true;
		}


		public void Release()
		{
			OnRelease(this);
			IsLoaded = false;
		}

	}
}
