using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    [Serializable]
    public struct HeatLevelStruct
    {
        public string name;
        public float addEnergy;
        [Range(0f, 1f)]
        public float width;
    }
}