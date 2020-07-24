using Sfs2X;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class ChipPlayedCommandHandler : BaseCommandHandler
    {
        public ChipPlayedCommandHandler(BattleManager battleManager) : base(battleManager) { }

        public override void Execute(ISFSArray cmd)
        {
            SmartFox sfs = SFSConnector.Instance.Connection;

            int playerId = cmd.GetInt(1);
            short chipId = cmd.GetShort(2);

            this.bm.Arena.ServerPlayedChip(playerId, chipId);
        }

    }
}
