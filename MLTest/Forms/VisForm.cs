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
	    VisAgent _agent = new VisAgent();
        VisRenderer _vis;
        public VisForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            _vis = new VisRenderer(_agent, panel1.Width, panel1.Height);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
	        OnDraw(e.Graphics);
        }

        public void OnDraw(Graphics g)
        {
	        var state = g.Save();

            float w = _vis.Width;
            float h = _vis.Height;
            g.ScaleTransform(w, h);
            //g.ScaleTransform((_vis.Width - 1f) / 2f, (_vis.Height - 1f) / 2f);
            //g.TranslateTransform(1f,1f); // center at 0,0, box is -1 to 1
            _vis.Draw(g);
	        g.Restore(state);
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
