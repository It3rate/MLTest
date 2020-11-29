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

        public List<VisNode> Nodes { get; } = new List<VisNode>();

        public override Point Anchor => StartNode.Anchor;
        public override float Length { get; }

        public VisStroke(VisNode first, VisNode second, params VisNode[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
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

	    public VisNode NodeAt(float position) => new VisNode(this, position);
	    public VisNode NodeAt(float position, float offset) => new VisTipNode(this, position, offset);
	    public VisNode StartNode => new VisNode(this, 0f);
	    public VisNode MidNode => new VisNode(this, 0.5f);
	    public VisNode EndNode => new VisNode(this, 1f);
	    public VisStroke FullStroke => new VisStroke(StartNode, EndNode);
	    public VisStroke Stroke(float start, float end) => new VisStroke(NodeAt(start), NodeAt(end));
    }
}
