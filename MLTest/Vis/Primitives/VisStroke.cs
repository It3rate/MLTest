using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Utilities;

namespace MLTest.Vis
{
    public class VisStroke : VisElement
    {
	    public override VisElementType ElementType => VisElementType.Stroke;

        public VisJoint[] Joints { get; }

	    public VisJoint Start => Joints[0];
	    public VisJoint End => Joints[Joints.Length - 1];
	    public override float Length() => 0;

	    public override Point Anchor => Start.Source.Anchor;

	    public VisStroke( params VisJoint[] joints)
	    {
		    Debug.Assert(joints.Length > 1);
		    Joints = joints;
	    }

	    public void Flip(){ }
	    public VisStroke OrientedClone() => null;
	    public override float CompareTo(VisElement element) => 0;
	    public bool BoundsOverlaps(VisStroke stroke) => false;
	    public float DistanceTo(VisStroke stroke, out float position, out float targetPosition)
	    {
		    position = 0;
		    targetPosition = 0;
		    return 0;
	    }

	    public float LikelyVertical { get; }
	    public float LikelyHorizontal { get; }
	    public float LikelyDiagonalUp { get; }
	    public float LikelyDiagonalDown { get; }
    }
}
