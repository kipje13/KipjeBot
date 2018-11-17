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
    }
}
