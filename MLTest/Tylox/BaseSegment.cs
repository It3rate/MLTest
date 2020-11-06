using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Tylox
{
    public abstract class BaseSegment
    {
        protected static int idCounter;
        public int Id { get; set; }

        public static List<BaseSegment> Segments { get; } = new List<BaseSegment>();

        public int ParentId { get; set; }
        public int PenIndex { get; set; }

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

        public BaseSegment(int parentId = -1, int penIndex = 0)
        {
            Id = idCounter++;
            ParentId = parentId;
            PenIndex = penIndex;
        }
        protected BaseSegment GetParent()
        {
            BaseSegment result = null;
            if (ParentId >= 0 && ParentId < Segments.Count)
            {
                result = Segments[ParentId];
            }
            return result;
        }
        public void Draw(Pen pen, Graphics g)
        {
            g.DrawLine(pen, Start, End);
        }
    }
}
