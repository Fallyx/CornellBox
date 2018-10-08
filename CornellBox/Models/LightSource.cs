using System.Numerics;

namespace CornellBox.Models
{
    public class LightSource
    {
        private Vector3 position;
        private Vector3 color;
        private double radius;

        public LightSource(Vector3 position, Vector3 color, double radius = 0)
        {
            Position = position;
            Color = color;
            Radius = radius;
        }

        public Vector3 Position { get => position; private set => position = value; }
        public Vector3 Color { get => color; private set => color = value; }
        public double Radius { get => radius; set => radius = value; }
    }
}
