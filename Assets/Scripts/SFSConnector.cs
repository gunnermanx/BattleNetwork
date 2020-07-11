using Sfs2X;
using System;
using UnityEngine;

public class SFSConnector : MonoBehaviour
{
    // TODO: Read from config file? 
    // Need to be flexible so we dont need a code change if server dies
    private string defaultHost = "3.12.181.63";
    private string defaultPort = "9933";

    private SmartFox sfs;

    private static SFSConnector instance = null;

    public static SFSConnector Instance
    {
        get
        {
            if (instance == null)
            {
                throw new Exception("SFSConnection not initialized?");
            }
            return instance;
        }        
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            throw new Exception("Trying to create another SFSConnection when one already exists");
        }
        instance = this;
        DontDestroyOnLoad(this);
        sfs = new SmartFox();
    }

    public SmartFox Connection
    {
        get
        {
            return sfs;
        }
    }

    private void OnApplicationQuit()
    {
        if (sfs != null && sfs.IsConnected)
        {
            sfs.Disconnect();
        }            
    }

}
