using UnityEngine;
using DigitalRubyShared;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using BattleNetwork.Events;

namespace BattleNetwork.Battle.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(FingersScript))]
    public class TouchGestureManager : MonoBehaviour
    {
        [SerializeField] private DraggedUIEvent draggedUIEvent;
        [SerializeField] private SwipeGestureEvent swipeGestureEvent;
        [SerializeField] private TapGestureEvent tapGestureEvent;
        [SerializeField] private RectTransform swipeArea;

        [SerializeField] private bool debugLogs;

        private GraphicRaycaster graphicRaycaster;

        private TapGestureRecognizer tapGesture;
        private SwipeGestureRecognizer swipeGesture;
        private LongPressGestureRecognizer longPressGesture;
        
        private IDraggableUI draggedObject;

        private void Start()
        {
            graphicRaycaster = gameObject.GetComponent<GraphicRaycaster>();
            SetupTouchGestures();
        }

        private void SetupTouchGestures()
        {
            CreateTapGesture();
            CreateSwipeGestures();
            CreateLongPressGesture();

            FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void CreateLongPressGesture()
        {
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.MinimumDurationSeconds = 0f;
            longPressGesture.MaximumNumberOfTouchesToTrack = 1;
            longPressGesture.StateUpdated += LongPressGestureCallback;
            FingersScript.Instance.AddGesture(longPressGesture);
        }
        private void CreateSwipeGestures()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.MinimumDistanceUnits = 0.5f;
            swipeGesture.MinimumSpeedUnits = 2.0f;
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                LogFormat("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                tapGestureEvent.Raise(new Vector2(gesture.FocusX, gesture.FocusY));
            }
        }

        private void LongPressGestureCallback(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Began:
                    BeginDrag(gesture.FocusX, gesture.FocusY);
                    break;
                case GestureRecognizerState.Executing:
                    DragTo(gesture.FocusX, gesture.FocusY);
                    break;
                case GestureRecognizerState.Ended:
                    EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
                    break;
                default:
                    break;
            }
        }

        private void SwipeGestureCallback(GestureRecognizer gesture)
        {            
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //bool validStart = RectTransformUtility.RectangleContainsScreenPoint(
                //    swipeArea, 
                //    new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY)
                //);
                //if (validStart)
                //{
                    LogFormat("Ending swipe at : {0},{1}", swipeGesture.StartFocusX, swipeGesture.StartFocusY);
                    swipeGestureEvent.Raise(swipeGesture.EndDirection);
                //}
            }
        }

        private void BeginDrag(float screenX, float screenY)
        {
            LogFormat("BeginDrag at {0}, {1}", screenX, screenY);

            PointerEventData pointer = new PointerEventData(null);
            pointer.position = new Vector2(screenX, screenY);

            List<RaycastResult> hits = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointer, hits);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    IDraggableUI draggable = hits[i].gameObject.GetComponent<IDraggableUI>();
                    if (draggable != null)
                    {
                        draggedObject = draggable;                        
                        draggable.DragStarted();
                        draggedUIEvent.Raise(DraggedUIEvent.State.Started, draggable);
                        break;
                    }
                }
            }
            else
            {
                longPressGesture.Reset();
            }
        }

        private void DragTo(float screenX, float screenY)
        {
            if (draggedObject != null)
            {
                LogFormat("DragTo at {0}, {1}, target: {2}", screenX, screenY, draggedObject.GetGameObject().name);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggedObject.GetRectTransform(),
                    new Vector3(screenX, screenY, 0.0f),
                    null,
                    out var localPosition
                );
                localPosition = localPosition + draggedObject.Offset();
                draggedObject.GetRectTransform().position = draggedObject.GetRectTransform().TransformPoint(localPosition);
            }            
        }

        private void EndDrag(float velocityXScreen, float velocityYScreen)
        {
            if (draggedObject != null)
            {
                LogFormat("EndDrag speed {0}, {1}, target: {2}", velocityXScreen, velocityYScreen, draggedObject.GetGameObject().name);

                draggedObject.DragEnded();
                draggedUIEvent.Raise(DraggedUIEvent.State.Ended, draggedObject);
                draggedObject = null;
            }
        }

        private bool? CaptureGestureHandler(GameObject obj)
        {
            // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
            if (obj.name.StartsWith("PassThrough"))
            {
                // allow the pass through for any element named "PassThrough*"               
                LogFormat("PassThrough {0}", obj.name);
                return false;
            }
            else if (obj.name.StartsWith("NoPass"))
            {
                // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
                // the triple tap gesture can tap on it
                LogFormat("Hit a NoPass object {0}", obj.name);
                return true;
            }

            LogFormat("Default {0}", obj.name);
            // fall-back to default behavior for anything else
            return false;
        }

        private void LogFormat(string format, params object[] args)
        {
            if (debugLogs)
            {
                Debug.LogFormat(format, args);
            }
        }
    }
}
