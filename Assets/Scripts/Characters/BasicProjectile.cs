using UnityEngine;
using Photon.Pun;
using System;

namespace BattleNetwork.Characters
{
    public class BasicProjectile : MonoBehaviour
    {
        private Double startTime;
        private Vector3 startPos;
        private Vector3 direction;

        private bool launched;

        private float speed = 5f;
        private Double lifetime = 10f;

        public void Initialize(Double _startTime, Vector3 _startPos, Vector3 _direction)
        {
            startTime = _startTime;
            startPos = _startPos;
            direction = _direction;

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

    }
}
