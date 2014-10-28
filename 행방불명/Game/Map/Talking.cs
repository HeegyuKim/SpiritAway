using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Talking
	{
		List<Script> scripts;

		public Talking(List<Script> scripts)
		{
			this.scripts = scripts;
		}

		public Talking(params Script []scripts)
		{
			this.scripts = new List<Script>(scripts);
		}

		int currIndex = 0;

		public void Next()
		{
			if (HasNext)
				currIndex ++;
		}

		public Script Current
		{
			get
			{
				return scripts[currIndex];
			}
		}

		public bool HasNext
		{
			get
			{
				return currIndex < scripts.Count - 1;
			}
		}

		public bool HasVoice
		{
			get
			{
				return Current.PlayerText != null && Current.PlayerText.Length > 0;
			}
		}
	}
}
