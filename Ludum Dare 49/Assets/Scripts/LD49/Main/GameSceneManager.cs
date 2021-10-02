﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;
using TMPro;
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
        protected RectTransform reactionsUIContainer;
        [SerializeField]
        protected Sprite plusSprite;
        [SerializeField]
        protected Sprite equalsSprite;
        [SerializeField]
        protected KnownReactionRow knownReactionRowPrefab;
        [SerializeField]
        protected TextMeshProUGUI heatLevelText;
        [SerializeField]
        protected ParticleSystem heatParticleSystem;
        [SerializeField]
        protected GameObject reactionParticlePrefab;

        [SerializeField]
        protected MoleculeTypeSO moleculesToAddY;
        [SerializeField]
        protected MoleculeTypeSO moleculesToAddR;

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
            GameModel.Current.Update(Time.deltaTime);
            DrawMolecules();
            DrawFlame();
            DrawReactions();
            if (GameModel.Current.KnownReactionsInvalidated) DrawKnownReactions();

            heatLevelText.SetText($"Heat: {GameModel.Current.CurrentHeatLevel}");
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
                mGO.transform.localPosition = md.position;
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

        protected void DrawKnownReactions()
        {
            GameModel.Current.KnownReactionsInvalidated = false;
            foreach (RectTransform child in reactionsUIContainer)
            {
                Destroy(child.gameObject);
            }
            foreach (ReactionStruct rs in GameModel.Current.KnownReactions)
            {
                KnownReactionRow krr = Instantiate(knownReactionRowPrefab, reactionsUIContainer);
                Sprite[] sprites = new Sprite[rs.results.Length + 4];

                sprites[0] = rs.input1.sprite;
                sprites[1] = plusSprite;
                sprites[2] = rs.input2.sprite;
                sprites[3] = equalsSprite;
                int i = 0;
                foreach(MoleculeTypeSO mt in rs.results)
                {
                    sprites[4 + i] = mt.sprite;
                    i++;
                }

                krr.CreateImages(sprites);
            }
        }

        public void ChangeHeat(float value)
        {
            GameModel.Current.SetHeatLevel(value);
        }

        public void AddMoleculesY()
        {
            GameModel.Current.AddMolecules(moleculesToAddY, Random.Range(4, 8));
            GameModel.Current.KnownReactionsInvalidated = true;
        }
        public void AddMoleculesR()
        {
            GameModel.Current.AddMolecules(moleculesToAddR, Random.Range(4, 8));
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
