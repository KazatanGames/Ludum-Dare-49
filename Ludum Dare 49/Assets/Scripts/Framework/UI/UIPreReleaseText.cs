namespace KazatanGames.Framework
{
    using UnityEngine;
    using TMPro;

    public class UIPreReleaseText : MonoBehaviour
    {

        [SerializeField]
        protected TextMeshProUGUI text;

        void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            string txt = "This is a prerelease version.";
            text.text = txt;
        }
    }
}