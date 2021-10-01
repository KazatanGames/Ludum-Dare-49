using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public interface IOptimizedMonobehaviour
    {
        void OptimizedUpdate();

        // could add lateupdate

        // could add fixedupdate
    }
}