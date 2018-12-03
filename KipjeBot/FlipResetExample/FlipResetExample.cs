using System;
using System.Numerics;
using System.Windows.Media;

using RLBotDotNet;
using RLBotDotNet.GameState;

using KipjeBot;
using KipjeBot.Actions;

namespace FlipResetExample
{
    public class FlipResetExample : Bot
    {
        GameInfo gameInfo;
        BallPredictionCollection ballPrediction = new BallPredictionCollection();

        private float timeout = 0;
        FlipReset flipReset = null;
        Recovery recovery = null;
        bool recoveryState = false;

        Random random = new Random();

        public FlipResetExample(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gameInfo = new GameInfo(botIndex, botTeam, botName);
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            gameInfo.Update(gameTickPacket, GetRigidBodyTick());
            ballPrediction.Update(GetBallPrediction());

            Controller controller = new Controller();

            if (timeout > 6)
            {
                double angle = random.NextDouble() * Math.PI * 2;

                GameState gamestate = new GameState();
                gamestate.BallState.PhysicsState.Location = new DesiredVector3(700 * (float)Math.Cos(angle), 700 * (float)Math.Sin(angle), 100);
                gamestate.BallState.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                gamestate.BallState.PhysicsState.Velocity = new DesiredVector3(300 * (float)Math.Cos(angle), 300 * (float)Math.Sin(angle), 1500);

                CarState carstate = new CarState();
                carstate.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Velocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Location = new DesiredVector3(0, 0, 17);
                carstate.PhysicsState.Rotation = new DesiredRotator(0, (float)(Math.PI / 2), 0);

                gamestate.SetCarState(index, carstate);

                SetGameState(gamestate);

                timeout = 0;
                flipReset = null;
                recovery = null;
                recoveryState = false;
            }

            Car car = gameInfo.Cars[index];

            Slice[] slices = ballPrediction.ToArray();

            if (timeout > 0.3)
            {
                if (flipReset == null)
                {
                    for (int i = 0; i < slices.Length; i++)
                    {
                        if (slices[i].Velocity.Z < 100)
                        {
                            Vector3 A = FlipReset.CalculateCourse(car, slices[i].Position, slices[i].Time - gameInfo.Time);

                            if (A.Length() < 800 && A.Length() > 700)
                            {
                                flipReset = new FlipReset(car, slices[i].Position, gameInfo.Time, slices[i].Time);
                            }
                        }
                    }
                }

                if (flipReset != null && !recoveryState)
                {
                    controller = flipReset.Step(gameInfo.Ball, 1f / 60f, gameInfo.Time);

                    Car c = new Car(car);
                    Vector3 lastpoint = c.Position;

                    for (int i = 0; i < ((flipReset.ArrivalTime - gameInfo.Time) / (5f / 60f)); i++)
                    {
                        c.Simulate(new Controller(), 5f / 60f);

                        Renderer.DrawLine3D(Colors.White, c.Position, lastpoint);
                        lastpoint = c.Position;
                    }

                    if (flipReset.Finished)
                    {
                        controller.Roll = 0;
                        controller.Pitch = -1;
                        controller.Yaw = 0;

                        controller.Jump = true;
                        recoveryState = true;
                        recovery = new Recovery(car);
                    }
                }
                else if (recovery != null)
                {
                    controller = recovery.Step(1f / 60f);
                }
            }

            timeout += gameInfo.DeltaTime;

            return controller;
        }
    }
}
