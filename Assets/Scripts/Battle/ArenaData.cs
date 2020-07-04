using BattleNetwork.Battle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNetwork.Battle
{
    [CreateAssetMenu]
    public class ArenaData : ScriptableObject
    {
        public string player1StartingTileName;
        public string player2StartingTileName;

        public List<TileData> tiles;
    }

    [Serializable]
    public class TileData
    {
        public string name;
        public Vector3 position;
        public GameObject tilePrefab;
        public Constants.Owner owner;
    }

}
