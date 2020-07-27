using UnityEngine;
using System;
using BattleNetwork.Battle;
using BattleNetwork.Events;
using BattleNetwork.Data;

namespace BattleNetwork.Characters
{
    public class StraightProjectile : MonoBehaviour
    {
        private Vector3 startPos;

        private Constants.Owner owner;


        public Vector2 tilePos;
        public int progress;
        public int speed;
        private Arena arena;

        private BattleTickEventListener battleTickEventListener;

        public void Initialize(Vector3 _startPos, Constants.Owner _owner, Vector2 _tilePos, int _speed, Arena _arena)
        {
            progress = 0;

            startPos = _startPos;
            speed = _speed;
            owner = _owner;
            tilePos = _tilePos;
            arena = _arena;

            transform.position = new Vector3(startPos.x, 1.55f, startPos.z);


            battleTickEventListener = gameObject.GetComponent<BattleTickEventListener>();
            battleTickEventListener.tickCallback += HandleBattleTick;

        }

        void HandleBattleTick(int currentTick)
        {
            progress++;

            Transform currentTransform = transform;
            // move the projectile based on ticks, for now, fuck it
            if (owner == Constants.Owner.Player1)
            {
                currentTransform.position = new Vector3(
                    currentTransform.position.x + 0.4f,
                    currentTransform.position.y,
                    currentTransform.position.z
                );
                
                if (progress % speed == 0)
                {
                    arena.TargetTile((int)tilePos.x, (int)tilePos.y, false);
                    tilePos = new Vector2(tilePos.x + 1, tilePos.y);
                    arena.TargetTile((int)tilePos.x, (int)tilePos.y, true);
                }
            } else
            {
                currentTransform.position = new Vector3(
                    // within 10 ticks, arrives at the next grid spot
                    currentTransform.position.x - 0.4f,
                    currentTransform.position.y,
                    currentTransform.position.z
                );

                if (progress % speed == 0)
                {
                    arena.TargetTile((int)tilePos.x, (int)tilePos.y, false);
                    tilePos = new Vector2(tilePos.x -1, tilePos.y);
                    arena.TargetTile((int)tilePos.x, (int)tilePos.y, true);
                }
            }
            
            if (!arena.IsTileCoordsValid((int)tilePos.x, (int)tilePos.y))
            {
                GameObject.Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            
            Damageable d = other.GetComponent<Damageable>();


            //Debug.LogFormat("TRIGGER hitting damageable : otherOwner{0}   bulletOwner {1}", d.owner, owner);
            if (d != null && d.owner != owner)
            {
                arena.TargetTile((int)tilePos.x, (int)tilePos.y, false);
                GameObject.Destroy(gameObject);
            }
        }
    }
}
