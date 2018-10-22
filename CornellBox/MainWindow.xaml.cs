using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Numerics;
using CornellBox.Scenes;
using CornellBox.Models;

namespace CornellBox
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int imgWidth = 770;
        const int imgHeight = 770;
        const int stride = 4 * imgWidth;
        const double FOV = 36;
        const int AASamples = 8;

        Vector3 Eye = new Vector3(0, 0, -4);
        Vector3 LookAt = new Vector3(0, 0, 6);     

        WriteableBitmap wbmap;

        public MainWindow()
        {
            InitializeComponent();

            wbmap = new WriteableBitmap(
                imgWidth,
                imgHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            cornellBoxImg.Source = wbmap;

            RenderPathTracing();
        }

        private void RenderRayTracing()
        {
            List<LightSource> lights = CornellBoxScene.InitLight(true, true); // Ray tracing lights
            List<Sphere> spheres = CornellBoxScene.InitSphere(); // Ray tracing spheres
            BoundingSphere bvh = BoundingSphere.BVH(spheres);

            byte[] pixels1d = CornellBoxScene.PixelArray(imgHeight, imgWidth, 4, bvh, lights, Eye, LookAt, FOV, AASamples);

            Render(pixels1d);
        }

        private void RenderPathTracing()
        {
            List<Sphere> spheres = CornellBoxScene.InitEmissiveSphere(); // Path tracing spheres
            BoundingSphere bvh = BoundingSphere.BVH(spheres);

            byte[] pixels1d = CornellBoxScene.PixelArray(imgHeight, imgWidth, 4, bvh, Eye, LookAt, FOV, AASamples);

            Render(pixels1d);
        }

        private void RenderBVH()
        {
            List<LightSource> lights = CornellBoxScene.InitBVHLight(Eye); // BVH Light
            List<Sphere> spheres = CornellBoxScene.InitBVHSphere(); // BVH spheres
            BoundingSphere bvh = BoundingSphere.BVH(spheres);

            byte[] pixels1d = CornellBoxScene.PixelArray(imgHeight, imgWidth, 4, bvh, lights, Eye, LookAt, FOV);

            Render(pixels1d);
        }

        private void RenderDOF()
        {
            List<LightSource> lights = CornellBoxScene.InitBVHLight(new Vector3(0.5f, 0.5f, -4));
            List<Sphere> spheres = CornellBoxScene.InitDOFSphere();
            BoundingSphere bvh = BoundingSphere.BVH(spheres);

            byte[] pixels1d = CornellBoxScene.PixelArray(imgHeight, imgWidth, 4, bvh, lights, Eye, LookAt, FOV);

            Render(pixels1d);
        }

        private void Render(byte[] pixels1d)
        {
            Int32Rect rect = new Int32Rect(0, 0, imgWidth, imgHeight);
            wbmap.WritePixels(rect, pixels1d, stride, 0);
        }
    }
}
