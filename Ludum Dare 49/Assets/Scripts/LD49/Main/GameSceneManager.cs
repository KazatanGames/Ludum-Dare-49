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
        protected GameConfigSO gameConfig;

        [SerializeField]
        protected Gradient debugSolutionGradient;
        [SerializeField]
        protected float minTemp = -100f;
        [SerializeField]
        protected float maxTemp = 100f;

        protected Color[] gradientLUT;

    protected override void Initialise()
        {
            gradientLUT = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                gradientLUT[i] = debugSolutionGradient.Evaluate((float)i / 255f);
            }

            GameModel.Current.Initialise(gameConfig);
        }

        protected void Update()
        {
            GameModel.Current.Update(Time.deltaTime);
            DrawFlask();
            heatLevelText.SetText($"Heat Level: {GameModel.Current.CurrentHeatLevel}");
        }

        protected void DrawFlask()
        {

        }

        public void IncreaseHeat()
        {
            GameModel.Current.SetHeatLevel(GameModel.Current.CurrentHeatLevel + 1);
        }

        public void DecreaseHeat()
        {
            GameModel.Current.SetHeatLevel(GameModel.Current.CurrentHeatLevel - 1);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !AppManager.INSTANCE.AppModel.debugMode) return;

            if (GameModel.Current.SolutionDataPoints == null) return;

            foreach (SolutionDataPoint sdp in GameModel.Current.SolutionDataPoints)
            {
                int gradTime = Mathf.RoundToInt(Mathf.Clamp(Mathf.InverseLerp(minTemp, maxTemp, sdp.Energy), 0f, 1f) * 255f);
                Handles.color = gradientLUT[gradTime];
                Handles.DrawWireDisc(sdp.Position, Vector3.forward, 1f);
            }
        }
#endif
    }
}
