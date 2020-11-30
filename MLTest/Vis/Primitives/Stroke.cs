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

        private List<Node> Nodes { get; } = new List<Node>();

        public override Point Anchor => StartNode.Anchor;
        public override float Length { get; }
        public Point StartPoint => StartNode.Anchor;
        public Point MidPoint => GetPoint(0.5f, 0);
        public Point EndPoint => EndNode.Anchor;
        public Point Center => MidPoint; // Will be the center of the bounds once that is calculated

        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();


        public Stroke(Node first, Node second, params Node[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
	    }

        private void GenerateSegments()
        {
	        Segments.Clear();
			Node lastNode = Nodes[0];
	        for (var i = 1; i < Nodes.Count; i++)
	        {
		        var curNode = Nodes[i];
		        Point curPoint;
	            IPrimitivePath curPath;
		        if (curNode is TangentNode tanNode)
		        {
			        curPath = Line.ByEndpoints(lastNode.Start, curNode.End);
                }
                else if (curNode is TipNode tipNode)
		        {
			        curPath = Line.ByEndpoints(lastNode.Start, curNode.End);
                }
		        else
		        {
			        curPath = Line.ByEndpoints(lastNode.Start, curNode.End);
		        }

                Segments.Add(curPath);
		        lastNode = curNode;
	        }
        }

        public void AddNode(Node node)
        {
            Nodes.Add(node);
            GenerateSegments();
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
            // go by length once that is running
            var ipos = position * Nodes.Count - 1;
            // should circles use ARCs when they are segments? 
            return StartPoint;
	    }

	    public Point GetPointFromCenter(float centeredPosition, float offset)
	    {
		    throw new NotImplementedException();
	    }

	    public Node NodeAt(float position) => new Node(this, position);
	    public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
	    public Node StartNode => Nodes[0];
	    public Node MidNode => new Node(this, 0.5f);
	    public Node EndNode => Nodes[Nodes.Count - 1];
        public Stroke FullStroke => new Stroke(StartNode, EndNode);
	    public Stroke PartialStroke(float start, float end) => new Stroke(NodeAt(start), NodeAt(end));
    }
}
