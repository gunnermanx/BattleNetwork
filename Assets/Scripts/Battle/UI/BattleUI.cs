using BattleNetwork.Events;
using UnityEngine;

namespace BattleNetwork.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private ChipDockUI chipDockUI;
        [SerializeField] private EnergyBar energyBar;

        [SerializeField] private BattleConfigurationData battleConfig;

        [SerializeField] private EnergyChangedEventListener energyChangedEventListener;
        [SerializeField] private DraggedUIEventListener draggedUIEventListener;
       
        private void Start()
        {
            energyChangedEventListener.energyChangedCallback += HandleEnergyChangedEvent;
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;

            energyBar.InitializeWithMaxAndInterval(battleConfig.maxEnergy, battleConfig.ticksPerEnergy);                       
        }

        private void HandleEnergyChangedEvent(int currentEnergy, float timeTillNextEnergySecs)
        {
            Debug.LogFormat("handleEnergyChangedEvent {0}", currentEnergy);
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
    }
}
