using UnityEngine;
using UnityEngine.UI;

namespace RhythmEditor
{
    /// <summary>
    /// 底部面板
    /// </summary>
    public class UIBottomPanel : MonoBehaviour
    {
        public RectTransform BottomPanel;
        public Image ButtonImage;
        public Sprite[] SwitchSprites;

        /// <summary>
        /// 开关底部面板
        /// </summary>
        public void ToggleBottomPanel()
        {
            if (BottomPanel.anchoredPosition == new Vector2(0, 50))
            {
                BottomPanel.anchoredPosition = new Vector2(0, 300);
                ButtonImage.sprite = SwitchSprites[0];
            }
            else
            {
                BottomPanel.anchoredPosition = new Vector2(0, 50);
                ButtonImage.sprite = SwitchSprites[1];
            }
        }
        

    }
}