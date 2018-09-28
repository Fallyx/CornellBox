﻿using System;
using System.Numerics;

namespace CornellBox.Models
{
    public class Ray
    {
        private Vector3 origin;
        private Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3 Origin { get => origin; set => origin = value; }
        public Vector3 Direction { get => direction; set => direction = value; }

        public static Ray CreateEyeRay(Vector3 Eye, Vector3 LookAt, double FOV, Vector2 Pixel)
        {
            double alpha = FOV * Math.PI / 180.0;

            Vector3 f = Vector3.Normalize(Vector3.Subtract(LookAt, Eye));
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, new Vector3(1, 0, 0))); // Up Vector should be (0,1,0), but (1,0,0) gives the right result
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));

            Vector3 d1 = f;
            Vector3 d2 = Vector3.Multiply(r, Pixel.X * (float)Math.Tan(alpha / 2));
            Vector3 d3 = Vector3.Multiply(u, Pixel.Y * (float)Math.Tan(alpha / 2));

            Vector3 d = Vector3.Add(d1, Vector3.Add(d2, d3));

            return new Ray(Eye, Vector3.Normalize(d));
        }
    }
}