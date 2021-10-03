using KazatanGames.Framework;
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
        public float z = 0f;
        protected float zRatio = 0f;
        protected bool zDir = false;

        protected bool rotationDirectionCCW = false;

        public MoleculeData()
        {
            zRatio = Random.Range(0f, 1f);
            zDir = Random.value < 0.5f;
        }

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

                    // the reaction could happen
                    if (reaction.bigEndothermic) GameModel.Current.Endothermics++;
                    if (reaction.bigExothermic) GameModel.Current.Exothermics++;

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
                NearestSolutionPoint.ReceiveEnergy(reactedReaction.energyCreated);
                foreach(MoleculeTypeSO mType in reactedReaction.results)
                {
                    GameModel.Current.CreateMolecule(mType, (position + reactee.position) / 2f, Random.Range(0f, 360f), (speed + reactee.speed) / 2f, (energy + reactee.energy) / 2f);
                    foreach(TargetStruct target in GameModel.Current.Config.targets)
                    {
                        if (target.type == mType)
                        {
                            GameModel.Current.AddScore(target.points);
                        }
                    }
                }
                GameModel.Current.ReactionLocations.Add((position + reactee.position) / 2f);
                if (!GameModel.Current.KnownReactions.Contains(reactedReaction))
                {
                    GameModel.Current.KnownReactions.Add(reactedReaction);
                    GameModel.Current.KnownReactionsInvalidated = true;
                }
            }
        }

        public SolutionDataPoint NearestSolutionPoint
        {
            get
            {
                float xRatio = Mathf.InverseLerp(0f, GameModel.Current.Config.visWidth, position.x);
                float yRatio = Mathf.InverseLerp(0f, GameModel.Current.Config.visHeight, position.y);

                int xEPoint = Mathf.FloorToInt(Mathf.Lerp(0, GameModel.Current.Config.dataWidth, xRatio));
                int yEPoint = Mathf.FloorToInt(Mathf.Lerp(0, GameModel.Current.Config.dataHeight, yRatio));

                return GameModel.Current.GetSolutionDataPoint(xEPoint, yEPoint);
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

            energy = Mathf.MoveTowards(energy, NearestSolutionPoint.Energy, type.energyAcclimatisationSpeed * time);

            if (!GameModel.Current.GlassCracked)
            {
                // add speed based on energy level
                speed += GameModel.Current.Config.speedChangePerEnergy * (energy - type.energiseEnergyMin) * type.energeticSpeedMulti * time;
                speed = Mathf.Max(speed, 0f);
            }

            // TODO: also find a direction travel should be encouraged in

            // Z
            if (zDir)
            {
                zRatio += (energy - type.energiseEnergyMin + 0.1f) * time * type.energiseWobbleMulti;
                if (zRatio >= 1f) zDir = false;
            } else
            {
                zRatio -= (energy - type.energiseEnergyMin + 0.1f) * time * type.energiseWobbleMulti;
                if (zRatio <= 0f) zDir = true;
            }

            zRatio = Mathf.Clamp(zRatio, 0f, 1f);

            z = 2.5f - Easing.Cubic.InOut(zRatio) * 5f;
        }
    }
}