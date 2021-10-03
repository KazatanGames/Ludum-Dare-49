using KazatanGames.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class UserInterface3D : MonoBehaviour
    {

        [SerializeField]
        protected TextMeshPro temperatureText;
        [SerializeField]
        protected TextMeshPro stabilityText;
        [SerializeField]
        protected TextMeshPro pointsText;
        [SerializeField]
        [ColorUsage(true, true)]
        protected Color unstableLow;
        [SerializeField]
        [ColorUsage(true, true)]
        protected Color unstableHigh;

        protected float unstable = 0;
        protected float unstableDecaying = 0;

        protected void Update()
        {
            float hotPotEnergy = GameModel.Current.SolutionEnergy + (Mathf.Sqrt(GameModel.Current.Exothermics) * 10f);
            float coldPotEnergy = GameModel.Current.SolutionEnergy - (Mathf.Sqrt(GameModel.Current.Endothermics) * 10f);

            float hotWorry = Mathf.InverseLerp(GameModel.Current.Config.maxSolutionEnergy, GameModel.Current.Config.maxSolutionEnergy + 25f, hotPotEnergy);
            float coldWorry = Mathf.InverseLerp(GameModel.Current.Config.minSolutionEnergy, GameModel.Current.Config.minSolutionEnergy - 25f, coldPotEnergy);

            unstable = Mathf.Max(coldWorry, hotWorry);

            if (unstable > unstableDecaying)
            {
                unstableDecaying = unstable;
            } else
            {
                unstableDecaying = Mathf.MoveTowards(unstableDecaying, unstable, Time.deltaTime * 0.25f);
            }
        }

        private void LateUpdate()
        {
            temperatureText.text = $"{TempDesc} | {GameModel.Current.SolutionEnergy.ToString("F0")}°";
            temperatureText.color = TempColor;

            stabilityText.text = StabilityDesc;
            stabilityText.color = StabilityColor;

            pointsText.text = PointsDesc;
        }


        protected string TempDesc
        {
            get
            {
                if (GameModel.Current.SolutionEnergy > GameModel.Current.Config.maxSolutionEnergy)
                {
                    return "HOT";
                } else if (GameModel.Current.SolutionEnergy < GameModel.Current.Config.minSolutionEnergy)
                {
                    return "COLD";
                }
                else if (GameModel.Current.SolutionEnergy >= GameModel.Current.Config.hotSolutionEnergyWarn)
                {
                    return "HOT";
                }
                else if (GameModel.Current.SolutionEnergy <= GameModel.Current.Config.coldSolutionEnergyWarn)
                {
                    return "COLD";
                } else
                {
                    return "OK";
                }
            }
        }

        protected Color TempColor
        {
            get
            {
                if (GameModel.Current.SolutionEnergy > GameModel.Current.Config.maxSolutionEnergy || GameModel.Current.SolutionEnergy < GameModel.Current.Config.minSolutionEnergy)
                {
                    return Color.red;
                }
                else if (GameModel.Current.SolutionEnergy >= GameModel.Current.Config.hotSolutionEnergyWarn || GameModel.Current.SolutionEnergy <= GameModel.Current.Config.coldSolutionEnergyWarn)
                {
                    return Color.yellow;
                }
                else
                {
                    return Color.green;
                }
            }
        }

        protected string StabilityDesc
        {
            get
            {
                if (GameModel.Current.GlassCracked)
                {
                    return "CRITICAL";
                }
                else
                {
                    if (unstableDecaying <= 0.01f) return "Stable";
                    if (unstableDecaying <= 0.5f) return "Unstable";
                    return "Highly Unstable";
                }
            }
        }

        protected Color StabilityColor
        {
            get
            {
                if (GameModel.Current.GlassCracked)
                {
                    return Color.red;
                }
                else if (unstableDecaying <= 0.01f)
                {
                    return Color.green;
                } else
                {
                    return Color.Lerp(unstableLow, unstableHigh, unstableDecaying);
                }
            }
        }

        protected string PointsDesc
        {
            get
            {
                return GameModel.Current.Score.ToString("F0");
            }
        }
    }
}