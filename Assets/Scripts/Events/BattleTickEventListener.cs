using UnityEngine;
using UnityEngine.Events;

namespace BattleNetwork.Events
{
    public class BattleTickEventListener : MonoBehaviour
    {
        public delegate void TickCallbackFunction(int currentTick);
        public event TickCallbackFunction tickCallback;
        public BattleTickEvent Event;
        

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(int currentTick)
        {
            tickCallback?.Invoke(currentTick);
        }
    }
}
