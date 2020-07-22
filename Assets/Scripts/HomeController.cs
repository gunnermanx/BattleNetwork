using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sfs2X.Entities.Match;
using Sfs2X.Entities.Data;

public class HomeController : MonoBehaviour
{
    [SerializeField] private GameObject matchmakingScreen;

    private SmartFox sfs;
    private bool shuttingDown;


    private void Awake()
    {
        Application.runInBackground = true;
        sfs =  SFSConnector.Instance.Connection;

        if (sfs == null)
        {
            SceneManager.LoadScene("Connector");
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
        //sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
    }
    private void UnregisterSFSListeners()
    {
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        //sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
    }

    public void PlayButtonClick()
    {
        matchmakingScreen.SetActive(true);
        sfs.Send(new ExtensionRequest("matchmaking", new SFSObject()));       
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        UnregisterSFSListeners();

        if (!shuttingDown)
        {
            SceneManager.LoadScene("Connector");
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

}
