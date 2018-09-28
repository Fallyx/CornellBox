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

        Vector3 Eye = new Vector3(0, 0, -4);
        Vector3 LookAt = new Vector3(0, 0, 6);     

        List<Sphere> spheres = new List<Sphere>();
        List<LightSource> lights = new List<LightSource>();

        WriteableBitmap wbmap;

        public MainWindow()
        {
            InitializeComponent();

            spheres = CornellBoxScene.InitSphere();
            lights = CornellBoxScene.InitLight(true);

            // CompositionTarget.Rendering += Render;

            wbmap = new WriteableBitmap(
                imgWidth,
                imgHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            cornellBoxImg.Source = wbmap;

            Render();
        }

        private void Render()
        {
            byte[] pixels1d = CornellBoxScene.PixelArray(imgHeight, imgWidth, 4, spheres, lights, Eye, LookAt, FOV);

            Int32Rect rect = new Int32Rect(0, 0, imgWidth, imgHeight);            
            wbmap.WritePixels(rect, pixels1d, stride, 0);
        }
    }
}
