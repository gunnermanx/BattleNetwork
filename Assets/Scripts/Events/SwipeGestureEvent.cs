using UnityEngine;
using System.Collections.Generic;
using DigitalRubyShared;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class SwipeGestureEvent : ScriptableObject
    {
        private List<SwipeGestureEventListener> listeners = new List<SwipeGestureEventListener>();

        public void Raise(SwipeGestureRecognizerDirection direction)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(direction);
            }
        }

        public void RegisterListener(SwipeGestureEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(SwipeGestureEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
