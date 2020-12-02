using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    // A line, but can also be a more complex bezier, like an S curve or more.
    // Can also be constructed when wanting to focus on part of an object, like the loop part of a 'P'.
    // We don't overtly care about stroke order (unless it matters while creating) because the stroke is always evaluated on inspection
    public class SimStroke : SimElement
    {
        // A circle can be defined with two nodes, a start (center) and an end (radius). May need a draw direction? Or not if always linked by nearness and tangent.
        // For a 'C', define a circle, then a start and end point on it (.85 to .65) and an edge/node on the left side of the circle (so it knows to go ccw).
        // May be able to get rid of edges completely for skeleton layer (all joints are straight lines or on tangent circles).
        public SimNode Start { get; set; }
        public SimNode End { get; set; }
        public List<SimEdge> Edges { get; set; } = new List<SimEdge>(); // straight line if empty

        public List<SimJointAttempt> JointAttempts { get; } = new List<SimJointAttempt>();

        public SimNode[] EndPoints { get; private set; }
        public SimNode[] AllElements { get; private set; }
        public PointF Center { get; private set; }

        public double StartAngle { get; protected set; }
        public double EndAngle { get; protected set; }

        public SimBezier Bezier { get; }

        public SimStroke(SimNode start, SimNode end, params SimEdge[] grooves)
        {
            Start = start;
            End = end;
            Edges.AddRange(grooves);

            float xDif = Start.AnchorPoint.X - End.AnchorPoint.X;
            float yDif = Start.AnchorPoint.Y - End.AnchorPoint.Y;

            float ang = (float)(Math.Atan2(yDif, xDif)); // (- Math.PI / 2.0) so zero is up?
            StartAngle = Utils.RadiansToNorm(ang);
            EndAngle = Utils.RadiansToNorm(ang + Math.PI);

            SetAccessArrays();
            SetCenter();

            SetCurvePoints();

            Bezier = new SimBezier(GetBezierPoints());
        }

        // needs to change to MaximumExtent - largest dimension of bounding box, or furthest two points? Or shape's anchor stroke or *bounds*?
        // Offset is relative to length, but a circle is long compared to how it 'looks'
        public double Length() => Start.DistanceTo(End);

        private void SetCurvePoints()
        {
            if(Edges.Count > 0)
            {
                var p0 = Start.AnchorPoint;
                for (int i = 0; i < Edges.Count; i++)
                {
                    var c = Edges[i].AnchorPoint;
                    var p1 = Edges.Count > i + 1 ? Edges[i + 1].AnchorPoint : End.AnchorPoint;
                    var pol = c.ProjectedOntoLine(p0, p1);

                    var dif = pol.Subtract(c);

                    var a0 = p0.Add(dif);// new PointF(c.X + (pol.X - p1.X), c.Y + (pol.Y - p1.Y));
                    var a1 = p1.Add(dif);//new PointF(c.X - (pol.X - p1.X), c.Y - (pol.Y - p1.Y));

                    Edges[i].Anchor0 = a0;// p0.ProjectedOntoLine(a0, a1);// new PointF(c.X - (pol.X - p1.X), c.Y + (pol.Y - p1.Y));
                    Edges[i].Anchor1 = a1;// p1.ProjectedOntoLine(a0, a1);// new PointF(c.X + (pol.X - p1.X), c.Y - (pol.Y - p1.Y));
                    p0 = c;
                }
            }
        }

        private void SetAccessArrays()
        {
            EndPoints = new SimNode[] { Start, End };

            var elms = new List<SimNode>();
            elms.Add(Start);
            elms.AddRange(Edges);
            elms.Add(End);
            AllElements = elms.ToArray();
        }
        private void SetCenter()
        {
            double x = 0;
            double y = 0;
            foreach (var element in AllElements)
            {
                x += element.AnchorPoint.X;
                y += element.AnchorPoint.Y;
            }
            Center = new PointF((float)(x / (double)AllElements.Length), (float)(y / (double)AllElements.Length));
        }
        public PointF[] Anchors
        {
            get
            {
                var anchors = new List<PointF>();
                anchors.Add(Start.AnchorPoint);
                foreach (var edge in Edges)
                {
                    anchors.Add(edge.AnchorPoint);
                }
                anchors.Add(End.AnchorPoint);
                return anchors.ToArray();
            }
        }

        public PointF[] GetBezierPoints()
        {
            List<PointF> pts = new List<PointF>();

            pts.Add(Start.AnchorPoint);
            if (Edges.Count > 0)
            {
                PointF p0 = Start.AnchorPoint;
                for (int i = 0; i < Edges.Count; i++)
                {
                    SimEdge edge = Edges[i];

                    PointF a0 = edge.Anchor0;
                    var mid0 = p0.MidPoint(a0);
                    PointF a1 = edge.Anchor1;
                    var mid1 = a0.MidPoint(a1);
                    pts.AddRange(new[] { mid0, a0, edge.AnchorPoint });

                    p0 = (i < Edges.Count - 1) ? Edges[i + 1].Anchor0 : End.AnchorPoint;
                    var mid2 = a1.MidPoint(p0);
                    pts.AddRange(new[] { a1, mid2, p0 });
                }
            }
            else
            {
                pts.Add(End.AnchorPoint);
            }
            return pts.ToArray();
        }

        /// <summary>
        /// Clone and return stroke oriented top to bottom, or if approximately horizontal, left to right.
        /// </summary>
        public SimStroke GetOrientedClone()
        {
            var result = Clone();
            var start = result.Start.AnchorPoint;
            var end = result.End.AnchorPoint;
            // this needs to eventually use the bounds scale of the stroke being tested
            if (Utils.LikelyLess(end.Y, start.Y))
            {
                result.FlipEnds();
            }
            else if (Utils.LikelyEqual(start.Y, end.Y) && Utils.LikelyLess(end.X, start.X))
            {
                result.FlipEnds();
            }
            return result;
        }
        public void FlipEnds()
        {
            var temp = Start;
            Start = End;
            End = temp;
            Edges.Reverse();
        }
        public PointF GetPointOnLine(double position, double offset)
        {
            PointF result;
            if(Edges.Count == 0)
            {
                var sp = Start.AnchorPoint;
                var ep = End.AnchorPoint;
                float xOffset = 0;
                float yOffset = 0;
                float xDif = ep.X - sp.X;
                float yDif = ep.Y - sp.Y;
                if (offset != 0)
                {
                    float ang = (float)(Math.Atan2(yDif, xDif));
                    xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
                    yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
                }
                result = new PointF(
                    sp.X + xDif * (float)position + xOffset, 
                    sp.Y + yDif * (float)position + yOffset);
            }
            else
            {
                result = Bezier.GetPointAtT(position);
            }
            return result;
        }
        public PointF GetJointOnLine(SimJoint joint)
        {
            return PointF.Empty;
        }


        public override double CompareTo(SimElement element)
        {
            var result = 0.0; // default if isn't Node, for now
            // has Edges? Edges: Start/EndPoint
            if (element is SimStroke stroke)
            {
                result = (stroke.Start.CompareTo(Start) + stroke.End.CompareTo(End)) / 2.0;
            }
            return result;
        }
        public SimStroke Clone()
        {
            return new SimStroke(Start, End, Edges.ToArray()); // clone these as well
        }
    }
}
