using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Tylox
{
    public class TyloxSegment : BaseSegment
    {
        public float CrossSlide { get; set; } // x for default line of origin to 0,1
        public float Position { get; set; } // y for default line of origin to 0,1
        public float AbsLength { get; set; }
        public float AbsAngle { get; set; }

        public float ArcAmount { get; set; }
        public float ArcAngle { get; set; }

        public TyloxSegment(int parentId = 0, float crossSlide = 0, float pos = 0, float len = 1, float angle = 0, int penIndex = 0) : base(parentId, penIndex)
        {
            Id = idCounter++;
            Position = pos;
            AbsLength = len;
            CrossSlide = crossSlide;
            AbsAngle = angle;
            BaseSegment p = GetParent();
            if (p != null)
            {
                // maybe angles and lens are absolute to parent segments, but relative to Anchor lines.
                float sin = (float)-Math.Sin(AbsAngle * Math.PI);
                float cos = (float)Math.Cos(AbsAngle * Math.PI);
                float anchorX = p.Start.X + (p.End.X - p.Start.X) * Position;
                float anchorY = p.Start.Y + (p.End.Y - p.Start.Y) * Position;
                float rLen = AbsLength * p.Length;
                float posCross = rLen * CrossSlide;
                float negCross = (rLen - posCross);
                Start = new PointF(anchorX + sin * -negCross, anchorY + cos * -negCross);
                End = new PointF(anchorX + sin * posCross, anchorY + cos * posCross);
            }
        }
    }

}
