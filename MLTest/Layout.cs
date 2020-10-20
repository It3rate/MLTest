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
        private Box[] _boxes;
        public Layout(int count)
        {
            _boxes = new Box[count];
            for (int i = 0; i < count; i++)
            {
                _boxes[i] = new Box();
            }
        }
        public Layout(params float[] values)
        {
            int count = (int)(values.Length / 4);
            _boxes = new Box[count];
            for (int i = 0; i < count; i++)
            {
                var st = i * 4;
                _boxes[i] = new Box(values[st + 0], values[st + 1], values[st + 2], values[st + 3]);
            }
        }
        public Layout(params double[] values)
        {
             int count = (int)(values.Length / 4);
            _boxes = new Box[count];
            for (int i = 0; i < count; i++)
            {
                var st = i * 4;
                _boxes[i] = new Box((float)values[st + 0], (float)values[st + 1], (float)values[st + 2], (float)values[st + 3]);
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
            for (int i = 0; i < _boxes.Length; i++)
            {
                result[i].Cx = _boxes[i].Cx;
                result[i].Cy = _boxes[i].Cy;
                result[i].Rx = _boxes[i].Rx;
                result[i].Ry = _boxes[i].Ry;
            }
            return result;
        }
        public float[] AsFloatArray()
        {
            var result = new float[_boxes.Length * 4];
            for (int i = 0; i < _boxes.Length; i++)
            {
                result[i * 4 + 0] = _boxes[i].Cx;
                result[i * 4 + 1] = _boxes[i].Cy;
                result[i * 4 + 2] = _boxes[i].Rx;
                result[i * 4 + 3] = _boxes[i].Ry;
            }
            return result;
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
        private Brush[] _brushes = new Brush[] { Brushes.DarkRed, Brushes.DarkSalmon, Brushes.DarkCyan, Brushes.DarkBlue, Brushes.DarkMagenta };
        public void Draw(Graphics g)
        {
            for (int i = 0; i < _boxes.Length; i++)
            {
                _boxes[i].Draw(_brushes[i], g);
            }
            Pen _kPen = new Pen(Brushes.Black, 0);
            g.DrawRectangle(_kPen, 0, 0, 1, 1);
        }

    }
    public class Box
    {
        public float Cx;
        public float Cy;
        public float Rx;
        public float Ry;

        public Box()
        {
        }
        public Box(float cx, float cy, float rx, float ry)
        {
            Cx = cx;
            Cy = cy;
            Rx = rx;
            Ry = ry;
        }

        public void Draw(Brush b, Graphics g)
        {
            var bl = Cx - Rx;
            var bt = Cy - Ry;
            var bw = Rx * 2f;
            var bh = Ry * 2f;
            RectangleF r = new RectangleF(bl, bt, bw, bh);
            g.FillRectangle(b, r);
        }

        public override string ToString()
        {
            return String.Format("{0:0.000},{1:0.000},{2:0.000},{3:0.000}", Cx, Cy, Rx, Ry);
        }
    }
}
