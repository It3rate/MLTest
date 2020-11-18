using Microsoft.ML.Probabilistic.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public enum SimShapeType { Any = 0, PadStructure, Focusing, Current, Previous, SecondLast, Exisiting, Planning, Comparing } 

    public class SimShape : SimElement
    {
        public SimShapeType ShapeType;

        public List<SimNode> Nodes { get; } = new List<SimNode>();

        // SimShape may need to also recognize enclosed, or partially enclosed spaces. SimJoint sibling class like SimContainer?
        public List<SimStroke> Strokes = new List<SimStroke>();

        // Joints are halucinated on top of shapes. They are informational, not structural.
        public List<SimJoint> Joints = new List<SimJoint>();

        public SimStroke this[int index] => Strokes[index];

        public SimNode Center { get; } = new SimNode(null, 0, 0); // default
        public SimNode[] Bounds => null;

        public SimShape(SimShapeType shapeType, params SimStroke[] strokes)
        {
            ShapeType = shapeType;
            Strokes.AddRange(strokes);
        }
        /// <summary>
        /// Adds a polyline with mutiple strokes.
        /// </summary>
        /// <param name="nodes">Each node in polyline, must contain two or more nodes.</param>
        public void AddPolyline(bool isClosed, params SimNode[] nodes)
        {
            Nodes.AddRange(nodes);
            if (nodes.Length > 1)
            {
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    Strokes.Add(new SimStroke(nodes[i], nodes[i + 1]));
                }
            }
            if (isClosed)
            {
                Strokes.Add(new SimStroke(nodes[nodes.Length - 1], nodes[0]));
            }
        }

        public SimShape GetBounding => null; // return a constructed copy of the bounding box/oval/line

        public static SimStroke MakeRectPoints(double left, double top, double right, double bottom) => null; // 5 points, center +tlbr
        public static SimStroke MakeOval(double cx, double cy, double rx, double ry) => null; // 5 points, center + NESW

        public bool IsClosed => false;
        // The exact point will be inside of the enclosed line, if this is a closed stroke. Otherwise just the approximate center of mass.
        public SimNode Interior => null;

        public IEnumerable<SimElement> AllNodes()
        {
            foreach (var stroke in Strokes)
            {
                yield return stroke.Start;
                foreach (var edge in stroke.Edges)
                {
                    yield return edge;
                }
                yield return stroke.End;
            }
        }
        public IEnumerable<SimNode> AllEndpoints()
        {
            foreach (var stroke in Strokes)
            {
                yield return stroke.Start;
                yield return stroke.End;
            }
        }
        public IEnumerable<SimEdge> AllEdges()
        {
            foreach (var stroke in Strokes)
            {
                foreach(var edge in stroke.Edges)
                {
                    yield return edge;
                }
            }
        }
        public IEnumerable<SimJoint> AllJoints()
        {
            foreach (var joint in Joints)
            {
                yield return joint;
            }
        }

        public void AddStroke(SimStroke stroke)
        {
            Strokes.Add(stroke);
            // check for joints
        }

        public static SimShape CreateRect(SimShapeType shapeType, double x, double y, double w, double h, bool includeCenter = true)
        {
            var result = new SimShape(shapeType);

            if (includeCenter)
            {
                result.Nodes.Add(new SimNode(null, 0.0, 0.0));
            }
            var tl = new SimNode(null, -1.0, -1.0); // make tl -1,-1 - looking up is 'away' from the action we mostly care about (level and below, existing on the ground)
            var tr = new SimNode(null, 1.0, -1.0);
            var br = new SimNode(null, 1.0, 1.0);
            var bl = new SimNode(null, -1.0, 1.0);

            result.AddPolyline(true, tl, tr, br, bl);

            var tlJoint = new SimJoint(JointType.Corner, new SimEdge(result[0], 0), new SimEdge(result[3], 1));
            var trJoint = new SimJoint(JointType.Corner, new SimEdge(result[0], 1), new SimEdge(result[1], 0));
            var brJoint = new SimJoint(JointType.Corner, new SimEdge(result[1], 1), new SimEdge(result[2], 0));
            var blJoint = new SimJoint(JointType.Corner, new SimEdge(result[2], 1), new SimEdge(result[3], 0));
            result.Joints.AddRange(new[] { tlJoint, trJoint, brJoint, blJoint });

            return result;
        }
        public override double CompareTo(SimElement element)
        {
            // bounds, rough shape, edge matches
            // maybe use small bitmap
            return base.CompareTo(element);
        }
        public SimElement GetMostSimilar(SimElement element, out double similarity)
        {
            SimElement result = this;
            similarity = 0;
            // account for possible shape comparison
            foreach (var stroke in Strokes)
            {
                if(element is SimStroke)
                {
                    var comp = stroke.CompareTo(element);
                    if(comp > similarity)
                    {
                        similarity = comp;
                        result = stroke;
                    }
                }
                else
                {
                    foreach (var el in stroke.AllElements)
                    {
                        var comp = el.CompareTo(element);
                        if (comp > similarity)
                        {
                            similarity = comp;
                            result = el;
                        }
                    }
                }
            }
            return result;
        }
    }
}
