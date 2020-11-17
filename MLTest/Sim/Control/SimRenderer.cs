using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimRenderer
    {
        public List<Pen> Pens;
        public void DrawSpot(SimSection spot, Graphics g)
        {
        }
        public void DrawStroke(SimStroke shape, Graphics g)
        {
        }
        public void DrawShape(SimShape shape, Graphics g)
        {
            //float r = pens[0].Width * 0.6f;
            //foreach (var pos in Positions)
            //{
            //    g.DrawEllipse(pens[0], pos.AnchorPoint.X - r, pos.AnchorPoint.Y - r, r * 2, r * 2);
            //}
            //if (Parent != null)
            //{
            //    foreach (var seg in Pairs)
            //    {
            //        var p0 = Positions[seg.Start];
            //        var p1 = Positions[seg.End];
            //        var penIndex = seg.Pen == 0 ? 0 : colorIndex;
            //        g.DrawLine(pens[penIndex], p0.AnchorPoint.X, p0.AnchorPoint.Y, p1.AnchorPoint.X, p1.AnchorPoint.Y);
            //    }
            //}
            //foreach (var child in Children)
            //{
            //    child.Draw(pens, g, colorIndex + 1);
            //}
        }

    }
}
