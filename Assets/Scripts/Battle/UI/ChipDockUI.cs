﻿using BattleNetwork.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNetwork.Battle.UI
{
    public class ChipDockUI : MonoBehaviour
    {
        [SerializeField] private ChipVisualData chipVisualsDatabase;
        [SerializeField] private ChipUI chipUIPrefab;
        [SerializeField] private RectTransform chipDock;

        [SerializeField] private Image nextChipImage;

        [SerializeField] private float leftPadding;
        [SerializeField] private float rightPadding;
        [SerializeField] private float chipSpacing;
        [SerializeField] private float chipWidth;
        [SerializeField] private float anchorY;

        [SerializeField] private float minimizeY;
        [SerializeField] private float minimizeTweenTime;
        [SerializeField] private float maximizeTweenTime;

        private bool isMinimized;
        private bool isTweening;
        private LTDescr currentTween;
        private Vector3 startingPosition;        

        private float chipDockWidth;
        private int numChips;

        private ChipUI[] dockedChips;
        private RectTransform[] anchors;

        private int lastRemovedIndex = -1;


        private void Start()
        {
            isMinimized = false;
            isTweening = false;

            startingPosition = gameObject.transform.localPosition;
        }

        public void InitializeChipDockUI(int _numChips)
        {
            numChips = _numChips;
            dockedChips = new ChipUI[numChips];
            anchors = new RectTransform[numChips];

            InitializeDockWidth(numChips);
            CreateChipAnchors(numChips);
        }



        public int GetChipDataForIndex(int i)
        {
            // read from some chip data
            return 0;
        } 

        public void Minimize()
        {
            // If there is a tween playing, and the dock is minimized,
            // the previous tween can be cancelled as it is maximizing the dock
            if (isTweening && isMinimized)
            {
                LeanTween.cancel(gameObject);

                isTweening = true;
                currentTween = LeanTween.moveLocal(gameObject, startingPosition - new Vector3(0f, minimizeY, 0f), minimizeTweenTime)
                    .setEaseInCubic()
                    .setOnComplete(() =>
                    {
                        isMinimized = true;
                        isTweening = false;
                    });
            }
            // Otherwise if no tween is playing and the dock is not minimized
            // the dock can be tweened to its minimized position
            else if (!isTweening && !isMinimized)
            {
                isTweening = true;
                currentTween = LeanTween.moveLocal(gameObject, startingPosition - new Vector3(0f, minimizeY, 0f), minimizeTweenTime)
                    .setEaseInCubic()
                    .setOnComplete(() =>
                    {
                        isMinimized = true;
                        isTweening = false;
                    });
                
            }       
        }

        public void Maximize()
        {
            // If there is a tween playing, and the dock is maximized
            // the previous tween can be cancelled as it is minimizing the dock
            if (isTweening && !isMinimized)
            {
                LeanTween.cancel(gameObject);

                isTweening = true;
                currentTween = LeanTween.moveLocal(gameObject, startingPosition, maximizeTweenTime)
                    .setEaseInCubic()
                    .setOnComplete(() =>
                    {
                        isMinimized = false;
                        isTweening = false;
                    });
            }
            // Otherwise if no tween is playing and the dock is minimized
            // the dock can be tweened to its maximized position
            else if (!isTweening && isMinimized)
            {
                isTweening = true;
                currentTween = LeanTween.moveLocal(gameObject, startingPosition, maximizeTweenTime)
                    .setEaseInCubic()
                    .setOnComplete(() =>
                    {
                        isMinimized = false;
                        isTweening = false;
                    });
            }
        }

        private void InitializeDockWidth(int numChips)
        {
            chipDockWidth = leftPadding +
                            rightPadding +
                            (numChips - 1) * chipSpacing +
                            numChips * chipWidth;
            chipDock.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, chipDockWidth);
        }

        private void CreateChipAnchors(int numChips)
        {
            float leftSpacing = leftPadding;
            // TODO: sanity check number of chips
            for (int i = 0; i < numChips; i++)
            {
                GameObject anchor = new GameObject("cardAnchor" + i, typeof(RectTransform));

                RectTransform rt = anchor.GetComponent<RectTransform>();
                rt.SetParent(chipDock, false);
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(0f, 0f);
                rt.pivot = new Vector2(0f, 0f);

                rt.anchoredPosition = new Vector3(leftSpacing, anchorY, 0f);

                leftSpacing += chipWidth + chipSpacing;

                anchors[i] = rt;
            }
        }

        public void AddChip(int index, short chipId)
        {
            // Probably want to clean this up..
            Chip c = GameDB.Instance.ChipsDB.GetChip(chipId);

            GameObject chip = GameObject.Instantiate(chipUIPrefab.gameObject);
            chip.transform.SetParent(anchors[index], false);
            ChipUI chipUI = chip.GetComponent<ChipUI>();
            chipUI.Index = index;
            chipUI.Initialize(chipId, chipVisualsDatabase.GetCachedData()[chipId], c.cost);
            dockedChips[index] = chipUI;
        }

        public void SetNextChipPreview(short chipId)
        {
            nextChipImage.sprite = chipVisualsDatabase.GetCachedData()[chipId];
        }

        public void AddChipAtLastIndex(short chipId)
        {
            if (lastRemovedIndex != -1)
            {
                AddChip(lastRemovedIndex, chipId);
                lastRemovedIndex = -1;
            } else
            {
                Debug.LogError("ERROR: Tried to add to last removed index, but there was no last removed chip");
            }            
        }

        public void RemoveChipAt(int index)
        {
            GameObject.DestroyImmediate(dockedChips[index].gameObject);
            dockedChips[index] = null;
            lastRemovedIndex = index;
        }

        public void ShuffleChipsForward(int removedIndex)
        {
            // numchips-1 because we dont care if the last card was removed, nothing to shuffle            
            for (int i = removedIndex; i < numChips - 1; i++)
            {
                dockedChips[i] = dockedChips[i + 1];
                // parent then tween to 0
                dockedChips[i + 1].transform.SetParent(anchors[i]);
                dockedChips[i + 1].TweenToZero(null, 0.75f);
            }
        }



        // ==================================================================================
        //  Debug
        // ==================================================================================
        [ContextMenu("InitChipDock")]
        public void DebugInitChipDock()
        {
            short[] chipIds = new short[4];
            chipIds[0] = 0;
            chipIds[1] = 1;
            chipIds[2] = 2;
            chipIds[3] = 0;

            InitializeChipDockUI(chipIds.Length);
            for (int i = 0; i < chipIds.Length; i++)
            {
                Debug.LogFormat("Adding chip with id: {0}", chipIds[i]);
                AddChip(i, chipIds[i]);
            }
            SetNextChipPreview(2);
        }
    }
}
