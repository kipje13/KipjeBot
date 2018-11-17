using System;
using System.Numerics;

using KipjeBot.Utility;

namespace KipjeBot
{
    /// <summary>
    /// A class to help rotate the car in the air.
    /// Adapted version of chip's AerialTurn class: 
    /// </summary>
    public static class RotationController
    {
        private const float ALPHA_MAX = 9.0f;

        private static Vector3 AerialRollPitchYaw(Vector3 omega_start, Vector3 omega_end, Quaternion theta_start, float dt)
        {
            const float T_r = -36.07956616966136f; // torque coefficient for roll
            const float T_p = -12.14599781908070f; // torque coefficient for pitch
            const float T_y = 8.91962804287785f; // torque coefficient for yaw
            const float D_r = -4.47166302201591f; // drag coefficient for roll
            const float D_p = -2.798194258050845f; // drag coefficient for pitch
            const float D_y = -1.886491900437232f; // drag coefficient for yaw

            // net torque in world coordinates
            Vector3 tau = (omega_end - omega_start) / dt;

            // net torque in local coordinates
            tau = Vector3.Transform(tau, Quaternion.Inverse(theta_start));

            // beginning-step angular velocity, in local coordinates
            Vector3 omega_local = Vector3.Transform(omega_start, Quaternion.Inverse(theta_start));

            Vector3 rhs = new Vector3(tau.X - D_r * omega_local.X,
                                      tau.Y - D_p * omega_local.Y,
                                      tau.Z - D_y * omega_local.Z);

            // user inputs: roll, pitch, yaw
            Vector3 u = new Vector3(rhs.X / T_r,
                                    rhs.Y / (T_p + Math.Sign(rhs.Y) * omega_local.Y * D_p),
                                    rhs.Z / (T_y - Math.Sign(rhs.Z) * omega_local.Z * D_y));

            // ensure that values are between -1 and +1 
            u.X = MathUtility.Clip(u.X, -1, 1);
            u.Y = MathUtility.Clip(u.Y, -1, 1);
            u.Z = MathUtility.Clip(u.Z, -1, 1);

            return u;
        }

        /// <summary>
        /// Returns the inputs required to rotate the car to a desired orientation.
        /// </summary>
        /// <param name="car">The car that needs to be rotated.</param>
        /// <param name="target">The desired rotation.</param>
        /// <param name="dt">The time till the next frame in seconds.</param>
        /// <returns>Returns a Vector3 where X is roll, Y is pitch and Z is yaw.</returns>
        public static Vector3 GetInputs(Car car, Quaternion target, float dt)
        {
            Quaternion relativeRotation = Quaternion.Multiply(Quaternion.Inverse(car.Rotation), target);
            Vector3 geodesicLocal = relativeRotation.ToRotationAxis();

            // figure out the axis of minimal rotation to target
            Vector3 geodesicWorld = Vector3.Transform(geodesicLocal, car.Rotation);

            // get the angular acceleration
            Vector3 alpha = new Vector3(Controller(geodesicWorld.X, car.AngularVelocity.X, dt),
                                        Controller(geodesicWorld.Y, car.AngularVelocity.Y, dt),
                                        Controller(geodesicWorld.Z, car.AngularVelocity.Z, dt));

            // reduce the corrections for when the solution is nearly converged
            Vector3 error = Vector3.Abs(geodesicWorld) + Vector3.Abs(car.AngularVelocity);
            alpha = q(error) * alpha;

            // set the desired next angular velocity
            Vector3 omega_next = car.AngularVelocity + alpha * dt;

            // determine the controls that produce that angular velocity
            Vector3 rollPitchYaw = AerialRollPitchYaw(car.AngularVelocity, omega_next, car.Rotation, dt);

            return rollPitchYaw;
        }

        private static float Controller(float delta, float v, float dt)
        {
            float ri = r(delta, v);

            float alpha = Math.Sign(ri) * ALPHA_MAX;

            float rf = r(delta - v * dt, v + alpha * dt);

            // use a single step of secant method to improve
            // the acceleration when residual changes sign
            if (ri * rf < 0)
                alpha *= (2.0f * (ri / (ri - rf)) - 1);

            return alpha;
        }

        private static float r(float delta, float v)
        {
            return delta - 0.5f * Math.Sign(v) * v * v / ALPHA_MAX;
        }

        private static Vector3 q(Vector3 x)
        {
            return Vector3.One - (Vector3.One / (Vector3.One + 500.0f * x * x));
        }
    }
}
