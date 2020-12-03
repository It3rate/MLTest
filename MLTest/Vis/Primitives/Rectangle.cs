using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    /// <summary>
    /// A rectangle defined by a center and corner (can not be rotated or skewed). This isn't a  path, but it's points and lines can be used for reference.
    /// For convenience it can be turned into four strokes if a box made of strokes is desired (though you may want more control over the stroke order).
    /// </summary>
    public class Rectangle : Point
    {
        public Point TopLeft { get; private set; }
        public Point Size => HalfSize.Multiply(2f);
        public Point HalfSize { get; private set; }
        public Point Center => this;

        public Rectangle(Point center, Point corner) : base(center.X, center.Y)
        {
            Initialize(corner.X, corner.Y);
        }
        public Rectangle(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
            Initialize(cornerX, cornerY);
        }
        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new Point(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = this.Subtract(TopLeft).Abs();
        }

        public Point GetPoint(float xRatio, float yRatio)
        {
            return new Point(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
        }


        public Line GetLine(CompassDirection direction)
        {
	        return direction.GetLineFrom(this);
        }
        public Point GetPoint(CompassDirection direction)
        {
	        return direction.GetPointFrom(this);
        }

        public Point NearestIntersectionTo(Point p) => null;
        public Point Overlaps(Point p) => null;
        public Point Overlaps(Line line) => null;
        public Point Overlaps(Rectangle rect) => null;
        public Point Contains(Point p) => null;
        public Point Contains(Line line) => null;
        public Point Contains(Rectangle rect) => null;

        public override string ToString()
        {
	        return String.Format("Rect:{0:0.##},{1:0.##} {2:0.##},{3:0.##}", TopLeft.X, TopLeft.Y, Size.X, Size.Y);
        }
    }

}
