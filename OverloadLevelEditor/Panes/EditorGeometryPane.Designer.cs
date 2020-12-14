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
	partial class EditorGeometryPane
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
			this.label_drag_select = new System.Windows.Forms.Label();
			this.button_rotate_at_selected_CCW = new System.Windows.Forms.Button();
			this.button_connect_with_segment = new System.Windows.Forms.Button();
			this.button_default_segment = new System.Windows.Forms.Button();
			this.button_paste_default = new System.Windows.Forms.Button();
			this.button_mark_walls_straight = new System.Windows.Forms.Button();
			this.button_copy_marked = new System.Windows.Forms.Button();
			this.slider_extrude_length = new OverloadLevelEditor.SliderLabel();
			this.label_insert_advance = new System.Windows.Forms.Label();
			this.button_merge_verts = new System.Windows.Forms.Button();
			this.label_side_select = new System.Windows.Forms.Label();
			this.slider_coplanar_angle = new OverloadLevelEditor.SliderLabel();
			this.button_flipx = new System.Windows.Forms.Button();
			this.button_mark_walls = new System.Windows.Forms.Button();
			this.button_flipy = new System.Windows.Forms.Button();
			this.button_mark_coplanar = new System.Windows.Forms.Button();
			this.button_flipz = new System.Windows.Forms.Button();
			this.button_join_all = new System.Windows.Forms.Button();
			this.button_paste_at_side = new System.Windows.Forms.Button();
			this.button_join_side = new System.Windows.Forms.Button();
			this.button_paste_mirrorx = new System.Windows.Forms.Button();
			this.button_isolate_marked = new System.Windows.Forms.Button();
			this.button_paste_mirrory = new System.Windows.Forms.Button();
			this.button_paste_mirrorz = new System.Windows.Forms.Button();
			this.button_2way = new System.Windows.Forms.Button();
			this.button_5way = new System.Windows.Forms.Button();
			this.button_7way = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.slider_rotate_angle = new OverloadLevelEditor.SliderLabel();
			this.button_rotate_at_selected_CW = new System.Windows.Forms.Button();
			this.button_paste_at_origin_with_entities = new System.Windows.Forms.Button();
			this.button_paste_at_side_with_entities = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label_drag_select
			// 
			this.label_drag_select.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_drag_select.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_drag_select.Location = new System.Drawing.Point(1, 520);
			this.label_drag_select.Margin = new System.Windows.Forms.Padding(1);
			this.label_drag_select.Name = "label_drag_select";
			this.label_drag_select.Size = new System.Drawing.Size(144, 19);
			this.label_drag_select.TabIndex = 97;
			this.label_drag_select.Text = "Drag Mode: ALL";
			this.label_drag_select.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_drag_select, "How segments are marked by dragging (how many verts must be in the drag box)");
			// 
			// button_rotate_at_selected_CCW
			// 
			this.button_rotate_at_selected_CCW.Location = new System.Drawing.Point(1, 185);
			this.button_rotate_at_selected_CCW.Margin = new System.Windows.Forms.Padding(1);
			this.button_rotate_at_selected_CCW.Name = "button_rotate_at_selected_CCW";
			this.button_rotate_at_selected_CCW.Size = new System.Drawing.Size(72, 21);
			this.button_rotate_at_selected_CCW.TabIndex = 96;
			this.button_rotate_at_selected_CCW.Text = "Rot CCW";
			this.tool_tip.SetToolTip(this.button_rotate_at_selected_CCW, "Rotate marked segments counter-clockwise around the center of the selected side.");
			this.button_rotate_at_selected_CCW.UseVisualStyleBackColor = true;
			this.button_rotate_at_selected_CCW.Click += new System.EventHandler(this.button_rotate_at_selected__CCW_Click);
			// 
			// button_connect_with_segment
			// 
			this.button_connect_with_segment.Location = new System.Drawing.Point(1, 321);
			this.button_connect_with_segment.Margin = new System.Windows.Forms.Padding(1);
			this.button_connect_with_segment.Name = "button_connect_with_segment";
			this.button_connect_with_segment.Size = new System.Drawing.Size(145, 21);
			this.button_connect_with_segment.TabIndex = 95;
			this.button_connect_with_segment.Text = "Connect with Segment";
			this.tool_tip.SetToolTip(this.button_connect_with_segment, "Connect the marked and selected sides by creating a new segment between them");
			this.button_connect_with_segment.UseVisualStyleBackColor = true;
			this.button_connect_with_segment.Click += new System.EventHandler(this.button_connect_with_segment_Click);
			// 
			// button_default_segment
			// 
			this.button_default_segment.Location = new System.Drawing.Point(1, 1);
			this.button_default_segment.Margin = new System.Windows.Forms.Padding(1);
			this.button_default_segment.Name = "button_default_segment";
			this.button_default_segment.Size = new System.Drawing.Size(145, 21);
			this.button_default_segment.TabIndex = 2;
			this.button_default_segment.Text = "Default Segment";
			this.tool_tip.SetToolTip(this.button_default_segment, "Create a default segment at the origin");
			this.button_default_segment.UseVisualStyleBackColor = true;
			this.button_default_segment.Click += new System.EventHandler(this.button_default_segment_Click);
			// 
			// button_paste_default
			// 
			this.button_paste_default.Location = new System.Drawing.Point(1, 93);
			this.button_paste_default.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_default.Name = "button_paste_default";
			this.button_paste_default.Size = new System.Drawing.Size(145, 21);
			this.button_paste_default.TabIndex = 36;
			this.button_paste_default.Text = "Paste At Origin";
			this.tool_tip.SetToolTip(this.button_paste_default, "Paste the copied segments at the origin");
			this.button_paste_default.UseVisualStyleBackColor = true;
			this.button_paste_default.Click += new System.EventHandler(this.button_paste_default_Click);
			// 
			// button_mark_walls_straight
			// 
			this.button_mark_walls_straight.Location = new System.Drawing.Point(1, 434);
			this.button_mark_walls_straight.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_walls_straight.Name = "button_mark_walls_straight";
			this.button_mark_walls_straight.Size = new System.Drawing.Size(145, 21);
			this.button_mark_walls_straight.TabIndex = 93;
			this.button_mark_walls_straight.Text = "Mark Walls Straight";
			this.tool_tip.SetToolTip(this.button_mark_walls_straight, "Mark all connected mostly vertical walls on same XY plane - Ctrl+Q");
			this.button_mark_walls_straight.UseVisualStyleBackColor = true;
			this.button_mark_walls_straight.Click += new System.EventHandler(this.button_mark_walls_straight_Click);
			// 
			// button_copy_marked
			// 
			this.button_copy_marked.Location = new System.Drawing.Point(1, 24);
			this.button_copy_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_copy_marked.Name = "button_copy_marked";
			this.button_copy_marked.Size = new System.Drawing.Size(145, 21);
			this.button_copy_marked.TabIndex = 35;
			this.button_copy_marked.Text = "Copy Marked";
			this.tool_tip.SetToolTip(this.button_copy_marked, "Copy the marked segments (selected side is origin) - Ctrl+C");
			this.button_copy_marked.UseVisualStyleBackColor = true;
			this.button_copy_marked.Click += new System.EventHandler(this.button_copy_marked_Click);
			// 
			// slider_extrude_length
			// 
			this.slider_extrude_length.Location = new System.Drawing.Point(1, 457);
			this.slider_extrude_length.Margin = new System.Windows.Forms.Padding(1);
			this.slider_extrude_length.Name = "slider_extrude_length";
			this.slider_extrude_length.RightMouseMultiplier = 2;
			this.slider_extrude_length.Size = new System.Drawing.Size(144, 19);
			this.slider_extrude_length.SlideText = "Extrude Length";
			this.slider_extrude_length.SlideTol = 15;
			this.slider_extrude_length.TabIndex = 92;
			this.tool_tip.SetToolTip(this.slider_extrude_length, "SEE BELOW");
			this.slider_extrude_length.ToolTop = "How far to extrude sides on insert (4 is standard)";
			this.slider_extrude_length.ValueText = "4";
			this.slider_extrude_length.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_extrude_length_Feedback);
			// 
			// label_insert_advance
			// 
			this.label_insert_advance.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_insert_advance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_insert_advance.Location = new System.Drawing.Point(1, 478);
			this.label_insert_advance.Margin = new System.Windows.Forms.Padding(1);
			this.label_insert_advance.Name = "label_insert_advance";
			this.label_insert_advance.Size = new System.Drawing.Size(144, 19);
			this.label_insert_advance.TabIndex = 46;
			this.label_insert_advance.Text = "Insert Advance: ON";
			this.label_insert_advance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_insert_advance, "Select the inserted segment when inserting");
			this.label_insert_advance.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_insert_advance_MouseDown);
			// 
			// button_merge_verts
			// 
			this.button_merge_verts.Location = new System.Drawing.Point(1, 344);
			this.button_merge_verts.Margin = new System.Windows.Forms.Padding(1);
			this.button_merge_verts.Name = "button_merge_verts";
			this.button_merge_verts.Size = new System.Drawing.Size(145, 21);
			this.button_merge_verts.TabIndex = 91;
			this.button_merge_verts.Text = "Merge Overlapping Verts";
			this.tool_tip.SetToolTip(this.button_merge_verts, "Merge all verts that overlap (hack to help with occasional extrude bug)");
			this.button_merge_verts.UseVisualStyleBackColor = true;
			this.button_merge_verts.Click += new System.EventHandler(this.button_merge_verts_Click);
			// 
			// label_side_select
			// 
			this.label_side_select.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_side_select.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_side_select.Location = new System.Drawing.Point(1, 499);
			this.label_side_select.Margin = new System.Windows.Forms.Padding(1);
			this.label_side_select.Name = "label_side_select";
			this.label_side_select.Size = new System.Drawing.Size(144, 19);
			this.label_side_select.TabIndex = 47;
			this.label_side_select.Text = "Side Select: VISIBLE";
			this.label_side_select.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_side_select, "Which sides of a segment can be selected");
			this.label_side_select.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_side_select_MouseDown);
			// 
			// slider_coplanar_angle
			// 
			this.slider_coplanar_angle.Location = new System.Drawing.Point(2, 390);
			this.slider_coplanar_angle.Margin = new System.Windows.Forms.Padding(1);
			this.slider_coplanar_angle.Name = "slider_coplanar_angle";
			this.slider_coplanar_angle.RightMouseMultiplier = 5;
			this.slider_coplanar_angle.Size = new System.Drawing.Size(143, 19);
			this.slider_coplanar_angle.SlideText = "CoPlanar Angle";
			this.slider_coplanar_angle.SlideTol = 15;
			this.slider_coplanar_angle.TabIndex = 89;
			this.tool_tip.SetToolTip(this.slider_coplanar_angle, "SEE BELOW");
			this.slider_coplanar_angle.ToolTop = "Angle for matching Co-Planar faces";
			this.slider_coplanar_angle.ValueText = "15";
			this.slider_coplanar_angle.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_coplanar_angle_Feedback);
			// 
			// button_flipx
			// 
			this.button_flipx.Location = new System.Drawing.Point(1, 162);
			this.button_flipx.Margin = new System.Windows.Forms.Padding(1);
			this.button_flipx.Name = "button_flipx";
			this.button_flipx.Size = new System.Drawing.Size(49, 21);
			this.button_flipx.TabIndex = 48;
			this.button_flipx.Text = "Flip X";
			this.tool_tip.SetToolTip(this.button_flipx, "Flip the marked segments (should be isolated)");
			this.button_flipx.UseVisualStyleBackColor = true;
			this.button_flipx.Click += new System.EventHandler(this.button_flipx_Click);
			// 
			// button_mark_walls
			// 
			this.button_mark_walls.Location = new System.Drawing.Point(1, 411);
			this.button_mark_walls.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_walls.Name = "button_mark_walls";
			this.button_mark_walls.Size = new System.Drawing.Size(145, 21);
			this.button_mark_walls.TabIndex = 59;
			this.button_mark_walls.Text = "Mark Walls";
			this.tool_tip.SetToolTip(this.button_mark_walls, "Mark all connected mostly vertical walls");
			this.button_mark_walls.UseVisualStyleBackColor = true;
			this.button_mark_walls.Click += new System.EventHandler(this.button_mark_walls_Click);
			// 
			// button_flipy
			// 
			this.button_flipy.Location = new System.Drawing.Point(49, 162);
			this.button_flipy.Margin = new System.Windows.Forms.Padding(1);
			this.button_flipy.Name = "button_flipy";
			this.button_flipy.Size = new System.Drawing.Size(49, 21);
			this.button_flipy.TabIndex = 49;
			this.button_flipy.Text = "Flip Y";
			this.tool_tip.SetToolTip(this.button_flipy, "Flip the marked segments (should be isolated)");
			this.button_flipy.UseVisualStyleBackColor = true;
			this.button_flipy.Click += new System.EventHandler(this.button_flipy_Click);
			// 
			// button_mark_coplanar
			// 
			this.button_mark_coplanar.Location = new System.Drawing.Point(1, 367);
			this.button_mark_coplanar.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_coplanar.Name = "button_mark_coplanar";
			this.button_mark_coplanar.Size = new System.Drawing.Size(145, 21);
			this.button_mark_coplanar.TabIndex = 58;
			this.button_mark_coplanar.Text = "Mark CoPlanar";
			this.tool_tip.SetToolTip(this.button_mark_coplanar, "Mark all faces coplanar with the selected side (within the CoPlanar Angle) - Shif" +
        "t+Q");
			this.button_mark_coplanar.UseVisualStyleBackColor = true;
			this.button_mark_coplanar.Click += new System.EventHandler(this.button_mark_coplanar_Click);
			// 
			// button_flipz
			// 
			this.button_flipz.Location = new System.Drawing.Point(97, 162);
			this.button_flipz.Margin = new System.Windows.Forms.Padding(1);
			this.button_flipz.Name = "button_flipz";
			this.button_flipz.Size = new System.Drawing.Size(49, 21);
			this.button_flipz.TabIndex = 50;
			this.button_flipz.Text = "Flip Z";
			this.tool_tip.SetToolTip(this.button_flipz, "Flip the marked segments (should be isolated)");
			this.button_flipz.UseVisualStyleBackColor = true;
			this.button_flipz.Click += new System.EventHandler(this.button_flipz_Click);
			// 
			// button_join_all
			// 
			this.button_join_all.Location = new System.Drawing.Point(1, 298);
			this.button_join_all.Margin = new System.Windows.Forms.Padding(1);
			this.button_join_all.Name = "button_join_all";
			this.button_join_all.Size = new System.Drawing.Size(145, 21);
			this.button_join_all.TabIndex = 57;
			this.button_join_all.Text = "Join All Marked";
			this.tool_tip.SetToolTip(this.button_join_all, "Join all overlapping marked sides (also merges verts when necessary)");
			this.button_join_all.UseVisualStyleBackColor = true;
			this.button_join_all.Click += new System.EventHandler(this.button_join_all_Click);
			// 
			// button_paste_at_side
			// 
			this.button_paste_at_side.Location = new System.Drawing.Point(1, 47);
			this.button_paste_at_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_at_side.Name = "button_paste_at_side";
			this.button_paste_at_side.Size = new System.Drawing.Size(145, 21);
			this.button_paste_at_side.TabIndex = 51;
			this.button_paste_at_side.Text = "Paste At Side";
			this.tool_tip.SetToolTip(this.button_paste_at_side, "Paste the copied segments at the selected side - Ctrl+V");
			this.button_paste_at_side.UseVisualStyleBackColor = true;
			this.button_paste_at_side.Click += new System.EventHandler(this.button_paste_at_side_Click);
			// 
			// button_join_side
			// 
			this.button_join_side.Location = new System.Drawing.Point(1, 275);
			this.button_join_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_join_side.Name = "button_join_side";
			this.button_join_side.Size = new System.Drawing.Size(145, 21);
			this.button_join_side.TabIndex = 56;
			this.button_join_side.Text = "Join Side";
			this.tool_tip.SetToolTip(this.button_join_side, "Join the selected side to the marked side (or nearby side if none marked) - Shift" +
        "+J");
			this.button_join_side.UseVisualStyleBackColor = true;
			this.button_join_side.Click += new System.EventHandler(this.button_join_side_Click);
			// 
			// button_paste_mirrorx
			// 
			this.button_paste_mirrorx.Location = new System.Drawing.Point(1, 139);
			this.button_paste_mirrorx.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_mirrorx.Name = "button_paste_mirrorx";
			this.button_paste_mirrorx.Size = new System.Drawing.Size(49, 21);
			this.button_paste_mirrorx.TabIndex = 52;
			this.button_paste_mirrorx.Text = "PasteX";
			this.tool_tip.SetToolTip(this.button_paste_mirrorx, "Paste the copied segments mirrored (at the origin)");
			this.button_paste_mirrorx.UseVisualStyleBackColor = true;
			this.button_paste_mirrorx.Click += new System.EventHandler(this.button_paste_mirrorx_Click);
			// 
			// button_isolate_marked
			// 
			this.button_isolate_marked.Location = new System.Drawing.Point(1, 229);
			this.button_isolate_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_isolate_marked.Name = "button_isolate_marked";
			this.button_isolate_marked.Size = new System.Drawing.Size(145, 21);
			this.button_isolate_marked.TabIndex = 55;
			this.button_isolate_marked.Text = "Isolate Marked";
			this.tool_tip.SetToolTip(this.button_isolate_marked, "Isolate the marked segments so no outside segments connect to them");
			this.button_isolate_marked.UseVisualStyleBackColor = true;
			this.button_isolate_marked.Click += new System.EventHandler(this.button_isolate_marked_Click);
			// 
			// button_paste_mirrory
			// 
			this.button_paste_mirrory.Location = new System.Drawing.Point(49, 139);
			this.button_paste_mirrory.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_mirrory.Name = "button_paste_mirrory";
			this.button_paste_mirrory.Size = new System.Drawing.Size(49, 21);
			this.button_paste_mirrory.TabIndex = 53;
			this.button_paste_mirrory.Text = "PasteY";
			this.tool_tip.SetToolTip(this.button_paste_mirrory, "Paste the copied segments mirrored (at the origin)");
			this.button_paste_mirrory.UseVisualStyleBackColor = true;
			this.button_paste_mirrory.Click += new System.EventHandler(this.button_paste_mirrory_Click);
			// 
			// button_paste_mirrorz
			// 
			this.button_paste_mirrorz.Location = new System.Drawing.Point(97, 139);
			this.button_paste_mirrorz.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_mirrorz.Name = "button_paste_mirrorz";
			this.button_paste_mirrorz.Size = new System.Drawing.Size(49, 21);
			this.button_paste_mirrorz.TabIndex = 54;
			this.button_paste_mirrorz.Text = "PasteZ";
			this.tool_tip.SetToolTip(this.button_paste_mirrorz, "Paste the copied segments mirrored (at the origin)");
			this.button_paste_mirrorz.UseVisualStyleBackColor = true;
			this.button_paste_mirrorz.Click += new System.EventHandler(this.button_paste_mirrorz_Click);
			// 
			// button_2way
			// 
			this.button_2way.Location = new System.Drawing.Point(1, 252);
			this.button_2way.Margin = new System.Windows.Forms.Padding(1);
			this.button_2way.Name = "button_2way";
			this.button_2way.Size = new System.Drawing.Size(49, 21);
			this.button_2way.TabIndex = 98;
			this.button_2way.Text = "2-Way";
			this.tool_tip.SetToolTip(this.button_2way, "Split the selected segment in 2, parallel to selected side");
			this.button_2way.UseVisualStyleBackColor = true;
			this.button_2way.Click += new System.EventHandler(this.button_2way_Click);
			// 
			// button_5way
			// 
			this.button_5way.Location = new System.Drawing.Point(49, 252);
			this.button_5way.Margin = new System.Windows.Forms.Padding(1);
			this.button_5way.Name = "button_5way";
			this.button_5way.Size = new System.Drawing.Size(49, 21);
			this.button_5way.TabIndex = 99;
			this.button_5way.Text = "5-Way";
			this.tool_tip.SetToolTip(this.button_5way, "Split the selected segment in 5, perpindicular to selected side");
			this.button_5way.UseVisualStyleBackColor = true;
			this.button_5way.Click += new System.EventHandler(this.button_5way_Click);
			// 
			// button_7way
			// 
			this.button_7way.Location = new System.Drawing.Point(97, 252);
			this.button_7way.Margin = new System.Windows.Forms.Padding(1);
			this.button_7way.Name = "button_7way";
			this.button_7way.Size = new System.Drawing.Size(49, 21);
			this.button_7way.TabIndex = 100;
			this.button_7way.Text = "7-Way";
			this.tool_tip.SetToolTip(this.button_7way, "Split the selected segment in 7");
			this.button_7way.UseVisualStyleBackColor = true;
			this.button_7way.Click += new System.EventHandler(this.button_7way_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 541);
			this.button1.Margin = new System.Windows.Forms.Padding(1);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(145, 21);
			this.button1.TabIndex = 93;
			this.button1.Text = "Show Tunnel Builder";
			this.tool_tip.SetToolTip(this.button1, "Open the Tunnel Builder panel");
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button_tunnel_builder_Click);
			// 
			// slider_rotate_angle
			// 
			this.slider_rotate_angle.Location = new System.Drawing.Point(2, 208);
			this.slider_rotate_angle.Margin = new System.Windows.Forms.Padding(1);
			this.slider_rotate_angle.Name = "slider_rotate_angle";
			this.slider_rotate_angle.RightMouseMultiplier = 5;
			this.slider_rotate_angle.Size = new System.Drawing.Size(143, 19);
			this.slider_rotate_angle.SlideText = "Rotate Angle";
			this.slider_rotate_angle.SlideTol = 15;
			this.slider_rotate_angle.TabIndex = 101;
			this.tool_tip.SetToolTip(this.slider_rotate_angle, "SEE BELOW");
			this.slider_rotate_angle.ToolTop = "Angle to rotate with above 2 buttons";
			this.slider_rotate_angle.ValueText = "15";
			this.slider_rotate_angle.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_rotate_angle_Feedback);
			// 
			// button_rotate_at_selected_CW
			// 
			this.button_rotate_at_selected_CW.Location = new System.Drawing.Point(74, 185);
			this.button_rotate_at_selected_CW.Margin = new System.Windows.Forms.Padding(1);
			this.button_rotate_at_selected_CW.Name = "button_rotate_at_selected_CW";
			this.button_rotate_at_selected_CW.Size = new System.Drawing.Size(72, 21);
			this.button_rotate_at_selected_CW.TabIndex = 102;
			this.button_rotate_at_selected_CW.Text = "Rot CW";
			this.tool_tip.SetToolTip(this.button_rotate_at_selected_CW, "Rotate marked segments clockwise around the center of the selected side.");
			this.button_rotate_at_selected_CW.UseVisualStyleBackColor = true;
			this.button_rotate_at_selected_CW.Click += new System.EventHandler(this.button_rotate_at_selected_CW_Click);
			// 
			// button_paste_at_origin_with_entities
			// 
			this.button_paste_at_origin_with_entities.Location = new System.Drawing.Point(1, 116);
			this.button_paste_at_origin_with_entities.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_at_origin_with_entities.Name = "button_paste_at_origin_with_entities";
			this.button_paste_at_origin_with_entities.Size = new System.Drawing.Size(145, 21);
			this.button_paste_at_origin_with_entities.TabIndex = 103;
			this.button_paste_at_origin_with_entities.Text = "Paste At Origin w/ Entities";
			this.tool_tip.SetToolTip(this.button_paste_at_origin_with_entities, "Paste the copied segments & entities at the origin");
			this.button_paste_at_origin_with_entities.UseVisualStyleBackColor = true;
			this.button_paste_at_origin_with_entities.Click += new System.EventHandler(this.button_paste_default_with_entities_Click);
			// 
			// button_paste_at_side_with_entities
			// 
			this.button_paste_at_side_with_entities.Location = new System.Drawing.Point(1, 70);
			this.button_paste_at_side_with_entities.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_at_side_with_entities.Name = "button_paste_at_side_with_entities";
			this.button_paste_at_side_with_entities.Size = new System.Drawing.Size(145, 21);
			this.button_paste_at_side_with_entities.TabIndex = 104;
			this.button_paste_at_side_with_entities.Text = "Paste At Side w/ Entities";
			this.tool_tip.SetToolTip(this.button_paste_at_side_with_entities, "Paste the copied segments & entities at the selected side - Ctrl+Shift+V");
			this.button_paste_at_side_with_entities.UseVisualStyleBackColor = true;
			this.button_paste_at_side_with_entities.Click += new System.EventHandler(this.button_paste_at_side_with_entities_Click);
			// 
			// EditorGeometryPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(148, 595);
			this.Controls.Add(this.button_paste_at_side_with_entities);
			this.Controls.Add(this.button_paste_at_origin_with_entities);
			this.Controls.Add(this.button_rotate_at_selected_CW);
			this.Controls.Add(this.slider_rotate_angle);
			this.Controls.Add(this.button_2way);
			this.Controls.Add(this.button_5way);
			this.Controls.Add(this.button_7way);
			this.Controls.Add(this.label_drag_select);
			this.Controls.Add(this.button_rotate_at_selected_CCW);
			this.Controls.Add(this.button_connect_with_segment);
			this.Controls.Add(this.button_default_segment);
			this.Controls.Add(this.button_paste_default);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button_mark_walls_straight);
			this.Controls.Add(this.button_copy_marked);
			this.Controls.Add(this.slider_extrude_length);
			this.Controls.Add(this.label_insert_advance);
			this.Controls.Add(this.button_merge_verts);
			this.Controls.Add(this.label_side_select);
			this.Controls.Add(this.slider_coplanar_angle);
			this.Controls.Add(this.button_flipx);
			this.Controls.Add(this.button_mark_walls);
			this.Controls.Add(this.button_flipy);
			this.Controls.Add(this.button_mark_coplanar);
			this.Controls.Add(this.button_flipz);
			this.Controls.Add(this.button_join_all);
			this.Controls.Add(this.button_paste_at_side);
			this.Controls.Add(this.button_join_side);
			this.Controls.Add(this.button_paste_mirrorx);
			this.Controls.Add(this.button_isolate_marked);
			this.Controls.Add(this.button_paste_mirrory);
			this.Controls.Add(this.button_paste_mirrorz);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.MinimumSize = new System.Drawing.Size(164, 39);
			this.Name = "EditorGeometryPane";
			this.Text = "GEOMETRY";
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button button_rotate_at_selected_CCW;
		private System.Windows.Forms.Button button_connect_with_segment;
		private System.Windows.Forms.Button button_mark_walls_straight;
		private SliderLabel slider_extrude_length;
		private System.Windows.Forms.Button button_merge_verts;
		private SliderLabel slider_coplanar_angle;
		private System.Windows.Forms.Button button_mark_walls;
		private System.Windows.Forms.Button button_mark_coplanar;
		private System.Windows.Forms.Button button_join_all;
		private System.Windows.Forms.Button button_join_side;
		private System.Windows.Forms.Button button_isolate_marked;
		private System.Windows.Forms.Button button_paste_mirrorz;
		private System.Windows.Forms.Button button_paste_mirrory;
		private System.Windows.Forms.Button button_paste_mirrorx;
		private System.Windows.Forms.Button button_paste_at_side;
		private System.Windows.Forms.Button button_flipz;
		private System.Windows.Forms.Button button_flipy;
		private System.Windows.Forms.Button button_flipx;
		private System.Windows.Forms.Label label_side_select;
		private System.Windows.Forms.Label label_insert_advance;
		private System.Windows.Forms.Button button_default_segment;
		private System.Windows.Forms.Button button_copy_marked;
		private System.Windows.Forms.Button button_paste_default;
		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.Label label_drag_select;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button_2way;
		private System.Windows.Forms.Button button_5way;
		private System.Windows.Forms.Button button_7way;
		private SliderLabel slider_rotate_angle;
		private System.Windows.Forms.Button button_rotate_at_selected_CW;
		private System.Windows.Forms.Button button_paste_at_origin_with_entities;
		private System.Windows.Forms.Button button_paste_at_side_with_entities;
	}
}