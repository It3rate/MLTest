using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Utilities;

namespace MLTest.Vis
{
    public class VisStroke : VisElement, IPath
    {
	    public override VisElementType ElementType => VisElementType.Stroke;

        public List<VisJoint> Joints { get; } = new List<VisJoint>();

	    public VisJoint Start => Joints[0];
	    public VisJoint End => Joints[Joints.Count - 1];
	    public override float Length() => 0;

	    public override Point Anchor => Start.Source.Anchor;

	    public VisStroke(VisJoint first, VisJoint second, params VisJoint[] remaining)
	    {
		    Joints.Add(first);
		    Joints.Add(second);
		    Joints.AddRange(remaining);
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

	    public Point GetPoint(float position, float offset)
	    {
		    throw new NotImplementedException();
	    }

	    public Point GetPointFromCenter(float centeredPosition, float offset)
	    {
		    throw new NotImplementedException();
	    }
    }
}
