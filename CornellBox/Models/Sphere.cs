using System.Numerics;

namespace CornellBox.Models
{
    public class Sphere
    {
        private Vector3 center;
        private double radius;
        private Vector3 color;
        private float reflection;

        public Sphere(Vector3 center, double radius, Vector3 color, float reflection = 0)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Reflection = reflection;
        }

        public Vector3 Center { get => center; private set => center = value; }
        public double Radius { get => radius; private set => radius = value; }
        public Vector3 Color { get => color; private set => color = value; }
        public float Reflection { get => reflection; private set => reflection = value; }
    }
}
