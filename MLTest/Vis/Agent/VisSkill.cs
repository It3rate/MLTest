using System;
using System.Collections.Generic;
using System.Linq;
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
	    VisPad<VisStroke> viewPad = new VisPad<VisStroke>();

	    public void LetterR()
	    {
            var letterbox = new Rectangle(0.4f, 0.5f, 0.1f, 0.1f);
            focusPad.Elements.Add(letterbox);

            // LB: imagine letterbox
            // LB0: find left vertical line (need to find sub pieces of imagined elements - if using the whole rect can just reference it)
            // LeftS: make left stroke along LB0 with top and bottom tipJoints
            // C: imagine circle to the top right (no need to find it as it is referenced as a whole)
            // LoopS: Stroke loop with joints LeftS:(corner), C:1 (top tangent), LeftS:0.5 (butt)
            // Find loop stroke
            // TailS: Make with joints LoopS:0.8 (butt), LeftS:1 offset 1 (tip)

        }
    }
}
