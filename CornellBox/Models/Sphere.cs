using System.Numerics;

namespace CornellBox.Models
{
    public abstract class Sphere
    {
        private Vector3 center;
        private double radius;

        public Sphere(Vector3 center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3 Center { get => center; private set => center = value; }
        public double Radius { get => radius; private set => radius = value; }
    }
}
