using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using CornellBox.Helpers;

namespace CornellBox.Models
{
    class PathTracing
    {
        const int BRDF = 1;
        const double PDF = 1 / 2 * Math.PI;
        private int samples;

        public PathTracing(int samples)
        {
            Samples = samples;
        }

        public int Samples { get => samples; set => samples = value; }

        public Vector3 CalcColor(Ray ray, BoundingSphere bSphere)
        {
            if (MathHelper.Rand.NextDouble() < 0.2) return Vector3.Zero;

            Hitpoint hPoint = Hitpoint.FindClosestHitPoint(bSphere, ray);

            if (hPoint.Sphere == null) return Vector3.Zero;



            return Vector3.Zero;
        }

        private Vector3 RandomVector(Vector3 normal)
        {
            double x = MathHelper.Rand.NextDouble() * 2 - 1;
            double y = MathHelper.Rand.NextDouble() * 2 - 1;
            double z = MathHelper.Rand.NextDouble() * 2 - 1;

            Vector3 wr = Vector3.Normalize(new Vector3((float)x, (float)y, (float)z));

            return Vector3.Dot(wr, normal) >= 0 ? wr : -wr;
        }
    }
}
