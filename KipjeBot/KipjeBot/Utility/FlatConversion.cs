using System;
using System.Numerics;

namespace KipjeBot.Utility
{
    public static class FlatConversion
    {
        /// <summary>
        /// Converts a flatbuffers vector to a System.Numerics vector.
        /// </summary>
        /// <param name="vector">The flatbuffers vector.</param>
        /// <returns>Returns a System.Numerics vector.</returns>
        public static Vector3 ToVector3(this rlbot.flat.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts a flatbuffers rotator to a System.Numerics quaternion.
        /// </summary>
        /// <param name="rotator">The flatbuffers rotator</param>
        /// <returns>Returns a System.Numerics quaternion.</returns>
        public static Quaternion ToQuaternion(this rlbot.flat.Rotator rotator)
        {
            Quaternion roll = Quaternion.CreateFromYawPitchRoll(0, -rotator.Roll, 0);
            Quaternion pitch = Quaternion.CreateFromYawPitchRoll(-rotator.Pitch, 0, 0);
            Quaternion yaw = Quaternion.CreateFromYawPitchRoll(0, 0, rotator.Yaw);

            return Quaternion.Multiply(yaw, Quaternion.Multiply(pitch, roll));
        }

        /// <summary>
        /// Converts a flatbuffers quaternion to a System.Numerics quaternion.
        /// </summary>
        /// <param name="quaternion">The flatbuffers quaternion.</param>
        /// <returns>Returns a System.Numerics quaternion.</returns>
        public static Quaternion ToQuaternion(this rlbot.flat.Quaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
