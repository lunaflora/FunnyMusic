using System;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmEditor
{
    public class UIPanelShow : MonoBehaviour
    {

        public CanvasGroup Panel;
        public Sprite[] SwitchIcons;
        public Image SwitchImage;

        private void Start()
        {
            DisablePanel();
            SwitchImage.sprite = SwitchIcons[1];
        }

        public void ShowPanel()
        {
            if (Panel.alpha != 1)
            {
                EnablePanel();
                SwitchImage.sprite = SwitchIcons[0];
            }
            else
            {
                DisablePanel();
                SwitchImage.sprite = SwitchIcons[1];
            }
        }

        public void HidePanel(bool changeIcon)
        {
            Panel.alpha = 0;
            Panel.interactable = false;
            Panel.blocksRaycasts = false;

            if (changeIcon)
            {
                SwitchImage.sprite = SwitchIcons[1];
            }
        }

        private void DisablePanel()
        {
            Panel.alpha = 0;
            Panel.interactable = false;
            Panel.blocksRaycasts = false;
        }

        private void EnablePanel()
        {
            Panel.alpha = 1;
            Panel.interactable = true;
            Panel.blocksRaycasts = true;
        }
    }
}