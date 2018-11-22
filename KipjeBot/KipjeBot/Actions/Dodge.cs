using System.Numerics;

using RLBotDotNet;

namespace KipjeBot.Actions
{
    public class Dodge
    {
        public float Time;
        public Vector2 Direction;

        public bool Finished { get; private set; } = false;

        private Car car;
        private float timeElapsed = 0;

        public Dodge(Car car, float time, Vector2 direction)
        {
            Time = time;
            Direction = direction;
            this.car = car;
        }

        public Controller Step(float dt)
        {
            Controller c = new Controller();

            if (car.HasWheelContact)
            {
                c.Jump = true;
            }

            if (timeElapsed > Time)
            {
                c.Jump = true;
                c.Pitch = Direction.X;
                c.Yaw = Direction.Y;
            }

            if (car.DoubleJumped)
            {
                Finished = true;
            }

            timeElapsed += dt;
            return c;
        }
    }
}
