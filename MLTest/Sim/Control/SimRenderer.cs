using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimRenderer
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public SimAgent Agent { get; set; }

        public SimRenderer(SimAgent agent = null, int width = 250, int height = 250)
        {
            Width = width;
            Height = height;
            Agent = agent; 
            GenPens(Width);
        }
        public void Draw(Graphics g)
        {
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var shape in Agent.ActivePad.Shapes)
            {
                DrawShape(g, shape);
            }
        }
        public void DrawShape(Graphics g, SimShape shape)
        {
            foreach(var stroke in shape.Strokes)
            {
                DrawStroke(g, stroke);
            }
        }

        public void DrawStroke(Graphics g, SimStroke stroke)
        {
            foreach (var point in stroke.Anchors)
            {
                DrawCircle(g, point);
            }

            if(stroke.Edges.Count == 0)
            {
                DrawLine(g, stroke.Start, stroke.End, 1);
            }
        }
        public void DrawSpot(Graphics g, SimSection spot)
        {
        }

        public void DrawCircle(Graphics g, PointF pos, double scale = 0)
        {
            float r = (float)(scale == 0 ? Pens[0].Width * 4f : Pens[0].Width * scale);
            g.DrawEllipse(Pens[0], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawLine(Graphics g, SimNode p0, SimNode p1, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], p0.AnchorPoint.X, p0.AnchorPoint.Y, p1.AnchorPoint.X, p1.AnchorPoint.Y);
        }

        public List<Pen> Pens = new List<Pen>();
        private enum PenTypes
        {
            LightGray,
            Black,
            DarkRed,
            DarkBlue,
            Orange,
        }
        private void GenPens(float scale)
        {
            Pens.Clear();
            Pens.Add(GetPen(Color.LightGray, 3f / scale));
            Pens.Add(GetPen(Color.Black, 8f / scale));
            Pens.Add(GetPen(Color.DarkRed, 8f / scale));
            Pens.Add(GetPen(Color.DarkBlue, 8f / scale));
            Pens.Add(GetPen(Color.Orange, 8f / scale));
        }
        private Pen GetPen(Color color, float width)
        {
            var pen = new Pen(color, width);
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
    }
}
