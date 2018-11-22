using System;
using System.Numerics;
using System.Windows.Media;

using RLBotDotNet;
using RLBotDotNet.GameState;
using RLBotDotNet.Renderer;

using KipjeBot;
using KipjeBot.Actions;
using KipjeBot.GameTickPacket;

namespace AerialExample
{
    public class AerialExample : Bot
    {
        private GameInfo gameInfo;
        private FieldInfo fieldInfo;
        private BallPredictionCollection ballPrediction;

        public Aerial aerial = null;

        private float timeout = 0;
        private Random random = new Random();

        public AerialExample(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gameInfo = new GameInfo(botIndex, botTeam, botName);
            fieldInfo = new FieldInfo(GetFieldInfo());
            ballPrediction = new BallPredictionCollection();
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            gameInfo.Update(gameTickPacket, GetRigidBodyTick());
            ballPrediction.Update(GetBallPrediction());

            if (timeout > 5)
            {
                GameState gamestate = new GameState();
                gamestate.BallState.PhysicsState.Location = new DesiredVector3(RandomFloat(-3000, 3000), 2000, 100);
                gamestate.BallState.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                gamestate.BallState.PhysicsState.Velocity = new DesiredVector3(RandomFloat(2000, 2000), RandomFloat(1000, 2000), RandomFloat(1500, 1600));

                CarState carstate = new CarState();
                carstate.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Velocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Location = new DesiredVector3(0, 0, 17);
                carstate.PhysicsState.Rotation = new DesiredRotator(0, (float)(Math.PI / 2), 0);

                gamestate.SetCarState(index, carstate);

                SetGameState(gamestate);

                timeout = 0;
                aerial = null;
            }

            timeout += gameInfo.DeltaTime;

            Controller controller = new Controller();

            Car car = gameInfo.Cars[index];
            Ball ball = gameInfo.Ball;

            Slice[] slices = ballPrediction.ToArray();

            if (timeout > 0.3)
            {
                if (aerial == null)
                {
                    for (int i = 0; i < slices.Length; i++)
                    {
                        float B_avg = Aerial.CalculateCourse(car, slices[i].Position, slices[i].Time - gameInfo.Time).Length();

                        if (B_avg > 900 && B_avg < 970)
                        {
                            aerial = new Aerial(car, slices[i].Position, gameInfo.Time, slices[i].Time);
                            break;
                        }
                    }
                }
                else
                {
                    controller = aerial.Step(ball, 0.016667f, gameInfo.Time);
                    Renderer.DrawLine3D(Colors.Red, car.Position, aerial.Target);

                    if (aerial.Finished)
                    {
                        aerial = null;
                    }
                    else
                    {
                        for (int i = 0; i < slices.Length; i++)
                        {
                            if (Math.Abs(slices[i].Time - aerial.ArrivalTime) < 0.02)
                            {
                                if ((aerial.Target - slices[i].Position).Length() > 40)
                                    aerial = null;
                                break;
                            }
                        }
                    }
                }
            }

            return controller;
        }

        private float RandomFloat(float min, float max)
        {
            return (float)(min + random.NextDouble() * (max - min));
        }
    }
}
