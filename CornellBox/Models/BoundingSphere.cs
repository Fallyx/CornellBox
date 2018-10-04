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


        public BoundingSphere(Vector3 center, double radius, Material material, bool hasChildren, BoundingSphere leftChild, BoundingSphere rightChild) : base (center, radius, material)
        {
            HasChildren = hasChildren;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public bool HasChildren { get => hasChildren; private set => hasChildren = value; }
        public BoundingSphere LeftChild { get => leftChild; private set => leftChild = value; }
        public BoundingSphere RightChild { get => rightChild; private set => rightChild = value; }

        public static BoundingSphere BVH(List<Sphere> spheres)
        {
            List<BoundingSphere> bSpheres = new List<BoundingSphere>();
            foreach(Sphere s in spheres)
            {
                bSpheres.Add(new BoundingSphere(s.Center, s.Radius, s.Material, false, null, null));
            }

            for(int i = 0; i < spheres.Count - 1; i++)
            {
                BoundingSphere bsLeft;
                BoundingSphere bsRight;
                Vector3 distance = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);


                for(int x = 0; x < bSpheres.Count - 1; x++)
                {
                    for(int y = x + 1; y < bSpheres.Count; y++)
                    {
                        if (Vector3.Subtract(bSpheres[x].Center, bSpheres[y].Center).Length() > distance.Length()) continue;

                        //Vector3 
                    }
                }
            }

            BoundingSphere rootBVH = null;



            return rootBVH;
        }
    }
}
