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


        [SerializeField] private SpriteRenderer spriteRenderer;

        private Damageable cachedDamageable;

        [SerializeField]  private Animator animator;

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

        public void SetFacingLeft(bool left)
        {
            spriteRenderer.flipX = left;            
        }

        public void TriggerAttackAnimation()
        {
            animator.SetTrigger("basic_attack");
        }

        public void TriggerMeleeAttackAnimation()
        {
            animator.SetTrigger("melee_attack");
        }

        public void TriggerMoveAnimation()
        {
            animator.SetTrigger("move");
        }

    }
}
