using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Tensorflow;

namespace MLTest.Tylox
{
    public class TyloxGenerator
    {
        public int Width { get; set; } = 250;
        public int Height { get; set; } = 250;
        private List<BaseSegment> Segments => BaseSegment.Segments;

        private List<Pen> Pens { get; } = new List<Pen>();
        public TyloxGenerator()
        {
            GenTestSegments();
        }
        public void Draw(float scale, Graphics g)
        {
            if(Pens.Count == 0)
            {
                GenPens(scale);
            }
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var baseSegment in Segments)
            {
                baseSegment.Draw(Pens[baseSegment.PenIndex], g);
            }
        }
        private void GenTestSegments()
        {
            var vert = AddSegment(new AnchorSegment(0.5f, .1f, 0.5f, .9f, penIndex: (int)PenTypes.LightGray));
            var horz = AddSegment(new AnchorSegment(0.1f, .9f, 0.9f, .9f, penIndex: (int)PenTypes.LightGray));
            
            AddSegment(new TyloxSegment(vert, crossSlide: 0.0f, pos: 0.4f, len: 0.3f, angle: 0.25f, penIndex: (int)PenTypes.Black));
            AddSegment(new TyloxSegment(vert, crossSlide: 0.0f, pos: 0.4f, len: 0.2f, angle: 0.50f, penIndex: (int)PenTypes.Black));
            AddSegment(new TyloxSegment(vert, crossSlide: 0.0f, pos: 0.4f, len: 0.3f, angle: 0.75f, penIndex: (int)PenTypes.Black));
            
            AddSegment(new TyloxSegment(vert, crossSlide: 0.5f, pos: 0.75f, len: 0.3f, angle: 0.5f, penIndex: (int)PenTypes.DarkBlue));
            AddSegment(new TyloxSegment(horz, crossSlide: 0f, pos: 1, len: 0.4f, angle: -0.3f, penIndex: (int)PenTypes.Orange));
            AddSegment(new TyloxSegment(horz, crossSlide: 0f, pos: 0f, len: 0.4f, angle: 0.3f, penIndex: (int)PenTypes.DarkRed));
        }

        private int AddSegment(BaseSegment seg)
        {
            Segments.Add(seg);
            return seg.Id;
        }

        private enum PenTypes
        {
            LightGray,
            Black,
            DarkRed,
            DarkBlue,
            Orange,
        }
        private void GenPens(float scale)
        {
            Pens.Add(GetPen(Color.LightGray, 2f / scale));
            Pens.Add(GetPen(Color.Black, 8f / scale));
            Pens.Add(GetPen(Color.DarkRed, 8f / scale));
            Pens.Add(GetPen(Color.DarkBlue, 8f / scale));
            Pens.Add(GetPen(Color.Orange, 8f / scale));
        }
        private Pen GetPen(Color color, float width)
        {
            var pen = new Pen(color, width);
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }


    }
}
