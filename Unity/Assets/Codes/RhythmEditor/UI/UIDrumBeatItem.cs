using TMPro;
using UnityEngine;

namespace RhythmEditor
{
    public class UIDrumBeatItem : MonoBehaviour
    {
        public TextMeshProUGUI DrumBeatID;
        public DrumBeatUIData DrumBeatUIData;

        public void UpdateDrumBeatItem()
        {
            DrumBeatID.text = $"{DrumBeatUIData.Int_0}";
            DrumBeatData drumBeatData;
            DrumBeatSceneData drumBeatSceneData;

            EditorDataManager.Instance.SearchDrumBeats(DrumBeatUIData.ID, out drumBeatData, out DrumBeatUIData,
                out drumBeatSceneData);

            float beatTime = drumBeatData.BeatTime;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(beatTime * UIConstValue.UIWidthScale, 0, 0);
            

        }


    }
}