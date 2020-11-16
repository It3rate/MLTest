using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimJointSalience
    {
        public SimPoint Reference { get; }

        public enum JointFeatures { Reference = 0, Position, Offset, CrossSlide, CurveAmount, Angle, Speed }
        // list of which features matter - used for defining different types of joints (e.g. we don't care if a butt joins curved or straight lines, like F or P or R)
        // These map to the JointFeature positions.
        public double[] FeatureActivations = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
    }

    /// <summary>
    /// Defines segment connections and lines. There aren't TYLOX style joints explicitly defined because they are inferred by probability.
    /// </summary>
    public class SimJoint
    {
        // Two segments become a joint automatically if listed here. The join area of each segment is the places in the line. 
        // So a corner is two endpoints, a butt is an endPoint and a mid section, and a cross is two midsections. All this is inferred and differentiable. 
        // So joints types are never specified, they are probabalistically inferred.
        public List<SimJointSalience> References { get; set; } // Usually 2 segments, but can be 3 or more (eg in Y branch). If for motion, first is anchor, second moves to it?

        //public SimPointOnSegment Reference { get; set; } // need to specify start end (maybe use SimWhere?)

        //public SimSpot CurveAmount { get; set; } // concave [.( is -1<n<0, convex [.) 0<n<1, straight line --- is 0, closed O--- or ---O is -1 (center outside) or 1 (center inside).
        //public SimSpot Position { get; set; }
        //public SimSpot Offset { get; set; }   // percent (or abs?) cx,cy of the peice segment is from the left (negative) or right (positive) of the start end line
        //public SimSpot CrossSlide { get; set; } // x for default line of origin to 0,1
        //public SimAngle Angle { get; set; }    // angle of intersection with endpoint. Set speed to zero if don't care
        //public SimSpot Speed { get; set; }   // inertia of intersection connection ( probably sets cubic bezier endpoint with angle)


        // Create a joint using the difference between this joint and a second one.
        // If expected and acutal are quite different, this would be the repair.
        public SimJoint GetJointDiff(SimJoint joint) => null;

        // How close is this exact joint to an ideal joint of the given type.
        public double GetJointProbability(JointType type) => 0;

        // smooth joint follows along a segment, defines location, curvature, offset, speed, angle that the segment needs at that point.
        // Also can define the way two tips from separate segments join (e.g. 'ST' at the join is continuous)
        public enum JointType { Smooth, Tip, Corner, Butt, Cross, Loop, Branch}
    }
}
