

using BattleNetwork.Characters;

namespace BattleNetwork.Battle.Chips
{
    public class ChipFactory
    {
		public static BaseChip GetChip(PlayerUnit unit, int playerId, short cid)
		{

			switch (cid)
			{
				case (short)0:
					return new Missile(unit, playerId, cid);
				case (short)1:
					return new Cannon(unit, playerId, cid);
				case (short)2:
					return new Sword(unit, playerId, cid);
			}


			return null;
		}
	}
}
