namespace KazatanGames.Framework
{
    using UnityEngine;

    /**
     * X, Y notation
     * 
     * L: Left
     * R: Right
     * T: Top
     * B: Bottom
     * M: Middle (either axis)
     */
    public enum NineGrid
    {
        MM = 5, // Self / Center point

        LB = 0, // match FourGrid
        RB = 1, // match FourGrid
        RT = 2, // match FourGrid
        LT = 3, // match FourGrid

        LM = 4,
        MT = 6,
        MB = 7,
        RM = 8
    }

    public static class NineGridHelpers
    {
        public static Vector2Int GridDifference(NineGrid ng)
        {
            Vector2Int result = Vector2Int.zero;
            switch (ng)
            {
                case NineGrid.LB:
                case NineGrid.LM:
                case NineGrid.LT:
                    result.x = -1;
                    break;
                case NineGrid.RB:
                case NineGrid.RM:
                case NineGrid.RT:
                    result.x = 1;
                    break;
            }
            switch (ng)
            {
                case NineGrid.LT:
                case NineGrid.MT:
                case NineGrid.RT:
                    result.y = 1;
                    break;
                case NineGrid.LB:
                case NineGrid.MB:
                case NineGrid.RB:
                    result.y = -1;
                    break;
            }
            return result;
        }
    }
}