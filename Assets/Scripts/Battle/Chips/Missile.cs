using BattleNetwork.Characters;
using System;
using UnityEngine;

namespace BattleNetwork.Battle.Chips
{
    public class Missile : BaseChip
    {
        public Missile(PlayerUnit unit, int playerId, short chipId) : base(unit, playerId, chipId)
        {

        }

        public override void Advance()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            this.unit.TriggerAttackAnimation();

            GameObject projectile = GameObject.Instantiate(
                Resources.Load("TestStraightProjectile", typeof(GameObject))
            ) as GameObject;
            StraightProjectile p = projectile.GetComponent<StraightProjectile>();

            Vector3 position = this.unit.transform.position;
            Constants.Owner owner = this.unit.owner;

            p.Initialize(position, owner);
        }

        public override bool IsComplete()
        {
            return true;
        }
    }
}
