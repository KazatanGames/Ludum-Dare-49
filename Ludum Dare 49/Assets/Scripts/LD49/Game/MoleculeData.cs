﻿using KazatanGames.Framework;
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

        public Vector2 position = Vector2.zero;
        public float direction = -90f;
        public float angle = Random.Range(0f, 360f);
        public float speed = 0f;
        public float turnSpeed = 0f;

        public float energy = 0f;

        protected bool rotationDirectionCCW = false;

        public void Update(float time)
        {
            SlowDown(time);

            CheckFlaskCollision(time);

            Energise(time);

            Turn(time);
            Move(time);
        }

        public void React()
        {
            MoleculeData reactee = null;
            ReactionStruct reactedReaction = new ReactionStruct();
            foreach (ReactionStruct reaction in GameModel.Current.Config.reactions)
            {
                if (reaction.input1 != type && reaction.input2 != type) continue;
                if (reaction.minEnergy > energy || reaction.maxEnergy < energy) continue;
                foreach(MoleculeData md in GameModel.Current.Molecules)
                {
                    if (md == this) continue;
                    if (GameModel.Current.DeadMolecules.Contains(md)) continue;
                    if (reaction.minEnergy > md.energy || reaction.maxEnergy < md.energy) continue;
                    if (reaction.input1 == type && reaction.input2 != md.type) continue;
                    if (reaction.input2 == type && reaction.input1 != md.type) continue;
                    if (Vector2.Distance(position, md.position) > GameModel.Current.Config.reactionDistance) continue;

                    reactedReaction = reaction;
                    reactee = md;
                    break;
                }
                if (reactee != null) break;
            }
            if (reactee != null)
            {
                // React!
                GameModel.Current.DeadMolecules.Add(this);
                GameModel.Current.DeadMolecules.Add(reactee);
                // TODO: energy change
                GameModel.Current.CreateMolecule(reactedReaction.result, (position + reactee.position) / 2f, (turnSpeed + reactee.turnSpeed) / 2f, (speed + reactee.speed) / 2f, (energy + reactee.energy) / 2f);
            }
        }

        protected void SlowDown(float time)
        {
            float slowDown = speed * 0.9f * time;

            turnSpeed -= turnSpeed * 0.8f * time;

            float turnDir = 1f;
            if (turnSpeed < 0f)
            {
                turnDir = -1f;
            } else if (turnSpeed == 0f)
            {
                turnDir = Random.value < 0.5 ? -1f : 1f;
            }

            // maybe use some of slowDown as change in direction towards 0, or -180?
            turnSpeed += slowDown * turnDir;

            speed -= slowDown;
        }

        protected void CheckFlaskCollision(float time)
        {
            if (position.x <= 0f)
            {
                position.x = 0.01f;
                speed *= 0.75f;
                turnSpeed *= 0.5f;
                direction = 180f - direction;
            } else if (position.x >= GameModel.Current.Config.visWidth)
            {
                position.x = GameModel.Current.Config.visWidth - 0.01f;
                speed *= 0.75f;
                turnSpeed *= 0.5f;
                direction = 180f - direction;
            }

            if (position.y <= 0f)
            {
                position.y = 0.01f;
                speed *= 0.75f;
                turnSpeed *= 0.5f;
                direction = 90f - (direction - 270f);
            }
            else if (position.y >= GameModel.Current.Config.visHeight)
            {
                position.y = GameModel.Current.Config.visHeight - 0.01f;
                speed *= 0.25f;
                turnSpeed *= 0.75f;
                direction = 90f - (direction - 270f);
            }
        }

        protected void Move(float time)
        {
            Quaternion q = Quaternion.AngleAxis(direction, Vector3.forward);
            Vector3 dPos = q * Vector3.right * speed * time;
            position = new Vector2(position.x + dPos.x, position.y + dPos.y);
        }

        protected void Turn(float time)
        {
            direction += turnSpeed * time;
            angle += (rotationDirectionCCW ? -energy : energy) * time * 3f;
            ClampAngle();
        }

        protected void ClampAngle()
        {
            direction %= 360f;
            if (direction < 0f) direction += 360f;
            angle %= 360f;
            if (angle < 0f) angle += 360f;
        }

        protected void Energise(float time)
        {
            // find the closest heat point and energise towards that energy level
            float xRatio = Mathf.InverseLerp(0f, GameModel.Current.Config.visWidth, position.x);
            float yRatio = Mathf.InverseLerp(0f, GameModel.Current.Config.visHeight, position.y);

            int xEPoint = Mathf.FloorToInt(Mathf.Lerp(0, GameModel.Current.Config.dataWidth, xRatio));
            int yEPoint = Mathf.FloorToInt(Mathf.Lerp(0, GameModel.Current.Config.dataHeight, yRatio));

            energy += (GameModel.Current.GetSolutionDataPoint(xEPoint, yEPoint).Energy - energy) * type.energiseMulti * time;

            // add speed based on energy level
            speed += GameModel.Current.Config.speedChangePerEnergy * (energy - GameModel.Current.Config.outsideEnergy) * time;
            speed = Mathf.Max(speed, 0f);

            // also find a direction travel should be encouraged in
        }
    }
}