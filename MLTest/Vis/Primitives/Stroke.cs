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
        private float _length;
        public override float Length => _length;
        public Point StartPoint => StartNode.Anchor;
        public Point MidPoint => GetPoint(0.5f, 0);
        public Point EndPoint => EndNode.Anchor;
        public Point Center => MidPoint; // Will be the center of the bounds once that is calculated

        private List<Point> Anchors = new List<Point>();
        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();


        public Stroke(Node first, Node second, params Node[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
		    GenerateSegments();
	    }

        private void GenerateSegments()
        {
	        Segments.Clear();
            Anchors.Clear();
            Point curPoint = Nodes[0].Start;
            Anchors.Add(Nodes[0].Start);

            for (var i = 0; i < Nodes.Count; i++)
	        {
		        var curNode = Nodes[i];
	            IPrimitivePath curPath;
		        if (curNode is TangentNode tanNode)
		        {
                    var pn = i > 0 ? Nodes[i - 1] : null;
                    var nn = i < Nodes.Count - 1 ? Nodes[i + 1] : null;
                    var p0 = tanNode.GetTangentFromPoint(pn);
                    var p1 = tanNode.GetTangentToPoint(nn);
                    var arc = new Arc(tanNode.CircleRef, p0, p1, tanNode.Direction);
                    Segments.Add(Line.ByEndpoints(curPoint, p0));
                    Segments.Add(arc);
                    Anchors.AddRange(arc.GetPolylinePoints());
                    curPoint = p1;
                }
                else if (curNode is TipNode tipNode)
		        {
                    Anchors.Add(curNode.Start);
                    Segments.Add(Line.ByEndpoints(curPoint, curNode.End));
                    curPoint = curNode.End;
                }
		        else
		        {
			        if (i > 0)
			        {
	                    Anchors.Add(curNode.Start);
	                    Segments.Add(Line.ByEndpoints(curPoint, curNode.Start));
	                    curPoint = curNode.End;
                    }
                }
	        }

            _length = 0;
            foreach (var segment in Segments)
            {
	            _length += segment.Length;
            }
        }

        public void AddNodes(params Node[] nodes)
        {
	        Nodes.AddRange(nodes);
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
            return GetPointOnLine(position, offset);


	    }
	    public Point GetPointOnLine(float position, float offset)
	    {
		    return StartPoint.GetPointOnLineTo(EndPoint, position, offset);
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
