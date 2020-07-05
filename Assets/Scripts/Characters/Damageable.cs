using System;
using UnityEngine;
using BattleNetwork.Battle;

namespace BattleNetwork.Characters
{
    public class Damageable : MonoBehaviour
    {
        public delegate void DamageTakenCallbackFunction(int amount, int remainingHitpoints);
        public event DamageTakenCallbackFunction damageTaken;

        private int hitpoints;

        public Constants.Owner owner;

        private void Start()
        {
            hitpoints = 100;
        }
        
        public void Damage(int amount)
        {
            hitpoints -= amount;
            damageTaken?.Invoke(amount, Math.Max(0, hitpoints));
        }
    }
}
