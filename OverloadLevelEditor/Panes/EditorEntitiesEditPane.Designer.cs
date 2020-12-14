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
	partial class EditorEntitiesEditPane
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
			this.button_entity_align_side = new System.Windows.Forms.Button();
			this.button_entity_move = new System.Windows.Forms.Button();
			this.button_entity_copy_subtype = new System.Windows.Forms.Button();
			this.button_entity_marked_reset_rotation = new System.Windows.Forms.Button();
			this.button_entity_mark_subtype = new System.Windows.Forms.Button();
			this.button_entity_marked_face_selected_side = new System.Windows.Forms.Button();
			this.button_entity_mark_type = new System.Windows.Forms.Button();
			this.label_entity_pivot = new System.Windows.Forms.Label();
			this.button_entity_copy_to_marked = new System.Windows.Forms.Button();
			this.button_entity_duplicate = new System.Windows.Forms.Button();
			this.button_entity_move_side = new System.Windows.Forms.Button();
			this.button_entity_move_segment = new System.Windows.Forms.Button();
			this.label_entity_guid_value = new System.Windows.Forms.Label();
			this.label_entity_type_value = new System.Windows.Forms.Label();
			this.button_entity_face_selected_side = new System.Windows.Forms.Button();
			this.button_entity_reset_rotation = new System.Windows.Forms.Button();
			this.label_entity_guid = new System.Windows.Forms.Label();
			this.label_entity_type = new System.Windows.Forms.Label();
			this.comboBox_entity_subtype = new System.Windows.Forms.ComboBox();
			this.prop_grid_entity = new System.Windows.Forms.PropertyGrid();
			this.label_entity_sel_type = new System.Windows.Forms.Label();
			this.button_place_entity_side = new System.Windows.Forms.Button();
			this.button_place_entity_segment = new System.Windows.Forms.Button();
			this.comboBox_entity_type = new System.Windows.Forms.ComboBox();
			this.label_selected = new System.Windows.Forms.Label();
			this.label_marked = new System.Windows.Forms.Label();
			this.label_team = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_entity_align_side
			// 
			this.button_entity_align_side.Location = new System.Drawing.Point(1, 393);
			this.button_entity_align_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_align_side.Name = "button_entity_align_side";
			this.button_entity_align_side.Size = new System.Drawing.Size(145, 21);
			this.button_entity_align_side.TabIndex = 121;
			this.button_entity_align_side.Text = "Align to Side";
			this.tool_tip.SetToolTip(this.button_entity_align_side, "Align selected entity to selected side");
			this.button_entity_align_side.UseVisualStyleBackColor = true;
			this.button_entity_align_side.Click += new System.EventHandler(this.button_entity_align_side_Click);
			// 
			// button_entity_move
			// 
			this.button_entity_move.Location = new System.Drawing.Point(1, 462);
			this.button_entity_move.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_move.Name = "button_entity_move";
			this.button_entity_move.Size = new System.Drawing.Size(145, 21);
			this.button_entity_move.TabIndex = 95;
			this.button_entity_move.Text = "Enter Position Coords";
			this.tool_tip.SetToolTip(this.button_entity_move, "Open a dialog to enter X,Y,Z coordinates for the entity");
			this.button_entity_move.UseVisualStyleBackColor = true;
			this.button_entity_move.Click += new System.EventHandler(this.button_entity_move_Click);
			// 
			// button_entity_copy_subtype
			// 
			this.button_entity_copy_subtype.Location = new System.Drawing.Point(1, 554);
			this.button_entity_copy_subtype.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_copy_subtype.Name = "button_entity_copy_subtype";
			this.button_entity_copy_subtype.Size = new System.Drawing.Size(145, 21);
			this.button_entity_copy_subtype.TabIndex = 119;
			this.button_entity_copy_subtype.Text = "Copy Subtype to Marked";
			this.tool_tip.SetToolTip(this.button_entity_copy_subtype, "Copy selected entity\'s subtype to all marked entities of the same type");
			this.button_entity_copy_subtype.UseVisualStyleBackColor = true;
			this.button_entity_copy_subtype.Click += new System.EventHandler(this.button_entity_copy_subtype_Click);
			// 
			// button_entity_marked_reset_rotation
			// 
			this.button_entity_marked_reset_rotation.Location = new System.Drawing.Point(1, 598);
			this.button_entity_marked_reset_rotation.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_marked_reset_rotation.Name = "button_entity_marked_reset_rotation";
			this.button_entity_marked_reset_rotation.Size = new System.Drawing.Size(145, 21);
			this.button_entity_marked_reset_rotation.TabIndex = 96;
			this.button_entity_marked_reset_rotation.Text = "Reset Rotation";
			this.tool_tip.SetToolTip(this.button_entity_marked_reset_rotation, "Set rotation of all marked entities to default (facing world Z)");
			this.button_entity_marked_reset_rotation.UseVisualStyleBackColor = true;
			this.button_entity_marked_reset_rotation.Click += new System.EventHandler(this.button_entity_marked_reset_rotation_Click);
			// 
			// button_entity_mark_subtype
			// 
			this.button_entity_mark_subtype.Location = new System.Drawing.Point(1, 508);
			this.button_entity_mark_subtype.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_mark_subtype.Name = "button_entity_mark_subtype";
			this.button_entity_mark_subtype.Size = new System.Drawing.Size(145, 21);
			this.button_entity_mark_subtype.TabIndex = 118;
			this.button_entity_mark_subtype.Text = "Mark Entities of Subtype";
			this.tool_tip.SetToolTip(this.button_entity_mark_subtype, "Mark all entities of the same type and subtype as the selected entitity");
			this.button_entity_mark_subtype.UseVisualStyleBackColor = true;
			this.button_entity_mark_subtype.Click += new System.EventHandler(this.button_entity_mark_subtype_Click);
			// 
			// button_entity_marked_face_selected_side
			// 
			this.button_entity_marked_face_selected_side.Location = new System.Drawing.Point(1, 621);
			this.button_entity_marked_face_selected_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_marked_face_selected_side.Name = "button_entity_marked_face_selected_side";
			this.button_entity_marked_face_selected_side.Size = new System.Drawing.Size(145, 21);
			this.button_entity_marked_face_selected_side.TabIndex = 97;
			this.button_entity_marked_face_selected_side.Text = "Face Selected Side";
			this.tool_tip.SetToolTip(this.button_entity_marked_face_selected_side, "Make all marked entities face the selected side");
			this.button_entity_marked_face_selected_side.UseVisualStyleBackColor = true;
			this.button_entity_marked_face_selected_side.Click += new System.EventHandler(this.button_entity_marked_face_selected_side_Click);
			// 
			// button_entity_mark_type
			// 
			this.button_entity_mark_type.Location = new System.Drawing.Point(1, 485);
			this.button_entity_mark_type.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_mark_type.Name = "button_entity_mark_type";
			this.button_entity_mark_type.Size = new System.Drawing.Size(145, 21);
			this.button_entity_mark_type.TabIndex = 117;
			this.button_entity_mark_type.Text = "Mark Entities of Type";
			this.tool_tip.SetToolTip(this.button_entity_mark_type, "Mark all entities of the same type as the selected entity");
			this.button_entity_mark_type.UseVisualStyleBackColor = true;
			this.button_entity_mark_type.Click += new System.EventHandler(this.button_entity_mark_type_Click);
			// 
			// label_entity_pivot
			// 
			this.label_entity_pivot.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_entity_pivot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_entity_pivot.Location = new System.Drawing.Point(1, 667);
			this.label_entity_pivot.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_pivot.Name = "label_entity_pivot";
			this.label_entity_pivot.Size = new System.Drawing.Size(145, 19);
			this.label_entity_pivot.TabIndex = 98;
			this.label_entity_pivot.Text = "Pivot: MARKED";
			this.label_entity_pivot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_pivot, "Change the global pivot of movement/rotation setting");
			this.label_entity_pivot.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_entity_pivot_MouseDown);
			// 
			// button_entity_copy_to_marked
			// 
			this.button_entity_copy_to_marked.Location = new System.Drawing.Point(1, 531);
			this.button_entity_copy_to_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_copy_to_marked.Name = "button_entity_copy_to_marked";
			this.button_entity_copy_to_marked.Size = new System.Drawing.Size(145, 21);
			this.button_entity_copy_to_marked.TabIndex = 116;
			this.button_entity_copy_to_marked.Text = "Copy Properties to Marked";
			this.tool_tip.SetToolTip(this.button_entity_copy_to_marked, "Copy selected entity\'s properties (including subtype) to all marked entities of t" +
        "he same type");
			this.button_entity_copy_to_marked.UseVisualStyleBackColor = true;
			this.button_entity_copy_to_marked.Click += new System.EventHandler(this.button_entity_copy_to_marked_Click);
			// 
			// button_entity_duplicate
			// 
			this.button_entity_duplicate.Location = new System.Drawing.Point(1, 644);
			this.button_entity_duplicate.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_duplicate.Name = "button_entity_duplicate";
			this.button_entity_duplicate.Size = new System.Drawing.Size(145, 21);
			this.button_entity_duplicate.TabIndex = 100;
			this.button_entity_duplicate.Text = "Duplicate";
			this.tool_tip.SetToolTip(this.button_entity_duplicate, "Create a copy at the same location of all marked entities");
			this.button_entity_duplicate.UseVisualStyleBackColor = true;
			this.button_entity_duplicate.Click += new System.EventHandler(this.button_entity_duplicate_Click);
			// 
			// button_entity_move_side
			// 
			this.button_entity_move_side.Location = new System.Drawing.Point(1, 370);
			this.button_entity_move_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_move_side.Name = "button_entity_move_side";
			this.button_entity_move_side.Size = new System.Drawing.Size(145, 21);
			this.button_entity_move_side.TabIndex = 115;
			this.button_entity_move_side.Text = "Move to Side";
			this.tool_tip.SetToolTip(this.button_entity_move_side, "Move selected entity to selected side");
			this.button_entity_move_side.UseVisualStyleBackColor = true;
			this.button_entity_move_side.Click += new System.EventHandler(this.button_entity_move_side_Click);
			// 
			// button_entity_move_segment
			// 
			this.button_entity_move_segment.Location = new System.Drawing.Point(1, 347);
			this.button_entity_move_segment.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_move_segment.Name = "button_entity_move_segment";
			this.button_entity_move_segment.Size = new System.Drawing.Size(145, 21);
			this.button_entity_move_segment.TabIndex = 114;
			this.button_entity_move_segment.Text = "Move to Segment";
			this.tool_tip.SetToolTip(this.button_entity_move_segment, "Move selected entity to the selected segment");
			this.button_entity_move_segment.UseVisualStyleBackColor = true;
			this.button_entity_move_segment.Click += new System.EventHandler(this.button_entity_move_segment_Click);
			// 
			// label_entity_guid_value
			// 
			this.label_entity_guid_value.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label_entity_guid_value.Location = new System.Drawing.Point(69, 139);
			this.label_entity_guid_value.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_guid_value.Name = "label_entity_guid_value";
			this.label_entity_guid_value.Size = new System.Drawing.Size(77, 19);
			this.label_entity_guid_value.TabIndex = 124;
			this.label_entity_guid_value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_guid_value, "The GUID of the selected entity");
			// 
			// label_entity_type_value
			// 
			this.label_entity_type_value.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label_entity_type_value.Location = new System.Drawing.Point(51, 91);
			this.label_entity_type_value.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_type_value.Name = "label_entity_type_value";
			this.label_entity_type_value.Size = new System.Drawing.Size(95, 19);
			this.label_entity_type_value.TabIndex = 125;
			this.label_entity_type_value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_type_value, "The type of the selected entity");
			// 
			// button_entity_face_selected_side
			// 
			this.button_entity_face_selected_side.Location = new System.Drawing.Point(1, 439);
			this.button_entity_face_selected_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_face_selected_side.Name = "button_entity_face_selected_side";
			this.button_entity_face_selected_side.Size = new System.Drawing.Size(145, 21);
			this.button_entity_face_selected_side.TabIndex = 102;
			this.button_entity_face_selected_side.Text = "Face Selected Side";
			this.tool_tip.SetToolTip(this.button_entity_face_selected_side, "Make selected entity face the selected side");
			this.button_entity_face_selected_side.UseVisualStyleBackColor = true;
			this.button_entity_face_selected_side.Click += new System.EventHandler(this.button_entity_face_selected_side_Click);
			// 
			// button_entity_reset_rotation
			// 
			this.button_entity_reset_rotation.Location = new System.Drawing.Point(1, 416);
			this.button_entity_reset_rotation.Margin = new System.Windows.Forms.Padding(1);
			this.button_entity_reset_rotation.Name = "button_entity_reset_rotation";
			this.button_entity_reset_rotation.Size = new System.Drawing.Size(145, 21);
			this.button_entity_reset_rotation.TabIndex = 101;
			this.button_entity_reset_rotation.Text = "Reset Rotation";
			this.tool_tip.SetToolTip(this.button_entity_reset_rotation, "Set entity\'s rotation to default (facing world Z)");
			this.button_entity_reset_rotation.UseVisualStyleBackColor = true;
			this.button_entity_reset_rotation.Click += new System.EventHandler(this.button_entity_reset_rotation_Click);
			// 
			// label_entity_guid
			// 
			this.label_entity_guid.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label_entity_guid.Location = new System.Drawing.Point(1, 139);
			this.label_entity_guid.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_guid.Name = "label_entity_guid";
			this.label_entity_guid.Size = new System.Drawing.Size(66, 19);
			this.label_entity_guid.TabIndex = 127;
			this.label_entity_guid.Text = "Num: GUID";
			this.label_entity_guid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_guid, "Entity type");
			// 
			// label_entity_type
			// 
			this.label_entity_type.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label_entity_type.Location = new System.Drawing.Point(1, 91);
			this.label_entity_type.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_type.Name = "label_entity_type";
			this.label_entity_type.Size = new System.Drawing.Size(48, 19);
			this.label_entity_type.TabIndex = 128;
			this.label_entity_type.Text = "Type";
			this.label_entity_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_type, "Entity type");
			// 
			// comboBox_entity_subtype
			// 
			this.comboBox_entity_subtype.BackColor = System.Drawing.SystemColors.ControlDark;
			this.comboBox_entity_subtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_entity_subtype.FormattingEnabled = true;
			this.comboBox_entity_subtype.Location = new System.Drawing.Point(4, 113);
			this.comboBox_entity_subtype.Margin = new System.Windows.Forms.Padding(1);
			this.comboBox_entity_subtype.Name = "comboBox_entity_subtype";
			this.comboBox_entity_subtype.Size = new System.Drawing.Size(142, 21);
			this.comboBox_entity_subtype.TabIndex = 1;
			this.tool_tip.SetToolTip(this.comboBox_entity_subtype, "Change the subtype of the selected entity.");
			this.comboBox_entity_subtype.SelectedIndexChanged += new System.EventHandler(this.comboBox_entity_subtype_SelectedIndexChanged);
			// 
			// prop_grid_entity
			// 
			this.prop_grid_entity.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.prop_grid_entity.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.prop_grid_entity.HelpVisible = false;
			this.prop_grid_entity.LineColor = System.Drawing.SystemColors.ControlDark;
			this.prop_grid_entity.Location = new System.Drawing.Point(1, 162);
			this.prop_grid_entity.Margin = new System.Windows.Forms.Padding(1);
			this.prop_grid_entity.Name = "prop_grid_entity";
			this.prop_grid_entity.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.prop_grid_entity.Size = new System.Drawing.Size(145, 162);
			this.prop_grid_entity.TabIndex = 2;
			this.prop_grid_entity.ToolbarVisible = false;
			this.tool_tip.SetToolTip(this.prop_grid_entity, "Properties of the selected entity");
			this.prop_grid_entity.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.prop_grid_entity_PropertyValueChanged);
			// 
			// label_entity_sel_type
			// 
			this.label_entity_sel_type.BackColor = System.Drawing.Color.Gold;
			this.label_entity_sel_type.Location = new System.Drawing.Point(0, 0);
			this.label_entity_sel_type.Margin = new System.Windows.Forms.Padding(1);
			this.label_entity_sel_type.Name = "label_entity_sel_type";
			this.label_entity_sel_type.Size = new System.Drawing.Size(147, 23);
			this.label_entity_sel_type.TabIndex = 94;
			this.label_entity_sel_type.Text = "Type";
			this.label_entity_sel_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_entity_sel_type, "Entity type");
			// 
			// button_place_entity_side
			// 
			this.button_place_entity_side.BackColor = System.Drawing.SystemColors.ControlLight;
			this.button_place_entity_side.Location = new System.Drawing.Point(1, 47);
			this.button_place_entity_side.Margin = new System.Windows.Forms.Padding(1);
			this.button_place_entity_side.Name = "button_place_entity_side";
			this.button_place_entity_side.Size = new System.Drawing.Size(145, 21);
			this.button_place_entity_side.TabIndex = 95;
			this.button_place_entity_side.Text = "Place on Segment Side";
			this.tool_tip.SetToolTip(this.button_place_entity_side, "Place entity on the current side, facing the side\'s normal. New entity will be ma" +
        "rked.");
			this.button_place_entity_side.UseVisualStyleBackColor = false;
			this.button_place_entity_side.Click += new System.EventHandler(this.button_place_entity_side_Click);
			// 
			// button_place_entity_segment
			// 
			this.button_place_entity_segment.BackColor = System.Drawing.SystemColors.ControlLight;
			this.button_place_entity_segment.Location = new System.Drawing.Point(1, 24);
			this.button_place_entity_segment.Margin = new System.Windows.Forms.Padding(1);
			this.button_place_entity_segment.Name = "button_place_entity_segment";
			this.button_place_entity_segment.Size = new System.Drawing.Size(145, 21);
			this.button_place_entity_segment.TabIndex = 94;
			this.button_place_entity_segment.Text = "Place at Segment Center";
			this.tool_tip.SetToolTip(this.button_place_entity_segment, "Place an entity in the selected segment facing the selected side.  New entity wil" +
        "l be marked.");
			this.button_place_entity_segment.UseVisualStyleBackColor = false;
			this.button_place_entity_segment.Click += new System.EventHandler(this.button_place_entity_segment_Click);
			// 
			// comboBox_entity_type
			// 
			this.comboBox_entity_type.BackColor = System.Drawing.SystemColors.ControlDark;
			this.comboBox_entity_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_entity_type.ForeColor = System.Drawing.SystemColors.WindowText;
			this.comboBox_entity_type.FormattingEnabled = true;
			this.comboBox_entity_type.Location = new System.Drawing.Point(51, 1);
			this.comboBox_entity_type.Margin = new System.Windows.Forms.Padding(1);
			this.comboBox_entity_type.Name = "comboBox_entity_type";
			this.comboBox_entity_type.Size = new System.Drawing.Size(95, 21);
			this.comboBox_entity_type.TabIndex = 122;
			this.tool_tip.SetToolTip(this.comboBox_entity_type, "Select type of entity to place");
			this.comboBox_entity_type.SelectedIndexChanged += new System.EventHandler(this.comboBox_entity_type_SelectedIndexChanged);
			// 
			// label_selected
			// 
			this.label_selected.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label_selected.Location = new System.Drawing.Point(1, 70);
			this.label_selected.Margin = new System.Windows.Forms.Padding(1);
			this.label_selected.Name = "label_selected";
			this.label_selected.Size = new System.Drawing.Size(145, 19);
			this.label_selected.TabIndex = 129;
			this.label_selected.Text = "--- SELECTED ENTITY ---";
			this.label_selected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_selected, "Entity type");
			// 
			// label_marked
			// 
			this.label_marked.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label_marked.Location = new System.Drawing.Point(1, 577);
			this.label_marked.Margin = new System.Windows.Forms.Padding(1);
			this.label_marked.Name = "label_marked";
			this.label_marked.Size = new System.Drawing.Size(145, 19);
			this.label_marked.TabIndex = 130;
			this.label_marked.Text = "--- MARKED ENTITIES ---";
			this.label_marked.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_marked, "Entity type");
			// 
			// label_team
			// 
			this.label_team.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_team.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_team.Location = new System.Drawing.Point(1, 326);
			this.label_team.Margin = new System.Windows.Forms.Padding(1);
			this.label_team.Name = "label_team";
			this.label_team.Size = new System.Drawing.Size(145, 19);
			this.label_team.TabIndex = 131;
			this.label_team.Text = "MP Team: -";
			this.label_team.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_team, "Multiplayer team (player starts in MP levels only)");
			this.label_team.Click += new System.EventHandler(this.label_team_Click);
			// 
			// EditorEntitiesEditPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(147, 762);
			this.Controls.Add(this.label_team);
			this.Controls.Add(this.button_entity_duplicate);
			this.Controls.Add(this.label_marked);
			this.Controls.Add(this.button_entity_marked_face_selected_side);
			this.Controls.Add(this.button_entity_face_selected_side);
			this.Controls.Add(this.label_entity_type);
			this.Controls.Add(this.button_entity_marked_reset_rotation);
			this.Controls.Add(this.button_entity_reset_rotation);
			this.Controls.Add(this.label_entity_guid);
			this.Controls.Add(this.button_entity_align_side);
			this.Controls.Add(this.label_selected);
			this.Controls.Add(this.button_entity_move);
			this.Controls.Add(this.comboBox_entity_type);
			this.Controls.Add(this.button_entity_move_segment);
			this.Controls.Add(this.button_entity_move_side);
			this.Controls.Add(this.button_entity_copy_subtype);
			this.Controls.Add(this.button_place_entity_segment);
			this.Controls.Add(this.button_entity_copy_to_marked);
			this.Controls.Add(this.button_entity_mark_type);
			this.Controls.Add(this.label_entity_type_value);
			this.Controls.Add(this.button_entity_mark_subtype);
			this.Controls.Add(this.button_place_entity_side);
			this.Controls.Add(this.label_entity_guid_value);
			this.Controls.Add(this.comboBox_entity_subtype);
			this.Controls.Add(this.label_entity_sel_type);
			this.Controls.Add(this.label_entity_pivot);
			this.Controls.Add(this.prop_grid_entity);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.Name = "EditorEntitiesEditPane";
			this.Text = "ENTITIES";
			this.Load += new System.EventHandler(this.EditorEntitiesEditPane_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip tool_tip;
		private System.Windows.Forms.ComboBox comboBox_entity_subtype;
		private System.Windows.Forms.Button button_entity_align_side;
		private System.Windows.Forms.Button button_entity_move;
		private System.Windows.Forms.Button button_entity_copy_subtype;
		private System.Windows.Forms.Button button_entity_mark_subtype;
		private System.Windows.Forms.Button button_entity_mark_type;
		private System.Windows.Forms.Button button_entity_copy_to_marked;
		private System.Windows.Forms.Button button_entity_move_side;
		private System.Windows.Forms.Button button_entity_move_segment;
		private System.Windows.Forms.PropertyGrid prop_grid_entity;
		private System.Windows.Forms.Button button_entity_duplicate;
		private System.Windows.Forms.Label label_entity_pivot;
		private System.Windows.Forms.Button button_entity_marked_face_selected_side;
		private System.Windows.Forms.Button button_entity_marked_reset_rotation;
		private System.Windows.Forms.Label label_entity_guid_value;
        private System.Windows.Forms.Label label_entity_type_value;
        private System.Windows.Forms.Button button_entity_face_selected_side;
        private System.Windows.Forms.Button button_entity_reset_rotation;
        private System.Windows.Forms.Label label_entity_type;
        private System.Windows.Forms.Label label_entity_guid;
		private System.Windows.Forms.Label label_entity_sel_type;
		private System.Windows.Forms.Button button_place_entity_side;
		private System.Windows.Forms.Button button_place_entity_segment;
		public System.Windows.Forms.ComboBox comboBox_entity_type;
		private System.Windows.Forms.Label label_selected;
		private System.Windows.Forms.Label label_marked;
		private System.Windows.Forms.Label label_team;
	}
}