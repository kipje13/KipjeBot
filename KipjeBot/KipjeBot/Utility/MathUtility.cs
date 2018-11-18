using System;
using System.Numerics;

namespace KipjeBot.Utility
{
    public static class MathUtility
    {
        /// <summary>
        /// Clips a value between a min and a max.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clip(float value, float min, float max)
        {
            return Math.Min(Math.Max(min, value), max);
        }

        /// <summary>
        /// Linearly interpolates a value.
        /// </summary>
        /// <param name="a">Value at t = 0.</param>
        /// <param name="b">Value at t = 1.</param>
        /// <param name="t">Float between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return a * (1.0f - t) + b * t;
        }

        /// <summary>
        /// Creates a Quaternion that points in the same direction as the forward vector.
        /// </summary>
        /// <param name="forward">The vector that specifies the direction.</param>
        /// <returns>The quaterion with the desired rotation.</returns>
        public static Quaternion LookAt(Vector3 forward)
        {
            return LookAt(forward, Vector3.UnitZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forward">The vector that specifies the direction.</param>
        /// <param name="up">A vector that specifies the roll of the rotation.</param>
        /// <returns>The quaterion with the desired rotation.</returns>
        public static Quaternion LookAt(Vector3 forward, Vector3 up)
        {
            Vector3 forwardVector = Vector3.Normalize(forward);

            float dot = Vector3.Dot(Vector3.UnitX, forwardVector);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                return new Quaternion(up, 3.1415926535897932f);
            }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                return Quaternion.Identity;
            }

            float rotAngle = (float)Math.Acos(dot);
            Vector3 rotAxis = Vector3.Cross(Vector3.UnitX, forwardVector);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }
    }
}
