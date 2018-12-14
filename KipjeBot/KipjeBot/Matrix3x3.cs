using System;
using System.Numerics;

namespace KipjeBot
{
    public struct Matrix3x3
    {
        private Vector3 v0;
        private Vector3 v1;
        private Vector3 v2;

        #region Properties
        public float M00 { get { return v0.X; } set { v0.X = value; } }
        public float M10 { get { return v1.X; } set { v1.X = value; } }
        public float M20 { get { return v2.X; } set { v2.X = value; } }

        public float M01 { get { return v0.Y; } set { v0.Y = value; } }
        public float M11 { get { return v1.Y; } set { v1.Y = value; } }
        public float M21 { get { return v2.Y; } set { v2.Y = value; } }

        public float M02 { get { return v0.Z; } set { v0.Z = value; } }
        public float M12 { get { return v1.Z; } set { v1.Z = value; } }
        public float M22 { get { return v2.Z; } set { v2.Z = value; } }

        public static Matrix3x3 Identity
        {
            get
            {
                return new Matrix3x3(new float[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
            }
        }

        public Vector3 Forward
        {
            get
            {
                return new Vector3(M00, M10, M20);
            }

            private set
            {
                M00 = value.X;
                M10 = value.Y;
                M20 = value.Z;
            }
        }

        public Vector3 Left
        {
            get
            {
                return new Vector3(M01, M11, M21);
            }

            private set
            {
                M01 = value.X;
                M11 = value.Y;
                M21 = value.Z;
            }
        }

        public Vector3 Up
        {
            get
            {
                return new Vector3(M02, M12, M22);
            }

            private set
            {
                M02 = value.X;
                M12 = value.Y;
                M22 = value.Z;
            }
        }
        #endregion

        #region Operators
        public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 m = new Matrix3x3();

            m.v0 = left.v0 + right.v0;
            m.v1 = left.v1 + right.v1;
            m.v2 = left.v2 + right.v2;

            return m;
        }

        public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 m = new Matrix3x3();

            m.v0 = left.v0 - right.v0;
            m.v1 = left.v1 - right.v1;
            m.v2 = left.v2 - right.v2;

            return m;
        }

        public static Matrix3x3 operator *(Matrix3x3 left, float right)
        {
            Matrix3x3 m = new Matrix3x3();

            m.v0 = left.v0 * right;
            m.v1 = left.v1 * right;
            m.v2 = left.v2 * right;

            return m;
        }

        public static Matrix3x3 operator *(float left, Matrix3x3 right)
        {
            return right * left;
        }

        public static Matrix3x3 operator /(Matrix3x3 left, float right)
        {
            Matrix3x3 m = new Matrix3x3();

            m.v0 = left.v0 / right;
            m.v1 = left.v1 / right;
            m.v2 = left.v2 / right;

            return m;
        }

        public static Matrix3x3 operator /(float left, Matrix3x3 right)
        {
            return right / left;
        } 
        #endregion

        public Matrix3x3(float[,] components)
        {
            v0 = new Vector3();
            v1 = new Vector3();
            v2 = new Vector3();

            M00 = components[0, 0];
            M10 = components[1, 0];
            M20 = components[2, 0];

            M01 = components[0, 1];
            M11 = components[1, 1];
            M21 = components[2, 1];

            M02 = components[0, 2];
            M12 = components[1, 2];
            M22 = components[2, 2];
        }

        public Matrix3x3(Vector3 forward, Vector3 left, Vector3 up)
        {
            v0 = new Vector3();
            v1 = new Vector3();
            v2 = new Vector3();

            Forward = forward;
            Left = left;
            Up = up;
        }

        public static Matrix3x3 Dot(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 m = new Matrix3x3();

            m.M00 = Vector3.Dot(left.v0, new Vector3(right.v0.X, right.v1.X, right.v2.X));
            m.M10 = Vector3.Dot(left.v1, new Vector3(right.v0.X, right.v1.X, right.v2.X));
            m.M20 = Vector3.Dot(left.v2, new Vector3(right.v0.X, right.v1.X, right.v2.X));

            m.M01 = Vector3.Dot(left.v0, new Vector3(right.v0.Y, right.v1.Y, right.v2.Y));
            m.M11 = Vector3.Dot(left.v1, new Vector3(right.v0.Y, right.v1.Y, right.v2.Y));
            m.M21 = Vector3.Dot(left.v2, new Vector3(right.v0.Y, right.v1.Y, right.v2.Y));

            m.M02 = Vector3.Dot(left.v0, new Vector3(right.v0.Z, right.v1.Z, right.v2.Z));
            m.M12 = Vector3.Dot(left.v1, new Vector3(right.v0.Z, right.v1.Z, right.v2.Z));
            m.M22 = Vector3.Dot(left.v2, new Vector3(right.v0.Z, right.v1.Z, right.v2.Z));

            return m;
        }

        public static Vector3 Dot(Matrix3x3 left, Vector3 right)
        {
            Vector3 v = new Vector3();

            v.X = Vector3.Dot(left.v0, right);
            v.Y = Vector3.Dot(left.v1, right);
            v.Z = Vector3.Dot(left.v2, right);

            return v;
        }
    }
}
