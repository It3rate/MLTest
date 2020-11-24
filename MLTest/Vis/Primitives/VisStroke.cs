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
        public bool IsImagined { get; }

	    public VisJoint Start { get => Joints[0]; }
	    public VisJoint End { get => Joints[Joints.Length - 1]; }
	    public override float Length { get; }

	    public override VisPoint AnchorPoint => Start.AnchorPoint;

	    public VisStroke(bool isImagined, params VisJoint[] joints)
	    {
		    Debug.Assert(joints.Length > 1);
		    IsImagined = isImagined;
		    Joints = joints;
		    Length = 1f;
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
