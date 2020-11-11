﻿using System;
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
            AddRect(Segments, .1f, .1f, .9f, .9f);
            AddRect(Segments, .3f, .4f, .7f, .6f);
            //var vert = AddSegment(new AnchorSegment(0.5f, .1f, 0.5f, .9f, penIndex: (int)PenTypes.LightGray));
            //var horz = AddSegment(new AnchorSegment(0.1f, .9f, 0.9f, .9f, penIndex: (int)PenTypes.LightGray));

            //AddSegment(segments, new TyloxSegment(vert, offset:0, crossSlide: 0.0f, pos: 0.4f, len: 0.3f, angle: 0.25f, penIndex: (int)PenTypes.Black));
            //AddSegment(segments, new TyloxSegment(vert, offset:0.2f, crossSlide: 0.0f, pos: 0.4f, len: 0.2f, angle: 0.50f, penIndex: (int)PenTypes.Black));
            //AddSegment(segments, new TyloxSegment(vert, offset:0, crossSlide: 0.0f, pos: 0.4f, len: 0.3f, angle: 0.75f, penIndex: (int)PenTypes.Black));

            //AddSegment(segments, new TyloxSegment(vert, offset:0, crossSlide: 0.5f, pos: 0.75f, len: 0.3f, angle: 0.5f, penIndex: (int)PenTypes.DarkBlue));
            //AddSegment(segments, new TyloxSegment(horz, offset:0.1f, crossSlide: 0f, pos: 1, len: 0.4f, angle: -0.3f, penIndex: (int)PenTypes.Orange));
            //AddSegment(segments, new TyloxSegment(horz, offset:0.1f, crossSlide: 0f, pos: 0f, len: 0.4f, angle: 0.3f, penIndex: (int)PenTypes.DarkRed));
        }

        private void AddRect(List<BaseSegment> segments, float x0, float y0, float x1, float y1)
        {
            var top  = AddSegment(segments, new AnchorSegment(x0, y0, x1, y0, penIndex: (int)PenTypes.LightGray));
            var left = AddSegment(segments, new AnchorSegment(x0, y0, x0, y1, penIndex: (int)PenTypes.LightGray));
            AddSegment(segments, new TyloxSegment(top.Id,  offset: 0, crossSlide: 0.2f, pos: 0f, len: top.Length * 1.4f, angle: 0.5f, penIndex: (int)PenTypes.Black));  // -
            AddSegment(segments, new TyloxSegment(left.Id, offset: 0, crossSlide: 0.2f, pos: 0f, len: left.Length * 1.4f, angle: 1f, penIndex: (int)PenTypes.DarkRed)); // |.
            AddSegment(segments, new TyloxSegment(top.Id, offset: 0, crossSlide: 0f, pos: 1f, len: left.Length, angle: 1f, penIndex: (int)PenTypes.DarkBlue)); // .|
            AddSegment(segments, new TyloxSegment(left.Id, offset: 0, crossSlide: 0f, pos: 1f, len: top.Length, angle: 0.5f, penIndex: (int)PenTypes.Black));  // _
        }

        private BaseSegment AddSegment(List<BaseSegment> segments, BaseSegment seg)
        {
            segments.Add(seg);
            return seg;
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
