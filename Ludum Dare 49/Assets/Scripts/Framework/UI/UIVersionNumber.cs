namespace KazatanGames.Framework
{
    using UnityEngine;
    using TMPro;

    public class UIVersionNumber : MonoBehaviour
    {

        [SerializeField]
        protected TextMeshProUGUI text;

        void Start()
        {
            text.text = "v" + Application.version;
        }
    }
}