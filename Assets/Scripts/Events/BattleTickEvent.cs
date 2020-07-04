using UnityEngine;
using System.Collections.Generic;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class BattleTickEvent : ScriptableObject
    {
        private List<BattleTickEventListener> listeners = new List<BattleTickEventListener>();

        public void Raise(int currentTick)
        {            
            for (int i = listeners.Count - 1; i >= 0;  i--)
            {                
                listeners[i].OnEventRaised(currentTick);
            }
        }

        public void RegisterListener(BattleTickEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(BattleTickEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
