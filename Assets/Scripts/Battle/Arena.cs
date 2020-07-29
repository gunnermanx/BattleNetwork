using UnityEngine;
using System;
using System.Collections.Generic;
using BattleNetwork.Characters;
using DigitalRubyShared;
using Sfs2X;
using BattleNetwork.Battle.Chips;

namespace BattleNetwork.Battle
{
    public class Arena : MonoBehaviour
    {
        // Arena needs to know what is on a tile, because it needs to evaluate if something can mvoe onto it

        private static readonly int ARENA_LENGTH = 6;
        private static readonly int ARENA_WIDTH = 3;

        [SerializeField]
        private ArenaData arenaData;

        [SerializeField]
        private Transform arenaAnchorsParent;

        private Dictionary<string, GameObject> anchors;
        private Tile[,] tiles;

        private Dictionary<int, BaseUnit> units;

        private PlayerUnit p1PlayerUnit;
        private PlayerUnit p2PlayerUnit;

        private BaseChip p1ActiveChip;
        private BaseChip p2ActiveChip;

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
            string[] startingTileNameArr = startingTileName.Split('_');
            int x = Int32.Parse(startingTileNameArr[0]);
            int z = Int32.Parse(startingTileNameArr[1]);

            unit.tilePos = new Vector2(x, z);
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

        public void ServerMoveUnit(int unitId, int x, int z)
        {
            SmartFox sfs = SFSConnector.Instance.Connection;

            BaseUnit unit;
            bool found = units.TryGetValue(unitId, out unit);
            
            // TODO pretty hacky association, fix later
            if (unit is PlayerUnit && unit.id != sfs.MySelf.PlayerId)
            {
                (unit as PlayerUnit).TriggerMoveAnimation();
            }

            Tile targetTile = tiles[x, z];            
            unit.tilePos = new Vector2(x, z);
            unit.transform.position = targetTile.transform.position + new Vector3(0f, 0.5f, 0f);                
            
            
        }

        public void ServerDamageUnit(int unitId, int damage)
        {
            BaseUnit unit;
            bool found = units.TryGetValue(unitId, out unit);

            Damageable d = unit.GetComponent<Damageable>();

            // TODO pretty hacky association, fix later
            if (unit is PlayerUnit)
            {
                (unit as PlayerUnit).TriggerHitAnimation();
            }

            d.Damage(damage);
        }

        public void ServerBasicAttack(int playerId)
        {
            if (playerId == 1)
            {
                p1PlayerUnit.TriggerAttackAnimation();
            }
            else if (playerId == 2)
            {
                p2PlayerUnit.TriggerAttackAnimation();
            }
        }

        public void ServerPlayedChip(int playerId, short chipId)
        {
            // Some kind of chip system here, similar to server that will tick chips 
            // simple test for now
            PlayerUnit target = null;
            if (playerId == 1)
            {
                target = p1PlayerUnit;
                p1ActiveChip = ChipFactory.GetChip(target, playerId, chipId, this);
                p1ActiveChip.Init();
            }
            else if (playerId == 2)
            {
                target = p2PlayerUnit;
                p2ActiveChip = ChipFactory.GetChip(target, playerId, chipId, this);
                p2ActiveChip.Init();
            }
        }


        public bool TryMove(PlayerUnit unit, byte dir)
        {
            Vector2 target = Vector3.zero;
            switch (dir)
            {
                case (byte) 'u':
                    target = new Vector2(unit.tilePos.x, unit.tilePos.y + 1);
                    break;
                case (byte) 'd':
                    target = new Vector2(unit.tilePos.x, unit.tilePos.y - 1);
                    break;
                case (byte) 'l':
                    target = new Vector2(unit.tilePos.x - 1, unit.tilePos.y);
                    break;
                case (byte) 'r':
                    target = new Vector2(unit.tilePos.x + 1, unit.tilePos.y);
                    break;
                default:
                    return false;
            }

            if (IsTileCoordsValid((int)target.x, (int)target.y))
            {
                Tile targetTile = tiles[(int)target.x, (int)target.y];                
                if (unit.owner != targetTile.owner)
                {
                    return false;
                }
                unit.tilePos = target;
                unit.transform.position = targetTile.transform.position + new Vector3(0f, 0.5f, 0f);
                unit.TriggerMoveAnimation();
                return true;
               
            }

            return false;
        }

        
        public void TargetRow(int startingX, int z, byte dir)
        {
        }

        public void TargetColumn(int x, bool add)
        {
            for (int z = 0; z < ARENA_WIDTH; z++)
            {
                TargetTile(x, z, add);
            }
        }

        public void TargetTile(int x, int z, bool add)
        {
            if ( IsTileCoordsValid(x,z) )
            {
                Debug.LogFormat("TargetTile [{0},{1}] : {2}", x, z, add);
                if (add)
                {
                    tiles[x, z].Target();
                } else
                {
                    tiles[x, z].Untarget();
                }
                
            }            
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
            tiles = new Tile[6, 3];

            for (int i = 0; i < arenaData.tiles.Count; i++)
            {
                TileData tileData = arenaData.tiles[i];

                String[] coordStrings = tileData.name.Split('_');
                int x = Int32.Parse(coordStrings[0]);
                int z = Int32.Parse(coordStrings[1]);

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
                tiles[x, z] = tile;
                
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
            tiles = null;
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

        public bool IsTileCoordsValid(int x, int z)
        {
            if (x >= 0 && x < ARENA_LENGTH && z >= 0 && z < ARENA_WIDTH)
            {
                return true;
            }
            return false;
        }
    }

}
