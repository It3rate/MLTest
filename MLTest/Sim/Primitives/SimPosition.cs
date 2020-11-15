using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimSpot
    {
        public double Exact { get; }
        private Gaussian _location;
        public Gaussian Location { get => _location; }
        private int _encoding;
        private int _sign;

        public bool IsEndPoint => _encoding == -2 || _encoding == 2;
        public bool IsMidPoint => _encoding == 0;
        public bool IsInnerArea => _encoding == -1 || _encoding == 1;
        public bool IsFirstHalf => _encoding < 0;
        public bool IsLastHalf => _encoding >0;

        public SimSpot(double loc)
        {
            Exact = loc;
            _location = SetEncoding(loc);
        }
        private Gaussian SetEncoding(double n)
        {
            _sign = Math.Sign(n);
            n = Math.Abs(n);
            double stdDev;
            double mean;
            if (n < 0.1)
            {
                _encoding = -2;
                mean = 0;
                stdDev = 0.1 / 3.0;
            }
            else if (n < 0.4)
            {
                _encoding = -1;
                mean = 0.25;
                stdDev = 0.1 / 2.0;
            }
            else if (n < 0.6)
            {
                _encoding = 0;
                mean = 0.5;
                stdDev = 0.1 / 3.0;
            }
            else if (n < 0.9)
            {
                _encoding = 1;
                mean = 0.5;
                stdDev = 0.1 / 2.0;
            }
            else
            {
                _encoding = 2;
                mean = 1.0;
                stdDev = 0.1 / 2.0;
            }
            return new Gaussian(mean, stdDev);
        }
    }
    public class SimSize
    {
        public enum HumanScale { Micro = -5, Flea = -4, Finger = -3, Hand = -2, Arm = -1, Body = 0, Room = 1, Camp = 2, Throw = 3, Walk = 4, Journey = 5 }
        private double[] ScaleRatios = new double[] {0.0001, 0.0033, 0.0366, 0.111, 0.333, 1.0, 3.0, 6.0, 9.0, 90.0, 1000.0 };

    }
    public class SimAngle
    {
        public SimSpot Direction { get; set; }
        public SimSpot Radius { get; set; } // negative is away
        public SimAngle(float direction, float radius)
        {
            Direction = new SimSpot(direction);
            Radius = new SimSpot(radius);
        }
    }
    public class SimPosition
    {
        // hold reference: rect, oval, 5grid hex, 9 grid hex (or dynamic sized hex grids)
        // positions are means in a gaussian, sigma is extra variable
        // locator queries can be exact, point areas, line areas, or 2d shaped areas 

        // Start with just letterbox rect Center, N NE E SE S SW W NW and H/V lines

        // Or separate X,Y into start, space mid, space end (10,30,20,30,10)
        // a set of XY choices as gaussians give an area, segment or point

        public SimSpot X { get; set; }
        public SimSpot Y { get; set; }

        public double ExactX => X.Exact;
        public double ExactY => Y.Exact;

        public SimPosition(float x, float y)
        {
            X = new SimSpot(x);
            Y = new SimSpot(y);
        }
    }
}
