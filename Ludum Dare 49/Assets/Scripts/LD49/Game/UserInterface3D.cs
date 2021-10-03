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
                    return "CRITICAL HOT";
                } else if (GameModel.Current.SolutionEnergy < GameModel.Current.Config.minSolutionEnergy)
                {
                    return "CRITICAL COLD";
                }
                else if (GameModel.Current.SolutionEnergy >= GameModel.Current.Config.hotSolutionEnergyWarn)
                {
                    return "WARNING HOT";
                }
                else if (GameModel.Current.SolutionEnergy <= GameModel.Current.Config.coldSolutionEnergyWarn)
                {
                    return "WARNING COLD";
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
                    return "OK";
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
                else
                {
                    return Color.green;
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