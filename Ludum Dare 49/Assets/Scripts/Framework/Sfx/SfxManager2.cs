using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class SfxManager2 : SingletonMonoBehaviour<SfxManager2>
    {

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

        public event Action<bool> OnEnabledChange;
        protected void SetEnabled(bool value)
        {
            if (enabled == value) return;
            enabled = value;
            OnEnabledChange?.Invoke(enabled);
        }

        public event Action<float> OnVolumeChange;
        protected void SetVolume(float value)
        {
            if (volume == value) return;
            volume = value;
            OnVolumeChange?.Invoke(volume);
        }
    }
}