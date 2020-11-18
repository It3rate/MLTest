using Microsoft.ML.Probabilistic.Distributions;
using MLTest.Sim;
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
    public partial class SimForm : Form
    {
        SimAgent _agent = new SimAgent();
        SimRenderer _sim;

        public SimForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _sim = new SimRenderer(_agent, panel1.Width, panel1.Height);

            //var g = Variable.GammaFromMeanAndVariance(0, 0.0333);

            Gaussian g0 = new Gaussian(0, 0.0333 * 0.0333);
            Gaussian g0b = new Gaussian(0.05, 0.025);
            Gaussian g0c = new Gaussian(0.1, 0.025);
            Gaussian g1 = new Gaussian(0.25, 0.05);


            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.000) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.033) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.050) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.100) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.150) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.200) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.220) * 2.0));
            Console.WriteLine(Math.Abs(g0.GetProbLessThan(-.250) * 2.0));

            var norm = g0.GetLogNormalizer();
            Console.WriteLine("-------" + g0.GetLogNormalizer());

            Console.WriteLine(g0.GetLogProb(.00));
            Console.WriteLine(g0.GetLogProb(.05));
            Console.WriteLine(g0.GetLogProb(.10));
            Console.WriteLine(g0.GetLogProb(.15));
            Console.WriteLine(g0.GetLogProb(.25));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            OnDraw(e.Graphics);
        }

        public void OnDraw(Graphics g)
        {
            var state = g.Save();

            g.ScaleTransform((_sim.Width - 1f) / 2f, (_sim.Height - 1f) / 2f);
            g.TranslateTransform(1f,1f); // center at 0,0, box is -1 to 1
            _sim.Draw(g);
            g.Restore(state);
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            Program._tyloxForm.DesktopLocation = this.DesktopLocation;
            Program._tyloxForm.Show();
        }
    }
}
