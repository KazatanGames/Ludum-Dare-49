namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Flags]
    public enum GameDirection : short
    {
        North = (1 << 0), // 1 in decimal
        East = (1 << 1), // 2 in decimal
        South = (1 << 2), // 4 in decimal
        West = (1 << 3), // 8 in decimal
    }

    public static class GameDirectionExtensions
    {
        public static bool Test(this GameDirection input, GameDirection test)
        {
            return (input & test) == test;
        }

        public static bool Within(this GameDirection input, GameDirection test)
        {
            return (input & test) == input;
        }

        public static GameDirection Invert(this GameDirection input)
        {
            return GameDirectionHelper.Invert(input);
        }

        public static GameDirection Opposite(this GameDirection input)
        {
            return GameDirectionHelper.Opposite(input);
        }

        public static GameDirection RotateCW(this GameDirection input)
        {
            return GameDirectionHelper.RotateCW(input);
        }

        public static GameDirection RotateCCW(this GameDirection input)
        {
            return GameDirectionHelper.RotateCCW(input);
        }

        public static GameDirection RandomSingle(this GameDirection input)
        {
            List<GameDirection> options = new List<GameDirection>();
            if (input.Test(GameDirection.North)) options.Add(GameDirection.North);
            if (input.Test(GameDirection.East)) options.Add(GameDirection.East);
            if (input.Test(GameDirection.South)) options.Add(GameDirection.South);
            if (input.Test(GameDirection.West)) options.Add(GameDirection.West);
            if (options.Count == 0) return GameDirectionHelper.Nil;
            return options[Random.Range(0, options.Count)];
        }

        public static int Count(this GameDirection input)
        {
            int total = 0;
            if (input.Test(GameDirection.North)) total++;
            if (input.Test(GameDirection.East)) total++;
            if (input.Test(GameDirection.South)) total++;
            if (input.Test(GameDirection.West)) total++;
            return total;
        }

        public static List<GameDirection> ToList(this GameDirection input)
        {
            List<GameDirection> results = new List<GameDirection>();
            if (input.Test(GameDirection.North)) results.Add(GameDirection.North);
            if (input.Test(GameDirection.East)) results.Add(GameDirection.East);
            if (input.Test(GameDirection.South)) results.Add(GameDirection.South);
            if (input.Test(GameDirection.West)) results.Add(GameDirection.West);
            return results;
        }

        public static int CountContains(this GameDirection input, params GameDirection[] tests)
        {
            int count = 0;
            foreach (GameDirection test in tests)
            {
                if (input.Test(test)) count++;
            }
            return count;
        }
    }

    public static class GameDirectionHelper
    {

        public static GameDirection All = GameDirection.North | GameDirection.East | GameDirection.South | GameDirection.West;
        public static GameDirection Nil = 0;

        public static GameDirection Invert(GameDirection input)
        {
            if (input == Nil) return All;
            if (input == All) return Nil;

            GameDirection output = Nil;
            if (!input.Test(GameDirection.North)) output |= GameDirection.North;
            if (!input.Test(GameDirection.East)) output |= GameDirection.East;
            if (!input.Test(GameDirection.South)) output |= GameDirection.South;
            if (!input.Test(GameDirection.West)) output |= GameDirection.West;
            return output;
        }

        public static GameDirection Opposite(GameDirection input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection output = Nil;
            if (input.Test(GameDirection.North)) output |= GameDirection.South;
            if (input.Test(GameDirection.East)) output |= GameDirection.West;
            if (input.Test(GameDirection.South)) output |= GameDirection.North;
            if (input.Test(GameDirection.West)) output |= GameDirection.East;
            return output;
        }

        public static GameDirection RotateCW(GameDirection input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection output = Nil;
            if (input.Test(GameDirection.North)) output |= GameDirection.East;
            if (input.Test(GameDirection.East)) output |= GameDirection.South;
            if (input.Test(GameDirection.South)) output |= GameDirection.West;
            if (input.Test(GameDirection.West)) output |= GameDirection.North;
            return output;
        }

        public static GameDirection RotateCCW(GameDirection input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection output = Nil;
            if (input.Test(GameDirection.North)) output |= GameDirection.West;
            if (input.Test(GameDirection.East)) output |= GameDirection.South;
            if (input.Test(GameDirection.South)) output |= GameDirection.East;
            if (input.Test(GameDirection.West)) output |= GameDirection.North;
            return output;
        }

        /**
         * Return in degrees
         */
        public static float AngleFrom(GameDirection from, GameDirection to)
        {
            switch (from)
            {
                case GameDirection.North:
                    switch (to)
                    {
                        case GameDirection.North: return 0f;
                        case GameDirection.East: return 90f;
                        case GameDirection.South: return 180f;
                        case GameDirection.West: return 270f;
                    }
                    break;
                case GameDirection.East:
                    switch (to)
                    {
                        case GameDirection.North: return 270f;
                        case GameDirection.East: return 0f;
                        case GameDirection.South: return 90f;
                        case GameDirection.West: return 180f;
                    }
                    break;
                case GameDirection.South:
                    switch (to)
                    {
                        case GameDirection.North: return 180f;
                        case GameDirection.East: return 270f;
                        case GameDirection.South: return 0f;
                        case GameDirection.West: return 90f;
                    }
                    break;
                case GameDirection.West:
                    switch (to)
                    {
                        case GameDirection.North: return 90f;
                        case GameDirection.East: return 180f;
                        case GameDirection.South: return 270f;
                        case GameDirection.West: return 0f;
                    }
                    break;
            }

            return 0f;
        }

        public static GameDirection RandomSingle
        {
            get
            {
                float val = Random.value;
                if (val <= 0.25f) return GameDirection.North;
                if (val <= 0.5f) return GameDirection.East;
                if (val <= 0.75f) return GameDirection.South;
                return GameDirection.West;
            }
        }
    }
}