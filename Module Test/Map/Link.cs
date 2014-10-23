using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Map
{
	public class Link
	{
		public string id, name;

		public Link(string id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public override bool Equals(object obj)
		{
			var that = obj as Link;
			if (that == null) return false;

			return id == that.id && name == that.name;
		}
	}
}
