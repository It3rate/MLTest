namespace MLTest
{
    partial class GeneratorForm
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
            this.lbTitleX = new System.Windows.Forms.Label();
            this.slColor = new System.Windows.Forms.HScrollBar();
            this.slLayout = new System.Windows.Forms.HScrollBar();
            this.lbColor = new System.Windows.Forms.Label();
            this.lbLayout = new System.Windows.Forms.Label();
            this.btNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbTitleX
            // 
            this.lbTitleX.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lbTitleX.Font = new System.Drawing.Font("Berlin Sans FB", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitleX.Location = new System.Drawing.Point(365, 589);
            this.lbTitleX.Name = "lbTitleX";
            this.lbTitleX.Size = new System.Drawing.Size(401, 35);
            this.lbTitleX.TabIndex = 0;
            this.lbTitleX.Text = "Original";
            this.lbTitleX.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
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
            this.lbColor.Location = new System.Drawing.Point(12, 574);
            this.lbColor.Name = "lbColor";
            this.lbColor.Size = new System.Drawing.Size(113, 20);
            this.lbColor.TabIndex = 3;
            this.lbColor.Text = "Color Variation";
            // 
            // lbLayout
            // 
            this.lbLayout.AutoSize = true;
            this.lbLayout.Location = new System.Drawing.Point(811, 574);
            this.lbLayout.Name = "lbLayout";
            this.lbLayout.Size = new System.Drawing.Size(124, 20);
            this.lbLayout.TabIndex = 4;
            this.lbLayout.Text = "Layout Variation";
            // 
            // btNext
            // 
            this.btNext.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNext.Location = new System.Drawing.Point(1043, 6);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(75, 35);
            this.btNext.TabIndex = 5;
            this.btNext.TabStop = false;
            this.btNext.Text = "Next";
            this.btNext.UseVisualStyleBackColor = true;
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 644);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.lbLayout);
            this.Controls.Add(this.lbColor);
            this.Controls.Add(this.slLayout);
            this.Controls.Add(this.slColor);
            this.Controls.Add(this.lbTitleX);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitleX;
        private System.Windows.Forms.HScrollBar slColor;
        private System.Windows.Forms.HScrollBar slLayout;
        private System.Windows.Forms.Label lbColor;
        private System.Windows.Forms.Label lbLayout;
        private System.Windows.Forms.Button btNext;
    }
}

