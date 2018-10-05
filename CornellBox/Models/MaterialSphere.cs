using System.Numerics;

namespace CornellBox.Models
{
    public class MaterialSphere : Sphere
    {
        private Material material;
        private bool hasDiffuse;
        private bool hasPhong;
        private bool hasShadow;
        private bool hasReflection;

        public MaterialSphere(Vector3 center, double radius, Material material, bool hasDiffuse = true, bool hasPhong = true, bool hasShadow = true, bool hasReflection = true) : base(center, radius)
        {
            Material = material;
            HasDiffuse = hasDiffuse;
            HasPhong = hasPhong;
            HasShadow = hasShadow;
            HasReflection = hasReflection;
        }

        public Material Material { get => material; private set => material = value; }
        public bool HasDiffuse { get => hasDiffuse; private set => hasDiffuse = value; }
        public bool HasPhong { get => hasPhong; private set => hasPhong = value; }
        public bool HasShadow { get => hasShadow; private set => hasShadow = value; }
        public bool HasReflection { get => hasReflection; private set => hasReflection = value; }
    }
}
