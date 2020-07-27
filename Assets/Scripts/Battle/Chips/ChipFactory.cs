

using BattleNetwork.Characters;

namespace BattleNetwork.Battle.Chips
{
    public class ChipFactory
    {
		public static BaseChip GetChip(PlayerUnit unit, int playerId, short cid, Arena arena)
		{

			switch (cid)
			{
				case (short)0:
					return new Missile(unit, playerId, cid, arena);
				case (short)1:
					return new Cannon(unit, playerId, cid, arena);
				case (short)2:
					return new Sword(unit, playerId, cid, arena);
			}


			return null;
		}
	}
}
