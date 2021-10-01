using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class MusicAudioSource : MonoBehaviour
    {
        [SerializeField]
        protected float volumeAdjustmentTime = 0.25f;

        public AudioSource Source { get; protected set; }
        public bool Playing { get; protected set; }
        public bool Looping { get; protected set; }
        public float Volume { get; protected set; }
        public float VolumeMultiplier { get; protected set; }

        protected Coroutine volumeAdjustmentCoroutine;

        private void Awake()
        {
            if (Source == null) Source = gameObject.AddComponent<AudioSource>();
        }

        public void Play(AudioClip clip, bool loop, float volume, bool fadeInDefault)
        {
            Play(clip, loop, volume, fadeInDefault ? volumeAdjustmentTime : 0);
        }
        public void Play(AudioClip clip, bool loop, float volume, float fadeInTime)
        {
            Debug.Log($"[MusicAudioSource].Play({clip}, {loop}, {volume}, {fadeInTime}) wasPlaying? = {Source.isPlaying}");
            Source.clip = clip;
            Source.volume = fadeInTime > 0 ? 0 : volume;
            Source.loop = loop;
            Looping = loop;
            Volume = volume;
            VolumeMultiplier = 1f;
            Resume(fadeInTime);
        }

        public void SetVolume(float newVolume, bool defaultAdjustTime)
        {
            SetVolume(newVolume, defaultAdjustTime ? volumeAdjustmentTime : 0);
        }
        public void SetVolume(float newVolume, float adjustTime)
        {
            Volume = newVolume;
            AdjustSourceVolume(Volume * VolumeMultiplier, adjustTime);
        }

        public void SetVolumeMultiplier(float multiplier, bool defaultAdjustTime)
        {
            SetVolumeMultiplier(multiplier, defaultAdjustTime ? volumeAdjustmentTime : 0);
        }
        public void SetVolumeMultiplier(float multiplier, float adjustTime)
        {
            VolumeMultiplier = multiplier;
            AdjustSourceVolume(Volume * VolumeMultiplier, adjustTime);
        }

        public void Stop(bool fadeOutDefault)
        {
            Stop(fadeOutDefault ? volumeAdjustmentTime : 0);
        }
        public void Stop(float fadeOutTime)
        {
            Debug.Log($"[MusicAudioSource].Stop({fadeOutTime})");

            Playing = false;
            if (fadeOutTime > 0)
            {
                SetVolume(0, fadeOutTime);
            } else
            {
                Source.Stop();
                DispatchStopped();
            }
        }

        public void Resume(bool fadeInDefault)
        {
            Resume(fadeInDefault ? volumeAdjustmentTime : 0);
        }
        public void Resume(float fadeInTime)
        {
            Debug.Log($"[MusicAudioSource].Resume({fadeInTime})");
            Playing = true;
            if (fadeInTime > 0)
            {
                SetVolume(Volume, fadeInTime);
            }
            if (!Source.isPlaying) Source.Play();
        }

        public bool IsAvailable => !Playing && !Source.isPlaying;

        public event Action<MusicAudioSource> OnStopped;
        protected void DispatchStopped()
        {
            OnStopped?.Invoke(this);
        }

        protected void AdjustSourceVolume(float volume, float adjustTime)
        {
            if (volumeAdjustmentCoroutine != null) StopCoroutine(volumeAdjustmentCoroutine);

            if (adjustTime > 0)
            {
                volumeAdjustmentCoroutine = StartCoroutine(AdjustSourceVolumeOverTime(volume, adjustTime));
            }
            else
            {
                Source.volume = volume;
            }
        }

        protected IEnumerator AdjustSourceVolumeOverTime(float to, float time)
        {
            float from = Source.volume;
            float startTime = Time.time;
            Debug.Log($"[MusicAudioSource].AdjustSourceVolumeOverTime() '{from}' to '{to}' over '{time}s'");
            while((Time.time - startTime) < time)
            {
                Source.volume = Mathf.Lerp(from, to, Mathf.Clamp((Time.time - startTime) / time, 0, 1));
                yield return new WaitForEndOfFrame();
            }
            if (!Playing)
            {
                Source.Stop();
                DispatchStopped();
            }
            Source.volume = to;
            volumeAdjustmentCoroutine = null;
        }
    }
}