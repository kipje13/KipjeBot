using System;
using System.Numerics;

using RLBotDotNet;

using KipjeBot.Utility;

namespace KipjeBot.Actions
{
    public class Recovery
    {
        public bool Finished { get; private set; } = false;

        private Car car;
        private Quaternion targetRotation;

        public Recovery(Car car)
        {
            this.car = car;
            targetRotation = FindLandingOrientation(car);
        }

        public Controller Step(float dt)
        {
            Finished = car.HasWheelContact;

            Vector3 inputs = RotationController.GetInputs(car, targetRotation, dt);

            Controller controller = new Controller();
            controller.Roll = inputs.X;
            controller.Pitch = inputs.Y;
            controller.Yaw = inputs.Z;

            return controller;
        }

        public static Quaternion FindLandingOrientation(Car car)
        {
            Car c = new Car(car);
            Vector3 normal;

            for (int i = 0; i < 300; i++)
            {
                c.Simulate(new Controller(), 1 / 60f);

                if (Physics.IntersectSphere(c.Position, 40, out normal))
                {
                    Vector3 forward;

                    if (c.Velocity != Vector3.Zero)
                        forward = Vector3.Normalize(c.Velocity - Vector3.Dot(c.Velocity, normal) * normal);
                    else
                        forward = c.Forward;

                    Quaternion target = MathUtility.LookAt(forward, normal);
                    return target;
                }
            }

            return Quaternion.Identity;
        }
    }
}
