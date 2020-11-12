using MLTest.Tylox;
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

namespace MLTest.Forms
{
    public partial class TyloxForm : Form
    {
        TyloxRenderer _tylox;
        public TyloxForm(TyloxRenderer generator)
        {
            InitializeComponent();

            DoubleBuffered = true;
            _tylox = generator;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            OnDraw(e.Graphics);
        }

        public void OnDraw(Graphics g)
        {
            float w = _tylox.Width;
            float h = _tylox.Height;
            var state = g.Save();
            
            g.ScaleTransform(w, h);
            //g.TranslateTransform(1f,1f); // center at 0,0, box is -1 to 1
            _tylox.Draw(w, g);
            g.Restore(state);
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            Program._interactForm.DesktopLocation = this.DesktopLocation;
            Program._interactForm.Show();
        }
    }
}
