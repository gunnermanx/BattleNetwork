using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleNetwork.Battle
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject attackIndicator;

        public Constants.Owner owner;

        public int attacksTargetting = 0;

        public void Target()
        {
            attacksTargetting++;
            if (attacksTargetting == 1)
            {
                attackIndicator.SetActive(true);
                
            }
        }

        public void Untarget()
        {
            attacksTargetting--;
            if (attacksTargetting == 0)
            {
                attackIndicator.SetActive(false);
            }
        }

    }
}
