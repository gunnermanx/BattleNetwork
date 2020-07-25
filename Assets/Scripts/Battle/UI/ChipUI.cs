using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ChipUI : MonoBehaviour, IDraggableUI
    {
        [SerializeField] private Vector2 dragOffset;
        [SerializeField] private Image chipArt;
        [SerializeField] private Text costText;
        public short cid;

        private Vector3 initialScale;

        private bool dragging = false;

        public int Index { get; set; }

        private RectTransform rectTransform;
        private void Start()
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            initialScale = rectTransform.localScale;
        }

        private void FixedUpdate()
        {
            if (dragging)
            {
                float distanceRatio = rectTransform.localPosition.y / 150f;
                Vector3 scale = Vector3.Lerp(initialScale, new Vector3(0.3f, 0.3f, 0.3f), distanceRatio);
                rectTransform.localScale = scale;
            }            
        }
        

        public void TweenToZero(Action tweenComplete, float time)
        {
            LeanTween.moveLocal(gameObject, Vector3.zero, time)
                .setEaseInCubic()
                .setOnComplete(tweenComplete);
        }

        public void DragEnded()
        {
            dragging = false;
            rectTransform.localPosition = Vector3.zero;            
            rectTransform.localScale = initialScale;
        }

        public void DragStarted()
        {
            dragging = true;
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

        public void Initialize(short cid, Sprite sprite, int cost)
        {
            this.cid = cid;
            chipArt.sprite = sprite;
            costText.text = cost.ToString();
        }
    }
}