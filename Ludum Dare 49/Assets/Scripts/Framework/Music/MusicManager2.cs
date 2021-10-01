using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * MusicManager2 is a simplified audio player which will loop sets of AudioClips.
 * It supports global volume adjustment including ducking.
 * 
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class MusicManager2 : SingletonMonoBehaviour<MusicManager2>
    {

        [SerializeField]
        protected float globalVolumeMultiplier = 1f;
        [SerializeField]
        protected float duckingMultiplier = 0.1f;
        [SerializeField]
        protected float duckingTime = 0.25f;
        [SerializeField]
        protected float unduckingTime = 1f;
        [Header("Registered Music Clips")]
        [SerializeField]
        protected List<MusicData> registeredMusic;

        protected Queue<MusicAudioSource> audioSourcePool;
        protected Dictionary<string, MusicAudioSource> audioSourcesInUse;

        protected HashSet<object> duckingObjects;

        protected Dictionary<string, AudioClip> registeredMusicDictionary;

        /**
         * TODO
         * 
         * - Listen to settings data
         * 
         */

        protected override void Initialise()
        {
            base.Initialise();
            audioSourcePool = new Queue<MusicAudioSource>();
            audioSourcesInUse = new Dictionary<string, MusicAudioSource>();
            duckingObjects = new HashSet<object>();
            registeredMusicDictionary = new Dictionary<string, AudioClip>();
            foreach(MusicData md in registeredMusic)
            {
                if (registeredMusicDictionary.ContainsKey(md.musicId))
                {
                    Debug.LogWarning($"[MusicManager2].Initialise() - Duplicate music registration for id: {md.musicId}");
                    registeredMusicDictionary[md.musicId] = md.clip;
                } else
                {
                    registeredMusicDictionary.Add(md.musicId, md.clip);
                }
            }
        }

        public void PlayRegisteredMusic(string musicId, float volume, bool loop)
        {
            AudioClip clip = registeredMusicDictionary.ContainsKey(musicId) ? registeredMusicDictionary[musicId] : null;
            if (clip == null)
            {
                Debug.LogWarning($"[MusicManager2].PlayRegisteredMusic({musicId}) - Cannot play music as id isn't registered.");
                return;
            }

            MusicAudioSource mas = GetSourcePlayingMusic(musicId);
            if (mas != null)
            {
                if (!mas.Playing) mas.Resume(true);
                // a source is already playing this, stop others
                StopExistingSourcesExcept(musicId);
            } else
            {
                mas = FindOrCreateAudioSource();
                StopExistingSources();
                audioSourcesInUse.Add(musicId, mas);
                mas.Play(clip, loop, volume * globalVolumeMultiplier * AppManager.INSTANCE.AppModel.audioPreferences.Data.globalVolume, true);
            }
            mas.OnStopped += ManageOnMusicAudioSourceStopped;
        }
        public void PlayRegisteredMusic(string musicId, float volume, bool loop, float crossfadeTime)
        {
            AudioClip clip = registeredMusicDictionary.ContainsKey(musicId) ? registeredMusicDictionary[musicId] : null;
            if (clip == null)
            {
                Debug.LogWarning($"[MusicManager2].PlayRegisteredMusic({musicId}) - Cannot play music as id isn't registered.");
                return;
            }

            MusicAudioSource mas = GetSourcePlayingMusic(musicId);
            if (mas != null)
            {
                if (!mas.Playing) mas.Resume(crossfadeTime);
                // a source is already playing this, stop others
                StopExistingSourcesExcept(musicId, crossfadeTime);
            }
            else
            {
                mas = FindOrCreateAudioSource();
                StopExistingSources(crossfadeTime);
                audioSourcesInUse.Add(musicId, mas);
                mas.Play(clip, loop, volume * globalVolumeMultiplier * AppManager.INSTANCE.AppModel.audioPreferences.Data.globalVolume, crossfadeTime);
            }
            mas.OnStopped += ManageOnMusicAudioSourceStopped;
        }

        public void StopExistingSources()
        {
            foreach (MusicAudioSource mas in audioSourcesInUse.Values)
            {
                if (mas.Playing) mas.Stop(true);
            }
        }
        public void StopExistingSources(float fadeOutTime)
        {
            foreach (MusicAudioSource mas in audioSourcesInUse.Values)
            {
                if (mas.Playing) mas.Stop(fadeOutTime);
            }
        }

        public void StopExistingSourcesExcept(string musicId)
        {
            foreach (KeyValuePair<string, MusicAudioSource> kvp in audioSourcesInUse)
            {
                if (kvp.Key != musicId && kvp.Value.Playing) kvp.Value.Stop(true);
            }
        }
        public void StopExistingSourcesExcept(string musicId, float fadeOutTime)
        {
            foreach (KeyValuePair<string, MusicAudioSource> kvp in audioSourcesInUse)
            {
                if (kvp.Key != musicId && kvp.Value.Playing) kvp.Value.Stop(fadeOutTime);
            }
        }

        public void AddDucker(object ducker)
        {
            if (duckingObjects.Count == 0)
            {
                foreach (MusicAudioSource mas in audioSourcesInUse.Values)
                {
                    mas.SetVolumeMultiplier(duckingMultiplier, duckingTime);
                }
            }
            duckingObjects.Add(ducker);
        }

        public void RemoveDucker(object ducker)
        {
            if (duckingObjects.Count > 0)
            {
                duckingObjects.Remove(ducker);
                if (duckingObjects.Count == 0)
                {
                    foreach (MusicAudioSource mas in audioSourcesInUse.Values)
                    {
                        mas.SetVolumeMultiplier(1f, unduckingTime);
                    }
                }
            }
        }

        protected void ManageOnMusicAudioSourceStopped(MusicAudioSource source)
        {
            source.OnStopped -= ManageOnMusicAudioSourceStopped;
            foreach (KeyValuePair<string, MusicAudioSource> kvp in audioSourcesInUse)
            {
                if (kvp.Value == source)
                {
                    audioSourcesInUse.Remove(kvp.Key);
                    break;
                }
            }
            audioSourcePool.Enqueue(source);
        }

        protected MusicAudioSource GetSourcePlayingMusic(string musicId)
        {
            // check if an audio source is already playing this music id
            foreach (string inUseMusicId in audioSourcesInUse.Keys)
            {
                if (inUseMusicId == musicId) return audioSourcesInUse[inUseMusicId];
            }
            return null;
        }

        protected MusicAudioSource FindOrCreateAudioSource()
        {
            if (audioSourcePool.Count > 0) return audioSourcePool.Dequeue();

            // no available MAS found, create one
            GameObject masGO = new GameObject("Music Audio Source");
            masGO.transform.parent = transform;
            MusicAudioSource newMas = masGO.AddComponent<MusicAudioSource>();
            return newMas;
        }

    }
}