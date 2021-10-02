using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class KnownReactionRow : MonoBehaviour
    {
        public void CreateImages(Sprite[] sprites)
        {
            foreach (Sprite s in sprites)
            {
                GameObject imgObj = new GameObject("Sprite Image");
                Image NewImage = imgObj.AddComponent<Image>();
                NewImage.sprite = s;
                imgObj.GetComponent<RectTransform>().SetParent(transform);
                imgObj.SetActive(true);
            }
        }
    }
}