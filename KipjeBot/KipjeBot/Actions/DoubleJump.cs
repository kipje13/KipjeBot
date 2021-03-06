﻿using RLBotDotNet;

namespace KipjeBot.Actions
{
    /// <summary>
    /// Double jump as fast as possible.
    /// </summary>
    public class DoubleJump
    {
        public bool Finished { get; private set; } = false;

        private int tick = 0;

        public Controller Step()
        {
            Controller c = new Controller();

            if (tick < 3 || tick > 5)
                c.Jump = true;

            if (tick > 7)
                Finished = true;

            tick++;

            return c;
        }
    }
}
