﻿using BattleNetwork.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNetwork.Battle.Chips
{
    public class Cannon : BaseChip
    {
        public Cannon(PlayerUnit unit, int playerId, short chipId) : base(unit, playerId, chipId)
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