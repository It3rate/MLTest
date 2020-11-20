using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimBindPoint
    {
        public SimNode Reference { get; }

        // default to start point origin being the higher and left most (regardless of encoding of reference)
        // maybe make all strokes align the same way? Or if finding by node maybe it doesn't matter.
        public SimDirection OriginLocator { get; set; } = SimDirection.NW;

        public enum JointFeatures { Reference = 0, Position, Offset, CrossSlide, CurveAmount, Angle, Speed }
        // list of which features matter - used for defining different types of joints (e.g. we don't care if a butt joins curved or straight lines, like F or P or R)
        // These map to the JointFeature positions.
        public double[] FeatureActivations = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
    }

    // smooth joint follows along a segment, defines location, curvature, offset, speed, angle that the segment needs at that point.
    // Also can define the way two tips from separate segments join (e.g. 'ST' at the join is continuous)
    public enum JointType { Smooth, Tip, Corner, ButtInto, CutOff, Cross, Loop, SplitFrom}

    /// <summary>
    /// A stroke that attempts to create a joint encodes where it expects a joint to happen. Include where on current stroke, target stroke, target location, joint type, joint metadata.
    /// Maybe this is what a joint is actually.
    /// </summary>
    public class SimJointAttempt
    {
        public JointType JointType { get; }
        public SimStroke Source { get; } // might be shape too?
        public SimSection SourcePosition { get; }
        public SimStroke Target { get; } // might be shape too?
        public SimSection TargetPosition { get; }
        public SimSection JoiningAngle { get; }


        public SimJointAttempt(JointType jointType, SimStroke source, double sourcePosition, SimStroke target, double targetPosition, double joiningAngle = 0.5)
        {
            JointType = JointType;
            Source = source;
            SourcePosition = new SimSection(sourcePosition);
            Target = target;
            TargetPosition = new SimSection(targetPosition);
            JoiningAngle = new SimSection(joiningAngle);
        }
    }

    /// <summary>
    /// Defines segment connections and lines at the intersection. There aren't TYLOX style joints explicitly defined because they are inferred by probability.
    /// </summary>
    public class SimJoint
    {
        public JointType JointType { get; }
        // Two segments become a joint automatically if listed here. The join area of each segment is the places in the line. 
        // So a corner is two endpoints, a butt is an endPoint and a mid section, and a cross is two midsections. All this is inferred and differentiable. 
        // So joints types are never specified, they are probabalistically inferred.
        // joints are created dynamically, so the edge 0 is the connection point (correctly oriented)
        public List<SimEdge> References { get; } = new List<SimEdge>(); // Usually 2 segments, but can be 3 or more (eg in Y branch). If for motion, first is anchor, second moves to it?

        public SimJoint(JointType jointType, params SimEdge[] edges)
        {
            JointType = JointType;
            References.AddRange(edges);
        }

        // Create a joint using the difference between this joint and a second one.
        // If expected and acutal are quite different, this would be the repair.
        public SimJoint GetJointDiff(SimJoint joint) => null;

        // How close is this exact joint to an ideal joint of the given type.
        // find the intercept. Check the probability of that point on each joint being tested. Multiply those (?)
        public double GetJointProbability(JointType type)
        {
            double result = 0;
            switch (type)
            {
                case JointType.Smooth:
                    break;
                case JointType.Tip:
                    break;
                case JointType.Corner:
                    break;
                case JointType.ButtInto:
                    break;
                case JointType.Cross:
                    break;
                case JointType.Loop:
                    break;
                case JointType.SplitFrom:
                    break;
        }
            return result;
        }

    }
}
