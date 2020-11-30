using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace MLTest.Vis
{
    public enum ClockDirection{CW, CCW }

    // maybe primitive paths always need to reference volume primitives? Or this is a type of stroke only.
    public class Arc : Point, IPath, IPrimitivePath
    {
        public Circle Reference { get; }
	    public float StartArc { get; }
	    public float EndArc { get; }
	    public ClockDirection Direction { get; }

        public float ArcLength => EndArc - StartArc;

        public override float Length
        {
            get
            {
                if (_length == 0)
                {
                    _length = Reference.Length * ArcLength;
                }
                return _length;
            }
        }
        public Point StartPoint => Reference.GetPoint(StartArc, 0);
        public Point MidPoint => Reference.GetPoint(StartArc + ArcLength / 2f, 0);
        public Point EndPoint => Reference.GetPoint(EndArc, 0);

        public Point Center => Reference.Center;

        public Arc(Circle circle, float startArc, float endArc, ClockDirection direction = ClockDirection.CW) : base(circle.GetPoint(startArc, 0))
        {
	        Reference = circle;
	        StartArc = startArc;
	        EndArc = endArc;
        }

        public override Point GetPoint(float position, float offset = 0)
        {
	        var pos = ArcLength * position + StartArc;
	        return Reference.GetPoint(pos, offset);
        }

        public Point GetPointFromCenter(float centeredPosition, float offset = 0)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }

        public Stroke GetTangentArc(Point leftPoint, Point rightPoint) => null;

        public Node NodeAt(float position) => new Node(this, position);
        public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public Node StartNode => new Node(this, 0f);
        public Node MidNode => new Node(this, 0.5f);
        public Node EndNode => new Node(this, 1f);
        public Stroke FullStroke => new Stroke(StartNode, EndNode);
        public Stroke PartialStroke(float start, float end) => new Stroke(NodeAt(start), NodeAt(end));

        public ClockDirection CounterDirection => Direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
        public Arc CounterArc => new Arc(Reference, StartArc, EndArc, CounterDirection);
    }

    /// <summary>
    /// A circle centered at the XY point, with a Radius. There is no natural 'home' angle -a second point is given to calculate the radius and orient it the circle (which becomes 0).
    /// Circles are the only natural primitive that can have an area - it is a 'large point', not a shape made of joints.
    /// The perimeter is 0-1 as it has a start and end from angle zero around (default is clockwise).
    /// It will be used as tangent targets to and from other points (it arrives and leaves, doesn't care about orientation), unless it is a point starting or ending on the circle.
    /// </summary>
    public class Circle : Point, IPath
	{
		public ClockDirection Direction { get; }
        public Point PerimeterOrigin { get; }
		public float Radius { get; private set; }
		public override bool IsRounded => true;

		public override float Length
		{
			get
			{
				if (_length == 0)
				{
					_length = (float)(2f * Radius * Math.PI);
				}
				return _length;
			}
		}
		public Point StartPoint => PerimeterOrigin;
		public Point MidPoint => GetPoint(0.5f, 0);
		public Point EndPoint => PerimeterOrigin;

        Point Center { get; }

        //public Circle(Point center, float radius) : base(center.X, center.Y)
        //{
        //	Radius = radius;
        //	PerimeterOrigin = new Point(X, Y - radius); // default north
        //}
        public Circle(Point center, Point perimeterOrigin, ClockDirection direction = ClockDirection.CW) : base(center.X, center.Y)
		{
			PerimeterOrigin = perimeterOrigin;
			Direction = direction;
        }
        public Circle(float cx, float cy, float perimeterX, float perimeterY, ClockDirection direction = ClockDirection.CW) : base(cx, cy)
		{
			PerimeterOrigin = new Point(perimeterX, perimeterY);
			Direction = direction;
			Initialize();
		}
		public Circle(Node center, Node perimeterOrigin, ClockDirection direction = ClockDirection.CW) : this(center.Anchor, perimeterOrigin.Anchor, direction){ }

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
		public override Point GetPoint(float position, float offset)
		{
			var rads = Utils.NormToRadians(position);
			return new Point(X + (float)Math.Sin(rads) * (Radius + offset), Y + (float)Math.Cos(rads) * (Radius + offset));
		}

		public Point GetPointFromCenter(float centeredPosition, float offset)
		{
			return GetPoint(centeredPosition * 2f - 1f, offset);
		}

		public Stroke GetTangentArc(Point leftPoint, Point rightPoint) => null;

		public Node NodeAt(float position) => new Node(this, position);
		public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public Node StartNode => new Node(this, 0f);
		public Node MidNode => new Node(this, 0.5f);
		public Node EndNode => new Node(this, 1f);
		public Stroke FullStroke => new Stroke(StartNode, EndNode);
		public Stroke PartialStroke(float start, float end) => new Stroke(NodeAt(start), NodeAt(end));

		public ClockDirection CounterDirection => Direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
        public Circle CounterCircle => new Circle(Center, PerimeterOrigin, CounterDirection);
    }

}
