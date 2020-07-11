using UnityEngine;
using Photon.Pun;
using System;
using BattleNetwork.Battle;
using BattleNetwork.Events;

namespace BattleNetwork.Characters
{
    public class BasicProjectile : MonoBehaviour
    {
        private Double startTime;
        private Vector3 startPos;
        private Vector3 direction;

        private Constants.Owner owner;

        private bool launched;

        // MOVE THIS LATER
        private float speed = 5f;
        private int damage = 10;
        private Double lifetime = 10f;

        private BattleTickEventListener battleTickEventListener;

        public void Initialize(Vector3 _startPos, Constants.Owner _owner)
        {
            startPos = _startPos;
            
            owner = _owner;

            transform.position = startPos;


            battleTickEventListener = gameObject.GetComponent<BattleTickEventListener>();
            battleTickEventListener.tickCallback += HandleBattleTick;

        }

        void HandleBattleTick(int currentTick)
        {
            Transform currentTransform = transform;
            // move the projectile based on ticks, for now, fuck it
            if (owner == Constants.Owner.Player1)
            {
                currentTransform.position = new Vector3(
                    currentTransform.position.x + 0.4f,
                    currentTransform.position.y,
                    currentTransform.position.z
                );
            } else
            {
                currentTransform.position = new Vector3(
                    // within 10 ticks, arrives at the next grid spot
                    currentTransform.position.x - 0.4f,
                    currentTransform.position.y,
                    currentTransform.position.z
                );
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            
            Damageable d = other.GetComponent<Damageable>();


            //Debug.LogFormat("TRIGGER hitting damageable : otherOwner{0}   bulletOwner {1}", d.owner, owner);
            //if (d != null && d.owner != owner)
            //{
            //    GameObject.Destroy(gameObject);
            //}
        }
    }
}
