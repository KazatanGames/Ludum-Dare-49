using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KazatanGames.Game
{
    public class GameSceneManager : BaseSceneManager
    {
        [SerializeField]
        protected Transform moleculeContainer;
        [SerializeField]
        protected ParticleSystem heatParticleSystem;
        [SerializeField]
        protected GameObject reactionParticlePrefab;

        [SerializeField]
        protected GameConfigSO gameConfig;

        [SerializeField]
        protected Gradient debugSolutionGradient;
        [SerializeField]
        protected int minParticlesPerSecond = 20;
        [SerializeField]
        protected int maxParticlesPerSecond = 40;

        protected Color[] gradientLUT;
        protected float particleTimeRem = 0f;
        protected ParticleSystem.MainModule psMain;

        protected Dictionary<MoleculeData, GameObject> moleculeGameObjects;

        protected override void Initialise()
        {
            moleculeGameObjects = new Dictionary<MoleculeData, GameObject>();

            ResetVis();

            gradientLUT = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                gradientLUT[i] = debugSolutionGradient.Evaluate((float)i / 255f);
            }

            GameModel.Current.Initialise(gameConfig);

            psMain = heatParticleSystem.main;
        }

        protected void Update()
        {
            if (GameModel.Current.ResetInvalidated) ResetVis();
            
            GameModel.Current.Update(Time.deltaTime);
            DrawMolecules();
            DrawFlame();
            DrawReactions();
        }

        protected void DrawMolecules()
        {
            foreach (MoleculeData md in GameModel.Current.DeadMolecules)
            {
                if (moleculeGameObjects.ContainsKey(md))
                {
                    Destroy(moleculeGameObjects[md]);
                    moleculeGameObjects.Remove(md);
                }
            }
            GameModel.Current.PurgeTheDead();

            foreach (MoleculeData md in GameModel.Current.Molecules)
            {
                GameObject mGO;
                if (!moleculeGameObjects.ContainsKey(md))
                {
                    mGO = Instantiate(md.type.moleculePrefab, md.position, Quaternion.AngleAxis(md.angle, Vector3.forward), moleculeContainer);
                    moleculeGameObjects.Add(md, mGO);
                }
                else
                {
                    mGO = moleculeGameObjects[md];
                }
                mGO.transform.localPosition = new Vector3(md.position.x, md.position.y, md.z);
                mGO.transform.localRotation = Quaternion.AngleAxis(md.angle, Vector3.forward);
            }
        }

        protected void DrawFlame()
        {
            if (GameModel.Current.CurrentHeatLevel > 0) {
                psMain.startColor = Color.Lerp(Color.yellow, Color.white, GameModel.Current.CurrentHeatLevel);

                float timePerEmit = 1f / Mathf.Lerp(minParticlesPerSecond, maxParticlesPerSecond, GameModel.Current.CurrentHeatLevel);
                particleTimeRem += Time.deltaTime;

                int particlesToEmit = Mathf.FloorToInt(particleTimeRem / timePerEmit);

                heatParticleSystem.Emit(particlesToEmit);
                particleTimeRem -= timePerEmit * particlesToEmit;
            }
        }

        protected void DrawReactions()
        {
            foreach(Vector3 position in GameModel.Current.ReactionLocations)
            {
                Instantiate(reactionParticlePrefab, position, Quaternion.identity, moleculeContainer).transform.localPosition = position;
            }
            GameModel.Current.ReactionLocations.Clear();
        }

        protected void ResetVis()
        {
            foreach (KeyValuePair<MoleculeData, GameObject> mgo in moleculeGameObjects)
            {
                Destroy(mgo.Value);
            }
            moleculeGameObjects = new Dictionary<MoleculeData, GameObject>();

            GameModel.Current.ResetInvalidated = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !AppManager.INSTANCE.AppModel.debugMode) return;

            if (GameModel.Current.SolutionDataPoints == null) return;

            foreach (SolutionDataPoint sdp in GameModel.Current.SolutionDataPoints)
            {
                int gradTime = 127;
                if (sdp.Energy < GameModel.Current.Config.outsideEnergy)
                {
                    gradTime = Mathf.RoundToInt(Mathf.Clamp(Mathf.InverseLerp(GameModel.Current.Config.minEnergy, GameModel.Current.Config.outsideEnergy, sdp.Energy), 0f, 1f) * 127f);
                } else if (sdp.Energy > GameModel.Current.Config.outsideEnergy)
                {
                    gradTime = 127 + Mathf.RoundToInt(Mathf.Clamp(Mathf.InverseLerp(GameModel.Current.Config.outsideEnergy, GameModel.Current.Config.maxEnergy, sdp.Energy), 0f, 1f) * 128f);
                }
                Handles.color = gradientLUT[gradTime];
                Handles.DrawWireDisc(sdp.Position + new Vector3(2.5f+0.5f, 2.5f, 0), Vector3.forward, 2f);
                //Handles.Label(sdp.Position + new Vector3(2f, 3f, 0), Mathf.Round(sdp.Energy) + "°");
            }

            foreach(MoleculeData md in GameModel.Current.Molecules)
            {
                Handles.Label(new Vector3(md.position.x, md.position.y, 0f) - new Vector3(45f, 0f, 0f), Mathf.Round(md.energy) + "°");
            }
        }
#endif
    }
}
