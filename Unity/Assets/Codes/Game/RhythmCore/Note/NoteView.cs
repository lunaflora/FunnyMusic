using UnityEditor;
using UnityEngine;

namespace Game
{
    public class NoteView : MonoBehaviour
    {
        #region Data
       
        public float SwipeThreshold = 0.5f;
        public Note.SwipeDirection SwipeDirection = Note.SwipeDirection.Right;

        #endregion


        public bool HandeSwipe(Vector3 swipeDiff)
        {
            bool addCombo = false;
            switch (SwipeDirection)
            {
                case Note.SwipeDirection.Up:
                    addCombo = swipeDiff.y >= SwipeThreshold;
                    break;
                case Note.SwipeDirection.Down:
                    addCombo = swipeDiff.y <= SwipeThreshold;
                    break;
                case Note.SwipeDirection.Left:
                    addCombo = swipeDiff.x <= -SwipeThreshold;
                    break;
                case Note.SwipeDirection.Right:
                    addCombo = swipeDiff.x >= SwipeThreshold;
                    break;
            }

            return addCombo;
        }
    }
}