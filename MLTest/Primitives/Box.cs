using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest
{
    public class Box
    {
        public float Cx;
        public float Cy;
        public float Rx;
        public float Ry;
        public float ColorOffset;
        private HSL _color;
        public HSL Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                _brush = new SolidBrush(_color.ColorValue());
            }
        }

        public Box()
        {
        }
        public Box(float cx, float cy, float rx, float ry, float colorOffset = 0)
        {
            Cx = cx;
            Cy = cy;
            Rx = rx;
            Ry = ry;

            ColorOffset = colorOffset;
        }
        public Box(float cx, float cy, float rx, float ry, HSL hsl)
        {
            Cx = cx;
            Cy = cy;
            Rx = rx;
            Ry = ry;

            Color = hsl;
        }
        public bool IsZeroed;
        public void ZeroPositions()
        {
            IsZeroed = true;
            Cx = 0f;
            Cy = 0f;
            Rx = 0f;
            Ry = 0f;
        }

        public float[] InputArray()
        {
            return new float[] { Cx, Cy, Rx, Ry, ColorOffset};
        }
        public float[] TargetArray()
        {
            return new float[] { Cx, Cy, Rx, Ry, Color.H, Color.S, Color.L };
        }

        Brush _brush;
        public void Draw(Graphics g)
        {
            var bl = Cx - Rx;
            var bt = Cy - Ry;
            var bw = Rx * 2f;
            var bh = Ry * 2f;
            RectangleF r = new RectangleF(bl, bt, bw, bh);
            Brush b = _brush == null ? Brushes.Red : _brush;
            g.FillRectangle(b, r);
        }

        public void InputSerialize(StreamWriter sw)
        {
            if(IsZeroed)
            {
                sw.Write(String.Format("{0:0.000}", ColorOffset));
            }
            else
            {
                sw.Write(String.Format("{0:0.000},{1:0.000},{2:0.000},{3:0.000},{4:0.000}", Cx, Cy, Rx, Ry, ColorOffset));
            }
        }
        public void TargetSerialize(StreamWriter sw)
        {
            sw.Write(String.Format("{0:0.000},{1:0.000},{2:0.000},{3:0.000},", Cx, Cy, Rx, Ry));
            _color.Serialize(sw);
        }
        public override string ToString()
        {
            return String.Format("{0:0.000},{1:0.000},{2:0.000},{3:0.000} - [{4:0.00},{5:0.00},{6:0.00}]", Cx, Cy, Rx, Ry, _color.H, _color.S, _color.L);
        }
    }
}
