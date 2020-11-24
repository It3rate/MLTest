using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public abstract class VisElement
    {
	    public abstract VisElementType ElementType { get; }

	    public abstract float Length { get; }
	    public abstract VisPoint AnchorPoint { get; }

        public virtual VisNode Center { get; }
	    public virtual VisNode[] Bounds { get => null; }

        public virtual float CompareTo(VisElement element) => 0;
        public virtual VisPoint GetPointOnLine(float position, float offset) => null;
        public virtual VisPoint GetPointOnLine(VisNode node) => null;
    }

    public enum VisElementType { Any, Point, Node, Circle, Square, Rectangle, Oval, Joint, Stroke, Shape }
}
