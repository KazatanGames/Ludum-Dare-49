using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public abstract class BaseDialogueViewer : MonoBehaviour
    {
        public virtual void ShowLine(DialogueLoadedLine line)
        {
            // do stuff
        }
    }
}