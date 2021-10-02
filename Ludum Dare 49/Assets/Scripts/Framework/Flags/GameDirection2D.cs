namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Flags]
    public enum GameDirection2D : short
    {
        Up = (1 << 0), // 1 in decimal
        Right = (1 << 1), // 2 in decimal
        Down = (1 << 2), // 4 in decimal
        Left = (1 << 3), // 8 in decimal
    }

    public static class GameDirection2DExtensions
    {
        public static bool Test(this GameDirection2D input, GameDirection2D test)
        {
            return (input & test) == test;
        }

        public static bool Within(this GameDirection2D input, GameDirection2D test)
        {
            return (input & test) == input;
        }

        public static GameDirection2D Invert(this GameDirection2D input)
        {
            return GameDirection2DHelper.Invert(input);
        }

        public static GameDirection2D Opposite(this GameDirection2D input)
        {
            return GameDirection2DHelper.Opposite(input);
        }

        public static GameDirection2D RotateCW(this GameDirection2D input)
        {
            return GameDirection2DHelper.RotateCW(input);
        }

        public static GameDirection2D RotateCCW(this GameDirection2D input)
        {
            return GameDirection2DHelper.RotateCCW(input);
        }

        public static GameDirection2D RandomSingle(this GameDirection2D input)
        {
            List<GameDirection2D> options = new List<GameDirection2D>();
            if (input.Test(GameDirection2D.Up)) options.Add(GameDirection2D.Up);
            if (input.Test(GameDirection2D.Right)) options.Add(GameDirection2D.Right);
            if (input.Test(GameDirection2D.Down)) options.Add(GameDirection2D.Down);
            if (input.Test(GameDirection2D.Left)) options.Add(GameDirection2D.Left);
            if (options.Count == 0) return GameDirection2DHelper.Nil;
            return options[Random.Range(0, options.Count)];
        }

        public static int Count(this GameDirection2D input)
        {
            int total = 0;
            if (input.Test(GameDirection2D.Up)) total++;
            if (input.Test(GameDirection2D.Right)) total++;
            if (input.Test(GameDirection2D.Down)) total++;
            if (input.Test(GameDirection2D.Left)) total++;
            return total;
        }

        public static List<GameDirection2D> ToList(this GameDirection2D input)
        {
            List<GameDirection2D> results = new List<GameDirection2D>();
            if (input.Test(GameDirection2D.Up)) results.Add(GameDirection2D.Up);
            if (input.Test(GameDirection2D.Right)) results.Add(GameDirection2D.Right);
            if (input.Test(GameDirection2D.Down)) results.Add(GameDirection2D.Down);
            if (input.Test(GameDirection2D.Left)) results.Add(GameDirection2D.Left);
            return results;
        }

        public static int CountContains(this GameDirection2D input, params GameDirection2D[] tests)
        {
            int count = 0;
            foreach (GameDirection2D test in tests)
            {
                if (input.Test(test)) count++;
            }
            return count;
        }
    }

    public static class GameDirection2DHelper
    {

        public static GameDirection2D All = GameDirection2D.Up | GameDirection2D.Right | GameDirection2D.Down | GameDirection2D.Left;
        public static GameDirection2D Nil = 0;

        public static GameDirection2D Invert(GameDirection2D input)
        {
            if (input == Nil) return All;
            if (input == All) return Nil;

            GameDirection2D output = Nil;
            if (!input.Test(GameDirection2D.Up)) output |= GameDirection2D.Up;
            if (!input.Test(GameDirection2D.Right)) output |= GameDirection2D.Right;
            if (!input.Test(GameDirection2D.Down)) output |= GameDirection2D.Down;
            if (!input.Test(GameDirection2D.Left)) output |= GameDirection2D.Left;
            return output;
        }

        public static GameDirection2D Opposite(GameDirection2D input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection2D output = Nil;
            if (input.Test(GameDirection2D.Up)) output |= GameDirection2D.Down;
            if (input.Test(GameDirection2D.Right)) output |= GameDirection2D.Left;
            if (input.Test(GameDirection2D.Down)) output |= GameDirection2D.Up;
            if (input.Test(GameDirection2D.Left)) output |= GameDirection2D.Right;
            return output;
        }

        public static GameDirection2D RotateCW(GameDirection2D input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection2D output = Nil;
            if (input.Test(GameDirection2D.Up)) output |= GameDirection2D.Right;
            if (input.Test(GameDirection2D.Right)) output |= GameDirection2D.Down;
            if (input.Test(GameDirection2D.Down)) output |= GameDirection2D.Left;
            if (input.Test(GameDirection2D.Left)) output |= GameDirection2D.Up;
            return output;
        }

        public static GameDirection2D RotateCCW(GameDirection2D input)
        {
            if (input == Nil) return Nil;
            if (input == All) return All;

            GameDirection2D output = Nil;
            if (input.Test(GameDirection2D.Up)) output |= GameDirection2D.Left;
            if (input.Test(GameDirection2D.Right)) output |= GameDirection2D.Down;
            if (input.Test(GameDirection2D.Down)) output |= GameDirection2D.Right;
            if (input.Test(GameDirection2D.Left)) output |= GameDirection2D.Up;
            return output;
        }

        /**
         * Return in degrees
         */
        public static float AngleFrom(GameDirection2D from, GameDirection2D to)
        {
            switch (from)
            {
                case GameDirection2D.Up:
                    switch (to)
                    {
                        case GameDirection2D.Up: return 0f;
                        case GameDirection2D.Right: return 90f;
                        case GameDirection2D.Down: return 180f;
                        case GameDirection2D.Left: return 270f;
                    }
                    break;
                case GameDirection2D.Right:
                    switch (to)
                    {
                        case GameDirection2D.Up: return 270f;
                        case GameDirection2D.Right: return 0f;
                        case GameDirection2D.Down: return 90f;
                        case GameDirection2D.Left: return 180f;
                    }
                    break;
                case GameDirection2D.Down:
                    switch (to)
                    {
                        case GameDirection2D.Up: return 180f;
                        case GameDirection2D.Right: return 270f;
                        case GameDirection2D.Down: return 0f;
                        case GameDirection2D.Left: return 90f;
                    }
                    break;
                case GameDirection2D.Left:
                    switch (to)
                    {
                        case GameDirection2D.Up: return 90f;
                        case GameDirection2D.Right: return 180f;
                        case GameDirection2D.Down: return 270f;
                        case GameDirection2D.Left: return 0f;
                    }
                    break;
            }

            return 0f;
        }

        public static GameDirection2D RandomSingle
        {
            get
            {
                float val = Random.value;
                if (val <= 0.25f) return GameDirection2D.Up;
                if (val <= 0.5f) return GameDirection2D.Right;
                if (val <= 0.75f) return GameDirection2D.Down;
                return GameDirection2D.Left;
            }
        }
    }
}