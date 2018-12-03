using System;
using System.Numerics;

using RLBotDotNet;

using KipjeBot.Utility;

namespace KipjeBot
{
    public class Car
    {
        public const float BoostAcceleration = 1000f;

        #region Properties
        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        public Vector3 Forward { get; private set; }
        public Vector3 Left { get; private set; }
        public Vector3 Up { get; private set; }

        public bool Jumped { get; private set; }
        public bool DoubleJumped { get; private set; }
        public bool HasWheelContact { get; private set; }

        public bool CanDodge { get; private set; }
        public float DodgeTimer { get; private set; } = 0;

        public bool IsSupersonic { get; private set; }
        public bool IsDemolished { get; private set; }

        public int Boost { get; private set; }

        public string Name { get; private set; }
        public int Team { get; private set; } 
        #endregion

        #region Constructors
        public Car() { }

        public Car(Car car)
        {
            Position = car.Position;
            Velocity = car.Velocity;
            Rotation = car.Rotation;
            AngularVelocity = car.AngularVelocity;

            Forward = Vector3.Transform(Vector3.UnitX, Rotation);
            Left = Vector3.Transform(Vector3.UnitY, Rotation);
            Up = Vector3.Transform(Vector3.UnitZ, Rotation);

            Jumped = car.Jumped;
            DoubleJumped = car.DoubleJumped;
            HasWheelContact = car.HasWheelContact;

            CanDodge = car.CanDodge;
            DodgeTimer = car.DodgeTimer;

            IsSupersonic = car.IsSupersonic;
            IsDemolished = car.IsDemolished;

            Boost = car.Boost;

            Name = car.Name;
            Team = car.Team;
        } 
        #endregion

        #region Update
        public void Update(rlbot.flat.PlayerInfo car, float dt)
        {
            if (car.Physics.HasValue)
            {
                if (car.Physics.Value.Location.HasValue)
                    Position = car.Physics.Value.Location.Value.ToVector3();

                if (car.Physics.Value.Velocity.HasValue)
                    Velocity = car.Physics.Value.Velocity.Value.ToVector3();

                if (car.Physics.Value.Rotation.HasValue)
                    Rotation = car.Physics.Value.Rotation.Value.ToQuaternion();

                if (car.Physics.Value.AngularVelocity.HasValue)
                    AngularVelocity = car.Physics.Value.AngularVelocity.Value.ToVector3();
            }

            Forward = Vector3.Transform(Vector3.UnitX, Rotation);
            Left = Vector3.Transform(Vector3.UnitY, Rotation);
            Up = Vector3.Transform(Vector3.UnitZ, Rotation);

            Jumped = car.Jumped;
            DoubleJumped = car.DoubleJumped;
            HasWheelContact = car.HasWheelContact;

            if (HasWheelContact)
            {
                CanDodge = false;
                DodgeTimer = 1.5f;
            }
            else if (DoubleJumped)
            {
                CanDodge = false;
                DodgeTimer = 0;
            }
            else if (Jumped)
            {
                DodgeTimer -= dt;

                if (DodgeTimer < 0)
                    DodgeTimer = 0;

                CanDodge = DodgeTimer > 0f;
            }
            else
            {
                CanDodge = true;
            }

            IsSupersonic = car.IsSupersonic;
            IsDemolished = car.IsDemolished;

            Boost = car.Boost;

            Team = car.Team;
            Name = car.Name;
        }

        public void Update(rlbot.flat.PlayerRigidBodyState car, float dt)
        {
            if (car.State.HasValue)
            {
                if (car.State.Value.Location.HasValue)
                    Position = car.State.Value.Location.Value.ToVector3();

                if (car.State.Value.Velocity.HasValue)
                    Velocity = car.State.Value.Velocity.Value.ToVector3();

                if (car.State.Value.Rotation.HasValue)
                    Rotation = car.State.Value.Rotation.Value.ToQuaternion();

                if (car.State.Value.AngularVelocity.HasValue)
                    AngularVelocity = car.State.Value.AngularVelocity.Value.ToVector3();
            }

            Forward = Vector3.Transform(Vector3.UnitX, Rotation);
            Left = Vector3.Transform(Vector3.UnitY, Rotation);
            Up = Vector3.Transform(Vector3.UnitZ, Rotation);
        } 
        #endregion 

        /// <summary>
        /// Extrapolates the physics of this car using the given inputs.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dt"></param>
        public void Simulate(Controller input, float dt)
        {
            if (HasWheelContact)
            {
                /// TODO: Driving simulation
            }
            else
            {
                SimulateAir(input, dt);
            }
        }

        public void SimulateAir(Controller input, float dt)
        {
            const float M = 180.0f;  // mass
            const float J = 10.5f;   // moment of inertia
            const float v_max = 2300.0f;
            const float w_max = 5.5f;
            const float boost_force = 178500.0f;
            const float throttle_force = 12000.0f;
            Vector3 g = new Vector3(0.0f, 0.0f, -651.47f);

            Vector3 rpy =new Vector3(input.Roll, input.Pitch, input.Yaw);

            // air control torque coefficients
            Vector3 T = new Vector3(-400.0f, -130.0f, 95.0f);

            // air damping torque coefficients
            Vector3 H = new Vector3(
                -50.0f, -30.0f * (1.0f - Math.Abs(input.Pitch)),
               -20.0f * (1.0f - Math.Abs(input.Yaw)));

            float thrust = 0.0f;

            if (input.Boost && Boost > 0)
            {
                thrust = (boost_force + throttle_force);
                Boost--;
            }
            else
            {
                thrust = input.Throttle* throttle_force;
            }

            Velocity += (g + (thrust / M) * Forward) * dt;
            Position += Velocity * dt;

            Vector3 w_local = Vector3.Transform(AngularVelocity, Quaternion.Inverse(Rotation));

            Vector3 old_w = AngularVelocity;
            AngularVelocity += Vector3.Transform(T * rpy + H * w_local, Rotation) * (dt / J);

            Vector3 angleAxis = 0.5f * (AngularVelocity + old_w) * dt;
            Quaternion R = Quaternion.CreateFromAxisAngle(Vector3.Normalize(angleAxis), angleAxis.Length());

            Rotation = Quaternion.Multiply(R, Rotation);

            // if the velocities exceed their maximum values, scale them back
            Velocity /= Math.Max(1.0f, Velocity.Length() / v_max);
            AngularVelocity /= Math.Max(1.0f, AngularVelocity.Length() / w_max);
        }
    }
}
