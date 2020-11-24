using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace MLTest.Vis
{
	public class VisPoint : VisElement
	{
		public override VisElementType ElementType => VisElementType.Point;
		public override float Length => 0;

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
    }

	public class VisNode : VisPoint
    {
	    public override VisElementType ElementType => VisElementType.Node;

	    public VisStroke Reference { get; }
	    public float Position => Val0;
	    public float Offset => Val1;

        public override VisPoint AnchorPoint { get; }

	    public VisNode(VisStroke reference, float position, float offset) : base(position, offset * reference.Length)
	    {
		    Reference = reference;
		    AnchorPoint = Reference.GetPointOnLine(Position, Offset);
        }
    }

	public class VisCircle : VisPoint
	{
		public override VisElementType ElementType => VisElementType.Circle;

		public VisStroke Reference { get; }
		public float Position => Val0;
		public float Radius => Val1;

		public override bool IsRounded => true;

		public override VisPoint AnchorPoint { get; }

		public VisCircle(VisStroke reference, float position, float offset) : base(position, offset * reference.Length)
		{
			Reference = reference;
			AnchorPoint = Reference.GetPointOnLine(Position, 0); // center
        }

		public VisStroke GetTangentArc(VisPoint leftPoint, VisPoint rightPoint) => null;
	}
	public class VisSquare : VisPoint
	{
		public override VisElementType ElementType => VisElementType.Square;

		public VisStroke Reference { get; }
		public float Position => Val0;
		public float Radius => Val1;


		public override VisPoint AnchorPoint { get; }

		public VisSquare(VisStroke reference, float position, float offset) : base(position, offset * reference.Length)
		{
			Reference = reference;
			AnchorPoint = Reference.GetPointOnLine(Position, 0); // center
		}

		public VisStroke GetElement(VisDirection direction) => null;
	}

	public class VisRectangle : VisPoint
	{
		public override VisElementType ElementType => VisElementType.Rectangle;

		public VisStroke Reference { get; }
		public float CenterPosition => Val0;
		public float CenterOffset => Val1;
		public VisPoint CornerOrigin { get; }


		public override VisPoint AnchorPoint { get; }

		public VisRectangle(VisStroke reference, float position, float offset, VisPoint cornerOrigin) : base(position, offset * reference.Length)
		{
			Reference = reference;
			CornerOrigin = cornerOrigin;
			AnchorPoint = Reference.GetPointOnLine(CenterPosition, CenterOffset); // center
		}

		public VisStroke GetElement(VisDirection direction) => null;
	}
	public class VisOval : VisPoint
	{
		public override VisElementType ElementType => VisElementType.Oval;

		public VisStroke Reference { get; }
		public float CenterPosition => Val0;
		public float CenterOffset => Val1;
		public VisPoint RadiusOrigin { get; }


		public override VisPoint AnchorPoint { get; }

		public VisOval(VisStroke reference, float position, float offset, VisPoint radiusOrigin) : base(position, offset * reference.Length)
		{
			Reference = reference;
			RadiusOrigin = radiusOrigin;
			AnchorPoint = Reference.GetPointOnLine(CenterPosition, CenterOffset); // center
		}

		public VisStroke GetElement(VisDirection direction) => null;
	}
}
