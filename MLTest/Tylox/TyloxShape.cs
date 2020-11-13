using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using PointIndex = System.Int32;
using ShapeIndex = System.Int32;

namespace MLTest.Tylox
{

    public abstract class TyloxPosition
    {
        protected static PointIndex _indexCounter = 0;
        public PointF AnchorPoint { get; set; }
        public virtual void SetPoint(TyloxShape parent) { }
    }

    public class TyloxPoint : TyloxPosition
    {
        public PointIndex Id { get; }

        public TyloxPoint(float x, float y) 
        {
            Id = _indexCounter++;
            AnchorPoint = new PointF(x, y);
        }
        public static TyloxPosition[] CreatePointList(float x, float y, params float[] points)
        {
            var result = new List<TyloxPosition>();
            result.Add(new TyloxPoint(x, y));
            for (int i = 0; i < points.Length; i+=2)
            {
                result.Add(new TyloxPoint(points[i], points[i + 1]));
            }
            return result.ToArray();
        }
    }

    public class TyloxJoint : TyloxPosition
    {
        public PointIndex Id { get; }

        // Parent is always the previous step up the parent/child chain, top level is abs distances in working area
        // if top level, Position is X and Offset is Y

        // It would be great to find segments by location - maybe using a covnet type filter, or like, vertical, left side
        // once we draw a stroke, our visual system reinterprets it to make a connection (we no longer care about stroke order, or if it was made top down or bottom up)
        //public float PairIndex { get; set; } 

        public float Position { get; set; } // 0-numPieces, the whole part is the starting piece p, the decimal part is how far along the line from p to p+1.
        public float Offset { get; set; }   // percent (or abs?) cx,cy of the peice segment is from the left (negative) or right (positive) of the start end line
        public float CrossSlide { get; set; } // x for default line of origin to 0,1
        public float Angle { get; set; }    // angle of intersection with endpoint. Set speed to zero if don't care
        public float Speed { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)

        // this should be in the segment pair data
        public float ZOffset { get; set; }  // pen up (-1) or down (1), use pen up for roughing in and moving to new positions. Needs to ramp somehow __/***\__
                                            // maybe this is just defined as odd up, even down, odd up... always alternating

        //public float Momentum { get; set; }
        //public float Flex { get; set; }
        //public float Mass { get; set; }
        //public float Sharpness { get; set; }
        //public float Friction { get; set; }
        //public PointF Center { get; set; }
        public TyloxJoint(float pos = 0, float offset = 0, float crossSlide = 0, float angle = 0, float speed = 0)
        {
            Id = _indexCounter++;

            Position = pos;
            Offset = offset;
            CrossSlide = crossSlide;
            Angle = angle;
            Speed = speed;
        }
        public override void SetPoint(TyloxShape parent)
        {
            // this will not work until the line is continuous
            int segIndex = (int)(Position + 0.00001f);
            float segPos = Position % 1;

            var seg = parent.Pairs[segIndex];
            //int last = parent.Pairs.Count - 1;
            //var seg = (segIndex < 0) ? parent.Pairs[0] : (segIndex > last) ? parent.Pairs[last] : parent.Pairs[segIndex];
            var sp = parent.Positions[seg.Start].AnchorPoint;
            var ep = parent.Positions[seg.End].AnchorPoint;
            float xOffset = 0;
            float yOffset = 0;
            float xDif = ep.X - sp.X;
            float yDif = ep.Y - sp.Y;
            if (Offset != 0)
            {
                float ang = (float)(Math.Atan2(yDif, xDif));
                xOffset = (float)(-Math.Sin(ang) * Math.Abs(Offset) * Math.Sign(-Offset));
                yOffset = (float)(Math.Cos(ang) * Math.Abs(Offset) * Math.Sign(-Offset));
            }
            AnchorPoint = new PointF(sp.X + (ep.X - sp.X) * segPos + xOffset, sp.Y + (ep.Y - sp.Y) * segPos + yOffset);
        }
    }

    //public class TyloxPositions:IEnumerable<TyloxPosition>
    //{
    //    List<TyloxPosition> Positions { get; set; }
    //    public TyloxPositions(params TyloxPosition[] positions)
    //    {
    //        Positions = new List<TyloxPosition>(positions);
    //    }
    //    public TyloxPosition this[int index] => Positions[index];

    //    public IEnumerator<TyloxPosition> GetEnumerator()
    //    {
    //        return Positions.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }

    //}

    public struct TyloxPair
    {
        public PointIndex Start { get; }
        public PointIndex End { get; }
        public int Pen { get; }
        public TyloxPair(PointIndex start, PointIndex end, int pen)
        {
            Start = start;
            End = end;
            Pen = pen;
        }
        public static List<TyloxPair> CreatePairList(PointIndex start, PointIndex end, int pen, params PointIndex[] pairs)
        {
            var result = new List<TyloxPair>();
            result.Add(new TyloxPair(start, end, pen));
            for (int i = 0; i < pairs.Length; i += 3)
            {
                result.Add(new TyloxPair(pairs[i], pairs[i + 1], pairs[i + 2]));
            }
            return result;
        }
    }

    public class TyloxPrimitive
    {
        public List<TyloxPoint> ShapePoints { get; }
    }
    public class TyloxWorldSaliency
    {
        public List<TyloxPrimitive> Shapes { get; }
    }
    public class TyloxAttention
    {
        // Sees both imagined positions and joints, as well as line drawn so far.
        // combine world, 
        public TyloxWorldSaliency WorldSaliency { get; }
        public List<TyloxPosition> Positions { get; }
        public TyloxShape Shape { get; }
    }

    // A shape is any list of strokes that only reference their parent layer. 
    // If a segment requires a built version of the shape to reference location, it must go in a child level.
    // E.g. the tail in K or R should be in a new layer as they reference the shape. 
    // An H is either way, but must reference the anchor lines and not vertical strokes if in the same shape.
    public class TyloxShape
    {
        private static ShapeIndex _indexCounter = 0;
        public ShapeIndex Id { get; }

        public TyloxShape Parent { get; private set; }
        List<TyloxShape> Children { get; } = new List<TyloxShape>();
        public List<TyloxPosition> Positions { get; }
        public List<TyloxPair> Pairs { get; }

        public TyloxShape(TyloxPosition[] positions, List<TyloxPair> pairs)
        {
            Id = _indexCounter++;
            Positions = new List<TyloxPosition>(positions);
            Pairs = pairs;
        }

        public void AddChild(TyloxShape child)
        {
            Children.Add(child);
            child.Parent = this;
            child.CalculatePoints(this);
        }

        public void CalculatePoints(TyloxShape parent)
        {
            foreach (var pos in Positions)
            {
                pos.SetPoint(parent);
            }
        }
        //public PointF[] GetSegmentPoints(int index)
        //{
        //    var posPair = Positions[index];
        //    var st = Parent.Positions[posPair.AnchorPoint];
        //}
        public void Draw(List<Pen> pens, Graphics g, int colorIndex)
        {
            float r = pens[0].Width * 0.6f;
            foreach (var pos in Positions)
            {
                g.DrawEllipse(pens[0], pos.AnchorPoint.X - r, pos.AnchorPoint.Y - r, r * 2, r * 2);
            }
            if(Parent != null)
            {
                foreach (var seg in Pairs)
                {
                    var p0 = Positions[seg.Start];
                    var p1 = Positions[seg.End];
                    var penIndex = seg.Pen == 0 ? 0 : colorIndex;
                    g.DrawLine(pens[penIndex], p0.AnchorPoint.X, p0.AnchorPoint.Y, p1.AnchorPoint.X, p1.AnchorPoint.Y);
                }
            }
            foreach (var child in Children)
            {
                child.Draw(pens, g, colorIndex + 1);
            }
        }

        public static TyloxShape CreateBoxx()
        {
            // Add angle and len for semantic reasons when you care, even if not needed.

            // reference attachments always have to attach to parent object in tree.
            // If you have an 'A', the cross has to be a sublayer of the upside down V
            // this maps to L being the same object, Y being a fork needing two paths, and X being two crossing objects
            // Also kind of maps to handwriting strokes, and helps the idea of roughing things in with upper layer,
            // and increasing level of detail as you go down the tree.
            // could even have lower layers do detail like serifs or texture, with a different system for different level of detail.

            var positions = TyloxPoint.CreatePointList(0.1f, 0.1f, 0.9f, 0.1f, 0.9f, 0.9f, 0.1f, 0.9f); // TL TR BR BL
            var pairs = TyloxPair.CreatePairList(0,1,0,  0,3,0, 1,2,0, 3,2,0); // TLRB
            var root = new TyloxShape(positions, pairs);

            var Atop = new TyloxJoint(0.5f, 0, 0, 0f, 0f);
            var Abl = new TyloxJoint(3.1f, 0, 0, 0f, 0f); 
            var Abr = new TyloxJoint(3.9f, 0, 0, 0f, 0f);
            var APairs = TyloxPair.CreatePairList(0, 1, 1,   0, 2, 1); //   / \ 
            var level1 = new TyloxShape(new TyloxPosition[] { Atop, Abl, Abr }, APairs);
            root.AddChild(level1);

            var ACrossLeft = new TyloxJoint(0.6f, 0, 0, 0.5f, 0f);
            var ACrossRight = new TyloxJoint(1.6f, 0, 0, -0.5f, 0f);
            var ACrossPairs = TyloxPair.CreatePairList(0, 1, 1); //   -
            var level2 = new TyloxShape(new TyloxPosition[] { ACrossLeft, ACrossRight }, ACrossPairs);
            level1.AddChild(level2);

            return root;
        }
        public static TyloxShape CreateBox()
        {
            // problem is skelton lines aren't mentally continuous, hmmmmmm
            var maxX = 0.7f;
            var positions = TyloxPoint.CreatePointList(0.1f, 0.1f, maxX, 0.1f, maxX, 0.9f, 0.1f, 0.9f); // TL TR BR BL
            var pairs = TyloxPair.CreatePairList(0,1,0, 1,0,0,  0,3,0, 3,1,0,  1,2,0, 2,3,0,  3,2,0, 2,0,0); // T-L-R-B-
            var root = new TyloxShape(positions, pairs);

            var top = new TyloxJoint(2.0f, 0, 0, 0f, 0f);
            var bl = new TyloxJoint(3.0f, 0, 0, 0f, 0f);
            var topB0 = new TyloxPoint(maxX - 0.2f, 0.1f);
            var topB1 = new TyloxPoint(maxX - 0.2f, 0.5f);
            var pairs1 = TyloxPair.CreatePairList(0,1,1,  1,2,0,  2,3,0); //  | / *
            var level1 = new TyloxShape(new TyloxPosition[] { top, bl, topB0, topB1 }, pairs1);
            root.AddChild(level1);

            var bTop0 = new TyloxJoint(0.0f, 0, 0, 0.5f, 0.5f);
            var bTop1 = new TyloxJoint(2.5f, 0, 0, 0.5f, 0.5f);
            var bTop2 = new TyloxJoint(0.5f, 0, 0, 0.5f, 0.5f);
            var bBot0 = new TyloxJoint(0.5f, 0, 0, 0.5f, 0.6f);
            var bBot1 = new TyloxJoint(0.75f, maxX - 0.1f, 0, 0.5f, 0.6f);
            var bBot2 = new TyloxJoint(1.0f, 0, 0, 0.5f, 0.6f);
            var pairs2 = TyloxPair.CreatePairList(0,1,1, 1,2,1, 2,3,1, 2,3,1, 3,4,1, 4,5,1); //   -
            var level2 = new TyloxShape(new TyloxPosition[] { bTop0, bTop1, bTop1, bBot0, bBot1, bBot2 }, pairs2);
            level1.AddChild(level2);

            return root;
        }
    }



    public class TyloxRenderer
    {
        public int Width { get; set; } = 250;
        public int Height { get; set; } = 250;
        List<TyloxShape> Shapes = new List<TyloxShape>();

        public TyloxRenderer()
        {
            Shapes.Add(TyloxShape.CreateBox());
        }
        public void Draw(float scale, Graphics g)
        {
            if (Pens.Count == 0)
            {
                GenPens(scale);
            }
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var shape in Shapes)
            {
                shape.Draw(Pens, g, 0);
            }
        }

        private List<Pen> Pens { get; } = new List<Pen>();
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
            Pens.Add(GetPen(Color.Gray, 2f / scale));
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

    //public class ITyloxAnchor 
    //{
    //    // Point, line. Jagged line, curvy line, repeating point, stacking container, mirrored points. Oval, rectangle, triangle.
    //}
    //public class TyloxAnchorLine : ITyloxAnchor
    //{
    //    PointF Point0 { get; set; }
    //    PointF Point1 { get; set; }
    //    public TyloxAnchorLine(float x0 = 0, float y0 = 0, float x1 = 1, float y1 = 0)
    //    {
    //        Point0 = new PointF(x0, y0);
    //        Point1 = new PointF(x1, y1);
    //    }
    //}
    //public class TyloxAnchorRect : ITyloxAnchor
    //{
    //    PointF Point0 { get; set; }
    //    PointF Point1 { get; set; }
    //    public TyloxAnchorRect(float x0 = 0, float y0 = 0, float x1 = 1, float y1 = 1)
    //    {
    //        Point0 = new PointF(x0, y0);
    //        Point1 = new PointF(x1, y1);
    //    }
    //}
    //public class TyloxAnchorOval : ITyloxAnchor
    //{
    //    PointF Center { get; set; }
    //    float Radius { get; set; }
    //    public TyloxAnchorOval(float cx = 0, float cy = 0, float r = 1)
    //    {
    //        Center = new PointF(cx, cy);
    //        Radius = r;
    //    }
    //}
}
