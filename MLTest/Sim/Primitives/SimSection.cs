using Microsoft.ML.Probabilistic.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public class SimSize : SimOrientation
    {
        public enum HumanScale { Micro = -5, Flea = -4, Finger = -3, Hand = -2, Arm = -1, Body = 0, Room = 1, Camp = 2, Throw = 3, Walk = 4, Journey = 5 }
        private double[] ScaleRatios = new double[] { 0.0001, 0.0033, 0.0366, 0.111, 0.333, 1.0, 3.0, 6.0, 9.0, 90.0, 1000.0 };

    }
    public class SimAngle : SimOrientation
    {
        public SimSection Direction { get; set; }
        public SimSection Radius { get; set; } // negative is away
        public SimAngle(float direction, float radius)
        {
            Direction = new SimSection(direction);
            Radius = new SimSection(radius);
        }
    }

    public class SimSection : SimOrientation
    {
        public double Exact { get; }
        private Gaussian _location;
        public Gaussian Location { get => _location; }
        private int _encoding;
        private int _sign;
        private SimDirection _startLocator;

        public double Mean { get => _location.GetMean(); }
        public double Variance { get => _location.GetVariance(); }

        public bool IsEndPoint => _encoding == -2 || _encoding == 2;
        public bool IsMidPoint => _encoding == 0;
        public bool IsInnerArea => _encoding == -1 || _encoding == 1;
        public bool IsFirstHalf => _encoding < 0;
        public bool IsLastHalf => _encoding > 0;

        public SimSection(double loc)
        {
            Exact = loc;
            _location = SetEncoding(loc);
        }
        public double Likelihood(double x)
        {
            var less = Location.GetProbLessThan(-Math.Abs(x));
            return Math.Abs(less * 2.0);
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
            return new Gaussian(mean, stdDev * stdDev);
        }
    }
}
