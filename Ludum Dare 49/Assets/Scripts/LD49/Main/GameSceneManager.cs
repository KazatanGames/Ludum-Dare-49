using System.Collections;
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
        protected GameObject flaskContainer;
        [SerializeField]
        protected TextMeshProUGUI heatLevelText;
        [SerializeField]
        protected ParticleSystem heatParticleSystem;

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

        protected override void Initialise()
        {
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
            DrawFlask();
            DrawFlame();
            heatLevelText.SetText($"Heat: {GameModel.Current.CurrentHeatLevel}");
        }

        protected void DrawFlask()
        {

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

        public void ChangeHeat(float value)
        {
            GameModel.Current.SetHeatLevel(value);
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
                Handles.DrawSolidDisc(sdp.Position + new Vector3(1f, -1f, 0f), Vector3.forward, 2f);
                Handles.Label(sdp.Position, Mathf.Round(sdp.Energy) + "°");
            }
        }
#endif
    }
}
