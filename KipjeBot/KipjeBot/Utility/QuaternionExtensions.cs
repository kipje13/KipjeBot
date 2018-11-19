using System;
using System.Numerics;

namespace KipjeBot.Utility
{
    public static class QuaternionExtensions
    {
        public static Vector3 ToEulerAngles(this Quaternion quaternion)
        {
            Vector3 v = new Vector3();

            // roll (x-axis rotation)
            double sinr_cosp = +2.0 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z);
            double cosr_cosp = +1.0 - 2.0 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);
            v.X = -(float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = +2.0 * (quaternion.W * quaternion.Y - quaternion.Z * quaternion.X);
            if (Math.Abs(sinp) >= 1)
                v.Y = -(float)((Math.PI / 2) * Math.Sign(sinp)); // use 90 degrees if out of range
            else
                v.Y = -(float)Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = +2.0 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            double cosy_cosp = +1.0 - 2.0 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            v.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return v;
        }

        public static Vector3 ToRotationAxis(this Quaternion quaternion)
        {
            if (quaternion.W > 1)
                quaternion = Quaternion.Normalize(quaternion);

            double theta = 2 * Math.Acos(MathUtility.Clip(quaternion.W, -1, 1));

            
            if (theta > Math.PI)
                theta = -2 * Math.PI + theta;

            double s = Math.Sqrt(1 - quaternion.W * quaternion.W);

            Vector3 result;

            if (s < 0.001)
                result = new Vector3(quaternion.X, quaternion.Y, quaternion.Z);
            else
                result = new Vector3((float)(quaternion.X / s), (float)(quaternion.Y / s), (float)(quaternion.Z / s));

            result *= (float)theta;

            if (float.IsNaN(result.X))
            {
                return new Vector3(quaternion.X, quaternion.Y, quaternion.Z); ;
            }

            return result;
        }
    }
}
