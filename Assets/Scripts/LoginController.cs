using System;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using BattleNetwork.Data;
using Newtonsoft.Json;

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
    private string defaultHost = "172.16.10.180";
    private string defaultPort = "9933";

    private static readonly string PLACEHOLDER_GAMEDATA_JSON_URL = "https://s3-us-west-1.amazonaws.com/battlenetwork.gamedata/chips.json";

    private void Start()
    {
        hostInputField.text = defaultHost;
        portInputField.text = defaultPort;
        usernameInputField.text = "test1";
        passwordInputField.text = "test";

        debugText.text = "";

        StartCoroutine(LoadGameData());
    }

    private IEnumerator LoadGameData()
    {
        // Later need to compare manifests and such, for now just keep it simple
        UnityWebRequest request = new UnityWebRequest(PLACEHOLDER_GAMEDATA_JSON_URL);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string jsonString = request.downloadHandler.text;
            GameDB.Instance.ChipsDB = JsonConvert.DeserializeObject<ChipsDB>(jsonString);
        }


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

            string hashedPassword = PasswordUtil.MD5Password(passwordInputField.text);

            Debug.Log(hashedPassword);

            sf.Send(new Sfs2X.Requests.LoginRequest(usernameInputField.text, hashedPassword));
        }
        else
        {
            UnregisterSFSListeners();
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        Debug.LogFormat("Connection Lost: {0}", (string)evt.Params["reason"]);
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
