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
        [SerializeField]
        private DraggedUIEvent draggedUIEvent;
        [SerializeField]
        private SwipeGestureEvent swipeGestureEvent;
        [SerializeField]
        private RectTransform swipeArea;


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
            longPressGesture.MinimumDurationSeconds = 0.025f;
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
                Debug.LogFormat("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                // TODO add TappedUIEvent or something similar
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
                bool validStart = RectTransformUtility.RectangleContainsScreenPoint(
                    swipeArea, 
                    new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY)
                );
                if (validStart)
                {
                    swipeGestureEvent.Raise(swipeGesture.EndDirection);
                }
            }
        }

        private void BeginDrag(float screenX, float screenY)
        {
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
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggedObject.GetRectTransform(),
                    new Vector3(screenX, screenY, 0.0f),
                    null,
                    out var localPosition
                );
                draggedObject.GetRectTransform().position = draggedObject.GetRectTransform().TransformPoint(localPosition);
            }            
        }

        private void EndDrag(float velocityXScreen, float velocityYScreen)
        {
            if (draggedObject != null)
            {
                draggedObject.DragEnded();
                draggedUIEvent.Raise(DraggedUIEvent.State.Ended, draggedObject);
                draggedObject = null;
            }
        }

        private static bool? CaptureGestureHandler(GameObject obj)
        {
            // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
            if (obj.name.StartsWith("PassThrough"))
            {
                // allow the pass through for any element named "PassThrough*"
                return false;
            }
            else if (obj.name.StartsWith("NoPass"))
            {
                // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
                // the triple tap gesture can tap on it
                return true;
            }

            // fall-back to default behavior for anything else
            return null;
        }
    }
}
