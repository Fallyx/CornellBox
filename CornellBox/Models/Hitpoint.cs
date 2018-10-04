using System;
using System.Numerics;
using System.Collections.Generic;
using CornellBox.Helpers;

namespace CornellBox.Models
{
    public class Hitpoint
    {
        private Vector3 position;
        private Sphere sphere;
        private Vector3 normal;
        private double lambda;

        public Hitpoint(Vector3 position, Sphere sphere, double lambda)
        {
            Position = position;
            Sphere = sphere;
            Normal = Vector3.Normalize(Vector3.Subtract(position, sphere.Center));
            Lambda = lambda;
        }

        public Vector3 Position { get => position; private set => position = value; }
        public Sphere Sphere { get => sphere; private set => sphere = value; }
        public Vector3 Normal { get => normal; private set => normal = value; }
        public double Lambda { get => lambda; private set => lambda = value; }

        public static Hitpoint FindClosestHitPoint(List<Sphere> spheres, Ray ray)
        {
            double closestHit = double.MaxValue;
            Sphere closestSphere = null;

            foreach (Sphere sphere in spheres)
            {
                float[] mVars = Midnight.MidnightVars(sphere, ray);

                if (mVars[1] * mVars[1] > 4 * mVars[0] * mVars[2])
                {
                    double lambda = Midnight.CalcLambda(mVars[0], mVars[1], mVars[2]);

                    closestHit = Math.Min(closestHit, lambda);
                    if(closestHit == lambda)
                    {
                        closestSphere = sphere;
                    }
                }
            }

            Vector3 pos = new Vector3((float)(ray.Origin.X + closestHit * ray.Direction.X), (float)(ray.Origin.Y + closestHit * ray.Direction.Y), (float)(ray.Origin.Z + closestHit * ray.Direction.Z));
            return new Hitpoint(pos, closestSphere, closestHit);
        }

        public static Hitpoint FindClosestHitPoint(BoundingSphere bSphere, Ray ray)
        {
            double closestHit = double.MaxValue;
            BoundingSphere closestSphere = null;
            Hitpoint hp = null;

            float[] mVars = Midnight.MidnightVars(bSphere, ray);

            if(mVars[1] * mVars[1] > 4 * mVars[0] * mVars[2])
            {
                double lambda = Midnight.CalcLambda(mVars[0], mVars[1], mVars[2]);

                closestHit = Math.Min(closestHit, lambda);
                if(closestHit == lambda)
                {
                    closestSphere = bSphere;
                }
            }

            if(closestSphere != null && bSphere.HasChildren)
            {
                Hitpoint hpLeft = FindClosestHitPoint(bSphere.LeftChild, ray);
                Hitpoint hpRight = FindClosestHitPoint(bSphere.RightChild, ray);

                hp = hpLeft.Lambda < hpRight.Lambda ? hpLeft : hpRight;
            }

            return hp;
        }
    }
}
