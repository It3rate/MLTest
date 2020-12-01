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

        private List<Point> Anchors = new List<Point>();
        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();


        public Stroke(Node first, Node second, params Node[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
	    }

        public List<Point> GenerateSegments()
        {
	        Segments.Clear();
            Anchors.Clear();
	        for (var i = 0; i < Nodes.Count; i++)
	        {
		        var curNode = Nodes[i];
	            IPrimitivePath curPath;
		        if (curNode is TangentNode tanNode)
		        {
                    // arc, start at tangent to last node end at tangent to next node. Set curPoint to end point.
                    //curPath = Line.ByEndpoints(curPoint, curNode.Start);
                    //Segments.Add(curPath);
                    // curpath = arc
                    //curPoint = curPath.EndPoint;
                    var pn = i > 0 ? Nodes[i - 1] : null;
                    var nn = i < Nodes.Count - 1 ? Nodes[i + 1] : null;
                    Anchors.Add(tanNode.GetStartFromPoint(pn));
                    Anchors.Add(tanNode.GetEndFromPoint(nn));

                }
                else if (curNode is TipNode tipNode)
		        {
                    // must be end
                    //curPath = Line.ByEndpoints(curPoint, curNode.End);
                    Anchors.Add(curNode.Anchor);
                }
		        else
		        {
                    // there should never be two lines in a row (otherwise it would make a joint). Unless that is ok? Maybe a stroke is a non pen lift line?
			        //curPath = Line.ByEndpoints(curPoint, curNode.Anchor);
			        //Segments.Add(curPath);
			        //curPoint = curNode.Anchor;
                    Anchors.Add(curNode.Anchor);
                }
	        }

	        return Anchors;
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
            return GetPointOnLine(position, offset);


	    }
	    public Point GetPointOnLine(double position, double offset)
	    {
            // todo: move this to Line, add for arcs, calculate based on lengths and tangents.
		    var sp = StartPoint;
		    var ep = EndPoint;
		    float xOffset = 0;
		    float yOffset = 0;
		    float xDif = ep.X - sp.X;
		    float yDif = ep.Y - sp.Y;
		    if (offset != 0)
		    {
			    float ang = (float)(Math.Atan2(yDif, xDif));
			    xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
			    yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
		    }
		    return new Point(
			    sp.X + xDif * (float)position + xOffset,
			    sp.Y + yDif * (float)position + yOffset);
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
