using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CornellBox
{
    class Hitpoint
    {
        private Vector3 position;
        private Sphere sphere;
        private Vector3 normal; // Add

        public Hitpoint(Vector3 position, Sphere sphere)
        {
            this.position = position;
            this.sphere = sphere;
        }

        public Vector3 Position { get => position; set => position = value; }
        public Sphere Sphere { get => sphere; set => sphere = value; }
    }
}
