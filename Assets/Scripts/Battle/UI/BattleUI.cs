using BattleNetwork.Characters;
using BattleNetwork.Events;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        private static readonly int RANK_OVERLAY_TICK = 0;
        private static readonly int PLAYERS_OVERLAY_TICK = BattleManager.ROUND_START_TICK - 60;
        private static readonly int FIGHT_OVERLAY_TICK = BattleManager.ROUND_START_TICK - 20;
        
        // UI Elements
        [SerializeField] private GameObject hpDisplayPrefab;
        [SerializeField] private ChipDockUI chipDockUI;
        [SerializeField] private EnergyBar energyBar;
        [SerializeField] private Transform dyanmicUIParent;
        [SerializeField] private PlayerNameplate localPlayerNameplate;
        [SerializeField] private PlayerNameplate remotePlayerNameplate;
        [SerializeField] private Text timerText;

        // Round starting overlays
        [SerializeField] private GameObject RankOverlay;
        [SerializeField] private GameObject PlayersOverlay;
        [SerializeField] private GameObject FightOverlay;

        // Event Listeners
        [SerializeField] private PlayerUnitCreatedEventListener playerUnitCreatedEventListener;
        [SerializeField] private EnergyChangedEventListener energyChangedEventListener;
        [SerializeField] private DraggedUIEventListener draggedUIEventListener;

        private bool rankOverlayShown = false;
        private bool playersOverlayShown = false;
        private bool fightOverlayShown = false;

        private int timerTick = 0;
        private int timer = BattleManager.ROUND_DURATION_SECONDS;

        private Animator cachedAnimator; 

        private void Start()
        {
            timerText.text = timer.ToString();

            energyChangedEventListener.energyChangedCallback += HandleEnergyChangedEvent;
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;
            playerUnitCreatedEventListener.playerCreatedCallback += HandlePlayerUnitCreatedEvent;

            energyBar.InitializeWithMaxAndInterval(0, 6);

            cachedAnimator = gameObject.GetComponent<Animator>();
        }

        public void HandleTick(int currentTick)
        {
            if (!rankOverlayShown && currentTick < PLAYERS_OVERLAY_TICK && currentTick >= RANK_OVERLAY_TICK)
            {
                rankOverlayShown = true;
                RankOverlay.SetActive(true);
            }
            else if (!playersOverlayShown && currentTick < FIGHT_OVERLAY_TICK && currentTick >= PLAYERS_OVERLAY_TICK)
            {
                playersOverlayShown = true;
                RankOverlay.SetActive(false);
                PlayersOverlay.SetActive(true);
            }
            else if (!fightOverlayShown && currentTick < BattleManager.ROUND_START_TICK && currentTick >= FIGHT_OVERLAY_TICK)
            {
                fightOverlayShown = true;
                PlayersOverlay.SetActive(false);
                FightOverlay.SetActive(true);
                cachedAnimator.SetTrigger("start");
            }
            else if (currentTick == BattleManager.ROUND_START_TICK)
            {
                FightOverlay.SetActive(false);
                timerTick = 0;
            }
            else if (currentTick > BattleManager.ROUND_START_TICK)
            {
                timerTick++;
                if (timerTick % BattleManager.TICKS_PER_SECOND == 0)
                {
                    timer--;
                    timerText.text = timer.ToString();
                }
            }


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
                    //chipDockUI.Minimize();
                }
                else if (state == DraggedUIEvent.State.Ended)
                {
                    //chipDockUI.Maximize();

                    
                }
            }            
        }

        private void HandlePlayerUnitCreatedEvent(PlayerUnit player)
        {
            Damageable damageable = player.gameObject.GetComponent<Damageable>();
            GameObject hpDisplay = GameObject.Instantiate(hpDisplayPrefab, dyanmicUIParent);
            hpDisplay.GetComponent<UnitHPDisplay>()
                .AttachToIDamageable(damageable, new Vector3(0f, -0.3f, 0f));
        }

        public int GetChipDataForIndex(int i)
        {
            return chipDockUI.GetChipDataForIndex(i);
        }

        public void InitializeHand(short[] chipIds, short nextChip)
        {
            chipDockUI.InitializeChipDockUI(chipIds.Length);
            for (int i = 0; i < chipIds.Length; i++)
            {
                Debug.LogFormat("Adding chip with id: {0}", chipIds[i]);
                chipDockUI.AddChip(i, chipIds[i]);
            }
            chipDockUI.SetNextChipPreview(nextChip);
        }

        public void ChipPlayedAtIndex(int i)
        {
            chipDockUI.RemoveChipAt(i);
        }

        public void AddChipAtLastRemoved(short chipId, short nextChipId)
        {
            chipDockUI.AddChipAtLastIndex(chipId);
            chipDockUI.SetNextChipPreview(nextChipId);
        }





    }
}
