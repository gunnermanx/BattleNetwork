using UnityEngine;
using System.Collections.Generic;
using DigitalRubyShared;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class TapGestureEvent : ScriptableObject
    {
        private List<TapGestureEventListener> listeners = new List<TapGestureEventListener>();

        public void Raise(Vector2 screenPos)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(screenPos);
            }
        }

        public void RegisterListener(TapGestureEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(TapGestureEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
