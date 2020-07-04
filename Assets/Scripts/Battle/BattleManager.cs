using UnityEngine;
using Photon.Pun;
using BattleNetwork.Characters;
using BattleNetwork.Events;
using System;
using DigitalRubyShared;
using BattleNetwork.Battle.UI;

namespace BattleNetwork.Battle
{

    [RequireComponent(typeof(DraggedUIEventListener))]
    public class BattleManager : MonoBehaviour
    {
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject arenaPrefab;
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject playerPrefab;


        [SerializeField] private BattleConfigurationData battleConfig;

        // Events
        [SerializeField] private BattleTickEvent battleTickEvent;
        [SerializeField] private EnergyChangedEvent energyChangedEvent;

        private DraggedUIEventListener draggedUIEventListener;
        private SwipeGestureEventListener swipeGestureEventListener;


        private int currentTick;
        private int energy = 0;

        private Arena arena;

        private PlayerUnit localPlayerUnit;        
        
        private void Start()
        {

            draggedUIEventListener = gameObject.GetComponent<DraggedUIEventListener>();
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;

            swipeGestureEventListener = gameObject.GetComponent<SwipeGestureEventListener>();
            swipeGestureEventListener.swipeGestureEventCallback += HandleSwipeGestureEvent;
        }

        public void StartBattle()
        {
            CreateUI();
            CreateArena();
            CreatePlayer();
        
            SetupRepeatingTick();  
        }

        private void CreateUI()
        {

        }

        private void CreateArena()
        {
            GameObject arenaGO = GameObject.Instantiate(arenaPrefab);
            arena = arenaGO.GetComponent<Arena>();
            arena.Initialize();
        }

        private void CreatePlayer()
        {
            if (localPlayerUnit == null)
            {              
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                GameObject localPlayerUnitGO = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);

                // "Place" the Player in the arena
                Constants.Owner owner = PhotonNetwork.IsMasterClient ? Constants.Owner.Player1 : Constants.Owner.Player2;
                localPlayerUnit = localPlayerUnitGO.GetComponent<PlayerUnit>();
                arena.PlacePlayerUnit(localPlayerUnit, owner);
            }
        }

        private void SetupRepeatingTick()
        {
            currentTick = 0;
            InvokeRepeating(nameof(Tick), 0f, 0.25f);
        }

        // TODO : lets have master client update this only
        private void Tick()
        {
            UpdateEnergy();

            currentTick++;
            battleTickEvent.Raise(currentTick);

            //Debug.LogFormat("Current Tick: {0}", tick);
        }

        private void UpdateEnergy()
        {
            if (currentTick == 0)
            {
                energyChangedEvent.Raise(energy, (float)battleConfig.ticksPerEnergy * 0.25f);
            }
            else if (currentTick % battleConfig.ticksPerEnergy == 0)
            {
                int newEnergy = energy + 1;
                newEnergy = Math.Min(battleConfig.maxEnergy, newEnergy);
                if (newEnergy != energy)
                {
                    energy = newEnergy;
                    energyChangedEvent.Raise(energy, (float)battleConfig.ticksPerEnergy * 0.25f);
                }
            }
        } 
        
        private void HandleDraggedUIEvent(DraggedUIEvent.State state, IDraggableUI draggable)
        {
            if (state == DraggedUIEvent.State.Started)
            {
                
            }

            if (state == DraggedUIEvent.State.Ended)
            {
                // TODO check and possibly play chips
            }            
        }

        private void HandleSwipeGestureEvent(SwipeGestureRecognizerDirection direction)
        {
            switch (direction)
            {
                case SwipeGestureRecognizerDirection.Up:
                    arena.TryMoveUp(localPlayerUnit);
                    break;
                case SwipeGestureRecognizerDirection.Down:
                    arena.TryMoveDown(localPlayerUnit);
                    break;
                case SwipeGestureRecognizerDirection.Left:
                    arena.TryMoveLeft(localPlayerUnit);
                    break;
                case SwipeGestureRecognizerDirection.Right:
                    arena.TryMoveRight(localPlayerUnit);
                    break;
            }
        }
    }
}
