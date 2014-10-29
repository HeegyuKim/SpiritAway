using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 행방불명.Game.Map;
using SharpDX;

namespace 행방불명.Game
{
	public class Player
	{
		Waypoint moveFrom,
			moveTo;
		Vector2 normal;

		float distance;
		float movedDistance;
		
		public Vector2 CurrentPosition
		{
			get
			{
				return new Vector2(
					moveFrom.X + normal.X * movedDistance,
					moveFrom.Y + normal.Y * movedDistance
					);
			}
		}
		public Vector2 Normal
		{
			get
			{
				return normal;
			}
		}

		public Waypoint MoveFrom { get { return moveFrom; } }
		public Waypoint MoveTo { get { return moveTo; } }
		public PlayerState State { get; set; }

		public int NumObtained { get; set; }
		public int NumPatients { get; set; }
		public int NumDead { get; set; }
		public bool HasHammer { get; set; }
		public bool HasKey { get; set; }
		public int NumMedicalKits { get; set; }


		public bool IsArrived
		{
			get
			{
				return State == PlayerState.Arrived;
			}
		}
		public bool IsProcessing
		{
			get
			{
				return State == PlayerState.Processing;
			}
		}
		public bool IsMoving
		{
			get
			{
				return State == PlayerState.Moving;
			}
		}


		public Player(Waypoint at)
		{
			NumMedicalKits = 0;
			NumPatients = 0;
			NumObtained = 0;
			NumDead = 0;
			HasHammer = false;
			HasKey = false;

			State = PlayerState.Arrived;
			moveTo = at;
			moveFrom = at;
			distance = movedDistance = 0;
			normal = new Vector2(0, 0);
		}

		public void Change(Waypoint at)
		{
			State = PlayerState.Arrived;
			moveTo = at;
			moveFrom = at;
			distance = movedDistance = 0;
			normal = new Vector2(0, 0);
		}
		
		public void StartTo(Waypoint moveTo)
		{
			StartTo(this.moveTo, moveTo);
		}

		public void StartTo(Waypoint moveFrom, Waypoint moveTo)
		{
			Console.WriteLine("Player is starting movement from {0} to {1}",
				moveFrom.Id,
				moveTo.Id
				);

			movedDistance = 0;

			this.moveFrom = moveFrom;
			this.moveTo = moveTo;

			float dx = moveTo.X - moveFrom.X,
				dy = moveTo.Y - moveFrom.Y;

			normal = new Vector2(dx, dy);
			if (!normal.IsZero)
			{
				distance = normal.Length();
				normal.Normalize();
			}
			else
			{
				distance = 0;
			}

			State = PlayerState.Moving;
		}


		public void Move(float d)
		{
			movedDistance += d;
			if (movedDistance >= distance)
			{
				movedDistance = distance;
				State = PlayerState.Arrived;
			}
		}
	}
}
