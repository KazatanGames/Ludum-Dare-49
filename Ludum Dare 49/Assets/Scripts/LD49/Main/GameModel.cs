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

        public float CurrentHeatLevel { get; protected set; } = 0f;

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
            solutionEnergyTickTime = 1f / Config.solutionEnergyTicksPerSecond;
            solutionEnergyTickTimeRem += time;

            while (solutionEnergyTickTimeRem >= solutionEnergyTickTime)
            {
                solutionEnergyTickTimeRem -= solutionEnergyTickTime;
                SolutionEnergyTick();
            }
        }

        public void SetHeatLevel(float heatLevel)
        {
            CurrentHeatLevel = Mathf.Clamp(heatLevel, 0f, 1f);
        }

        protected void SolutionEnergyTick()
        {
            int heatWidth = Mathf.RoundToInt(Mathf.Lerp(Config.minHeat.width, Config.maxHeat.width, CurrentHeatLevel));
            float heatEnergy = Mathf.Lerp(Config.minHeat.addEnergy, Config.maxHeat.addEnergy, CurrentHeatLevel);

            Debug.Log("hw = " + heatWidth + "he = " + heatEnergy);

            int heatXMin = (Config.dataWidth - heatWidth) / 2;
            int heatXMax = heatXMin + heatWidth - 1;

            float globalEffect = 0f;

            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            {
                globalEffect += sdp.Energy;
            }
            float globalAverage = globalEffect / SolutionDataPoints.Length;
            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            { 
                sdp.CalculateEnergyTransfer(globalAverage);
                if (CurrentHeatLevel > 0 && sdp.ShouldBeHeated(heatXMin, heatXMax))
                {
                    sdp.ReceiveEnergy(heatEnergy);
                }
            }
            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            {
                sdp.ApplyEnergyTransfer();
            }
        }
    }
}