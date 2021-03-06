﻿using System;
using System.Numerics;
using CornellBox.Models;

namespace CornellBox.Helpers
{
    class Midnight
    {
        public static float[] MidnightVars(BoundingSphere bSphere, Ray ray)
        {
            float[] mVars = new float[3];

            Vector3 sr = Vector3.Subtract(ray.Origin, bSphere.Center);
            mVars[0] = 1; // a
            mVars[1] = 2 * Vector3.Dot(sr, Vector3.Normalize(ray.Direction)); // b
            mVars[2] = (float)(sr.Length() * sr.Length() - bSphere.Radius * bSphere.Radius); // c

            return mVars;
        }

        public static double CalcLambda(float a, float b, float c)
        {
            float determin = b * b - 4 * a * c;

            if (determin < 0) return double.MaxValue;

            double lambda1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double lambda2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            if(lambda1 > 0 && lambda2 < 0)
            {
                return lambda1;
            }
            else if(lambda2 > 0 && lambda1 < 0)
            {
                return lambda2;
            }

            double shorterLambda = (float)Math.Min(lambda1, lambda2);

            return shorterLambda > 0 ? shorterLambda : double.MaxValue;
        }
    }
}
