using System.Numerics;

namespace CornellBox
{
    class Sphere
    {
        private Vector3 center;
        private double radius;
        private Vector3 color;
        private float reflection;

        public Sphere(Vector3 center, double radius, Vector3 color, float reflection = 0)
        {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.reflection = reflection;
        }

        public Vector3 Center { get => center; set => center = value; }
        public double Radius { get => radius; set => radius = value; }
        public Vector3 Color { get => color; set => color = value; }
        public float Reflection { get => reflection; set => reflection = value; }
    }
}
