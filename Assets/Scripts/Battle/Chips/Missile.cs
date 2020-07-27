using BattleNetwork.Characters;
using BattleNetwork.Data;
using System;
using UnityEngine;

namespace BattleNetwork.Battle.Chips
{
    public class Missile : BaseChip
    {
        public Missile(PlayerUnit unit, int playerId, short chipId, Arena arena) : base(unit, playerId, chipId, arena)
        {

        }

        public override void Advance()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            this.unit.TriggerAttackAnimation();

            // TODO: need to get this from the data we receive from the server
            int level = 1;

            Chip c = GameDB.Instance.ChipsDB.GetChip(chipId);
            ChipData cd = c.data[level];

            GameObject projectile = GameObject.Instantiate(
                Resources.Load("TestStraightProjectile", typeof(GameObject))
            ) as GameObject;
            StraightProjectile p = projectile.GetComponent<StraightProjectile>();

            Vector3 position = this.unit.transform.position;
            Constants.Owner owner = this.unit.owner;


            p.Initialize(position, owner, this.unit.tilePos, cd.projectileSpeed, this.arenaRef);
        }

        public override bool IsComplete()
        {
            return true;
        }
    }
}
