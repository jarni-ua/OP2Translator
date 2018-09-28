namespace StalTran
{
    partial class Completion
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
            this.m_lbProposals = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // m_lbProposals
            // 
            this.m_lbProposals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_lbProposals.FormattingEnabled = true;
            this.m_lbProposals.Location = new System.Drawing.Point(0, 0);
            this.m_lbProposals.Name = "m_lbProposals";
            this.m_lbProposals.Size = new System.Drawing.Size(284, 262);
            this.m_lbProposals.TabIndex = 0;
            this.m_lbProposals.SelectedIndexChanged += new System.EventHandler(this.m_lbProposals_SelectedIndexChanged);
            this.m_lbProposals.DoubleClick += new System.EventHandler(this.m_lbProposals_DoubleClick);
            // 
            // Completion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.ControlBox = false;
            this.Controls.Add(this.m_lbProposals);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Completion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Completion";
            this.Deactivate += new System.EventHandler(this.Completion_Deactivate);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Completion_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Completion_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_lbProposals;
    }
}