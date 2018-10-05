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

        public Vector3 GetColorFromImage(Vector3 position)
        {
            double s = Math.Atan2(Math.Min(1, Math.Max(-1, position.X)), Math.Min(1, Math.Max(-1, position.Z)));
            double t = Math.Acos(Math.Min(1, Math.Max(-1, position.Z))); 

            Vector3 imgColor = Vector3.Zero;

            using (Bitmap bmp = new Bitmap(ImgPath))
            {
                //int x = (int)(Math.Min(Math.Round((s+1) * bmp.Width / 2f), bmp.Width - 1));
                //int y = (int)(Math.Min(Math.Round((t+1) * bmp.Height / 2f), bmp.Height - 1));

                int x = (int)((Math.Min(1, Math.Max(-1, s)) + 1) / 2f * (bmp.Width - 1));
                int y = (int)((Math.Min(1, Math.Max(-1, t)) + 1) / 2f * (bmp.Height - 1));

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
