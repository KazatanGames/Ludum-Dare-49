namespace KazatanGames.Framework
{
    using UnityEngine;

    public static class NotSoRandom
    {
        public delegate float EasingMethod(float value);

        public static float RandomWithTarget(float min, float max, float target)
        {
            return RandomWithTarget(min, max, target, Easing.Quadratic.Out, Easing.Quadratic.In);
        }
        public static float RandomWithTarget(float min, float max, float target, EasingMethod lowEasingMethod, EasingMethod highEasingMethod)
        {
            if ((min > target && !Mathf.Approximately(min, target)) || (target > max && !Mathf.Approximately(max, target)))
            {
                Debug.LogError("[NotSoRandom].RandomWithTarget(float) Invalid arguments. (" + min + ", " + max + ", " + target + ")\n" + System.Environment.StackTrace);
            }
            // ensure valid numbers and fix floating point rounding errors
            if (min > target) target = min;
            if (max < target) target = max;

            float low = target - min;
            float high = max - target;

            float lowRand = Random.value;
            float highRand = Random.value;

            float lowRandEased = lowEasingMethod(lowRand);
            float highRandEased = highEasingMethod(highRand);

            float lowPortion = lowRandEased * low;
            float highPortion = highRandEased * high;

            return min + lowPortion + highPortion;
        }

        public static int RandomWithTarget(int min, int max, int target)
        {
            return RandomWithTarget(min, max, target, Easing.Quadratic.Out, Easing.Quadratic.In);
        }
        public static int RandomWithTarget(int min, int max, int target, EasingMethod lowEasingMethod, EasingMethod highEasingMethod)
        {
            if (min > target || target > max)
            {
                Debug.LogError("[NotSoRandom].RandomWithTarget(int) Invalid arguments. (" + min + ", " + max + ", " + target + ")\n" + System.Environment.StackTrace);
            }

            // ensure valid numbers
            if (min > target) target = min;
            if (max < target) target = max;

            int low = target - min;
            int high = max - target;

            float lowRand = Random.value;
            float highRand = Random.value;

            float lowRandEased = lowEasingMethod(lowRand);
            float highRandEased = highEasingMethod(highRand);

            float lowPortion = lowRandEased * low;
            float highPortion = highRandEased * high;

            int lowRounded = Mathf.RoundToInt(lowPortion);
            int highRounded = Mathf.RoundToInt(highPortion);

            return min + lowRounded + highRounded;
        }
    }
}