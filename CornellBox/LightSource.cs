using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CornellBox
{
    class LightSource
    {
        private Vector3 center;
        private double radius;
        private Vector3 color;

        public LightSource(Vector3 center, Vector3 color)
        {
            this.center = center;
            this.color = color;
        }

        public Vector3 Center { get => center; set => center = value; }
        public double Radius { get => radius; set => radius = value; }
        public Vector3 Color { get => color; set => color = value; }
    }
}
