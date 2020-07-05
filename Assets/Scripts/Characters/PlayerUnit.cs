using System;
using BattleNetwork.Battle;
using Photon.Pun;
using UnityEngine;

namespace BattleNetwork.Characters
{
    public class PlayerUnit: MonoBehaviour, IPunObservable
    {
        public class InstantiationData {
            public Constants.Owner owner;
            public string currentTile;
        }

        public string currentTile;
        public Constants.Owner owner;

        private PhotonView cachedPhotonView;

        //test
        [SerializeField] private GameObject basicBulletPrefab;

        private Damageable cachedDamageable;

        private void Start()
        {
            cachedPhotonView = gameObject.GetComponent<PhotonView>();
            cachedDamageable = gameObject.GetComponent<Damageable>();

            cachedDamageable.damageTaken += HandleDamageTaken;
            cachedDamageable.owner = owner;
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
           
            cachedPhotonView.RPC("SendAttack", RpcTarget.All, startingPos, attackStartTime, direction, owner);
        }

        // prototype, later we should have attacks have own serialization function, and pass that over the wire instead.
        [PunRPC]
        private void SendAttack(Vector3 startingPos, Double startTime, Vector3 direction, Constants.Owner owner)
        {
            Debug.LogFormat("Got SendAttack RPC {0}, {1}, {2}, {3}", startingPos, startTime, direction, owner);


            GameObject bullet = GameObject.Instantiate(basicBulletPrefab);
            bullet.transform.position = startingPos;            
            bullet.GetComponent<BasicProjectile>().Initialize(startTime, startingPos, direction, owner);

        }

        private void HandleDamageTaken(int amount, int remaining)
        {
            if (remaining <= 0)
            {
                
                Debug.Log("omg died");
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(owner);
                stream.SendNext(currentTile);
            }
            else
            {
                owner = (Constants.Owner)stream.ReceiveNext();
                currentTile = (string)stream.ReceiveNext();
            }
        }
    }
}
