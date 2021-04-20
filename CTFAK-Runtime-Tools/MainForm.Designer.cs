using System.ComponentModel;

namespace CTFAK_Runtime_Tools
{
    partial class MainForm
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
            this.updateProcesses = new System.Windows.Forms.Button();
            this.procList = new System.Windows.Forms.ListBox();
            this.testBtn1 = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.currentFrameInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // updateProcesses
            // 
            this.updateProcesses.BackColor = System.Drawing.Color.Black;
            this.updateProcesses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateProcesses.Location = new System.Drawing.Point(12, 12);
            this.updateProcesses.Name = "updateProcesses";
            this.updateProcesses.Size = new System.Drawing.Size(249, 71);
            this.updateProcesses.TabIndex = 0;
            this.updateProcesses.Text = "Update";
            this.updateProcesses.UseVisualStyleBackColor = false;
            this.updateProcesses.Click += new System.EventHandler(this.updateProcesses_Click);
            // 
            // procList
            // 
            this.procList.BackColor = System.Drawing.Color.Black;
            this.procList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.procList.ForeColor = System.Drawing.Color.Lime;
            this.procList.FormattingEnabled = true;
            this.procList.Location = new System.Drawing.Point(12, 115);
            this.procList.Name = "procList";
            this.procList.Size = new System.Drawing.Size(249, 327);
            this.procList.TabIndex = 1;
            // 
            // testBtn1
            // 
            this.testBtn1.BackColor = System.Drawing.Color.Black;
            this.testBtn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testBtn1.Location = new System.Drawing.Point(264, 12);
            this.testBtn1.Name = "testBtn1";
            this.testBtn1.Size = new System.Drawing.Size(116, 70);
            this.testBtn1.TabIndex = 2;
            this.testBtn1.Text = "Read";
            this.testBtn1.UseVisualStyleBackColor = false;
            this.testBtn1.Click += new System.EventHandler(this.testBtn1_Click);
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.Black;
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.ForeColor = System.Drawing.Color.Lime;
            this.searchBox.Location = new System.Drawing.Point(12, 89);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(249, 20);
            this.searchBox.TabIndex = 3;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox1.Location = new System.Drawing.Point(517, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(271, 426);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // currentFrameInfo
            // 
            this.currentFrameInfo.Location = new System.Drawing.Point(269, 94);
            this.currentFrameInfo.Name = "currentFrameInfo";
            this.currentFrameInfo.Size = new System.Drawing.Size(242, 165);
            this.currentFrameInfo.TabIndex = 5;
            this.currentFrameInfo.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.currentFrameInfo);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.testBtn1);
            this.Controls.Add(this.procList);
            this.Controls.Add(this.updateProcesses);
            this.ForeColor = System.Drawing.Color.Lime;
            this.Name = "MainForm";
            this.Text = "CTFAK-RUNTIME";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label currentFrameInfo;

        private System.Windows.Forms.RichTextBox richTextBox1;

        private System.Windows.Forms.TextBox searchBox;

        private System.Windows.Forms.Button testBtn1;

        private System.Windows.Forms.ListBox procList;
        private System.Windows.Forms.Button updateProcesses;

        #endregion
    }
}