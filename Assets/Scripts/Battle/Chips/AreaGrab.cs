using BattleNetwork.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetwork.Battle.Chips
{
    public class AreaGrab : BaseChip
    {
        public AreaGrab(PlayerUnit unit, int playerId, short chipId, Arena arena) : base(unit, playerId, chipId, arena)
        {

        }

        public override void Advance()
        {
        }

        public override void Init()
        {
            this.unit.TriggerAttackAnimation();

        }

        public override bool IsComplete()
        {
            return true;
        }
    }
}
