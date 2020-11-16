using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Sim
{
    /// <summary>
    /// The mind that holds the focus, memory, reasoning, skills, cummunication and judgement. 
    /// </summary>
    public class SimAgent
    {
        List<SimFocusPad> FocusPads { get; } = new List<SimFocusPad>();
    }
}
