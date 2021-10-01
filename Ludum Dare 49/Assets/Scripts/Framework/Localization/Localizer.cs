namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    using TMPro;

    public class Localizer : MonoBehaviour
    {
        [SerializeField]
        protected string key;

        protected void Awake()
        {
            if (key != "")
            {
                string value = LocalizationManager.INSTANCE.__(key);
                foreach (TextMeshPro tmp in GetComponents<TextMeshPro>())
                {
                    tmp.SetText(value);
                }
                foreach (TextMeshProUGUI tmpu in GetComponents<TextMeshProUGUI>())
                {
                    tmpu.SetText(value);
                }
            }
        }
    }
}