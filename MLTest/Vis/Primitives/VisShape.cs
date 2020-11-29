﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public class VisShape : VisElement
    {
	    public override VisElementType ElementType => VisElementType.Shape;

	    public override float Length => 0;
	    public override Point Anchor { get; } = null;

	    public List<Stroke> Strokes { get; } = new List<Stroke>();

        // computed
	    public List<VisJoint> ComputedJoints { get; } = new List<VisJoint>();

	    public float IsInside(VisElement element) => 0;

	    public VisShape(params Stroke[] strokes)
	    {
		    Strokes.AddRange(strokes);
	    }
    }
}
