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
        public SimNode Start { get; set; }
        public SimNode End { get; set; }
        public List<SimEdge> Edges { get; set; } = new List<SimEdge>(); // straight line if empty

        public SimNode[] EndPoints { get; private set; }
        public SimNode[] AllElements { get; private set; }
        public PointF Center { get; private set; }

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

            SetAccessArrays();
            SetCenter();
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
        public SimStroke GetOrientedClone()
        {
            var result = Clone();
            if(result.Start.AnchorPoint.Y > result.End.AnchorPoint.Y)
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

        public override double CompareTo(SimElement element)
        {
            var result = 0.0; // default if isn't Node, for now
            // has Edges? Edges: Start/End
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
