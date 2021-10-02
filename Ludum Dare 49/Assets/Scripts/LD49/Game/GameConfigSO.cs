using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game Config", order = 1)]
    public class GameConfigSO : ScriptableObject
    {
        public int dataWidth = 10;
        public int dataHeight = 10;

        public float visWidth = 100f;
        public float visHeight = 140f;

        public float outsideEnergy = 17f;
        public float minEnergy = -20f;
        public float maxEnergy = 105f;

        [Range(0f, 1f)]
        public float heatTransfer = 0.25f;
        [Range(0f, 1f)]
        public float heatTransferSide = 0.33f;
        [Range(0f, 1f)]
        public float heatTransferTop = 0.5f;

        [Range(1, 240)]
        public int solutionEnergyTicksPerSecond = 1;

        public HeatLevelStruct[] heatLevels = new HeatLevelStruct[] {
            new HeatLevelStruct()
            {
                addEnergy = 0f,
                name = "Off",
                rows = 1
            },
            new HeatLevelStruct()
            {
                addEnergy = 1f,
                name = "On",
                rows = 2
            }
        };
    }
}