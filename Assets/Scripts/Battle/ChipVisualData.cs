using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle
{
    [CreateAssetMenu]
    public class ChipVisualData : ScriptableObject
    {
        public List<ChipVisualDataEntry> data;
        private Dictionary<short, Sprite> cachedData = null;

        private void Start() {
            InitializeCachedData();
        }

        private void InitializeCachedData()
        {
            if (cachedData == null)
            {
                cachedData = new Dictionary<short, Sprite>();
                foreach (ChipVisualDataEntry c in data)
                {
                    cachedData.Add(c.cid, c.sprite);
                }
            }            
        }

        public Dictionary<short, Sprite> GetCachedData()
        {
            InitializeCachedData();
            return cachedData;
        }
    }

    [Serializable]
    public class ChipVisualDataEntry 
    {
        public short cid;
        public Sprite sprite;
    }


}
