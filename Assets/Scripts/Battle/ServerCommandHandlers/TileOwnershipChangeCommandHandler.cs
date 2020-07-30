using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class TileOwnershipChangeCommandHandler : BaseCommandHandler
    {
        public TileOwnershipChangeCommandHandler(BattleManager battleManager) : base(battleManager) { }

        public override void Execute(ISFSArray cmd)
        {
            Debug.Log("EXECUTEEEEE");

            int playerId = cmd.GetInt(1);
            int x = cmd.GetInt(2);
            int y = cmd.GetInt(3);

            this.bm.Arena.ServerTileOwnershipChange(playerId, x, y);                      
        }

    }
}
