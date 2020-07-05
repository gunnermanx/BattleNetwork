using BattleNetwork.Characters;
using BattleNetwork.Events;
using UnityEngine;

namespace BattleNetwork.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private GameObject hpDisplayPrefab;

        [SerializeField] private ChipDockUI chipDockUI;
        [SerializeField] private EnergyBar energyBar;

        [SerializeField] private BattleConfigurationData battleConfig;

        [SerializeField] private PlayerUnitCreatedEventListener playerUnitCreatedEventListener;
        [SerializeField] private EnergyChangedEventListener energyChangedEventListener;
        [SerializeField] private DraggedUIEventListener draggedUIEventListener;
       
        private void Start()
        {
            energyChangedEventListener.energyChangedCallback += HandleEnergyChangedEvent;
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;
            playerUnitCreatedEventListener.playerCreatedCallback += HandlePlayerUnitCreatedEvent;

            energyBar.InitializeWithMaxAndInterval(battleConfig.maxEnergy, battleConfig.ticksPerEnergy);                       
        }

        private void HandleEnergyChangedEvent(int currentEnergy, float timeTillNextEnergySecs)
        {            
            energyBar.SetToValue(currentEnergy, timeTillNextEnergySecs);                                   
        }
        
        private void HandleDraggedUIEvent(DraggedUIEvent.State state, IDraggableUI draggable)
        {
            if (draggable.GetGameObject().GetComponent<ChipUI>() != null)
            {
                if (state == DraggedUIEvent.State.Started)
                {
                    chipDockUI.Minimize();
                }
                else if (state == DraggedUIEvent.State.Ended)
                {
                    chipDockUI.Maximize();
                }
            }            
        }

        private void HandlePlayerUnitCreatedEvent(PlayerUnit player)
        {
            Damageable damageable = player.gameObject.GetComponent<Damageable>();
            GameObject hpDisplay = GameObject.Instantiate(hpDisplayPrefab, transform);
            hpDisplay.GetComponent<UnitHPDisplay>()
                .AttachToIDamageable(damageable, new Vector2(0f, -20f));
        }
    }
}
