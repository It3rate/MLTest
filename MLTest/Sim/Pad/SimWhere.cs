﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    public enum SimDirection { Default, N, NE, E, SE, S, SW, W, NW, Center }

    // maybe just use shape/joint/edge/point similarity?
    // draw plan on one pad, copy those expectations to what is seen - use the expectations to find new anchors for next step.
    public class SimWhere
    {
        // Start with just PostionXY
        public SimNode Locator { get; }
        public SimStructuralType ShapeType { get; }
        public SimElementType ElementType { get; }

        public SimWhere(SimNode pos, SimStructuralType shapeType = SimStructuralType.Any, SimElementType elementType = SimElementType.Any)
        {
            Locator = pos;
            ShapeType = shapeType;
            ElementType = elementType;
        }
        public SimWhere(SimDirection dir, SimStructuralType shapeType = SimStructuralType.Any, SimElementType elementType = SimElementType.Any) : this(NodeFromDirection(dir), shapeType, elementType){ }
        
        public static SimNode NodeFromDirection(SimDirection dir)
        {
            double x = 0;
            double y = 0;
            switch (dir)
            {
                case SimDirection.N:
                    y = -1;
                    break;
                case SimDirection.NE:
                    x = -1;
                    y = -1;
                    break;
                case SimDirection.E:
                    x = -1;
                    break;        
                case SimDirection.SE:
                    x = -1;
                    y = 1;
                    break;
                case SimDirection.S:
                    y = 1;
                    break;
                case SimDirection.SW:
                    x = 1;
                    y = 1;
                    break;
                case SimDirection.W:
                    x = 1;
                    break;
                case SimDirection.NW:
                    x = 1;
                    y = -1;
                    break;
            }
            return new SimNode(null, x, y);
        }

        // Reference is a SimFocus or ScratchPad
        // Orientation - need to explicitily reorient shape origin in case strokes aren't in expected order.

        // Scales (all relative to reference size, scaled to body - default body if abstract e.g. in emotion, ideas etc)
        // barely see (1/16th inch), finger (up to inch), hand (up to foot), arm (up to yard), body (room size), walk (up to mile), days walk (miles)

        // Absolute points (no reference)
        // left, right, center, top, bottom, +corners (maybe use Hexagon system here, can use the 5grid or 9 grid dot(s) locations as position/area)
        // Segments can be divided into: left tip (10%), left inner (30%), middle (20%), right inner (30%), right tip (10%) |ooo()ooo| 
        //  - can be used for top half, most, a bit, all. % signifies 2 std dev? More precise envs means less variance from the mean. Mean can change with style.
        //  - joints are defined on the most precise locations (first endpoints, then mid points, and last inners).
        //  - ovals use this with NSEW as segments
        // Nodes probably are sensitive in the same way

        // Absolute lines
        // box sides, center lines, circle raduius's based on angle, circle diameters, any point to point above. Specify Vert, Horz etc

        // Absoulte areas
        // rects or ovals centered in center, quadrants (or hexagon equivilent). Should be scaled

        // Real objects - place with bounds, rect of oval based on shape and characteristics
        // parts can be lines or points, and should be a sub location using this system on object

        // Anchored (uses a reference element as a reference (focus), positions are relative)
        // specific: left, right, above, below, on, at, in, under, before, after
        // less specified: beside,by, near, inside, outside, far from, around, along, beyond, close, separated
        // two refs: between, across, joining, middle
        // motion: following, circling, into, from, to, toward, away from, over, past, through, up to, via
        // qualitative: like, minus, with, copy, equivilent
        // shape (self anchor): tip, edge, corner, base
        // body metaphor: foot, head, tail, body, wing, neck, elbow, branch, finger, leg

        // Percent qualifiers of Anchors
        // exact: exactly, precisely, perfectly
        // imprecise: more or less, approximately, sort of, something like, imagined
        // close: nearly, mostly, almost, a lot, more, not quite
        // mid: partly, part way, half way
        // some: starting out, a little, not much, less
        // far: nowhere near,  not really


    }
}
