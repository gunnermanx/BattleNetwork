using UnityEngine;
using BattleNetwork.Characters;
using System.Collections.Generic;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class PlayerUnitCreatedEvent : ScriptableObject
    {
        private List<PlayerUnitCreatedEventListener> listeners = new List<PlayerUnitCreatedEventListener>();

        public void Raise(PlayerUnit player)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(player);
            }
        }

        public void RegisterListener(PlayerUnitCreatedEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(PlayerUnitCreatedEventListener listener)
        {
            listeners.Remove(listener);
        }
    }

}
