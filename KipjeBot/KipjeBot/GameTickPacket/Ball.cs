using System.Numerics;

using KipjeBot.Utility;

namespace KipjeBot
{
    public class Ball
    {
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

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
    }
}
