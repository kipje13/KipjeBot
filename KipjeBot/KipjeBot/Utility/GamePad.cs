using RLBotDotNet;
using XInput.Wrapper;

namespace KipjeBot.Utility
{
    public static class GamePad
    {
        /// <summary>
        /// Generates Controller inputs from a gamepad input.
        /// Buttons are bound the same as the default Rocket League binds.
        /// </summary>
        /// <param name="gamepad"></param>
        /// <returns></returns>
        public static Controller GenerateControlsDefault(X.Gamepad gamepad)
        {
            Controller c = new Controller();

            c.Throttle = gamepad.RTrigger_N - gamepad.LTrigger_N;
            c.Steer = gamepad.LStick_N.X;
            c.Pitch = -gamepad.LStick_N.Y;

            if (gamepad.X_down)
                c.Roll = gamepad.LStick_N.X;
            else
                c.Yaw = gamepad.LStick_N.X;

            c.Jump = gamepad.A_down;
            c.Boost = gamepad.B_down;
            c.Handbrake = gamepad.X_down;

            return c;
        }

        /// <summary>
        /// Generates Controller inputs from a gamepad input.
        /// Buttons are bound to match my own Rocket League settings.
        /// </summary>
        /// <param name="gamepad"></param>
        /// <returns></returns>
        public static Controller GenerateControlsCustom(X.Gamepad gamepad)
        {
            Controller c = new Controller();

            c.Throttle = gamepad.RTrigger_N - gamepad.LTrigger_N;
            c.Steer = gamepad.LStick_N.X;
            c.Yaw = gamepad.LStick_N.X;
            c.Pitch = -gamepad.LStick_N.Y;
            c.Roll = gamepad.RStick_N.X;

            c.Jump = gamepad.A_down;
            c.Boost = gamepad.RBumper_down;
            c.Handbrake = gamepad.LBumper_down;

            return c;
        }
    }
}
