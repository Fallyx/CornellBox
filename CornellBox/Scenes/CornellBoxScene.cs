using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Numerics;
using CornellBox.Models;
using CornellBox.Helpers;

namespace CornellBox.Scenes
{
    public class CornellBoxScene
    {
        public static List<Sphere> InitSphere()
        {
            List<Sphere> spheres = new List<Sphere>();

            Sphere aRed = new MaterialSphere(new Vector3(-1001, 0, 0), 1000, new Material(new Vector3(0, 0, 1)));
            Sphere bBlue = new MaterialSphere(new Vector3(1001, 0, 0), 1000, new Material(new Vector3(1, 0, 0)));
            Sphere cWhite = new MaterialSphere(new Vector3(0, 0, 1001), 1000, new Material(new Vector3(1, 1, 1)));
            Sphere dWhite = new MaterialSphere(new Vector3(0, -1001, 0), 1000, new Material(new Vector3(1, 1, 1)));
            Sphere eWhite = new MaterialSphere(new Vector3(0, 1001, 0), 1000, new Material(new Vector3(1, 1, 1)));
            //Sphere fYellow = new MaterialSphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3, new Material(new Vector3(0, 1, 1), 0.5f));
            //Sphere gCyan = new MaterialSphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6, new Material(new Vector3(1, 1, 0.88f), 0.5f));
            Sphere fYellow = new MaterialSphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3, new Material(new Vector3(0, 1, 1), 0.5f, Material.BrickImage()));
            Sphere gCyan = new MaterialSphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6, new Material(new Vector3(1, 1, 0.88f), imgPath: Material.EarthImage(), offset: new Vector2(0.5f, 0)),false, false,false, false);

            spheres.Add(aRed);
            spheres.Add(bBlue);
            spheres.Add(cWhite);
            spheres.Add(dWhite);
            spheres.Add(eWhite);
            spheres.Add(fYellow);
            spheres.Add(gCyan);

            return spheres;
        }

        public static List<LightSource> InitLight(bool singleLight, bool hasRadius)
        {
            List<LightSource> lights = new List<LightSource>();
            LightSource WhiteLight;

            double radius = hasRadius ? 0.3 : 0;


            if (singleLight)
            {
                WhiteLight = new LightSource(new Vector3(0, -0.9f, 0), new Vector3(1f, 1f, 1f), radius);
                lights.Add(WhiteLight);

                return lights;
            }

            WhiteLight = new LightSource(new Vector3(0, -0.9f, 0), new Vector3(0.5f, 0.5f, 0.5f), radius);
            LightSource MagentaLight = new LightSource(new Vector3(-0.8f, -0.9f, 0), new Vector3(0.5f, 0f, 0.5f), radius);
            LightSource YellowLight = new LightSource(new Vector3(0.8f, -0.9f, 0), new Vector3(0.5f, 0.5f, 0f), radius);

            lights.Add(WhiteLight);
            lights.Add(MagentaLight);
            lights.Add(YellowLight);

            return lights;
        }
        
        public static byte[] PixelArray(int imgHeight, int imgWidth, int stride, List<Sphere> spheres, List<LightSource> lights, Vector3 Eye, Vector3 LookAt, double FOV, int AASamples = 0)
        {
            byte[] pixels = new byte[imgHeight * imgWidth * stride];
            int index = 0;
            Ray eyeRay;

            RayTracing rayTracing = new RayTracing(spheres, lights);
            Random r = new Random();

            for (int col = 0; col < imgWidth; col++)
            {
                for (int row = 0; row < imgHeight; row++)
                {
                    Vector3 color = Vector3.Zero;
                    Vector3 finalColor = Vector3.Zero;

                    if(AASamples != 0)
                    {
                        for (int i = 0; i < AASamples; i++)
                        {
                            eyeRay = Ray.CreateEyeRay(Eye, LookAt, FOV, GaussDomainPixels(col, row, imgWidth, imgHeight));
                            color += rayTracing.CalcColor(eyeRay);
                        }

                        finalColor = new Vector3(color.X / (float)AASamples, color.Y / (float)AASamples, color.Z / (float)AASamples);
                    }
                    else
                    {
                        eyeRay = Ray.CreateEyeRay(Eye, LookAt, FOV, DomainPixels(col, row, imgWidth, imgHeight));
                        color = rayTracing.CalcColor(eyeRay);

                        finalColor = color;
                    }

                    Color c = Color.FromScRgb(1, finalColor.Z, finalColor.Y, finalColor.X);

                    pixels[index++] = c.B;
                    pixels[index++] = c.G;
                    pixels[index++] = c.R;

                    index++; // Skip Alpha
                }
            }

            return pixels;
        }

        public static byte[] PixelArray(int imgHeight, int imgWidth, int stride, BoundingSphere bSphere, List<LightSource> lights, Vector3 Eye, Vector3 LookAt, double FOV, int AASamples = 0)
        {
            byte[] pixels = new byte[imgHeight * imgWidth * stride];
            int index = 0;
            Ray eyeRay;

            RayTracing rayTracing = new RayTracing(lights);

            for (int col = 0; col < imgWidth; col++)
            {
                for (int row = 0; row < imgHeight; row++)
                {
                    Vector3 color = Vector3.Zero;
                    Vector3 finalColor = Vector3.Zero;

                    if (AASamples != 0)
                    {
                        for (int i = 0; i < AASamples; i++)
                        {
                            eyeRay = Ray.CreateEyeRay(Eye, LookAt, FOV, GaussDomainPixels(col, row, imgWidth, imgHeight));
                            color += rayTracing.CalcColor(eyeRay, bSphere);
                        }

                        finalColor = new Vector3(color.X / (float)AASamples, color.Y / (float)AASamples, color.Z / (float)AASamples);
                    }
                    else
                    {
                        eyeRay = Ray.CreateEyeRay(Eye, LookAt, FOV, DomainPixels(col, row, imgWidth, imgHeight));
                        color = rayTracing.CalcColor(eyeRay, bSphere);

                        finalColor = color;
                    }
                    
                    Color c = Color.FromScRgb(1, finalColor.Z, finalColor.Y, finalColor.X);

                    pixels[index++] = c.B;
                    pixels[index++] = c.G;
                    pixels[index++] = c.R;

                    index++; // Skip Alpha
                }
            }

            return pixels;
        }

        private static Vector2 GaussDomainPixels(int col, int row, int imgWidth, int imgHeight)
        {
            double gauss1 = MathHelper.NextGaussian(0, 0.3);
            double gauss2 = MathHelper.NextGaussian(0, 0.3);

            double px = ((col + gauss1) / (imgWidth - 1d)) * 2 - 1;
            double py = ((row + gauss2) / (imgHeight - 1d)) * 2 - 1;

            return new Vector2((float)px, (float)py);
        }

        private static Vector2 DomainPixels(int col, int row, int imgWidth, int imgHeight)
        {
            double px = (col / (imgWidth - 1d)) * 2 - 1;
            double py = (row / (imgHeight - 1d)) * 2 - 1;

            return new Vector2((float)px, (float)py);
        }
    }
}
