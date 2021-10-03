using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class FlaskGameItem : MonoBehaviour
    {

        [SerializeField]
        protected MeshRenderer flaskMeshRenderer;

        [SerializeField]
        protected Material mGlass;
        [SerializeField]
        protected Material mGlassCracked;

        protected bool cracked;

        private void Update()
        {
            if (GameModel.Current.GlassCracked != cracked)
            {
                cracked = GameModel.Current.GlassCracked;
                UpdateMaterial();
            }
        }

        protected void UpdateMaterial()
        {
            flaskMeshRenderer.material = cracked ? mGlassCracked : mGlass;
        }

    }
}