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
    public class BattleManager : MonoBehaviour, IPunObservable
    {
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject arenaPrefab;
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject playerPrefab;


        [SerializeField] private BattleUI battleUI;       

        [SerializeField] private BattleConfigurationData battleConfig;

        // Events
        [SerializeField] private BattleTickEvent battleTickEvent;
        [SerializeField] private EnergyChangedEvent energyChangedEvent;

        private DraggedUIEventListener draggedUIEventListener;
        private SwipeGestureEventListener swipeGestureEventListener;
        private TapGestureEventListener tapGestureEventListener;        

        private int currentTick = -1;
        private int sentTick = -1;
        private int energy = 0;

        private readonly float tickTime = 0.5f;

        private Arena arena;

        private PlayerUnit localPlayerUnit;        
        
        private void Start()
        {
            draggedUIEventListener = gameObject.GetComponent<DraggedUIEventListener>();
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;

            swipeGestureEventListener = gameObject.GetComponent<SwipeGestureEventListener>();
            swipeGestureEventListener.swipeGestureEventCallback += HandleSwipeGestureEvent;

            tapGestureEventListener = gameObject.GetComponent<TapGestureEventListener>();
            tapGestureEventListener.tapGestureEventCallback += HandleTapGestureEvent;
        }

        public void StartBattle()
        {            
            CreateArena();
            CreatePlayer();
            CreateUI();

            // only the master client will update the tick
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating(nameof(MasterClientTick), 0f, tickTime);
            }            
        }

        private void CreateUI()
        {
            // tODO later create the battle ui dynamically maybe
            
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


        // Only the master client updates the tick
        private void MasterClientTick()
        {
            currentTick++;
            HandleTickUpdated(currentTick);
        }

        private void HandleTickUpdated(int tick)
        {
            UpdateEnergy();
            battleTickEvent.Raise(currentTick);
        }

        private void UpdateEnergy()
        {
            if (currentTick == 0)
            {
                energyChangedEvent.Raise(energy, (float)battleConfig.ticksPerEnergy * tickTime);
            }
            else if (currentTick % battleConfig.ticksPerEnergy == 0)
            {
                int newEnergy = energy + 1;
                newEnergy = Math.Min(battleConfig.maxEnergy, newEnergy);
                if (newEnergy != energy)
                {
                    energy = newEnergy;
                    energyChangedEvent.Raise(energy, (float)battleConfig.ticksPerEnergy * tickTime);
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

        private void HandleTapGestureEvent(Vector2 screenPos)
        {
            localPlayerUnit.BasicAttack();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (currentTick != sentTick)
                {
                    //Debug.LogFormat("sending tick: {0}", currentTick);
                    stream.SendNext(currentTick);
                    sentTick = currentTick;
                }                
            } else
            {
                currentTick = (int)stream.ReceiveNext();
                //Debug.LogFormat("receiving tick: {0}", currentTick);
                HandleTickUpdated(currentTick);
            }
        }

    }
}
