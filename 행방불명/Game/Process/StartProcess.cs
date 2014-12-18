using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;

namespace 행방불명.Game.Process
{
	public class StartProcess
		: IProcess
	{
		GameStage stage;
		Player player;
		Waypoint moveTo;

		public StartProcess(GameStage stage, Waypoint moveTo)
		{
			this.stage = stage;
			this.player = stage.Player;
			this.moveTo = moveTo;
		}

		public StartProcess(GameStage stage, string nextId)
		{
			player = stage.Player;
			moveTo = stage.Map.GetWaypoint(nextId);

			if (moveTo == null)
				throw new Exception("다음 목적지가 없잖아! ㅡㅡ;");
		}

		public void Start()
		{
		}

		public void Update(float delta)
		{
		}

		public void End()
		{
			player.StartTo(moveTo);
		}

		public bool IsEnded()
		{
			return true;
		}
	}
}
