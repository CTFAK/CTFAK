using System.ComponentModel;

namespace DotNetCTFDumper.GUI
{
    partial class HexViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexViewForm));
            this.rawBox = new System.Windows.Forms.CheckBox();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rawBox
            // 
            this.rawBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.rawBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.rawBox.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.rawBox.Location = new System.Drawing.Point(0, 0);
            this.rawBox.Name = "rawBox";
            this.rawBox.Size = new System.Drawing.Size(646, 28);
            this.rawBox.TabIndex = 3;
            this.rawBox.Text = "Raw";
            this.rawBox.UseVisualStyleBackColor = true;
            this.rawBox.CheckedChanged += new System.EventHandler(this.rawBox_CheckedChanged);
            // 
            // hexBox1
            // 
            this.hexBox1.BackColor = System.Drawing.Color.Black;
            this.hexBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.hexBox1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 28);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.SelectionBackColor = System.Drawing.Color.FromArgb(((int) (((byte) (128)))), ((int) (((byte) (64)))), ((int) (((byte) (0)))));
            this.hexBox1.SelectionForeColor = System.Drawing.Color.Red;
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int) (((byte) (100)))), ((int) (((byte) (60)))), ((int) (((byte) (188)))), ((int) (((byte) (255)))));
            this.hexBox1.Size = new System.Drawing.Size(646, 362);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 4;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.CursorChanged += new System.EventHandler(this.hexBox1_Click);
            // 
            // sizeLabel
            // 
            this.sizeLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sizeLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.sizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.sizeLabel.Location = new System.Drawing.Point(0, 369);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(646, 21);
            this.sizeLabel.TabIndex = 5;
            this.sizeLabel.Text = "Size: 1000MB";
            // 
            // HexViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(646, 390);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.hexBox1);
            this.Controls.Add(this.rawBox);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "HexViewForm";
            this.Text = "Hex View";
            this.Load += new System.EventHandler(this.HexViewForm_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label sizeLabel;

        private Be.Windows.Forms.HexBox hexBox1;

        private System.Windows.Forms.CheckBox rawBox;

        #endregion
    }
}