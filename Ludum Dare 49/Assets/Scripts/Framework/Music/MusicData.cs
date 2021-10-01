namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    /**
     * Music Data
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * A key/value pair allowing music to be added with a catalog (category) which will allow easy
     * randomising/playlisting of the track being played based on the location within the App.
     */
    [CreateAssetMenu(fileName = "MusicData", menuName = "Music/MusicData", order = 1)]
    public class MusicData : ScriptableObject
    {
        public string musicId;
        public AudioClip clip;
    }
}