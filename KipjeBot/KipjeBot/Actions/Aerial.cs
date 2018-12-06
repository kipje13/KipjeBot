using System;
using System.Numerics;

using RLBotDotNet;

using KipjeBot.Utility;

namespace KipjeBot.Actions
{
    public class Aerial
    {
        public Vector3 Target;
        public Car Car;
        public float StartTime;
        public float ArrivalTime;

        public bool Finished { get; private set; } = false;

        const float jump_t = 0.25f;
        const float jump_dx = 100.0f;
        const float jump_dv = 600.0f;

        private DoubleJump doubleJump;

        public Aerial(Car car, Vector3 target, float startTime, float arrivalTime)
        {
            Target = target;
            Car = car;
            StartTime = startTime;
            ArrivalTime = arrivalTime;

            if (car.HasWheelContact)
            {
                doubleJump = new DoubleJump();
            }
        }

        public Controller Step(Ball ball, float dt, float currentTime)
        {
            Vector3 A = CalculateCourse(Car, Target, ArrivalTime - currentTime);

            Controller c = new Controller();

            Vector3 dir = Vector3.Normalize(A);

            if (doubleJump != null && !doubleJump.Finished)
            {
                c = doubleJump.Step();
            }
            else
            {
                Quaternion t;

                Car test = new Car(Car);

                for (int i = 0; i < (int)((ArrivalTime - currentTime) / 0.016667f); i++)
                {
                    test.Simulate(new Controller(), 0.016667f);
                }

                if ((Target - test.Position).Length() > 120)
                    t = MathUtility.LookAt(dir, Car.Up);
                else
                    t = MathUtility.LookAt(Vector3.Normalize(ball.Position - Car.Position), Car.Up);


                Vector3 inputs = RotationController.GetInputs(Car, t, dt);

                c.Roll = inputs.X;
                c.Pitch = inputs.Y;
                c.Yaw = inputs.Z;

                if (MathUtility.Angle(Car.Rotation, MathUtility.LookAt(dir, Car.Up)) < 0.4f)
                    c.Boost = true;
            }

            if (currentTime > ArrivalTime || A.Length() > 1050)
                Finished = true;

            return c;
        }

        /// <summary>
        /// Updates the aerial target so the bot wont miss the ball.
        /// </summary>
        /// <param name="slices"></param>
        public void UpdateAerialTarget(Slice[] slices)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                if (Math.Abs(slices[i].Time - ArrivalTime) < 0.02)
                {
                    if ((Target - slices[i].Position).Length() > 40)
                    {
                        Finished = true; /// TODO: Calculate a new intercept location.
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Tries to create an Aerial action from the ball prediction. Returns null when no suitable aerial is found.
        /// </summary>
        /// <param name="car"></param>
        /// <param name="slices"></param>
        /// <param name="currentTime"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Aerial FindAerialOpportunity(Car car, Slice[] slices, float currentTime, AerialSettings settings)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                float B_avg = CalculateCourse(car, slices[i].Position, slices[i].Time - currentTime).Length();

                if (B_avg > settings.MinAcceleration && B_avg < settings.MaxAcceleration)
                {
                    return new Aerial(car, slices[i].Position, currentTime, slices[i].Time);
                }
            }

            return null;
        }

        /// <summary>
        /// Calculates the required acceleration to arrive at a location in a given amount of time.
        /// </summary>
        /// <param name="car">The car to use in the calculation.</param>
        /// <param name="target">The target location for the aerial.</param>
        /// <param name="time">The time allowed to complete the aerial.</param>
        /// <returns></returns>
        public static Vector3 CalculateCourse(Car car, Vector3 target, float time)
        {
            Vector3 z = Vector3.UnitZ;
            Vector3 P = target;
            Vector3 x0 = car.Position;
            Vector3 v0 = car.Velocity;

            float delta_t = time;

            if (car.HasWheelContact)
            {
                v0 += car.Up * jump_dv;
                x0 += car.Up * jump_dx;
            }

            float g = -650.0f;
            float a = 9.0f;

            Vector3 A = P - x0 - v0 * delta_t - 0.5f * g * delta_t * delta_t * z;

            Vector3 dir = Vector3.Normalize(A);

            // estimate the time required to turn
            float phi = MathUtility.Angle(car.Rotation, MathUtility.LookAt(dir, car.Up));

            float T = (float)(0.7 * (2.0 * Math.Sqrt(phi / a)));           

            // see if the boost acceleration needed to reach the target is achievable
            return dir * 2.0f * A.Length() / ((delta_t - T) * (delta_t - T));
        }
    }

    public struct AerialSettings
    {
        public float MinAcceleration;
        public float MaxAcceleration;

        public float MinHeight;
        public float MaxHeight;

        public AerialSettings(float minAcceleration, float maxAcceleration, float minHeight, float maxHeight)
        {
            MinAcceleration = minAcceleration;
            MaxAcceleration = maxAcceleration;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
        }
    }
}
