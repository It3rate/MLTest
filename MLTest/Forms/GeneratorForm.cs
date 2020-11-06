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
    public partial class GeneratorForm : Form
    {
        public DesignGenerator Generator { get; set; }
        public DrawTarget CurrentDrawTarget = DrawTarget.Truth;

        public List<Design> _mutated;
        public List<Design> _targets;
        public List<Design> _predictions;

        public GeneratorForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            //var t = new Test1(this);
            Generator = new DesignGenerator();

            _mutated = Generator.Mutated;
            _targets = Generator.Targets;
            _predictions = Generator.Predictions;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Middle)
            {
                Generator.GenerateLocalData(50);
                lbTitleX.Text = "Refresh";
            }
            else
            {
                //drawable.TransformAll();
                CurrentDrawTarget = (e.Button == MouseButtons.Left) ? DrawTarget.Mutated : DrawTarget.Predictions;

                lbTitleX.Text = (CurrentDrawTarget == DrawTarget.Mutated) ? "Mutated Data" : "Predictions";
            }
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            CurrentDrawTarget = DrawTarget.Truth;
            lbTitleX.Text = "Ground Truth";
            Invalidate();
        }

        private void OnColorVariationChange(object sender, EventArgs e)
        {
        }

        bool dbuf;
        private void slColor_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                CurrentDrawTarget = DrawTarget.Truth;
                lbTitleX.Text = "Ground Truth";
                Generator.TestModel();
            }
            else
            {
                CurrentDrawTarget = DrawTarget.Predictions;
                lbTitleX.Text = "Color Adjust Inference";
                var stdDev = e.NewValue * 0.0006f;
                Generator.RecolorDesigns(_mutated, stdDev);
                if (dbuf)
                {
                    Generator.TestModel();
                    lbColor.Text = "Color Variation (stdDev): " + stdDev.ToString("F4");
                }
                dbuf = !dbuf;
                Console.WriteLine("val: " + _mutated[0].BoxesRef[0].ColorOffset);
            }
            Invalidate();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            Program._tyloxForm.DesktopLocation = this.DesktopLocation;
            Program._tyloxForm.Show();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnDraw(e.Graphics);
        }

        public void OnDraw(Graphics g)
        {
            var toDraw = CurrentDrawTarget == DrawTarget.Truth ? _targets : CurrentDrawTarget == DrawTarget.Mutated ? _mutated : _predictions;
            for (int i = 0; i < toDraw.Count; i++)
            {
                var state = g.Save();
                ScaleTranslateTo(i, g);
                toDraw[i].Draw(g);
                g.Restore(state);
            }

            ScaleTranslateTo(toDraw.Count, g);
        }

        private void ScaleTranslateTo(int index, Graphics g)
        {
            int orgX = 50;
            int orgY = 30;
            int cols = 10;
            float w = 50;
            float h = 50;
            float marg = 20;
            float left = (index % cols) * (w + marg) + orgX;
            float top = (int)(index / cols) * (h + marg) + orgY;
            g.ScaleTransform(w, h);
            g.TranslateTransform(left / w, top / h);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }

    public enum DrawTarget
    {
        Truth,
        Mutated,
        Predictions,
    }

}
