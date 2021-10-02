using KazatanGames.Framework;
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
            Position = new Vector3((-GameModel.Current.Config.visWidth / 2f) + GameModel.Current.Config.visWidth * ((float)x / GameModel.Current.Config.dataWidth), GameModel.Current.Config.visHeight * ((float)y / GameModel.Current.Config.dataHeight), 0f);
        }

        public void ReceiveEnergy(float dE)
        {
            energyChange += dE;
        }

        public void ApplyEnergyTransfer()
        {
            Energy += energyChange;
            energyChange = 0f;
        }

        public void CalculateEnergyTransfer(float globalAverage)
        {
            float totalTemp = Energy * GameModel.Current.Config.heatPreservation;
            float totalDivisor = GameModel.Current.Config.heatPreservation;

            totalTemp += globalAverage * GameModel.Current.Config.globalEffect;
            totalDivisor += GameModel.Current.Config.globalEffect;

            if (side)
            {
                totalTemp += GameModel.Current.Config.outsideEnergy * GameModel.Current.Config.heatTransferSide;
                totalDivisor += GameModel.Current.Config.heatTransferSide;
            }
            if (top)
            {
                totalTemp += GameModel.Current.Config.outsideEnergy * GameModel.Current.Config.heatTransferTop;
                totalDivisor += GameModel.Current.Config.heatTransferTop;
            }
            if (nU != null) {
                totalTemp += nU.Energy * GameModel.Current.Config.heatVerticalBias;
                totalDivisor += GameModel.Current.Config.heatVerticalBias;
            }
            if (nD != null)
            {
                totalTemp += nD.Energy * GameModel.Current.Config.heatVerticalBias;
                totalDivisor += GameModel.Current.Config.heatVerticalBias;
            }
            if (nL != null)
            {
                totalTemp += nL.Energy;
                totalDivisor += 1f;
            }
            if (nR != null)
            {
                totalTemp += nR.Energy;
                totalDivisor += 1f;
            }

            float avg = totalTemp / totalDivisor;

            float difference = avg - Energy;

            float dE = difference * GameModel.Current.Config.heatTransfer;

            ReceiveEnergy(dE);
        }

        public bool ShouldBeHeated(int minX, int maxX)
        {
            return Y == 0 && X >= minX && X <= maxX;
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

        protected void CalcLoseToSide()
        {
            float dE = (Energy - GameModel.Current.Config.outsideEnergy) * GameModel.Current.Config.heatTransferSide;
            energyChange -= dE;
        }

        protected void CalcLoseToTop()
        {
            float dE = (Energy - GameModel.Current.Config.outsideEnergy) * GameModel.Current.Config.heatTransferSide;
            energyChange -= dE;
        }

    }
}