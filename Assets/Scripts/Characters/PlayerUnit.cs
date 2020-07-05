using System;
using BattleNetwork.Battle;
using Photon.Pun;
using UnityEngine;

namespace BattleNetwork.Characters
{
    public class PlayerUnit: MonoBehaviour
    {
        public string currentTile;
        public Constants.Owner owner;

        private PhotonView cachedPhotonView;

        //test
        [SerializeField] private GameObject basicBulletPrefab;


        private void Start()
        {
            cachedPhotonView = gameObject.GetComponent<PhotonView>();
        }

        public void BasicAttack()
        {
            Double attackStartTime = PhotonNetwork.Time;
            Vector3 startingPos = transform.position;

            Vector3 direction;
            if (owner == Constants.Owner.Player1)
            {
                direction = Vector3.right;
            }
            else
            {
                direction = Vector3.left;
            }
           
            cachedPhotonView.RPC("SendAttack", RpcTarget.All, startingPos, attackStartTime, direction);
        }

        // prototype, later we should have attacks have own serialization function, and pass that over the wire instead.
        [PunRPC]
        private void SendAttack(Vector3 startingPos, Double startTime, Vector3 direction)
        {
            Debug.LogFormat("Got SendAttack RPC {0}, {1}, {2}", startingPos, startTime, direction);


            GameObject bullet = GameObject.Instantiate(basicBulletPrefab);
            bullet.transform.position = startingPos;
            bullet.GetComponent<BasicProjectile>().Initialize(startTime, startingPos, direction);

        }
    }
}
