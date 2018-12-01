using System.IO;
using System.Collections.Generic;
using System.Numerics;

using KipjeBot;
using System;

using System.Runtime.InteropServices;

namespace KipjeBot
{
    public static class Physics
    {
        [DllImport("Bvh.dll")]
        extern static void LoadMap(IntPtr triangles, int length);

        [DllImport("Bvh.dll")]
        extern static bool CastRay(IntPtr ray, IntPtr result);

        [DllImport("Bvh.dll")]
        extern static bool IntersectSphere(Vector3 center, float radius, IntPtr normal);

        public static void LoadMapGeometry(string path)
        {
            List<float> geometry = new List<float>();

            using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] stringValues = line.Split(' ', '\t');
                    float[] floatValues = new float[9];

                    for (int i = 0; i < 9; i++)
                    {
                        floatValues[i] = float.Parse(stringValues[i], System.Globalization.CultureInfo.InvariantCulture);
                    }

                    geometry.AddRange(floatValues);
                }
            }

            IntPtr ptr = Marshal.AllocHGlobal(geometry.Count * 4);
            Marshal.Copy(geometry.ToArray(), 0, ptr, geometry.Count);
            LoadMap(ptr, geometry.Count);
        }

        public static bool RayCast(Ray ray, out RayCastHit result)
        {
            IntPtr rayptr = Marshal.AllocHGlobal(24);
            IntPtr resultptr = Marshal.AllocHGlobal(24);

            bool hit = false;
            result = new RayCastHit();

            try
            {
                Marshal.StructureToPtr(ray, rayptr, true);
                Marshal.StructureToPtr(new RayCastHit(), resultptr, true);

                hit = CastRay(rayptr, resultptr);

                result = (RayCastHit)Marshal.PtrToStructure(resultptr, typeof(RayCastHit));
            }
            finally
            {
                Marshal.FreeHGlobal(rayptr);
                Marshal.FreeHGlobal(resultptr);
            }

            return hit;
        }

        public static bool IntersectSphere(Vector3 center, float radius, out Vector3 normal)
        {
            IntPtr normalptr = Marshal.AllocHGlobal(12);

            bool intersect = false;
            normal = new Vector3();

            try
            {
                Marshal.StructureToPtr(new Vector3(), normalptr, true);

                intersect = IntersectSphere(center, radius, normalptr);

                normal = (Vector3)Marshal.PtrToStructure(normalptr, typeof(Vector3));
            }
            finally
            {
                Marshal.FreeHGlobal(normalptr);
            }

            return intersect;
        }
    }

    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    public struct RayCastHit
    {
        public Vector3 Point;
        public Vector3 Normal;
    }
}
