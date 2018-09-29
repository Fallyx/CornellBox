using System.Numerics;

namespace CornellBox.Models
{
    public class Sphere
    {
        private Vector3 center;
        private double radius;
        private Material material;

        public Sphere(Vector3 center, double radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Vector3 Center { get => center; private set => center = value; }
        public double Radius { get => radius; private set => radius = value; }
        internal Material Material { get => material; private set => material = value; }
    }
}
