using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLTest
{
    public partial class InteractForm : Form
    {
        DesignGenerator _generator;
        public InteractForm(DesignGenerator generator)
        {
            InitializeComponent();

            DoubleBuffered = true;
            _generator = generator;
        }
    }
}
