using System;
using System.Drawing;
using System.Numerics;


namespace CornellBox.Models
{
    public class Material
    {
        private Vector3 color;
        private float reflection;
        private string imgPath;
        private bool hasImg;

        private static string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public Material(Vector3 color, float reflection = 0, string imgPath = null)
        {
            Color = color;
            Reflection = reflection;
            ImgPath = imgPath;
            HasImg = String.IsNullOrEmpty(imgPath) ? false : true;
        }

        public Vector3 Color { get => color; private set => color = value; }
        public float Reflection { get => reflection; private set => reflection = value; }
        public string ImgPath { get => imgPath; set => imgPath = value; }
        public bool HasImg { get => hasImg; private set => hasImg = value; }

        public Vector3 SphericalProjection(Vector3 position)
        {
            Vector3 imgColor = Vector3.Zero;

            if (position.X >= -1f && position.X <= 1 && position.Y >= -1f && position.Y <= 1 && position.Z >= -1f && position.Z <= 1)
            {
                double s = Math.Atan2(position.X, position.Z) / (2 * Math.PI);
                double t = Math.Acos(position.Y) / Math.PI;

                double _s = (s - -Math.PI) * (1 - -1) / (Math.PI - -Math.PI) + -1;
                double _t = (t - 0) * (1 - -1) / (Math.PI - 0) + -1;

                imgColor = GetColorFromImage(_s, _t);
            }
            return imgColor;
        }

        public Vector3 PlanarProjection(Vector3 position)
        {
            double s = position.X;
            double t = position.Y;
            Vector3 imgColor = GetColorFromImage(s, t);

            return imgColor;
        }

        private Vector3 GetColorFromImage(double s, double t)
        {
            Vector3 imgColor = Vector3.One;

            using (Bitmap bmp = new Bitmap(ImgPath))
            {
                int x = (int)((s + 1) / 2f * (bmp.Width - 1)) % bmp.Width;
                int y = (int)((t + 1) / 2f * (bmp.Height - 1)) % bmp.Height;

                var clr = bmp.GetPixel(x, y);
                System.Windows.Media.Color c = System.Windows.Media.Color.FromRgb(clr.R, clr.G, clr.B);

                imgColor = new Vector3(c.B / 255f, c.G / 255f, c.R / 255f);
            }

            return imgColor;
        }



        public static string BrickImage()
        {
            return rootPath + @"\Textures\bricks.jpg";
        }

        public static string EarthImage()
        {
            return rootPath + @"\Textures\earth.jpg";
        }
    }
}
