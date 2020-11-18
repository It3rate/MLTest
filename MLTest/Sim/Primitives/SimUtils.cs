using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimUtils
    {
        private static Gaussian _narrowCompare = new Gaussian(0, 0.025);
        private static Gaussian _wideCompare = new Gaussian(0, 0.1);
        private static Gaussian _fullCompare = new Gaussian(0, 1);
        public static double ComparePoints(PointF p0, PointF p1)
        {
            var difX = Math.Abs(p0.X - p1.X);
            var difY = Math.Abs(p0.Y - p1.Y);
            return SimUtils.Likelihood(_fullCompare, difX) * SimUtils.Likelihood(_fullCompare, difY);
        }

        public static double Likelihood(Gaussian gaussian, double x)
        {
            var less = gaussian.GetProbLessThan(-Math.Abs(x));
            return Math.Abs(less * 2.0);
        }

    }
}
