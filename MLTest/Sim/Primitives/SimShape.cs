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

        // Joints are halucinated on top of shapes. They are informational, not structural.
        List<SimJoint> Joints = new List<SimJoint>();

        public SimShape GetBounding => null; // return a constructed copy of the bounding box/oval/line

        public static SimStroke MakeRectPoints(double left, double top, double right, double bottom) => null; // 5 points, center +tlbr
        public static SimStroke MakeOval(double cx, double cy, double rx, double ry) => null; // 5 points, center + NESW

        public bool IsClosed => false;
        // The exact point will be inside of the enclosed line, if this is a closed stroke. Otherwise just the approximate center of mass.
        public SimPosition Interior => null;
        public SimPosition GetPosition(SimWhere where) => null;
    }
}
