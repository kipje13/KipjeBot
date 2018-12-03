using System;
using System.Numerics;

using KipjeBot.Utility;

namespace KipjeBot
{
    public class Ball
    {
        public const float Radius = 92.75f;

        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        public Ball() { }

        public Ball(Ball ball)
        {
            Position = ball.Position;
            Velocity = ball.Velocity;
            Rotation = ball.Rotation;
            AngularVelocity = ball.AngularVelocity;
        }

        public void Update(rlbot.flat.BallInfo ball)
        {
            if (ball.Physics.HasValue)
            {
                if (ball.Physics.Value.Location.HasValue)
                    Position = ball.Physics.Value.Location.Value.ToVector3();

                if (ball.Physics.Value.Velocity.HasValue)
                    Velocity = ball.Physics.Value.Velocity.Value.ToVector3();

                if (ball.Physics.Value.Rotation.HasValue)
                    Rotation = ball.Physics.Value.Rotation.Value.ToQuaternion();

                if (ball.Physics.Value.AngularVelocity.HasValue)
                    AngularVelocity = ball.Physics.Value.AngularVelocity.Value.ToVector3();
            }
            
        }

        public void Update(rlbot.flat.BallRigidBodyState ball)
        {
            if (ball.State.HasValue)
            {
                if (ball.State.Value.Location.HasValue)
                    Position = ball.State.Value.Location.Value.ToVector3();

                if (ball.State.Value.Velocity.HasValue)
                    Velocity = ball.State.Value.Velocity.Value.ToVector3();

                if (ball.State.Value.Rotation.HasValue)
                    Rotation = ball.State.Value.Rotation.Value.ToQuaternion();

                if (ball.State.Value.AngularVelocity.HasValue)
                    AngularVelocity = ball.State.Value.AngularVelocity.Value.ToVector3();
            }
        }

        /// <summary>
        /// Extrapolates the physics of the ball.
        /// Made by Chip: https://github.com/samuelpmish/RLUtilities/blob/master/RLUtilities/cpp/inc/ball.h
        /// </summary>
        /// <param name="dt">The time between the current state and the extrapolated state.</param>
        public void Simulate(float dt)
        {
            const float R = Radius;        // ball radius
            const float G = -650.0f;       // gravitational acceleration
            const float A = 0.0003f;       // inverse moment of inertia
            const float Y = 2.0f;          // maximum frictional contribution
            const float mu = 0.280f;       // Coulomb friction coefficient
            const float C_R = 0.6f;        // coefficient of restitution
            const float drag = -0.0305f;   // velocity-proportional drag coefficient
            const float w_max = 6.0f;      // maximum angular velocity

            if (Velocity.Length() > 0.0001)
            {
                Vector3 a = drag * Velocity + new Vector3(0, 0, G);
                Vector3 v_pred = Velocity + a * dt;
                Vector3 x_pred = Position + v_pred * dt;
                Vector3 w_pred = AngularVelocity;

                Vector3 normal;
                if (Physics.IntersectSphere(x_pred, R, out normal))
                {
                    Vector3 n = normal;

                    Vector3 v_perp = Vector3.Dot(v_pred, n) * n;
                    Vector3 v_para = v_pred - v_perp;
                    Vector3 v_spin = R * Vector3.Cross(n, w_pred);
                    Vector3 s = v_para + v_spin;

                    float ratio = v_perp.Length() / s.Length();

                    Vector3 delta_v_perp = -(1.0f + C_R) * v_perp;
                    Vector3 delta_v_para = -Math.Min(1.0f, Y * ratio) * mu * s;

                    AngularVelocity = w_pred + A * R * Vector3.Cross(delta_v_para, n);
                    Velocity = v_pred + delta_v_perp + delta_v_para;

                    Position = Position + 0.5f * (Velocity + v_pred) * dt;
                }
                else
                {
                    AngularVelocity = w_pred;
                    Velocity = v_pred;
                    Position = x_pred;
                }
            }

            AngularVelocity *= Math.Min(1.0f, w_max / AngularVelocity.Length());
        }
    }
}
