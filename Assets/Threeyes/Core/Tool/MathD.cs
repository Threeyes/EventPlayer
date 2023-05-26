using System;

namespace Threeyes.Utility
{
    /// <summary>
    /// Math for double
    /// </summary>
    public static class Mathd
    {
        public static int CeilToInt(double f)
        {
            return (int)Math.Ceiling(f);
        }
        public static double Repeat(double t, double length)
        {
            return Clamp(t - Floor(t / length) * length, 0f, length);
        }
        public static double Floor(double f)
        {
            return Math.Floor(f);
        }
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }
        public static double Abs(double f)
        {
            return Math.Abs(f);
        }
    }
}
