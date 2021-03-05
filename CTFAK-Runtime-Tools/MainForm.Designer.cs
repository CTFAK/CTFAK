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
            this.SuspendLayout();
            // 
            // updateProcesses
            // 
            this.updateProcesses.Location = new System.Drawing.Point(12, 12);
            this.updateProcesses.Name = "updateProcesses";
            this.updateProcesses.Size = new System.Drawing.Size(249, 71);
            this.updateProcesses.TabIndex = 0;
            this.updateProcesses.Text = "Update";
            this.updateProcesses.UseVisualStyleBackColor = true;
            this.updateProcesses.Click += new System.EventHandler(this.updateProcesses_Click);
            // 
            // procList
            // 
            this.procList.FormattingEnabled = true;
            this.procList.Location = new System.Drawing.Point(12, 89);
            this.procList.Name = "procList";
            this.procList.Size = new System.Drawing.Size(249, 355);
            this.procList.TabIndex = 1;
            // 
            // testBtn1
            // 
            this.testBtn1.Location = new System.Drawing.Point(264, 15);
            this.testBtn1.Name = "testBtn1";
            this.testBtn1.Size = new System.Drawing.Size(116, 67);
            this.testBtn1.TabIndex = 2;
            this.testBtn1.Text = "Read";
            this.testBtn1.UseVisualStyleBackColor = true;
            this.testBtn1.Click += new System.EventHandler(this.testBtn1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.testBtn1);
            this.Controls.Add(this.procList);
            this.Controls.Add(this.updateProcesses);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button testBtn1;

        private System.Windows.Forms.ListBox procList;
        private System.Windows.Forms.Button updateProcesses;

        #endregion
    }
}