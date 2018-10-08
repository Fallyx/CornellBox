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
            Normal = sphere != null ? Vector3.Normalize(Vector3.Subtract(position, sphere.Center)) : Vector3.Zero;
            Lambda = lambda;
        }

        public Vector3 Position { get => position; private set => position = value; }
        public Sphere Sphere { get => sphere; private set => sphere = value; }
        public Vector3 Normal { get => normal; private set => normal = value; }
        public double Lambda { get => lambda; private set => lambda = value; }

        /// <summary>
        /// Find the closest hitpoint in the sphere list
        /// </summary>
        /// <param name="spheres">List of spheres</param>
        /// <param name="ray">ray</param>
        /// <returns>Closest hitpoint</returns>
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

        /// <summary>
        /// Find closest hitpoint in the bounding sphere tree
        /// </summary>
        /// <param name="bSphere">Node of the bounding sphere tree</param>
        /// <param name="ray">ray</param>
        /// <returns>Closest hitpoint</returns>
        public static Hitpoint FindClosestHitPoint(BoundingSphere bSphere, Ray ray)
        {
            double noHit = double.MaxValue;
            Hitpoint hp = null;

            float[] mVars = Midnight.MidnightVars(bSphere, ray);

            if (mVars[1] * mVars[1] > 4 * mVars[0] * mVars[2])
            {
                double lambda = Midnight.CalcLambda(mVars[0], mVars[1], mVars[2]);

                if(lambda == noHit)
                {
                    return new Hitpoint(Vector3.Zero, null, noHit);
                }
                else if(bSphere.HasChildren)
                {
                    Hitpoint hpLeft = FindClosestHitPoint(bSphere.LeftChild, ray);
                    Hitpoint hpRight = FindClosestHitPoint(bSphere.RightChild, ray);
                    
                    if(hpLeft != null && hpRight != null)
                    {
                        hp = hpLeft.Lambda < hpRight.Lambda ? hpLeft : hpRight;
                    }
                    else if (hpLeft != null)
                    {
                        hp = hpLeft;
                    }
                    else if (hpRight != null)
                    {
                        hp = hpRight;
                    }
                }
                else
                {
                    Vector3 pos = new Vector3((float)(ray.Origin.X + lambda * ray.Direction.X), (float)(ray.Origin.Y + lambda * ray.Direction.Y), (float)(ray.Origin.Z + lambda * ray.Direction.Z));
                    hp = new Hitpoint(pos, bSphere.MSphere, lambda);
                }         
            }
            return hp;
        }
    }
}
