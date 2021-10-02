using KazatanGames.Framework;
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

        public List<MoleculeData> Molecules { get; protected set; }
        public List<MoleculeData> DeadMolecules { get; protected set; }

        public void Initialise(GameConfigSO config)
        {
            Config = config;
            Molecules = new List<MoleculeData>();
            DeadMolecules = new List<MoleculeData>();

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

            foreach(MoleculeData md in Molecules)
            {
                md.Update(time);
            }
        }

        public void SetHeatLevel(float heatLevel)
        {
            CurrentHeatLevel = Mathf.Clamp(heatLevel, 0f, 1f);
        }

        public void AddMolecules(MoleculeTypeSO type, int amount)
        {
            while(amount-- > 0)
            {
                Molecules.Add(new MoleculeData()
                {
                    type = type,
                    position = new Vector2(Random.Range(0f, Config.visWidth), Config.visHeight),
                    direction = -90f,
                    speed = Random.Range(Config.minEntrySpeed, Config.maxEntrySpeed),
                    energy = Config.outsideEnergy
                });
            }
        }

        public void PurgeTheDead()
        {
            DeadMolecules = new List<MoleculeData>();
        }

        protected void SolutionEnergyTick()
        {
            int heatWidth = Mathf.RoundToInt(Mathf.Lerp(Config.minHeat.width, Config.maxHeat.width, CurrentHeatLevel));
            float heatEnergy = Mathf.Lerp(Config.minHeat.addEnergy, Config.maxHeat.addEnergy, CurrentHeatLevel);

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