namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Flags]
    public enum GameDiagonal : short
    {
        NorthWest = (1 << 0), // 1 in decimal
        NorthEast = (1 << 1), // 2 in decimal
        SouthEast = (1 << 2), // 4 in decimal
        SouthWest = (1 << 3), // 8 in decimal
    }

    public static class GameDiagonalExtensions
    {
        public static bool Test(this GameDiagonal input, GameDiagonal test)
        {
            return (input & test) == test;
        }

        public static bool Within(this GameDiagonal input, GameDiagonal test)
        {
            return (input & test) == input;
        }

        public static GameDiagonal Invert(this GameDiagonal input)
        {
            return GameDiagonalHelper.Invert(input);
        }

        public static GameDiagonal Opposite(this GameDiagonal input)
        {
            return GameDiagonalHelper.Opposite(input);
        }

        public static GameDiagonal RotateCW(this GameDiagonal input)
        {
            return GameDiagonalHelper.RotateCW(input);
        }

        public static GameDiagonal RotateCCW(this GameDiagonal input)
        {
            return GameDiagonalHelper.RotateCCW(input);
        }

        public static GameDiagonal RandomSingle(this GameDiagonal input)
        {
            List<GameDiagonal> options = new List<GameDiagonal>();
            if (input.Test(GameDiagonal.NorthWest)) options.Add(GameDiagonal.NorthWest);
            if (input.Test(GameDiagonal.NorthEast)) options.Add(GameDiagonal.NorthEast);
            if (input.Test(GameDiagonal.SouthEast)) options.Add(GameDiagonal.SouthEast);
            if (input.Test(GameDiagonal.SouthWest)) options.Add(GameDiagonal.SouthWest);
            if (options.Count == 0) return GameDiagonalHelper.Nil;
            return options[Random.Range(0, options.Count)];
        }

        public static int Count(this GameDiagonal input)
        {
            int total = 0;
            if (input.Test(GameDiagonal.NorthWest)) total++;
            if (input.Test(GameDiagonal.NorthEast)) total++;
            if (input.Test(GameDiagonal.SouthEast)) total++;
            if (input.Test(GameDiagonal.SouthWest)) total++;
            return total;
        }

        public static List<GameDiagonal> ToList(this GameDiagonal input)
        {
            List<GameDiagonal> results = new List<GameDiagonal>();
            if (input.Test(GameDiagonal.NorthWest)) results.Add(GameDiagonal.NorthWest);
            if (input.Test(GameDiagonal.NorthEast)) results.Add(GameDiagonal.NorthEast);
            if (input.Test(GameDiagonal.SouthEast)) results.Add(GameDiagonal.SouthEast);
            if (input.Test(GameDiagonal.SouthWest)) results.Add(GameDiagonal.SouthWest);
            return results;
        }

        public static int CountContains(this GameDiagonal input, params GameDiagonal[] tests)
        {
            int count = 0;
            foreach (GameDiagonal test in tests)
            {
                if (input.Test(test)) count++;
            }
            return count;
        }
    }

    public static class GameDiagonalHelper
    {

        public static GameDiagonal All = GameDiagonal.NorthWest | GameDiagonal.NorthEast | GameDiagonal.SouthEast | GameDiagonal.SouthWest;
        public static GameDiagonal Nil = 0;

        public static GameDiagonal Invert(GameDiagonal input)
        {
            if (input == Nil) return All;
            if (input == All) return Nil;

            GameDiagonal output = Nil;
            if (!input.Test(GameDiagonal.NorthWest)) output |= GameDiagonal.NorthWest;
            if (!input.Test(GameDiagonal.NorthEast)) output |= GameDiagonal.NorthEast;
            if (!input.Test(GameDiagonal.SouthEast)) output |= GameDiagonal.SouthEast;
            if (!input.Test(GameDiagonal.SouthWest)) output |= GameDiagonal.SouthWest;
            return output;
        }

        public static GameDiagonal Opposite(GameDiagonal input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDiagonal output = Nil;
            if (input.Test(GameDiagonal.NorthWest)) output |= GameDiagonal.SouthEast;
            if (input.Test(GameDiagonal.NorthEast)) output |= GameDiagonal.SouthWest;
            if (input.Test(GameDiagonal.SouthEast)) output |= GameDiagonal.NorthWest;
            if (input.Test(GameDiagonal.SouthWest)) output |= GameDiagonal.NorthEast;
            return output;
        }

        public static GameDiagonal RotateCW(GameDiagonal input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDiagonal output = Nil;
            if (input.Test(GameDiagonal.NorthWest)) output |= GameDiagonal.NorthEast;
            if (input.Test(GameDiagonal.NorthEast)) output |= GameDiagonal.SouthEast;
            if (input.Test(GameDiagonal.SouthEast)) output |= GameDiagonal.SouthWest;
            if (input.Test(GameDiagonal.SouthWest)) output |= GameDiagonal.NorthWest;
            return output;
        }

        public static GameDiagonal RotateCCW(GameDiagonal input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDiagonal output = Nil;
            if (input.Test(GameDiagonal.NorthWest)) output |= GameDiagonal.SouthWest;
            if (input.Test(GameDiagonal.NorthEast)) output |= GameDiagonal.SouthEast;
            if (input.Test(GameDiagonal.SouthEast)) output |= GameDiagonal.NorthEast;
            if (input.Test(GameDiagonal.SouthWest)) output |= GameDiagonal.NorthWest;
            return output;
        }

        /**
         * Return in degrees
         */
        public static float AngleFrom(GameDiagonal from, GameDiagonal to)
        {
            switch (from)
            {
                case GameDiagonal.NorthWest:
                    switch (to)
                    {
                        case GameDiagonal.NorthWest: return 0f;
                        case GameDiagonal.NorthEast: return 90f;
                        case GameDiagonal.SouthEast: return 180f;
                        case GameDiagonal.SouthWest: return 270f;
                    }
                    break;
                case GameDiagonal.NorthEast:
                    switch (to)
                    {
                        case GameDiagonal.NorthWest: return 270f;
                        case GameDiagonal.NorthEast: return 0f;
                        case GameDiagonal.SouthEast: return 90f;
                        case GameDiagonal.SouthWest: return 180f;
                    }
                    break;
                case GameDiagonal.SouthEast:
                    switch (to)
                    {
                        case GameDiagonal.NorthWest: return 180f;
                        case GameDiagonal.NorthEast: return 270f;
                        case GameDiagonal.SouthEast: return 0f;
                        case GameDiagonal.SouthWest: return 90f;
                    }
                    break;
                case GameDiagonal.SouthWest:
                    switch (to)
                    {
                        case GameDiagonal.NorthWest: return 90f;
                        case GameDiagonal.NorthEast: return 180f;
                        case GameDiagonal.SouthEast: return 270f;
                        case GameDiagonal.SouthWest: return 0f;
                    }
                    break;
            }

            return 0f;
        }

        public static GameDiagonal RandomSingle
        {
            get
            {
                float val = Random.value;
                if (val <= 0.25f) return GameDiagonal.NorthWest;
                if (val <= 0.5f) return GameDiagonal.NorthEast;
                if (val <= 0.75f) return GameDiagonal.SouthEast;
                return GameDiagonal.SouthWest;
            }
        }
    }
}