using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Media;

using XInput.Wrapper;
using RLBotDotNet;
using RLBotDotNet.Renderer;
using RLBotDotNet.GameState;

using KipjeBot.Utility;

namespace KipjeBot
{
    public class KipjeBot : Bot
    {
        private GameInfo gameInfo;
        private BallPredictionCollection ballPrediction;
        private X.Gamepad gamePad;

        private bool playerControlled = true;
        private bool XPressed = false;

        public KipjeBot(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gameInfo = new GameInfo(botIndex, botTeam, botName);
            gameInfo.FieldInfo(GetFieldInfo());
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

            Vector3 target = Vector3.Zero;

            if (playerControlled)
            {
                controller = GamePad.GenerateControlsCustom(gamePad);


            }
            else
            {
                Vector3 inputs = RotationController.GetInputs(car, Quaternion.Identity, 0.016667f);

                controller.Roll = inputs.X;
                controller.Pitch = inputs.Y;
                controller.Yaw = inputs.Z;
            }

            return controller;
        }
    }
}
