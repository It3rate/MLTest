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
        MLDesignGenerator _generator;

        public List<MLDesign> _targets;
        public List<MLDesign> _mutated;
        public List<MLDesign> _predictions;
        int _currentIndex = 0;

        public DrawTarget CurrentDrawTarget = DrawTarget.Predictions;

        public InteractForm(MLDesignGenerator generator)
        {
            InitializeComponent();

            DoubleBuffered = true;
            _generator = generator;
            _targets = _generator.Targets;
            _mutated = _generator.Mutated;
            _predictions = _generator.Predictions;
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            Program._generatorForm.DesktopLocation = this.DesktopLocation;
            Program._generatorForm.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            OnDraw(e.Graphics);
        }

        Pen dotPen = new Pen(Color.Red, 1 / 200f);
        public void OnDraw(Graphics g)
        {
            var toDraw = CurrentDrawTarget == DrawTarget.Predictions ? _predictions : _mutated;
            var state = g.Save();
            ScaleTranslateTo(g);
            toDraw[_currentIndex].Draw(g);
            var targ = _mutated[_currentIndex];

            var bx = targ.BoxesRef[targ.SmallestIndex()];
            var rx = bx.Rx / 10f;
            var ry = bx.Ry / 10f;
            var rect = new RectangleF(bx.Cx - rx, bx.Cy - ry, rx * 2f, +ry * 2f);
            g.DrawEllipse(dotPen, rect);
            rx = bx.Rx / 2f;
            ry = bx.Ry / 2f;
            rect = new RectangleF(bx.Cx - rx, bx.Cy - ry, rx * 2f, +ry * 2f);
            g.DrawEllipse(dotPen, rect);
            g.Restore(state);
        }
        private void ScaleTranslateTo(Graphics g)
        {
            int orgX = 0;
            int orgY = 0;
            float w = 250;
            float h = 250;
            g.ScaleTransform(w, h);
            g.TranslateTransform(orgX / w, orgY / h);
        }



        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _currentIndex++;
                if(_currentIndex >= _predictions.Count)
                {
                    _currentIndex = 0;
                }
                CurrentDrawTarget = DrawTarget.Predictions;
            }
            panel1.Invalidate();
        }
        protected void OnMouseUp(object sender, MouseEventArgs e)
        {
            panel1.Invalidate();
        }
        private void InteractForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
