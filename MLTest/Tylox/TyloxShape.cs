using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using PointIndex = System.Int32;
using ShapeIndex = System.Int32;

namespace MLTest.Tylox
{

    public abstract class TyloxPosition
    {
        protected static PointIndex _indexCounter = 0;
        public PointF Point { get; set; }
    }
    public class TyloxPoint : TyloxPosition
    {
        public PointIndex Id { get; }

        public TyloxPoint(float x, float y) 
        {
            Id = _indexCounter++;
            Point = new PointF(x, y);
        }
        public static TyloxPositions CreatePointList(float x, float y, params float[] points)
        {
            var result = new List<TyloxPosition>();
            result.Add(new TyloxPoint(x, y));
            for (int i = 0; i < points.Length; i+=2)
            {
                result.Add(new TyloxPoint(points[i], points[i + 1]));
            }
            return new TyloxPositions(result.ToArray());
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
    }
    public class TyloxPositions
    {
        List<TyloxPosition> Positions { get; set; }
        public TyloxPositions(params TyloxPosition[] positions)
        {
            Positions = new List<TyloxPosition>(positions);
        }
        public TyloxPosition this[int index] => Positions[index];

        public void CalculatePoints(TyloxShape parent)
        {
            foreach (var pos in Positions)
            {
                if(pos is TyloxJoint joint)
                {
                    joint.Point = parent.GetPositionOnPath(joint.Position);
                }
            }
        }
    }
    public struct TyloxPair
    {
        public PointIndex Start { get; }
        public PointIndex End { get; }
        public TyloxPair(PointIndex start, PointIndex end)
        {
            Start = start;
            End = end;
        }
        public static List<TyloxPair> CreatePairList(PointIndex start, PointIndex end, params PointIndex[] pairs)
        {
            var result = new List<TyloxPair>();
            result.Add(new TyloxPair(start, end));
            for (int i = 0; i < pairs.Length; i += 2)
            {
                result.Add(new TyloxPair(pairs[i], pairs[i + 1]));
            }
            return result;
        }
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
        List<TyloxShape> Children { get; }
        private TyloxPositions Positions { get; }
        private List<TyloxPair> Pairs { get; }

        public TyloxShape(TyloxPositions positions, List<TyloxPair> pairs)
        {
            Id = _indexCounter++;
            Positions = positions;
            Pairs = pairs;
        }
        public PointF GetPositionOnPath(float index)
        {
            int segIndex = (int)index;
            float segPos = index % 1;
            int last = Pairs.Count - 1;
            var seg = (segIndex < 0) ? Pairs[0] : (segIndex > last) ? Pairs[last] : Pairs[segIndex];
            var sp = Positions[seg.Start].Point;
            var ep = Positions[seg.End].Point;
            return new PointF(sp.X + (ep.X - sp.X) * segPos, sp.Y + (ep.Y - sp.Y) * segPos);
        }

        public void AddChild(TyloxShape child)
        {
            Children.Add(child);
            child.Parent = this;
            child.Positions.CalculatePoints(this);
        }
        private static TyloxShape CreateBox()
        {
            // Add angle and len for semantic reasons when you care, even if not needed.

            // reference attachments always have to attach to parent object in tree.
            // If you have an 'A', the cross has to be a sublayer of the upside down V
            // this maps to L being the same object, Y being a fork needing two paths, and X being two crossing objects
            // Also kind of maps to handwriting strokes, and helps the idea of roughing things in with upper layer,
            // and increasing level of detail as you go down the tree.
            // could even have lower layers do detail like serifs or texture, with a different system for different level of detail.

            var positions = TyloxPoint.CreatePointList(0.1f, 0.1f, 0.9f, 0.1f, 0.9f, 0.9f, 0.1f, 0.9f); // TL TR BR BL
            var pairs = TyloxPair.CreatePairList(0,1,0,3,1,2,3,2); // TLRB
            var root = new TyloxShape(positions, pairs);

            var Atop = new TyloxJoint(0.5f, 0, 0, 0f, 0f);
            var Abl = new TyloxJoint(3.2f, 0, 0, 0f, 0f); 
            var Abr = new TyloxJoint(3.8f, 0, 0, 0f, 0f);
            var APairs = TyloxPair.CreatePairList(0, 1, 0, 2); //   / \ 
            var level1 = new TyloxShape(new TyloxPositions(Atop, Abl, Abr), APairs);
            root.AddChild(level1);

            var ACrossLeft = new TyloxJoint(0.3f, 0, 0, 0.5f, 0f);
            var ACrossRight = new TyloxJoint(1.3f, 0, 0, -0.5f, 0f);
            var ACrossPairs = TyloxPair.CreatePairList(0, 1); //   -
            var level2 = new TyloxShape(new TyloxPositions(ACrossLeft, ACrossRight), APairs);
            level1.AddChild(level2);

            return root;
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
