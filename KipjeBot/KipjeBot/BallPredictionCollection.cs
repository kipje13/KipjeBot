using System;
using System.Numerics;

using KipjeBot.Utility;

namespace KipjeBot
{
    public class BallPredictionCollection
    {
        private rlbot.flat.BallPrediction flatprediction;

        public int Length { get; private set; } = 0;

        public Slice this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();

                return new Slice(flatprediction.Slices(index).Value);
            }
        }

        public void Update(rlbot.flat.BallPrediction prediction)
        {
            flatprediction = prediction;
            Length = prediction.SlicesLength;
        }

        public Slice[] ToArray(int count)
        {
            if (count < 0 || count > Length)
                throw new IndexOutOfRangeException();

            Slice[] slices = new Slice[count];

            for (int i = 0; i < count; i++)
            {
                slices[i] = new Slice(flatprediction.Slices(i).Value);
            }

            return slices;
        }

        public Slice[] ToArray()
        {
            return ToArray(Length);
        }
    }

    public struct Slice
    {
        public float Time { get; private set; }

        public Vector3 Position { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        public Slice(rlbot.flat.PredictionSlice slice)
        {
            Time = slice.GameSeconds;
            Position = slice.Physics.Value.Location.Value.ToVector3();
            Velocity = slice.Physics.Value.Velocity.Value.ToVector3();
            AngularVelocity = slice.Physics.Value.AngularVelocity.Value.ToVector3();
        }
    }
}
