using Core;
using DigitalRubyShared;
using FLib;
using ParadoxNotion;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Game
{
    public class InputSystemHandler : Singleton<InputSystemHandler>
    {
        #region GestureRecognizer

        /// <summary>
        /// 单击
        /// </summary>
        private TapGestureRecognizer tapGesture;
        /// <summary>
        /// 滑动
        /// </summary>
        private SwipeGestureRecognizer swipeGesture;

        #endregion

        public override void Initialize()
        {
            TouchSimulation.Enable();
            InitGestureRecognizer();
        }

        private void InitGestureRecognizer()
        {
            CreateTapGesture();
            CreateSwipeGesture();
        }
        
        #region InputEvent

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
          
            if (gesture.State == GestureRecognizerState.Ended)
            {
                FDebug.Print($"Single tapped at {gesture.FocusX}, {gesture.FocusY}");
                HandleTouchNote(gesture.FocusX, gesture.FocusY, 0.2f);

             
            }

        }

        
        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void SwipeGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                HandleSwipeNote(gesture.FocusX, gesture.FocusY);
             
            }
        }
        

        #endregion
       
        private void HandleTouchNote(float screenX, float screenY, float radius)
        {
            NotesContainer.Instance.CurrentNoteView = OverlapNote(screenX, screenY,0.0f);
            
            if (NotesContainer.Instance.CurrentNoteView != null)
            {
                GameObject.Destroy(NotesContainer.Instance.CurrentNoteView.gameObject);
                NotesContainer.Instance.CurrentNoteView = null;
            }
        }

        private void HandleSwipeNote(float endX, float endY)
        {
            Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
            Vector3 startWorld = Camera.main.ScreenToWorldPoint(start);
            Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(endX, endY));
            float distance = Vector3.Distance(startWorld, endWorld);

            Vector3 swipeDiff = endWorld - startWorld;
            
            Debug.Log(  string.Format("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}, Distance : {6}", swipeGesture.StartFocusX, swipeGesture.StartFocusY, 
                swipeGesture.FocusX, swipeGesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY,distance));

            //滑动音符应该是通过滑动到判定线来判断命中
            RaycastHit2D noteCol = Physics2D.Linecast(startWorld, endWorld);

            if (noteCol.collider != null)
            {
                NotesContainer.Instance.CurrentNoteView = noteCol.collider.gameObject.GetComponent<NoteView>();
            }
            
            Debug.Log($"{swipeDiff}");
            if (NotesContainer.Instance.CurrentNoteView != null)
            {
               
                bool addCombo = NotesContainer.Instance.CurrentNoteView.HandeSwipe(swipeDiff);
                if (addCombo)
                {
                    GameObject.Destroy(NotesContainer.Instance.CurrentNoteView.gameObject);
                    NotesContainer.Instance.CurrentNoteView = null;
                }

            }
            
          
        }

        private NoteView OverlapNote(float screenX, float screenY, float radius)
        {
            Vector3 pos = new Vector3(screenX, screenY, 0.0f);
            pos = Camera.main.ScreenToWorldPoint(pos);

            Collider2D[] notes = Physics2D.OverlapPointAll(pos);

            if (notes.Length > 0)
            {
                NotesContainer.Instance.CurrentNoteView = notes[0].GetComponent<NoteView>();
                return NotesContainer.Instance.CurrentNoteView;
            }

            return null;
        }
    }
}