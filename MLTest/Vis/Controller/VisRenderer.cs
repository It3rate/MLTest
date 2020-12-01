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

	    public VisRenderer(VisAgent agent, int width = 250, int height = 250)
	    {
		    Width = width;
		    Height = height;
		    Agent = agent ?? new VisAgent();
		    GenPens(Width*4);
        }
        public void Draw(Graphics g)
        {
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var prim in Agent.FocusPad.Paths)
            {
	            DrawPrimitive(g, prim, 2);
            }

            foreach (var path in Agent.ViewPad.Paths)
            {
                DrawPath(g, path, 1);
            }
        }
        public void DrawShape(Graphics g, Stroke shape)
        {
            //foreach (var stroke in shape.Strokes)
            //{
            //    DrawStroke(g, stroke, (int)shape.StructuralType);
            //}
        }

        public void DrawPath(Graphics g, IPath path, int penIndex = 0)
        {
	        var anchors = path.GenerateSegments();
	        Point start = anchors[0];
	        for (int i = 1; i < anchors.Count; i++)
	        {
		        var end = anchors[i];
		        DrawLine(g, start, end, penIndex);
		        start = end;
	        }
            
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
        public void DrawSpot(Graphics g, Point pos, int penIndex = 0, float scale = 1f)
        {
	        var r = Pens[penIndex].Width * scale;
	        g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawCircle(Graphics g, Circle circ, int penIndex = 0)
        {
	        var pos = circ.Center;
	        var r = circ.Radius;
	        g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }
        public void DrawRect(Graphics g, Rectangle rect, int penIndex = 0)
        {
	        g.DrawRectangle(Pens[penIndex], rect.TopLeft.X, rect.TopLeft.Y, rect.Size.X, rect.Size.Y);
        }

        public void DrawLine(Graphics g, Point p0, Point p1, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], p0.X, p0.Y, p1.X, p1.Y);
        }

        public void DrawPrimitive(Graphics g, IPrimitive path, int penIndex = 0)
        {
	        if (path is Line line)
	        {
                DrawLine(g, line.StartPoint, line.EndPoint);
	        }
	        else if (path is Circle circ)
	        {
		        DrawSpot(g, circ.Center, penIndex);
		        DrawCircle(g, circ, penIndex);
            }
	        else if (path is Rectangle rect)
	        {
		        DrawRect(g, rect, penIndex);
	        }
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
