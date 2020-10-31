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
            _generator.TestModel();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            Program._generatorForm.DesktopLocation = this.DesktopLocation;
            Program._generatorForm.Show();
        }

        public void OnDraw(Graphics g)
        {
            var toDraw = _generator.Targets;
            var state = g.Save();
            ScaleTranslateTo(0, g);
            toDraw[0].Draw(g);
            g.Restore(state);
        }
        private void ScaleTranslateTo(int index, Graphics g)
        {
            int orgX = 0;
            int orgY = 0;
            int cols = 10;
            float w = 250;
            float h = 250;
            float marg = 2;
            float left = (index % cols) * (w + marg) + orgX;
            float top = (int)(index / cols) * (h + marg) + orgY;
            g.ScaleTransform(w, h);
            g.TranslateTransform(left / w, top / h);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            OnDraw(e.Graphics);
        }

        private void InteractForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
