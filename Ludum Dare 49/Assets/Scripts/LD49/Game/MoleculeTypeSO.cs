using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    [CreateAssetMenu(fileName = "MoleculeType", menuName = "Molecule Type", order = 1)][Serializable]
    public class MoleculeTypeSO : ScriptableObject
    {
        public string title;
        public GameObject moleculePrefab;
        public Sprite sprite;
        public float energiseMulti = 0.5f;
        public float energeticSpeedMulti = 1f;
    }
}