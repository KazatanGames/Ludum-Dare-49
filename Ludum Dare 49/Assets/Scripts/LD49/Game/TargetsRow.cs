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

        protected MoleculeTypeSO type;

        protected void LateUpdate()
        {
            if (type == null) return;

            // TODO: Count
            txt.text = "0";
        }

        public void SetType(MoleculeTypeSO type)
        {
            this.type = type;

            img.sprite = type.sprite;
        }
    }
}