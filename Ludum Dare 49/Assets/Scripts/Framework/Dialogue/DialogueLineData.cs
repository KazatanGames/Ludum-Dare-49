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
    public class DialogueLineData
    {
        public string lineId; // format: scene-l000-character
        public string text;
        public string waveFile;
        public string actor;
        public string actorName;
        public float volumeAdjust; // negative means down, positive up
        //public VolumeEnum volume;
        //public EmotionEnum emotion;
        //public bool preload;
        //public string audioCueGroup; // for random dialogue based on 'cue' events
    }
}