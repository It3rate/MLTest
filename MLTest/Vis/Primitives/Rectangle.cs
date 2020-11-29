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
        public override bool IsRounded => false;

        public Rectangle(Point center, Point corner) : base(center.X, center.Y)
        {
            Initialize(corner.X, corner.Y);
        }
        public Rectangle(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
            TopLeft = new Point(cornerX, cornerY);
            Initialize(cornerX, cornerY);
        }
        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new Point(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = this.Subtract(TopLeft).Abs();
        }

        public override Point GetPoint(float xRatio, float yRatio)
        {
            return new Point(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
        }


        public Line GetLine(CompassDirection direction)
        {
            Line result;
            switch (direction)
            {
                case CompassDirection.N:
                    result = Line.ByEndpoints(X - HalfSize.X, Y - HalfSize.Y, X + HalfSize.X, Y - HalfSize.Y);
                    break;
                case CompassDirection.S:
                    result = Line.ByEndpoints(X - HalfSize.X, Y + HalfSize.Y, X + HalfSize.X, Y + HalfSize.Y);
                    break;
                case CompassDirection.E:
                    result = Line.ByEndpoints(X + HalfSize.X, Y - HalfSize.Y, X + HalfSize.X, Y + HalfSize.Y);
                    break;
                case CompassDirection.W:
                    result = Line.ByEndpoints(X - HalfSize.X, Y - HalfSize.Y, X - HalfSize.X, Y + HalfSize.Y);
                    break;

                // diagonal from given point - these can go both directions because the origin depends on the use case (V vs 7)
                case CompassDirection.NW:
                    result = Line.ByEndpoints(X - HalfSize.X, Y - HalfSize.Y, X + HalfSize.X, Y + HalfSize.Y);
                    break;
                case CompassDirection.NE:
                    result = Line.ByEndpoints(X + HalfSize.X, Y - HalfSize.Y, X - HalfSize.X, Y + HalfSize.Y);
                    break;
                case CompassDirection.SW:
                    result = Line.ByEndpoints(X - HalfSize.X, Y + HalfSize.Y, X + HalfSize.X, Y - HalfSize.Y);
                    break;
                case CompassDirection.SE:
                    result = Line.ByEndpoints(X + HalfSize.X, Y + HalfSize.Y, X - HalfSize.X, Y - HalfSize.Y);
                    break;

                // centered lines
                case CompassDirection.NS:
                    result = Line.ByEndpoints(X, Y - HalfSize.Y, X, Y + HalfSize.Y);
                    break;
                case CompassDirection.WE:
                    result = Line.ByEndpoints(X - HalfSize.X, Y, X + HalfSize.X, Y);
                    break;
                default:
                    result = Line.ByEndpoints(X - HalfSize.X, Y - HalfSize.Y, X - HalfSize.X, Y + HalfSize.Y);
                    break;
            }
            return result;
        }
        public Point GetPoint(CompassDirection direction)
        {
	        Point result;
            switch (direction)
            {
                case CompassDirection.N:
                    result = new Point(X, Y - HalfSize.Y);
                    break;
                case CompassDirection.S:
	                result = new Point(X, Y + HalfSize.Y);
                    break;
                case CompassDirection.E:
	                result = new Point(X + HalfSize.X, Y);
                    break;
                case CompassDirection.W:
	                result = new Point(X - HalfSize.X, Y);
                    break;

                case CompassDirection.NW:
	                result = new Point(X - HalfSize.X, Y - HalfSize.Y);
                    break;
                case CompassDirection.NE:
	                result = new Point(X + HalfSize.X, Y - HalfSize.Y);
                    break;
                case CompassDirection.SW:
	                result = new Point(X - HalfSize.X, Y + HalfSize.Y);
                    break;
                case CompassDirection.SE:
	                result = new Point(X + HalfSize.X, Y + HalfSize.Y);
                    break;
                default:
	                result = new Point(X, Y);
                    break;
            }
            return result;
        }

        public VisStroke GetStroke(CompassDirection direction)
        {
            VisStroke result= GetLine(direction).FullStroke;
            //Line line = GetLine(direction);
            //switch (direction)
            //{
            //    case CompassDirection.N:
	           //     result = line.FullStroke;
            //        break;
            //    case CompassDirection.S:
            //        result = new VisStroke(line);
            //        break;
            //    case CompassDirection.E:
            //        result = new VisStroke(line);
            //        break;
            //    case CompassDirection.W:
            //        result = new VisStroke(line);
            //        break;
            //    default:
            //        result = new VisStroke(line);
            //        break;
            //}
            return result;
        }

        public Point NearestIntersectionTo(Point p) => null;
        public Point Overlaps(Point p) => null;
        public Point Overlaps(Line line) => null;
        public Point Overlaps(Rectangle rect) => null;
        public Point Contains(Point p) => null;
        public Point Contains(Line line) => null;
        public Point Contains(Rectangle rect) => null;
    }

}
