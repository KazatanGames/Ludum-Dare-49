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

        public List<Vector3> ReactionLocations { get; protected set; } = new List<Vector3>();

        public List<ReactionStruct> KnownReactions { get; protected set; } = new List<ReactionStruct>();
        public bool KnownReactionsInvalidated { get; set; } = true;

        public bool GlassCracked { get; protected set; } = false;
        public bool GameOverFrozen { get; protected set; } = false;
        public bool GameOverCombust { get; protected set; } = false;
        public bool ResetInvalidated { get; set; } = false;

        public int Score { get; protected set; } = 0;

        protected List<MoleculeData> newMolecules;
        public float SolutionEnergy { get; protected set; }

        protected float timeSlowDownTime = 0f;

        public void Initialise(GameConfigSO config)
        {
            Config = config;
            Reset();
        }

        public void Update(float time)
        {
            if (GlassCracked && timeSlowDownTime < Config.crackSlowDownTime)
            {
                timeSlowDownTime += time;
                Time.timeScale = 1f - (0.98f * Easing.Quadratic.Out(timeSlowDownTime / Config.crackSlowDownTime));

                float newHeatLevel = 1f - Easing.Quadratic.Out(timeSlowDownTime / Config.crackSlowDownTime);
                if (newHeatLevel < CurrentHeatLevel) SetHeatLevel(newHeatLevel);
            }

            if (!GlassCracked)
            {

                solutionEnergyTickTime = 1f / Config.solutionEnergyTicksPerSecond;
                solutionEnergyTickTimeRem += time;

                while (solutionEnergyTickTimeRem >= solutionEnergyTickTime)
                {
                    solutionEnergyTickTimeRem -= solutionEnergyTickTime;
                    SolutionEnergyTick();
                }

            }

            newMolecules = new List<MoleculeData>();

            foreach (MoleculeData md in Molecules)
            {
                if (!DeadMolecules.Contains(md))
                {
                    md.Update(time);
                    if (!GlassCracked) md.React();
                }
            }

            Molecules.AddRange(newMolecules);

            foreach (MoleculeData md in DeadMolecules)
            {
                Molecules.Remove(md);
            }

            if (GlassCracked) return;

            CheckForCrack();
        }

        public void SetHeatLevel(float heatLevel)
        {
            CurrentHeatLevel = Mathf.Clamp(heatLevel, 0f, 1f);
        }

        public void AddScore(int plus)
        {
            Score += plus;
        }

        public void AddMolecules(MoleculeTypeSO type, int amount)
        {
            if (GlassCracked) return;

            while (amount-- > 0)
            {
                Molecules.Add(new MoleculeData()
                {
                    type = type,
                    position = new Vector2(Random.Range(0f, Config.visWidth), Config.visHeight - 0.01f),
                    direction = 270f,
                    speed = Random.Range(Config.minEntrySpeed, Config.maxEntrySpeed),
                    energy = Config.outsideEnergy
                });
            }
        }

        public void PurgeTheDead()
        {
            DeadMolecules = new List<MoleculeData>();
        }

        public SolutionDataPoint GetSolutionDataPoint(int x, int y)
        {
            x = Mathf.Clamp(x, 0, Config.dataWidth - 1);
            y = Mathf.Clamp(y, 0, Config.dataHeight - 1);
            return SolutionDataPoints[(x * Config.dataHeight) + y];
        }

        public void CreateMolecule(MoleculeTypeSO type, Vector2 position, float angularSpeed, float speed, float energy)
        {
            if (GlassCracked) return;

            newMolecules.Add(new MoleculeData()
            {
                type = type,
                position = position,
                turnSpeed = angularSpeed,
                speed = speed,
                energy = energy
            });
        }

        public void Reset()
        {
            Molecules = new List<MoleculeData>();
            DeadMolecules = new List<MoleculeData>();

            SolutionEnergy = Config.outsideEnergy;

            solutionEnergyTickTime = 1f / Config.solutionEnergyTicksPerSecond;

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
            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            {
                sdp.FindNeighbours(SolutionDataPoints);
            }

            ReactionLocations = new List<Vector3>();
            CurrentHeatLevel = 0;
            GlassCracked = false;
            GameOverFrozen = false;
            GameOverCombust = false;
            KnownReactionsInvalidated = true;
            Score = 0;

            Time.timeScale = 1f;

            timeSlowDownTime = 0f;

            ResetInvalidated = true;
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
            SolutionEnergy = globalEffect / SolutionDataPoints.Length;
            foreach (SolutionDataPoint sdp in SolutionDataPoints)
            { 
                sdp.CalculateEnergyTransfer(SolutionEnergy);
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

        protected void CheckForCrack()
        {
            GameOverFrozen = SolutionEnergy < Config.minSolutionEnergy;
            GameOverCombust = SolutionEnergy > Config.maxSolutionEnergy;

            GlassCracked = GameOverFrozen || GameOverCombust;
        }
    }
}