using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BattleNetwork.Events;

namespace BattleNetwork.Battle
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject attackIndicator;

        [SerializeField] private GameObject p1Tile;
        [SerializeField] private GameObject p2Tile;

        private Constants.Owner owner;

        public int attacksTargetting = 0;

        private BattleTickEventListener battleTickEventListener;

        private BaseTileEffect tileEffect;

        private int currentTick;        
        private Dictionary<int, int> attackIndicatorExpiration; // source id to expiration
        private List<int> expiredSources;

        private void Start()
        {
            attackIndicatorExpiration = new Dictionary<int, int>();
            expiredSources = new List<int>();

            battleTickEventListener = gameObject.GetComponent<BattleTickEventListener>();
            battleTickEventListener.tickCallback += HandleTick;
        }

        public void SetOwner(Constants.Owner owner)
        {
            this.owner = owner;
            if (this.owner == Constants.Owner.Player1)
            {
                p1Tile.SetActive(true);
                p2Tile.SetActive(false);
            } else
            {
                p1Tile.SetActive(false);
                p2Tile.SetActive(true);
            }
        }

        public Constants.Owner Owner()
        {
            return this.owner;
        }

        private void HandleTick(int currentTick)
        {
            this.currentTick = currentTick;
            
            foreach (KeyValuePair<int, int> kvp in attackIndicatorExpiration)
            {
                if (kvp.Value <= this.currentTick)
                {
                    expiredSources.Add(kvp.Key);
                }
            }

            for (int i = 0; i < expiredSources.Count; i++)
            {
                attackIndicatorExpiration.Remove(expiredSources[i]);
            }
            expiredSources.Clear();

            if (attackIndicatorExpiration.Count == 0)
            {
                attackIndicator.SetActive(false);
            }
        }

        public void Target(int sourceId, int duration)
        {
            if (!attackIndicatorExpiration.ContainsKey(sourceId))
            {
                attackIndicatorExpiration.Add(sourceId, this.currentTick + duration);
            } else
            {
                attackIndicatorExpiration[sourceId] = this.currentTick + duration;
            }
            
            if (attackIndicator.activeSelf == false)
            {
                attackIndicator.SetActive(true);
            }
        }

        public void Untarget(int sourceId)
        {
            if (!attackIndicatorExpiration.ContainsKey(sourceId))
            {
                attackIndicatorExpiration.Remove(sourceId);
            }
        }
    }
}
