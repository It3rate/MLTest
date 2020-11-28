using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;

namespace MLTest.Vis
{
    public interface IJoiner{}

    public class VisJoint : IJoiner
    {
	    //public override VisElementType ElementType => VisElementType.Joint;

	    //public override float Length() => 0;
	    //public override Point Anchor => Target.Anchor;

	    public VisJointType JointType { get; }

	    public VisNode Source { get; }
	    public VisNode Target { get; }

	    // computed
	    public double JointAngle { get; }

	    public VisJoint(VisNode source, VisNode target, VisJointType jointType = VisJointType.Inferred)
	    {
		    Source = source;
		    Target = target;
		    JointType = jointType;
	    }
	    public VisJoint(VisNode source)
	    {
		    JointType = VisJointType.Tip;
		    Source = source;
	    }

        public static Gaussian TipProbability;
	    public static Gaussian ButtProbability;
	    public static Gaussian TangentProbability;
    }

	public enum VisJointType
	{
        Inferred,
        Tip,
        Line,
        Curve,
        Corner,
		Butt,
		Tangent,
		Cross,
	}
}