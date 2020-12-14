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
	partial class EditorGeometryDecalsPane
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
			this.button_decal_hide_marked2 = new System.Windows.Forms.Button();
			this.button_decal_hide_marked1 = new System.Windows.Forms.Button();
			this.button_decal_hide2 = new System.Windows.Forms.Button();
			this.button_decal_hide1 = new System.Windows.Forms.Button();
			this.button_preset_whole = new System.Windows.Forms.Button();
			this.button_preset_none = new System.Windows.Forms.Button();
			this.button_preset_adj_ud = new System.Windows.Forms.Button();
			this.button_preset_avg_ud = new System.Windows.Forms.Button();
			this.button_preset_adj_lr = new System.Windows.Forms.Button();
			this.button_preset_avg_lr = new System.Windows.Forms.Button();
			this.label_draw_clips = new System.Windows.Forms.Label();
			this.slider_decal_rotation = new OverloadLevelEditor.SliderLabel();
			this.button_decal_copy_2 = new System.Windows.Forms.Button();
			this.button_decal_copy_1 = new System.Windows.Forms.Button();
			this.slider_decal_offset_v = new OverloadLevelEditor.SliderLabel();
			this.slider_decal_offset_u = new OverloadLevelEditor.SliderLabel();
			this.slider_decal_repeat_v = new OverloadLevelEditor.SliderLabel();
			this.slider_decal_repeat_u = new OverloadLevelEditor.SliderLabel();
			this.label_decal_clip_down = new System.Windows.Forms.Label();
			this.label_decal_clip_up = new System.Windows.Forms.Label();
			this.label_decal_clip_right = new System.Windows.Forms.Label();
			this.label_decal_clip_left = new System.Windows.Forms.Label();
			this.label_decal_alignment = new System.Windows.Forms.Label();
			this.button_decal_list = new System.Windows.Forms.Button();
			this.label_decal2 = new System.Windows.Forms.Label();
			this.label_decal1 = new System.Windows.Forms.Label();
			this.label_decal_mirror = new System.Windows.Forms.Label();
			this.label_edge_clip = new System.Windows.Forms.Label();
			this.label_decal_insert = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_decal_hide_marked2
			// 
			this.button_decal_hide_marked2.Location = new System.Drawing.Point(2, 413);
			this.button_decal_hide_marked2.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_hide_marked2.Name = "button_decal_hide_marked2";
			this.button_decal_hide_marked2.Size = new System.Drawing.Size(145, 21);
			this.button_decal_hide_marked2.TabIndex = 104;
			this.button_decal_hide_marked2.Text = "Hide Marked Decals 2";
			this.tool_tip.SetToolTip(this.button_decal_hide_marked2, "Copy Decal 2 from the selected side to all the marked sides");
			this.button_decal_hide_marked2.UseVisualStyleBackColor = true;
			this.button_decal_hide_marked2.Click += new System.EventHandler(this.button_decal_hide_marked2_Click);
			// 
			// button_decal_hide_marked1
			// 
			this.button_decal_hide_marked1.Location = new System.Drawing.Point(2, 390);
			this.button_decal_hide_marked1.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_hide_marked1.Name = "button_decal_hide_marked1";
			this.button_decal_hide_marked1.Size = new System.Drawing.Size(145, 21);
			this.button_decal_hide_marked1.TabIndex = 103;
			this.button_decal_hide_marked1.Text = "Hide Marked Decals 1";
			this.tool_tip.SetToolTip(this.button_decal_hide_marked1, "Copy Decal 1 from the selected side to all the marked sides");
			this.button_decal_hide_marked1.UseVisualStyleBackColor = true;
			this.button_decal_hide_marked1.Click += new System.EventHandler(this.button_decal_hide_marked1_Click);
			// 
			// button_decal_hide2
			// 
			this.button_decal_hide2.Location = new System.Drawing.Point(76, 367);
			this.button_decal_hide2.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_hide2.Name = "button_decal_hide2";
			this.button_decal_hide2.Size = new System.Drawing.Size(71, 21);
			this.button_decal_hide2.TabIndex = 102;
			this.button_decal_hide2.Text = "Hide 2";
			this.tool_tip.SetToolTip(this.button_decal_hide2, "Toggle Decal 2 being hidden");
			this.button_decal_hide2.UseVisualStyleBackColor = true;
			this.button_decal_hide2.Click += new System.EventHandler(this.button_decal_hide2_Click);
			// 
			// button_decal_hide1
			// 
			this.button_decal_hide1.Location = new System.Drawing.Point(2, 367);
			this.button_decal_hide1.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_hide1.Name = "button_decal_hide1";
			this.button_decal_hide1.Size = new System.Drawing.Size(71, 21);
			this.button_decal_hide1.TabIndex = 101;
			this.button_decal_hide1.Text = "Hide 1";
			this.tool_tip.SetToolTip(this.button_decal_hide1, "Toggle Decal 1 being hidden");
			this.button_decal_hide1.UseVisualStyleBackColor = true;
			this.button_decal_hide1.Click += new System.EventHandler(this.button_decal_hide1_Click);
			// 
			// button_preset_whole
			// 
			this.button_preset_whole.Location = new System.Drawing.Point(76, 344);
			this.button_preset_whole.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_whole.Name = "button_preset_whole";
			this.button_preset_whole.Size = new System.Drawing.Size(71, 21);
			this.button_preset_whole.TabIndex = 100;
			this.button_preset_whole.Text = "Reset";
			this.tool_tip.SetToolTip(this.button_preset_whole, "Reset all rotation/repeat/offset settings to default");
			this.button_preset_whole.UseVisualStyleBackColor = true;
			this.button_preset_whole.Click += new System.EventHandler(this.button_preset_whole_Click);
			// 
			// button_preset_none
			// 
			this.button_preset_none.Location = new System.Drawing.Point(2, 344);
			this.button_preset_none.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_none.Name = "button_preset_none";
			this.button_preset_none.Size = new System.Drawing.Size(71, 21);
			this.button_preset_none.TabIndex = 99;
			this.button_preset_none.Text = "None";
			this.tool_tip.SetToolTip(this.button_preset_none, "Set all edge clipping to None");
			this.button_preset_none.UseVisualStyleBackColor = true;
			this.button_preset_none.Click += new System.EventHandler(this.button_preset_none_Click);
			// 
			// button_preset_adj_ud
			// 
			this.button_preset_adj_ud.Location = new System.Drawing.Point(76, 321);
			this.button_preset_adj_ud.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_adj_ud.Name = "button_preset_adj_ud";
			this.button_preset_adj_ud.Size = new System.Drawing.Size(71, 21);
			this.button_preset_adj_ud.TabIndex = 98;
			this.button_preset_adj_ud.Text = "Adj UD";
			this.tool_tip.SetToolTip(this.button_preset_adj_ud, "Set UD edge clipping to Adjacent");
			this.button_preset_adj_ud.UseVisualStyleBackColor = true;
			this.button_preset_adj_ud.Click += new System.EventHandler(this.button_preset_adj_ud_Click);
			// 
			// button_preset_avg_ud
			// 
			this.button_preset_avg_ud.Location = new System.Drawing.Point(76, 298);
			this.button_preset_avg_ud.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_avg_ud.Name = "button_preset_avg_ud";
			this.button_preset_avg_ud.Size = new System.Drawing.Size(71, 21);
			this.button_preset_avg_ud.TabIndex = 97;
			this.button_preset_avg_ud.Text = "Seg UD";
			this.tool_tip.SetToolTip(this.button_preset_avg_ud, "Set UD edge clipping to Segment");
			this.button_preset_avg_ud.UseVisualStyleBackColor = true;
			this.button_preset_avg_ud.Click += new System.EventHandler(this.button_preset_avg_ud_Click);
			// 
			// button_preset_adj_lr
			// 
			this.button_preset_adj_lr.Location = new System.Drawing.Point(2, 321);
			this.button_preset_adj_lr.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_adj_lr.Name = "button_preset_adj_lr";
			this.button_preset_adj_lr.Size = new System.Drawing.Size(71, 21);
			this.button_preset_adj_lr.TabIndex = 96;
			this.button_preset_adj_lr.Text = "Adj LR";
			this.tool_tip.SetToolTip(this.button_preset_adj_lr, "Set LR edge clipping to Adjacent");
			this.button_preset_adj_lr.UseVisualStyleBackColor = true;
			this.button_preset_adj_lr.Click += new System.EventHandler(this.button_preset_adj_lr_Click);
			// 
			// button_preset_avg_lr
			// 
			this.button_preset_avg_lr.Location = new System.Drawing.Point(2, 298);
			this.button_preset_avg_lr.Margin = new System.Windows.Forms.Padding(1);
			this.button_preset_avg_lr.Name = "button_preset_avg_lr";
			this.button_preset_avg_lr.Size = new System.Drawing.Size(71, 21);
			this.button_preset_avg_lr.TabIndex = 95;
			this.button_preset_avg_lr.Text = "Seg LR";
			this.tool_tip.SetToolTip(this.button_preset_avg_lr, "Set LR edge clipping to Segment");
			this.button_preset_avg_lr.UseVisualStyleBackColor = true;
			this.button_preset_avg_lr.Click += new System.EventHandler(this.button_preset_avg_lr_Click);
			// 
			// label_draw_clips
			// 
			this.label_draw_clips.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_draw_clips.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_draw_clips.Location = new System.Drawing.Point(2, 483);
			this.label_draw_clips.Margin = new System.Windows.Forms.Padding(1);
			this.label_draw_clips.Name = "label_draw_clips";
			this.label_draw_clips.Size = new System.Drawing.Size(144, 19);
			this.label_draw_clips.TabIndex = 94;
			this.label_draw_clips.Text = "Clip Planes: SHOW";
			this.label_draw_clips.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_draw_clips, "Show the clip planes visually");
			this.label_draw_clips.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_draw_clips_MouseDown);
			// 
			// slider_decal_rotation
			// 
			this.slider_decal_rotation.Location = new System.Drawing.Point(2, 109);
			this.slider_decal_rotation.Margin = new System.Windows.Forms.Padding(1);
			this.slider_decal_rotation.Name = "slider_decal_rotation";
			this.slider_decal_rotation.RightMouseMultiplier = 5;
			this.slider_decal_rotation.Size = new System.Drawing.Size(144, 19);
			this.slider_decal_rotation.SlideText = "Rotation";
			this.slider_decal_rotation.SlideTol = 20;
			this.slider_decal_rotation.TabIndex = 54;
			this.tool_tip.SetToolTip(this.slider_decal_rotation, "SEE BELOW");
			this.slider_decal_rotation.ToolTop = "Rotation of the decal";
			this.slider_decal_rotation.ValueText = "0";
			this.slider_decal_rotation.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_decal_rotation_Feedback);
			// 
			// button_decal_copy_2
			// 
			this.button_decal_copy_2.Location = new System.Drawing.Point(2, 459);
			this.button_decal_copy_2.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_copy_2.Name = "button_decal_copy_2";
			this.button_decal_copy_2.Size = new System.Drawing.Size(145, 21);
			this.button_decal_copy_2.TabIndex = 93;
			this.button_decal_copy_2.Text = "Copy 2 To Marked";
			this.tool_tip.SetToolTip(this.button_decal_copy_2, "Copy Decal 2 from the selected side to all the marked sides");
			this.button_decal_copy_2.UseVisualStyleBackColor = true;
			this.button_decal_copy_2.Click += new System.EventHandler(this.button_decal_copy_2_Click);
			// 
			// button_decal_copy_1
			// 
			this.button_decal_copy_1.Location = new System.Drawing.Point(2, 436);
			this.button_decal_copy_1.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_copy_1.Name = "button_decal_copy_1";
			this.button_decal_copy_1.Size = new System.Drawing.Size(145, 21);
			this.button_decal_copy_1.TabIndex = 91;
			this.button_decal_copy_1.Text = "Copy 1 To Marked";
			this.tool_tip.SetToolTip(this.button_decal_copy_1, "Copy Decal 1 from the selected side to all the marked sides");
			this.button_decal_copy_1.UseVisualStyleBackColor = true;
			this.button_decal_copy_1.Click += new System.EventHandler(this.button_decal_copy_1_Click);
			// 
			// slider_decal_offset_v
			// 
			this.slider_decal_offset_v.Location = new System.Drawing.Point(2, 193);
			this.slider_decal_offset_v.Margin = new System.Windows.Forms.Padding(1);
			this.slider_decal_offset_v.Name = "slider_decal_offset_v";
			this.slider_decal_offset_v.RightMouseMultiplier = 5;
			this.slider_decal_offset_v.Size = new System.Drawing.Size(144, 19);
			this.slider_decal_offset_v.SlideText = "Offset V";
			this.slider_decal_offset_v.SlideTol = 20;
			this.slider_decal_offset_v.TabIndex = 88;
			this.tool_tip.SetToolTip(this.slider_decal_offset_v, "SEE BELOW");
			this.slider_decal_offset_v.ToolTop = "Offset for the decal in the V direction";
			this.slider_decal_offset_v.ValueText = "0/4";
			this.slider_decal_offset_v.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_decal_offset_v_Feedback);
			// 
			// slider_decal_offset_u
			// 
			this.slider_decal_offset_u.Location = new System.Drawing.Point(2, 172);
			this.slider_decal_offset_u.Margin = new System.Windows.Forms.Padding(1);
			this.slider_decal_offset_u.Name = "slider_decal_offset_u";
			this.slider_decal_offset_u.RightMouseMultiplier = 5;
			this.slider_decal_offset_u.Size = new System.Drawing.Size(144, 19);
			this.slider_decal_offset_u.SlideText = "Offset U";
			this.slider_decal_offset_u.SlideTol = 20;
			this.slider_decal_offset_u.TabIndex = 87;
			this.tool_tip.SetToolTip(this.slider_decal_offset_u, "SEE BELOW");
			this.slider_decal_offset_u.ToolTop = "Offset for the decal in the U direction";
			this.slider_decal_offset_u.ValueText = "0/4";
			this.slider_decal_offset_u.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_decal_offset_u_Feedback);
			// 
			// slider_decal_repeat_v
			// 
			this.slider_decal_repeat_v.Location = new System.Drawing.Point(2, 151);
			this.slider_decal_repeat_v.Margin = new System.Windows.Forms.Padding(1);
			this.slider_decal_repeat_v.Name = "slider_decal_repeat_v";
			this.slider_decal_repeat_v.RightMouseMultiplier = 5;
			this.slider_decal_repeat_v.Size = new System.Drawing.Size(144, 19);
			this.slider_decal_repeat_v.SlideText = "Repeat V";
			this.slider_decal_repeat_v.SlideTol = 15;
			this.slider_decal_repeat_v.TabIndex = 86;
			this.tool_tip.SetToolTip(this.slider_decal_repeat_v, "SEE BELOW");
			this.slider_decal_repeat_v.ToolTop = "How many times to repeat the decal in its V direction";
			this.slider_decal_repeat_v.ValueText = "MAX";
			this.slider_decal_repeat_v.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_decal_repeat_v_Feedback);
			// 
			// slider_decal_repeat_u
			// 
			this.slider_decal_repeat_u.Location = new System.Drawing.Point(2, 130);
			this.slider_decal_repeat_u.Margin = new System.Windows.Forms.Padding(1);
			this.slider_decal_repeat_u.Name = "slider_decal_repeat_u";
			this.slider_decal_repeat_u.RightMouseMultiplier = 5;
			this.slider_decal_repeat_u.Size = new System.Drawing.Size(144, 19);
			this.slider_decal_repeat_u.SlideText = "Repeat U";
			this.slider_decal_repeat_u.SlideTol = 15;
			this.slider_decal_repeat_u.TabIndex = 55;
			this.tool_tip.SetToolTip(this.slider_decal_repeat_u, "SEE BELOW");
			this.slider_decal_repeat_u.ToolTop = "How many times to repeat the decal in its U direction";
			this.slider_decal_repeat_u.ValueText = "MAX";
			this.slider_decal_repeat_u.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_decal_repeat_u_Feedback);
			// 
			// label_decal_clip_down
			// 
			this.label_decal_clip_down.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_clip_down.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_clip_down.Location = new System.Drawing.Point(2, 277);
			this.label_decal_clip_down.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_clip_down.Name = "label_decal_clip_down";
			this.label_decal_clip_down.Size = new System.Drawing.Size(144, 19);
			this.label_decal_clip_down.TabIndex = 83;
			this.label_decal_clip_down.Text = "NONE";
			this.label_decal_clip_down.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_decal_clip_down, "Clipping type for the bottom edge");
			this.label_decal_clip_down.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_clip_down_MouseDown);
			// 
			// label_decal_clip_up
			// 
			this.label_decal_clip_up.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_clip_up.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_clip_up.Location = new System.Drawing.Point(2, 235);
			this.label_decal_clip_up.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_clip_up.Name = "label_decal_clip_up";
			this.label_decal_clip_up.Size = new System.Drawing.Size(144, 19);
			this.label_decal_clip_up.TabIndex = 82;
			this.label_decal_clip_up.Text = "NONE";
			this.label_decal_clip_up.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_decal_clip_up, "Clipping type for the top edge");
			this.label_decal_clip_up.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_clip_up_MouseDown);
			// 
			// label_decal_clip_right
			// 
			this.label_decal_clip_right.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_clip_right.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_clip_right.Location = new System.Drawing.Point(75, 256);
			this.label_decal_clip_right.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_clip_right.Name = "label_decal_clip_right";
			this.label_decal_clip_right.Size = new System.Drawing.Size(71, 19);
			this.label_decal_clip_right.TabIndex = 81;
			this.label_decal_clip_right.Text = "NONE";
			this.label_decal_clip_right.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_decal_clip_right, "Clipping type for the right edge");
			this.label_decal_clip_right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_clip_right_MouseDown);
			// 
			// label_decal_clip_left
			// 
			this.label_decal_clip_left.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_clip_left.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_clip_left.Location = new System.Drawing.Point(2, 256);
			this.label_decal_clip_left.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_clip_left.Name = "label_decal_clip_left";
			this.label_decal_clip_left.Size = new System.Drawing.Size(71, 19);
			this.label_decal_clip_left.TabIndex = 78;
			this.label_decal_clip_left.Text = "NONE";
			this.label_decal_clip_left.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tool_tip.SetToolTip(this.label_decal_clip_left, "Clipping type for the left edge");
			this.label_decal_clip_left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_clip_left_MouseDown);
			// 
			// label_decal_alignment
			// 
			this.label_decal_alignment.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_alignment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_alignment.Location = new System.Drawing.Point(2, 67);
			this.label_decal_alignment.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_alignment.Name = "label_decal_alignment";
			this.label_decal_alignment.Size = new System.Drawing.Size(144, 19);
			this.label_decal_alignment.TabIndex = 71;
			this.label_decal_alignment.Text = "Align: TEXTURE";
			this.label_decal_alignment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_decal_alignment, "How to align the decal to the side by default");
			this.label_decal_alignment.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_alignment_MouseDown);
			// 
			// button_decal_list
			// 
			this.button_decal_list.Location = new System.Drawing.Point(2, 44);
			this.button_decal_list.Margin = new System.Windows.Forms.Padding(1);
			this.button_decal_list.Name = "button_decal_list";
			this.button_decal_list.Size = new System.Drawing.Size(145, 21);
			this.button_decal_list.TabIndex = 68;
			this.button_decal_list.Text = "Show Decal List";
			this.tool_tip.SetToolTip(this.button_decal_list, "Show the list of decals [F2]");
			this.button_decal_list.UseVisualStyleBackColor = true;
			this.button_decal_list.Click += new System.EventHandler(this.button_decal_list_Click);
			// 
			// label_decal2
			// 
			this.label_decal2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label_decal2.Location = new System.Drawing.Point(2, 23);
			this.label_decal2.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal2.Name = "label_decal2";
			this.label_decal2.Size = new System.Drawing.Size(144, 19);
			this.label_decal2.TabIndex = 61;
			this.label_decal2.Text = "2: -";
			this.label_decal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_decal2, "The selected side\'s 2nd geometry decal");
			this.label_decal2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal2_MouseDown);
			// 
			// label_decal1
			// 
			this.label_decal1.BackColor = System.Drawing.Color.Gold;
			this.label_decal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal1.Location = new System.Drawing.Point(2, 2);
			this.label_decal1.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal1.Name = "label_decal1";
			this.label_decal1.Size = new System.Drawing.Size(144, 19);
			this.label_decal1.TabIndex = 60;
			this.label_decal1.Text = "1: -";
			this.label_decal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_decal1, "The selected side\'s 1st geometry decal");
			this.label_decal1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal1_MouseDown);
			// 
			// label_decal_mirror
			// 
			this.label_decal_mirror.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_mirror.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_mirror.Enabled = false;
			this.label_decal_mirror.ForeColor = System.Drawing.SystemColors.AppWorkspace;
			this.label_decal_mirror.Location = new System.Drawing.Point(2, 88);
			this.label_decal_mirror.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_mirror.Name = "label_decal_mirror";
			this.label_decal_mirror.Size = new System.Drawing.Size(144, 19);
			this.label_decal_mirror.TabIndex = 72;
			this.label_decal_mirror.Text = "Mirror: DISABLED";
			this.label_decal_mirror.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_decal_mirror.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_mirror_MouseDown);
			// 
			// label_edge_clip
			// 
			this.label_edge_clip.BackColor = System.Drawing.SystemColors.Control;
			this.label_edge_clip.Location = new System.Drawing.Point(2, 214);
			this.label_edge_clip.Margin = new System.Windows.Forms.Padding(1);
			this.label_edge_clip.Name = "label_edge_clip";
			this.label_edge_clip.Size = new System.Drawing.Size(144, 19);
			this.label_edge_clip.TabIndex = 84;
			this.label_edge_clip.Text = "EDGE CLIPPING";
			this.label_edge_clip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_decal_insert
			// 
			this.label_decal_insert.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_decal_insert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_decal_insert.Location = new System.Drawing.Point(2, 504);
			this.label_decal_insert.Margin = new System.Windows.Forms.Padding(1);
			this.label_decal_insert.Name = "label_decal_insert";
			this.label_decal_insert.Size = new System.Drawing.Size(144, 19);
			this.label_decal_insert.TabIndex = 105;
			this.label_decal_insert.Text = "Insert Decals: ALL";
			this.label_decal_insert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_decal_insert, "When inserting new segments, which decals to copy");
			this.label_decal_insert.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_decal_insert_MouseDown);
			// 
			// EditorGeometryDecalsPane
			// 
			this.AutoHidePortion = 0.2D;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(148, 534);
			this.Controls.Add(this.label_decal_insert);
			this.Controls.Add(this.button_decal_hide_marked2);
			this.Controls.Add(this.button_decal_hide_marked1);
			this.Controls.Add(this.label_decal1);
			this.Controls.Add(this.button_decal_hide2);
			this.Controls.Add(this.label_decal2);
			this.Controls.Add(this.button_decal_hide1);
			this.Controls.Add(this.button_decal_list);
			this.Controls.Add(this.button_preset_whole);
			this.Controls.Add(this.label_decal_alignment);
			this.Controls.Add(this.button_preset_none);
			this.Controls.Add(this.label_decal_mirror);
			this.Controls.Add(this.button_preset_adj_ud);
			this.Controls.Add(this.label_decal_clip_left);
			this.Controls.Add(this.button_preset_avg_ud);
			this.Controls.Add(this.label_decal_clip_right);
			this.Controls.Add(this.button_preset_adj_lr);
			this.Controls.Add(this.label_decal_clip_up);
			this.Controls.Add(this.button_preset_avg_lr);
			this.Controls.Add(this.label_decal_clip_down);
			this.Controls.Add(this.label_draw_clips);
			this.Controls.Add(this.label_edge_clip);
			this.Controls.Add(this.slider_decal_rotation);
			this.Controls.Add(this.slider_decal_repeat_u);
			this.Controls.Add(this.button_decal_copy_2);
			this.Controls.Add(this.slider_decal_repeat_v);
			this.Controls.Add(this.button_decal_copy_1);
			this.Controls.Add(this.slider_decal_offset_u);
			this.Controls.Add(this.slider_decal_offset_v);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.Name = "EditorGeometryDecalsPane";
			this.Text = "DECALS";
			this.Load += new System.EventHandler(this.EditorGeometryDecalsPane_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button button_decal_hide_marked2;
		private System.Windows.Forms.Button button_decal_hide_marked1;
		private System.Windows.Forms.Button button_decal_hide2;
		private System.Windows.Forms.Button button_decal_hide1;
		private System.Windows.Forms.Button button_preset_whole;
		private System.Windows.Forms.Button button_preset_none;
		private System.Windows.Forms.Button button_preset_adj_ud;
		private System.Windows.Forms.Button button_preset_avg_ud;
		private System.Windows.Forms.Button button_preset_adj_lr;
		private System.Windows.Forms.Button button_preset_avg_lr;
		private System.Windows.Forms.Label label_draw_clips;
		private SliderLabel slider_decal_rotation;
		private System.Windows.Forms.Button button_decal_copy_2;
		private System.Windows.Forms.Button button_decal_copy_1;
		private SliderLabel slider_decal_offset_v;
		private SliderLabel slider_decal_offset_u;
		private SliderLabel slider_decal_repeat_v;
		private SliderLabel slider_decal_repeat_u;
		private System.Windows.Forms.Label label_edge_clip;
		private System.Windows.Forms.Label label_decal_clip_down;
		private System.Windows.Forms.Label label_decal_clip_up;
		private System.Windows.Forms.Label label_decal_clip_right;
		private System.Windows.Forms.Label label_decal_clip_left;
		private System.Windows.Forms.Label label_decal_mirror;
		private System.Windows.Forms.Label label_decal_alignment;
		private System.Windows.Forms.Button button_decal_list;
		private System.Windows.Forms.Label label_decal2;
		private System.Windows.Forms.Label label_decal1;
		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.Label label_decal_insert;
	}
}