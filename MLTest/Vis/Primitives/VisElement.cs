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

	    public abstract VisPoint AnchorPoint { get; }

        public virtual VisNode Center { get; protected set; }
	    public virtual VisNode[] Bounds { get => null; }

	    public abstract float Length();
        public virtual float CompareTo(VisElement element) => 0;
        public virtual VisPoint GetPointUsing(float position, float offset) => null;
        public virtual VisPoint GetPointUsing(VisNode node) => null;
    }

    public enum VisElementType { Any, Point, Node, Circle, Square, Rectangle, Oval, Joint, Stroke, Shape }
}
