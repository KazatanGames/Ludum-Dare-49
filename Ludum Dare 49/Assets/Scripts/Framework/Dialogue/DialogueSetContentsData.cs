using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    [Serializable]
    public class DialogueSetContentsData
    {
        public bool isSet;
        public string setId; // is also path
        public bool playOnlyOne;
        public bool random;

        public string lineId;
        public float prePause;
        public string animationTriggerName;
    }
}