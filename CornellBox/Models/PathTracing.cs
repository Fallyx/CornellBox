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
        const double PDF = 1 / (2 * Math.PI);
        const int MAX_RECURSION = 4;

        public Vector3 CalcColor(Ray ray, BoundingSphere bSphere)
        {
            Vector3 color = MonteCarloCalcColor(ray, bSphere);
            //color *= (float)(1 / (1 - 0.2));

            return color;
        }

        private Vector3 MonteCarloCalcColor(Ray ray, BoundingSphere bSphere, int recursion_count = 0)
        {
            if (recursion_count > MAX_RECURSION) return Vector3.Zero;
            //if (MathHelper.Rand.NextDouble() < 0.2) return Vector3.Zero;

            Hitpoint hPoint = Hitpoint.FindClosestHitPoint(bSphere, ray);

            if (hPoint == null || hPoint.Sphere == null) return Vector3.Zero;

            Vector3 color = Vector3.Zero;
            MaterialSphere mSphere = hPoint.Sphere as MaterialSphere;
            Vector3 mColor = mSphere.Material.Color;
            Vector3 emission = mSphere.Material.Emission * mColor;

            Vector3 wr = RandomVector(hPoint.Normal);
            Ray randomRay = new Ray(hPoint.Position, wr);

            color += (MonteCarloCalcColor(randomRay, bSphere, recursion_count + 1) * Vector3.Dot(wr, hPoint.Normal) * (float)BRDF * mColor)/ (float)PDF;

            return emission + color;
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
