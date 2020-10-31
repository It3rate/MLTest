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
    public partial class Form1 : Form
    {
        DesignGenerator generator;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            //var t = new Test1(this);
            generator = new DesignGenerator(false, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            generator.OnDraw(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Middle)
            {
                generator.GenerateLocalData();
                lbTitleX.Text = "Refresh";
            }
            else
            {
                //drawable.TransformAll();
                generator.DrawTarget = (e.Button == MouseButtons.Left) ? DrawTarget.Mutated : DrawTarget.Predictions;

                lbTitleX.Text = (generator.DrawTarget == DrawTarget.Mutated) ? "Mutated Data" : "Predictions";
            }
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            generator.DrawTarget = DrawTarget.Truth;
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
                generator.DrawTarget = DrawTarget.Truth;
                lbTitleX.Text = "Ground Truth";
                generator.TestModel();
            }
            else
            {
                generator.DrawTarget = DrawTarget.Predictions;
                lbTitleX.Text = "Color Adjust Inference";
                var stdDev = e.NewValue * 0.0006f;
                generator.RecolorDesigns(generator.Mutated, stdDev);
                if (dbuf)
                {
                    generator.TestModel();
                    lbColor.Text = "Color Variation (stdDev): " + stdDev.ToString("F4");
                }
                dbuf = !dbuf;
                Console.WriteLine("val: " + generator.Mutated[0].BoxesRef[0].ColorOffset);
            }
            Invalidate();
        }

    }
}
