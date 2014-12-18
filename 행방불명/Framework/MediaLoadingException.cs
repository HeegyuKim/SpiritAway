using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	class MediaLoadingException : Exception
	{
		public MediaLoadingException(string message)
			: base(message)
		{	
		}
	}
}
