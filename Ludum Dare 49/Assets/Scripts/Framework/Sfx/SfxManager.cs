namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    /**
     * Sfx Manager
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * The Sfx Manager allows App wide control of non-positional sound effects and also
     * should be checked to determine if positional sfx should play.
     */
    public class SfxManager : SingletonMonoBehaviour<SfxManager>
    {
        [SerializeField]
        protected SfxAudioSource globalSource;

        public bool SfxEnabled
        {
            get { return AppManager.INSTANCE.AppModel.audioPreferences.Data.sfxEnabled; }
        }

        public void ToggleSfxEnabled()
        {
            AppManager.INSTANCE.AppModel.audioPreferences.SetSfxEnabled(!SfxEnabled);

            if (!SfxEnabled)
            {
                SfxStopAll();
            }
        }

        public AudioSource SfxPlayRegistered(SfxRegisterEnum sfx)
        {
            return globalSource.SfxPlayRegistered(sfx);
        }

        public AudioSource SfxPlayClip(AudioClip clip)
        {
            return globalSource.SfxPlayClip(clip);
        }
        public AudioSource SfxPlayClip(AudioClip clip, float volume)
        {
            return globalSource.PlayClip(clip, volume);
        }

        public void SfxPlayClip(AudioClip clip, AudioSource source)
        {
            if (!SfxEnabled) return;
            source.clip = clip;
            source.Play();
        }
        public void SfxPlayClip(AudioClip clip, AudioSource source, float volume)
        {
            source.volume = volume;
            SfxPlayClip(clip, source);
        }

        public void SfxStopAll()
        {
            globalSource.SfxStopAll();
        }

        protected override void Initialise()
        {
        }
    }
}