using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public class VisPad<T>
    {
        public List<T> Elements { get; } = new List<T>();

	    public VisElement GetByLocation(VisElement reference, VisLocator locator) => null;
        public VisElement GetNearby(VisNode node, VisElementType elementType = VisElementType.Any) => null;
	    public VisElement GetSimilar(VisElement element) => null;
    }
}
