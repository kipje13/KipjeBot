using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Media;

using XInput.Wrapper;
using RLBotDotNet;
using RLBotDotNet.Renderer;
using RLBotDotNet.GameState;

using KipjeBot.GameTickPacket;
using KipjeBot.Utility;

namespace KipjeBot
{
    public class KipjeBot : Bot
    {
        private GameInfo gameInfo;
        private FieldInfo fieldInfo;
        private BallPredictionCollection ballPrediction;
        private X.Gamepad gamePad;

        private bool playerControlled = true;
        private bool XPressed = false;

        public KipjeBot(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gameInfo = new GameInfo(botIndex, botTeam, botName);
            fieldInfo = new FieldInfo(GetFieldInfo());
            ballPrediction = new BallPredictionCollection();

            gamePad = X.Gamepad_1;
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            gameInfo.Update(gameTickPacket, GetRigidBodyTick());
            ballPrediction.Update(GetBallPrediction());
            gamePad.Update();

            if (gamePad.X_down && !XPressed)
            {
                playerControlled = !playerControlled;
                XPressed = true;
            }

            if (!gamePad.X_down)
            {
                XPressed = false;
            }

            Controller controller = new Controller();

            Car car = gameInfo.Cars[index];
            Ball ball = gameInfo.Ball;

            if (playerControlled)
            {
                controller = GamePad.GenerateControlsCustom(gamePad);
            }
            else
            {
                Quaternion target = MathUtility.LookAt(new Vector3(car.Velocity.X, car.Velocity.Y, 0));

                Renderer.DrawLine3D(Colors.Red, car.Position, car.Position + Vector3.Transform(Vector3.UnitX, target) * 100);

                Vector3 inputs = RotationController.GetInputs(car, target, 0.016667f);

                controller.Roll = inputs.X;
                controller.Pitch = inputs.Y;
                controller.Yaw = inputs.Z;
            }

            return controller;
        }
    }
}
