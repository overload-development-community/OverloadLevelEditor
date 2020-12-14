/*
THE COMPUTER CODE CONTAINED HEREIN IS THE SOLE PROPERTY OF REVIVAL
PRODUCTIONS, LLC ("REVIVAL").  REVIVAL, IN DISTRIBUTING THE CODE TO
END-USERS, AND SUBJECT TO ALL OF THE TERMS AND CONDITIONS HEREIN, GRANTS A
ROYALTY-FREE, PERPETUAL LICENSE TO SUCH END-USERS FOR USE BY SUCH END-USERS
IN USING, DISPLAYING,  AND CREATING DERIVATIVE WORKS THEREOF, SO LONG AS
SUCH USE, DISPLAY OR CREATION IS FOR NON-COMMERCIAL, ROYALTY OR REVENUE
FREE PURPOSES.  IN NO EVENT SHALL THE END-USER USE THE COMPUTER CODE
CONTAINED HEREIN FOR REVENUE-BEARING PURPOSES.  THE END-USER UNDERSTANDS
AND AGREES TO THE TERMS HEREIN AND ACCEPTS THE SAME BY USE OF THIS FILE.  
COPYRIGHT 2015-2020 REVIVAL PRODUCTIONS, LLC.  ALL RIGHTS RESERVED.
*/

namespace OverloadLevelEditor
{
    partial class Editor
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
			this.gl_panel = new OverloadLevelEditor.GLEditorViewPanel();
			this.tool_tip = new System.Windows.Forms.ToolTip(this.components);
			this.label_editmode = new System.Windows.Forms.Label();
			this.label_count_selected = new System.Windows.Forms.Label();
			this.label_count_marked = new System.Windows.Forms.Label();
			this.label_count_total = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gl_panel
			// 
			this.gl_panel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.gl_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gl_panel.Location = new System.Drawing.Point(0, 19);
			this.gl_panel.Margin = new System.Windows.Forms.Padding(1, 20, 1, 1);
			this.gl_panel.Name = "gl_panel";
			this.gl_panel.Size = new System.Drawing.Size(1348, 624);
			this.gl_panel.TabIndex = 1;
			// 
			// label_editmode
			// 
			this.label_editmode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_editmode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_editmode.Location = new System.Drawing.Point(0, 0);
			this.label_editmode.Margin = new System.Windows.Forms.Padding(1);
			this.label_editmode.Name = "label_editmode";
			this.label_editmode.Size = new System.Drawing.Size(198, 19);
			this.label_editmode.TabIndex = 0;
			this.label_editmode.Text = "SubMode: SEGMENT";
			this.label_editmode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_editmode, "Editing mode for geometry - Tab");
			this.label_editmode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_editmode_MouseDown);
			// 
			// label_count_selected
			// 
			this.label_count_selected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_selected.Location = new System.Drawing.Point(351, 0);
			this.label_count_selected.Margin = new System.Windows.Forms.Padding(1);
			this.label_count_selected.Name = "label_count_selected";
			this.label_count_selected.Size = new System.Drawing.Size(149, 19);
			this.label_count_selected.TabIndex = 2;
			this.label_count_selected.Text = "Selected: --/--/--";
			this.label_count_selected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_selected, "Index of the selected segment/side/vertex");
			// 
			// label_count_marked
			// 
			this.label_count_marked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_marked.Location = new System.Drawing.Point(200, 0);
			this.label_count_marked.Margin = new System.Windows.Forms.Padding(1);
			this.label_count_marked.Name = "label_count_marked";
			this.label_count_marked.Size = new System.Drawing.Size(149, 19);
			this.label_count_marked.TabIndex = 1;
			this.label_count_marked.Text = "Marked: 0/0/0";
			this.label_count_marked.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_marked, "Marked segments/sides/vertices");
			// 
			// label_count_total
			// 
			this.label_count_total.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_total.Location = new System.Drawing.Point(502, 0);
			this.label_count_total.Margin = new System.Windows.Forms.Padding(1);
			this.label_count_total.Name = "label_count_total";
			this.label_count_total.Size = new System.Drawing.Size(149, 19);
			this.label_count_total.TabIndex = 3;
			this.label_count_total.Text = "Total: 0/--/0";
			this.label_count_total.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_total, "Total segments/sides/verts in the level");
			// 
			// Editor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1348, 643);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.Controls.Add(this.gl_panel);
			this.Controls.Add(this.label_editmode);
			this.Controls.Add(this.label_count_marked);
			this.Controls.Add(this.label_count_total);
			this.Controls.Add(this.label_count_selected);
			this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HideOnClose = true;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Editor";
			this.ShowIcon = false;
			this.Text = "Level";
			this.Load += new System.EventHandler(this.Editor_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
			this.ResumeLayout(false);

        }

        #endregion

		private GLEditorViewPanel gl_panel;
		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.Label label_editmode;
		private System.Windows.Forms.Label label_count_marked;
		private System.Windows.Forms.Label label_count_selected;
		private System.Windows.Forms.Label label_count_total;
	}
}

