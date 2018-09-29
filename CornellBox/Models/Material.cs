using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CornellBox.Models
{
    public class Material
    {
        private Vector3 color;
        private float reflection;

        public Material(Vector3 color, float reflection = 0)
        {
            Color = color;
            Reflection = reflection;
        }

        public Vector3 Color { get => color; private set => color = value; }
        public float Reflection { get => reflection; private set => reflection = value; }
    }
}
