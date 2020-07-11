using BattleNetwork.Battle;
using BattleNetwork.Events;
using UnityEngine;

namespace BattleNetwork.Characters
{
    public class PlayerUnit: BaseUnit
    {
        public class InstantiationData {
            public Constants.Owner owner;
            public string currentTile;
        }

        [SerializeField] private PlayerUnitCreatedEvent playerCreatedEvent;

        //test
        [SerializeField] private GameObject basicBulletPrefab;

        private Damageable cachedDamageable;

        private void Start()
        {
            //cachedPhotonView = gameObject.GetComponent<PhotonView>();
            cachedDamageable = gameObject.GetComponent<Damageable>();

            cachedDamageable.damageTaken += HandleDamageTaken;
            cachedDamageable.owner = owner;
            cachedDamageable.SetCurrent(100);  // TODO read from config 

            playerCreatedEvent.Raise(this);
        }

        private void HandleDamageTaken(int amount, int remaining)
        {
            if (remaining <= 0)
            {                
                // TODO               
            }
        }

    }
}
