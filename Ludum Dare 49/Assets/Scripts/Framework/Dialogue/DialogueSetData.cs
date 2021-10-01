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
    public class DialogueSetData
    {
        public int setPlayId;
        public string setId; // format scene-s000-event
        public string[] lineFiles;
        public string title;
        public DialogueSetContentsData[] contents;
    }
}