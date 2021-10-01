using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class SfxLocalPlayer2 : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource audioSource;

        [SerializeField]
        public bool Enabled
        {
            get => enabled;
            set => SetEnabled(value);
        }

        [SerializeField]
        public float Volume
        {
            get => volume;
            set => SetVolume(value);
        }

        protected new bool enabled;
        protected float volume;
        protected bool play;
        protected bool loop;

        protected void ManageSfxEnabledChange(bool enabled)
        {
            UpdateEnabled();
        }

        protected void ManageSfxVolumeChange(float volume)
        {
            UpdateVolume();
        }

        protected void SetEnabled(bool value)
        {
            if (enabled == value) return;
            enabled = value;
            UpdateEnabled();
        }

        protected void SetVolume(float value)
        {
            if (volume == value) return;
            volume = value;
            UpdateVolume();
        }

        protected void UpdateEnabled()
        {
            audioSource.enabled = Enabled && SfxManager2.INSTANCE.Enabled;
        }

        protected void UpdateVolume()
        {
            audioSource.volume = Volume * SfxManager2.INSTANCE.Volume;
        }

        private void OnEnable()
        {
            SfxManager2.INSTANCE.OnEnabledChange += ManageSfxEnabledChange;
            SfxManager2.INSTANCE.OnVolumeChange += ManageSfxVolumeChange;
            ManageSfxEnabledChange(SfxManager2.INSTANCE.Enabled);
            ManageSfxVolumeChange(SfxManager2.INSTANCE.Volume);
        }

        private void OnDisable()
        {
            SfxManager2.INSTANCE.OnEnabledChange -= ManageSfxEnabledChange;
            SfxManager2.INSTANCE.OnVolumeChange -= ManageSfxVolumeChange;
        }

        private void Awake()
        {
            enabled = audioSource.enabled;
            play = audioSource.playOnAwake;
            loop = audioSource.loop;
        }
    }
}