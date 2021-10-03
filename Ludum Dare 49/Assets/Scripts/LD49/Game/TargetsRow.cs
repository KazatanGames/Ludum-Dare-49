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
    public class TargetsRow : MonoBehaviour
    {
        [SerializeField]
        protected Image img;
        [SerializeField]
        protected TextMeshProUGUI txt;
        [SerializeField]
        protected TextMeshProUGUI ptsTxt;

        protected MoleculeTypeSO type;

        protected void LateUpdate()
        {
            if (type == null) return;
            txt.text = GameModel.Current.GetCreatedCount(type).ToString("F0");
        }

        public void SetTarget(TargetStruct ts)
        {
            type = ts.type;
            ptsTxt.text = $"{ts.points.ToString("F0")} x";
            img.sprite = type.sprite;
        }
    }
}