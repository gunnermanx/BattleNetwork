using Sfs2X;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class BasicAttackCommandHandler : BaseCommandHandler
    {
        public BasicAttackCommandHandler(BattleManager battleManager) : base(battleManager) { }

        public override void Execute(ISFSArray cmd)
        {
            SmartFox sfs = SFSConnector.Instance.Connection;

            int playerId = cmd.GetInt(1);
            Debug.LogFormat("received basic attack cmd for player {0}", playerId);

            if (sfs.MySelf.PlayerId != playerId)
            {
                this.bm.Arena.ServerBasicAttack(playerId);
            }
        }

    }
}
