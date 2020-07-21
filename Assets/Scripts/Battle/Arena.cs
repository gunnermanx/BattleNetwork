﻿using UnityEngine;
using System;
using System.Collections.Generic;
using BattleNetwork.Characters;
using DigitalRubyShared;

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

        private Dictionary<int, BaseUnit> units;

        private PlayerUnit p1PlayerUnit;
        private PlayerUnit p2PlayerUnit;

        public void Initialize()
        {
            // will need to be changed, load data based on name, and what server told us
            LoadArenaData();

            units = new Dictionary<int, BaseUnit>();
        }


        public void PlacePlayerUnit(PlayerUnit unit, Constants.Owner owner)
        {
            Debug.LogFormat("placing a player unit for player {0}", owner);


            string startingTileName = (owner == Constants.Owner.Player1) ? arenaData.player1StartingTileName : arenaData.player2StartingTileName;
            unit.currentTile = startingTileName;
            GameObject anchor = GetAnchorForTileName(startingTileName);
            if (anchor != null)
            {
                unit.transform.position = anchor.transform.position + new Vector3(0f, 0.5f, 0f);
            }
            unit.owner = owner;

            units.Add(unit.id, unit);

            if (owner == Constants.Owner.Player1)
            {
                p1PlayerUnit = unit;
            }
            else if (owner == Constants.Owner.Player2)
            {
                p2PlayerUnit = unit;            }

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

        public void ServerMoveUnit(int unitId, int x, int y)
        {
            BaseUnit unit;
            bool found = units.TryGetValue(unitId, out unit);

            string newTileName = x + "_" + y;

            Tile newTile;
            if (tiles.TryGetValue(newTileName, out newTile))
            {
                unit.currentTile = newTileName;
                unit.transform.position = newTile.transform.position + new Vector3(0f, 0.5f, 0f);                
            }
        }

        public void ServerDamageUnit(int unitId, int damage)
        {
            BaseUnit unit;
            bool found = units.TryGetValue(unitId, out unit);

            Damageable d = unit.GetComponent<Damageable>();

            d.Damage(damage);
        }

        public void PlayChip(int playerId, int chipId)
        {
            // TEMP just create a straight projectile

            GameObject projectile = Instantiate(
                Resources.Load("TestStraightProjectile", typeof(GameObject))
            ) as GameObject;
            StraightProjectile p = projectile.GetComponent<StraightProjectile>();

            Vector3 position = Vector3.zero;
            Constants.Owner owner = Constants.Owner.None;
            if (playerId == 1)
            {
                position = p1PlayerUnit.transform.position;
                owner = Constants.Owner.Player1;
            }
            else if (playerId == 2)
            {
                position = p2PlayerUnit.transform.position;
                owner = Constants.Owner.Player2;
            }
            p.Initialize(position, owner);
        }



        public bool TryMove(PlayerUnit unit, SwipeGestureRecognizerDirection direction)
        {
            string currentTileName = unit.currentTile;
            string[] currentTileNameArr = currentTileName.Split('_');

            int x = Int32.Parse(currentTileNameArr[0]);
            int z = Int32.Parse(currentTileNameArr[1]);

            string newTileName = "";

            switch (direction)
            {
                case SwipeGestureRecognizerDirection.Up:
                    newTileName = x + "_" + (z + 1);
                    break;
                case SwipeGestureRecognizerDirection.Down:
                    newTileName = x + "_" + (z - 1);
                    break;
                case SwipeGestureRecognizerDirection.Left:
                    newTileName = (x - 1) + "_" + z;
                    break;
                case SwipeGestureRecognizerDirection.Right:
                    newTileName = (x + 1) + "_" + z;
                    break;                
            }

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
