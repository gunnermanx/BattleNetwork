using UnityEngine;
using System;
using System.Collections.Generic;
using BattleNetwork.Characters;

namespace BattleNetwork.Battle
{
    public class Arena : MonoBehaviour
    {
        // Arena needs to know what is on a tile, because it needs to evaluate if something can mvoe onto it

        [SerializeField]
        private ArenaData arenaData;

        [SerializeField]
        private Transform arenaAnchorsParent;
      
        private Dictionary<string, GameObject> anchors;
        private Dictionary<string, Tile> tiles;


        public void Initialize()
        {
            LoadArenaData();
        }


        public void PlacePlayerUnit(PlayerUnit unit, Constants.Owner owner)
        {
            string startingTileName = (owner == Constants.Owner.Player1) ? arenaData.player1StartingTileName : arenaData.player2StartingTileName;
            unit.currentTile = startingTileName;
            GameObject anchor = GetAnchorForTileName(startingTileName);
            if (anchor != null)
            {
                unit.transform.position = anchor.transform.position + new Vector3(0f, 0.5f, 0f);                
            }
            unit.owner = owner;
        }

        private GameObject GetAnchorForTileName(string tileName)
        {
            GameObject anchor;
            if (anchors.TryGetValue(tileName, out anchor))
            {
                return anchor;
            }
            return null;
        }



        // statically determine which path to go instead of recalculating

        public bool TryMoveUp(PlayerUnit unit)
        {
            string currentTileName = unit.currentTile;
            string[] currentTileNameArr = currentTileName.Split('_');
            int z = Int32.Parse(currentTileNameArr[1]);
            string newTileName = currentTileNameArr[0] + "_" + (z + 1);

            Tile newTile;
            if (tiles.TryGetValue(newTileName, out newTile))
            {
                if (unit.owner != newTile.owner)
                {
                    return false;
                }
                unit.currentTile = newTileName;
                unit.transform.position = newTile.transform.position + new Vector3(0f, 0.5f, 0f);
                return true;
            }

            return false;
        }

        public bool TryMoveDown(PlayerUnit unit)
        {
            string currentTileName = unit.currentTile;
            string[] currentTileNameArr = currentTileName.Split('_');
            int z = Int32.Parse(currentTileNameArr[1]);
            string newTileName = currentTileNameArr[0] + "_" + (z - 1);

            Tile newTile;
            if (tiles.TryGetValue(newTileName, out newTile))
            {
                if (unit.owner != newTile.owner)
                {
                    return false;
                }
                unit.currentTile = newTileName;
                unit.transform.position = newTile.transform.position + new Vector3(0f, 0.5f, 0f);
                return true;
            }

            return false;
        }

        public bool TryMoveLeft(PlayerUnit unit)
        {
            string currentTileName = unit.currentTile;
            string[] currentTileNameArr = currentTileName.Split('_');
            int x = Int32.Parse(currentTileNameArr[0]);
            string newTileName = (x - 1) + "_" + currentTileNameArr[1];

            Tile newTile;
            if (tiles.TryGetValue(newTileName, out newTile))
            {
                if (unit.owner != newTile.owner)
                {
                    return false;
                }
                unit.currentTile = newTileName;
                unit.transform.position = newTile.transform.position + new Vector3(0f, 0.5f, 0f);
                return true;
            }

            return false;
        }

        public bool TryMoveRight(PlayerUnit unit)
        {
            string currentTileName = unit.currentTile;
            string[] currentTileNameArr = currentTileName.Split('_');
            int x = Int32.Parse(currentTileNameArr[0]);
            string newTileName = (x + 1) + "_" + currentTileNameArr[1];

            Tile newTile;
            if (tiles.TryGetValue(newTileName, out newTile))
            {
                if (unit.owner != newTile.owner)
                {
                    return false;
                }
                unit.currentTile = newTileName;
                unit.transform.position = newTile.transform.position + new Vector3(0f, 0.5f, 0f);
                return true;
            }

            return false;
        }

        private void LoadArenaData()
        {
            // Doubt this will happen during normal loads, but for editor, 
            // make sure we are clear before loading again
            if (Application.isEditor && anchors != null && anchors.Count > 0)
            {
                DestroyTiles();
            }

            anchors = new Dictionary<string, GameObject>();
            tiles = new Dictionary<string, Tile>();
           
            for (int i = 0; i < arenaData.tiles.Count; i++)
            {
                TileData tileData = arenaData.tiles[i];
                // Create anchor
                GameObject anchor = new GameObject(tileData.name);
                Transform anchorTransform = anchor.transform;
                anchorTransform.SetParent(arenaAnchorsParent);
                anchorTransform.localPosition = tileData.position;
                anchors.Add(tileData.name, anchor);
                // Create tile
                GameObject tileGO = GameObject.Instantiate(tileData.tilePrefab, anchorTransform);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.owner = tileData.owner;
                tiles.Add(tileData.name, tile);
            }

            Debug.LogFormat("Loaded Arena data: {0}", arenaData.name);
        }

        private void DestroyTiles()
        {
            foreach (KeyValuePair<string, GameObject> kvp in anchors)
            {
                GameObject.DestroyImmediate(kvp.Value);
            }

            anchors.Clear();
            tiles.Clear();
        }

        private void OnDrawGizmos()
        {
            if (arenaAnchorsParent != null)
            {
                for (int i = 0; i < arenaAnchorsParent.childCount; i++)
                {
                    Transform anchor = arenaAnchorsParent.GetChild(i);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(anchor.position, new Vector3(2.0f, 0.5f, 2.0f));
                }
            }            
        }
    }

}
