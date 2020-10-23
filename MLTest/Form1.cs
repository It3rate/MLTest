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
        BoxGen drawable;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            //var t = new Test1(this);
            drawable = new BoxGen();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            drawable.OnDraw(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button == MouseButtons.Middle)
            {
                drawable.GenerateLocalData();
                lbTitle.Text = "Refresh";
            }
            else
            {
                //drawable.TransformAll();
                drawable.DrawTarget = (e.Button == MouseButtons.Left) ? DrawTarget.Mutated : DrawTarget.Predictions;

                lbTitle.Text = (drawable.DrawTarget == DrawTarget.Mutated) ? "Mutated Data" : "Predictions";
            }
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            drawable.DrawTarget = DrawTarget.Truth;
            lbTitle.Text = "Ground Truth";
            Invalidate();
        }

        private void OnColorVariationChange(object sender, EventArgs e)
        {
        }

        private void slColor_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                drawable.DrawTarget = DrawTarget.Truth;
                lbTitle.Text = "Ground Truth";
            }
            else
            {
                drawable.DrawTarget = DrawTarget.Mutated;
                lbTitle.Text = "Color Adjust";
                var stdDev = e.NewValue * 0.0006f;
                drawable.RecolorDesigns(drawable.Mutated, stdDev);
                Console.WriteLine("val: " + stdDev);
            }
            Invalidate();
        }

    }
}
