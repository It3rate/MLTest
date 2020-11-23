using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Tylox
{
    public class TyloxAnchorSegment : TyloxBaseSegment
    {
        // rather than anchors being absolute they can be instances. They can apply selectivly to only x/y/len/cross etc, or all
        // so eg. each letter gets a dup of the letterbox anchor frame, and these frames are strung along a baseline anchor line.
        // Maybe the baseline, ascender, decender, crossbar etc are for Y information only, while width, volume, space etc is X only.
        public TyloxAnchorSegment(float startX = 0, float startY = 0, float endX = 0, float endY = 1, int penIndex = 0) : base(-1, penIndex)
        {
            Start = new PointF(startX, startY);
            End = new PointF(endX, endY);
        }
    }
}
