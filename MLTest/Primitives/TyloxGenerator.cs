using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Tensorflow;

namespace MLTest.Primitives
{
    public abstract class SegmentBase
    {
        protected static int idCounter;
        public int Id { get; set; }

        public static List<SegmentBase> Segments { get; } = new List<SegmentBase>();

        public int ParentId { get; set; }
        public Pen Pen { get; set; }

        public PointF Start { get; set; }
        public PointF End { get; set; }
        public float Length
        {
            get
            {
                var difX = End.X - Start.X;
                var difY = End.Y - Start.Y;
                return (float)Math.Sqrt(difX * difX + difY * difY);
            }
        }

        public SegmentBase(int parentId = -1, Pen pen = null)
        {
            Id = idCounter++;
            ParentId = parentId;
            Pen = pen;
        }
        protected SegmentBase GetParent()
        {
            SegmentBase result = null;
            if (ParentId >= 0 && ParentId < Segments.Count)
            {
                result = Segments[ParentId];
            }
            return result;
        }
        public void Draw(Graphics g)
        {
            if(Pen != null)
            {
                g.DrawLine(Pen, Start, End);
            }
        }
    }


    public class AnchorSegment : SegmentBase
    {
        public AnchorSegment(float startX = 0, float startY = 0, float endX = 0, float endY = 1, Pen pen = null) : base(-1, pen)
        {
            Start = new PointF(startX, startY);
            End = new PointF(endX, endY);
        }
    }

    public class TyloxSegment : SegmentBase
    {
        public float Crossing { get; set; } // x for default line of origin to 0,1
        public float Position { get; set; } // y for default line of origin to 0,1
        public float AbsLength { get; set; }
        public float AbsAngle { get; set; }

        public float ArcAmount { get; set; }
        public float ArcAngle { get; set; }
        public TyloxSegment(int parentId=0, float cross=0, float pos=0, float len=1, float angle = 0, Pen pen = null) : base(parentId, pen)
        {
            Id = idCounter++;
            Position = pos;
            AbsLength = len;
            Crossing = cross;
            AbsAngle = angle;
            SegmentBase p = GetParent();
            if(p != null)
            {
                // maybe angles and lens are absolute to parent segments, but relative to Anchor lines.
                float sin = (float)-Math.Sin(AbsAngle * Math.PI);
                float cos = (float)Math.Cos(AbsAngle * Math.PI);
                float anchorX = p.Start.X + (p.End.X - p.Start.X) * Position;
                float anchorY = p.Start.Y + (p.End.Y - p.Start.Y) * Position;
                float rLen = AbsLength * p.Length;
                float posCross = rLen * Crossing;
                float negCross = (rLen - posCross);
                Start = new PointF(anchorX + sin * -negCross, anchorY + cos * -negCross);
                End = new PointF(anchorX + sin * posCross, anchorY + cos * posCross);
            }
        }
    }

    public class TyloxGenerator
    {
        public int Width { get; set; } = 250;
        public int Height { get; set; } = 250;
        private AnchorSegment _root;
        private List<SegmentBase> Segments => SegmentBase.Segments;

        private Pen _defaultPen;
        private Pen _penRed;
        private Pen _penBlue;
        private Pen _penGray;

        public TyloxGenerator()
        {
            _penRed = new Pen(Color.DarkRed, 4f / (float)Width);
            _penBlue = new Pen(Color.DarkBlue, 4f / (float)Width);
            _penGray = new Pen(Color.LightGray, 2f / (float)Width);

            _defaultPen = new Pen(Color.Black, 4f / (float)Width);
            _defaultPen.LineJoin = LineJoin.Round;
            _defaultPen.StartCap = LineCap.Round;
            _defaultPen.EndCap = LineCap.Round;

            _root = new AnchorSegment(0.5f, .1f, 0.5f, .9f, pen: _penGray);
            Segments.Add(_root);

            Segments.Add(new TyloxSegment(0, cross: 0.0f, pos: 0.5f, len: 0.3f, angle: 0.25f, pen: _defaultPen));
            Segments.Add(new TyloxSegment(0, cross: 0.0f, pos: 0.5f, len: 0.3f, angle: 0.50f, pen: _defaultPen));
            Segments.Add(new TyloxSegment(0, cross: 0.0f, pos: 0.5f, len: 0.3f, angle: 0.75f, pen: _defaultPen));

            Segments.Add(new TyloxSegment(0, cross: 0.2f, pos: 0.7f, len: 0.9f, angle: -0.1f, pen: _penBlue));
            Segments.Add(new TyloxSegment(4, cross: 0.2f, pos: 0.7f, len: 0.9f, angle: 0.1f, pen: _penRed));
            Segments.Add(new TyloxSegment(4, cross: 0f, pos: 0f, len: 0.4f, angle: 0.5f, pen: _penRed));

        }

        public void Draw(Graphics g)
        {
            g.DrawLine(_penGray, new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(_penGray, new PointF(0, -1f), new PointF(0, 1f));

            foreach (var baseSegment in Segments)
            {
                baseSegment.Draw(g);
            }
        }
    }
}
