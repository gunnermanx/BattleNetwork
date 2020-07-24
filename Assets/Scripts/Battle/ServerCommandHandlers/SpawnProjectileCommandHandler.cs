using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class SpawnProjectileCommandHandler : BaseCommandHandler
    {
        public SpawnProjectileCommandHandler(BattleManager battleManager) : base(battleManager) { }

        public override void Execute(ISFSArray cmd)
        {
            Debug.Log("received command to spawn projectile");

            int playerId = cmd.GetInt(1);
            int chipId = cmd.GetInt(2);

            this.bm.Arena.ServerSpawnedProjectile(playerId, chipId);
        }

    }
}
