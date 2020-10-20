using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest
{
    public class Layout
    {
        public HSL BaseColor { get; set; }
        public float Variation { get; set; }

        private static HSL _defaultColor = new HSL(0.9f, 0.7f, 0.3f);

        Gaussian similarGaussian = new Gaussian(0, 0.4);

        private Box[] _boxes;
        public Box[] BoxesRef => _boxes;

        public Layout(int count)
        {
            Init(count);
        }
        public Layout(int count, params float[] values)
        {
            Init(count, values);
        }
        public Layout(int count, float variation, HSL baseColor = null)
        {
            BaseColor = baseColor == null ? _defaultColor : baseColor;
            Variation = variation;
            Init(count);
        }
        private void Init(int count)
        {
            float[] values = new float[count * 4];
            Init(count, values);
        }
        private void Init(int count, float[] values)
        {
            _boxes = new Box[count];
            for (int i = 0; i < count; i++)
            {
                var st = i * 4;
                _boxes[i] = new Box(values[st + 0], values[st + 1], values[st + 2], values[st + 3]);
            }
        }

        public void ColorBoxes()
        {
            Variation = (float)similarGaussian.Sample();
            foreach (var box in _boxes) 
            {
                box.Color = BaseColor.HSLFromDistance(Variation + box.ColorOffset);
            }
        }

        public Box this[int i] => _boxes[i];
        public int Count => _boxes.Length;

        public void Rotate()
        {
            foreach (Box box in _boxes)
            {
                float temp;
                temp = box.Cy;
                box.Cy = box.Cx;
                box.Cx = temp;

                temp = box.Ry;
                box.Ry = box.Rx;
                box.Rx = temp;
            }
        }
        public Layout Clone()
        {
            var result = new Layout(_boxes.Length);
            result.BaseColor = BaseColor.Clone();
            result.Variation = Variation;
            for (int i = 0; i < _boxes.Length; i++)
            {
                result[i].Cx = _boxes[i].Cx;
                result[i].Cy = _boxes[i].Cy;
                result[i].Rx = _boxes[i].Rx;
                result[i].Ry = _boxes[i].Ry;
                result[i].ColorOffset = _boxes[i].ColorOffset;
                result[i].Color = _boxes[i].Color.Clone();
            }
            return result;
        }
        public float[] InputArray()
        {
            List<float> vals = new List<float>();
            vals.Add(Variation);
            vals.AddRange(BaseColor.AsArray());
            foreach (var box in _boxes)
            {
                vals.AddRange(box.InputArray());
            }
            return vals.ToArray();
        }
        public float[] TargetArray()
        {
            List<float> vals = new List<float>();
            vals.Add(Variation);
            vals.AddRange(BaseColor.AsArray());
            foreach (var box in _boxes)
            {
                vals.AddRange(box.TargetArray());
            }
            return vals.ToArray();
        }
        public override string ToString()
        {
            var result = "";
            var comma = "";
            foreach (var box in _boxes)
            {
                result += comma + box.ToString();
                comma = ", ";
            }
            return result;
        }
        public void Serialize(StringBuilder sb)
        {
            foreach (var box in _boxes)
            {
                sb.AppendLine(box.ToString());
            }
        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < _boxes.Length; i++)
            {
                _boxes[i].Draw(g);
            }
            Pen _kPen = new Pen(Brushes.Black, 0);
            g.DrawRectangle(_kPen, 0, 0, 1, 1);
        }
    }
}
