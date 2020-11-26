using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;

namespace MLTest.Vis
{

    public class VisJoint : VisElement
    {
	    public override VisElementType ElementType => VisElementType.Joint;

	    public override float Length() => 0;
	    public override VisPoint AnchorPoint => SourceLocation.AnchorPoint;

	    public VisJointType JointType { get; }

	    public VisNode SourceLocation { get; }
	    public VisNode TargetLocation { get; }

	    // computed
	    public double JointAngle { get; }

	    public VisJoint(VisNode source, VisNode target, VisJointType jointType = VisJointType.Inferred)
	    {
		    SourceLocation = source;
		    TargetLocation = target;
		    JointType = jointType;
	    }

        public static Gaussian TipProbability;
	    public static Gaussian ButtProbability;
	    public static Gaussian TangentProbability;
    }

	public enum VisJointType
	{
        Inferred,
		Tip,
		Corner,
		Butt,
		Tangent,
		Cross,
	}
}