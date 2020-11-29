using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis.Agent
{
	public class VisSkill
	{
		// Use separate pads for imagined and seen elements

		// pads are a complex version of the visual-motor connection of neurons in early life: see, process/transmit, signal muscles
		// Sensory capture - Input (visualization)
		// Understanding - processing to primitive shapes on pad (Primitives)
		// Planning - Lookup of letter recipes (cell)
		// Deciding - Form next stroke plan (thought)
		// Transmitting - encode and send to renderer (via nervous system)
		// Motor - render (muscle motion)
		// Feedback - back to step 1

		VisPad<Point> focusPad = new VisPad<Point>();
		VisPad<Stroke> viewPad = new VisPad<Stroke>();

		public Stroke[] LetterR()
		{
            // LB: imagine letterbox
            // LB0: find left vertical line (need to find sub pieces of imagined elements - if using the whole rect can just reference it)
            // LeftS: make left stroke along LB0 with top and bottom tipJoints
            // C: imagine circle to the top right (no need to find it as it is referenced as a whole)
            // LoopS: FullStroke loop with joints LeftS:(corner), C:1 (top tangent), LeftS:0.5 (butt)
            // Find loop stroke
            // TailS: Make with joints LoopS:0.8 (butt), LeftS:1 offset 1 (tip)


            var letterbox = new Rectangle(0.4f, 0.5f, 0.1f, 0.1f);
			focusPad.Elements.Add(letterbox);

			var leftLine = letterbox.GetLine(CompassDirection.W);
			var leftStroke = leftLine.FullStroke;
			viewPad.Elements.Add(leftStroke);

			var seenLeftStroke = viewPad.GetSimilar(leftLine);
			var radius = seenLeftStroke.NodeAt(0.25f, 0f);
			var center = seenLeftStroke.NodeAt(0.25f, 0.5f);
			var topCircle = new Circle(center, radius);

			var circleNode = new Node(topCircle, 0);

			//var loopStartJoint = new VisJoint(seenLeftStroke.StartNode, circleNode0, VisJointType.Corner);
			//var circleJoint = new VisJoint(circleNode0, circleNode1, VisJointType.Curve);
			//var loopEndJoint = new VisJoint(circleNode1, seenLeftStroke.NodeAt(0.5f), VisJointType.Butt);

			var loopStroke = new Stroke(seenLeftStroke.StartNode, circleNode, seenLeftStroke.NodeAt(0.5f));

			return viewPad.Elements.ToArray();
        }

		public Stroke[] LetterC()
		{
			var letterbox = new Rectangle(0.4f, 0.5f, 0.1f, 0.1f);
			focusPad.Elements.Add(letterbox);

			var circle = new Circle(letterbox.Center, letterbox.GetPoint(CompassDirection.E), ClockDirection.CCW);
			var startC = circle.NodeAt(0.1f);
			var endC = circle.NodeAt(0.9f);
			var loopStroke = new Stroke(startC, endC);
			viewPad.Elements.Add(loopStroke);

			return viewPad.Elements.ToArray();
        }
	}
}
