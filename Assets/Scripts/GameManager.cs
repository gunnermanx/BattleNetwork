using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using BattleNetwork.Battle;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private BattleManager battleManager;

    void Start()
    {
        StartBattle();
    }

    private void StartBattle()
    {
        battleManager.StartBattle();
    }
}
