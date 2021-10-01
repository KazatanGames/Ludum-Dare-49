namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;
    public class SfxStatic
    {
        public static AudioSource PlayClip(AudioClip clip) { return SfxManager.INSTANCE.SfxPlayClip(clip); }
        public static AudioSource PlayClip(AudioClip clip, float volume) { return SfxManager.INSTANCE.SfxPlayClip(clip, volume); }

        public static AudioSource PlayUIPanelIn() { return SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Panel_In); }
        public static AudioSource PlayUIPanelOut() { return SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Panel_Out); }
        public static AudioSource PlayUIClickOk() { return SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Ok); }
        public static AudioSource PlayUIClickBad() { return SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Bad); }
        public static AudioSource PlayUIClickIllegal() { return SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Illegal); }

        public static AudioSource PlayRandomClip(List<AudioClip> clips) { return PlayClip(clips[Random.Range(0, clips.Count)]); }
    }
}