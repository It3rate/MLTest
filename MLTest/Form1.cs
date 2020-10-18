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
            //drawable.TransformAll();
            drawable.DrawTarget = (e.Button == MouseButtons.Left) ? DrawTarget.Truth : DrawTarget.Predictions;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            drawable.DrawTarget = DrawTarget.Mutated;
            Invalidate();
        }


    }
}
