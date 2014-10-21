using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Framework
{
	// 
	// 모든 스테이지들이 상속받는
	// 스테이지 인터페이스
	//
	public interface Stage
	{

		void Update(float delta);
		
		void Draw(Program program);
		
		Stage getNextStage();

		bool IsEnded();
	}
}
