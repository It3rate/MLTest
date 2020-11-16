using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    // A FocusPad is variable size, and each side comes with a center and bounds, so creating this is automatically a referernce.
    // It should have a bitmap (hex grid) pad, and joints

    // List of ref objects, and how they are located (locator, features, surprise diff etc)
    // Maybe 'look' at an area (in the shape or hexgrid sense), and then collect joints from there - from that discover strokes or shapes?
    // Probably also need to collect center/sizes of eclosed and partially enclosed areas.
    public class SimFocusPad
    {
        // Definitional skeleton of pad.
        List<SimJoint> PadJoints = new List<SimJoint>();

        // Hex grid, spots are approximate because points can be different sizes (rod vs cone, fovea density) and may not align perfectly.
        // Also signals can drift to neighbours, and positions are always estimates anyway.
        // Make and actual hexgrid class.
        List<SimZone> PadBitmap = new List<SimZone>();

        List<SimShape> Shapes = new List<SimShape>();

        // This can be from the pad structure, or from the held elements
        public SimStroke GetStroke(SimWhere where, List<SimJoint> expectations) => null;

        // Load something into Pad. When 'looking' at an image, the coverts it from cartesian, scales to grid, and maps joints.
        // It can also look at memory, skill expectations, diffs etc
        public void LookAt(/*image etc*/) { }
    }
}
