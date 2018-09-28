using System.Numerics;

namespace CornellBox.Models
{
    public class LightSource
    {
        private Vector3 position;
        private Vector3 color;

        public LightSource(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }

        public Vector3 Position { get => position; private set => position = value; }
        public Vector3 Color { get => color; private set => color = value; }
    }
}
