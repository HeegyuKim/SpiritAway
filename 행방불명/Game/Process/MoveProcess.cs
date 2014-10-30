using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 행방불명.Game.Process
{
	class MoveProcess
		: IProcess
	{
		GameStage stage;

		public MoveProcess(GameStage stage)
		{
			this.stage = stage;
		}

		public void Start() { }
		public void Update(float delta) { }
		public void End()
		{
			stage.Player.State = PlayerState.Moving;
		}
		public bool IsEnded()
		{
			return true;
		}
	}
}
