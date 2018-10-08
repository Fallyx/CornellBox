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
        private Bitmap img;
        private Vector2 offset;

        private static string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public Material(Vector3 color, float reflection = 0, string imgPath = null, Vector2 offset = new Vector2())
        {
            Color = color;
            Reflection = reflection;
            ImgPath = imgPath;
            HasImg = String.IsNullOrEmpty(imgPath) ? false : true;
            Img = String.IsNullOrEmpty(imgPath) ? null : new Bitmap(imgPath);
            Offset = offset;
        }

        public Vector3 Color { get => color; private set => color = value; }
        public float Reflection { get => reflection; private set => reflection = value; }
        public string ImgPath { get => imgPath; set => imgPath = value; }
        public bool HasImg { get => hasImg; private set => hasImg = value; }
        public Bitmap Img { get => img; set => img = value; }
        public Vector2 Offset { get => offset; set => offset = value; }

        public Vector3 SphericalProjection(Vector3 position)
        {
            Vector3 imgColor = Vector3.Zero;

            if (position.X >= -1f && position.X <= 1 && position.Y >= -1f && position.Y <= 1 && position.Z >= -1f && position.Z <= 1)
            {
                double s = Math.Atan2(position.X, position.Z);
                double t = Math.Acos(position.Y);

                double _s = (s - -Math.PI) * (1 - -1) / (Math.PI - -Math.PI) + -1;
                double _t = (t - 0) * (1 - -1) / (Math.PI - 0) + -1;
                _s *= -1;
                _t *= -1;

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

            int x = (int)((s + 1) / 2f * (Img.Width - 1) + ((Img.Width - 1) * Offset.X)) % Img.Width;
            int y = (int)((t + 1) / 2f * (Img.Height - 1) + ((Img.Height - 1) * Offset.Y)) % Img.Height;

            var clr = Img.GetPixel(x, y);
            System.Windows.Media.Color c = System.Windows.Media.Color.FromRgb(clr.R, clr.G, clr.B);

            imgColor = new Vector3(c.B / 255f, c.G / 255f, c.R / 255f);

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
