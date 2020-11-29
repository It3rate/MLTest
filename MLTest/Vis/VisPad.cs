using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	public enum PadType { Rectangle, Oval, Hexagon }

    public class VisPad<T>
    {
	    public PadType Type { get; } = PadType.Rectangle;

        public List<T> Elements { get; } = new List<T>();

	    public VisElement GetByLocation(VisElement reference, VisLocator locator) => null;
        public VisElement GetNearby(Node node, VisElementType elementType = VisElementType.Any) => null;

        public VisJoint GetSimilar(VisJoint joint) => null;
        public IPath GetSimilar(IPath element) => null;
    }
}
