using BattleNetwork.Events;
using Sfs2X;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    public class EnergyChangedCommandHandler : BaseCommandHandler
    {
        private EnergyChangedEvent energyChangedEvent;

        public EnergyChangedCommandHandler(BattleManager battleManager, EnergyChangedEvent energyChangedEvent) : base(battleManager) {
            this.energyChangedEvent = energyChangedEvent;
        }

        public override void Execute(ISFSArray cmd)
        {
            int playerId = cmd.GetInt(1);
            int deltaEnergy = cmd.GetInt(2);

            SmartFox sfs = SFSConnector.Instance.Connection;

            //Debug.LogFormat("energy changed event received for : %d", playerId);

            if (sfs.MySelf.PlayerId == playerId)
            {
                Debug.LogFormat("change in energy, previous {0}, after: {1}", this.bm.Energy, this.bm.Energy + deltaEnergy);
                this.bm.Energy = this.bm.Energy + deltaEnergy;
                energyChangedEvent.Raise(this.bm.Energy, BattleManager.TICKS_PER_ENERGY * BattleManager.TICK_TIME);
            }
        }
    }
}
