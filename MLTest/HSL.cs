using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest
{
    public class HSL
    {
        public float H { get; set; } // 0-360 is normalized to 0-1 here!!
        public float S { get; set; }
        public float L { get; set; }

        private static Random rnd = new Random();

        public HSL(float h, float s, float l)
        {
            H = Math.Min(1f, Math.Max(0, h));
            S = Math.Min(1f, Math.Max(0, s));
            L = Math.Min(1f, Math.Max(0, l));
        }

        /// <summary>
        /// Divides distance into 3 random sections, applies to existing H, S, and L.
        /// </summary>
        public HSL HSLFromDistance(float distance)
        {
            var div0 = (float)rnd.NextDouble() * distance;
            var div1 = (float)rnd.NextDouble() * distance;
            var p0 = Math.Min(div0, div1);
            var p1 = Math.Max(div0, div1);
            var hNudge = (p0 * (float)rnd.NextDouble()) - (p0 / 2f);
            var sNudge = ((p1 - p0) * (float)rnd.NextDouble()) - ((p1-p0) / 2f);
            var lNudge = ((1f - p1) * (float)rnd.NextDouble()) - ((1f - p1) / 2f);
            // now is -nudge to nudge
            // account for remaining room in each HSL for shift. Clamp?

            return new HSL(H+hNudge, S+sNudge, L+lNudge);
        }
        public float[] AsArray() 
        {
            return new float[] { H, S, L };
        }

        public HSL Clone()
        {
            return new HSL(H, S, L);
        }
        public Color ColorValue()
        {
            int r, g, b;
            HlsToRgb(H * 360, S, L, out r, out g, out b);
            return Color.FromArgb(255, r, g, b);
        }

        public static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        public static void RgbToHls(int r, int g, int b,
            out double h, out double l, out double s)
        {
            // Convert RGB to a 0.0 to 1.0 range.
            double double_r = r / 255.0;
            double double_g = g / 255.0;
            double double_b = b / 255.0;

            // Get the maximum and minimum RGB components.
            double max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            double min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            double diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                double r_dist = (max - double_r) / diff;
                double g_dist = (max - double_g) / diff;
                double b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;

                h = h / 360.0; // normalize
            }
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }

        public void Serialize(StreamWriter sw)
        {
            sw.Write(String.Format("{0:0.000},{1:0.000},{2:0.000}", H, S, L));
        }
        public override string ToString()
        {
            return "HSL:[" + String.Format("{0:0.000},{1:0.000},{2:0.000}", H, S, L) + "]";
        }
    }
}
