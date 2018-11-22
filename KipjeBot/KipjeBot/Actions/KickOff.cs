using System;
using System.Numerics;

using RLBotDotNet;

using KipjeBot.Utility;

namespace KipjeBot.Actions
{
    public enum KickOffPositions { Center, BackCorner, FrontCorner, Unknown };

    public class KickOff
    {
        private Dodge dodge;
        private Car car;

        private KickOffPositions kickOffPosition;

        public KickOff(Car car)
        {
            this.car = car;
            kickOffPosition = GetKickOffPosition(car.Position);
        }

        public Controller Step(float dt)
        {
            switch (kickOffPosition)
            {
                case KickOffPositions.Center:
                    return KickOffCenter(dt);

                case KickOffPositions.FrontCorner:
                    return KickOffFrontCorner(dt);

                default:
                    return new Controller();
            }
        }

        private Controller KickOffCenter(float dt)
        {
            Controller controller = new Controller();

            if (Math.Abs(car.Position.Y) > 3700) // Boost in a straight line.
            {
                controller.Boost = true;
            }
            else if (Math.Abs(car.Position.Y) > 1000) // Dodge forward.
            {
                if (dodge == null)
                    dodge = new Dodge(car, 0.2f, new Vector2(-1, 0));

                controller = dodge.Step(dt);

                controller.Boost = Math.Abs(car.Position.Y) > 3000; // Make sure we keep boosting during the first part of the dodge.
            }
            else if (Math.Abs(car.Position.Y) > 700)
            {
                dodge = null;
            }
            else // Final dodge when we are close to the ball.
            {
                if (dodge == null)
                    dodge = new Dodge(car, 0.2f, new Vector2(-1, 0));

                controller = dodge.Step(dt);
            }

            controller.Throttle = 1; // No reason not to hold throttle.

            return controller;
        }

        private Controller KickOffFrontCorner(float dt)
        {
            float distance = (new Vector2(car.Position.X, car.Position.Y) - new Vector2(0, 0)).Length();

            Controller controller = new Controller();

            if (car.Velocity.Length() < 1000)
            {
                controller.Boost = true;

                Vector3 local = Vector3.Transform(-car.Position, Quaternion.Inverse(car.Rotation));
                controller.Steer = MathUtility.Clip((float)Math.Atan2(local.Y, local.X) * 0.5f, -1, 1);
            }
            else if (distance > 2400)
            {
                if (dodge == null)
                    dodge = new Dodge(car, 0.13f, new Vector2(-1, 0));

                controller = dodge.Step(dt);

                controller.Boost = true;
            }
            else if (distance > 500)
            {
                Vector3 local = Vector3.Transform(-car.Position, Quaternion.Inverse(car.Rotation));
                controller.Steer = MathUtility.Clip((float)Math.Atan2(local.Y, local.X) * 10, -1, 1);

                controller.Boost = distance < 2000 && car.HasWheelContact;

                dodge = null;
            }
            else
            {
                if (dodge == null)
                {
                    Vector3 local = Vector3.Normalize(Vector3.Transform(-car.Position, Quaternion.Inverse(car.Rotation)));
                    dodge = new Dodge(car, 0.2f, new Vector2(-local.X, local.Y));
                }

                controller = dodge.Step(dt);
            }


            controller.Throttle = 1; // No reason not to hold throttle.

            return controller;
        }

        public static KickOffPositions GetKickOffPosition(Vector3 location)
        {
            location = Vector3.Abs(location);

            if ((location - new Vector3(1952, 2464, 17)).Length() < 10)
            {
                return KickOffPositions.FrontCorner;
            }
            else if ((location - new Vector3(256, 3840, 17)).Length() < 10)
            {
                return KickOffPositions.BackCorner;
            }
            else if ((location - new Vector3(0, 4608, 17)).Length() < 10)
            {
                return KickOffPositions.Center;
            }
            else
            {
                return KickOffPositions.Unknown;
            }
        }
    }
}
