using System.Numerics;

namespace CornellBox
{
    class Ray
    {
        private Vector3 origin;
        private Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3 Origin { get => origin; set => origin = value; }
        public Vector3 Direction { get => direction; set => direction = value; }
    }
}
