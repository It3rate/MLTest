using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimJoint
    {
        public SimStroke Reference { get; set; } // need to specify start end (maybe use SimWhere?)

        public SimSpot CurveAmount { get; set; } // negative ), positive (, straight line is 0.

        public SimSpot Position { get; set; }
        public SimSpot Offset { get; set; }   // percent (or abs?) cx,cy of the peice segment is from the left (negative) or right (positive) of the start end line
        public SimSpot CrossSlide { get; set; } // x for default line of origin to 0,1
        public SimAngle Angle { get; set; }    // angle of intersection with endpoint. Set speed to zero if don't care
        public SimSpot Speed { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)
    }
}
