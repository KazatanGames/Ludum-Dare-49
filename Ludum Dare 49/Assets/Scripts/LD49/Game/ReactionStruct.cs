using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    [Serializable]
    public struct ReactionStruct
    {
        public MoleculeTypeSO input1;
        public MoleculeTypeSO input2;
        public float minEnergy;
        public float maxEnergy;
        public MoleculeTypeSO[] results;
        public float energyCreated;
        public float exothermicInstability;
        public float endothermicInstability;
    }
}