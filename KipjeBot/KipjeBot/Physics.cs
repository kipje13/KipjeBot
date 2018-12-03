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

        /// <summary>
        /// Loads the map geometry for the physics dll to use.
        /// This method needs to be called before any other methods from this class can be used.
        /// </summary>
        /// <param name="path">The path to the geometry data.</param>
        public static void LoadMapGeometry(string path)
        {
            if (!Environment.Is64BitProcess)
                throw new PlatformNotSupportedException();

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

        /// <summary>
        /// Casts a ray against the map geometry.
        /// </summary>
        /// <param name="ray">The ray to be cast.</param>
        /// <param name="result">The RayCastHit struct to populate with the result of the raycast.</param>
        /// <returns>Returns True on a hit, False when no hit.</returns>
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

        /// <summary>
        /// Intersects a sphere with the map geometry.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="normal">A Vector3 that will contain the average normal of the intersecting triangles.</param>
        /// <returns>Returns True on a intersect, False when no intersect.</returns>
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
