using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class DamageUnitCommandHandler : BaseCommandHandler
    {
        public DamageUnitCommandHandler(BattleManager battleManager) : base(battleManager) {}

        public override void Execute(ISFSArray cmd)
        {
            int unitId = cmd.GetInt(1);
            int damage = cmd.GetInt(2);

            Debug.LogFormat("unit {0} was damaged for {1}", unitId, damage);

            this.bm.Arena.ServerDamageUnit(unitId, damage);
        }
    }
}
