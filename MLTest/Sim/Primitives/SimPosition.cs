using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public abstract class SimOrientation { }

    public class SimPosition : SimOrientation
    {
        // hold reference: rect, oval, 5grid hex, 9 grid hex (or dynamic sized hex grids)
        // positions are means in a gaussian, sigma is extra variable
        // locator queries can be exact, point areas, line areas, or 2d shaped areas 

        // Start with just letterbox rect Center, N NE E SE S SW W NW and H/V lines

        // Or separate X,Y into start, space mid, space end (10,30,20,30,10)
        // a set of XY choices as gaussians give an area, segment or point

        public SimZone X { get; set; }
        public SimZone Y { get; set; }

        public double ExactX => X.Exact;
        public double ExactY => Y.Exact;

        public SimPosition(float x, float y)
        {
            X = new SimZone(x);
            Y = new SimZone(y);
        }
    }
}
