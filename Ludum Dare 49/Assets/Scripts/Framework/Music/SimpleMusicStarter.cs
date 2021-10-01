using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class SimpleMusicStarter : MonoBehaviour
    {
        [SerializeField]
        protected string musicId;
        [SerializeField]
        protected float volume;
        [SerializeField]
        protected float fadeInTime;

        private void Awake()
        {
            if (musicId != "" && volume > 0) MusicManager2.INSTANCE.PlayRegisteredMusic(musicId, volume, true, fadeInTime);
            Destroy(gameObject);
        }
    }
}