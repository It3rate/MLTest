using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace MLTest.Vis
{
    /// <summary>
    /// The mental map primitives when we conceptualize things at a high level. These are meant to be (ideally) what we use, not what is mathematically possible or even simple.
    /// </summary>
    public interface IPrimitive
    {
	    float Length { get; }
        Point GetPointUsing(float position, float offset);
        float Similarity(IPrimitive p);
    }

	public class Point : IPrimitive
    {
		public float X { get; }
        public float Y { get; }

        public virtual bool IsRounded => false;
		protected float _length = 0;

        public Point(float x, float y)
        {
	        X = x;
	        Y = y;
        }
        public Point(Point p)
        {
	        X = p.X;
	        Y = p.Y;
        }
        public PointF PointF => new PointF(X, Y);
        public virtual float Similarity(IPrimitive p) => 0;
         
		public virtual Point GetPointUsing(float position, float offset)
		{
            return new Point(X + position, Y + offset);
		}
		public virtual float Length => _length;
		//public override float Length() => (float)Math.Sqrt(X * X + Y * Y);
		public float SquaredLength() => X * X + Y * Y;
		public Point Abs() => new Point(Math.Abs(X), Math.Abs(Y));
        public Point Swap() => new Point(Y, X);

		public Point Add(Point pt) => new Point(X + pt.X, Y + pt.Y);
		public Point Subtract(Point pt) => new Point(pt.X - X, pt.Y - Y);
		public Point Multiply(Point pt) => new Point(X * pt.X, Y * pt.Y);
		public Point MidPoint(Point pt) => new Point((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
		public Point Multiply(float scalar) => new Point(X * scalar, Y * scalar);
		public Point DivideBy(float scalar) => new Point(X / scalar, Y / scalar);
		public float DistanceTo(Point pt) => (float)Math.Sqrt(Math.Pow(pt.X - X, 2) + Math.Pow(pt.Y - Y, 2));
		public float DotProduct(Point pt) => -(X * pt.X) + (Y * pt.Y); // negative because inverted Y
    }

    /// <summary>
    /// A line where XY is the midpoint (0), and Start (-1) and End (1) are defined.
    /// </summary>
	public class Line: Point
	{
		public Point Start { get; private set; }
		public Point End { get; private set; }
        public override bool IsRounded => false;

		public override float Length
		{
			get
			{
				if (_length == 0)
				{
					_length = (float) Math.Sqrt((End.X - X) * (End.X - X) + (End.Y - Y) * (End.Y - Y));
				}
				return _length;
			}
		}

		private Line(float centerX, float centerY, float originX, float originY) : base(centerX, centerY)
		{
			Start = new Point(originX, originY);
			End = new Point(X + (X - originX), Y + (Y - originY));
		}
		private Line(Point start, Point end) : base(start.X + (end.X - start.X) / 2f, start.Y + (end.Y - start.Y) / 2f)
		{
			Start = start;
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

		public override Point GetPointUsing(float position, float offset)
		{
			position = position / 2f + 0.5f;
			var xOffset = 0f;
			var yOffset = 0f;
			var xDif = End.X - Start.X;
			var yDif = End.Y - Start.Y;
			if (offset != 0)
			{
				var ang = (float)(Math.Atan2(yDif, xDif));
				xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
				yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
			}
			return new Point(Start.X + xDif * position + xOffset, Start.Y + yDif * position + yOffset);
		}

        public VisStroke GetStroke(float startPosition, float endPosition)
		{
			VisStroke result = new VisStroke();
			return result;
        }
        public Point IntersectionPoint(Line line) => null;
        public Point ProjectedOntoLine(Point p)
        {
	        var e1 = End.Subtract(this);
	        var e2 = p.Subtract(this);
	        var dp = e1.DotProduct(e2);
	        var len2 = e1.SquaredLength();
	        return new Point(X + (dp * e1.X) / len2, Y + (dp * e1.Y) / len2);
        }

    }
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

        public override Point GetPointUsing(float xRatio, float yRatio)
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

		        // diagonal from given point
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
		            result = new VisStroke();
		            break;
	            case VisDirection.S:
		            result = new VisStroke();
		            break;
	            case VisDirection.E:
		            result = new VisStroke();
		            break;
	            case VisDirection.W:
		            result = new VisStroke();
		            break;
                default:
	                result = new VisStroke();
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

    /// <summary>
    /// North pointing circle at given point with an offset radius. For directional circles, use ovals, or strokes.
    /// </summary>
	public class Circle : Point
	{
		public Point PerimeterOrigin { get; }
		public float Radius { get; private set; }
		public override bool IsRounded => true;

		public Circle(Point center, Point perimeterOrigin) : base(center.X, center.Y)
		{
			PerimeterOrigin = perimeterOrigin;
		}
		public Circle(float cx, float cy, float perimeterX, float perimeterY) : base(cx, cy)
		{
			PerimeterOrigin = new Point(perimeterX, perimeterY);
			Initialize();
		}
		private void Initialize()
		{
			Radius = this.DistanceTo(PerimeterOrigin);
        }

        /// <summary>
        /// Gets point along circumference of this circle using position and offset.
        /// </summary>
        /// <param name="position">Normalized (0-1) amount along the circle (0 is north, positive is clockwise, negative is counter clockwise). </param>
        /// <param name="offset">Offset from circumference. Negative is inside, positive is outside. Zero is default, -1 is start.</param>
        /// <returns></returns>
		public override Point GetPointUsing(float position, float offset)
		{
			var rads = Utils.NormToRadians(position);
            return new Point(X + (float)Math.Sin(rads) * (Radius + offset), Y + (float)Math.Cos(rads) * (Radius + offset));
		}
        public VisStroke GetTangentArc(Point leftPoint, Point rightPoint) => null;
    }


 //   public class VisOval : Circle
 //   {
	//    public override VisElementType ElementType => VisElementType.Oval;

	//    public VisNode PerimeterSide { get; }
	//    public float RadiusSide{ get; }

	//    public VisOval(VisNode center, VisNode perimeterOrigin, VisNode perimeterSide) : base(center, perimeterOrigin)
	//    {
	//	    PerimeterSide = perimeterSide;
	//	    RadiusSide = center.Anchor.DistanceTo(PerimeterSide.Anchor);
 //       }

	//    public override Point GetPointUsing(float position, float offset)
	//    {
	//	    var rads = Utils.NormToRadians(position);
	//	    return new Point(Anchor.X + (float)Math.Sin(rads) * (Radius + offset), Anchor.Y + (float)Math.Cos(rads) * (Radius + offset));
	//    }
 //       public VisStroke GetElement(VisDirection direction) => null;
 //   }
 //   public class VisSquare : Point
	//{
	//	public override VisElementType ElementType => VisElementType.Square;

	//	public VisStroke Reference { get; }
	//	public float Position => Val0;
	//	public float Radius => Val1;


	//	public override Point Anchor { get; }

	//	public VisSquare(VisStroke reference, float position, float offset) : base(position, offset * reference.Length())
	//	{
	//		Reference = reference;
	//		Anchor = Reference.GetPointUsing(Position, 0); // start
	//	}

	//	public VisStroke GetElement(VisDirection direction) => null;

	//	public override Point GetPointUsing(float position, float offset)
	//	{
	//		var rads = Utils.NormToRadians(position);
	//		return new Point(Anchor.X + (float)Math.Sin(rads) * (Radius + offset), Anchor.Y + (float)Math.Cos(rads) * (Radius + offset));
	//	}
 //   }

}
