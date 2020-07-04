using UnityEngine;
using DigitalRubyShared;

namespace BattleNetwork.Events
{
    public class SwipeGestureEventListener : MonoBehaviour
    {

        public delegate void SwipeGestureEventCallbackFunction(SwipeGestureRecognizerDirection direction);
        public event SwipeGestureEventCallbackFunction swipeGestureEventCallback;
        public SwipeGestureEvent Event;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(SwipeGestureRecognizerDirection direction)
        {
            swipeGestureEventCallback?.Invoke(direction);
        }
    }
}
