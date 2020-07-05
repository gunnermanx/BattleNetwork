using BattleNetwork.Characters;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNetwork.Events
{
    public class PlayerUnitCreatedEventListener : MonoBehaviour
    {
        public delegate void PlayerCreatedCallbackFunction(PlayerUnit player);
        public event PlayerCreatedCallbackFunction playerCreatedCallback;
        public PlayerUnitCreatedEvent Event;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(PlayerUnit player)
        {
            playerCreatedCallback?.Invoke(player);
        }
    }
}
