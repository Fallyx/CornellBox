using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;

namespace CornellBox
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int imgWidth = 770;
        const int imgHeight = 770;

        Vector3 Eye = new Vector3(0, 0, -4);
        Vector3 LookAt = new Vector3(0, 0, 6);
        const double FOV = 36;

        

        Sphere[] spheres = new Sphere[7];
        List<LightSource> lights = new List<LightSource>();

        int stride = 4 * imgWidth;

        WriteableBitmap wbmap;

        public MainWindow()
        {
            InitializeComponent();

            Sphere aRed = new Sphere(new Vector3(-1001, 0, 0), 1000, new Vector3(0, 0, 1));
            Sphere bBlue = new Sphere(new Vector3(1001, 0, 0), 1000, new Vector3(1, 0, 0));
            Sphere cWhite = new Sphere(new Vector3(0, 0, 1001), 1000, new Vector3(1, 1, 1));
            Sphere dWhite = new Sphere(new Vector3(0, -1001, 0), 1000, new Vector3(1, 1, 1));
            Sphere eWhite = new Sphere(new Vector3(0, 1001, 0), 1000, new Vector3(1, 1, 1));
            Sphere fYellow = new Sphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3, new Vector3(0, 1, 1));
            Sphere gCyan = new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6, new Vector3(1, 1, 0.88f));

            spheres[0] = aRed;
            spheres[1] = bBlue;
            spheres[2] = cWhite;
            spheres[3] = dWhite;
            spheres[4] = eWhite;
            spheres[5] = fYellow;
            spheres[6] = gCyan;

            LightSource WhiteLight = new LightSource(new Vector3(0, -0.9f, 0), new Vector3(0.5f, 0.5f, 0.5f));
            LightSource MagentaLight = new LightSource(new Vector3(-0.8f, -0.9f, 0), new Vector3(0.5f, 0f, 0.5f));
            LightSource YellowLight = new LightSource(new Vector3(0.8f, -0.9f, 0), new Vector3(0.5f, 0.5f, 0f));

            lights.Add(WhiteLight);
            lights.Add(MagentaLight);
            lights.Add(YellowLight);

            // CompositionTarget.Rendering += Render;

            wbmap = new WriteableBitmap(
                imgWidth,
                imgHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            cornellBoxImg.Source = wbmap;

            Render();
        }

        private void Render()
        {
            byte[] pixels1d = new byte[imgHeight * imgWidth * 4];
            int index1d = 0;
            Ray eyeRay;
            Hitpoint hPoint;

            

            for (int col = 0; col < imgWidth; col++)
            {
                for (int row = 0; row < imgHeight; row++)
                {
                    double px = (col / (imgWidth - 1d)) * 2 - 1;
                    double py = (row / (imgHeight - 1d)) * 2 - 1;

                    eyeRay = CreateEyeRay(Eye, LookAt, FOV, new Vector2((float)px, (float)py));
                    hPoint = FindClosestHitPoint(spheres, eyeRay);

                    Vector3 Ie = Vector3.Zero;
                    Vector3 I = Vector3.Zero;

                    Vector3 diff = Vector3.Zero;
                    Vector3 phong = Vector3.Zero;
                    Vector3 shadow = Vector3.Zero;

                    foreach(LightSource light in lights)
                    {
                        diff = Diffuse(light, hPoint);
                        phong = Phong(light, hPoint, 40, eyeRay);
                        shadow = Shadow(light, hPoint, spheres);

                        I += (diff * shadow) + phong;
                    }


                    /*
                    Vector3 diff = Diffuse(WhiteLight, hPoint);
                    Vector3 phong = Phong(WhiteLight, hPoint, 40, eyeRay);
                    Vector3 shadow = Shadow(WhiteLight, hPoint, spheres);

                    I = Ie + (diff * shadow) + phong;
                    */

                    I += Ie;

                    byte b = Convert.ToByte(Math.Min((I.X * hPoint.Sphere.Color.X) * 255, 255));
                    byte g = Convert.ToByte(Math.Min((I.Y * hPoint.Sphere.Color.Y) * 255, 255));
                    byte r = Convert.ToByte(Math.Min((I.Z * hPoint.Sphere.Color.Z) * 255, 255));

                    pixels1d[index1d++] = b;
                    pixels1d[index1d++] = g;
                    pixels1d[index1d++] = r;
                    
                    index1d++; // Skip Alpha
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, imgWidth, imgHeight);            
            wbmap.WritePixels(rect, pixels1d, stride, 0);
        }

        private Ray CreateEyeRay(Vector3 Eye, Vector3 LookAt, double FOV, Vector2 Pixel)
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

        private Hitpoint FindClosestHitPoint(Sphere[] spheres, Ray ray)
        {
            double closestHit = double.MaxValue;
            Sphere closestSphere = null;

            for (int i = 0; i < spheres.Length; i++)
            { 
                double r = spheres[i].Radius;
                Vector3 CE = Vector3.Subtract(ray.Origin, spheres[i].Center);
                float a = 1;
                Vector3 b1 = Vector3.Multiply(CE, 2);
                float b = Vector3.Dot(b1, Vector3.Normalize(ray.Direction)); 
                float c = (float)(CE.Length() * CE.Length() - spheres[i].Radius * spheres[i].Radius);

                if (b * b > 4 * a * c)
                {
                    double lambda1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                    double lambda2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

                    if(lambda1 >= 0 && lambda2 >= 0)
                    {
                        closestHit = Math.Min(closestHit, Math.Min(lambda1, lambda2));
                        if (closestHit == lambda1 || closestHit == lambda2)
                        {
                            closestSphere = spheres[i];
                        }
                    }
                    else if(lambda1 >= 0 && lambda2 < 0)
                    {
                        closestHit = Math.Min(closestHit, lambda1);
                        if(closestHit == lambda1)
                        {
                            closestSphere = spheres[i];
                        }
                    }
                    else if(lambda1 < 0 && lambda2 >= 0)
                    {
                        closestHit = Math.Min(closestHit, lambda2);
                        if (closestHit == lambda2)
                        {
                            closestSphere = spheres[i];
                        }
                    }
                }
            }

            Vector3 pos = new Vector3((float)(Eye.X + closestHit * ray.Direction.X), (float)(Eye.Y + closestHit * ray.Direction.Y), (float)(Eye.Z + closestHit * ray.Direction.Z));
            return new Hitpoint(pos, closestSphere);
        }

        private Vector3 Diffuse(LightSource light, Hitpoint h)
        {
            Vector3 diff = Vector3.Zero;

            Vector3 n = Vector3.Normalize(Vector3.Subtract(h.Position, h.Sphere.Center));
            Vector3 l = Vector3.Normalize(Vector3.Subtract(light.Position, h.Position));
            float nL = Vector3.Dot(n, l);

            if (nL >= 0)
            {
                Vector3 ilm = Vector3.Multiply(light.Color, h.Sphere.Color);
                diff = Vector3.Multiply(ilm, nL);
            }

            return diff;
        }

        
        private Vector3 Phong (LightSource light, Hitpoint h, int phongExp, Ray ray)
        {
            Vector3 phong = Vector3.Zero;

            Vector3 l = Vector3.Subtract(light.Position, h.Position);
            Vector3 n = Vector3.Normalize(Vector3.Subtract(h.Position, h.Sphere.Center));

            float nL = Vector3.Dot(n, Vector3.Normalize(l));
            if(nL >= 0)
            {
                float s1 = Vector3.Dot(l, n);
                Vector3 s2 = Vector3.Multiply(s1, n);
                Vector3 s = Vector3.Subtract(l, s2);
                
                Vector3 r1 = Vector3.Multiply(2f, s);
                Vector3 r = Vector3.Subtract(l, r1);

                float rEH = Vector3.Dot(Vector3.Normalize(r), ray.Direction);
                rEH = (float)Math.Pow(rEH, phongExp);
                phong = Vector3.Multiply(light.Color, rEH);
            }

            return phong;
        }

        private Vector3 Shadow(LightSource light, Hitpoint h, Sphere[] spheres)
        {
            Vector3 shadow = Vector3.One;
            
            Vector3 hl = Vector3.Subtract(light.Position,  Vector3.Multiply(h.Position, 0.9f));
            Ray lightRay = new Ray(h.Position, hl);

            foreach(Sphere s in spheres)
            {
                double lambda = CalcLambda(s, lightRay);
                if (lambda < hl.Length())
                {
                    shadow = new Vector3(light.Color.X * 0.2f, light.Color.Y * 0.2f, light.Color.Z * 0.2f);
                    break;
                }
            }
    
            return shadow;
        }

        private double CalcLambda(Sphere sphere, Ray ray)
        {
            Vector3 sr = Vector3.Subtract(ray.Origin, sphere.Center);
            float a = 1;
            float b = 2 * Vector3.Dot(Vector3.Normalize(ray.Direction), sr);
            float c = (float)(sr.Length() * sr.Length() - sphere.Radius * sphere.Radius);

            float determin = b * b - 4 * a * c;

            if (determin < 0) return double.MaxValue;

            double lambda1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            double lambda2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            double shorterLambda = (float)Math.Min(lambda1, lambda2);

            return shorterLambda > 0 ? shorterLambda : double.MaxValue;

        }
    }
}
