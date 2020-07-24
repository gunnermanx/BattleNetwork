using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleNetwork.Battle
{ 
    class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 p1Pos;
        [SerializeField] private Vector3 p1Rot;

        [SerializeField] private Vector3 p2Pos;
        [SerializeField] private Vector3 p2Rot;

        public void SetPlayer(int playerId)
        {
            if (playerId == 1)
            {
                gameObject.transform.eulerAngles = p1Rot;
                gameObject.transform.position = p1Pos;
            } 
            else if (playerId == 2)
            {
                gameObject.transform.eulerAngles = p2Rot;
                gameObject.transform.position = p2Pos;
            }
        }
    }
}
