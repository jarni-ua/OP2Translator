namespace StalTran
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.m_lbStringTable = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.m_rtbRus = new System.Windows.Forms.RichTextBox();
            this.m_rtbTranslation = new System.Windows.Forms.RichTextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.m_cbTranslationLanguage = new System.Windows.Forms.ComboBox();
            this.m_lWordCounter = new System.Windows.Forms.Label();
            this.m_cbIngnoreIns = new System.Windows.Forms.CheckBox();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.m_nudFontSize = new System.Windows.Forms.NumericUpDown();
            this.m_ctxMenuTranslation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToContextReplacementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToAutomaticReplacementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudFontSize)).BeginInit();
            this.m_ctxMenuTranslation.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.m_lbStringTable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1162, 515);
            this.splitContainer1.SplitterDistance = 227;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.TabStop = false;
            // 
            // m_lbStringTable
            // 
            this.m_lbStringTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_lbStringTable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_lbStringTable.FormattingEnabled = true;
            this.m_lbStringTable.Location = new System.Drawing.Point(0, 0);
            this.m_lbStringTable.Name = "m_lbStringTable";
            this.m_lbStringTable.Size = new System.Drawing.Size(227, 515);
            this.m_lbStringTable.TabIndex = 1;
            this.m_lbStringTable.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.m_lbStringTable_DrawItem);
            this.m_lbStringTable.SelectedIndexChanged += new System.EventHandler(this.m_lbStringTable_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.m_rtbRus);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.m_rtbTranslation);
            this.splitContainer2.Size = new System.Drawing.Size(931, 515);
            this.splitContainer2.SplitterDistance = 247;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.TabStop = false;
            // 
            // m_rtbRus
            // 
            this.m_rtbRus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_rtbRus.Location = new System.Drawing.Point(0, 0);
            this.m_rtbRus.Name = "m_rtbRus";
            this.m_rtbRus.ReadOnly = true;
            this.m_rtbRus.Size = new System.Drawing.Size(931, 247);
            this.m_rtbRus.TabIndex = 1;
            this.m_rtbRus.TabStop = false;
            this.m_rtbRus.Text = "";
            // 
            // m_rtbTranslation
            // 
            this.m_rtbTranslation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_rtbTranslation.Enabled = false;
            this.m_rtbTranslation.Location = new System.Drawing.Point(0, 0);
            this.m_rtbTranslation.Name = "m_rtbTranslation";
            this.m_rtbTranslation.Size = new System.Drawing.Size(931, 264);
            this.m_rtbTranslation.TabIndex = 0;
            this.m_rtbTranslation.Text = "";
            this.m_rtbTranslation.SelectionChanged += new System.EventHandler(this.m_rtbTranslation_SelectionChanged);
            this.m_rtbTranslation.TextChanged += new System.EventHandler(this.m_rtbTranslation_TextChanged);
            this.m_rtbTranslation.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_rtbTranslation_MouseUp);
            // 
            // btnFolder
            // 
            this.btnFolder.AutoSize = true;
            this.btnFolder.Location = new System.Drawing.Point(3, 3);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(72, 32);
            this.btnFolder.TabIndex = 0;
            this.btnFolder.TabStop = false;
            this.btnFolder.Text = "File";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.m_cbTranslationLanguage);
            this.splitContainer4.Panel1.Controls.Add(this.m_lWordCounter);
            this.splitContainer4.Panel1.Controls.Add(this.m_cbIngnoreIns);
            this.splitContainer4.Panel1.Controls.Add(this.btnTranslate);
            this.splitContainer4.Panel1.Controls.Add(this.btnSave);
            this.splitContainer4.Panel1.Controls.Add(this.m_nudFontSize);
            this.splitContainer4.Panel1.Controls.Add(this.btnFolder);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer4.Size = new System.Drawing.Size(1162, 554);
            this.splitContainer4.SplitterDistance = 35;
            this.splitContainer4.TabIndex = 3;
            this.splitContainer4.TabStop = false;
            // 
            // m_cbTranslationLanguage
            // 
            this.m_cbTranslationLanguage.FormattingEnabled = true;
            this.m_cbTranslationLanguage.Location = new System.Drawing.Point(158, 9);
            this.m_cbTranslationLanguage.Name = "m_cbTranslationLanguage";
            this.m_cbTranslationLanguage.Size = new System.Drawing.Size(67, 21);
            this.m_cbTranslationLanguage.TabIndex = 0;
            this.m_cbTranslationLanguage.TabStop = false;
            this.m_cbTranslationLanguage.SelectedIndexChanged += new System.EventHandler(this.m_cbTranslationLanguage_SelectedIndexChanged);
            // 
            // m_lWordCounter
            // 
            this.m_lWordCounter.AutoSize = true;
            this.m_lWordCounter.Location = new System.Drawing.Point(449, 14);
            this.m_lWordCounter.Name = "m_lWordCounter";
            this.m_lWordCounter.Size = new System.Drawing.Size(0, 13);
            this.m_lWordCounter.TabIndex = 7;
            this.m_lWordCounter.Click += new System.EventHandler(this.wordCounter_Click);
            // 
            // m_cbIngnoreIns
            // 
            this.m_cbIngnoreIns.AutoSize = true;
            this.m_cbIngnoreIns.Location = new System.Drawing.Point(358, 13);
            this.m_cbIngnoreIns.Name = "m_cbIngnoreIns";
            this.m_cbIngnoreIns.Size = new System.Drawing.Size(85, 17);
            this.m_cbIngnoreIns.TabIndex = 0;
            this.m_cbIngnoreIns.TabStop = false;
            this.m_cbIngnoreIns.Text = "Ignore Insert";
            this.m_cbIngnoreIns.UseVisualStyleBackColor = true;
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(231, 4);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(75, 32);
            this.btnTranslate.TabIndex = 6;
            this.btnTranslate.TabStop = false;
            this.btnTranslate.Text = "Translate";
            this.btnTranslate.UseVisualStyleBackColor = true;
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(81, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 32);
            this.btnSave.TabIndex = 0;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // m_nudFontSize
            // 
            this.m_nudFontSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_nudFontSize.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_nudFontSize.Location = new System.Drawing.Point(312, 8);
            this.m_nudFontSize.Maximum = new decimal(new int[] {
            28,
            0,
            0,
            0});
            this.m_nudFontSize.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.m_nudFontSize.Name = "m_nudFontSize";
            this.m_nudFontSize.Size = new System.Drawing.Size(40, 23);
            this.m_nudFontSize.TabIndex = 2;
            this.m_nudFontSize.TabStop = false;
            this.m_nudFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.m_nudFontSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.m_nudFontSize.ValueChanged += new System.EventHandler(this.m_nudFontSize_ValueChanged);
            // 
            // m_ctxMenuTranslation
            // 
            this.m_ctxMenuTranslation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToContextReplacementToolStripMenuItem,
            this.addToAutomaticReplacementToolStripMenuItem});
            this.m_ctxMenuTranslation.Name = "m_ctxMenuTranslation";
            this.m_ctxMenuTranslation.Size = new System.Drawing.Size(237, 48);
            // 
            // addToContextReplacementToolStripMenuItem
            // 
            this.addToContextReplacementToolStripMenuItem.Name = "addToContextReplacementToolStripMenuItem";
            this.addToContextReplacementToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.addToContextReplacementToolStripMenuItem.Text = "Add to context replacement";
            this.addToContextReplacementToolStripMenuItem.Click += new System.EventHandler(this.addToContextReplacementToolStripMenuItem_Click);
            // 
            // addToAutomaticReplacementToolStripMenuItem
            // 
            this.addToAutomaticReplacementToolStripMenuItem.Enabled = false;
            this.addToAutomaticReplacementToolStripMenuItem.Name = "addToAutomaticReplacementToolStripMenuItem";
            this.addToAutomaticReplacementToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.addToAutomaticReplacementToolStripMenuItem.Text = "Add to automatic replacement";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 554);
            this.Controls.Add(this.splitContainer4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "S.T.A.L.K.E.R. Translator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_nudFontSize)).EndInit();
            this.m_ctxMenuTranslation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox m_rtbRus;
        private System.Windows.Forms.RichTextBox m_rtbTranslation;
        private System.Windows.Forms.ListBox m_lbStringTable;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.NumericUpDown m_nudFontSize;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.CheckBox m_cbIngnoreIns;
        private System.Windows.Forms.ContextMenuStrip m_ctxMenuTranslation;
        private System.Windows.Forms.ToolStripMenuItem addToContextReplacementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToAutomaticReplacementToolStripMenuItem;
        private System.Windows.Forms.Label m_lWordCounter;
        private System.Windows.Forms.ComboBox m_cbTranslationLanguage;
    }
}

