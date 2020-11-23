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
            var letterbox = pad.GetStroke(new SimWhere(SimDirection.E, SimStructuralType.PadStructure, SimElementType.Stroke));
            var start = new SimNode(letterbox, .1, .1);
            var end = new SimNode(letterbox, .9, .2);
            var newStroke = new SimStroke(start, end);
            pad.AddStroke(newStroke);

            // Make top Loop (P)
            var refStroke = pad.GetStroke(new SimWhere(SimDirection.E, SimStructuralType.Current, SimElementType.Stroke));
            start = new SimNode(refStroke, 0, 0);
            var edge = new SimEdge(refStroke, .25, .5);
            var edge2 = new SimEdge(refStroke, .45, .5);//, 0.35, 0.0, -1, 0.3);
            //var edge2 = new SimEdge(refStroke, .35, .3, 0.25, -.2, -1, 0.2);
            end = new SimNode(refStroke, 0.5, 0);
            var loopStroke = new SimStroke(start, end, edge);
            pad.AddStroke(loopStroke);

            // Add top and mid expected joints
            var topJoint = new SimJointAttempt(JointType.Corner, loopStroke, 0, refStroke, 0, 0.5);
            var midJoint = new SimJointAttempt(JointType.ButtInto, loopStroke, 0, refStroke, 0.5, -0.5);
            loopStroke.JointAttempts.AddRange(new[] { topJoint, midJoint });

            // Make R tail
            // getstroke needs an option to pass an expected joint and direction, returning the stroke with Start 0 at passed joint
            refStroke = pad.GetStroke(new SimWhere(SimDirection.NW, SimStructuralType.Current, SimElementType.Stroke));
            start = new SimNode(refStroke, .8, 0);
            edge = new SimEdge(newStroke, .5, .4, .2, -.3, 0, 0);
            //end = new SimNode(newStroke, 1, .5);
            end = new SimNode(letterbox, .9, .6);
            var tailStroke = new SimStroke(start, end); // , edge
            pad.AddStroke(tailStroke);

            // Add tail and loop expected joint
            var tailJoint = new SimJointAttempt(JointType.SplitFrom, tailStroke, 0, refStroke, 0.75, 1); // Hmm, really focused on midJoint here, 0 is bottom joint, 1 is top (of P loop)
            newStroke.JointAttempts.Add(tailJoint);

            pad.AddShape();
        }
    }
}
