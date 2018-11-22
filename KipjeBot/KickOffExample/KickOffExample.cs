using System;
using System.Numerics;

using RLBotDotNet;
using RLBotDotNet.GameState;

using KipjeBot;
using KipjeBot.Actions;

namespace KickOffExample
{
    public class KickOffExample : Bot
    {
        private GameInfo gameInfo;

        private float timeout = 0;

        private KickOff kickoff;

        private KickOffStruct kickOffCenter = new KickOffStruct(new Vector2(0, 4608), -0.5 * Math.PI);
        private KickOffStruct kickOffBackCorner = new KickOffStruct(new Vector2(256, 3840), -0.5 * Math.PI);
        private KickOffStruct kickOffFrontCorner = new KickOffStruct(new Vector2(1952, 2464), -0.75 * Math.PI);

        public KickOffExample(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gameInfo = new GameInfo(botIndex, botTeam, botName);
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            gameInfo.Update(gameTickPacket);

            if (timeout > 5)
            {
                KickOffStruct k = kickOffFrontCorner;

                GameState gamestate = new GameState();
                gamestate.BallState.PhysicsState.Location = new DesiredVector3(0, 0, 100);
                gamestate.BallState.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                gamestate.BallState.PhysicsState.Velocity = new DesiredVector3(0, 0, 0);

                CarState carstate = new CarState();
                carstate.PhysicsState.AngularVelocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Velocity = new DesiredVector3(0, 0, 0);
                carstate.PhysicsState.Location = new DesiredVector3(new Vector3(k.Position, 17));
                carstate.PhysicsState.Rotation = new DesiredRotator(0, k.Yaw, 0);
                carstate.Boost = 33;

                gamestate.SetCarState(index, carstate);

                SetGameState(gamestate);

                timeout = 0;
                kickoff = null;
            }

            timeout += gameInfo.DeltaTime;

            Controller controller = new Controller();

            if (timeout > 2)
            {
                if (kickoff == null)
                    kickoff = new KickOff(gameInfo.Cars[index]);

                controller = kickoff.Step(0.0166667f);
            }

            return controller;
        }
    }

    struct KickOffStruct
    {
        public Vector2 Position;
        public float Yaw;

        public KickOffStruct(Vector2 position, double yaw)
        {
            Position = position;
            Yaw = (float)yaw;
        }
    }
}
