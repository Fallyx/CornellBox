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
        private Vector3 position;
        private Vector3 color;

        public LightSource(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 Color { get => color; set => color = value; }
    }
}
