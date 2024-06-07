using Core;
using DigitalRubyShared;
using FLib;
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

        #endregion

        public override void Initialize()
        {
            TouchSimulation.Enable();
            InitGestureRecognizer();
        }

        private void InitGestureRecognizer()
        {
            CreateTapGesture();
        }

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
                TouchNote(gesture.FocusX, gesture.FocusY, 0.2f);
            }

        }
        
        private void TouchNote(float screenX, float screenY, float radius)
        {
            Vector3 pos = new Vector3(screenX, screenY, 0.0f);
            Ray ray = Camera.main.ScreenPointToRay(pos);
            // print(touch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var t = hit.rigidbody.GetComponent<NoteView>();
                if (!t) return;
                GameObject.Destroy(t.transform.gameObject);
            }
            
            /*Vector3 pos = new Vector3(screenX, screenY, 0.0f);
            pos = Camera.main.ScreenToWorldPoint(pos);

            
            RaycastHit2D[] hits = Physics2D.CircleCastAll(pos, radius, Vector2.zero);
            foreach (RaycastHit2D h in hits)
            {
                GameObject.Destroy(h.transform.gameObject);
            }*/
        }
    }
}