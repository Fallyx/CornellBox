using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CornellBox.Helpers
{
    class Midnight
    {
        public float[] MidnightVars(Sphere sphere, Ray ray)
        {
            float[] mVars = new float[3];

            Vector3 sr = Vector3.Subtract(ray.Origin, sphere.Center);
            mVars[0] = 1; // a
            mVars[1] = 2 * Vector3.Dot(sr, Vector3.Normalize(ray.Direction)); // b
            mVars[2] = (float)(sr.Length() * sr.Length() - sphere.Radius * sphere.Radius); // c

            return mVars;
        }

        private double CalcLambda(float a, float b, float c)
        {
            float determin = b * b - 4 * a * c;

            if (determin < 0) return double.MaxValue;

            double lambda1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double lambda2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            double shorterLambda = (float)Math.Min(lambda1, lambda2);

            return shorterLambda > 0 ? shorterLambda : double.MaxValue;
        }
    }
}
