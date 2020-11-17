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
        public enum PadType { Rectangle, Oval, Hexagon }

        public PadType Type { get; }
        public int[,] Pad { get; } // needs to be fast bitmap

        private int _skeletonNodeCount;
        private int _skeletonShapeCount;
        private int _skeletonJointCount;

        // Definitional skeleton of pad.
        public List<SimJoint> Joints = new List<SimJoint>();

        // Hex grid, spots are approximate because points can be different sizes (rod vs cone, fovea density) and may not align perfectly.
        // Also signals can drift to neighbours, and positions are always estimates anyway.
        // Make and actual hexgrid class.
        public List<SimSection> PadBitmap = new List<SimSection>();
        List<SimNode> Nodes { get; } = new List<SimNode>();
        List<SimShape> Shapes { get; } = new List<SimShape>();

        private SimFocusPad(PadType type, int horizontalSize, int verticalSize)
        {
            Type = type;
            Pad = new int[horizontalSize, verticalSize];
        }

        // This can be from the pad structure, or from the held elements
        public SimStroke GetStroke(SimWhere where, List<SimJoint> expectations) => null;

        // Load something into Pad. When 'looking' at an image, the coverts it from cartesian, scales to grid, and maps joints.
        // It can also look at memory, skill expectations, diffs etc
        public void LookAt(/*image etc*/) { }

        public void Erase()
        {
            Nodes.RemoveRange(_skeletonNodeCount, Nodes.Count - _skeletonNodeCount);
            Shapes.RemoveRange(_skeletonShapeCount, Shapes.Count - _skeletonShapeCount);
            Joints.RemoveRange(_skeletonJointCount, Joints.Count - _skeletonJointCount);
            // clear bitmap
        }

        public void CreateRectSkeleton()
        {
            Nodes.Clear();
            Shapes.Clear();
            Joints.Clear();

            var ct = new SimNode(null, 0.0, 0.0);
            var tl = new SimNode(null, -1.0, -1.0); // make tl -1,-1 - looking up is 'away' from the action we mostly care about (level and below, existing on the ground)
            var tr = new SimNode(null, 1.0, -1.0);
            var br = new SimNode(null, 1.0, 1.0);
            var bl = new SimNode(null, -1.0, 1.0);
            Nodes.AddRange(new[] { ct, tl, tr, br, bl });
            var box = new SimShape(tl, tr, br, bl);
            Shapes.Add(box);

            var tlJoint = new SimJoint(JointType.Corner, new SimEdge(box[0], 0), new SimEdge(box[3], 1));
            var trJoint = new SimJoint(JointType.Corner, new SimEdge(box[0], 1), new SimEdge(box[1], 0));
            var brJoint = new SimJoint(JointType.Corner, new SimEdge(box[1], 1), new SimEdge(box[2], 0));
            var blJoint = new SimJoint(JointType.Corner, new SimEdge(box[2], 1), new SimEdge(box[3], 0));
            Joints.AddRange(new[] { tlJoint, trJoint, brJoint, blJoint });

            _skeletonNodeCount = Nodes.Count;
            _skeletonShapeCount = Shapes.Count;
            _skeletonJointCount = Joints.Count;
        }

        public static SimFocusPad CreateRectPad(int w, int h)
        {
            var result = new SimFocusPad(PadType.Rectangle, w, h);
            result.CreateRectSkeleton();
            return result;
        }
    }
}
