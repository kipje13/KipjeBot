using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipjeBot.Utility
{
    public static class MathUtility
    {
        public static float Clip(float value, float min, float max)
        {
            return Math.Min(Math.Max(min, value), max);
        }

        public static float Lerp(float a, float b, float t)
        {
            return a * (1.0f - t) + b * t;
        }
    }
}
