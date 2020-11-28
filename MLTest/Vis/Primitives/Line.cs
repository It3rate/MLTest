using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    /// <summary>
    /// Maybe primitives are always 0-1 (lengths are always positive) and joints/nodes are -1 to 1 (we balance by midpoints of objects)?
    /// Or because lines have a start and end (no volume) they are 0-1, where rects and circles are a point mass that is centered (no start and end). How does inside/outside map to start/end? (center0, edge1, outside>1)
    /// We only use rects and circles to express containment boundaries so they are 0 centered, the corner (or edge) of a rect isn't a volume so it has a start (and end).
    /// 0 is past (known duration), 1 is present, > 1 is future (unknown potentially infinite duration)
    /// </summary>
    public class Line : Point, IPath
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
            position = position / 2f + 0.5f;
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

        public VisNode NodeAt(float position, float offset = 0) => new VisNode(this, position, offset);
        public VisNode StartNode => new VisNode(this, 0f, 0);
        public VisNode MidNode => new VisNode(this, 0.5f, 0);
        public VisNode EndNode => new VisNode(this, 1f, 0);
        public VisJoint StartTipJoint => new VisJoint(StartNode);
        public VisJoint TipJointAt(float position, float offset = 0) => new VisJoint(NodeAt(position, offset));
        public VisJoint EndTipJoint => new VisJoint(EndNode);
        public VisStroke FullStroke => new VisStroke(StartTipJoint, EndTipJoint);
        public VisStroke Stroke(float start, float end) => new VisStroke(TipJointAt(start), TipJointAt(end));

        public VisStroke GetStroke(float startPosition, float endPosition)
        {
            VisStroke result = new VisStroke(null, null);
            return result;
        }

        public Point MidPoint => new Point(X + (End.X - X) / 2f, Y + (End.Y - Y) / 2f);
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

    }
}
