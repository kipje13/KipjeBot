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

        public Aerial aerial = null;

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

            Slice[] slices = ballPrediction.ToArray();

            for (int i = 1; i < slices.Length; i++)
            {
                Renderer.DrawLine3D(Colors.Magenta, slices[i - 1].Position, slices[i].Position);
            }

            if (playerControlled)
            {
                aerial = null;
                controller = GamePad.GenerateControlsCustom(gamePad);
            }
            else
            {
                if (aerial == null)
                {
                    for (int i = 0; i < slices.Length; i++)
                    {
                        float B_avg = Aerial.CalculateCourse(car, slices[i].Position, slices[i].Time - gameInfo.Time).Length();

                        if (B_avg > 900 && B_avg < 970)
                        {
                            aerial = new Aerial(car, slices[i].Position, slices[i].Time - gameInfo.Time);
                            break;
                        }
                    }
                }
                else
                {
                    controller = aerial.Step(0.016667f);
                    Renderer.DrawLine3D(Colors.Red, car.Position, aerial.Target);

                    if (aerial.Finished)
                        aerial = null;
                }
            }

            return controller;
        }
    }
}
