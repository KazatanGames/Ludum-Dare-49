namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Globalization;

    [CreateAssetMenu(fileName = "Config", menuName = "Localization", order = 0)]
    public class LocalizationConfigSO : ScriptableObject
    {
        public List<string> matchedCultureNames;
        public List<string> matchedTwoLetterISOs;
        public string filename;
    }
}