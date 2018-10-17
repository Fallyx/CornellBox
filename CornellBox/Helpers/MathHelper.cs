using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBox.Helpers
{
    public static class MathHelper
    {
        private static Random r = new Random();

        public static Random Rand { get => r; }

        // https://bitbucket.org/Superbest/superbest-random/src/f067e1dc014c31be62c5280ee16544381e04e303/Superbest%20random/RandomExtensions.cs
        public static double NextGaussian(double mu = 0, double sigma = 1)
        {
            var u1 = Rand.NextDouble();
            var u2 = Rand.NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = mu + sigma * rand_std_normal;

            return rand_normal;
        }

        public static double RangeConverter(double A, double B, double a, double b, double val)
        {
            return (val - A) * (b - a) / (B - A) + a;
        }
    }
}
