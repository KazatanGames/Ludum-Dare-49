namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;

    public class LocalizationManager : SingletonMonoBehaviour<LocalizationManager>
    {
        public static string TranslationsPath = "Translations/";

        [SerializeField]
        protected List<LocalizationConfigSO> configs; // the first item will be the fallback
        [SerializeField]
        protected string forceOverrideCulture = "";

        public LocalizationConfigSO Config { get; protected set; }

        protected Dictionary<string, string> translationEntries;

        public string __(string key)
        {
            if (translationEntries.ContainsKey(key))
            {
                return translationEntries[key];
            }

            Debug.LogWarning("[LocalizationManager] Translation not found: " + key);

            return "";
        }

        protected override void Initialise()
        {
            DetermineLanguage();
            LoadLanguage();
        }

        protected void DetermineLanguage()
        {
            // try for a full culture name first
            string curName = forceOverrideCulture != "" ? forceOverrideCulture : CultureInfo.CurrentCulture.Name;
            curName = curName.Replace("_", "-");
            foreach (LocalizationConfigSO lc in configs)
            {
                if (lc.matchedCultureNames.Contains(curName))
                {
                    Config = lc;
                    return;
                }
            }

            // try for a two letter iso next
            foreach (LocalizationConfigSO lc in configs)
            {
                if (lc.matchedTwoLetterISOs.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
                {
                    Config = lc;
                    return;
                }
            }

            // there's no configs
            if (configs.Count == 0) return;

            // fallback to first config in list
            Config = configs[0];
        }

        protected void LoadLanguage()
        {
            if (Config == null)
            {
                Debug.LogWarning("[LocalizationManager] Failed to localize! (No config)");
                translationEntries = new Dictionary<string, string>();
                return;
            }

            string fullPath = Path.Combine(TranslationsPath, Config.filename);

            TextAsset asset = Resources.Load<TextAsset>(fullPath);
            if (asset == null)
            {
                Debug.LogError("[LocalizationManager] Failed to load asset.");
                return;
            }

            translationEntries = JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text);
        }
    }
}