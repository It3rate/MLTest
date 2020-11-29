using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLTest.Vis;

namespace MLTest.Vis
{
    public class VisRenderer
    {
	    public int Width { get; set; }
	    public int Height { get; set; }
	    public VisAgent Agent { get; set; }

	    public VisRenderer(VisAgent agent = null, int width = 250, int height = 250)
	    {
		    Width = width;
		    Height = height;
		    Agent = agent ?? new VisAgent();
		    GenPens(Width);
        }
        public void Draw(Graphics g)
        {
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            //foreach (var element in Agent.ActivePad.Elements)
            //{
            //    DrawShape(g, element);
            //}
        }
        public void DrawShape(Graphics g, Stroke shape)
        {
            //foreach (var stroke in shape.Strokes)
            //{
            //    DrawStroke(g, stroke, (int)shape.StructuralType);
            //}
        }

        public void DrawStroke(Graphics g, Stroke stroke, int penIndex = 0)
        {
            //foreach (var point in stroke.Anchors)
            //{
            //    DrawCircle(g, point, 0);
            //}

            //if (stroke.Edges.Count == 0)
            //{
            //    DrawLine(g, stroke.Start, stroke.End, penIndex);
            //}
            //else
            //{
            //    foreach (var edge in stroke.Edges)
            //    {
            //        DrawCircle(g, edge.Anchor0, 2, 0.5);
            //        DrawCircle(g, edge.Anchor1, 3, 0.5);
            //    }
            //    //DrawCurve(g, stroke.Start, stroke.Edges[0], stroke.End, penIndex);
            //    DrawCurve(g, stroke, penIndex);
            //}
        }
        public void DrawSpot(Graphics g, Point spot, int penIndex = 0)
        {
        }

        public void DrawCircle(Graphics g, PointF pos, int penIndex = 0, double scale = 0)
        {
            float r = (float)(scale == 0 ? Pens[penIndex].Width * 4f : Pens[penIndex].Width * scale);
            g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawLine(Graphics g, Node p0, Node p1, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], p0.Anchor.X, p0.Anchor.Y, p1.Anchor.X, p1.Anchor.Y);
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
