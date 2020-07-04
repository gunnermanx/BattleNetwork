using UnityEngine;
using UnityEngine.Events;

namespace BattleNetwork.Events
{
    public class EnergyChangedEventListener : MonoBehaviour
    {
        public delegate void EnergyChangedCallbackFunction(int currentEnergy, float timeTillNextEnergySecs);
        public event EnergyChangedCallbackFunction energyChangedCallback;
        public EnergyChangedEvent Event;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(int currentEnergy, float timeTillNextEnergySecs)
        {
            energyChangedCallback?.Invoke(currentEnergy, timeTillNextEnergySecs);
        }
    }
}
