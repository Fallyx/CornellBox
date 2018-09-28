﻿using System.Windows.Media;
using System.Collections.Generic;
using System.Numerics;

namespace CornellBox.Scenes
{
    class CornellBoxScene
    {
        public List<Sphere> InitSphere()
        {
            List<Sphere> spheres = new List<Sphere>();

            Sphere aRed = new Sphere(new Vector3(-1001, 0, 0), 1000, new Vector3(0, 0, 1));
            Sphere bBlue = new Sphere(new Vector3(1001, 0, 0), 1000, new Vector3(1, 0, 0));
            Sphere cWhite = new Sphere(new Vector3(0, 0, 1001), 1000, new Vector3(1, 1, 1));
            Sphere dWhite = new Sphere(new Vector3(0, -1001, 0), 1000, new Vector3(1, 1, 1));
            Sphere eWhite = new Sphere(new Vector3(0, 1001, 0), 1000, new Vector3(1, 1, 1));
            Sphere fYellow = new Sphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3, new Vector3(0, 1, 1), 0.5f);
            Sphere gCyan = new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6, new Vector3(1, 1, 0.88f), 0.5f);

            spheres.Add(aRed);
            spheres.Add(bBlue);
            spheres.Add(cWhite);
            spheres.Add(dWhite);
            spheres.Add(eWhite);
            spheres.Add(fYellow);
            spheres.Add(gCyan);

            return spheres;
        }

        public List<LightSource> InitLight(bool singleLight)
        {
            List<LightSource> lights = new List<LightSource>();
            LightSource WhiteLight;

            if (singleLight)
            {
                WhiteLight = new LightSource(new Vector3(0, -0.9f, 0), new Vector3(1f, 1f, 1f));
                lights.Add(WhiteLight);

                return lights;

            }

            WhiteLight = new LightSource(new Vector3(0, -0.9f, 0), new Vector3(0.5f, 0.5f, 0.5f));
            LightSource MagentaLight = new LightSource(new Vector3(-0.8f, -0.9f, 0), new Vector3(0.5f, 0f, 0.5f));
            LightSource YellowLight = new LightSource(new Vector3(0.8f, -0.9f, 0), new Vector3(0.5f, 0.5f, 0f));

            lights.Add(WhiteLight);
            lights.Add(MagentaLight);
            lights.Add(YellowLight);

            return lights;
        }

        /*
        public byte[] PixelArray (int imgHeight, int imgWidth, int stride, Vector3 Eye, Vector3 LookAt, double FOV)
        {
            byte[] pixels = new byte[imgHeight * imgWidth * stride];
            int index = 0;
            Ray eyeRay;

            for (int col = 0; col < imgWidth; col++)
            {
                for (int row = 0; row < imgHeight; row++)
                {
                    double px = (col / (imgWidth - 1d)) * 2 - 1;
                    double py = (row / (imgHeight - 1d)) * 2 - 1;

                    eyeRay = CreateEyeRay(Eye, LookAt, FOV, new Vector2((float)px, (float)py));

                    Vector3 color = CalcColor(eyeRay, 1);
                    Color c = Color.FromScRgb(1, color.Z, color.Y, color.X);

                    pixels[index++] = c.B;
                    pixels[index++] = c.G;
                    pixels[index++] = c.R;

                    index++; // Skip Alpha
                }
            }

            return pixels;
        }
        */
    }
}
