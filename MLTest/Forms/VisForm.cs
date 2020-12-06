using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MLTest.Vis;

namespace MLTest.Forms
{
    public partial class VisForm : Form
    {
	    VisAgent _agent;
        VisRenderer _renderer;
        public VisForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            _renderer = new VisRenderer(panel1.Width, panel1.Height);
            _agent = new VisAgent(_renderer);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
	        OnDraw(e.Graphics);
        }

        public void OnDraw(Graphics g)
        {
            _agent.Draw(g);
        }

        private void btNext_Click(object sender, EventArgs e)
        {
	        Program.NextForm();
        }
        private void _formClosed(object sender, FormClosedEventArgs e)
        {
	        Application.Exit();
        }
    }
}
