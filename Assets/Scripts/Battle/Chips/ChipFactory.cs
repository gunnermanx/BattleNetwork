using BattleNetwork.Characters;

namespace BattleNetwork.Battle.Chips
{
    public class ChipFactory
    {
		public static BaseChip GetChip(PlayerUnit unit, int playerId, short cid, Arena arena)
		{

			switch (cid)
			{
				case (short)10:
					return new Cannon(unit, playerId, cid, arena);					
				case (short)20:
					return new Missile(unit, playerId, cid, arena);
				case (short)50:
					return new Sword(unit, playerId, cid, arena);
				case (short)80:
					return new Poison(unit, playerId, cid, arena);
				case (short)100:
					return new AreaGrab(unit, playerId, cid, arena);
			}


			return null;
		}
	}
}
