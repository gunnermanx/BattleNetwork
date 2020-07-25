using Sfs2X.Entities.Data;

namespace BattleNetwork.Battle.ServerCommandHandlers
{
    
    public abstract class BaseCommandHandler
    {
        public static readonly byte MOVE_CMD_ID             = (byte)0;
        public static readonly byte DAMAGE_UNIT_CMD_ID      = (byte)1;
        public static readonly byte ENERGY_CHANGED_CMD_ID   = (byte)2;
        public static readonly byte CHIP_DRAWN_CMD_ID       = (byte)4;
        public static readonly byte CHIP_PLAYED_CMD_ID      = (byte)5;
        public static readonly byte BASIC_ATTACK_CMD_ID     = (byte)6;

        protected BattleManager bm;

        protected BaseCommandHandler(BattleManager bm) {
            this.bm = bm;
        }

        public abstract void Execute(ISFSArray cmdData);
    }
}
