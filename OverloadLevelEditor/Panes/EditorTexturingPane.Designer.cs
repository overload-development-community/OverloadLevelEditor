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
	partial class EditorTexturingPane
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
			this.tool_tip = new System.Windows.Forms.ToolTip(this.components);
			this.button_mark_caves = new System.Windows.Forms.Button();
			this.button_cave4 = new System.Windows.Forms.Button();
			this.button_cave3 = new System.Windows.Forms.Button();
			this.button_cave2 = new System.Windows.Forms.Button();
			this.button_cave1 = new System.Windows.Forms.Button();
			this.button_copy_def_height = new System.Windows.Forms.Button();
			this.slider_deformation_height = new OverloadLevelEditor.SliderLabel();
			this.button_texture_box_map = new System.Windows.Forms.Button();
			this.button_align_marked = new System.Windows.Forms.Button();
			this.button_texture_planar_x = new System.Windows.Forms.Button();
			this.button_texture_planar_y = new System.Windows.Forms.Button();
			this.button_texture_center_v = new System.Windows.Forms.Button();
			this.button_texture_planar_z = new System.Windows.Forms.Button();
			this.button_texture_center_u = new System.Windows.Forms.Button();
			this.button_texture_snap8 = new System.Windows.Forms.Button();
			this.button_texture_show_list = new System.Windows.Forms.Button();
			this.button_texture_snap4 = new System.Windows.Forms.Button();
			this.label_texture_name = new System.Windows.Forms.Label();
			this.button_texture_default_map = new System.Windows.Forms.Button();
			this.label_selected = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button_mark_height = new System.Windows.Forms.Button();
			this.label_side_data = new System.Windows.Forms.Label();
			this.sliderLabelSplitPlaneOrder = new OverloadLevelEditor.SliderLabel();
			this.sliderLabelPathfinding = new OverloadLevelEditor.SliderLabel();
			this.sliderLabelExitSegment = new OverloadLevelEditor.SliderLabel();
			this.sliderLabelDark = new OverloadLevelEditor.SliderLabel();
			this.SuspendLayout();
			// 
			// button_mark_caves
			// 
			this.button_mark_caves.Location = new System.Drawing.Point(2, 248);
			this.button_mark_caves.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_caves.Name = "button_mark_caves";
			this.button_mark_caves.Size = new System.Drawing.Size(71, 21);
			this.button_mark_caves.TabIndex = 103;
			this.button_mark_caves.Text = "Mk Group";
			this.tool_tip.SetToolTip(this.button_mark_caves, "Mark all sides with the same cave group (height > 0)");
			this.button_mark_caves.UseVisualStyleBackColor = true;
			this.button_mark_caves.Click += new System.EventHandler(this.button_mark_caves_Click);
			// 
			// button_cave4
			// 
			this.button_cave4.Location = new System.Drawing.Point(111, 204);
			this.button_cave4.Margin = new System.Windows.Forms.Padding(1);
			this.button_cave4.Name = "button_cave4";
			this.button_cave4.Size = new System.Drawing.Size(35, 21);
			this.button_cave4.TabIndex = 102;
			this.button_cave4.Text = "C4";
			this.tool_tip.SetToolTip(this.button_cave4, "Set the marked/selected cave preset to 4");
			this.button_cave4.UseVisualStyleBackColor = true;
			this.button_cave4.Click += new System.EventHandler(this.button_cave4_Click);
			// 
			// button_cave3
			// 
			this.button_cave3.Location = new System.Drawing.Point(75, 204);
			this.button_cave3.Margin = new System.Windows.Forms.Padding(1);
			this.button_cave3.Name = "button_cave3";
			this.button_cave3.Size = new System.Drawing.Size(35, 21);
			this.button_cave3.TabIndex = 101;
			this.button_cave3.Text = "C3";
			this.tool_tip.SetToolTip(this.button_cave3, "Set the marked/selected cave preset to 3");
			this.button_cave3.UseVisualStyleBackColor = true;
			this.button_cave3.Click += new System.EventHandler(this.button_cave3_Click);
			// 
			// button_cave2
			// 
			this.button_cave2.Location = new System.Drawing.Point(38, 204);
			this.button_cave2.Margin = new System.Windows.Forms.Padding(1);
			this.button_cave2.Name = "button_cave2";
			this.button_cave2.Size = new System.Drawing.Size(35, 21);
			this.button_cave2.TabIndex = 100;
			this.button_cave2.Text = "C2";
			this.tool_tip.SetToolTip(this.button_cave2, "Set the marked/selected cave preset to 2");
			this.button_cave2.UseVisualStyleBackColor = true;
			this.button_cave2.Click += new System.EventHandler(this.button_cave2_Click);
			// 
			// button_cave1
			// 
			this.button_cave1.BackColor = System.Drawing.Color.Yellow;
			this.button_cave1.Location = new System.Drawing.Point(2, 204);
			this.button_cave1.Margin = new System.Windows.Forms.Padding(1);
			this.button_cave1.Name = "button_cave1";
			this.button_cave1.Size = new System.Drawing.Size(35, 21);
			this.button_cave1.TabIndex = 98;
			this.button_cave1.Text = "C1";
			this.tool_tip.SetToolTip(this.button_cave1, "Set the marked/selected cave preset to 1");
			this.button_cave1.UseVisualStyleBackColor = false;
			this.button_cave1.Click += new System.EventHandler(this.button_cave1_Click);
			// 
			// button_copy_def_height
			// 
			this.button_copy_def_height.Location = new System.Drawing.Point(2, 269);
			this.button_copy_def_height.Margin = new System.Windows.Forms.Padding(1);
			this.button_copy_def_height.Name = "button_copy_def_height";
			this.button_copy_def_height.Size = new System.Drawing.Size(145, 21);
			this.button_copy_def_height.TabIndex = 97;
			this.button_copy_def_height.Text = "Copy Height to Marked";
			this.tool_tip.SetToolTip(this.button_copy_def_height, "Copy the selected sides\' Deformation Height value to the marked sides");
			this.button_copy_def_height.UseVisualStyleBackColor = true;
			this.button_copy_def_height.Click += new System.EventHandler(this.button_copy_def_height_Click);
			// 
			// slider_deformation_height
			// 
			this.slider_deformation_height.Location = new System.Drawing.Point(2, 227);
			this.slider_deformation_height.Margin = new System.Windows.Forms.Padding(1);
			this.slider_deformation_height.Name = "slider_deformation_height";
			this.slider_deformation_height.RightMouseMultiplier = 5;
			this.slider_deformation_height.Size = new System.Drawing.Size(144, 19);
			this.slider_deformation_height.SlideText = "Deformation Height";
			this.slider_deformation_height.SlideTol = 15;
			this.slider_deformation_height.TabIndex = 96;
			this.tool_tip.SetToolTip(this.slider_deformation_height, "SEE BELOW");
			this.slider_deformation_height.ToolTop = "Maximum vertex deformation amount (in meters)";
			this.slider_deformation_height.ValueText = "0.00";
			this.slider_deformation_height.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_deformation_height_Feedback);
			// 
			// button_texture_box_map
			// 
			this.button_texture_box_map.Location = new System.Drawing.Point(2, 68);
			this.button_texture_box_map.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_box_map.Name = "button_texture_box_map";
			this.button_texture_box_map.Size = new System.Drawing.Size(145, 21);
			this.button_texture_box_map.TabIndex = 60;
			this.button_texture_box_map.Text = "Box Map";
			this.tool_tip.SetToolTip(this.button_texture_box_map, "Box map the marked sides");
			this.button_texture_box_map.UseVisualStyleBackColor = true;
			this.button_texture_box_map.Click += new System.EventHandler(this.button_texture_box_map_Click);
			// 
			// button_align_marked
			// 
			this.button_align_marked.Location = new System.Drawing.Point(2, 114);
			this.button_align_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_align_marked.Name = "button_align_marked";
			this.button_align_marked.Size = new System.Drawing.Size(145, 21);
			this.button_align_marked.TabIndex = 61;
			this.button_align_marked.Text = "Align To Selected";
			this.tool_tip.SetToolTip(this.button_align_marked, "Aligned the marked sides\' UVs to line up with the selected side - Shift+T");
			this.button_align_marked.UseVisualStyleBackColor = true;
			this.button_align_marked.Click += new System.EventHandler(this.button_align_marked_Click);
			// 
			// button_texture_planar_x
			// 
			this.button_texture_planar_x.Location = new System.Drawing.Point(2, 45);
			this.button_texture_planar_x.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_x.Name = "button_texture_planar_x";
			this.button_texture_planar_x.Size = new System.Drawing.Size(49, 21);
			this.button_texture_planar_x.TabIndex = 62;
			this.button_texture_planar_x.Text = "Map X";
			this.tool_tip.SetToolTip(this.button_texture_planar_x, "Planar map the marked sides");
			this.button_texture_planar_x.UseVisualStyleBackColor = true;
			this.button_texture_planar_x.Click += new System.EventHandler(this.button_texture_planar_x_Click);
			// 
			// button_texture_planar_y
			// 
			this.button_texture_planar_y.Location = new System.Drawing.Point(50, 45);
			this.button_texture_planar_y.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_y.Name = "button_texture_planar_y";
			this.button_texture_planar_y.Size = new System.Drawing.Size(49, 21);
			this.button_texture_planar_y.TabIndex = 63;
			this.button_texture_planar_y.Text = "Map Y";
			this.tool_tip.SetToolTip(this.button_texture_planar_y, "Planar map the marked sides");
			this.button_texture_planar_y.UseVisualStyleBackColor = true;
			this.button_texture_planar_y.Click += new System.EventHandler(this.button_texture_planar_y_Click);
			// 
			// button_texture_center_v
			// 
			this.button_texture_center_v.Location = new System.Drawing.Point(76, 137);
			this.button_texture_center_v.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_center_v.Name = "button_texture_center_v";
			this.button_texture_center_v.Size = new System.Drawing.Size(71, 21);
			this.button_texture_center_v.TabIndex = 89;
			this.button_texture_center_v.Text = "Center V";
			this.tool_tip.SetToolTip(this.button_texture_center_v, "Center the marked sides\' UV in the V direction");
			this.button_texture_center_v.UseVisualStyleBackColor = true;
			this.button_texture_center_v.Click += new System.EventHandler(this.button_texture_center_v_Click);
			// 
			// button_texture_planar_z
			// 
			this.button_texture_planar_z.Location = new System.Drawing.Point(98, 45);
			this.button_texture_planar_z.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_z.Name = "button_texture_planar_z";
			this.button_texture_planar_z.Size = new System.Drawing.Size(49, 21);
			this.button_texture_planar_z.TabIndex = 64;
			this.button_texture_planar_z.Text = "Map Z";
			this.tool_tip.SetToolTip(this.button_texture_planar_z, "Planar map the marked sides");
			this.button_texture_planar_z.UseVisualStyleBackColor = true;
			this.button_texture_planar_z.Click += new System.EventHandler(this.button_texture_planar_z_Click);
			// 
			// button_texture_center_u
			// 
			this.button_texture_center_u.Location = new System.Drawing.Point(2, 137);
			this.button_texture_center_u.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_center_u.Name = "button_texture_center_u";
			this.button_texture_center_u.Size = new System.Drawing.Size(71, 21);
			this.button_texture_center_u.TabIndex = 88;
			this.button_texture_center_u.Text = "Center U";
			this.tool_tip.SetToolTip(this.button_texture_center_u, "Center the marked sides\' UV in the U direction");
			this.button_texture_center_u.UseVisualStyleBackColor = true;
			this.button_texture_center_u.Click += new System.EventHandler(this.button_texture_center_u_Click);
			// 
			// button_texture_snap8
			// 
			this.button_texture_snap8.Location = new System.Drawing.Point(2, 160);
			this.button_texture_snap8.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_snap8.Name = "button_texture_snap8";
			this.button_texture_snap8.Size = new System.Drawing.Size(71, 21);
			this.button_texture_snap8.TabIndex = 65;
			this.button_texture_snap8.Text = "1/8 Snap";
			this.tool_tip.SetToolTip(this.button_texture_snap8, "Snap the marked sides\' textures to the closest 1/8th increment");
			this.button_texture_snap8.UseVisualStyleBackColor = true;
			this.button_texture_snap8.Click += new System.EventHandler(this.button_texture_snap8_Click);
			// 
			// button_texture_show_list
			// 
			this.button_texture_show_list.Location = new System.Drawing.Point(2, 22);
			this.button_texture_show_list.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_show_list.Name = "button_texture_show_list";
			this.button_texture_show_list.Size = new System.Drawing.Size(145, 21);
			this.button_texture_show_list.TabIndex = 86;
			this.button_texture_show_list.Text = "Show Texture List";
			this.tool_tip.SetToolTip(this.button_texture_show_list, "Show the list of textures [F1]");
			this.button_texture_show_list.UseVisualStyleBackColor = true;
			this.button_texture_show_list.Click += new System.EventHandler(this.button_texture_show_list_Click);
			// 
			// button_texture_snap4
			// 
			this.button_texture_snap4.Location = new System.Drawing.Point(76, 160);
			this.button_texture_snap4.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_snap4.Name = "button_texture_snap4";
			this.button_texture_snap4.Size = new System.Drawing.Size(71, 21);
			this.button_texture_snap4.TabIndex = 66;
			this.button_texture_snap4.Text = "1/4 Snap";
			this.tool_tip.SetToolTip(this.button_texture_snap4, "Snap the marked sides\' textures to the closest 1/4th increment");
			this.button_texture_snap4.UseVisualStyleBackColor = true;
			this.button_texture_snap4.Click += new System.EventHandler(this.button_texture_snap4_Click);
			// 
			// label_texture_name
			// 
			this.label_texture_name.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_texture_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_texture_name.Location = new System.Drawing.Point(2, 2);
			this.label_texture_name.Margin = new System.Windows.Forms.Padding(1);
			this.label_texture_name.Name = "label_texture_name";
			this.label_texture_name.Size = new System.Drawing.Size(144, 19);
			this.label_texture_name.TabIndex = 85;
			this.label_texture_name.Text = "-";
			this.label_texture_name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_texture_name, "Texture name of the selected sides face");
			// 
			// button_texture_default_map
			// 
			this.button_texture_default_map.Location = new System.Drawing.Point(2, 91);
			this.button_texture_default_map.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_default_map.Name = "button_texture_default_map";
			this.button_texture_default_map.Size = new System.Drawing.Size(145, 21);
			this.button_texture_default_map.TabIndex = 67;
			this.button_texture_default_map.Text = "Default Map";
			this.tool_tip.SetToolTip(this.button_texture_default_map, "Reset the marked sides to the default mapping - Ctrl+T");
			this.button_texture_default_map.UseVisualStyleBackColor = true;
			this.button_texture_default_map.Click += new System.EventHandler(this.button_texture_default_map_Click);
			// 
			// label_selected
			// 
			this.label_selected.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label_selected.Location = new System.Drawing.Point(2, 292);
			this.label_selected.Margin = new System.Windows.Forms.Padding(1);
			this.label_selected.Name = "label_selected";
			this.label_selected.Size = new System.Drawing.Size(145, 19);
			this.label_selected.TabIndex = 130;
			this.label_selected.Text = "--- SELECTED SEGMENT ---";
			this.label_selected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_selected, "Entity type");
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label1.Location = new System.Drawing.Point(2, 379);
			this.label1.Margin = new System.Windows.Forms.Padding(1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 19);
			this.label1.TabIndex = 131;
			this.label1.Text = "--- SELECTED SIDE  ---";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label1, "Entity type");
			// 
			// button_mark_height
			// 
			this.button_mark_height.Location = new System.Drawing.Point(76, 248);
			this.button_mark_height.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_height.Name = "button_mark_height";
			this.button_mark_height.Size = new System.Drawing.Size(71, 21);
			this.button_mark_height.TabIndex = 132;
			this.button_mark_height.Text = "Mk Height";
			this.tool_tip.SetToolTip(this.button_mark_height, "Mark sides with the same height (and group)");
			this.button_mark_height.UseVisualStyleBackColor = true;
			this.button_mark_height.Click += new System.EventHandler(this.button_mark_height_Click);
			// 
			// label_side_data
			// 
			this.label_side_data.BackColor = System.Drawing.SystemColors.Control;
			this.label_side_data.Location = new System.Drawing.Point(3, 183);
			this.label_side_data.Margin = new System.Windows.Forms.Padding(1);
			this.label_side_data.Name = "label_side_data";
			this.label_side_data.Size = new System.Drawing.Size(144, 19);
			this.label_side_data.TabIndex = 93;
			this.label_side_data.Text = "OTHER SIDE DATA";
			this.label_side_data.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// sliderLabelSplitPlaneOrder
			// 
			this.sliderLabelSplitPlaneOrder.Location = new System.Drawing.Point(2, 401);
			this.sliderLabelSplitPlaneOrder.Name = "sliderLabelSplitPlaneOrder";
			this.sliderLabelSplitPlaneOrder.RightMouseMultiplier = 5;
			this.sliderLabelSplitPlaneOrder.Size = new System.Drawing.Size(143, 19);
			this.sliderLabelSplitPlaneOrder.SlideText = "Split plane order";
			this.sliderLabelSplitPlaneOrder.SlideTol = 15;
			this.sliderLabelSplitPlaneOrder.TabIndex = 104;
			this.tool_tip.SetToolTip(this.sliderLabelSplitPlaneOrder, "The priority of chunk splitting (lowest = first, highest = last)");
			this.sliderLabelSplitPlaneOrder.ToolTop = "";
			this.sliderLabelSplitPlaneOrder.ValueText = "";
			this.sliderLabelSplitPlaneOrder.Feedback += new System.EventHandler<SliderLabelArgs>(this.sliderLabelSplitPlaneOrder_Feedback);
			// 
			// sliderLabelPathfinding
			// 
			this.sliderLabelPathfinding.Location = new System.Drawing.Point(2, 314);
			this.sliderLabelPathfinding.Name = "sliderLabelPathfinding";
			this.sliderLabelPathfinding.RightMouseMultiplier = 1;
			this.sliderLabelPathfinding.Size = new System.Drawing.Size(143, 19);
			this.sliderLabelPathfinding.SlideText = "Pathfinding";
			this.sliderLabelPathfinding.SlideTol = 15;
			this.sliderLabelPathfinding.TabIndex = 105;
			this.tool_tip.SetToolTip(this.sliderLabelPathfinding, "Pathfinding flag for the segment");
			this.sliderLabelPathfinding.ToolTop = "";
			this.sliderLabelPathfinding.ValueText = "";
			this.sliderLabelPathfinding.Feedback += new System.EventHandler<SliderLabelArgs>(this.sliderLabelPathfinding_Feedback);
			// 
			// sliderLabelExitSegment
			// 
			this.sliderLabelExitSegment.Location = new System.Drawing.Point(2, 356);
			this.sliderLabelExitSegment.Name = "sliderLabelExitSegment";
			this.sliderLabelExitSegment.RightMouseMultiplier = 1;
			this.sliderLabelExitSegment.Size = new System.Drawing.Size(143, 19);
			this.sliderLabelExitSegment.SlideText = "ExitSegmentType";
			this.sliderLabelExitSegment.SlideTol = 15;
			this.sliderLabelExitSegment.TabIndex = 107;
			this.tool_tip.SetToolTip(this.sliderLabelExitSegment, "Set Start for the first segment past the exit door, and END for the segment when " +
        "the explosion should occur");
			this.sliderLabelExitSegment.ToolTop = "";
			this.sliderLabelExitSegment.ValueText = "";
			this.sliderLabelExitSegment.Feedback += new System.EventHandler<SliderLabelArgs>(this.sliderLabelExitSegment_Feedback);
			// 
			// sliderLabelDark
			// 
			this.sliderLabelDark.Enabled = false;
			this.sliderLabelDark.Location = new System.Drawing.Point(2, 335);
			this.sliderLabelDark.Name = "sliderLabelDark";
			this.sliderLabelDark.RightMouseMultiplier = 5;
			this.sliderLabelDark.Size = new System.Drawing.Size(143, 19);
			this.sliderLabelDark.SlideText = "Dark - UNUSED";
			this.sliderLabelDark.SlideTol = 15;
			this.sliderLabelDark.TabIndex = 106;
			this.tool_tip.SetToolTip(this.sliderLabelDark, "UNUSED");
			this.sliderLabelDark.ToolTop = "";
			this.sliderLabelDark.ValueText = "";
			this.sliderLabelDark.Feedback += new System.EventHandler<SliderLabelArgs>(this.sliderLabelDark_Feedback);
			// 
			// EditorTexturingPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(148, 480);
			this.Controls.Add(this.button_mark_height);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label_selected);
			this.Controls.Add(this.sliderLabelDark);
			this.Controls.Add(this.sliderLabelPathfinding);
			this.Controls.Add(this.sliderLabelExitSegment);
			this.Controls.Add(this.sliderLabelSplitPlaneOrder);
			this.Controls.Add(this.button_mark_caves);
			this.Controls.Add(this.button_cave4);
			this.Controls.Add(this.button_cave3);
			this.Controls.Add(this.button_cave2);
			this.Controls.Add(this.button_cave1);
			this.Controls.Add(this.button_copy_def_height);
			this.Controls.Add(this.slider_deformation_height);
			this.Controls.Add(this.button_texture_box_map);
			this.Controls.Add(this.button_align_marked);
			this.Controls.Add(this.label_side_data);
			this.Controls.Add(this.button_texture_planar_x);
			this.Controls.Add(this.button_texture_planar_y);
			this.Controls.Add(this.button_texture_center_v);
			this.Controls.Add(this.button_texture_planar_z);
			this.Controls.Add(this.button_texture_center_u);
			this.Controls.Add(this.button_texture_snap8);
			this.Controls.Add(this.button_texture_show_list);
			this.Controls.Add(this.button_texture_snap4);
			this.Controls.Add(this.label_texture_name);
			this.Controls.Add(this.button_texture_default_map);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.MinimumSize = new System.Drawing.Size(164, 39);
			this.Name = "EditorTexturingPane";
			this.Text = "TEXTURING";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.Button button_texture_box_map;
		private System.Windows.Forms.Button button_align_marked;
		private System.Windows.Forms.Button button_texture_planar_x;
		private System.Windows.Forms.Button button_texture_planar_y;
		private System.Windows.Forms.Button button_texture_planar_z;
		private System.Windows.Forms.Button button_texture_snap8;
		private System.Windows.Forms.Button button_texture_snap4;
		private System.Windows.Forms.Button button_texture_default_map;
		private System.Windows.Forms.Label label_texture_name;
		private System.Windows.Forms.Button button_texture_show_list;
		private System.Windows.Forms.Button button_texture_center_u;
		private System.Windows.Forms.Button button_texture_center_v;
		private System.Windows.Forms.Label label_side_data;
		private SliderLabel slider_deformation_height;
		private System.Windows.Forms.Button button_copy_def_height;
		private System.Windows.Forms.Button button_cave1;
		private System.Windows.Forms.Button button_cave2;
		private System.Windows.Forms.Button button_cave4;
		private System.Windows.Forms.Button button_cave3;
		private System.Windows.Forms.Button button_mark_caves;
		private SliderLabel sliderLabelSplitPlaneOrder;
		private SliderLabel sliderLabelPathfinding;
		private SliderLabel sliderLabelExitSegment;
		private SliderLabel sliderLabelDark;
		private System.Windows.Forms.Label label_selected;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_mark_height;
	}
}