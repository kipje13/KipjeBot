using System;
using System.Numerics;

using KipjeBot.Utility;

namespace KipjeBot.GameTickPacket
{
    public class FieldInfo
    {
        private BoostPad[] boostPads;
        private Goal[] goals;

        public BoostPad[] BoostPads { get { return (BoostPad[])boostPads.Clone(); } }
        public Goal[] Goals { get { return (Goal[])goals.Clone(); } }

        public FieldInfo() { }

        public FieldInfo(rlbot.flat.FieldInfo fieldInfo)
        {
            Update(fieldInfo);
        }

        /// <summary>
        /// Updates the fields of this object to reflect a new fieldinfo flatbuffers object.
        /// </summary>
        /// <param name="fieldInfo"></param>
        public void Update(rlbot.flat.FieldInfo fieldInfo)
        {
            boostPads = new BoostPad[fieldInfo.BoostPadsLength];

            for (int i = 0; i < boostPads.Length; i++)
            {
                boostPads[i] = new BoostPad(fieldInfo.BoostPads(i).Value);
            }

            goals = new Goal[fieldInfo.GoalsLength];

            for (int i = 0; i < goals.Length; i++)
            {
                goals[i] = new Goal(fieldInfo.Goals(i).Value);
            }
        }
    }

    public struct BoostPad
    {
        public Vector3 Position { get; private set; }
        public bool IsBigBoost { get; private set; }

        public BoostPad(rlbot.flat.BoostPad boostPad)
        {
            Position = boostPad.Location.Value.ToVector3();
            IsBigBoost = boostPad.IsFullBoost;
        }

        public BoostPad(Vector3 position, bool isBigBoost)
        {
            Position = position;
            IsBigBoost = isBigBoost;
        }
    }

    public struct Goal
    {
        public Vector3 Position { get; private set; }
        public int Team { get; private set; }

        public const float Width = 1785.51f;
        public const float Height = 642.775f;

        public Goal(rlbot.flat.GoalInfo goal)
        {
            Position = goal.Location.Value.ToVector3();
            Team = goal.TeamNum;
        }

        public Goal(Vector3 position, int team)
        {
            Position = position;
            Team = team;
        }
    }
}