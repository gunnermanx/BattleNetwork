using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{

    private SmartFox sfs;
    private bool shuttingDown;


    private bool TEMP_searching_for_game = false;

    private void Awake()
    {
        Application.runInBackground = true;
        sfs =  SFSConnector.Instance.Connection;

        if (sfs == null)
        {
            SceneManager.LoadScene("Login");
            return;
        }

        RegisterSFSListeners();

        // try to join the lobby

        // do matchmaking later, for now just create/join a room
        sfs.Send(new JoinRoomRequest("Lobby"));
    }

    private void Update()
    {
        if (sfs != null)
        {
           sfs.ProcessEvents();
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void RegisterSFSListeners()
    {
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
    }
    private void UnregisterSFSListeners()
    {
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
    }

    public void PlayButtonClick()
    {
        TEMP_searching_for_game = true; 

        // needs matchmakig later just test this for now
        bool foundBattleRoom = false;
        List<Room> rooms = sfs.RoomManager.GetRoomList();
        foreach (Room room in rooms)
        {
            if (room.Name == "battle")
            {
                foundBattleRoom = true;
                sfs.Send(new JoinRoomRequest("battle"));                
                break;
            }
        }

        if (!foundBattleRoom)
        {
            RoomSettings settings = new RoomSettings("battle");
            settings.GroupId = "games";
            settings.IsGame = true;
            settings.MaxUsers = 2;
            settings.MaxSpectators = 0;
            //settings.Extension = new RoomExtension(EXTENSION_ID, EXTENSION_CLASS);

            // creatiing room doesnt make you join a room
            sfs.Send(new CreateRoomRequest(settings));
        }        
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        UnregisterSFSListeners();

        if (!shuttingDown)
        {
            SceneManager.LoadScene("Login");
        }
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        // Either joined or created
        if (room.IsGame)
        {
            UnregisterSFSListeners();

            // Load game scene
            SceneManager.LoadScene("Battle");
        }
        else
        {
            Debug.LogFormat("You joined the lobby: ", room.Name);
        }
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        Debug.LogFormat("Room join failed: " + (string)evt.Params["errorMessage"]);
    }

    private void OnRoomAdded(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        // Update view (only if room is game)
        if (TEMP_searching_for_game && room.IsGame)
        {
            sfs.Send(new JoinRoomRequest("battle"));
        }
    }
}
