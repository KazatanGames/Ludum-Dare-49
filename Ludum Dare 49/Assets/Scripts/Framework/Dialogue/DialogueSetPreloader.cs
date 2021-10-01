using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class DialogueSetPreloader : MonoBehaviour
    {
        [SerializeField]
        protected string[] setIds;

        private void Awake()
        {
            foreach (string setId in setIds)
            {
                if (setId != "")
                {
                    DialogueManager.INSTANCE.LoadSet(setId);
                }
            }
        }
    }
}