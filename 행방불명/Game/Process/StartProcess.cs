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
		Player player;
		Waypoint moveTo;

		public StartProcess(Player player, Waypoint moveTo)
		{
			this.player = player;
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
			Console.WriteLine("StartProcess started. to " + moveTo.Id);
			player.StartTo(moveTo);
		}

		public bool IsEnded()
		{
			return true;
		}
	}
}
