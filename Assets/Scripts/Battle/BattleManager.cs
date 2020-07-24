﻿using UnityEngine;
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

        [SerializeField] private CameraController cameraController;


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

        private bool gameStarted;

        private void Update()
        {
            if (currentTick + 1 <= serverTick)
            {
                currentTick++;
                battleTickEvent.Raise(currentTick);

                // figure out better way later to init energy bar value + movement
                if (currentTick == 1)
                {
                    gameStarted = true;
                    energyChangedEvent.Raise(energy, ticksPerEnergy * tickTime);
                    readyInfoOverlay.SetActive(false);
                }
            }
        }

        public void InitializeBattle()
        {
            sfs = SFSConnector.Instance.Connection;
            if (sfs != null)
            {
                sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            } else
            {
                //throw Exception("sfs not connected?");
            }

            cameraController.SetPlayer(sfs.MySelf.PlayerId);

            // send a message to the server to let it know we are ready to receive messages
            SFSObject obj = new SFSObject();
            sfs.Send(new ExtensionRequest("pr", obj, sfs.LastJoinedRoom));

            draggedUIEventListener = gameObject.GetComponent<DraggedUIEventListener>();
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;

            swipeGestureEventListener = gameObject.GetComponent<SwipeGestureEventListener>();
            swipeGestureEventListener.swipeGestureEventCallback += HandleSwipeGestureEvent;

            tapGestureEventListener = gameObject.GetComponent<TapGestureEventListener>();
            tapGestureEventListener.tapGestureEventCallback += HandleTapGestureEvent;

            CreateArena();
            CreatePlayerUnits();

            loadingScreen.SetActive(false);
        }
        
        private void CreateArena()
        {
            GameObject arenaGO = GameObject.Instantiate(arenaPrefab);
            arena = arenaGO.GetComponent<Arena>();
            arena.Initialize();
        }

        private void CreatePlayerUnits()
        {
            // clients should already have data about starting positions and player unit data...
            // do something here            
            GameObject p1PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p1PlayerUnit = p1PlayerUnitGO.GetComponent<PlayerUnit>();
            p1PlayerUnit.id = p1PlayerUnitId;
            p1PlayerUnit.SetFacingLeft(false);
            arena.PlacePlayerUnit(p1PlayerUnit, Constants.Owner.Player1);

            GameObject p2PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p2PlayerUnit = p2PlayerUnitGO.GetComponent<PlayerUnit>();
            p2PlayerUnit.id = p2PlayerUnitId;
            p2PlayerUnit.SetFacingLeft(true);
            arena.PlacePlayerUnit(p2PlayerUnit, Constants.Owner.Player2);
        }

        // ======================================================================================================
        //  Gesture Handlers
        // ======================================================================================================
        private void HandleDraggedUIEvent(DraggedUIEvent.State state, IDraggableUI draggable)
        {
            if (!this.gameStarted) return;

            if (state == DraggedUIEvent.State.Started)
            {

            }

            if (state == DraggedUIEvent.State.Ended)
            {
                // TEMP
                if (energy >= 2)
                {
                    // TODO check and possibly play chips
                    ChipUI chipUI = draggable.GetGameObject().GetComponent<ChipUI>();
                    SFSObject obj = new SFSObject();
                    obj.PutShort("cid", chipUI.cid);
                    Debug.LogFormat("chip played {0}", chipUI.cid);
                    sfs.Send(new ExtensionRequest("ch", obj, sfs.LastJoinedRoom));

                    battleUI.ChipPlayedAtIndex(chipUI.Index);
                }
            }
        }

        private void HandleSwipeGestureEvent(SwipeGestureRecognizerDirection direction)
        {
            if (!this.gameStarted) return;

            byte dir = DirectionToByteDir(direction);
            if (dir != (byte)'0')
            {
                if (currentTick >= 1)
                {
                    if (IsPlayer1())
                    {
                        arena.TryMove(p1PlayerUnit, dir);
                    }
                    else
                    {
                        arena.TryMove(p2PlayerUnit, dir);
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
            if (!this.gameStarted) return;

            if (IsPlayer1())
            {
                if (p1PlayerUnit.TryBasicAttack(currentTick)) {
                    SFSObject obj = new SFSObject();
                    sfs.Send(new ExtensionRequest("ba", obj, sfs.LastJoinedRoom));
                    p1PlayerUnit.TriggerAttackAnimation();
                }                
            }
            else
            {
                if (p2PlayerUnit.TryBasicAttack(currentTick))
                {
                    SFSObject obj = new SFSObject();
                    sfs.Send(new ExtensionRequest("ba", obj, sfs.LastJoinedRoom));
                    p2PlayerUnit.TriggerAttackAnimation();
                }                
            }
        }

        

        // ======================================================================================================
        //  SmartFox Message Handlers
        // ======================================================================================================    

        public void OnExtensionResponse(BaseEvent evt)
        {
            string cmd = (string)evt.Params["cmd"];
            SFSObject dataObject = (SFSObject)evt.Params["params"];

            switch (cmd)
            {
                case "tick":
                    CommandsReceived(dataObject);
                    break;
                case "hand":
                    

                    ISFSArray chips = dataObject.GetSFSArray("chips");
                    short[] chipsArr = new short[4];
                    chipsArr[0] = chips.GetShort(0);
                    chipsArr[1] = chips.GetShort(1);
                    chipsArr[2] = chips.GetShort(2);
                    chipsArr[3] = chips.GetShort(3);
                    short nextChip = dataObject.GetShort("next");

                    Debug.LogFormat("Received initial hand! [{0},{1},{2},{3}], next: {4}", chipsArr[0], chipsArr[1], chipsArr[2], chipsArr[3], nextChip);

                    battleUI.InitializeHand(chipsArr, nextChip);
                    break;
                case "pv":
                    gameStarted = false;
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

            //if (cmds.Count > 0)
            //  Debug.LogFormat("At tick {0}, received {1} cmds", latestTick, cmds.Count);

            foreach (ISFSArray cmd in cmds)
            {
                byte cmdId = cmd.GetByte(0);
                // create a function to multiplex to different parsers

                // move cmd
                if (cmdId == (byte) 0)
                {
                    int unitId = cmd.GetInt(1);
                    int x = cmd.GetInt(2);
                    int y = cmd.GetInt(3);

                    arena.ServerMoveUnit(sfs.MySelf.PlayerId, unitId, x, y);

                    

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

                    arena.ServerSpawnedProjectile(playerId, chipId);
                }
                // chip drawn event
                else if (cmdId == (byte) 4)
                {
                    Debug.Log("chip drawn");

                    int playerId = cmd.GetInt(1);
                    short chipId = cmd.GetShort(2);
                    short nextChipId = cmd.GetShort(3);

                    if (playerId == sfs.MySelf.PlayerId)
                    {
                        battleUI.AddChipAtLastRemoved(chipId, nextChipId);
                    }
                    
                }
                // chip played event
                else if (cmdId == (byte) 5)
                {
                    int playerId = cmd.GetInt(1);
                    short chipId = cmd.GetShort(2);

                    arena.ServerPlayedChip(playerId, chipId);
                }
                // player basic attack
                else if (cmdId == (byte) 6)
                {
                    int playerId = cmd.GetInt(1);
                    Debug.LogFormat("received basic attack cmd for player {0}", playerId);

                    if (sfs.MySelf.PlayerId != playerId)
                    {
                        arena.ServerBasicAttack(playerId);
                    }
                }
            }
            
        }

        // ======================================================================================================
        //  Helpers
        // ======================================================================================================

        private bool IsPlayer1()
        {
            if(sfs.MySelf.PlayerId == 1)
            {
                return true;
            }
            return false;
        }

        public void TEMPFinishGame()
        {
            // TEMP NEED TO REFACTOR
            sfs.RemoveAllEventListeners();
            sfs.Send(new LeaveRoomRequest());
            SceneManager.LoadScene("Home");
        }

        private byte DirectionToByteDir(SwipeGestureRecognizerDirection direction)
        {
            byte dir;
            switch (direction)
            {
                case SwipeGestureRecognizerDirection.Up:
                    dir = (sfs.MySelf.PlayerId == 1) ? (byte)'u' : (byte)'d';
                    break;
                case SwipeGestureRecognizerDirection.Down:
                    dir = (sfs.MySelf.PlayerId == 1) ? (byte)'d' : (byte)'u';
                    break;
                case SwipeGestureRecognizerDirection.Left:
                    dir = (sfs.MySelf.PlayerId == 1) ? (byte)'l' : (byte)'r';
                    break;
                case SwipeGestureRecognizerDirection.Right:
                    dir = (sfs.MySelf.PlayerId == 1) ? (byte)'r' : (byte)'l';
                    break;
                default:
                    dir = (byte)'0';
                    break;
            }

            return dir;
        }
    }
}
