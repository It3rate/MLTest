using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MLTest.Vis;

namespace MLTest
{
    public class Utils
    {
        private static Gaussian _narrowCompare = new Gaussian(0, 0.025);
        private static Gaussian _wideCompare = new Gaussian(0, 0.1);
        private static Gaussian _fullCompare = new Gaussian(0, 1);
        private static double _narrowCutoff = 0.001;

        public static double ComparePoints(PointF p0, PointF p1)
        {
            var difX = Math.Abs(p0.X - p1.X);
            var difY = Math.Abs(p0.Y - p1.Y);
            return Utils.Likelihood(_fullCompare, difX) * Utils.Likelihood(_fullCompare, difY);
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

        private static double TWO_PI = 2 * Math.PI;
        public static double RadiansToNorm(double radians)
        {
            double normalized = radians % TWO_PI;
            normalized = (normalized + TWO_PI) % TWO_PI;
            return normalized <= Math.PI ? normalized : normalized - TWO_PI;
        }
        /// <summary>
        /// 0 North, 0.5 East, 1/-1 South, -0.5 West
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        public static double NormToRadians(double norm)
        {
            double normalized = -(1.0 - (norm / 2.0 + 0.5) - 0.5); // 0-1 cw, 0 is east with extra 0.25
            normalized *= TWO_PI;
            normalized = (normalized + TWO_PI) % TWO_PI;
            return normalized;
        }
    }
    public class SimCircle
    {
        public PointF Center { get; }
        public float Radius { get; }
    } 

    public static class ExtensionMethods
    {
        public static Vis.Point VisPoint(this PointF p0) => new Vis.Point(p0.X, p0.Y);

        public static float Length(this PointF p0) => (float)Math.Sqrt(p0.X * p0.X + p0.Y * p0.Y);
        public static float SquaredLength(this PointF p0) => p0.X * p0.X + p0.Y * p0.Y;
        public static float DistanceTo(this PointF p0, PointF p1) => (float)Math.Sqrt(Math.Pow(p1.X - p0.X, 2) + Math.Pow(p1.Y - p0.Y, 2));
        public static PointF Add(this PointF p0, PointF p1) => new PointF(p0.X + p1.X, p0.Y + p1.Y);
        public static PointF Subtract(this PointF p0, PointF p1) => new PointF(p1.X - p0.X, p1.Y - p0.Y);
        public static PointF Multiply(this PointF p0, float scalar) => new PointF(p0.X * scalar, p0.Y * scalar);
        public static PointF Multiply(this PointF p0, PointF p1) =>  new PointF(p0.X * p1.X, p0.Y * p1.Y);
        public static PointF DivideBy(this PointF p0, float scalar) => new PointF(p0.X / scalar, p0.Y / scalar);
        public static PointF Swap(this PointF a) => new PointF(a.Y, a.X);
        public static PointF MidPoint(this PointF p0, PointF p1) => new PointF((p1.X - p0.X) / 2f + p0.X, (p1.Y - p0.Y) / 2f + p0.Y);
        public static float DotProduct(this PointF a, PointF b) => -((a.X * b.X) + (a.Y * b.Y)); // negative because inverted Y


        public static PointF ProjectedOntoLine(this PointF p, PointF v1, PointF v2)
        {
            var e1 = v2.Subtract(v1);
            var e2 = p.Subtract(v1);
            var dp = e1.DotProduct(e2);
            var len2 = e1.SquaredLength();
            return new PointF(v1.X + (dp * e1.X) / len2, v1.Y + (dp * e1.Y) / len2) ;
        }
        public static bool FindTangents(this PointF p, PointF c, float r, out PointF pt1, out PointF pt2)
        {
            float dist = c.DistanceTo(p);
            float diameter = r * r;
            double L = Math.Sqrt(dist - diameter);
            FindTwoCircleIntersections(c, r, p, (float)L, out pt1, out pt2);
            return true;
        }

        public static int FindTwoCircleIntersections(SimCircle c0, SimCircle c1, out PointF intersect0, out PointF intersect1) => FindTwoCircleIntersections(c0.Center, c0.Radius, c1.Center, c1.Radius, out intersect0, out intersect1);
        public static int FindTwoCircleIntersections(PointF c0, float r0, PointF c1, float r1, out PointF intersect0, out PointF intersect1)
        {
            float dist = c0.DistanceTo(c1); 

            // See how many solutions there are.
            if (dist > r0 + r1)
            {
                // No solutions, the circles are too far apart.
                intersect0 = new PointF(float.NaN, float.NaN);
                intersect1 = new PointF(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(r0 - r1))
            {
                // No solutions, one circle contains the other.
                intersect0 = new PointF(float.NaN, float.NaN);
                intersect1 = new PointF(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (r0 == r1))
            {
                // No solutions, the circles coincide.
                intersect0 = new PointF(float.NaN, float.NaN);
                intersect1 = new PointF(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                float a = (r0 * r0 - r1 * r1 + dist * dist) / (2 * dist);
                float h = (float)Math.Sqrt(r0 * r0 - a * a);

                // Find P2.
                var p2 = c1.Subtract(c0).DivideBy(dist).Multiply(a).Add(c0);
                //var p2 = new PointF(c0.X + a * (c1.X - c0.X) / dist, c0.Y + a * (c1.Y - c0.Y) / dist);

                // Get the points P3.
                var dif = c1.Subtract(c0).Swap().DivideBy(dist).Multiply(h);
                intersect0 = new PointF(p2.X + dif.X, p2.Y - dif.Y);
                intersect1 = new PointF(p2.X - dif.X, p2.Y + dif.Y);
                //    (float)(cx2 + h * (c1.Y - c0.Y) / dist),
                //    (float)(cy2 - h * (c1.X - c0.X) / dist));
                //intersection2 = new PointF(
                //    (float)(cx2 - h * (c1.Y - c0.Y) / dist),
                //    (float)(cy2 + h * (c1.X - c0.X) / dist));

                // See if we have 1 or 2 solutions.
                return (dist == r0 + r1) ? 1 : 2;
            }
        }
    }

}
