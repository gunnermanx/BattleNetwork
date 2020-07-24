using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class MoveCommandHandler : BaseCommandHandler
    {
        public MoveCommandHandler(BattleManager battleManager) : base(battleManager) { }
       
        public override void Execute(ISFSArray cmd)
        {
            int unitId = cmd.GetInt(1);
            int x = cmd.GetInt(2);
            int y = cmd.GetInt(3);

            this.bm.Arena.ServerMoveUnit(unitId, x, y);
            //Debug.LogFormat("    received move command from server at tick {0}: move {1} to [{2},{3}]", latestTick, unitId, x, y);            
        }

    }
}
