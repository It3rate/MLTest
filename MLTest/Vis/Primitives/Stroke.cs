using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Utilities;

namespace MLTest.Vis
{
    public class Stroke : VisElement, IPath
    {
	    public override VisElementType ElementType => VisElementType.Stroke;

        public List<Node> Nodes { get; } = new List<Node>();

        public override Point Anchor => StartNode.Anchor;
        public override float Length { get; }

        public Stroke(Node first, Node second, params Node[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
	    }

        public void Flip(){ }
	    public Stroke OrientedClone() => null;
	    public override float CompareTo(VisElement element) => 0;
	    public bool BoundsOverlaps(Stroke stroke) => false;
	    public float DistanceTo(Stroke stroke, out float position, out float targetPosition)
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

	    public Node NodeAt(float position) => new Node(this, position);
	    public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
	    public Node StartNode => new Node(this, 0f);
	    public Node MidNode => new Node(this, 0.5f);
	    public Node EndNode => new Node(this, 1f);
	    public Stroke FullStroke => new Stroke(StartNode, EndNode);
	    public Stroke PartialStroke(float start, float end) => new Stroke(NodeAt(start), NodeAt(end));
    }
}
