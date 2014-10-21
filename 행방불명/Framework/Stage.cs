using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{

	public interface Stage
	{
		void Update(float delta);
		void Draw(Program program);
		Stage getNextStage();
		bool IsEnd();
	}
}
