using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Tylox
{
    public class AnchorSegment : BaseSegment
    {
        public AnchorSegment(float startX = 0, float startY = 0, float endX = 0, float endY = 1, int penIndex = 0) : base(-1, penIndex)
        {
            Start = new PointF(startX, startY);
            End = new PointF(endX, endY);
        }
    }
}
