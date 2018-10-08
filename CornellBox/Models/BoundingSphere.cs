using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CornellBox.Models
{
    public class BoundingSphere : Sphere
    {
        private bool hasChildren;
        private BoundingSphere leftChild;
        private BoundingSphere rightChild;
        private MaterialSphere mSphere;

        public BoundingSphere(Vector3 center, double radius, bool hasChildren, BoundingSphere leftChild, BoundingSphere rightChild, MaterialSphere mSphere = null) : base (center, radius)
        {
            HasChildren = hasChildren;
            LeftChild = leftChild;
            RightChild = rightChild;
            MSphere = mSphere;
        }

        public bool HasChildren { get => hasChildren; private set => hasChildren = value; }
        public BoundingSphere LeftChild { get => leftChild; private set => leftChild = value; }
        public BoundingSphere RightChild { get => rightChild; private set => rightChild = value; }
        public MaterialSphere MSphere { get => mSphere; private set => mSphere = value; }

        /// <summary>
        /// Creates a bounding volume hierarchy
        /// </summary>
        /// <param name="spheres">List of spheres</param>
        /// <returns>Tree with bounding volumes</returns>
        public static BoundingSphere BVH(List<Sphere> spheres)
        {
            BoundingSphere rootBVH = null;

            List<BoundingSphere> bSpheres = new List<BoundingSphere>();
            foreach(Sphere s in spheres)
            {
                MaterialSphere mS = s as MaterialSphere;
                bSpheres.Add(new BoundingSphere(s.Center, s.Radius, false, null, null, mS));
            }

            for(int i = 0; i < spheres.Count - 1; i++)
            {
                BoundingSphere bsLeft = null;
                BoundingSphere bsRight = null;
                BoundingSphere node = null;
                Vector3 distance = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

                for(int x = 0; x < bSpheres.Count - 1; x++)
                {
                    for(int y = x + 1; y < bSpheres.Count; y++)
                    {
                        if (Vector3.Subtract(bSpheres[x].Center, bSpheres[y].Center).Length() > distance.Length()) continue;

                        distance = bSpheres[y].Center - bSpheres[x].Center;

                        double radius = (distance.Length() + bSpheres[x].Radius + bSpheres[y].Radius) / 2.0;
                        Vector3 center = bSpheres[x].Center + Vector3.Normalize(bSpheres[y].Center - bSpheres[x].Center) * (float)(radius - bSpheres[x].Radius);

                        bsLeft = bSpheres[x];
                        bsRight = bSpheres[y];
                        node = new BoundingSphere(center, radius, true, bSpheres[x], bSpheres[y]);
                    }
                }

                bSpheres.Remove(bsLeft);
                bSpheres.Remove(bsRight);
                bSpheres.Add(node);

                rootBVH = bSpheres[0];
            }

            return rootBVH;
        }
    }
}
