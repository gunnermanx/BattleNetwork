using UnityEngine;
using System.Collections.Generic;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class EnergyChangedEvent : ScriptableObject
    {
        private List<EnergyChangedEventListener> listeners = new List<EnergyChangedEventListener>();

        public void Raise(int currentTick, float timeTillNextEnergySecs)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(currentTick, timeTillNextEnergySecs);
            }
        }

        public void RegisterListener(EnergyChangedEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(EnergyChangedEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
