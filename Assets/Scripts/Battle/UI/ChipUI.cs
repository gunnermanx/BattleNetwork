using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNetwork.Battle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ChipUI : MonoBehaviour, IDraggableUI
    {
        [SerializeField] private Vector2 dragOffset;

        public int Index { get; set; }

        private RectTransform rectTransform;
        private void Start()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        

        public void TweenToZero(Action tweenComplete, float time)
        {
            LeanTween.moveLocal(gameObject, Vector3.zero, time)
                .setEaseInCubic()
                .setOnComplete(tweenComplete);
        }

        public void DragEnded()
        {
            TweenToZero(null, 0f);
        }

        public void DragStarted()
        {
            
        }

        public RectTransform GetRectTransform()
        {
            return rectTransform;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector2 Offset()
        {
            return dragOffset;
        }
    }
}