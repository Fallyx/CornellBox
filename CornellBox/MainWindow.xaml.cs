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
        const int imgWidth = 771;
        const int imgHeight = 771;

        Vector3 Eye = new Vector3(0, 0, -4);
        Vector3 LookAt = new Vector3(0, 0, 6);
        const double FOV = 36;
        Sphere[] spheres = new Sphere[7];

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
            Sphere gCyan = new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6, new Vector3(1, 1, (float)0.88));

            spheres[0] = aRed;
            spheres[1] = bBlue;
            spheres[2] = cWhite;
            spheres[3] = dWhite;
            spheres[4] = eWhite;
            spheres[5] = fYellow;
            spheres[6] = gCyan;

            CompositionTarget.Rendering += Render;

            wbmap = new WriteableBitmap(
                imgWidth,
                imgHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            cornellBoxImg.Source = wbmap;
        }

        private void Render(object sener, EventArgs e)
        {
            byte[,,] pixels = new byte[imgHeight, imgWidth, stride];
            //byte[] pixels1d = null;
            byte[] pixels1d = new byte[imgHeight * imgWidth * 4];
            int index1d = 0;
            Ray eyeRay;
            Hitpoint hPoint;

            for (int col = 0; col < imgWidth; col++)
            {
                for (int row = 0; row < imgHeight; row++)
                {
                    double px = col / (imgWidth - 1D) * 2 - 1;
                    double py = row / (imgHeight - 1D) * 2 - 1;

                    eyeRay = CreateEyeRay(Eye, LookAt, FOV, new Vector2((float)px, (float)py));
                    hPoint = FindClosestHitPoint(spheres, eyeRay);
                    //pixels[row, col, 0] = Convert.ToByte(hPoint.Sphere.Color.X * 255);
                    //pixels[row, col, 1] = Convert.ToByte(hPoint.Sphere.Color.Y * 255);
                    //pixels[row, col, 2] = Convert.ToByte(hPoint.Sphere.Color.Z * 255);

                    pixels1d[index1d++] = Convert.ToByte(hPoint.Sphere.Color.X * 255);
                    pixels1d[index1d++] = Convert.ToByte(hPoint.Sphere.Color.Y * 255);
                    pixels1d[index1d++] = Convert.ToByte(hPoint.Sphere.Color.Z * 255);
                    index1d++;
                }
            }


            //pixels1d = ConvertTo1d(pixels, stride, imgWidth, imgHeight);
            Int32Rect rect = new Int32Rect(0, 0, imgWidth, imgHeight);            
            wbmap.WritePixels(rect, pixels1d, stride, 0);
        }

        private Ray CreateEyeRay(Vector3 Eye, Vector3 LookAt, double FOV, Vector2 Pixel)
        {
            double alpha = FOV * Math.PI / 180.0;
            Vector3 f = Vector3.Subtract(LookAt, Eye);
            Vector3 r = Vector3.Cross(new Vector3(0,1,0), f);
            Vector3 u = Vector3.Cross(r, f);

            // d = normalized f + x * normalized r * tan (a/2) + y * normalizd u * tan (alpha/2)
            Vector3 d1 = Vector3.Normalize(f);
            Vector3 d2 = Vector3.Multiply(Vector3.Normalize(r), Pixel.X * (float)Math.Tan(alpha / 2));
            Vector3 d3 = Vector3.Multiply(Vector3.Normalize(u), Pixel.Y * (float)Math.Tan(alpha / 2));

            //Vector3 d = Vector3.Normalize(f) + (Pixel.X * Vector3.Normalize(r) * (float) Math.Tan(FOV / 2)) + (Pixel.Y * Vector3.Normalize(u) * (float) Math.Tan(FOV / 2));
            Vector3 d = Vector3.Add(d1, Vector3.Add(d2, d3));

            return new Ray(Eye, Vector3.Normalize(d));
        }

        private Hitpoint FindClosestHitPoint(Sphere[] spheres, Ray ray)
        {
            double closestHit = double.MaxValue;
            Sphere closestSphere = null;

            for (int i = 0; i < spheres.Length; i++)
            {
                Vector3 H = Eye + ray.Origin;
                double r = spheres[i].Radius;
                Vector3 CE = ray.Origin - spheres[i].Center;
                float a = 1;
                //Vector3 b = 2 * CE + ray.Direction;
                float b = 2 * Vector3.Dot(CE, ray.Direction);
                float c = (float)(CE.Length() * CE.Length() - spheres[i].Radius * spheres[i].Radius);

                if (b * b > 4 * a * c)
                {
                    double lambda1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / 2 * a;
                    double lambda2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / 2 * a;

                    closestHit = Math.Min(closestHit, Math.Min(lambda1, lambda2));
                    if(closestHit == lambda1 || closestHit == lambda2)
                    {
                        closestSphere = spheres[i];
                    }
                }
            }

            Vector3 pos = new Vector3((float)(Eye.X + closestHit * ray.Origin.X), (float)(Eye.Y + closestHit * ray.Origin.Y), (float)(Eye.Z + closestHit * ray.Origin.Z));
            return new Hitpoint(pos, closestSphere);
        }

        private byte[] ConvertTo1d(byte[,,] pixels, int stride, int imgWidth, int imgHeight)
        {
            byte[] pixels1d = new byte[imgHeight * imgWidth * 4];
            int index = 0;

            for (int row = 0; row < imgHeight; row++)
            {
                for (int col = 0; col < imgWidth; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }

            return pixels1d;
        }
    }
}
