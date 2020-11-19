using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    /// <summary>
    /// A recipie to do a task, such as draw or read a letter.
    /// </summary>
    public class SimSkill
    {
        // Draw an R

        // find the line on the page
        // imagine a letterbox in the next space (create FocusPad of appropriate size)
        // get ends of a vertical line along the left
        // draw the line
        // evaluate the work in progress
        // find the line, find the top
        // get the joints and curve for the enclosed part of the P
        // draw the curve
        // evaluate the work in progress
        // find the enclosed circle, locate the SE area
        // get the joints for a line from the above focus to BR of the letterbox
        // draw the line
        // evaluate the finished work
        // check for errors, if corrections needed, new cycle for corrections
        // notify base controller of completed letter, wait for next task

        public static void LetterR(SimFocusPad pad)
        {
            // Probably should swap Pos and offset here? Or maybe vertical line isn't typical for an anchor.
            var refStroke = pad.GetStroke(new SimWhere(SimDirection.E, SimShapeType.PadStructure, SimElementType.Stroke));
            var start = new SimNode(refStroke, .1, .2);
            var end = new SimNode(refStroke, .9, .2);
            var newStroke = new SimStroke(start, end);
            pad.AddStroke(newStroke);

            refStroke = pad.GetStroke(new SimWhere(SimDirection.E, SimShapeType.Current, SimElementType.Stroke));
            start = new SimNode(refStroke, 0, 0);
            var edge = new SimEdge(refStroke, .25, .5, 0.35, 0, -1, 0.3);
            //var edge2 = new SimEdge(refStroke, .35, .3, 0.25, -.2, -1, 0.2);
            end = new SimNode(refStroke, 0.5, 0);
            newStroke = new SimStroke(start, end, new[] { edge });
            pad.AddStroke(newStroke);

            //refStroke = pad.GetStroke(new SimWhere(SimDirection.E, SimShapeType.Current, SimElementType.Stroke));
            //start = new SimNode(refStroke, 0.5, 0);
            //end = new SimNode(refStroke, 0.5, .6);
            //newStroke = new SimStroke(start, end);
            //pad.AddStroke(newStroke);

            refStroke = pad.GetStroke(new SimWhere(SimDirection.E, SimShapeType.Current, SimElementType.Stroke));
            start = new SimNode(refStroke, 0.44, 0.25);
            end = new SimNode(refStroke, 1, .5);
            newStroke = new SimStroke(start, end);
            pad.AddStroke(newStroke);

            pad.AddShape();
        }
    }
}
