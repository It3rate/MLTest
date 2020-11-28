using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	/// <summary>
	/// North pointing circle at given point with an offset radius. For directional circles, use ovals, or strokes.
	/// </summary>
	public class Circle : Point, IPath
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
		public Circle(VisNode center, VisNode perimeterOrigin) : this(center.Anchor, perimeterOrigin.Anchor){ }

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

		public VisStroke GetTangentArc(Point leftPoint, Point rightPoint) => null;

		public VisNode StartNode => new VisNode(this, 0f, 0);
		public VisNode EndNode => new VisNode(this, 1f, 0);
    }

}
