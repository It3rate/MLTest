using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public enum SimElementType { Any, EndPoint, Node, Edge, Stroke, Shape, Joint }
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

        private int _skeletonShapeCount;

        // Hex grid, spots are approximate because points can be different sizes (rod vs cone, fovea density) and may not align perfectly.
        // Also signals can drift to neighbours, and positions are always estimates anyway.
        // Make and actual hexgrid class.
        private List<SimSection> PadBitmap = new List<SimSection>();
        public List<SimShape> Shapes { get; } = new List<SimShape>();
        public SimShape CurrentShape { get; set; }

        private SimFocusPad(PadType type, int horizontalSize, int verticalSize)
        {
            Type = type;
            Pad = new int[horizontalSize, verticalSize];
        }

        public SimElement GetMostSimilar(SimElement element)
        {
            SimElement result = null;
            double similarity = 0;
            foreach (var shape in Shapes)
            {
                double max;
                var mostSimilar = shape.GetMostSimilar(element, out max);
                if(max > similarity)
                {
                    similarity = max;
                    result = mostSimilar;
                }
            }
            return result;
        }
        // This can be from the pad structure, or from the held elements
        public SimStroke GetStroke(SimWhere where)
        {
            SimStroke result = null;
            double similarity = 0;
            foreach (var stroke in AllStrokes(where.ShapeType))
            {
                double max = Utils.ComparePoints(where.Locator.AnchorPoint, stroke.Center);
                if (max > similarity)
                {
                    similarity = max;
                    result = stroke;
                }
            }
            // need to reorient to be top first or left first depending on how v/h this is. Should return a clone.
            return result.GetOrientedClone();
        }

        public SimElement GetMostSimilar(SimWhere where)
        {
            SimElement result = null;
            switch (where.ElementType)
            {
                case SimElementType.Shape:
                    break;
                case SimElementType.Stroke:
                    result = GetStroke(where);
                    break;
                case SimElementType.EndPoint:
                    break;
                case SimElementType.Node:
                    break;
                case SimElementType.Edge:
                    break;
                case SimElementType.Joint:
                    break;
            }
            return result;
        }

        public IEnumerable<SimNode> AllEndpoints(SimStructuralType shapeType = SimStructuralType.Any)
        {
            foreach (var shape in Shapes)
            {
                if (shapeType == SimStructuralType.Any || shape.StructuralType == shapeType)
                {
                    foreach (var stroke in shape.Strokes)
                    {
                        yield return stroke.Start;
                        yield return stroke.End;
                    }
                }
            }
        }
        public IEnumerable<SimEdge> AllEdges(SimStructuralType shapeType = SimStructuralType.Any)
        {
            foreach (var shape in Shapes)
            {
                if (shapeType == SimStructuralType.Any || shape.StructuralType == shapeType)
                {
                    foreach (var stroke in shape.Strokes)
                    {
                        foreach (var edge in stroke.Edges)
                        {
                            yield return edge;
                        }
                    }
                }
            }
        }
        public IEnumerable<SimElement> AllNodes(SimStructuralType shapeType = SimStructuralType.Any)
        {
            foreach (var shape in Shapes)
            {
                if (shapeType == SimStructuralType.Any || shape.StructuralType == shapeType)
                {
                    foreach (var stroke in shape.Strokes)
                    {
                        yield return stroke.Start;
                        foreach (var edge in stroke.Edges)
                        {
                            yield return edge;
                        }
                        yield return stroke.End;
                    }
                }
            }
        }
        public IEnumerable<SimStroke> AllStrokes(SimStructuralType shapeType = SimStructuralType.Any)
        {
            foreach (var shape in Shapes)
            {
                if (shapeType == SimStructuralType.Any || shape.StructuralType == shapeType)
                {
                    foreach (var stroke in shape.Strokes)
                    {
                        yield return stroke;
                    }
                }
            }
        }
        public IEnumerable<SimJoint> AllJoints(SimStructuralType shapeType = SimStructuralType.Any)
        {
            foreach (var shape in Shapes)
            {
                if (shapeType == SimStructuralType.Any || shape.StructuralType == shapeType)
                {
                    foreach (var joint in shape.Joints)
                    {
                        yield return joint;
                    }
                }
            }
        }


        // Load something into Pad. When 'looking' at an image, the coverts it from cartesian, scales to grid, and maps joints.
        // It can also look at memory, skill expectations, diffs etc
        public void LookAt(/*image etc*/) { }

        public void Erase()
        {
            Shapes.RemoveRange(_skeletonShapeCount, Shapes.Count - _skeletonShapeCount);
            // clear bitmap
        }

        public SimShape AddShape()
        {
            if(CurrentShape != null)
            {
                CurrentShape.StructuralType = SimStructuralType.Exisiting;
            }
            CurrentShape = new SimShape(SimStructuralType.Current);
            Shapes.Add(CurrentShape);
            return CurrentShape;
        }

        public void AddStroke(SimStroke stroke)
        {
            if(CurrentShape == null)
            {
                AddShape();
            }
            CurrentShape.AddStroke(stroke);
        }

        public void AddJoint(SimJoint joint)
        {
            CurrentShape.AddJoint(joint);
        }
        public void CreateRectSkeleton()
        {
            Shapes.Clear();
            var rect = SimShape.CreateRect(SimStructuralType.PadStructure, 0, 0, 1, 1, true);
            Shapes.Add(rect);
            _skeletonShapeCount = Shapes.Count;
        }

        public static SimFocusPad CreateRectPad(int w, int h)
        {
            var result = new SimFocusPad(PadType.Rectangle, w, h);
            result.CreateRectSkeleton();
            return result;
        }
    }
}
