using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;

namespace MLTest.Vis
{
    /// <summary>
    /// The mental map primitives when we conceptualize things at a high level. These are meant to be (ideally) what we use, not what is mathematically possible or even simple.
    /// </summary>
    public interface IPrimitive
    {
        //float Length { get; }
        Point GetPoint(float position, float offset = 0);
        float Similarity(IPrimitive p);
    }

    public interface IPath
    {
	    float Length { get; }
        Point GetPoint(float position, float offset = 0);
        Point GetPointFromCenter(float centeredPosition, float offset);

        VisNode NodeAt(float position, float offset = 0);
        VisNode StartNode { get; }
        VisNode MidNode { get; }
        VisNode EndNode { get; }
        VisStroke FullStroke { get; }
        VisStroke Stroke(float start, float end);

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

        public Point Sample(Gaussian g) => new Point(X + (float)g.Sample(), Y + (float)g.Sample());
        public virtual float Similarity(IPrimitive p) => 0;

        public virtual Point GetPoint(float position, float offset)
        {
            return new Point(X + position, Y + offset);
        }

        public Point Center => new Point(X, Y);

        public virtual float Length => _length;
        //public override float Length() => (float)Math.Sqrt(X * X + Y * Y);
        public float SquaredLength() => X * X + Y * Y;
        public Point Abs() => new Point(Math.Abs(X), Math.Abs(Y));
        public Point Swap() => new Point(Y, X);

        public Point Add(Point pt) => new Point(X + pt.X, Y + pt.Y);
        public Point Subtract(Point pt) => new Point(pt.X - X, pt.Y - Y);
        public Point Multiply(Point pt) => new Point(X * pt.X, Y * pt.Y);
        public Point MidPointOf(Point pt) => new Point((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
        public Point Multiply(float scalar) => new Point(X * scalar, Y * scalar);
        public Point DivideBy(float scalar) => new Point(X / scalar, Y / scalar);
        public float DistanceTo(Point pt) => (float)Math.Sqrt(Math.Pow(pt.X - X, 2) + Math.Pow(pt.Y - Y, 2));
        public float DotProduct(Point pt) => -(X * pt.X) + (Y * pt.Y); // negative because inverted Y
    }

}
