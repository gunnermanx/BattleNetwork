using UnityEngine;
using Photon.Pun;
using System;
using BattleNetwork.Battle;

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

        public void Initialize(Double _startTime, Vector3 _startPos, Vector3 _direction, Constants.Owner _owner)
        {
            startTime = _startTime;
            startPos = _startPos;
            direction = _direction;

            owner = _owner;

            transform.position = startPos;           

            launched = true;
        }

        void Update()
        {
            if (launched)
            {
                float timePassed = (float)(PhotonNetwork.Time - startTime); 
                transform.position = startPos + direction * speed * timePassed;

                if (timePassed > lifetime)
                {
                    Destroy(gameObject);
                }
            }            
        }

        private void OnTriggerEnter(Collider other)
        {
            // gotta check if its allowed to shoot this other thing
            
            Damageable d = other.GetComponent<Damageable>();


            Debug.LogFormat("TRIGGER hitting damageable : otherOwner{0}   bulletOwner {1}", d.owner, owner);
            if (d != null && d.owner != owner)
            {
                d.Damage(damage);

                GameObject.Destroy(gameObject);
            }
        }
    }
}
