namespace KazatanGames.Framework
{
    using UnityEngine;
    public class SfxGlobal : MonoBehaviour
    {
        public void SfxPlayClip(AudioClip clip) { SfxManager.INSTANCE.SfxPlayClip(clip); }

        public void SfxPlayUIPanelIn() { SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Panel_In); }
        public void SfxPlayUIPanelOut() { SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Panel_Out); }
        public void SfxPlayUIClickOk() { SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Ok); }
        public void SfxPlayUIClickBad() { SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Bad); }
        public void SfxPlayUIClickIllegal() { SfxManager.INSTANCE.SfxPlayRegistered(SfxRegisterEnum.UI_Click_Illegal); }
    }
}