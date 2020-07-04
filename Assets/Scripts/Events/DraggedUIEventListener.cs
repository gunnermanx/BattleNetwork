using BattleNetwork.Battle.UI;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNetwork.Events
{
    public class DraggedUIEventListener : MonoBehaviour
    {
        

        public delegate void DraggedUIEventCallbackFunction(DraggedUIEvent.State state, IDraggableUI draggedUI);
        public event DraggedUIEventCallbackFunction draggedUIEventCallback;
        public DraggedUIEvent Event;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(DraggedUIEvent.State state, IDraggableUI draggedUI)
        {
            draggedUIEventCallback?.Invoke(state, draggedUI);
        }
    }
}
