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
        [Min(1)]
        public int rows;
    }
}