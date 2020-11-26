using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace MLTest.Vis
{
	public interface IImagined
	{
	}

	public class VisPoint : VisElement, IImagined
    {
		public override VisElementType ElementType => VisElementType.Point;

		protected float Val0 { get; }
		protected float Val1 { get; }
		public float X => Val0;
		public float Y => Val1;

		public virtual bool IsRounded => false;

        public override VisPoint AnchorPoint => this;

        public VisPoint(float x, float y)
        {
	        Val0 = x;
	        Val1 = y;
        }
        public VisPoint(VisPoint p)
        {
	        Val0 = p.X;
	        Val1 = p.Y;
        }
        public PointF PointF => new PointF(X, Y);

		public override VisPoint GetPointUsing(float position, float offset)
		{
            return new VisPoint(X + position, Y + offset);
		}
		public override float Length() => (float)Math.Sqrt(X * X + Y * Y);
		public float SquaredLength() => X * X + Y * Y;
		public VisPoint Abs() => new VisPoint(Math.Abs(X), Math.Abs(Y));
        public VisPoint Swap() => new VisPoint(Y, X);

		public VisPoint Add(VisPoint pt) => new VisPoint(X + pt.X, Y + pt.Y);
		public VisPoint Subtract(VisPoint pt) => new VisPoint(pt.X - X, pt.Y - Y);
		public VisPoint Multiply(VisPoint pt) => new VisPoint(X * pt.X, Y * pt.Y);
		public VisPoint MidPoint(VisPoint pt) => new VisPoint((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
		public VisPoint Multiply(float scalar) => new VisPoint(X * scalar, Y * scalar);
		public VisPoint DivideBy(float scalar) => new VisPoint(X / scalar, Y / scalar);
		public float DistanceTo(VisPoint pt) => (float)Math.Sqrt(Math.Pow(pt.X - X, 2) + Math.Pow(pt.Y - Y, 2));
		public float DotProduct(VisPoint pt) => -(X * pt.X) + (Y * pt.Y); // negative because inverted Y
    }

	public class VisNode : VisPoint
    {
	    public override VisElementType ElementType => VisElementType.Node;

	    public VisStroke Reference { get; }
	    public float Position => Val0;
	    public float Offset => Val1;

        public override VisPoint AnchorPoint { get; }

	    public VisNode(VisStroke reference, float position, float offset) : base(position, offset * reference.Length())
	    {
		    Reference = reference;
		    AnchorPoint = Reference.GetPointUsing(Position, Offset);
        }
	    public VisNode(VisNode p):base(p.Position, p.Offset)
	    {
		    this.Reference = p.Reference;
	    }

        public override VisPoint GetPointUsing(float position, float offset)
	    {
		    return new VisNode(Reference, Position + position, Offset + offset);
	    }
    }

    public class VisRectangle : VisNode
    {
	    public override VisElementType ElementType => VisElementType.Rectangle;

	    public VisPoint CornerOrigin { get; }
	    public VisPoint Size { get; private set; }
        public override bool IsRounded => false;

        public VisRectangle(VisPoint center, VisPoint cornerOrigin) : base(center is VisNode c ? c.Reference : null, center.X, center.Y)
        {
	        CornerOrigin = cornerOrigin;
	        Initialize();
        }
        public VisRectangle(float cx, float cy, float cornerX, float cornerY) : base(null, cx, cy)
        {
	        CornerOrigin = new VisPoint(cornerX, cornerY);
	        Initialize();
        }
        private void Initialize()
        {
	        Size = this.AnchorPoint.Subtract(CornerOrigin.AnchorPoint).Multiply(2f).Abs();
        }

        public VisStroke GetStroke(VisDirection direction)
        {
	        VisStroke result;
	        switch (direction)
	        {
                case VisDirection.N:
                    result = new VisStroke();
	        }
	        return result;
        }
    }

    /// <summary>
    /// North pointing circle at given point with an offset radius. For directional circles, use ovals, or strokes.
    /// </summary>
	public class VisCircle : VisNode
	{
		public override VisElementType ElementType => VisElementType.Circle;
		public VisPoint PerimeterOrigin { get; }
		public float Radius { get; private set; }
		public override bool IsRounded => true;

		public VisCircle(VisNode center, VisNode perimeterOrigin) : base(center.Reference, center.Position, center.Offset)
		{
			PerimeterOrigin = perimeterOrigin;
		}
		public VisCircle(float cx, float cy, float perimeterX, float perimeterY) : base(null, cx, cy)
		{
			PerimeterOrigin = new VisPoint(perimeterX, perimeterY);
			Initialize();
		}
		private void Initialize()
		{
			Radius = this.AnchorPoint.DistanceTo(PerimeterOrigin.AnchorPoint);
        }

        /// <summary>
        /// Gets point along circumference of this circle using position and offset.
        /// </summary>
        /// <param name="position">Normalized (0-1) amount along the circle (0 is north, positive is clockwise, negative is counter clockwise). </param>
        /// <param name="offset">Offset from circumference. Negative is inside, positive is outside. Zero is default, -1 is center.</param>
        /// <returns></returns>
		public override VisPoint GetPointUsing(float position, float offset)
		{
			var rads = Utils.NormToRadians(position);
            return new VisPoint(AnchorPoint.X + (float)Math.Sin(rads) * (Radius + offset), AnchorPoint.Y + (float)Math.Cos(rads) * (Radius + offset));
		}
        public VisStroke GetTangentArc(VisPoint leftPoint, VisPoint rightPoint) => null;
    }


    public class VisOval : VisCircle
    {
	    public override VisElementType ElementType => VisElementType.Oval;

	    public VisNode PerimeterSide { get; }
	    public float RadiusSide{ get; }

	    public VisOval(VisNode center, VisNode perimeterOrigin, VisNode perimeterSide) : base(center, perimeterOrigin)
	    {
		    PerimeterSide = perimeterSide;
		    RadiusSide = center.AnchorPoint.DistanceTo(PerimeterSide.AnchorPoint);
        }

	    public override VisPoint GetPointUsing(float position, float offset)
	    {
		    var rads = Utils.NormToRadians(position);
		    return new VisPoint(AnchorPoint.X + (float)Math.Sin(rads) * (Radius + offset), AnchorPoint.Y + (float)Math.Cos(rads) * (Radius + offset));
	    }
        public VisStroke GetElement(VisDirection direction) => null;
    }
    public class VisSquare : VisPoint
	{
		public override VisElementType ElementType => VisElementType.Square;

		public VisStroke Reference { get; }
		public float Position => Val0;
		public float Radius => Val1;


		public override VisPoint AnchorPoint { get; }

		public VisSquare(VisStroke reference, float position, float offset) : base(position, offset * reference.Length())
		{
			Reference = reference;
			AnchorPoint = Reference.GetPointUsing(Position, 0); // center
		}

		public VisStroke GetElement(VisDirection direction) => null;

		public override VisPoint GetPointUsing(float position, float offset)
		{
			var rads = Utils.NormToRadians(position);
			return new VisPoint(AnchorPoint.X + (float)Math.Sin(rads) * (Radius + offset), AnchorPoint.Y + (float)Math.Cos(rads) * (Radius + offset));
		}
    }

}
