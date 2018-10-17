using System;
using System.Numerics;

namespace CornellBox.Models
{
    public class Ray
    {
        private Vector3 origin;
        private Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 Origin { get => origin; private set => origin = value; }
        public Vector3 Direction { get => direction; private set => direction = value; }

        /// <summary>
        /// Creates a ray from the eye
        /// </summary>
        /// <param name="eye">Position of eye</param>
        /// <param name="lookAt">Look at</param>
        /// <param name="FOV">Field of view</param>
        /// <param name="pixel">pixel in the [-1, 1] space</param>
        /// <returns></returns>
        public static Ray CreateEyeRay(Vector3 eye, Vector3 lookAt, double FOV, Vector2 pixel)
        {
            double alpha = FOV * Math.PI / 180.0;

            Vector3 f = Vector3.Normalize(Vector3.Subtract(lookAt, eye));
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, new Vector3(1, 0, 0))); // Up Vector should be (0,1,0), but (1,0,0) gives the right result
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));

            Vector3 d1 = f;
            Vector3 d2 = Vector3.Multiply(r, pixel.X * (float)Math.Tan(alpha / 2));
            Vector3 d3 = Vector3.Multiply(u, pixel.Y * (float)Math.Tan(alpha / 2));

            Vector3 d = Vector3.Add(d1, Vector3.Add(d2, d3));

            return new Ray(eye, Vector3.Normalize(d));
        }
    }
}
