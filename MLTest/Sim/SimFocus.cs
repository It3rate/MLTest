using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    // **** Maybe this is just part of scratchPad

    // List of ref objects, and how they are located (locator, features, surprise diff etc)
    // Maybe 'look' at an area (in the shape or hexgrid sense), and then collect joints from there - from that discover strokes or shapes?
    // Probably also need to collect center/sizes of eclosed and partially enclosed areas.
    public class SimFocus
    {
        List<SimJoint> FocusElements = new List<SimJoint>();
    }
}
