using Keras;
using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest
{
    public class Design
    {
        public float Variation { get; set; }
        public HSL BaseColor { get; set; }

        private static HSL _defaultColor = new HSL(0.9f, 0.7f, 0.3f);


        private Box[] _boxes;
        public Box[] BoxesRef => _boxes;

        public Design(int count)
        {
            Init(count);
        }
        public Design(int count, params float[] values)
        {
            if (values.Length == 19) // input array (has base color and variation)
            {
                InitWithInputArray(count, values);
            }
            else if (values.Length == 21) // target array (has colors for boxes)
            {
                InitWithTargetArray(count, values);
            }
            else
            {
                Init(count, values);
            }
            
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

        static Random rnd = new Random();
        public static Design GenLayout3(float h, float v)
        {
            Design result = new Design(3);
            TruncatedGaussian colorGenGaussian = new TruncatedGaussian(0.5, 0.04, 0.2, 0.8);
            //Gaussian colorGenGaussian = new Gaussian(0.5, 0.3);
            result.BaseColor = new HSL(
                (float)rnd.NextDouble(),
                (float)(colorGenGaussian.Sample() * 0.70 + 0.25),
                (float)(colorGenGaussian.Sample()));
            TruncatedGaussian variationGaussian = new TruncatedGaussian(0, 0.02, -0.1, 0.1);
            var vs = variationGaussian.Sample();
            result.Variation = (float) ((Math.Abs(vs) + 0.025) * Math.Sign(vs));

            var boxes = result.BoxesRef;

            bool wideBot = v > 0.5;
            boxes[0].Cx = 0.5f;
            boxes[0].Cy = wideBot ? v / 2.0f : 1f - (1f - v) / 2.0f;
            boxes[0].Rx = 0.5f;
            boxes[0].Ry = wideBot ? v / 2.0f : (1.0f - v) / 2.0f;
                
            boxes[1].Cx = h / 2.0f;
            boxes[1].Cy = wideBot ? v + (1.0f - v) / 2.0f : v / 2.0f;
            boxes[1].Rx = h / 2.0f;
            boxes[1].Ry = wideBot ? (1.0f - v) / 2.0f : v / 2.0f;

            boxes[2].Cx = h + (1.0f - h) / 2.0f;
            boxes[2].Cy = boxes[1].Cy;
            boxes[2].Rx = (1.0f - h) / 2.0f;
            boxes[2].Ry = boxes[1].Ry;

            ColorBoxesWithOffset(result, 0.03f);

            if (rnd.NextDouble() > 0.5)
            {
                result.Rotate();
            }
            boxes.Shuffle();
            return result;

        }
        public static void ColorBoxesWithOffset(Design design, float stdDev)
        {
            var boxes = design.BoxesRef;

            // sort by size to keep coloring constant across shuffles
            var dict = new Dictionary<int, float>() { { 0, 0 }, { 1, 0 }, { 2, 0 } };
            var list = new List<KeyValuePair<int, float>>();
            foreach (var val in dict)
            {
                list.Add(new KeyValuePair<int, float>(val.Key, boxes[val.Key].Rx * boxes[val.Key].Ry));
            }
            list.Sort((x, y) => x.Value.CompareTo(y.Value));

            TruncatedGaussian boxColorMOffset = new TruncatedGaussian(0, stdDev, -stdDev * 3f, stdDev * 3f);
            var offset = (float)Math.Abs(boxColorMOffset.Sample()) + 0.05f;
            boxes[list[2].Key].ColorOffset = 0;
            boxes[list[1].Key].ColorOffset = offset;
            boxes[list[0].Key].ColorOffset = -offset;
            foreach (var box in boxes)
            {
                box.Color = design.BaseColor.HSLFromDistance(design.Variation + box.ColorOffset);
            }
        }
        public void GenColor()
        {
            foreach (var box in _boxes)
            {
                box.Color = BaseColor.HSLFromDistance(Variation * box.ColorOffset);
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
        public Design Clone()
        {
            var result = new Design(_boxes.Length);
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

        public void InitWithInputArray(int count, float[] v)
        {
            Variation = v[0];
            BaseColor = new HSL(v[1], v[2], v[3]);
            int vIndex = 4;
            _boxes = new Box[count];
            for (int i = 0; i < count; i++)
            {
                _boxes[i] = new Box(v[vIndex + 0], v[vIndex + 1], v[vIndex + 2], v[vIndex + 3], v[vIndex + 4]);
                vIndex += 5;
            }
            GenColor(); // This won't be the exact target or input color, as that isn't saved (input is only variation).
        }
        public void InitWithTargetArray(int count, float[] v)
        {
            int vIndex = 0;
            _boxes = new Box[count];
            for (int i = 0; i < _boxes.Length; i++)
            {
                _boxes[i] = new Box(v[vIndex + 0], v[vIndex + 1], v[vIndex + 2], v[vIndex + 3], new HSL(v[vIndex + 4], v[vIndex + 5], v[vIndex + 6]));
                vIndex += 7;
            }
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
        public void InputSerialize(StreamWriter sw)
        {
            sw.Write(String.Format("{0:0.000},", Variation));
            BaseColor.Serialize(sw);
            foreach (var box in _boxes)
            {
                sw.Write(",");
                box.InputSerialize(sw);
            }
            sw.WriteLine();
        }
        public void TargetSerialize(StreamWriter sw)
        {
            var comma = "";
            foreach (var box in _boxes)
            {
                sw.Write(comma);
                box.TargetSerialize(sw);
                comma = ",";
            }
            sw.WriteLine();
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
