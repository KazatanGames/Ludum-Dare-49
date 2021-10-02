﻿using KazatanGames.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class SolutionDataPoint
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public float Energy { get; protected set; }
        public Vector3 Position { get; protected set; }

        protected bool side;
        protected bool top;
        protected SolutionDataPoint nU;
        protected SolutionDataPoint nD;
        protected SolutionDataPoint nL;
        protected SolutionDataPoint nR;
        protected float energyChange = 0f;

        public SolutionDataPoint(int x, int y, bool side, bool top)
        {
            X = x;
            Y = y;
            this.side = side;
            this.top = top;

            Energy = GameModel.Current.Config.outsideEnergy;
            Position = new Vector3(GameModel.Current.Config.visWidth * ((float)x / GameModel.Current.Config.dataWidth), GameModel.Current.Config.visHeight * ((float)y / GameModel.Current.Config.dataHeight), 0f);
        }

        public void ReceiveEnergy(float dE)
        {
            energyChange += dE;
        }

        public void ApplyEnergyTransfer()
        {
            Energy = Mathf.Clamp(Energy + energyChange, GameModel.Current.Config.minEnergy, GameModel.Current.Config.maxEnergy);
            energyChange = 0f;
        }

        public void CalculateEnergyTransfer()
        {
            CalcTransferToNeighbour(nU, GameDirection2D.Up);
            CalcTransferToNeighbour(nD, GameDirection2D.Down);
            CalcTransferToNeighbour(nL, GameDirection2D.Left);
            CalcTransferToNeighbour(nR, GameDirection2D.Right);
            if (side) CalcLoseToSide();
            if (top) CalcLoseToTop();

            if (X == 5 && Y == 5)
            {
                Debug.Log($"{energyChange}");
            }
        }

        public bool ShouldBeHeated()
        {
            return Y <= GameModel.Current.Config.heatLevels[GameModel.Current.CurrentHeatLevel].rows;
        }

        public void FindNeighbours(SolutionDataPoint[] neighbours)
        {
            // left neighbour
            if (X > 0) nL = neighbours[((X - 1) * GameModel.Current.Config.dataHeight) + Y];
            // right neighbour
            if (X < GameModel.Current.Config.dataWidth - 1) nR = neighbours[((X + 1) * GameModel.Current.Config.dataHeight) + Y];

            // down neighbour
            if (Y > 0) nD = neighbours[(X * GameModel.Current.Config.dataHeight) + (Y - 1)];
            // up neighbour
            if (Y < GameModel.Current.Config.dataHeight - 1) nU = neighbours[(X * GameModel.Current.Config.dataHeight) + (Y + 1)];
        }

        // only calculate energy we _send_ to our neighbours as they do the same for us
        protected void CalcTransferToNeighbour(SolutionDataPoint n, GameDirection2D d)
        {
            if (n == null) return;

            float dE = (Energy - n.Energy) * 0.25f;

            //dE = Mathf.Max(dE, 0f);

            energyChange += dE;
            //n.ReceiveEnergy(dE);
        }

        protected void CalcLoseToSide()
        {
            float dE = (Energy - GameModel.Current.Config.outsideEnergy) * 0.25f;
            // dE = Mathf.Max(dE, 0f);
            energyChange += dE;
        }

        protected void CalcLoseToTop()
        {
            float dE = (Energy - GameModel.Current.Config.outsideEnergy) * 0.25f;
            // dE = Mathf.Max(dE, 0f);
            energyChange += dE;
        }

    }
}