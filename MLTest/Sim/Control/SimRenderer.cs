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
                DrawStroke(g, stroke, (int)shape.ShapeType);
            }
        }

        public void DrawStroke(Graphics g, SimStroke stroke, int penIndex = 0)
        {
            foreach (var point in stroke.Anchors)
            {
                DrawCircle(g, point, 0);
            }

            if(stroke.Edges.Count == 0)
            {
                DrawLine(g, stroke.Start, stroke.End, penIndex);
            }
            else
            {
                DrawCircle(g, stroke.Edges[0].Anchor0, 2, 1.5);
                DrawCircle(g, stroke.Edges[0].Anchor1, 3, 1.5);
                DrawCurve(g, stroke.Start, stroke.Edges[0], stroke.End, penIndex);
            }
        }
        public void DrawSpot(Graphics g, SimSection spot, int penIndex = 0)
        {
        }

        public void DrawCircle(Graphics g, PointF pos, int penIndex = 0, double scale = 0)
        {
            float r = (float)(scale == 0 ? Pens[penIndex].Width * 4f : Pens[penIndex].Width * scale);
            g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawLine(Graphics g, SimNode p0, SimNode p1, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], p0.AnchorPoint.X, p0.AnchorPoint.Y, p1.AnchorPoint.X, p1.AnchorPoint.Y);
        }
        public void DrawCurve(Graphics g, SimNode p0, SimEdge edge, SimNode p1, int penIndex = 0)
        {
            //g.DrawBezier(Pens[penIndex], p0.AnchorPoint, edge.Anchor0, edge.Anchor1, p1.AnchorPoint);
            var mid = new PointF(
                (float)((edge.Anchor1.X - edge.Anchor0.X) / 2.0 + edge.Anchor0.X),
                (float)((edge.Anchor1.Y - edge.Anchor0.Y) / 2.0 + edge.Anchor0.Y));
            g.DrawBezier(Pens[penIndex], p0.AnchorPoint, edge.Anchor0, edge.Anchor0, mid);
            g.DrawBezier(Pens[penIndex], mid, edge.Anchor1, edge.Anchor1, p1.AnchorPoint);
        }

        public List<Pen> Pens = new List<Pen>();
        private enum PenTypes
        {
            LightGray,
            Black,
            DarkRed,
            Orange,
            DarkGreen,
            DarkBlue,
            DarkViolet,
        }
        private void GenPens(float scale)
        {
            Pens.Clear();
            Pens.Add(GetPen(Color.LightGray, 4f / scale));
            Pens.Add(GetPen(Color.Black, 8f / scale));
            Pens.Add(GetPen(Color.DarkRed, 8f / scale));
            Pens.Add(GetPen(Color.Orange, 8f / scale));
            Pens.Add(GetPen(Color.DarkGreen, 8f / scale));
            Pens.Add(GetPen(Color.DarkBlue, 8f / scale));
            Pens.Add(GetPen(Color.DarkViolet, 8f / scale));
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
