using BattleNetwork.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetwork.Battle.Chips
{ 
    public class Sword : BaseChip
    {
        public Sword(PlayerUnit unit, int playerId, short chipId, Arena arena) : base(unit, playerId, chipId, arena)
        {

        }

        public override void Advance()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            this.unit.TriggerMeleeAttackAnimation();
        }

        public override bool IsComplete()
        {
            return true;
        }
    }
}
