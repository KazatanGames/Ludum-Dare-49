namespace KazatanGames.Framework
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AppConfig", menuName = "Initialisation Config", order = 99999)]
    public class AppConfigSO : ScriptableObject
    {
        public int version = 1;
        public string versionCode = "0.0.1";
        public string releaseBranch = "dev";
        public Utilities.SceneField initialScene;
        public bool debugMode = true;
        public bool skipIntros = false;
    }
}