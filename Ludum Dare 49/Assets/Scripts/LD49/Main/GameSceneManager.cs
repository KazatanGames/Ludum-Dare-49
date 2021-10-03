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
        protected ParticleSystem bubblesParticleSystem;
        [SerializeField]
        protected GameObject reactionParticlePrefab;
        [SerializeField]
        protected GameObject icePrefab;
        [SerializeField]
        protected GameObject smokePrefab;
        [SerializeField]
        protected Light burnerLight;
        [SerializeField]
        protected AudioSource bubblesAudio;
        [SerializeField]
        protected AudioSource flameAudio;
        [SerializeField]
        protected AudioSource bangAudio;
        [SerializeField]
        protected AudioSource glassAudio;
        [SerializeField]
        protected AudioSource iceAudio;
        [SerializeField]
        protected AudioSource hotReactAudio;

        [SerializeField]
        protected GameConfigSO gameConfig;

        [SerializeField]
        protected Gradient debugSolutionGradient;
        [SerializeField]
        protected int minParticlesPerSecond = 20;
        [SerializeField]
        protected int maxParticlesPerSecond = 40;
        [SerializeField]
        protected int bubblesPerSecondMax = 20;
        [SerializeField]
        protected float bubbleStartEnergy = 30f;

        protected Color[] gradientLUT;
        protected float particleTimeRemHeat = 0f;
        protected float particleTimeRemBubbles = 0f;
        protected ParticleSystem.MainModule psMainHeat;
        protected ParticleSystem.MainModule psMainBubbles;

        protected float lightFlickerTime = 0f;
        protected Vector3 lightFlickerTarget;
        protected Vector3 lightFlickerOld;
        protected Vector3 lightFlickerOriginal;

        protected Dictionary<MoleculeData, GameObject> moleculeGameObjects;

        protected bool gameOver = false;

        protected GameObject gameOverObject;

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

            psMainHeat = heatParticleSystem.main;
            psMainBubbles = bubblesParticleSystem.main;

            lightFlickerOriginal = lightFlickerOld = burnerLight.transform.localPosition;
        }

        protected void Update()
        {
            if (gameOver != GameModel.Current.GlassCracked)
            {
                gameOver = GameModel.Current.GlassCracked;

                if (gameOver)
                {
                    glassAudio.Play();

                    if (GameModel.Current.GameOverFrozen)
                    {
                        gameOverObject = Instantiate(icePrefab);
                        iceAudio.Play();
                    } else
                    {
                        gameOverObject = Instantiate(smokePrefab);
                        bangAudio.Play();
                    }
                } else
                {
                    Destroy(gameOverObject);
                    gameOverObject = null;
                }
            }

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
                psMainHeat.startColor = Color.Lerp(Color.yellow, Color.white, GameModel.Current.CurrentHeatLevel);

                float timePerEmit = 1f / Mathf.Lerp(minParticlesPerSecond, maxParticlesPerSecond, GameModel.Current.CurrentHeatLevel);
                particleTimeRemHeat += Time.deltaTime;

                int particlesToEmit = Mathf.FloorToInt(particleTimeRemHeat / timePerEmit);

                heatParticleSystem.Emit(particlesToEmit);
                particleTimeRemHeat -= timePerEmit * particlesToEmit;

                if (gameOver)
                {
                    flameAudio.volume = GameModel.Current.CurrentHeatLevel;
                }
                else
                {
                    flameAudio.volume = Mathf.Lerp(0.5f, 1f, GameModel.Current.CurrentHeatLevel);
                }
            } else
            {
                flameAudio.volume = 0f;
            }

            if (GameModel.Current.SolutionEnergy > bubbleStartEnergy)
            {
                particleTimeRemBubbles += Time.deltaTime;

                float bubbleRatio = Mathf.Clamp(Mathf.InverseLerp(bubbleStartEnergy, gameConfig.maxSolutionEnergy, GameModel.Current.SolutionEnergy), 0f, 1f);

                if (gameOver)
                {
                    bubblesAudio.volume = Mathf.Min(bubbleRatio, GameModel.Current.CurrentHeatLevel);
                } else
                {
                    bubblesAudio.volume = bubbleRatio;
                }
                if (bubbleRatio > 0)
                {
                    float bubblesDesired = bubblesPerSecondMax * bubbleRatio;
                    float timePerEmit = 1f / bubblesDesired;

                    int particlesToEmit = Mathf.FloorToInt(particleTimeRemBubbles / timePerEmit);

                    bubblesParticleSystem.Emit(particlesToEmit);
                    particleTimeRemBubbles -= timePerEmit * particlesToEmit;
                }
            } else
            {
                bubblesAudio.volume = 0;
                particleTimeRemBubbles = 0f;
            }

            lightFlickerTime -= Time.deltaTime;
            if (lightFlickerTime <= 0f)
            {
                lightFlickerTime = Random.Range(0.05f, 0.2f);
                lightFlickerOld = lightFlickerTarget;
                lightFlickerTarget = lightFlickerOriginal + new Vector3(Random.Range(-gameConfig.burnerFlickerDistance, gameConfig.burnerFlickerDistance), Random.Range(-gameConfig.burnerFlickerDistance, gameConfig.burnerFlickerDistance), Random.Range(-gameConfig.burnerFlickerDistance, gameConfig.burnerFlickerDistance));
            }

            burnerLight.transform.localPosition = Vector3.Lerp(lightFlickerTarget, lightFlickerOld, Easing.Cubic.InOut(lightFlickerTime / 0.2f));

            burnerLight.intensity = 3f * GameModel.Current.CurrentHeatLevel;
        }

        protected void DrawReactions()
        {
            foreach(Vector3 position in GameModel.Current.ReactionLocations)
            {
                Instantiate(reactionParticlePrefab, position, Quaternion.identity, moleculeContainer).transform.localPosition = position;
                hotReactAudio.transform.localPosition = position;
                hotReactAudio.Play();
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

            bubblesParticleSystem.Clear();
            heatParticleSystem.Clear();

            Destroy(gameOverObject);
            gameOverObject = null;
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
