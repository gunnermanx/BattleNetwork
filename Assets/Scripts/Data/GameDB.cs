using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNetwork.Data
{

    public class GameDB : MonoBehaviour
    {
        private static GameDB instance = null;
        public ChipsDB ChipsDB { get; set; }

        public static GameDB Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("GameDB not initialized?");
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                throw new Exception("Trying to create another GameDB when one already exists");
            }
            instance = this;
            DontDestroyOnLoad(this);           
        }
    }


    public class ChipsDB : Dictionary<short, Chip>
    {
        public Chip GetChip(short cid)
        {
            Chip cd;
            if (this.TryGetValue(cid, out cd))
            {
                return cd;
            }
            throw new Exception(string.Format("Did not find chip with id {0} in chipsDB", cid));
        }
    }

    public class Chip
    {
        public string name;
        public int cost;
        public List<ChipData> data;
    }

    public class ChipData
    {
        public int damage;
        public int projectileSpeed;
        public int width;
        public int depth;
        public int duration;
    }
}
