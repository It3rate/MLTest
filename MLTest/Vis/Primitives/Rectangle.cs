using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public class Rectangle : Point
    {
        public Point TopLeft { get; private set; }
        public Point Size { get; private set; }
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
            Size = this.Subtract(TopLeft).Multiply(2f).Abs();
        }

        public override Point GetPoint(float xRatio, float yRatio)
        {
            return new Point(TopLeft.X + Size.X * xRatio, TopLeft.Y + Size.Y * yRatio);
        }

        public Line GetLine(VisDirection direction)
        {
            Line result;
            switch (direction)
            {
                case VisDirection.N:
                    result = Line.ByEndpoints(X - Size.X, Y - Size.Y, X + Size.X, Y - Size.Y);
                    break;
                case VisDirection.S:
                    result = Line.ByEndpoints(X - Size.X, Y + Size.Y, X + Size.X, Y + Size.Y);
                    break;
                case VisDirection.E:
                    result = Line.ByEndpoints(X + Size.X, Y - Size.Y, X + Size.X, Y + Size.Y);
                    break;
                case VisDirection.W:
                    result = Line.ByEndpoints(X - Size.X, Y - Size.Y, X - Size.X, Y + Size.Y);
                    break;

                // diagonal from given point - these can go both directions because the origin depends on the use case (V vs 7)
                case VisDirection.NW:
                    result = Line.ByEndpoints(X - Size.X, Y - Size.Y, X + Size.X, Y + Size.Y);
                    break;
                case VisDirection.NE:
                    result = Line.ByEndpoints(X + Size.X, Y - Size.Y, X - Size.X, Y + Size.Y);
                    break;
                case VisDirection.SW:
                    result = Line.ByEndpoints(X - Size.X, Y + Size.Y, X + Size.X, Y - Size.Y);
                    break;
                case VisDirection.SE:
                    result = Line.ByEndpoints(X + Size.X, Y + Size.Y, X - Size.X, Y - Size.Y);
                    break;

                // centered lines
                case VisDirection.NS:
                    result = Line.ByEndpoints(X, Y - Size.Y, X, Y + Size.Y);
                    break;
                case VisDirection.WE:
                    result = Line.ByEndpoints(X - Size.X, Y, X + Size.X, Y);
                    break;
                default:
                    result = Line.ByEndpoints(X - Size.X, Y - Size.Y, X - Size.X, Y + Size.Y);
                    break;
            }
            return result;
        }

        public VisStroke GetStroke(VisDirection direction)
        {
            VisStroke result;
            Line line = GetLine(direction);
            switch (direction)
            {
                case VisDirection.N:
                    result = new VisStroke(line);
                    break;
                case VisDirection.S:
                    result = new VisStroke(line);
                    break;
                case VisDirection.E:
                    result = new VisStroke(line);
                    break;
                case VisDirection.W:
                    result = new VisStroke(line);
                    break;
                default:
                    result = new VisStroke(line);
                    break;
            }
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
