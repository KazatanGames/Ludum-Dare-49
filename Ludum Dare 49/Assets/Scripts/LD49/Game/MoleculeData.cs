using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class MoleculeData
    {
        public MoleculeTypeSO type;

        public Vector2 position;
        public float direction;
        public float speed;

        public float energy;

        public void Update(float time)
        {
            SlowDown(time);
            Move(time);
        }

        protected void SlowDown(float time)
        {
            speed -= speed * 0.95f * time;
        }

        protected void Move(float time)
        {
            Quaternion q = Quaternion.AngleAxis(direction, Vector3.forward);
            Vector3 dPos = q * Vector3.right * speed * time;
            position = new Vector2(position.x + dPos.x, position.y + dPos.y);
        }
    }
}