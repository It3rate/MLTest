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
        private static double _narrowCutoff = 0.001;

        public static double ComparePoints(PointF p0, PointF p1)
        {
            var difX = Math.Abs(p0.X - p1.X);
            var difY = Math.Abs(p0.Y - p1.Y);
            return SimUtils.Likelihood(_fullCompare, difX) * SimUtils.Likelihood(_fullCompare, difY);
        }

        /// <summary>
        /// Tests if a is about equal to b using the narrow compare gaussian.
        /// </summary>
        public static bool LikelyEqual(double a, double b)
        {
            return _narrowCompare.GetProbLessThan(-Math.Abs(a-b)) > _narrowCutoff;
        }
        /// <summary>
        /// Tests if a is probably less that b using the narrow compare gaussian.
        /// </summary>
        public static bool LikelyLess(double a, double b)
        {
            bool result = false;
            if (a < b)
            {
                result = _narrowCompare.GetProbLessThan(a - b) < _narrowCutoff;
            }
            return result;
        }

        public static double Likelihood(Gaussian gaussian, double x)
        {
            var less = gaussian.GetProbLessThan(-Math.Abs(x));
            return Math.Abs(less * 2.0);
        }

    }
}
