namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;

    public class SfxAudioSource : SfxPlayer
    {
        [SerializeField]
        protected List<SfxRegisterStruct> sfxRegister;

        protected Dictionary<SfxRegisterEnum, AudioClip> registerDict;

        public AudioSource SfxPlayRegistered(SfxRegisterEnum sfx)
        {
            if (registerDict.ContainsKey(sfx))
            {
                return PlayAudioClipOnFreeSource(registerDict[sfx], 1f);
            }
            Debug.LogWarning("[SfxAudioSource] Tried to play '" + sfx + "' SFX but a clip is not registered!");
            return null;
        }

        public void SfxStopAll()
        {
            foreach (AudioSource source in sources)
            {
                source.Stop();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            registerDict = new Dictionary<SfxRegisterEnum, AudioClip>();
            foreach (SfxRegisterStruct srs in sfxRegister)
            {
                if (registerDict.ContainsKey(srs.target))
                {
                    Debug.LogWarning("[SfxAudioSource] Multiple registrations for SFX register: " + srs.target + ". Ignoring a registration");
                    continue;
                }

                if (srs.clip == null)
                {
                    Debug.LogWarning("[SfxAudioSource] Registration of " + srs.target + " has a missing clip. Ignoring the registration.");
                    continue;
                }

                registerDict[srs.target] = srs.clip;
            }
        }
    }
}