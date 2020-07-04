using UnityEngine;

namespace BattleNetwork.Battle
{
    [CreateAssetMenu]
    public class BattleConfigurationData : ScriptableObject
    {
        public int ticksPerEnergy;
        public int maxEnergy;
    }
}
