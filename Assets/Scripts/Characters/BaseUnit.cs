using System;
using BattleNetwork.Battle;
using BattleNetwork.Events;
using Photon.Pun;
using UnityEngine;


namespace BattleNetwork.Characters
{
    public class BaseUnit : MonoBehaviour
    {
        public int id;

        public string currentTile;
        public Constants.Owner owner;
    }
}
