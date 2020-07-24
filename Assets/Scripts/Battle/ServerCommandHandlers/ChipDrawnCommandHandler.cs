using Sfs2X;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class ChipDrawnCommandHandler : BaseCommandHandler
    {
        public ChipDrawnCommandHandler(BattleManager battleManager) : base(battleManager) { }

        public override void Execute(ISFSArray cmd)
        {
            SmartFox sfs = SFSConnector.Instance.Connection;

            int playerId = cmd.GetInt(1);
            short chipId = cmd.GetShort(2);
            short nextChipId = cmd.GetShort(3);

            if (playerId == sfs.MySelf.PlayerId)
            {
                this.bm.battleUI.AddChipAtLastRemoved(chipId, nextChipId);
            }
        }

    }
}
