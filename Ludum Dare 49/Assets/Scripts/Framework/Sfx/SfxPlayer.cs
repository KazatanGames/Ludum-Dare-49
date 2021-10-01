namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;

    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource[] sources;

        public AudioSource SfxPlayClip(AudioClip clip)
        {
            return PlayAudioClipOnFreeSource(clip, 1f);
        }
        public AudioSource PlayClip(AudioClip clip, float volume)
        {
            return PlayAudioClipOnFreeSource(clip, volume);
        }

        protected AudioSource PlayAudioClipOnFreeSource(AudioClip clip, float volume)
        {
            if (!SfxManager.INSTANCE.SfxEnabled) return null;
            foreach (AudioSource source in sources)
            {
                if (!source.isPlaying)
                {
                    PlayAudioClipOnSource(clip, volume, source);
                    return source;
                }
            }

            Debug.LogWarning("[SfxPlayer].PlayAudioClipOnFreeSource() All sources are already playing. Maybe you need more sources?\nNew Clip: " + clip.name);
            foreach (AudioSource source in sources)
            {
                Debug.LogWarning("Source was playing: " + source.clip.name);
            }

            return null;
        }

        protected void PlayAudioClipOnSource(AudioClip clip, float volume, AudioSource source)
        {
            if (!SfxManager.INSTANCE.SfxEnabled) return;
            if (source.isPlaying)
            {
                Debug.LogWarning("[SfxPlayer].PlayAudioClipSource() Source was playing: " + source.clip.name);
            }
            source.Stop();
            source.clip = clip;
            source.volume = volume;
            source.pitch = 1f;
            source.Play();
        }

        protected virtual void Awake()
        {
            if (sources.Length == 0)
            {
                Debug.LogWarning("SfxPlayer without any attached sources!");
            }
        }
    }
}