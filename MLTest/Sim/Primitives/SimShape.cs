using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimShape
    {
        // SimShape may need to also recognize enclosed, or partially enclosed spaces. SimJoint sibling class like SimContainer?
        List<SimStroke> Strokes = new List<SimStroke>();

        public SimStroke GetBounding => null; // return a constructed copy of the bounding box/oval/line
    }
}
