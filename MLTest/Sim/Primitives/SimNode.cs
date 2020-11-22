using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public abstract class SimElement
    {
        public virtual double CompareIsPartial(SimElement element)
        {
            // Check how closely the passed object's parts resemble the best-coresponding parts of this one.
            return 0;
        }
        public virtual double CompareTo(SimElement element)
        {
            // can compare different element types, so weight things based on how similar out of how many possible elements.
            // eg 2 rects are similar on three sides, that is 3/4 similar and 1/4 less similar. A line and a shape is treated in the same way.
            // another option as things get complex is to low res render both, and compare the bitmaps.

            // Account for: bounds, positions, joint types/locations, enclosedness, curviness
            return 0;
        }
        public virtual double CompareToConceptual(SimElement element)
        {
            // Joints relative to each other. Even just similar joint types is a relation (triangle, square, twisted/concave square)
            // Account for shape, scale/rotation/translation/flip/squish/skew, pointyness, complexity, stability, variation. 
            // Later: motion, momentum, friction, rigidity, count, risk, plan, emotion etc.
            return 0;
        }
    }

    //public enum SimPrimitiveType { Dot, Line, TwoLine, RightTriangle, Triangle, Square, Rectangle, Circle, Oval, Arc, Polyline, ConvexShape, ComplexShape }
    public enum SimPrimitiveType { Triangle, Rectangle, Oval }
    // two points define a line (maybe this is always a rect with zero thickness? No, because it is part of a rect, triangle etc) (maybe an arc with zero curve?)
    // maybe dots, lines and polylines+ shouldn't be primitives? Seems right. Maybe merged primitive made of multiple basic shapes.
    // three points can define a triangle, rect, oval (including start angle), arc

    public class SimPrimitive : SimElement // maybe this is a stroke or a shape... Whatever the minimum reference is? Or only used when imagining strokes, so stays primitive and only used to generate reference nodes/strokes.
    {
        public SimPrimitiveType PrimitiveType { get; }
        public SimStructuralType StructuralType { get => SimStructuralType.PadStructure; }
        PointF P0 { get; }
        PointF P1 { get; }
        PointF P2 { get; }

        /// methods to get points, corners, lines, arcs, tangents etc from imagined element
    }

    public class SimNode : SimElement
    {
        public SimStroke Reference { get; set; } // if reference is null, this is skeleton point, position is x, offset is y
        public SimSection Position_X { get; set; }
        public SimSection Offset_Y { get; set; }   // perpendicular distance from line of reference at intersection point

        public PointF AnchorPoint { get; }

        public SimNode(SimStroke reference, SimSection position, double offset)
        {
            Reference = reference;
            Position_X = position;
            offset = reference == null ? offset : offset * reference.Length();
            Offset_Y = new SimSection(offset);
            if (Reference == null)
            {
                AnchorPoint = new PointF((float)Position_X.Exact, (float)Offset_Y.Exact);
            }
            else
            {
                AnchorPoint = Reference.GetPointOnLine(Position_X.Exact, Offset_Y.Exact);
            }
        }
        public SimNode(SimStroke reference, double position, double offset = 0) : this(reference, new SimSection(position), offset) { }

        public virtual bool IsTip => true;
        public double DistanceTo(SimNode node) => Math.Sqrt((AnchorPoint.X - node.AnchorPoint.X) * (AnchorPoint.X - node.AnchorPoint.X) + (AnchorPoint.Y - node.AnchorPoint.Y) * (AnchorPoint.Y - node.AnchorPoint.Y));

        public virtual double TouchLikelihood(double x)
        {
            // need to account for offset, that is probably job of joint though
            // x should already be extended from comaparing stroke to touch this section.
            return Position_X.Likelihood(x);
        }

        public override double CompareTo(SimElement element)
        {
            var result = 0.0; // default if isn't Node, for now
            // anchor only 
            if(element is SimNode node)
            {
                var dist = node.DistanceTo(this);
                result = Position_X.Likelihood(dist/2.0);
            }
            return result;
        }
        public override string ToString()
        {
            return "Node[" + AnchorPoint.X + "," + AnchorPoint.Y + "]";
        }
    }

    public class SimEdge : SimNode
    {
        bool _isTip;
        public override bool IsTip => _isTip;

        // based on segment start/end that this edge is part of.
        //public SimSection Position { get; set; }
        //public SimSection Offset { get; set; }   // perpendicular distance from line of reference at intersection point
        public SimSection CurveAmount { get; set; }// concave [.( is -1<n<0, convex [.) 0<n<1, straight line --- is 0, closed O--- or ---O is -1 (center outside) or 1 (center inside).
        public SimSection CrossSlide { get; set; } // needed? how much of the curve is at the top vs the bottom (based on segment)

        public SimSection Angle { get; set; }    // abs angle of intersection with endpoint. Set speed to zero if don't care
        public SimSection Spread { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)

        public PointF Anchor0 { get; set; }
        public PointF Anchor1 { get; set; }

        public SimEdge(SimStroke reference, double position, double offset = 0, double spread = 1, double angle = 0, double curve = 0, double cross = 0) : base(reference, position, offset)
        {
            CurveAmount = new SimSection(curve);
            CrossSlide = new SimSection(cross);
            Angle = new SimSection(angle);
            spread = reference == null ? spread : spread * reference.Length();
            Spread = new SimSection(spread);
            _isTip = Position_X.Likelihood(0) > 0.01 || Position_X.Likelihood(1) > 0.01; // todo: account for offset
            //SetCurveAnchors();
        }

        public double AngleSimilarity(SimEdge edge) => edge.Angle.Likelihood(Angle.Exact);

        public void SetCurveAnchors()
        {
            var negLen = Spread.Exact * ((CrossSlide.Exact / 2.0) + 0.5);
            var posLen = Spread.Exact - negLen;
            var ang = SimUtils.NormalizedToRadians(Angle.Exact);
            var x = AnchorPoint.X;
            var y = AnchorPoint.Y;
            var xNeg = (float)(-Math.Sin(ang) * negLen);
            var yNeg = (float)(Math.Cos(ang) * negLen);
            var xPos = (float)(-Math.Sin(ang) * posLen);
            var yPos = (float)(Math.Cos(ang) * posLen);
            Anchor0 = new PointF(x - xNeg, y - yNeg);
            Anchor1 = new PointF(x + xPos, y + yPos);
        }

        public override double CompareTo(SimElement element)
        {
            // anchor, angle, curve, isTip 
            // if not tip, cross, pos down, offset
            var result = 0.0; // default if isn't Edge, for now
            if (element is SimEdge edge)
            {
                var dist = edge.DistanceTo(this);
                result = (Position_X.Likelihood(dist) + AngleSimilarity(edge)) / 2.0;
            }
            return result;
        }
        public override string ToString()
        {
            return "Edge[" + AnchorPoint.X + "," + AnchorPoint.Y + "]";
        }
    }
}
