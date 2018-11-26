using System;

namespace KipjeBot
{
    public class GameInfo
    {
        private int index;
        private int team;
        private string name;

        /// <summary>
        /// The time since the match started in seconds.
        /// </summary>
        public float Time { get; private set; } = 0;

        /// <summary>
        /// The time since the last update in seconds.
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// True when the match is running and not paused.
        /// </summary>
        public bool IsRoundActive { get; private set; }

        /// <summary>
        /// True when the kick-off countdown reaches 0, false when the clock starts running again.
        /// Note: True before kick-off countdown starts on the first kick-off.
        /// </summary>
        public bool IsKickOffPause { get; private set; }

        public Ball Ball { get; private set; } = new Ball();
        public Car[] Cars { get; private set; }

        public Car MyCar { get; private set; }

        /// <summary>
        /// Event that gets triggered once kick-off starts. (countdown reaches 0)
        /// </summary>
        public event EventHandler OnKickOffStarted;

        protected virtual void KickOffStarted(EventArgs e)
        {
            OnKickOffStarted?.Invoke(this, e);
        }

        public GameInfo(int index, int team, string name)
        {
            this.index = index;
            this.team = team;
            this.name = name;
        }

        /// <summary>
        /// Updates the GameInfo to reflect the new GameTickPacket.
        /// </summary>
        /// <param name="packet">The GameTickPacket object.</param>
        public void Update(rlbot.flat.GameTickPacket packet)
        {
            if (packet.GameInfo.HasValue)
            {
                DeltaTime = packet.GameInfo.Value.SecondsElapsed - Time;
                Time = packet.GameInfo.Value.SecondsElapsed;

                IsRoundActive = packet.GameInfo.Value.IsRoundActive;

                if (IsKickOffPause == false && packet.GameInfo.Value.IsKickoffPause == true)
                    KickOffStarted(new EventArgs());

                IsKickOffPause = packet.GameInfo.Value.IsKickoffPause;
            }

            if (packet.Ball.HasValue)
                Ball.Update(packet.Ball.Value);

            if (Cars == null || Cars.Length != packet.PlayersLength)
                Cars = new Car[packet.PlayersLength];

            for (int i = 0; i < packet.PlayersLength; i++)
            {
                if (Cars[i] == null)
                    Cars[i] = new Car();

                if (packet.Players(i).HasValue)
                    Cars[i].Update(packet.Players(i).Value, DeltaTime);
            }

            MyCar = Cars[index];
        }

        /// <summary>
        /// Updates the GameInfo to reflect the new GameTickPacket and RigidBodyTick.
        /// </summary>
        /// <param name="packet">The GameTickPacket object.</param>
        /// <param name="rigidBodyTick">The RigidBodyTick object.</param>
        public void Update(rlbot.flat.GameTickPacket packet, rlbot.flat.RigidBodyTick rigidBodyTick)
        {
            Update(packet);

            if (rigidBodyTick.Ball.HasValue)
                Ball.Update(rigidBodyTick.Ball.Value);

            for (int i = 0; i < packet.PlayersLength; i++)
            {
                if (rigidBodyTick.Players(i).HasValue)
                    Cars[i].Update(rigidBodyTick.Players(i).Value, DeltaTime);
            }
        }
    }
}
