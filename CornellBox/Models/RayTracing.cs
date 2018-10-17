using System;
using System.Collections.Generic;
using System.Numerics;
using CornellBox.Helpers;

namespace CornellBox.Models
{
    public class RayTracing
    {
        const int MAX_RECURSION = 1;

        private List<LightSource> lights;

        public RayTracing(List<LightSource> lights)
        {
            Lights = lights;
        }

        public List<LightSource> Lights { get => lights; private set => lights = value; }

        /// <summary>
        /// Calculate the color at the ray hitpoint. Iterates through a bounding sphere tree
        /// </summary>
        /// <param name="ray">ray</param>
        /// <param name="bSphere">Bounding sphere node</param>
        /// <param name="recursionCount">current recursion count</param>
        /// <returns>Color vector</returns>
        public Vector3 CalcColor(Ray ray, BoundingSphere bSphere, int recursionCount = 0)
        {
            Hitpoint hPoint = Hitpoint.FindClosestHitPoint(bSphere, ray);

            if (hPoint.Sphere == null) return Vector3.Zero;

            MaterialSphere mSphere = hPoint.Sphere as MaterialSphere;

            Vector3 Ie = Vector3.Zero;
            Vector3 I = Vector3.Zero;
            Vector3 diff = Vector3.Zero;
            Vector3 phong = Vector3.Zero;
            Vector3 shadow = Vector3.One;
            Vector3 refl = Vector3.Zero;

            foreach (LightSource light in Lights)
            {
                if (mSphere.HasDiffuse) diff = Diffuse(light, hPoint);
                else if (mSphere.Material.HasImg) diff = mSphere.Material.SphericalProjection(hPoint.Normal);
                else diff = mSphere.Material.Color;
                if (recursionCount == 0 && mSphere.HasPhong) phong = Phong(light, hPoint, 40, ray);
                if (mSphere.HasShadow && light.Radius == 0) shadow = Shadow(light, hPoint, bSphere);
                else if (mSphere.HasShadow) shadow = SoftShadow(light, hPoint, bSphere, 10);

                I += (diff * shadow) + phong;
            }

            if (mSphere.HasReflection) refl = Reflection(hPoint, ray, bSphere, recursionCount);
            I += Ie + refl;

            return I;
        }

        /// <summary>
        /// Calculate the diffuse
        /// </summary>
        /// <param name="light">Lightsoruce</param>
        /// <param name="h">Hitpoint</param>
        /// <returns>Diffuse</returns>
        private Vector3 Diffuse(LightSource light, Hitpoint h)
        {
            Vector3 diff = Vector3.Zero;
            Vector3 l = Vector3.Normalize(Vector3.Subtract(light.Position, h.Position));
            float nL = Vector3.Dot(h.Normal, l);

            if (nL >= 0)
            {
                MaterialSphere mSphere = h.Sphere as MaterialSphere;

                Vector3 color;
                if (mSphere.Material.HasImg)
                    color = mSphere.Material.SphericalProjection(h.Normal);
                else
                    color = mSphere.Material.Color;

                Vector3 ilm = Vector3.Multiply(light.Color, color);
                diff = Vector3.Multiply(ilm, nL);
            }

            return diff;
        }

        /// <summary>
        /// Calculates the phong
        /// </summary>
        /// <param name="light">Lightsource</param>
        /// <param name="h">hitpoint</param>
        /// <param name="phongExp">phong exponent</param>
        /// <param name="ray">ray</param>
        /// <returns>phong</returns>
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

        /// <summary>
        /// Calculates the shadow. Iterates through a bounding sphere tree
        /// </summary>
        /// <param name="light">Lightsource</param>
        /// <param name="h">Hitpoint</param>
        /// <param name="bSphere">Bounding sphere node</param>
        /// <returns>Shadow</returns>
        private Vector3 Shadow(LightSource light, Hitpoint h, BoundingSphere bSphere)
        {
            Vector3 shadow = Vector3.One;
            Vector3 hl = Vector3.Subtract(light.Position, h.Position);

            Ray lightRay = new Ray(h.Position + h.Normal * 0.001f, Vector3.Normalize(hl));
            Hitpoint shadowHitpoint = Hitpoint.FindClosestHitPoint(bSphere, lightRay);

            if (shadowHitpoint.Sphere == null) return shadow;

            if (shadowHitpoint.Lambda < hl.Length())
            {
                shadow = new Vector3(light.Color.X * 0.2f, light.Color.Y * 0.2f, light.Color.Z * 0.2f);
            }

            return shadow;
        }

        private Vector3 SoftShadow(LightSource light, Hitpoint h, BoundingSphere bSphere, int lightSamples)
        {
            Vector3 shadow = Vector3.One;
            int nrInShadow = 0;

            Vector3 Nl = Vector3.Normalize(h.Position - light.Position);
            Vector3 Nx = Nl == new Vector3(0, 1, 0) ? Vector3.Cross(Nl, new Vector3(1, 0, 0)) : Vector3.Cross(Nl, new Vector3(0, 1, 0));
            Nx = Vector3.Normalize(Nx);
            Vector3 Ny = Vector3.Cross(Nl, Nx);

            for (int i = 0; i < lightSamples; i++)
            {            
                double r = MathHelper.Rand.NextDouble();
                double t = MathHelper.Rand.NextDouble() * 2 * Math.PI;

                double x = Math.Sqrt(r) * Math.Sin(t);
                double y = Math.Sqrt(r) * Math.Cos(t);

                Vector3 randPoint = light.Position + (float)light.Radius * Nx * (float)x + Ny * (float)y * (float)light.Radius;
                Vector3 hl = randPoint - h.Position;

                Ray randomLightRay = new Ray(h.Position + h.Normal * 0.001f, Vector3.Normalize(hl));

                Hitpoint shadowHitpoint = Hitpoint.FindClosestHitPoint(bSphere, randomLightRay);

                if (shadowHitpoint == null || shadowHitpoint.Sphere == null) return shadow;

                if (shadowHitpoint.Lambda < hl.Length())
                {
                    nrInShadow++;
                }
            }

            double sMultiplier = nrInShadow != 0 ? Helpers.MathHelper.RangeConverter(0, lightSamples, 0, 0.8, nrInShadow) : 0;
            float sM = 1 - (float)sMultiplier;

            shadow = new Vector3(light.Color.X * sM, light.Color.Y * sM, light.Color.Z * sM);

            return shadow;
        }

        /// <summary>
        /// Calculate the reflection. Bounding sphere method
        /// </summary>
        /// <param name="h">Hitpoint</param>
        /// <param name="ray">Ray</param>
        /// <param name="bSphere">Bounding sphere node</param>
        /// <param name="recursionCount">current recursion Count</param>
        /// <returns>Reflection</returns>
        private Vector3 Reflection(Hitpoint h, Ray ray, BoundingSphere bSphere, int recursionCount)
        {
            Vector3 reflection = Vector3.Zero;
            MaterialSphere mSphere = h.Sphere as MaterialSphere;

            if (mSphere.Material.Reflection > 0 && recursionCount < MAX_RECURSION)
            {
                Vector3 EH = Vector3.Subtract(h.Position, ray.Origin);
                Vector3 r = Vector3.Normalize(Vector3.Reflect(Vector3.Normalize(EH), h.Normal));

                Ray reflectRay = new Ray(h.Position + h.Normal * 0.001f, r);

                float Krf1 = 1 - mSphere.Material.Reflection;
                float Krf2 = (float)Math.Pow(1 - Vector3.Dot(h.Normal, r), 5);
                float Krf = mSphere.Material.Reflection + Krf1 * Krf2;

                reflection = CalcColor(reflectRay, bSphere, recursionCount + 1) * Krf;
            }

            return reflection;
        }
    }
}
