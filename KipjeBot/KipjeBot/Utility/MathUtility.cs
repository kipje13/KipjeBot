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
            Vector3 left = Vector3.Cross(up, forward);

            Matrix4x4 m = Matrix4x4.Identity;

            m.M11 = forward.X;
            m.M12 = forward.Y;
            m.M13 = forward.Z;

            m.M21 = left.X;
            m.M22 = left.Y;
            m.M23 = left.Z;

            m.M31 = up.X;
            m.M32 = up.Y;
            m.M33 = up.Z;

            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            return q;
        }

        /// <summary>
        /// Calculates the angle between two quaternions.
        /// </summary>
        /// <param name="a">First quaternion.</param>
        /// <param name="b">Second quaternion.</param>
        /// <returns>Returns the angle in radians between two quaternions.</returns>
        public static float Angle(Quaternion a, Quaternion b)
        {
            float dot = Quaternion.Dot(a, b);

            float alpha = dot > (1 - 0.000001F) ? 0.0f : (float)Math.Acos(Math.Min(Math.Abs(dot), 1.0F)) * 2.0F;

            return alpha;
        }
    }
}
