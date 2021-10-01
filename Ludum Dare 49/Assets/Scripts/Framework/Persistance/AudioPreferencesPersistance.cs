namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;

    public class AudioPreferencesPersistance : BasePersistance<AudioPreferencesData>
    {
        protected static int fileVersion = 1;
        protected static string filePathName = "audio-prefs";
        protected static string filePathExt = "dat";

        protected override string Filepath { get { return filePathName + "-" + fileVersion + "." + filePathExt; } }

        public void SetMusicEnabled(bool enabled)
        {
            Data.musicEnabled = enabled;
            Save();
        }

        public void SetSfxEnabled(bool enabled)
        {
            Data.sfxEnabled = enabled;
            Save();
        }
    }
}