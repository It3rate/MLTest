using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimNode
    {
        public SimStroke Reference { get; set; } // if reference is null, this is skeleton point, position is x, offset is y
        public SimSection Position { get; set; }
        public SimSection Offset { get; set; }   // perpendicular distance from line of reference at intersection point

        public PointF AnchorPoint { get; }

        public SimNode(SimStroke reference, SimSection position, SimSection offset)
        {
            Reference = reference;
            Position = position;
            Offset = offset;
            if(Reference == null)
            {
                AnchorPoint = new PointF((float)Position.Exact, (float)Offset.Exact);
            }
            else
            {
                AnchorPoint = Reference.GetPointOnLine(Position.Exact, Offset.Exact);
            }
        }
        public SimNode(SimStroke reference, double position, double offset = 0) : this(reference, new SimSection(position), new SimSection(offset))
        {
        }

        public virtual bool IsTip => true;

        public virtual double TouchLikelihood(double x)
        {
            // need to account for offset, that is probably job of joint though
            // x should already be extended from comaparing stroke to touch this section.
            return Position.Likelihood(x);
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

        public SimAngle Angle { get; set; }    // angle of intersection with endpoint. Set speed to zero if don't care
        public SimSection Speed { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)

        public SimEdge(SimStroke reference, double position, double offset = 0, double curve = 0, double cross = 0) : base(reference, position, offset)
        {
            CurveAmount = new SimSection(curve);
            CrossSlide = new SimSection(cross);
            _isTip = Position.Likelihood(0) > 0.01 || Position.Likelihood(1) > 0.01; // todo: account for offset
        }

    }

    // A line, but can also be a more complex bezier, like an S curve or more.
    // Can also be constructed when wanting to focus on part of an object, like the loop part of a 'P'.
    // We don't overtly care about stroke order (unless it matters while creating) because the stroke is always evaluated on inspection
    public class SimStroke
    {
        public SimNode Start { get; set; }
        public SimNode End { get; set; }
        public List<SimEdge> Edges { get; set; } = new List<SimEdge>(); // straight line if empty

        public double StartAngle { get; protected set; }
        public double EndAngle { get; protected set; }
        public SimStroke(SimNode start, SimNode end, params SimEdge[] grooves)
        {
            Start = start;
            End = end;
            Edges.AddRange(grooves);

            float xDif = Start.AnchorPoint.X - End.AnchorPoint.X;
            float yDif = Start.AnchorPoint.Y - End.AnchorPoint.Y;

            float ang = (float)(Math.Atan2(yDif, xDif)); // (- Math.PI / 2.0) so zero is up?
            StartAngle = NormalizeRadians(ang);
            EndAngle = NormalizeRadians(ang + Math.PI);
        }

        public PointF GetPointOnLine(double position, double offset)
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
            return new PointF(
                sp.X + (ep.X - sp.X) * (float)position + xOffset, 
                sp.Y + (ep.Y - sp.Y) * (float)position + yOffset);
        }

        private static double TWO_PI = 2 * Math.PI;
        public double NormalizeRadians(double radians)
        {
            double normalized = radians % TWO_PI;
            normalized = (normalized + TWO_PI) % TWO_PI;
            return normalized <= Math.PI ? normalized : normalized - TWO_PI;
        }
    }
}
