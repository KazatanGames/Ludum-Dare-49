using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class KnownReactionRow : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI tempTxt;
        [SerializeField]
        protected Image input1Img;
        [SerializeField]
        protected Image input2Img;
        [SerializeField]
        protected Image output1Img;
        [SerializeField]
        protected Image output2Img;
        [SerializeField]
        protected Image output3Img;
        [SerializeField]
        protected Image greySquare;

        public void SetData(ReactionStruct rs)
        {
            input1Img.sprite = rs.input1.sprite;
            input2Img.sprite = rs.input2.sprite;

            if (rs.results.Length > 0)
            {
                output1Img.sprite = rs.results[0].sprite;
                output1Img.enabled = true;
            }
            else
            {
                output1Img.enabled = false;
            }
            if (rs.results.Length > 1)
            {
                output2Img.sprite = rs.results[1].sprite;
                output2Img.enabled = true;
            } else
            {
                output2Img.sprite = null;
                output2Img.enabled = false;
            }
            if (rs.results.Length > 2)
            {
                output3Img.sprite = rs.results[2].sprite;
                output3Img.enabled = true;
            } else
            {
                output3Img.sprite = null;
                output3Img.enabled = false;
            }

            tempTxt.text = $"~{rs.displayEnergy}°";
        }

        public void SetSeen(bool seen)
        {
            greySquare.enabled = !seen;
        }
    }
}