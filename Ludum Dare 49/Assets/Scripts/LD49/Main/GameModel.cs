using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class GameModel : Singleton<GameModel>
    {
        public GameConfigSO Config { get; protected set; }

        public SolutionDataPoint[] SolutionDataPoints { get; protected set; }

        protected float solutionEnergyTickTime;
        protected float solutionEnergyTickTimeRem = 0f;

        public int CurrentHeatLevel { get; protected set; } = 0;

        public void Initialise(GameConfigSO config)
        {
            Config = config;

            solutionEnergyTickTime = 1f / config.solutionEnergyTicksPerSecond;

            SolutionDataPoints = new SolutionDataPoint[Config.dataWidth * Config.dataHeight];

            // create
            for (int y = 0; y < Config.dataHeight; y++)
            {
                for (int x = 0; x < Config.dataWidth; x++)
                {
                    bool side = x == 0 || x == (Config.dataWidth - 1);
                    bool top = y == (Config.dataHeight - 1);
                    SolutionDataPoint sdp = new SolutionDataPoint(x, y, side, top);
                    SolutionDataPoints[(x * Config.dataHeight) + y] = sdp;
                }
            }

            // set neighbours
            foreach(SolutionDataPoint sdp in SolutionDataPoints)
            {
                sdp.FindNeighbours(SolutionDataPoints);
            }
        }

        public void Update(float time)
        {
            solutionEnergyTickTimeRem += time;

            while (solutionEnergyTickTimeRem >= solutionEnergyTickTime)
            {
                solutionEnergyTickTimeRem -= solutionEnergyTickTime;
                SolutionEnergyTick();
            }
        }

        public void SetHeatLevel(int heatLevel)
        {
            CurrentHeatLevel = Mathf.Clamp(heatLevel, 0, Config.heatLevels.Length - 1);
        }

        protected void SolutionEnergyTick()
        {
            HeatLevelStruct heatLevel = Config.heatLevels[CurrentHeatLevel];
            int heatedPoints = Mathf.CeilToInt(heatLevel.width * Config.dataWidth);
            int heatXMin = (Config.dataWidth - heatedPoints) / 2;
            int heatXMax = heatXMin + heatedPoints - 1;

            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            {
                sdp.CalculateEnergyTransfer();
                if (sdp.ShouldBeHeated(heatXMin, heatXMax))
                {
                    sdp.ReceiveEnergy(heatLevel.addEnergy);
                }
            }
            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            {
                sdp.ApplyEnergyTransfer();
            }
        }
    }
}