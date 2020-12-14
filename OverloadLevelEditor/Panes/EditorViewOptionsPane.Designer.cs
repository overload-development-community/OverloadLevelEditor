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
	partial class EditorViewOptionsPane
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
			this.components = new System.ComponentModel.Container();
			this.label_auto_center = new System.Windows.Forms.Label();
			this.label_show_segment_numbers = new System.Windows.Forms.Label();
			this.label_lighting = new System.Windows.Forms.Label();
			this.label_gimbal = new System.Windows.Forms.Label();
			this.label_view_dark = new System.Windows.Forms.Label();
			this.label_view_layout = new System.Windows.Forms.Label();
			this.label_view_ortho = new System.Windows.Forms.Label();
			this.label_view_persp = new System.Windows.Forms.Label();
			this.slider_grid_snap = new OverloadLevelEditor.SliderLabel();
			this.button_snap_marked = new System.Windows.Forms.Button();
			this.slider_grid_spacing = new OverloadLevelEditor.SliderLabel();
			this.label_grid_display = new System.Windows.Forms.Label();
			this.tool_tip = new System.Windows.Forms.ToolTip(this.components);
			this.label_show_cutters = new System.Windows.Forms.Label();
			this.label_grid = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label_auto_center
			// 
			this.label_auto_center.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_auto_center.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_auto_center.Location = new System.Drawing.Point(2, 149);
			this.label_auto_center.Margin = new System.Windows.Forms.Padding(1);
			this.label_auto_center.Name = "label_auto_center";
			this.label_auto_center.Size = new System.Drawing.Size(144, 19);
			this.label_auto_center.TabIndex = 53;
			this.label_auto_center.Text = "Auto-center: OFF";
			this.label_auto_center.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_auto_center, "Background color in viewports");
			this.label_auto_center.Click += new System.EventHandler(this.label_auto_center_Click);
			// 
			// label_show_segment_numbers
			// 
			this.label_show_segment_numbers.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_show_segment_numbers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_show_segment_numbers.Location = new System.Drawing.Point(2, 128);
			this.label_show_segment_numbers.Margin = new System.Windows.Forms.Padding(1);
			this.label_show_segment_numbers.Name = "label_show_segment_numbers";
			this.label_show_segment_numbers.Size = new System.Drawing.Size(144, 19);
			this.label_show_segment_numbers.TabIndex = 52;
			this.label_show_segment_numbers.Text = "Show Seg Nums: NONE";
			this.label_show_segment_numbers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_show_segment_numbers, "Background color in viewports");
			this.label_show_segment_numbers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_show_segment_numbers_MouseDown);
			// 
			// label_lighting
			// 
			this.label_lighting.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_lighting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_lighting.Location = new System.Drawing.Point(2, 107);
			this.label_lighting.Margin = new System.Windows.Forms.Padding(1);
			this.label_lighting.Name = "label_lighting";
			this.label_lighting.Size = new System.Drawing.Size(144, 19);
			this.label_lighting.TabIndex = 51;
			this.label_lighting.Text = "Lighting: NONE";
			this.label_lighting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_lighting, "Background color in viewports");
			this.label_lighting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_lighting_MouseDown);
			// 
			// label_gimbal
			// 
			this.label_gimbal.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_gimbal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_gimbal.Location = new System.Drawing.Point(2, 86);
			this.label_gimbal.Margin = new System.Windows.Forms.Padding(1);
			this.label_gimbal.Name = "label_gimbal";
			this.label_gimbal.Size = new System.Drawing.Size(144, 19);
			this.label_gimbal.TabIndex = 50;
			this.label_gimbal.Text = "Gimbal: HIDE";
			this.label_gimbal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_gimbal, "How to render the gimbal");
			this.label_gimbal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_gimbal_MouseDown);
			// 
			// label_view_dark
			// 
			this.label_view_dark.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_dark.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_dark.Location = new System.Drawing.Point(2, 65);
			this.label_view_dark.Margin = new System.Windows.Forms.Padding(1);
			this.label_view_dark.Name = "label_view_dark";
			this.label_view_dark.Size = new System.Drawing.Size(144, 19);
			this.label_view_dark.TabIndex = 49;
			this.label_view_dark.Text = "Background: LIGHT";
			this.label_view_dark.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_dark, "Background color in viewports");
			this.label_view_dark.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_dark_MouseDown);
			// 
			// label_view_layout
			// 
			this.label_view_layout.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_layout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_layout.Location = new System.Drawing.Point(2, 44);
			this.label_view_layout.Margin = new System.Windows.Forms.Padding(1);
			this.label_view_layout.Name = "label_view_layout";
			this.label_view_layout.Size = new System.Drawing.Size(144, 19);
			this.label_view_layout.TabIndex = 48;
			this.label_view_layout.Text = "Layout: FOURWAY";
			this.label_view_layout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_layout, "Arrangement type for the viewports");
			this.label_view_layout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_layout_MouseDown);
			// 
			// label_view_ortho
			// 
			this.label_view_ortho.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_ortho.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_ortho.Location = new System.Drawing.Point(2, 2);
			this.label_view_ortho.Margin = new System.Windows.Forms.Padding(1);
			this.label_view_ortho.Name = "label_view_ortho";
			this.label_view_ortho.Size = new System.Drawing.Size(144, 19);
			this.label_view_ortho.TabIndex = 47;
			this.label_view_ortho.Text = "Ortho: WIREFRAME";
			this.label_view_ortho.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_ortho, "Display type for the orthographic views");
			this.label_view_ortho.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_ortho_MouseDown);
			// 
			// label_view_persp
			// 
			this.label_view_persp.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_persp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_persp.Location = new System.Drawing.Point(2, 23);
			this.label_view_persp.Margin = new System.Windows.Forms.Padding(1);
			this.label_view_persp.Name = "label_view_persp";
			this.label_view_persp.Size = new System.Drawing.Size(144, 19);
			this.label_view_persp.TabIndex = 45;
			this.label_view_persp.Text = "Persp: SOLID";
			this.label_view_persp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_persp, "Display type for the perspective view");
			this.label_view_persp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_persp_MouseDown);
			// 
			// slider_grid_snap
			// 
			this.slider_grid_snap.Location = new System.Drawing.Point(2, 232);
			this.slider_grid_snap.Margin = new System.Windows.Forms.Padding(1);
			this.slider_grid_snap.Name = "slider_grid_snap";
			this.slider_grid_snap.RightMouseMultiplier = 5;
			this.slider_grid_snap.Size = new System.Drawing.Size(144, 21);
			this.slider_grid_snap.SlideText = "Snap";
			this.slider_grid_snap.SlideTol = 20;
			this.slider_grid_snap.TabIndex = 92;
			this.tool_tip.SetToolTip(this.slider_grid_snap, "SEE BELOW");
			this.slider_grid_snap.ToolTop = "Grid snap amount - [ or ]";
			this.slider_grid_snap.ValueText = "2.0";
			this.slider_grid_snap.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_snap_Feedback);
			// 
			// button_snap_marked
			// 
			this.button_snap_marked.Location = new System.Drawing.Point(2, 274);
			this.button_snap_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_snap_marked.Name = "button_snap_marked";
			this.button_snap_marked.Size = new System.Drawing.Size(144, 21);
			this.button_snap_marked.TabIndex = 48;
			this.button_snap_marked.Text = "Snap Marked To Grid";
			this.tool_tip.SetToolTip(this.button_snap_marked, "Snap all marked elements to the grid - Ctrl+G");
			this.button_snap_marked.UseVisualStyleBackColor = true;
			this.button_snap_marked.Click += new System.EventHandler(this.button_snap_marked_Click);
			// 
			// slider_grid_spacing
			// 
			this.slider_grid_spacing.Location = new System.Drawing.Point(2, 211);
			this.slider_grid_spacing.Margin = new System.Windows.Forms.Padding(1);
			this.slider_grid_spacing.Name = "slider_grid_spacing";
			this.slider_grid_spacing.RightMouseMultiplier = 5;
			this.slider_grid_spacing.Size = new System.Drawing.Size(144, 21);
			this.slider_grid_spacing.SlideText = "Spacing";
			this.slider_grid_spacing.SlideTol = 20;
			this.slider_grid_spacing.TabIndex = 91;
			this.tool_tip.SetToolTip(this.slider_grid_spacing, "SEE BELOW");
			this.slider_grid_spacing.ToolTop = "Visible grid spacing - Shift+[ or Shift+]";
			this.slider_grid_spacing.ValueText = "4";
			this.slider_grid_spacing.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_spacing_Feedback);
			// 
			// label_grid_display
			// 
			this.label_grid_display.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_grid_display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_grid_display.Location = new System.Drawing.Point(2, 253);
			this.label_grid_display.Margin = new System.Windows.Forms.Padding(1);
			this.label_grid_display.Name = "label_grid_display";
			this.label_grid_display.Size = new System.Drawing.Size(144, 19);
			this.label_grid_display.TabIndex = 44;
			this.label_grid_display.Text = "Display: ALL";
			this.label_grid_display.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_grid_display, "Which views to show the grid in - Shift+G");
			this.label_grid_display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_grid_display_MouseDown);
			// 
			// label_show_cutters
			// 
			this.label_show_cutters.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_show_cutters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_show_cutters.Location = new System.Drawing.Point(2, 170);
			this.label_show_cutters.Margin = new System.Windows.Forms.Padding(1);
			this.label_show_cutters.Name = "label_show_cutters";
			this.label_show_cutters.Size = new System.Drawing.Size(144, 19);
			this.label_show_cutters.TabIndex = 95;
			this.label_show_cutters.Text = "Split Planes: SHOW";
			this.label_show_cutters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_show_cutters, "Background color in viewports");
			this.label_show_cutters.MouseClick += new System.Windows.Forms.MouseEventHandler(this.label_show_cutters_MouseClick);
			// 
			// label_grid
			// 
			this.label_grid.BackColor = System.Drawing.SystemColors.Control;
			this.label_grid.Location = new System.Drawing.Point(2, 191);
			this.label_grid.Margin = new System.Windows.Forms.Padding(1);
			this.label_grid.Name = "label_grid";
			this.label_grid.Size = new System.Drawing.Size(144, 19);
			this.label_grid.TabIndex = 94;
			this.label_grid.Text = "GRID";
			this.label_grid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// EditorViewOptionsPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(148, 323);
			this.Controls.Add(this.label_show_cutters);
			this.Controls.Add(this.label_grid);
			this.Controls.Add(this.label_auto_center);
			this.Controls.Add(this.slider_grid_snap);
			this.Controls.Add(this.label_show_segment_numbers);
			this.Controls.Add(this.button_snap_marked);
			this.Controls.Add(this.label_lighting);
			this.Controls.Add(this.label_gimbal);
			this.Controls.Add(this.slider_grid_spacing);
			this.Controls.Add(this.label_view_dark);
			this.Controls.Add(this.label_view_layout);
			this.Controls.Add(this.label_grid_display);
			this.Controls.Add(this.label_view_ortho);
			this.Controls.Add(this.label_view_persp);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.MinimumSize = new System.Drawing.Size(164, 34);
			this.Name = "EditorViewOptionsPane";
			this.Text = "VIEW OPTIONS";
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label_auto_center;
		private System.Windows.Forms.Label label_show_segment_numbers;
		private System.Windows.Forms.Label label_lighting;
		private System.Windows.Forms.Label label_gimbal;
		private System.Windows.Forms.Label label_view_dark;
		private System.Windows.Forms.Label label_view_layout;
		private System.Windows.Forms.Label label_view_ortho;
		private System.Windows.Forms.Label label_view_persp;
		private SliderLabel slider_grid_snap;
		private System.Windows.Forms.Button button_snap_marked;
		private SliderLabel slider_grid_spacing;
		private System.Windows.Forms.Label label_grid_display;
		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.Label label_grid;
		private System.Windows.Forms.Label label_show_cutters;
	}
}