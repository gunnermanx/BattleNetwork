using BattleNetwork.Characters;
using BattleNetwork.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetwork.Battle.Chips
{
    public class Poison : BaseChip
    {
        private int duration;
        private int progress = 0;

        public Poison(PlayerUnit unit, int playerId, short chipId, Arena arena) : base(unit, playerId, chipId, arena)
        {

        }

        public override void Advance()
        {
        }

        public override void Init()
        {
            this.unit.TriggerAttackAnimation();

            progress = 0;

            // TODO: need to get this from the data we receive from the server
            int level = 1;

            Chip c = GameDB.Instance.ChipsDB.GetChip(chipId);
            ChipData cd = c.data[level];

            if (playerId == 1)
            {
                this.arenaRef.TargetColumn(4, true);
                this.arenaRef.TargetColumn(5, true);
            } else
            {
                this.arenaRef.TargetColumn(0, true);
                this.arenaRef.TargetColumn(1, true);
            }
            
        }

        public override bool IsComplete()
        {
            return true;
        }
    }
}
