using UnityEngine;
using UnityEngine.SceneManagement;
using BattleNetwork.Battle;
using Sfs2X.Core;
using Sfs2X;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BattleManager battleManager;

    private SmartFox sfs;
    private bool shuttingDown;

    private void Awake()
    {
        Application.runInBackground = true;
        sfs = SFSConnector.Instance.Connection;

        if (sfs == null)
        {
            SceneManager.LoadScene("Connector");
            return;
        }

        RegisterSFSListeners();
    }

    void Start()
    {
        battleManager.InitializeBattle();
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
        //sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        //sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
    }
    private void UnregisterSFSListeners()
    {       
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        //sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        //sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        UnregisterSFSListeners();

        if (!shuttingDown)
        {
            SceneManager.LoadScene("Connector");
        }
    }
}
