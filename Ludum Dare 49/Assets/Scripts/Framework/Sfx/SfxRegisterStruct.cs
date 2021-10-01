namespace KazatanGames.Framework
{
    using UnityEngine;
    using System;
    [Serializable]
    public struct SfxRegisterStruct
    {
        public SfxRegisterEnum target;
        public AudioClip clip;
    }
}