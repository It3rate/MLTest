using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimPoint
    {
        public SimStroke Reference { get; set; }
        public SimZone Position { get; set; }
        public SimZone Offset { get; set; }   // perpendicular distance from line of reference at intersection point
    }

    public class SimGroove
    {
        // based on segment start/end that this groove is part of.
        public SimZone Position { get; set; }
        public SimZone Offset { get; set; }   // perpendicular distance from line of reference at intersection point
        public SimZone CurveAmount { get; set; }// concave [.( is -1<n<0, convex [.) 0<n<1, straight line --- is 0, closed O--- or ---O is -1 (center outside) or 1 (center inside).
        public SimZone CrossSlide { get; set; } // needed? how much of the curve is at the top vs the bottom (based on segment)

        public SimAngle Angle { get; set; }    // angle of intersection with endpoint. Set speed to zero if don't care
        public SimZone Speed { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)

    }

    // A line, but can also be a more complex bezier, like an S curve or more.
    // Can also be constructed when wanting to focus on part of an object, like the loop part of a 'P'.
    // We don't overtly care about stroke order (unless it matters while creating) because the stroke is always evaluated on inspection
    public class SimStroke
    {
        public SimPoint Start { get; set; }
        public SimPoint End { get; set; }
        public List<SimGroove> Grooves { get; set; } = new List<SimGroove>(); // straight line if empty

    }
}
