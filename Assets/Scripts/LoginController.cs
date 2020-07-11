using System;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject progressLabel;

    [SerializeField] private InputField hostInputField;
    [SerializeField] private InputField portInputField;
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;

    [SerializeField] private ScrollRect debugScrollRect;
    [SerializeField] private Text debugText;

    //private string defaultHost = "3.12.181.63";
    private string defaultHost = "127.0.0.1";
    private string defaultPort = "9933";

    private void Start()
    {
        hostInputField.text = defaultHost;
        portInputField.text = defaultPort;

        debugText.text = "";

        enableInterface(true);
    }

    private void Update()
    {
        if (SFSConnector.Instance.Connection != null)
        {
            SFSConnector.Instance.Connection.ProcessEvents();
        }
    }

    public void Connect()
    {
        if (!SFSConnector.Instance.Connection.IsConnected)
        {
            enableInterface(false);

            RegisterSFSListeners();

            // TODO: should come from config
            ConfigData cfg = new ConfigData();
            cfg.Host = hostInputField.text;
            cfg.Port = Int32.Parse(portInputField.text);
            cfg.Zone = "BattleNetwork";
            cfg.Debug = true;

            SFSConnector.Instance.Connection.Connect(cfg);
        }
        else
        {
            SFSConnector.Instance.Connection.Disconnect();
        }
    }

    private void RegisterSFSListeners()
    {
        SmartFox sf = SFSConnector.Instance.Connection;

        sf.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sf.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sf.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sf.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);

        sf.AddLogListener(Sfs2X.Logging.LogLevel.INFO, OnInfoMessage);
        sf.AddLogListener(Sfs2X.Logging.LogLevel.WARN, OnWarnMessage);
        sf.AddLogListener(Sfs2X.Logging.LogLevel.ERROR, OnErrorMessage);
    }
    private void UnregisterSFSListeners()
    {
        SmartFox sf = SFSConnector.Instance.Connection;

        sf.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        sf.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sf.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
        sf.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);

        sf.RemoveLogListener(Sfs2X.Logging.LogLevel.INFO, OnInfoMessage);
        sf.RemoveLogListener(Sfs2X.Logging.LogLevel.WARN, OnWarnMessage);
        sf.RemoveLogListener(Sfs2X.Logging.LogLevel.ERROR, OnErrorMessage);

        enableInterface(true);
    }

    private void enableInterface(bool enable)
    {
        controlPanel.SetActive(enable);
        progressLabel.SetActive(!enable);
    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            SmartFox sf = SFSConnector.Instance.Connection;

            Debug.LogFormat("SFS2X API version {0}", sf.Version);
            Debug.LogFormat("Connection mode {0}", sf.ConnectionMode);


            sf.Send(new Sfs2X.Requests.LoginRequest(usernameInputField.text));
        }
        else
        {
            UnregisterSFSListeners();
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        Debug.LogFormat("Connection Lost: {0}", (string)evt.Params["reason"]);
        // remove sfs2x listeners
        UnregisterSFSListeners();
    }

    private void OnLogin(BaseEvent evt)
    {
        UnregisterSFSListeners();
        SceneManager.LoadScene("Home");
    }

    private void OnLoginError(BaseEvent evt)
    {
        SFSConnector.Instance.Connection.Disconnect();
        UnregisterSFSListeners();
    }





    public void OnInfoMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("INFO", message);
    }

    public void OnWarnMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("WARN", message);
    }

    public void OnErrorMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        ShowLogMessage("ERROR", message);
    }

    private void ShowLogMessage(string level, string message)
    {
        message = "[SFS > " + level + "] " + message;
        trace(message);
        Debug.Log(message);
    }

    private void trace(string msg)
    {
        debugText.text += (debugText.text != "" ? "\n" : "") + msg;
        Canvas.ForceUpdateCanvases();
        debugScrollRect.verticalNormalizedPosition = 0;
    }
}
