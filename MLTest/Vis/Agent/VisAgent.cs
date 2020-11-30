using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    public class VisAgent
    {
	    public VisPad<Point> FocusPad { get; private set; }
	    public VisPad<Stroke> ViewPad { get; private set; }

	    VisSkills Skills { get; }

	    public VisAgent()
	    {
            Skills = new VisSkills();
		    FocusPad = new VisPad<Point>(250, 250);
		    ViewPad = new VisPad<Stroke>(250, 250);

		    Skills.LetterR(FocusPad, ViewPad);
	    }
    }
}
