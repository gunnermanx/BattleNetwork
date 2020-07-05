using UnityEngine;
using DigitalRubyShared;

namespace BattleNetwork.Events
{
    public class TapGestureEventListener : MonoBehaviour
    {

        public delegate void TapGestureEventCallbackFunction(Vector2 screenPos);
        public event TapGestureEventCallbackFunction tapGestureEventCallback;
        public TapGestureEvent Event;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Vector2 screenPos)
        {
            tapGestureEventCallback?.Invoke(screenPos);
        }
    }
}
