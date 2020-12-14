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
	partial class EditorLevelGlobalPane
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.slider_cave_simplify = new OverloadLevelEditor.SliderLabel();
			this.label_cave_preset4 = new System.Windows.Forms.Label();
			this.label_cave_preset3 = new System.Windows.Forms.Label();
			this.label_cave_preset2 = new System.Windows.Forms.Label();
			this.label_cave_preset1 = new System.Windows.Forms.Label();
			this.label_cave_postsmooth = new System.Windows.Forms.Label();
			this.label_cave_presmooth = new System.Windows.Forms.Label();
			this.label_cave_grid = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// slider_cave_simplify
			// 
			this.slider_cave_simplify.Enabled = false;
			this.slider_cave_simplify.Location = new System.Drawing.Point(2, 149);
			this.slider_cave_simplify.Margin = new System.Windows.Forms.Padding(1);
			this.slider_cave_simplify.Name = "slider_cave_simplify";
			this.slider_cave_simplify.RightMouseMultiplier = 4;
			this.slider_cave_simplify.Size = new System.Drawing.Size(144, 21);
			this.slider_cave_simplify.SlideText = "Simplify Strength";
			this.slider_cave_simplify.SlideTol = 15;
			this.slider_cave_simplify.TabIndex = 93;
			this.slider_cave_simplify.ToolTop = "DISABLED/INVISIBLE - Strength of the post-process simplification algorithm";
			this.slider_cave_simplify.ValueText = "0.0";
			this.slider_cave_simplify.Visible = false;
			this.slider_cave_simplify.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_cave_simplify_Feedback);
			// 
			// label_cave_preset4
			// 
			this.label_cave_preset4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_preset4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_preset4.Location = new System.Drawing.Point(2, 128);
			this.label_cave_preset4.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_preset4.Name = "label_cave_preset4";
			this.label_cave_preset4.Size = new System.Drawing.Size(144, 19);
			this.label_cave_preset4.TabIndex = 54;
			this.label_cave_preset4.Text = "Preset 4: NONE";
			this.label_cave_preset4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_preset4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_preset4_MouseDown);
			// 
			// label_cave_preset3
			// 
			this.label_cave_preset3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_preset3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_preset3.Location = new System.Drawing.Point(2, 107);
			this.label_cave_preset3.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_preset3.Name = "label_cave_preset3";
			this.label_cave_preset3.Size = new System.Drawing.Size(144, 19);
			this.label_cave_preset3.TabIndex = 53;
			this.label_cave_preset3.Text = "Preset 3: NONE";
			this.label_cave_preset3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_preset3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_preset3_MouseDown);
			// 
			// label_cave_preset2
			// 
			this.label_cave_preset2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_preset2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_preset2.Location = new System.Drawing.Point(2, 86);
			this.label_cave_preset2.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_preset2.Name = "label_cave_preset2";
			this.label_cave_preset2.Size = new System.Drawing.Size(144, 19);
			this.label_cave_preset2.TabIndex = 52;
			this.label_cave_preset2.Text = "Preset 2: NONE";
			this.label_cave_preset2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_preset2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_preset2_MouseDown);
			// 
			// label_cave_preset1
			// 
			this.label_cave_preset1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_preset1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_preset1.Location = new System.Drawing.Point(2, 65);
			this.label_cave_preset1.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_preset1.Name = "label_cave_preset1";
			this.label_cave_preset1.Size = new System.Drawing.Size(144, 19);
			this.label_cave_preset1.TabIndex = 51;
			this.label_cave_preset1.Text = "Preset 1: H_RIDGES";
			this.label_cave_preset1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_preset1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_preset1_MouseDown);
			// 
			// label_cave_postsmooth
			// 
			this.label_cave_postsmooth.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_postsmooth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_postsmooth.Location = new System.Drawing.Point(2, 44);
			this.label_cave_postsmooth.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_postsmooth.Name = "label_cave_postsmooth";
			this.label_cave_postsmooth.Size = new System.Drawing.Size(144, 19);
			this.label_cave_postsmooth.TabIndex = 50;
			this.label_cave_postsmooth.Text = "PostProcess Smooth: 1";
			this.label_cave_postsmooth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_postsmooth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_postsmooth_MouseDown);
			// 
			// label_cave_presmooth
			// 
			this.label_cave_presmooth.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_presmooth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_presmooth.Location = new System.Drawing.Point(2, 23);
			this.label_cave_presmooth.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_presmooth.Name = "label_cave_presmooth";
			this.label_cave_presmooth.Size = new System.Drawing.Size(144, 19);
			this.label_cave_presmooth.TabIndex = 49;
			this.label_cave_presmooth.Text = "PreProcess Smooth: 4";
			this.label_cave_presmooth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_presmooth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_presmooth_MouseDown);
			// 
			// label_cave_grid
			// 
			this.label_cave_grid.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_cave_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_cave_grid.Location = new System.Drawing.Point(2, 2);
			this.label_cave_grid.Margin = new System.Windows.Forms.Padding(1);
			this.label_cave_grid.Name = "label_cave_grid";
			this.label_cave_grid.Size = new System.Drawing.Size(144, 19);
			this.label_cave_grid.TabIndex = 48;
			this.label_cave_grid.Text = "Grid: 8x8";
			this.label_cave_grid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_cave_grid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_cave_grid_MouseDown);
			// 
			// EditorLevelGlobalPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(148, 170);
			this.Controls.Add(this.slider_cave_simplify);
			this.Controls.Add(this.label_cave_preset4);
			this.Controls.Add(this.label_cave_preset3);
			this.Controls.Add(this.label_cave_preset2);
			this.Controls.Add(this.label_cave_preset1);
			this.Controls.Add(this.label_cave_postsmooth);
			this.Controls.Add(this.label_cave_presmooth);
			this.Controls.Add(this.label_cave_grid);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.MinimumSize = new System.Drawing.Size(164, 34);
			this.Name = "EditorLevelGlobalPane";
			this.Text = "LEVEL GLOBAL DATA";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label_cave_grid;
		private System.Windows.Forms.Label label_cave_presmooth;
		private System.Windows.Forms.Label label_cave_postsmooth;
		private System.Windows.Forms.Label label_cave_preset1;
		private System.Windows.Forms.Label label_cave_preset2;
		private System.Windows.Forms.Label label_cave_preset3;
		private System.Windows.Forms.Label label_cave_preset4;
		private SliderLabel slider_cave_simplify;
	}
}