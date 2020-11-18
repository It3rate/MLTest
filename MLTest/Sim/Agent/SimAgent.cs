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
        List<SimSkill> Skills { get; } = new List<SimSkill>();

        private int _activePadIndex = 0;
        public SimFocusPad ActivePad => FocusPads[_activePadIndex];

        public SimAgent()
        {
            FocusPads.Add(SimFocusPad.CreateRectPad(1, 1));
            SimSkill.LetterR(ActivePad);
        }
    }
}
