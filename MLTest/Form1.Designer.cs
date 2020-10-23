namespace MLTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbTitle = new System.Windows.Forms.Label();
            this.slColor = new System.Windows.Forms.HScrollBar();
            this.slLayout = new System.Windows.Forms.HScrollBar();
            this.lbColor = new System.Windows.Forms.Label();
            this.lbLayout = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Berlin Sans FB", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Location = new System.Drawing.Point(477, 589);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(122, 35);
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "Original";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // slColor
            // 
            this.slColor.Location = new System.Drawing.Point(9, 598);
            this.slColor.Name = "slColor";
            this.slColor.Size = new System.Drawing.Size(328, 26);
            this.slColor.TabIndex = 1;
            this.slColor.Value = 50;
            this.slColor.Scroll += new System.Windows.Forms.ScrollEventHandler(this.slColor_Scroll);
            this.slColor.ValueChanged += new System.EventHandler(this.OnColorVariationChange);
            // 
            // slLayout
            // 
            this.slLayout.Location = new System.Drawing.Point(791, 598);
            this.slLayout.Name = "slLayout";
            this.slLayout.Size = new System.Drawing.Size(328, 26);
            this.slLayout.TabIndex = 2;
            // 
            // lbColor
            // 
            this.lbColor.AutoSize = true;
            this.lbColor.Location = new System.Drawing.Point(127, 578);
            this.lbColor.Name = "lbColor";
            this.lbColor.Size = new System.Drawing.Size(113, 20);
            this.lbColor.TabIndex = 3;
            this.lbColor.Text = "Color Variation";
            // 
            // lbLayout
            // 
            this.lbLayout.AutoSize = true;
            this.lbLayout.Location = new System.Drawing.Point(901, 578);
            this.lbLayout.Name = "lbLayout";
            this.lbLayout.Size = new System.Drawing.Size(124, 20);
            this.lbLayout.TabIndex = 4;
            this.lbLayout.Text = "Layout Variation";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 644);
            this.Controls.Add(this.lbLayout);
            this.Controls.Add(this.lbColor);
            this.Controls.Add(this.slLayout);
            this.Controls.Add(this.slColor);
            this.Controls.Add(this.lbTitle);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.HScrollBar slColor;
        private System.Windows.Forms.HScrollBar slLayout;
        private System.Windows.Forms.Label lbColor;
        private System.Windows.Forms.Label lbLayout;
    }
}

