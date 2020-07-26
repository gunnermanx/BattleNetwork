using UnityEngine;
using BattleNetwork.Characters;
using BattleNetwork.Events;

using DigitalRubyShared;
using BattleNetwork.Battle.UI;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Core;

using UnityEngine.SceneManagement;
using BattleNetwork.Battle.ServerCommandHandlers;
using System.Collections.Generic;
using BattleNetwork.Data;

namespace BattleNetwork.Battle
{

    [RequireComponent(typeof(DraggedUIEventListener))]
    public class BattleManager : MonoBehaviour
    {
        private static readonly int P1_PLAYER_UNIT_ID = 1;
        private static readonly int P2_PLAYER_UNIT_ID = 2;

        // Values ripped from server code essentially.
        // 50ms tick time => 20hz game ticker
        public static readonly int TICKS_PER_SECOND = 20;
        public static readonly int INTERVAL_MS = 1000 / TICKS_PER_SECOND;
        public static readonly float INTERVAL_S = INTERVAL_MS / 1000;
        // Energy is gained at a rate of 1 second
        private static readonly int MILLISECONDS_PER_ENERGY = 2000;
        // ticks per energy is naturally = MILLISECONDS_PER_ENERGY / INTERVAL_MS
        public static readonly int TICKS_PER_ENERGY = MILLISECONDS_PER_ENERGY / INTERVAL_MS;
        // Max energy
        private static readonly int MAX_ENERGY = 6;

        // Milliseconds before the game starts after the tick starts
        private static readonly int STARTING_TIME_MILLISECONDS = 5000;
        private static readonly int ROUND_DURATION_MILLISECONDS = 60000;
        public static readonly int ROUND_DURATION_SECONDS = ROUND_DURATION_MILLISECONDS / 1000;

        public static readonly int ROUND_START_TICK = STARTING_TIME_MILLISECONDS / INTERVAL_MS;
        public static readonly int ROUND_END_TICK = ROUND_START_TICK + ROUND_DURATION_MILLISECONDS / INTERVAL_MS;



        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject arenaPrefab;
        // temporary, we want to load it dynamically later
        [SerializeField] private GameObject playerPrefab;

        // Connections to other prefab elements
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject readyInfoOverlay;
        [SerializeField] public BattleUI battleUI;
        [SerializeField] private ResultScreen resultScreen;
        [SerializeField] private CameraController cameraController;

        // Events
        [SerializeField] private BattleTickEvent battleTickEvent;
        [SerializeField] private EnergyChangedEvent energyChangedEvent;
        private DraggedUIEventListener draggedUIEventListener;
        private SwipeGestureEventListener swipeGestureEventListener;
        private TapGestureEventListener tapGestureEventListener;

        // Public Properties
        public int Energy { get; set; }
        public Arena Arena { get; set; }

        // Private vars
        private int currentTick = 0;
        private int serverTick = 0;

        private PlayerUnit p1PlayerUnit;
        private PlayerUnit p2PlayerUnit;
        private SmartFox sfs;

        private bool gameStarted;

        private Dictionary<byte, BaseCommandHandler> serverHandlerCommands;



        private void Update()
        {
            if (currentTick + 1 <= serverTick)
            {
                currentTick++;              
                battleTickEvent.Raise(currentTick);
                
                battleUI.HandleTick(currentTick);
                
                // TODO:
                if (!gameStarted && currentTick >= ROUND_START_TICK)
                {
                    gameStarted = true;
                    energyChangedEvent.Raise(Energy, TICKS_PER_ENERGY * INTERVAL_S);
                    readyInfoOverlay.SetActive(false);
                }

                if (gameStarted && currentTick >= ROUND_END_TICK)
                {
                    gameStarted = false;
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

            // Register server cmd handlers
            serverHandlerCommands = new Dictionary<byte, BaseCommandHandler>();
            serverHandlerCommands.Add(BaseCommandHandler.MOVE_CMD_ID,               new MoveCommandHandler(this));
            serverHandlerCommands.Add(BaseCommandHandler.DAMAGE_UNIT_CMD_ID,        new DamageUnitCommandHandler(this));
            serverHandlerCommands.Add(BaseCommandHandler.ENERGY_CHANGED_CMD_ID,     new EnergyChangedCommandHandler(this, energyChangedEvent));
            serverHandlerCommands.Add(BaseCommandHandler.CHIP_DRAWN_CMD_ID,         new ChipDrawnCommandHandler(this));
            serverHandlerCommands.Add(BaseCommandHandler.CHIP_PLAYED_CMD_ID,        new ChipPlayedCommandHandler(this));
            serverHandlerCommands.Add(BaseCommandHandler.BASIC_ATTACK_CMD_ID,       new BasicAttackCommandHandler(this));


            // Gesture event handlers
            draggedUIEventListener = gameObject.GetComponent<DraggedUIEventListener>();
            draggedUIEventListener.draggedUIEventCallback += HandleDraggedUIEvent;
            swipeGestureEventListener = gameObject.GetComponent<SwipeGestureEventListener>();
            swipeGestureEventListener.swipeGestureEventCallback += HandleSwipeGestureEvent;
            tapGestureEventListener = gameObject.GetComponent<TapGestureEventListener>();
            tapGestureEventListener.tapGestureEventCallback += HandleTapGestureEvent;


            // send a message to the server to let it know we are ready to receive messages
            SFSObject obj = new SFSObject();
            sfs.Send(new ExtensionRequest("pr", obj, sfs.LastJoinedRoom));


            CreateArena();
            CreatePlayerUnits();

            //loadingScreen.SetActive(false);
        }
        
        private void CreateArena()
        {
            GameObject arenaGO = GameObject.Instantiate(arenaPrefab);
            Arena = arenaGO.GetComponent<Arena>();
            Arena.Initialize();
        }

        private void CreatePlayerUnits()
        {
            // clients should already have data about starting positions and player unit data...
            // do something here            
            GameObject p1PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p1PlayerUnit = p1PlayerUnitGO.GetComponent<PlayerUnit>();
            p1PlayerUnit.id = P1_PLAYER_UNIT_ID;
            p1PlayerUnit.SetFacingLeft(false);
            Arena.PlacePlayerUnit(p1PlayerUnit, Constants.Owner.Player1);

            GameObject p2PlayerUnitGO = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p2PlayerUnit = p2PlayerUnitGO.GetComponent<PlayerUnit>();
            p2PlayerUnit.id = P2_PLAYER_UNIT_ID;
            p2PlayerUnit.SetFacingLeft(true);
            Arena.PlacePlayerUnit(p2PlayerUnit, Constants.Owner.Player2);
        }

        // ======================================================================================================
        //  Gesture Handlers
        // ======================================================================================================
        private void HandleDraggedUIEvent(DraggedUIEvent.State state, IDraggableUI draggable)
        {
            if (!this.gameStarted) return;

            if (state == DraggedUIEvent.State.Ended)
            {   
                ChipUI chipUI = draggable.GetGameObject().GetComponent<ChipUI>();

                Chip c = GameDB.Instance.ChipsDB.GetChip(chipUI.cid);
                if (Energy >= c.cost)
                {
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
                        Arena.TryMove(p1PlayerUnit, dir);
                    }
                    else
                    {
                        Arena.TryMove(p2PlayerUnit, dir);
                    }
                }

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
                    InitializeHand(dataObject);
                    break;
                case "pv":
                    GameEnded(dataObject);
                    break;
            }
        }

        private void InitializeHand(SFSObject dataObject)
        {
            // Maybe this shouldnt be initialize hand but in general receive intialization data from the server
            ISFSArray chips = dataObject.GetSFSArray("chips");
            short[] chipsArr = new short[4];
            chipsArr[0] = chips.GetShort(0);
            chipsArr[1] = chips.GetShort(1);
            chipsArr[2] = chips.GetShort(2);
            chipsArr[3] = chips.GetShort(3);
            short nextChip = dataObject.GetShort("next");

            Debug.LogFormat("Received initial hand! [{0},{1},{2},{3}], next: {4}", chipsArr[0], chipsArr[1], chipsArr[2], chipsArr[3], nextChip);

            battleUI.InitializeHand(chipsArr, nextChip);

            
            // Maybe
            loadingScreen.SetActive(false);
        }

        private void CommandsReceived(SFSObject dataObject)
        {
            int latestTick = dataObject.GetInt("t");
            serverTick = latestTick;

            ISFSArray cmds = dataObject.GetSFSArray("c");
            foreach (ISFSArray cmd in cmds)
            {
                // The first byte in array is the command identitifer
                byte cmdId = cmd.GetByte(0);
                if ( serverHandlerCommands.TryGetValue(cmdId, out BaseCommandHandler cmdHandler) )
                {
                    cmdHandler.Execute(cmd);
                } else
                {
                    Debug.LogFormat("Received unknown command {0} from server", cmdId);
                }
            }            
        }

        private void GameEnded(SFSObject dataObject)
        {
            gameStarted = false;
            int winner = dataObject.GetInt("pid");
            if (winner == sfs.MySelf.PlayerId)
            {
                resultScreen.SetWon();
            }
            else
            {
                resultScreen.SetLost();
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
