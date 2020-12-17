using System.ComponentModel;
using System.Windows.Forms;

namespace DotNetCTFDumper.GUI
{
    partial class FrameViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameViewer));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.infoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteObjBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.infoMenuItem, this.deleteObjBtn});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // infoMenuItem
            // 
            this.infoMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.nameMenuItem, this.positionMenuItem});
            this.infoMenuItem.Name = "infoMenuItem";
            this.infoMenuItem.Size = new System.Drawing.Size(107, 22);
            this.infoMenuItem.Text = "Info";
            // 
            // nameMenuItem
            // 
            this.nameMenuItem.Name = "nameMenuItem";
            this.nameMenuItem.Size = new System.Drawing.Size(117, 22);
            this.nameMenuItem.Text = "Name";
            // 
            // positionMenuItem
            // 
            this.positionMenuItem.Name = "positionMenuItem";
            this.positionMenuItem.Size = new System.Drawing.Size(117, 22);
            this.positionMenuItem.Text = "Position";
            // 
            // deleteObjBtn
            // 
            this.deleteObjBtn.Name = "deleteObjBtn";
            this.deleteObjBtn.Size = new System.Drawing.Size(107, 22);
            this.deleteObjBtn.Text = "Delete";
            // 
            // FrameViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1008, 733);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "FrameViewer";
            this.Text = "FrameViewer";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteObjBtn;
        private System.Windows.Forms.ToolStripMenuItem infoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionMenuItem;

        #endregion
    }
}