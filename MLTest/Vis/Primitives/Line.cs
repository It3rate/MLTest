using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public interface IPrimitivePath : IPath{}
    /// <summary>
    /// Maybe primitives are always 0-1 (lengths are always positive) and joints/nodes are -1 to 1 (we balance by midpoints of objects)?
    /// Or because lines have a start and end (no volume) they are 0-1, where rects and circles are a point mass that is centered (no start and end). How does inside/outside map to start/end? (center0, edge1, outside>1)
    /// We only use rects and circles to express containment boundaries so they are 0 centered, the corner (or edge) of a rect isn't a volume so it has a start (and end).
    /// 0 is past (known duration), 1 is present, > 1 is future (unknown potentially infinite duration)
    /// </summary>
    public class Line : Point, IPath, IPrimitivePath
    {
        /// No. (A line where XY is the midpoint (0), and Start (-1) and End (1) are defined.)
        public Point Start => new Point(X, Y);
        public Point End { get; private set; }
        public override bool IsRounded => false;
        public override float Length
        {
            get
            {
                if (_length == 0)
                {
                    _length = (float)Math.Sqrt((End.X - X) * (End.X - X) + (End.Y - Y) * (End.Y - Y));
                }
                return _length;
            }
        }
        public Point StartPoint => Start.Center;
        public Point MidPoint => GetPoint(0.5f, 0);
        public Point EndPoint => End.Center;

        private Line(float startX, float startY, float endX, float endY) : base(startX, startY)
        {
            End = new Point(endX, endY);
        }
        private Line(Point start, Point end) : base(start.X, start.Y)
        {
            End = end;
        }

        public static Line ByCenter(float centerX, float centerY, float originX, float originY)
        {
            return new Line(centerX, centerY, originX, originY);
        }
        public static Line ByEndpoints(Point start, Point end)
        {
            return new Line(start, end);
        }
        public static Line ByEndpoints(float startX, float startY, float endX, float endY)
        {
            var start = new Point(startX, startY);
            var end = new Point(endX, endY);
            return new Line(start, end);
        }

        public override Point GetPoint(float position, float offset)
        {
            var xOffset = 0f;
            var yOffset = 0f;
            var xDif = End.X - X;
            var yDif = End.Y - Y;
            if (offset != 0)
            {
                var ang = (float)(Math.Atan2(yDif, xDif));
                xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
                yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
            }
            return new Point(X + xDif * position + xOffset, Y + yDif * position + yOffset);
        }

        public Point GetPointFromCenter(float centeredPosition, float offset)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }

        public Node NodeAt(float position) => new Node(this, position);
        public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public Node StartNode => new Node(this, 0f);
        public Node MidNode => new Node(this, 0.5f);
        public Node EndNode => new Node(this, 1f);
        public Stroke FullStroke => new Stroke(StartNode, EndNode);
        public Stroke PartialStroke(float start, float end) => new Stroke(NodeAt(start), NodeAt(end));

        public Point IntersectionPoint(Line line) => null;
        public Point ProjectedOntoLine(Point p)
        {
            var e1 = End.Subtract(this);
            var e2 = p.Subtract(this);
            var dp = e1.DotProduct(e2);
            var len2 = e1.SquaredLength();
            return new Point(X + (dp * e1.X) / len2, Y + (dp * e1.Y) / len2);
        }

        public Circle CircleFrom() => new Circle(this, End);
        public Rectangle RectangleFrom() => new Rectangle(this, End);


        public Point ProjectPointOnto(Point p)
        {
	        var e1 = EndPoint.Subtract(StartPoint);
	        var e2 = p.Subtract(StartPoint);
	        var dp = e1.DotProduct(e2);
	        var len2 = e1.SquaredLength();
	        return new Point(StartPoint.X + (dp * e1.X) / len2, StartPoint.Y + (dp * e1.Y) / len2);
        }

        public List<Point> GenerateSegments()
        {
	        var result = new List<Point>() {StartPoint, EndPoint};
	        return result;
        }
        public override string ToString()
        {
	        return String.Format("Ln:{0:0.##},{1:0.##} {2:0.##},{3:0.##}", X, Y, End.X, End.Y);
        }
    }
}
