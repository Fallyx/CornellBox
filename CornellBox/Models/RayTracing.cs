using System;
using System.Collections.Generic;
using System.Numerics;
using CornellBox.Helpers;

namespace CornellBox.Models
{
    public class RayTracing
    {
        const int MAX_RECURSION = 1;

        private List<Sphere> spheres;
        private List<LightSource> lights;

        public RayTracing(List<Sphere> spheres, List<LightSource> lights)
        {
            Spheres = spheres;
            Lights = lights;
        }

        public List<Sphere> Spheres { get => spheres; private set => spheres = value; }
        public List<LightSource> Lights { get => lights; private set => lights = value; }

        public Vector3 CalcColor(Ray ray, int recursionCount = 0)
        {
            Hitpoint hPoint = Hitpoint.FindClosestHitPoint(Spheres, ray);

            if (hPoint.Sphere == null) return Vector3.Zero;
            
            Vector3 Ie = Vector3.Zero;
            Vector3 I = Vector3.Zero;
            Vector3 diff = Vector3.Zero;
            Vector3 phong = Vector3.Zero;
            Vector3 shadow = Vector3.Zero;
            Vector3 refl = Vector3.Zero;

            foreach (LightSource light in Lights)
            {
                diff = Diffuse(light, hPoint);
                if (recursionCount == 0) phong = Phong(light, hPoint, 40, ray);
                shadow = Shadow(light, hPoint, Spheres);

                I += (diff * shadow) + phong;
            }

            refl = Reflection(hPoint, ray, recursionCount);
            I += Ie + refl;

            return I;
        }

        private Vector3 Diffuse(LightSource light, Hitpoint h)
        {
            Vector3 diff = Vector3.Zero;
            Vector3 l = Vector3.Normalize(Vector3.Subtract(light.Position, h.Position));
            float nL = Vector3.Dot(h.Normal, l);

            if (nL >= 0)
            {
                Vector3 ilm = Vector3.Multiply(light.Color, h.Sphere.Material.Color);
                diff = Vector3.Multiply(ilm, nL);
            }

            return diff;
        }

        private Vector3 Phong(LightSource light, Hitpoint h, int phongExp, Ray ray)
        {
            Vector3 phong = Vector3.Zero;
            Vector3 l = Vector3.Subtract(light.Position, h.Position);

            float nL = Vector3.Dot(h.Normal, Vector3.Normalize(l));
            if (nL >= 0)
            {
                float s1 = Vector3.Dot(l, h.Normal);
                Vector3 s2 = Vector3.Multiply(s1, h.Normal);
                Vector3 s = Vector3.Subtract(l, s2);

                Vector3 r1 = Vector3.Multiply(2f, s);
                Vector3 r = Vector3.Subtract(l, r1);

                float rEH = Vector3.Dot(Vector3.Normalize(r), ray.Direction);
                rEH = (float)Math.Pow(rEH, phongExp);
                phong = Vector3.Multiply(light.Color, rEH);
            }

            return phong;
        }

        private Vector3 Shadow(LightSource light, Hitpoint h, List<Sphere> spheres)
        {
            Vector3 shadow = Vector3.One;
            Vector3 hl = Vector3.Subtract(light.Position, h.Position);

            Ray lightRay = new Ray(h.Position + h.Normal * 0.001f, Vector3.Normalize(hl));

            foreach (Sphere s in spheres)
            {
                float[] mVars = Midnight.MidnightVars(s, lightRay);

                double lambda = Midnight.CalcLambda(mVars[0], mVars[1], mVars[2]);
                if (lambda < hl.Length())
                {
                    shadow = new Vector3(light.Color.X * 0.2f, light.Color.Y * 0.2f, light.Color.Z * 0.2f);
                    break;
                }
            }

            return shadow;
        }

        private Vector3 Reflection(Hitpoint h, Ray ray, int recursionCount)
        {
            Vector3 reflection = Vector3.Zero;

            if (h.Sphere.Material.Reflection > 0 && recursionCount < MAX_RECURSION)
            {
                Vector3 EH = Vector3.Subtract(h.Position, ray.Origin);
                Vector3 r = Vector3.Normalize(Vector3.Reflect(Vector3.Normalize(EH), h.Normal));

                Ray reflectRay = new Ray(h.Position + h.Normal * 0.001f, r);

                float Krf1 = 1 - h.Sphere.Material.Reflection;
                float Krf2 = (float)Math.Pow(1 - Vector3.Dot(h.Normal, r), 5);
                float Krf = h.Sphere.Material.Reflection + Krf1 * Krf2;

                reflection = CalcColor(reflectRay, recursionCount + 1) * Krf;
            }

            return reflection;
        }
    }
}
