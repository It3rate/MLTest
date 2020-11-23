using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Tylox
{
    public class TyloxSegment : TyloxBaseSegment
    {
        public float CrossSlide { get; set; } // x for default line of origin to 0,1
        public float Position { get; set; } // y for default line of origin to 0,1
        public float Offset { get; set; } // percent (or abs?) cx,cy is from the left (negative) or right (positive) of the start end line
        public float AbsLength { get; set; }
        public float AbsAngle { get; set; }

        public float ArcAmount { get; set; }
        public float ArcAngle { get; set; }
        public PointF Center { get; set; }

        public TyloxSegment(int parentId = 0, float offset = 0, float crossSlide = 0, float pos = 0, float len = 1, float angle = 0, int penIndex = 0) : base(parentId, penIndex)
        {
            Offset = offset;
            Position = pos;
            AbsLength = len;
            CrossSlide = crossSlide;
            AbsAngle = angle;
            TyloxBaseSegment p = GetParent();
            if (p != null)
            {
                // maybe angles and lens are absolute to parent segments, but relative to Anchor lines.
                float sin = (float)-Math.Sin(AbsAngle * Math.PI);
                float cos = (float)Math.Cos(AbsAngle * Math.PI);
                float xOffset = 0;
                float yOffset = 0;
                float xDif = p.End.X - p.Start.X;
                float yDif = p.End.Y - p.Start.Y;

                if(Offset != 0)
                {
                    float ang = (float)(Math.Atan2(yDif, xDif));
                    xOffset = (float)(-Math.Sin(ang) * Math.Abs(Offset / 2.0) * Math.Sign(-Offset));
                    yOffset = (float)(Math.Cos(ang) * Math.Abs(Offset / 2.0) * Math.Sign(-Offset));
                }
                float anchorX = p.Start.X + xDif * Position + xOffset;
                float anchorY = p.Start.Y + yDif * Position + yOffset;
                Center = new PointF(anchorX, anchorY);
                float rLen = AbsLength;// * p.Length;
                float posCross = rLen * CrossSlide;
                float negCross = (rLen - posCross);
                Start = new PointF(anchorX + sin * -negCross, anchorY + cos * -negCross);
                End = new PointF(anchorX + sin * posCross, anchorY + cos * posCross);
            }
        }
        public override void Draw(Pen pen, Graphics g)
        {
            base.Draw(pen, g);
            float r = pen.Width * 0.3f;
            g.DrawEllipse(pen, Center.X - r, Center.Y-r, r * 2, r * 2);
        }
    }

}
