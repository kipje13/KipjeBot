using RLBotDotNet;
using XInput.Wrapper;

using KipjeBot.Utility;

namespace ControllerPassthrough
{
    /// <summary>
    /// Example on how to use controller input to control a bot.
    /// Uses XInput so only supports xbox controllers without using extra drivers.
    /// </summary>
    public class ControllerPassthrough : Bot
    {
        private X.Gamepad gamePad;

        public ControllerPassthrough(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex)
        {
            gamePad = X.Gamepad_1;
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            gamePad.Update();
            Controller controller = GamePad.GenerateControlsCustom(gamePad);

            return controller;
        }
    }
}
