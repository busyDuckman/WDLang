/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
namespace WDLang.Words.Controls
{
    partial class WordSelectionBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtWordInput = new System.Windows.Forms.TextBox();
            this.txtDefinition = new System.Windows.Forms.TextBox();
            this.numDefNum = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNumMeanings = new System.Windows.Forms.Label();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpSearch = new System.Windows.Forms.TabPage();
            this.lstMatches = new System.Windows.Forms.ListBox();
            this.tpDefinition = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.numDefNum)).BeginInit();
            this.tcMain.SuspendLayout();
            this.tpSearch.SuspendLayout();
            this.tpDefinition.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWordInput
            // 
            this.txtWordInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWordInput.Location = new System.Drawing.Point(10, 3);
            this.txtWordInput.Name = "txtWordInput";
            this.txtWordInput.Size = new System.Drawing.Size(262, 20);
            this.txtWordInput.TabIndex = 0;
            this.txtWordInput.TextChanged += new System.EventHandler(this.txtWordInput_TextChanged);
            // 
            // txtDefinition
            // 
            this.txtDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefinition.BackColor = System.Drawing.SystemColors.Control;
            this.txtDefinition.Location = new System.Drawing.Point(6, 35);
            this.txtDefinition.Multiline = true;
            this.txtDefinition.Name = "txtDefinition";
            this.txtDefinition.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDefinition.Size = new System.Drawing.Size(250, 119);
            this.txtDefinition.TabIndex = 1;
            // 
            // numDefNum
            // 
            this.numDefNum.Location = new System.Drawing.Point(60, 9);
            this.numDefNum.Name = "numDefNum";
            this.numDefNum.Size = new System.Drawing.Size(42, 20);
            this.numDefNum.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Meaning";
            // 
            // lblNumMeanings
            // 
            this.lblNumMeanings.AutoSize = true;
            this.lblNumMeanings.Location = new System.Drawing.Point(111, 11);
            this.lblNumMeanings.Name = "lblNumMeanings";
            this.lblNumMeanings.Size = new System.Drawing.Size(25, 13);
            this.lblNumMeanings.TabIndex = 5;
            this.lblNumMeanings.Text = "of 5";
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpSearch);
            this.tcMain.Controls.Add(this.tpDefinition);
            this.tcMain.Location = new System.Drawing.Point(6, 29);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(270, 183);
            this.tcMain.TabIndex = 6;
            // 
            // tpSearch
            // 
            this.tpSearch.Controls.Add(this.lstMatches);
            this.tpSearch.Location = new System.Drawing.Point(4, 22);
            this.tpSearch.Name = "tpSearch";
            this.tpSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tpSearch.Size = new System.Drawing.Size(262, 157);
            this.tpSearch.TabIndex = 1;
            this.tpSearch.Text = "Matches";
            this.tpSearch.UseVisualStyleBackColor = true;
            // 
            // lstMatches
            // 
            this.lstMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMatches.FormattingEnabled = true;
            this.lstMatches.Location = new System.Drawing.Point(3, 3);
            this.lstMatches.Name = "lstMatches";
            this.lstMatches.Size = new System.Drawing.Size(256, 151);
            this.lstMatches.TabIndex = 0;
            this.lstMatches.SelectedIndexChanged += new System.EventHandler(this.lstMatches_SelectedIndexChanged);
            // 
            // tpDefinition
            // 
            this.tpDefinition.Controls.Add(this.txtDefinition);
            this.tpDefinition.Controls.Add(this.lblNumMeanings);
            this.tpDefinition.Controls.Add(this.label2);
            this.tpDefinition.Controls.Add(this.numDefNum);
            this.tpDefinition.Location = new System.Drawing.Point(4, 22);
            this.tpDefinition.Name = "tpDefinition";
            this.tpDefinition.Padding = new System.Windows.Forms.Padding(3);
            this.tpDefinition.Size = new System.Drawing.Size(262, 157);
            this.tpDefinition.TabIndex = 0;
            this.tpDefinition.Text = "Definition";
            this.tpDefinition.UseVisualStyleBackColor = true;
            // 
            // WordSelectionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.txtWordInput);
            this.Name = "WordSelectionBox";
            this.Size = new System.Drawing.Size(279, 215);
            this.Load += new System.EventHandler(this.WordSelectionBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numDefNum)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.tpSearch.ResumeLayout(false);
            this.tpDefinition.ResumeLayout(false);
            this.tpDefinition.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWordInput;
        private System.Windows.Forms.TextBox txtDefinition;
        private System.Windows.Forms.NumericUpDown numDefNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNumMeanings;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpSearch;
        private System.Windows.Forms.ListBox lstMatches;
        private System.Windows.Forms.TabPage tpDefinition;
    }
}
