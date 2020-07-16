using UnityEngine;
using BattleNetwork.Characters;
using BattleNetwork.Events;
using System;
using DigitalRubyShared;
using BattleNetwork.Battle.UI;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Core;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BattleNetwork.Battle
{

    [RequireComponent(typeof(DraggedUIEventListener))]
    public class BattleManager : MonoBehaviour
    {
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject arenaPrefab;
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject playerPrefab;


        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject readyInfoOverlay;

        [SerializeField] private BattleUI battleUI;

        [SerializeField] private ResultScreen resultScreen;

        [SerializeField] private BattleConfigurationData battleConfig;

        // Events
        [SerializeField] private BattleTickEvent battleTickEvent;
        [SerializeField] private EnergyChangedEvent energyChangedEvent;

        private DraggedUIEventListener draggedUIEventListener;
        private SwipeGestureEventListener swipeGestureEventListener;
        private TapGestureEventListener tapGestureEventListener;

        private int currentTick = 0;
        private int serverTick = 0;

        private int energy = 0;

        private readonly float tickTime = 0.05f;

        private Arena arena;

        private PlayerUnit p1PlayerUnit;
        private PlayerUnit p2PlayerUnit;

        private readonly int p1PlayerUnitId = 1;
        private readonly int p2PlayerUnitId = 2;


        // TEST
        private int ticksPerEnergy = 40;

        private SmartFox sfs;

        public void InitializeBattle()
        {
            draggedUIEventListener = gameObject.GetComponent<DraggedUIEventListener>();
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;

            swipeGestureEventListener = gameObject.GetComponent<SwipeGestureEventListener>();
            swipeGestureEventListener.swipeGestureEventCallback += HandleSwipeGestureEvent;

            tapGestureEventListener = gameObject.GetComponent<TapGestureEventListener>();
            tapGestureEventListener.tapGestureEventCallback += HandleTapGestureEvent;

            sfs = SFSConnector.Instance.Connection;
            if (sfs != null)
            {
                sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            }            

            CreateArena();
            CreatePlayerUnits();
            CreateDeck();
            CreateUI();

            StartCoroutine(FakeLoad());

            //sfs.Send(new ExtensionRequest("r", new SFSObject(), sfs.LastJoinedRoom));
        }
        private IEnumerator FakeLoad()
        {
            yield return new WaitForSeconds(1.0f);
            loadingScreen.SetActive(false);
        }

        private void CreateArena()
        {
            GameObject arenaGO = GameObject.Instantiate(arenaPrefab);
            arena = arenaGO.GetComponent<Arena>();
            arena.Initialize();
        }


        private void CreateDeck()
        {

        }


        private void CreateUI()
        {
            // tODO later create the battle ui dynamically maybe

        }

        private void CreatePlayerUnits()
        {
            // clients should already have data about starting positions and player unit data...
            // do something here            
            GameObject p1PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p1PlayerUnit = p1PlayerUnitGO.GetComponent<PlayerUnit>();
            p1PlayerUnit.id = p1PlayerUnitId;
            arena.PlacePlayerUnit(p1PlayerUnit, Constants.Owner.Player1);

            GameObject p2PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p2PlayerUnit = p2PlayerUnitGO.GetComponent<PlayerUnit>();
            p2PlayerUnit.id = p2PlayerUnitId;
            arena.PlacePlayerUnit(p2PlayerUnit, Constants.Owner.Player2);
        }

        private void Update()
        {
            if (currentTick + 1 <= serverTick)
            {
                currentTick++;
                battleTickEvent.Raise(currentTick);

                // figure out better way later to init energy bar value + movement
                if (currentTick == 1)
                {
                    energyChangedEvent.Raise(energy, ticksPerEnergy * tickTime);
                    readyInfoOverlay.SetActive(false);
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
                Debug.Log("chip played");


                // TODO check and possibly play chips
                ChipUI chipUI = draggable.GetGameObject().GetComponent<ChipUI>();
                int temp_data = battleUI.GetChipDataForIndex(chipUI.Index);

                // todo check costs

                // cool? lets tell battleUI to cycle chip

                // test cid
                int cid = 0;

                SFSObject obj = new SFSObject();
                obj.PutInt("cid", cid);
                sfs.Send(new ExtensionRequest("ch", obj, sfs.LastJoinedRoom));
            }
        }

        public void TEMPFinishGame()
        {
            // TEMP NEED TO REFACTOR
            sfs.RemoveAllEventListeners();
            sfs.Send(new LeaveRoomRequest());
            SceneManager.LoadScene("Home");
        }

        private void HandleSwipeGestureEvent(SwipeGestureRecognizerDirection direction)
        {
            byte dir;
            switch (direction)
            {
                case SwipeGestureRecognizerDirection.Up:
                    dir = (byte)'u';                    
                    break;
                case SwipeGestureRecognizerDirection.Down:
                    dir = (byte)'d';                    
                    break;
                case SwipeGestureRecognizerDirection.Left:
                    dir = (byte)'l';                    
                    break;
                case SwipeGestureRecognizerDirection.Right:
                    dir = (byte)'r';
                    break;
                default:
                    dir = (byte)'0';
                    break;
            }

            if (dir != (byte)'0')
            {
                if (currentTick >= 1)
                {
                    if (IsPlayer1())
                    {
                        arena.TryMove(p1PlayerUnit, direction);
                    }
                    else
                    {
                        arena.TryMove(p2PlayerUnit, direction);
                    }
                }                

                //Debug.LogFormat("Sending swipe {0}", direction);
                SFSObject obj = new SFSObject();                
                obj.PutByte("d", dir);
                sfs.Send(new ExtensionRequest("m", obj, sfs.LastJoinedRoom));
            }
        }

        private void HandleTapGestureEvent(Vector2 screenPos)
        {
            SFSObject obj = new SFSObject();
            sfs.Send(new ExtensionRequest("ba", obj, sfs.LastJoinedRoom));
        }

        public void OnExtensionResponse(BaseEvent evt)
        {
            string cmd = (string)evt.Params["cmd"];
            SFSObject dataObject = (SFSObject)evt.Params["params"];

            switch (cmd)
            {
                case "tick":
                    CommandsReceived(dataObject);
                    break;
                case "pv":
                    int winner = dataObject.GetInt("pid");
                    if (winner == sfs.MySelf.PlayerId)
                    {
                        resultScreen.SetWon();
                    } else
                    {
                        resultScreen.SetLost();
                    }                    
                    break;
            }
        }

        private void CommandsReceived(SFSObject dataObject)
        {
            int latestTick = dataObject.GetInt("t");
            serverTick = latestTick;

            //Debug.LogFormat("tick update: {0}", serverTick);

            ISFSArray cmds = dataObject.GetSFSArray("c");

            if (cmds.Count > 0)
                Debug.LogFormat("At tick {0}, received {1} cmds", latestTick, cmds.Count);

            foreach (ISFSArray cmd in cmds)
            {
                byte cmdId = cmd.GetByte(0);
                // create a function to multiplex to different parsers

                //Debug.LogFormat("    processing cmd with id == {0}", cmdId);

                // move cmd
                if (cmdId == (byte) 0)
                {
                    int unitId = cmd.GetInt(1);
                    int x = cmd.GetInt(2);
                    int y = cmd.GetInt(3);

                    arena.ServerMoveUnit(unitId, x, y);

                    Debug.LogFormat("    received move command from server at tick {0}: move {1} to [{2},{3}]", latestTick, unitId, x, y);
                }
                // damage dealt cmd
                else if (cmdId == (byte) 1)
                {
                    int unitId = cmd.GetInt(1);
                    int damage = cmd.GetInt(2);

                    Debug.LogFormat("unit {0} was damaged for {1}", unitId, damage);

                    arena.ServerDamageUnit(unitId, damage);
                }
                // energy changed event
                else if (cmdId == (byte) 2)
                {
                    int playerId = cmd.GetInt(1);
                    int deltaEnergy = cmd.GetInt(2);

                    //Debug.LogFormat("energy changed event received for : %d", playerId);

                    if (sfs.MySelf.PlayerId == playerId)
                    {
                        Debug.LogFormat("change in energy, previous {0}, after: {1}", energy, energy + deltaEnergy);
                        energy = energy + deltaEnergy;
                        energyChangedEvent.Raise(energy, ticksPerEnergy * tickTime);
                    }                    
                }
                // spawn projectile event
                else if (cmdId == (byte) 3)
                {
                    Debug.Log("received command to spawn projectile");

                    int playerId = cmd.GetInt(1);
                    int chipId = cmd.GetInt(2);

                    arena.PlayChip(playerId, chipId);
                }                
            }
            
        }

        private bool IsPlayer1()
        {
            if(sfs.MySelf.PlayerId == 1)
            {
                return true;
            }
            return false;
        }
    }
}
