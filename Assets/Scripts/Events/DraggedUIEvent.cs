using UnityEngine;
using System.Collections.Generic;
using BattleNetwork.Battle.UI;

namespace BattleNetwork.Events
{

    [CreateAssetMenu]
    public class DraggedUIEvent : ScriptableObject
    {
        public enum State
        {
            Started,
            Dragging,
            Ended,
        }

        private List<DraggedUIEventListener> listeners = new List<DraggedUIEventListener>();

        public void Raise(DraggedUIEvent.State state, IDraggableUI draggedUI)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(state, draggedUI);
            }
        }

        public void RegisterListener(DraggedUIEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(DraggedUIEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
