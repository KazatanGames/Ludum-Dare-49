using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class DialogueLoadedLine
    {
        public string lineId; // format: scene-l000-character
        public string text;
        public string actor;
        public string animationTriggerName;
        public string actorName;
        public float prePause;
        public float volumeMulti;
        public AudioClip audioClip;

        public DialogueLoadedLine(DialogueLineData dld, DialogueSetContentsData dscd)
        {
            lineId = dld.lineId;
            text = dld.text;
            actor = dld.actor;
            actorName = dld.actorName;
            prePause = dscd.prePause;
            animationTriggerName = dscd.animationTriggerName;

            volumeMulti = 1f + (float.IsNaN(dld.volumeAdjust) ? 0 : dld.volumeAdjust);

            if (dld.waveFile != "")
            {
                audioClip = Resources.Load<AudioClip>(dld.waveFile);
                if (audioClip == null)
                {
                    Debug.LogWarning("[DialogueLoadedLine()] - File not found: " + dld.waveFile);
                }
            }

            Debug.Log(this);
        }

        public void Destroy()
        {
            if (audioClip != null) UnityEngine.Object.Destroy(audioClip);
        }

        public override string ToString() => $"[DialogueLoadedLine] lineId: {lineId}" +
            $"\ntext: {text}\nactor: {actor}\nprePause: {prePause:F2}\nvolumeMulti: {volumeMulti}" +
            (audioClip != null ? $"\naudioClip(${audioClip.name}): {audioClip.length}s" : "\nNo audio clip.");
    }
}