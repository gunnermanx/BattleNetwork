using BattleNetwork.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetwork.Battle.Chips
{
    public abstract class BaseChip
    {
        public static int MISSILE_ID = 0;
        public static int CANNON_ID = 1;
        public static int SWORD_ID = 2;

        public PlayerUnit unit;
        public int playerId;
        public short chipId;

        protected Arena arenaRef;

        public BaseChip(PlayerUnit unit, int playerId, short chipId, Arena arena) {
            this.unit = unit;
            this.playerId = playerId;
            this.chipId = chipId;
            this.arenaRef = arena;
        }

        public abstract void Init();

        public abstract void Advance();

        public abstract bool IsComplete();
    }
}
