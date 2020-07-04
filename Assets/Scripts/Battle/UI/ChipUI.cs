using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNetwork.Battle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ChipUI : MonoBehaviour, IDraggableUI
    {
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
            TweenToZero(null, 0.10f);
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
    }
}