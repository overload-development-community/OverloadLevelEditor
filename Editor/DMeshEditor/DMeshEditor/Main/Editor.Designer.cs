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
			this.menu_strip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.recent1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recent2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recent3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recent4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.checkPolygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.forceLowResTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showShortcutsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.filePathLocationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gl_panel = new System.Windows.Forms.Panel();
			this.label_editmode = new System.Windows.Forms.Label();
			this.label_grid_display = new System.Windows.Forms.Label();
			this.panel_grid = new System.Windows.Forms.Panel();
			this.slider_grid_width = new OverloadLevelEditor.SliderLabel();
			this.slider_grid_snap = new OverloadLevelEditor.SliderLabel();
			this.button_snap_marked = new System.Windows.Forms.Button();
			this.slider_grid_spacing = new OverloadLevelEditor.SliderLabel();
			this.label_grid = new System.Windows.Forms.Label();
			this.slider_extrude_length = new OverloadLevelEditor.SliderLabel();
			this.button_merge_verts = new System.Windows.Forms.Button();
			this.slider_coplanar_angle = new OverloadLevelEditor.SliderLabel();
			this.button_mark_coplanar = new System.Windows.Forms.Button();
			this.tool_tip = new System.Windows.Forms.ToolTip(this.components);
			this.label_view_ortho = new System.Windows.Forms.Label();
			this.label_view_persp = new System.Windows.Forms.Label();
			this.label_view_layout = new System.Windows.Forms.Label();
			this.label_view_dark = new System.Windows.Forms.Label();
			this.label_count_total = new System.Windows.Forms.Label();
			this.label_count_marked = new System.Windows.Forms.Label();
			this.label_count_selected = new System.Windows.Forms.Label();
			this.button_texture_center_v = new System.Windows.Forms.Button();
			this.button_texture_center_u = new System.Windows.Forms.Button();
			this.button_texture_show_list = new System.Windows.Forms.Button();
			this.label_texture_name = new System.Windows.Forms.Label();
			this.button_texture_default_map = new System.Windows.Forms.Button();
			this.button_texture_snap4 = new System.Windows.Forms.Button();
			this.button_texture_snap8 = new System.Windows.Forms.Button();
			this.button_texture_planar_z = new System.Windows.Forms.Button();
			this.button_texture_planar_y = new System.Windows.Forms.Button();
			this.button_texture_planar_x = new System.Windows.Forms.Button();
			this.button_align_marked = new System.Windows.Forms.Button();
			this.button_texture_box_map = new System.Windows.Forms.Button();
			this.label_gimbal = new System.Windows.Forms.Label();
			this.label_lighting = new System.Windows.Forms.Label();
			this.label_pivotmode = new System.Windows.Forms.Label();
			this.label_scalemode = new System.Windows.Forms.Label();
			this.button_create_cyl_z = new System.Windows.Forms.Button();
			this.button_create_cyl_y = new System.Windows.Forms.Button();
			this.button_create_cyl_x = new System.Windows.Forms.Button();
			this.slider_sizex = new OverloadLevelEditor.SliderLabel();
			this.slider_sizey = new OverloadLevelEditor.SliderLabel();
			this.button_combine_two = new System.Windows.Forms.Button();
			this.button_triangulate_fan = new System.Windows.Forms.Button();
			this.button_triangulate_vert = new System.Windows.Forms.Button();
			this.button_flip_edge = new System.Windows.Forms.Button();
			this.button_uveditor = new System.Windows.Forms.Button();
			this.button_copy_marked = new System.Windows.Forms.Button();
			this.button_paste_polys = new System.Windows.Forms.Button();
			this.button_mark_dup_polys = new System.Windows.Forms.Button();
			this.button_mark_connected = new System.Windows.Forms.Button();
			this.button_browser = new System.Windows.Forms.Button();
			this.button_flip_u = new System.Windows.Forms.Button();
			this.button_edge_left = new System.Windows.Forms.Button();
			this.button_center = new System.Windows.Forms.Button();
			this.button_edge_right = new System.Windows.Forms.Button();
			this.button_rotate90 = new System.Windows.Forms.Button();
			this.button_flip_v = new System.Windows.Forms.Button();
			this.button_edge_bottom = new System.Windows.Forms.Button();
			this.button_edge_top = new System.Windows.Forms.Button();
			this.button_import_replace = new System.Windows.Forms.Button();
			this.button_import = new System.Windows.Forms.Button();
			this.button_planarize = new System.Windows.Forms.Button();
			this.button_create_quad = new System.Windows.Forms.Button();
			this.button_create_box = new System.Windows.Forms.Button();
			this.button_poly_flip = new System.Windows.Forms.Button();
			this.button_align_vert_x = new System.Windows.Forms.Button();
			this.button_align_vert_y = new System.Windows.Forms.Button();
			this.button_align_vert_z = new System.Windows.Forms.Button();
			this.slider_smooth_angle_diff = new OverloadLevelEditor.SliderLabel();
			this.slider_smooth_angle_same = new OverloadLevelEditor.SliderLabel();
			this.label_poly_filter = new System.Windows.Forms.Label();
			this.button_mark_untextured = new System.Windows.Forms.Button();
			this.slider_inset_length = new OverloadLevelEditor.SliderLabel();
			this.slider_inset_bevel = new OverloadLevelEditor.SliderLabel();
			this.button_extrude_selected = new System.Windows.Forms.Button();
			this.button_extrude_marked = new System.Windows.Forms.Button();
			this.button_inset_bevel = new System.Windows.Forms.Button();
			this.button_split_edge = new System.Windows.Forms.Button();
			this.button_duplicate_3way = new System.Windows.Forms.Button();
			this.button_duplicate_4way = new System.Windows.Forms.Button();
			this.button_duplicate_z = new System.Windows.Forms.Button();
			this.button_split_poly = new System.Windows.Forms.Button();
			this.button_duplicate_x = new System.Windows.Forms.Button();
			this.button_bevel_edge = new System.Windows.Forms.Button();
			this.button_combine_verts = new System.Windows.Forms.Button();
			this.button_uv_quarter1 = new System.Windows.Forms.Button();
			this.button_uv_quarter2 = new System.Windows.Forms.Button();
			this.button_uv_quarter4 = new System.Windows.Forms.Button();
			this.button_uv_button3 = new System.Windows.Forms.Button();
			this.button_subdivide_mesh = new System.Windows.Forms.Button();
			this.button_triangle_marked_nonplanar = new System.Windows.Forms.Button();
			this.button_triangulate_all_nonplanar = new System.Windows.Forms.Button();
			this.button_scale_dn1 = new System.Windows.Forms.Button();
			this.button_scale_up1 = new System.Windows.Forms.Button();
			this.button_scale_dn2 = new System.Windows.Forms.Button();
			this.button_scale_up2 = new System.Windows.Forms.Button();
			this.slider_bevel_width = new OverloadLevelEditor.SliderLabel();
			this.button_color4 = new System.Windows.Forms.Button();
			this.button_color3 = new System.Windows.Forms.Button();
			this.button_color2 = new System.Windows.Forms.Button();
			this.button_color1 = new System.Windows.Forms.Button();
			this.button_bisect_poly = new System.Windows.Forms.Button();
			this.slider_sizeheight = new OverloadLevelEditor.SliderLabel();
			this.slider_sizesegments = new OverloadLevelEditor.SliderLabel();
			this.label_vert_display = new System.Windows.Forms.Label();
			this.panel_view = new System.Windows.Forms.Panel();
			this.label_view = new System.Windows.Forms.Label();
			this.panel_texturing = new System.Windows.Forms.Panel();
			this.label_textures = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label_geometry = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.button_color_paste_all = new System.Windows.Forms.Button();
			this.button_color_copy_all = new System.Windows.Forms.Button();
			this.button_color_paste = new System.Windows.Forms.Button();
			this.button_color_copy = new System.Windows.Forms.Button();
			this.slider_color_brightness = new OverloadLevelEditor.SliderLabel();
			this.slider_color_saturation = new OverloadLevelEditor.SliderLabel();
			this.slider_color_hue = new OverloadLevelEditor.SliderLabel();
			this.slider_color_blue = new OverloadLevelEditor.SliderLabel();
			this.slider_color_green = new OverloadLevelEditor.SliderLabel();
			this.slider_color_red = new OverloadLevelEditor.SliderLabel();
			this.label6 = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.button_face_mark_same = new System.Windows.Forms.Button();
			this.button_face_clear = new System.Windows.Forms.Button();
			this.checklist_face = new System.Windows.Forms.CheckedListBox();
			this.button_face_copy = new System.Windows.Forms.Button();
			this.label_face = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.checklist_lights = new System.Windows.Forms.CheckedListBox();
			this.button_lights_paste = new System.Windows.Forms.Button();
			this.button_lights_copy = new System.Windows.Forms.Button();
			this.slider_light_rot_pitch = new OverloadLevelEditor.SliderLabel();
			this.slider_light_rot_yaw = new OverloadLevelEditor.SliderLabel();
			this.slider_light_posz = new OverloadLevelEditor.SliderLabel();
			this.slider_light_posy = new OverloadLevelEditor.SliderLabel();
			this.slider_light_posx = new OverloadLevelEditor.SliderLabel();
			this.label_light_flare = new System.Windows.Forms.Label();
			this.label_light_color = new System.Windows.Forms.Label();
			this.label_light_type = new System.Windows.Forms.Label();
			this.slider_light_angle = new OverloadLevelEditor.SliderLabel();
			this.slider_light_range = new OverloadLevelEditor.SliderLabel();
			this.slider_light_intensity = new OverloadLevelEditor.SliderLabel();
			this.button_light_reset_rot = new System.Windows.Forms.Button();
			this.button_light_reset_pos = new System.Windows.Forms.Button();
			this.label_lights = new System.Windows.Forms.Label();
			this.label_geometry2 = new System.Windows.Forms.Label();
			this.outputTextBox = new System.Windows.Forms.TextBox();
			this.menu_strip.SuspendLayout();
			this.panel_grid.SuspendLayout();
			this.panel_view.SuspendLayout();
			this.panel_texturing.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu_strip
			// 
			this.menu_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.editToolStripMenuItem,
			this.optionsToolStripMenuItem});
			this.menu_strip.Location = new System.Drawing.Point(0, 0);
			this.menu_strip.Name = "menu_strip";
			this.menu_strip.Size = new System.Drawing.Size(1466, 24);
			this.menu_strip.TabIndex = 6;
			this.menu_strip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.newToolStripMenuItem,
			this.loadToolStripMenuItem,
			this.saveToolStripMenuItem,
			this.saveasToolStripMenuItem,
			this.toolStripMenuItem5,
			this.exportToOBJToolStripMenuItem,
			this.toolStripMenuItem1,
			this.recent1ToolStripMenuItem,
			this.recent2ToolStripMenuItem,
			this.recent3ToolStripMenuItem,
			this.recent4ToolStripMenuItem,
			this.toolStripMenuItem2,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New Decal";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.loadToolStripMenuItem.Text = "Load...";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveasToolStripMenuItem
			// 
			this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
			this.saveasToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.saveasToolStripMenuItem.Text = "Save As...";
			this.saveasToolStripMenuItem.Click += new System.EventHandler(this.saveasToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(149, 6);
			// 
			// exportToOBJToolStripMenuItem
			// 
			this.exportToOBJToolStripMenuItem.Name = "exportToOBJToolStripMenuItem";
			this.exportToOBJToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exportToOBJToolStripMenuItem.Text = "Export To OBJ";
			this.exportToOBJToolStripMenuItem.Click += new System.EventHandler(this.exportToOBJToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
			// 
			// recent1ToolStripMenuItem
			// 
			this.recent1ToolStripMenuItem.Name = "recent1ToolStripMenuItem";
			this.recent1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.recent1ToolStripMenuItem.Text = "recent1";
			this.recent1ToolStripMenuItem.Click += new System.EventHandler(this.recent1ToolStripMenuItem_Click);
			// 
			// recent2ToolStripMenuItem
			// 
			this.recent2ToolStripMenuItem.Name = "recent2ToolStripMenuItem";
			this.recent2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.recent2ToolStripMenuItem.Text = "recent2";
			this.recent2ToolStripMenuItem.Click += new System.EventHandler(this.recent2ToolStripMenuItem_Click);
			// 
			// recent3ToolStripMenuItem
			// 
			this.recent3ToolStripMenuItem.Name = "recent3ToolStripMenuItem";
			this.recent3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.recent3ToolStripMenuItem.Text = "recent3";
			this.recent3ToolStripMenuItem.Click += new System.EventHandler(this.recent3ToolStripMenuItem_Click);
			// 
			// recent4ToolStripMenuItem
			// 
			this.recent4ToolStripMenuItem.Name = "recent4ToolStripMenuItem";
			this.recent4ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.recent4ToolStripMenuItem.Text = "recent4";
			this.recent4ToolStripMenuItem.Click += new System.EventHandler(this.recent4ToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.undoToolStripMenuItem,
			this.redoToolStripMenuItem,
			this.toolStripMenuItem3,
			this.toolStripMenuItem4,
			this.checkPolygonsToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Enabled = false;
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.Enabled = false;
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
			| System.Windows.Forms.Keys.Z)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.redoToolStripMenuItem.Text = "Redo";
			this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(171, 6);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(171, 6);
			// 
			// checkPolygonsToolStripMenuItem
			// 
			this.checkPolygonsToolStripMenuItem.Name = "checkPolygonsToolStripMenuItem";
			this.checkPolygonsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.checkPolygonsToolStripMenuItem.Text = "Check Polygons";
			this.checkPolygonsToolStripMenuItem.Click += new System.EventHandler(this.checkPolygonsToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem,
			this.toolStripMenuItem6,
			this.forceLowResTexturesToolStripMenuItem,
			this.showShortcutsToolStripMenuItem,
			this.filePathLocationsToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "Options";
			// 
			// autoCopyFaceFlagsToMarkedToolStripMenuItem
			// 
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.Checked = true;
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.Name = "autoCopyFaceFlagsToMarkedToolStripMenuItem";
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.Text = "Auto-Copy Poly Flags to Marked";
			this.autoCopyFaceFlagsToMarkedToolStripMenuItem.Click += new System.EventHandler(this.autoCopyFaceFlagsToMarkedToolStripMenuItem_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(243, 6);
			// 
			// forceLowResTexturesToolStripMenuItem
			// 
			this.forceLowResTexturesToolStripMenuItem.Name = "forceLowResTexturesToolStripMenuItem";
			this.forceLowResTexturesToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.forceLowResTexturesToolStripMenuItem.Text = "Force Low Res Textures";
			this.forceLowResTexturesToolStripMenuItem.Click += new System.EventHandler(this.forceLowResTexturesToolStripMenuItem_Click);
			// 
			// showShortcutsToolStripMenuItem
			// 
			this.showShortcutsToolStripMenuItem.Name = "showShortcutsToolStripMenuItem";
			this.showShortcutsToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.showShortcutsToolStripMenuItem.Text = "Show Shortcuts";
			this.showShortcutsToolStripMenuItem.Click += new System.EventHandler(this.showShortcutsToolStripMenuItem_Click);
			// 
			// filePathLocationsToolStripMenuItem
			// 
			this.filePathLocationsToolStripMenuItem.Name = "filePathLocationsToolStripMenuItem";
			this.filePathLocationsToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.filePathLocationsToolStripMenuItem.Text = "File Path Locations";
			this.filePathLocationsToolStripMenuItem.Click += new System.EventHandler(this.filePathLocationsToolStripMenuItem_Click);
			// 
			// gl_panel
			// 
			this.gl_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.gl_panel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.gl_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gl_panel.Location = new System.Drawing.Point(152, 27);
			this.gl_panel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.gl_panel.Name = "gl_panel";
			this.gl_panel.Size = new System.Drawing.Size(1005, 958);
			this.gl_panel.TabIndex = 7;
			this.gl_panel.Resize += new System.EventHandler(this.gl_panel_Resize);
			// 
			// label_editmode
			// 
			this.label_editmode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_editmode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_editmode.Location = new System.Drawing.Point(0, 28);
			this.label_editmode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_editmode.Name = "label_editmode";
			this.label_editmode.Size = new System.Drawing.Size(149, 37);
			this.label_editmode.TabIndex = 34;
			this.label_editmode.Text = "Mode: POLY";
			this.label_editmode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_editmode, "Editing mode for geometry - Tab");
			this.label_editmode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_editmode_MouseDown);
			// 
			// label_grid_display
			// 
			this.label_grid_display.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_grid_display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_grid_display.Location = new System.Drawing.Point(2, 76);
			this.label_grid_display.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_grid_display.Name = "label_grid_display";
			this.label_grid_display.Size = new System.Drawing.Size(143, 19);
			this.label_grid_display.TabIndex = 44;
			this.label_grid_display.Text = "Display: ALL";
			this.label_grid_display.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_grid_display, "Which views to show the grid in - Shift+G");
			this.label_grid_display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_grid_display_MouseDown);
			// 
			// panel_grid
			// 
			this.panel_grid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panel_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_grid.Controls.Add(this.slider_grid_width);
			this.panel_grid.Controls.Add(this.slider_grid_snap);
			this.panel_grid.Controls.Add(this.button_snap_marked);
			this.panel_grid.Controls.Add(this.slider_grid_spacing);
			this.panel_grid.Controls.Add(this.label_grid);
			this.panel_grid.Controls.Add(this.label_grid_display);
			this.panel_grid.Location = new System.Drawing.Point(0, 777);
			this.panel_grid.Margin = new System.Windows.Forms.Padding(1);
			this.panel_grid.Name = "panel_grid";
			this.panel_grid.Size = new System.Drawing.Size(149, 122);
			this.panel_grid.TabIndex = 38;
			// 
			// slider_grid_width
			// 
			this.slider_grid_width.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_grid_width.Location = new System.Drawing.Point(2, 57);
			this.slider_grid_width.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_grid_width.Name = "slider_grid_width";
			this.slider_grid_width.RightMouseMultiplier = 5;
			this.slider_grid_width.Size = new System.Drawing.Size(143, 19);
			this.slider_grid_width.SlideText = "Width";
			this.slider_grid_width.SlideTol = 20;
			this.slider_grid_width.TabIndex = 93;
			this.tool_tip.SetToolTip(this.slider_grid_width, "SEE BELOW");
			this.slider_grid_width.ToolTop = "Grid total width";
			this.slider_grid_width.ValueText = "2";
			this.slider_grid_width.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_width_Feedback);
			// 
			// slider_grid_snap
			// 
			this.slider_grid_snap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_grid_snap.Location = new System.Drawing.Point(2, 38);
			this.slider_grid_snap.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_grid_snap.Name = "slider_grid_snap";
			this.slider_grid_snap.RightMouseMultiplier = 5;
			this.slider_grid_snap.Size = new System.Drawing.Size(143, 19);
			this.slider_grid_snap.SlideText = "Snap";
			this.slider_grid_snap.SlideTol = 20;
			this.slider_grid_snap.TabIndex = 92;
			this.tool_tip.SetToolTip(this.slider_grid_snap, "SEE BELOW");
			this.slider_grid_snap.ToolTop = "Grid snap amount - [ or ]";
			this.slider_grid_snap.ValueText = "0.25";
			this.slider_grid_snap.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_snap_Feedback);
			// 
			// button_snap_marked
			// 
			this.button_snap_marked.Location = new System.Drawing.Point(3, 96);
			this.button_snap_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_snap_marked.Name = "button_snap_marked";
			this.button_snap_marked.Size = new System.Drawing.Size(143, 21);
			this.button_snap_marked.TabIndex = 48;
			this.button_snap_marked.Text = "Snap Marked To Grid";
			this.tool_tip.SetToolTip(this.button_snap_marked, "Snap all marked elements to the grid - Ctrl+G");
			this.button_snap_marked.UseVisualStyleBackColor = true;
			this.button_snap_marked.Click += new System.EventHandler(this.button_snap_marked_Click);
			// 
			// slider_grid_spacing
			// 
			this.slider_grid_spacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_grid_spacing.Location = new System.Drawing.Point(2, 19);
			this.slider_grid_spacing.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_grid_spacing.Name = "slider_grid_spacing";
			this.slider_grid_spacing.RightMouseMultiplier = 5;
			this.slider_grid_spacing.Size = new System.Drawing.Size(143, 19);
			this.slider_grid_spacing.SlideText = "Spacing";
			this.slider_grid_spacing.SlideTol = 20;
			this.slider_grid_spacing.TabIndex = 91;
			this.tool_tip.SetToolTip(this.slider_grid_spacing, "SEE BELOW");
			this.slider_grid_spacing.ToolTop = "Visible grid spacing - Shift+[ or Shift+]";
			this.slider_grid_spacing.ValueText = "0.5";
			this.slider_grid_spacing.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_spacing_Feedback);
			// 
			// label_grid
			// 
			this.label_grid.BackColor = System.Drawing.SystemColors.Control;
			this.label_grid.Location = new System.Drawing.Point(2, -1);
			this.label_grid.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_grid.Name = "label_grid";
			this.label_grid.Size = new System.Drawing.Size(143, 19);
			this.label_grid.TabIndex = 45;
			this.label_grid.Text = "GRID";
			this.label_grid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// slider_extrude_length
			// 
			this.slider_extrude_length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_extrude_length.Location = new System.Drawing.Point(1320, 522);
			this.slider_extrude_length.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_extrude_length.Name = "slider_extrude_length";
			this.slider_extrude_length.RightMouseMultiplier = 4;
			this.slider_extrude_length.Size = new System.Drawing.Size(143, 19);
			this.slider_extrude_length.SlideText = "Extrude Length";
			this.slider_extrude_length.SlideTol = 15;
			this.slider_extrude_length.TabIndex = 92;
			this.tool_tip.SetToolTip(this.slider_extrude_length, "SEE BELOW");
			this.slider_extrude_length.ToolTop = "";
			this.slider_extrude_length.ValueText = "0.25";
			this.slider_extrude_length.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_extrude_length_Feedback);
			// 
			// button_merge_verts
			// 
			this.button_merge_verts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_merge_verts.Location = new System.Drawing.Point(1321, 954);
			this.button_merge_verts.Margin = new System.Windows.Forms.Padding(1);
			this.button_merge_verts.Name = "button_merge_verts";
			this.button_merge_verts.Size = new System.Drawing.Size(143, 21);
			this.button_merge_verts.TabIndex = 91;
			this.button_merge_verts.Text = "Merge Overlapping Verts";
			this.tool_tip.SetToolTip(this.button_merge_verts, "Merge all verts that overlap and remove all extra verts");
			this.button_merge_verts.UseVisualStyleBackColor = true;
			this.button_merge_verts.Click += new System.EventHandler(this.button_merge_verts_Click);
			// 
			// slider_coplanar_angle
			// 
			this.slider_coplanar_angle.Location = new System.Drawing.Point(4, 708);
			this.slider_coplanar_angle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_coplanar_angle.Name = "slider_coplanar_angle";
			this.slider_coplanar_angle.RightMouseMultiplier = 5;
			this.slider_coplanar_angle.Size = new System.Drawing.Size(143, 19);
			this.slider_coplanar_angle.SlideText = "CoPlanar Angle";
			this.slider_coplanar_angle.SlideTol = 15;
			this.slider_coplanar_angle.TabIndex = 89;
			this.tool_tip.SetToolTip(this.slider_coplanar_angle, "SEE BELOW");
			this.slider_coplanar_angle.ToolTop = "CoPlanar poly tolerance";
			this.slider_coplanar_angle.ValueText = "15";
			this.slider_coplanar_angle.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_coplanar_angle_Feedback);
			// 
			// button_mark_coplanar
			// 
			this.button_mark_coplanar.Location = new System.Drawing.Point(4, 686);
			this.button_mark_coplanar.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_coplanar.Name = "button_mark_coplanar";
			this.button_mark_coplanar.Size = new System.Drawing.Size(143, 21);
			this.button_mark_coplanar.TabIndex = 58;
			this.button_mark_coplanar.Text = "Mark CoPlanar";
			this.tool_tip.SetToolTip(this.button_mark_coplanar, "Mark all polys co-planar with the selected poly - [Shift + Q]");
			this.button_mark_coplanar.UseVisualStyleBackColor = true;
			this.button_mark_coplanar.Click += new System.EventHandler(this.button_mark_coplanar_Click);
			// 
			// label_view_ortho
			// 
			this.label_view_ortho.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_ortho.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_ortho.Location = new System.Drawing.Point(2, 19);
			this.label_view_ortho.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view_ortho.Name = "label_view_ortho";
			this.label_view_ortho.Size = new System.Drawing.Size(143, 19);
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
			this.label_view_persp.Location = new System.Drawing.Point(2, 38);
			this.label_view_persp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view_persp.Name = "label_view_persp";
			this.label_view_persp.Size = new System.Drawing.Size(143, 19);
			this.label_view_persp.TabIndex = 45;
			this.label_view_persp.Text = "Persp: WIRE_TEXTURE";
			this.label_view_persp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_persp, "Display type for the perspective view");
			this.label_view_persp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_persp_MouseDown);
			// 
			// label_view_layout
			// 
			this.label_view_layout.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_layout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_layout.Location = new System.Drawing.Point(2, 57);
			this.label_view_layout.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view_layout.Name = "label_view_layout";
			this.label_view_layout.Size = new System.Drawing.Size(143, 19);
			this.label_view_layout.TabIndex = 48;
			this.label_view_layout.Text = "Layout: FOURWAY";
			this.label_view_layout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_layout, "Arrangement type for the viewports");
			this.label_view_layout.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_layout_MouseDown);
			// 
			// label_view_dark
			// 
			this.label_view_dark.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_dark.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_dark.Location = new System.Drawing.Point(2, 76);
			this.label_view_dark.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view_dark.Name = "label_view_dark";
			this.label_view_dark.Size = new System.Drawing.Size(143, 19);
			this.label_view_dark.TabIndex = 49;
			this.label_view_dark.Text = "Background: LIGHT";
			this.label_view_dark.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_view_dark, "Background color in viewports");
			this.label_view_dark.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_dark_MouseDown);
			// 
			// label_count_total
			// 
			this.label_count_total.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_count_total.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_total.Location = new System.Drawing.Point(152, 988);
			this.label_count_total.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_count_total.Name = "label_count_total";
			this.label_count_total.Size = new System.Drawing.Size(149, 19);
			this.label_count_total.TabIndex = 50;
			this.label_count_total.Text = "Total: 0/0";
			this.label_count_total.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_total, "Total polys/verts in the level");
			// 
			// label_count_marked
			// 
			this.label_count_marked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_count_marked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_marked.Location = new System.Drawing.Point(152, 1007);
			this.label_count_marked.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_count_marked.Name = "label_count_marked";
			this.label_count_marked.Size = new System.Drawing.Size(149, 19);
			this.label_count_marked.TabIndex = 51;
			this.label_count_marked.Text = "Marked: 0/0";
			this.label_count_marked.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_marked, "Marked polys/verts");
			// 
			// label_count_selected
			// 
			this.label_count_selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_count_selected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_count_selected.Location = new System.Drawing.Point(152, 1026);
			this.label_count_selected.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_count_selected.Name = "label_count_selected";
			this.label_count_selected.Size = new System.Drawing.Size(149, 19);
			this.label_count_selected.TabIndex = 52;
			this.label_count_selected.Text = "Selected: --/--";
			this.label_count_selected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_count_selected, "Index of the selected poly/vertex");
			// 
			// button_texture_center_v
			// 
			this.button_texture_center_v.Location = new System.Drawing.Point(75, 154);
			this.button_texture_center_v.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_center_v.Name = "button_texture_center_v";
			this.button_texture_center_v.Size = new System.Drawing.Size(70, 21);
			this.button_texture_center_v.TabIndex = 89;
			this.button_texture_center_v.Text = "Center V";
			this.tool_tip.SetToolTip(this.button_texture_center_v, "Center the marked polys\' UV in the V direction");
			this.button_texture_center_v.UseVisualStyleBackColor = true;
			this.button_texture_center_v.Click += new System.EventHandler(this.button_texture_center_v_Click);
			// 
			// button_texture_center_u
			// 
			this.button_texture_center_u.Location = new System.Drawing.Point(2, 154);
			this.button_texture_center_u.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_center_u.Name = "button_texture_center_u";
			this.button_texture_center_u.Size = new System.Drawing.Size(70, 21);
			this.button_texture_center_u.TabIndex = 88;
			this.button_texture_center_u.Text = "Center U";
			this.tool_tip.SetToolTip(this.button_texture_center_u, "Center the marked polys\' UV in the U direction");
			this.button_texture_center_u.UseVisualStyleBackColor = true;
			this.button_texture_center_u.Click += new System.EventHandler(this.button_texture_center_u_Click);
			// 
			// button_texture_show_list
			// 
			this.button_texture_show_list.Location = new System.Drawing.Point(2, 39);
			this.button_texture_show_list.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_show_list.Name = "button_texture_show_list";
			this.button_texture_show_list.Size = new System.Drawing.Size(143, 21);
			this.button_texture_show_list.TabIndex = 86;
			this.button_texture_show_list.Text = "Show Texture List";
			this.tool_tip.SetToolTip(this.button_texture_show_list, "Show the list of textures [F1]");
			this.button_texture_show_list.UseVisualStyleBackColor = true;
			this.button_texture_show_list.Click += new System.EventHandler(this.button_texture_show_list_Click);
			// 
			// label_texture_name
			// 
			this.label_texture_name.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_texture_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_texture_name.Location = new System.Drawing.Point(2, 19);
			this.label_texture_name.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_texture_name.Name = "label_texture_name";
			this.label_texture_name.Size = new System.Drawing.Size(143, 19);
			this.label_texture_name.TabIndex = 85;
			this.label_texture_name.Text = "-";
			this.label_texture_name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tool_tip.SetToolTip(this.label_texture_name, "Texture name of the selected poly");
			// 
			// button_texture_default_map
			// 
			this.button_texture_default_map.Location = new System.Drawing.Point(2, 108);
			this.button_texture_default_map.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_default_map.Name = "button_texture_default_map";
			this.button_texture_default_map.Size = new System.Drawing.Size(143, 21);
			this.button_texture_default_map.TabIndex = 67;
			this.button_texture_default_map.Text = "Default Map";
			this.tool_tip.SetToolTip(this.button_texture_default_map, "Reset the marked polys to the default mapping - Ctrl+T");
			this.button_texture_default_map.UseVisualStyleBackColor = true;
			this.button_texture_default_map.Click += new System.EventHandler(this.button_texture_default_map_Click);
			// 
			// button_texture_snap4
			// 
			this.button_texture_snap4.Location = new System.Drawing.Point(75, 177);
			this.button_texture_snap4.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_snap4.Name = "button_texture_snap4";
			this.button_texture_snap4.Size = new System.Drawing.Size(70, 21);
			this.button_texture_snap4.TabIndex = 66;
			this.button_texture_snap4.Text = "1/32 Snap";
			this.tool_tip.SetToolTip(this.button_texture_snap4, "Snap the marked polys\' textures to the closest 1/32nd increment");
			this.button_texture_snap4.UseVisualStyleBackColor = true;
			this.button_texture_snap4.Click += new System.EventHandler(this.button_texture_snap4_Click);
			// 
			// button_texture_snap8
			// 
			this.button_texture_snap8.Location = new System.Drawing.Point(2, 177);
			this.button_texture_snap8.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_snap8.Name = "button_texture_snap8";
			this.button_texture_snap8.Size = new System.Drawing.Size(70, 21);
			this.button_texture_snap8.TabIndex = 65;
			this.button_texture_snap8.Text = "1/8 Snap";
			this.tool_tip.SetToolTip(this.button_texture_snap8, "Snap the marked polys\' textures to the closest 1/8th increment");
			this.button_texture_snap8.UseVisualStyleBackColor = true;
			this.button_texture_snap8.Click += new System.EventHandler(this.button_texture_snap8_Click);
			// 
			// button_texture_planar_z
			// 
			this.button_texture_planar_z.Location = new System.Drawing.Point(99, 62);
			this.button_texture_planar_z.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_z.Name = "button_texture_planar_z";
			this.button_texture_planar_z.Size = new System.Drawing.Size(46, 21);
			this.button_texture_planar_z.TabIndex = 64;
			this.button_texture_planar_z.Text = "Map Z";
			this.tool_tip.SetToolTip(this.button_texture_planar_z, "Planar map the marked polys");
			this.button_texture_planar_z.UseVisualStyleBackColor = true;
			this.button_texture_planar_z.Click += new System.EventHandler(this.button_texture_planar_z_Click);
			// 
			// button_texture_planar_y
			// 
			this.button_texture_planar_y.Location = new System.Drawing.Point(50, 62);
			this.button_texture_planar_y.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_y.Name = "button_texture_planar_y";
			this.button_texture_planar_y.Size = new System.Drawing.Size(47, 21);
			this.button_texture_planar_y.TabIndex = 63;
			this.button_texture_planar_y.Text = "Map Y";
			this.tool_tip.SetToolTip(this.button_texture_planar_y, "Planar map the marked polys");
			this.button_texture_planar_y.UseVisualStyleBackColor = true;
			this.button_texture_planar_y.Click += new System.EventHandler(this.button_texture_planar_y_Click);
			// 
			// button_texture_planar_x
			// 
			this.button_texture_planar_x.Location = new System.Drawing.Point(2, 62);
			this.button_texture_planar_x.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_planar_x.Name = "button_texture_planar_x";
			this.button_texture_planar_x.Size = new System.Drawing.Size(46, 21);
			this.button_texture_planar_x.TabIndex = 62;
			this.button_texture_planar_x.Text = "Map X";
			this.tool_tip.SetToolTip(this.button_texture_planar_x, "Planar map the marked polys");
			this.button_texture_planar_x.UseVisualStyleBackColor = true;
			this.button_texture_planar_x.Click += new System.EventHandler(this.button_texture_planar_x_Click);
			// 
			// button_align_marked
			// 
			this.button_align_marked.Location = new System.Drawing.Point(2, 131);
			this.button_align_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_align_marked.Name = "button_align_marked";
			this.button_align_marked.Size = new System.Drawing.Size(143, 21);
			this.button_align_marked.TabIndex = 61;
			this.button_align_marked.Text = "Align To Selected";
			this.tool_tip.SetToolTip(this.button_align_marked, "Aligned the marked polys\' UVs to line up with the selected poly - Shift+T");
			this.button_align_marked.UseVisualStyleBackColor = true;
			this.button_align_marked.Click += new System.EventHandler(this.button_align_marked_Click);
			// 
			// button_texture_box_map
			// 
			this.button_texture_box_map.Location = new System.Drawing.Point(2, 85);
			this.button_texture_box_map.Margin = new System.Windows.Forms.Padding(1);
			this.button_texture_box_map.Name = "button_texture_box_map";
			this.button_texture_box_map.Size = new System.Drawing.Size(143, 21);
			this.button_texture_box_map.TabIndex = 60;
			this.button_texture_box_map.Text = "Box Map";
			this.tool_tip.SetToolTip(this.button_texture_box_map, "Box map the marked polys");
			this.button_texture_box_map.UseVisualStyleBackColor = true;
			this.button_texture_box_map.Click += new System.EventHandler(this.button_texture_box_map_Click);
			// 
			// label_gimbal
			// 
			this.label_gimbal.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_gimbal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_gimbal.Location = new System.Drawing.Point(2, 95);
			this.label_gimbal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_gimbal.Name = "label_gimbal";
			this.label_gimbal.Size = new System.Drawing.Size(143, 19);
			this.label_gimbal.TabIndex = 50;
			this.label_gimbal.Text = "Gimbal: HIDE";
			this.label_gimbal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_gimbal, "How to render the gimbal");
			this.label_gimbal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_gimbal_MouseDown);
			// 
			// label_lighting
			// 
			this.label_lighting.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_lighting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_lighting.Location = new System.Drawing.Point(2, 114);
			this.label_lighting.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_lighting.Name = "label_lighting";
			this.label_lighting.Size = new System.Drawing.Size(143, 19);
			this.label_lighting.TabIndex = 51;
			this.label_lighting.Text = "Lighting: NONE";
			this.label_lighting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_lighting, "Amount of lighting in viewports (if solid)");
			this.label_lighting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_lighting_MouseDown);
			// 
			// label_pivotmode
			// 
			this.label_pivotmode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_pivotmode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_pivotmode.Location = new System.Drawing.Point(0, 84);
			this.label_pivotmode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_pivotmode.Name = "label_pivotmode";
			this.label_pivotmode.Size = new System.Drawing.Size(149, 19);
			this.label_pivotmode.TabIndex = 99;
			this.label_pivotmode.Text = "Pivot: ORIGIN";
			this.label_pivotmode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_pivotmode, "Pivot for rotation and scaling");
			this.label_pivotmode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_pivot_MouseDown);
			// 
			// label_scalemode
			// 
			this.label_scalemode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_scalemode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_scalemode.Location = new System.Drawing.Point(0, 65);
			this.label_scalemode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_scalemode.Name = "label_scalemode";
			this.label_scalemode.Size = new System.Drawing.Size(149, 19);
			this.label_scalemode.TabIndex = 100;
			this.label_scalemode.Text = "Scale: VIEW_XY";
			this.label_scalemode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_scalemode, "Relative direction for scaling geometry");
			this.label_scalemode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_scalemode_MouseDown);
			// 
			// button_create_cyl_z
			// 
			this.button_create_cyl_z.Location = new System.Drawing.Point(100, 405);
			this.button_create_cyl_z.Margin = new System.Windows.Forms.Padding(1);
			this.button_create_cyl_z.Name = "button_create_cyl_z";
			this.button_create_cyl_z.Size = new System.Drawing.Size(46, 21);
			this.button_create_cyl_z.TabIndex = 104;
			this.button_create_cyl_z.Text = "Cyl Z";
			this.tool_tip.SetToolTip(this.button_create_cyl_z, "Flip Create a default cylinder with the given height/radius (Z)");
			this.button_create_cyl_z.UseVisualStyleBackColor = true;
			this.button_create_cyl_z.Click += new System.EventHandler(this.button_create_cyl_z_Click);
			// 
			// button_create_cyl_y
			// 
			this.button_create_cyl_y.Location = new System.Drawing.Point(51, 405);
			this.button_create_cyl_y.Margin = new System.Windows.Forms.Padding(1);
			this.button_create_cyl_y.Name = "button_create_cyl_y";
			this.button_create_cyl_y.Size = new System.Drawing.Size(47, 21);
			this.button_create_cyl_y.TabIndex = 103;
			this.button_create_cyl_y.Text = "Cyl Y";
			this.tool_tip.SetToolTip(this.button_create_cyl_y, "Create a default cylinder with the given height/radius (Y)");
			this.button_create_cyl_y.UseVisualStyleBackColor = true;
			this.button_create_cyl_y.Click += new System.EventHandler(this.button_create_cyl_y_Click);
			// 
			// button_create_cyl_x
			// 
			this.button_create_cyl_x.Location = new System.Drawing.Point(3, 405);
			this.button_create_cyl_x.Margin = new System.Windows.Forms.Padding(1);
			this.button_create_cyl_x.Name = "button_create_cyl_x";
			this.button_create_cyl_x.Size = new System.Drawing.Size(46, 21);
			this.button_create_cyl_x.TabIndex = 102;
			this.button_create_cyl_x.Text = "Cyl X";
			this.tool_tip.SetToolTip(this.button_create_cyl_x, "Create a default cylinder with the given height/radius (X)");
			this.button_create_cyl_x.UseVisualStyleBackColor = true;
			this.button_create_cyl_x.Click += new System.EventHandler(this.button_create_cyl_x_Click);
			// 
			// slider_sizex
			// 
			this.slider_sizex.Location = new System.Drawing.Point(4, 427);
			this.slider_sizex.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_sizex.Name = "slider_sizex";
			this.slider_sizex.RightMouseMultiplier = 4;
			this.slider_sizex.Size = new System.Drawing.Size(143, 19);
			this.slider_sizex.SlideText = "Size X/Radius";
			this.slider_sizex.SlideTol = 15;
			this.slider_sizex.TabIndex = 105;
			this.tool_tip.SetToolTip(this.slider_sizex, "SEE BELOW");
			this.slider_sizex.ToolTop = "Size of the default objects (X)";
			this.slider_sizex.ValueText = "1.0";
			this.slider_sizex.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_sizex_Feedback);
			// 
			// slider_sizey
			// 
			this.slider_sizey.Location = new System.Drawing.Point(4, 446);
			this.slider_sizey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_sizey.Name = "slider_sizey";
			this.slider_sizey.RightMouseMultiplier = 4;
			this.slider_sizey.Size = new System.Drawing.Size(143, 19);
			this.slider_sizey.SlideText = "Size Y";
			this.slider_sizey.SlideTol = 15;
			this.slider_sizey.TabIndex = 106;
			this.tool_tip.SetToolTip(this.slider_sizey, "SEE BELOW");
			this.slider_sizey.ToolTop = "Size of the default objects (Y)";
			this.slider_sizey.ValueText = "1.0";
			this.slider_sizey.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_sizey_Feedback);
			// 
			// button_combine_two
			// 
			this.button_combine_two.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_combine_two.Location = new System.Drawing.Point(1321, 908);
			this.button_combine_two.Margin = new System.Windows.Forms.Padding(1);
			this.button_combine_two.Name = "button_combine_two";
			this.button_combine_two.Size = new System.Drawing.Size(143, 21);
			this.button_combine_two.TabIndex = 110;
			this.button_combine_two.Text = "Combine Two Polys";
			this.tool_tip.SetToolTip(this.button_combine_two, "Combine two polygons into one");
			this.button_combine_two.UseVisualStyleBackColor = true;
			this.button_combine_two.Click += new System.EventHandler(this.button_combine_two_Click);
			// 
			// button_triangulate_fan
			// 
			this.button_triangulate_fan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_triangulate_fan.Location = new System.Drawing.Point(1321, 885);
			this.button_triangulate_fan.Margin = new System.Windows.Forms.Padding(1);
			this.button_triangulate_fan.Name = "button_triangulate_fan";
			this.button_triangulate_fan.Size = new System.Drawing.Size(143, 21);
			this.button_triangulate_fan.TabIndex = 111;
			this.button_triangulate_fan.Text = "Triangulate - Fan";
			this.tool_tip.SetToolTip(this.button_triangulate_fan, "Triangulate the marked polygons at their center points");
			this.button_triangulate_fan.UseVisualStyleBackColor = true;
			this.button_triangulate_fan.Click += new System.EventHandler(this.button_triangulate_fan_Click);
			// 
			// button_triangulate_vert
			// 
			this.button_triangulate_vert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_triangulate_vert.Location = new System.Drawing.Point(1321, 862);
			this.button_triangulate_vert.Margin = new System.Windows.Forms.Padding(1);
			this.button_triangulate_vert.Name = "button_triangulate_vert";
			this.button_triangulate_vert.Size = new System.Drawing.Size(143, 21);
			this.button_triangulate_vert.TabIndex = 112;
			this.button_triangulate_vert.Text = "Triangulate - Vert";
			this.tool_tip.SetToolTip(this.button_triangulate_vert, "Triangulate the marked polygons at the selected vert");
			this.button_triangulate_vert.UseVisualStyleBackColor = true;
			this.button_triangulate_vert.Click += new System.EventHandler(this.button_triangulate_vert_Click);
			// 
			// button_flip_edge
			// 
			this.button_flip_edge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_flip_edge.Location = new System.Drawing.Point(1321, 784);
			this.button_flip_edge.Margin = new System.Windows.Forms.Padding(1);
			this.button_flip_edge.Name = "button_flip_edge";
			this.button_flip_edge.Size = new System.Drawing.Size(143, 21);
			this.button_flip_edge.TabIndex = 113;
			this.button_flip_edge.Text = "Switch Triangles Edge";
			this.tool_tip.SetToolTip(this.button_flip_edge, "Switch the triangle edges for two adjacent triangles");
			this.button_flip_edge.UseVisualStyleBackColor = true;
			this.button_flip_edge.Click += new System.EventHandler(this.button_flip_edge_Click);
			// 
			// button_uveditor
			// 
			this.button_uveditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_uveditor.Location = new System.Drawing.Point(1320, 328);
			this.button_uveditor.Margin = new System.Windows.Forms.Padding(1);
			this.button_uveditor.Name = "button_uveditor";
			this.button_uveditor.Size = new System.Drawing.Size(143, 21);
			this.button_uveditor.TabIndex = 114;
			this.button_uveditor.Text = "Show UV Editor";
			this.tool_tip.SetToolTip(this.button_uveditor, "Show the UV Editor popup");
			this.button_uveditor.UseVisualStyleBackColor = true;
			this.button_uveditor.Click += new System.EventHandler(this.button_uveditor_Click);
			// 
			// button_copy_marked
			// 
			this.button_copy_marked.Location = new System.Drawing.Point(4, 525);
			this.button_copy_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_copy_marked.Name = "button_copy_marked";
			this.button_copy_marked.Size = new System.Drawing.Size(143, 21);
			this.button_copy_marked.TabIndex = 115;
			this.button_copy_marked.Text = "Copy Marked Polys";
			this.tool_tip.SetToolTip(this.button_copy_marked, "Copy the marked polys to the buffer [Ctrl + C]");
			this.button_copy_marked.UseVisualStyleBackColor = true;
			this.button_copy_marked.Click += new System.EventHandler(this.button_copy_marked_Click);
			// 
			// button_paste_polys
			// 
			this.button_paste_polys.Location = new System.Drawing.Point(4, 548);
			this.button_paste_polys.Margin = new System.Windows.Forms.Padding(1);
			this.button_paste_polys.Name = "button_paste_polys";
			this.button_paste_polys.Size = new System.Drawing.Size(143, 21);
			this.button_paste_polys.TabIndex = 116;
			this.button_paste_polys.Text = "Paste Polys";
			this.tool_tip.SetToolTip(this.button_paste_polys, "Paste the buffer polygons [Ctrl + V]");
			this.button_paste_polys.UseVisualStyleBackColor = true;
			this.button_paste_polys.Click += new System.EventHandler(this.button_paste_polys_Click);
			// 
			// button_mark_dup_polys
			// 
			this.button_mark_dup_polys.Location = new System.Drawing.Point(4, 640);
			this.button_mark_dup_polys.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_dup_polys.Name = "button_mark_dup_polys";
			this.button_mark_dup_polys.Size = new System.Drawing.Size(143, 21);
			this.button_mark_dup_polys.TabIndex = 117;
			this.button_mark_dup_polys.Text = "Mark Duplicates";
			this.tool_tip.SetToolTip(this.button_mark_dup_polys, "Mark all duplicate polygons");
			this.button_mark_dup_polys.UseVisualStyleBackColor = true;
			this.button_mark_dup_polys.Click += new System.EventHandler(this.button_mark_dup_polys_Click);
			// 
			// button_mark_connected
			// 
			this.button_mark_connected.Location = new System.Drawing.Point(4, 663);
			this.button_mark_connected.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_connected.Name = "button_mark_connected";
			this.button_mark_connected.Size = new System.Drawing.Size(143, 21);
			this.button_mark_connected.TabIndex = 118;
			this.button_mark_connected.Text = "Mark Connected";
			this.tool_tip.SetToolTip(this.button_mark_connected, "Mark all polygons connected to the selected one");
			this.button_mark_connected.UseVisualStyleBackColor = true;
			this.button_mark_connected.Click += new System.EventHandler(this.button_mark_connected_Click);
			// 
			// button_browser
			// 
			this.button_browser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_browser.Location = new System.Drawing.Point(1320, 1007);
			this.button_browser.Margin = new System.Windows.Forms.Padding(1);
			this.button_browser.Name = "button_browser";
			this.button_browser.Size = new System.Drawing.Size(143, 38);
			this.button_browser.TabIndex = 119;
			this.button_browser.Text = "Show DMesh Browser";
			this.tool_tip.SetToolTip(this.button_browser, "Show the DMesh Browser popup");
			this.button_browser.UseVisualStyleBackColor = true;
			this.button_browser.Click += new System.EventHandler(this.button_browser_Click);
			// 
			// button_flip_u
			// 
			this.button_flip_u.Location = new System.Drawing.Point(3, 20);
			this.button_flip_u.Margin = new System.Windows.Forms.Padding(1);
			this.button_flip_u.Name = "button_flip_u";
			this.button_flip_u.Size = new System.Drawing.Size(142, 21);
			this.button_flip_u.TabIndex = 70;
			this.button_flip_u.Text = "Flip U";
			this.tool_tip.SetToolTip(this.button_flip_u, "Flip the U (X) coordinates of the mesh (inverts the mesh)");
			this.button_flip_u.UseVisualStyleBackColor = true;
			this.button_flip_u.Click += new System.EventHandler(this.button_flip_u_Click);
			// 
			// button_edge_left
			// 
			this.button_edge_left.Location = new System.Drawing.Point(2, 135);
			this.button_edge_left.Margin = new System.Windows.Forms.Padding(1);
			this.button_edge_left.Name = "button_edge_left";
			this.button_edge_left.Size = new System.Drawing.Size(70, 21);
			this.button_edge_left.TabIndex = 74;
			this.button_edge_left.Text = "Left Edge";
			this.tool_tip.SetToolTip(this.button_edge_left, "Align the mesh with the left edge");
			this.button_edge_left.UseVisualStyleBackColor = true;
			this.button_edge_left.Click += new System.EventHandler(this.button_edge_left_Click);
			// 
			// button_center
			// 
			this.button_center.Location = new System.Drawing.Point(2, 89);
			this.button_center.Margin = new System.Windows.Forms.Padding(1);
			this.button_center.Name = "button_center";
			this.button_center.Size = new System.Drawing.Size(143, 21);
			this.button_center.TabIndex = 73;
			this.button_center.Text = "Center Mesh";
			this.tool_tip.SetToolTip(this.button_center, "Center the mesh on the origin (except for Y axis)");
			this.button_center.UseVisualStyleBackColor = true;
			this.button_center.Click += new System.EventHandler(this.button_center_Click);
			// 
			// button_edge_right
			// 
			this.button_edge_right.Location = new System.Drawing.Point(75, 135);
			this.button_edge_right.Margin = new System.Windows.Forms.Padding(1);
			this.button_edge_right.Name = "button_edge_right";
			this.button_edge_right.Size = new System.Drawing.Size(70, 21);
			this.button_edge_right.TabIndex = 75;
			this.button_edge_right.Text = "Right Edge";
			this.tool_tip.SetToolTip(this.button_edge_right, "Align the mesh with the right edge");
			this.button_edge_right.UseVisualStyleBackColor = true;
			this.button_edge_right.Click += new System.EventHandler(this.button_edge_right_Click);
			// 
			// button_rotate90
			// 
			this.button_rotate90.Location = new System.Drawing.Point(2, 66);
			this.button_rotate90.Margin = new System.Windows.Forms.Padding(1);
			this.button_rotate90.Name = "button_rotate90";
			this.button_rotate90.Size = new System.Drawing.Size(143, 21);
			this.button_rotate90.TabIndex = 72;
			this.button_rotate90.Text = "Rotate Mesh 90";
			this.tool_tip.SetToolTip(this.button_rotate90, "Rotate the mesh 90 degrees clockwise");
			this.button_rotate90.UseVisualStyleBackColor = true;
			this.button_rotate90.Click += new System.EventHandler(this.button_rotate90_Click);
			// 
			// button_flip_v
			// 
			this.button_flip_v.Location = new System.Drawing.Point(2, 43);
			this.button_flip_v.Margin = new System.Windows.Forms.Padding(1);
			this.button_flip_v.Name = "button_flip_v";
			this.button_flip_v.Size = new System.Drawing.Size(143, 21);
			this.button_flip_v.TabIndex = 71;
			this.button_flip_v.Text = "Flip V";
			this.tool_tip.SetToolTip(this.button_flip_v, "Flip the V (Y) coordinates of the mesh (inverts the mesh)");
			this.button_flip_v.UseVisualStyleBackColor = true;
			this.button_flip_v.Click += new System.EventHandler(this.button_flip_v_Click);
			// 
			// button_edge_bottom
			// 
			this.button_edge_bottom.Location = new System.Drawing.Point(2, 158);
			this.button_edge_bottom.Margin = new System.Windows.Forms.Padding(1);
			this.button_edge_bottom.Name = "button_edge_bottom";
			this.button_edge_bottom.Size = new System.Drawing.Size(143, 21);
			this.button_edge_bottom.TabIndex = 77;
			this.button_edge_bottom.Text = "Bottom Edge";
			this.tool_tip.SetToolTip(this.button_edge_bottom, "Align the mesh with the bottom edge");
			this.button_edge_bottom.UseVisualStyleBackColor = true;
			this.button_edge_bottom.Click += new System.EventHandler(this.button_edge_bottom_Click);
			// 
			// button_edge_top
			// 
			this.button_edge_top.Location = new System.Drawing.Point(2, 112);
			this.button_edge_top.Margin = new System.Windows.Forms.Padding(1);
			this.button_edge_top.Name = "button_edge_top";
			this.button_edge_top.Size = new System.Drawing.Size(143, 21);
			this.button_edge_top.TabIndex = 76;
			this.button_edge_top.Text = "Top Edge";
			this.tool_tip.SetToolTip(this.button_edge_top, "Align the mesh with the top edge");
			this.button_edge_top.UseVisualStyleBackColor = true;
			this.button_edge_top.Click += new System.EventHandler(this.button_edge_top_Click);
			// 
			// button_import_replace
			// 
			this.button_import_replace.Location = new System.Drawing.Point(3, 150);
			this.button_import_replace.Margin = new System.Windows.Forms.Padding(1);
			this.button_import_replace.Name = "button_import_replace";
			this.button_import_replace.Size = new System.Drawing.Size(143, 21);
			this.button_import_replace.TabIndex = 95;
			this.button_import_replace.Text = "Import OBJ - Replace";
			this.tool_tip.SetToolTip(this.button_import_replace, "Import an OBJ file to replace the current decal geometry");
			this.button_import_replace.UseVisualStyleBackColor = true;
			// 
			// button_import
			// 
			this.button_import.Location = new System.Drawing.Point(3, 127);
			this.button_import.Margin = new System.Windows.Forms.Padding(1);
			this.button_import.Name = "button_import";
			this.button_import.Size = new System.Drawing.Size(143, 21);
			this.button_import.TabIndex = 94;
			this.button_import.Text = "Import OBJ - New";
			this.tool_tip.SetToolTip(this.button_import, "Import an OBJ file as a new decal");
			this.button_import.UseVisualStyleBackColor = true;
			// 
			// button_planarize
			// 
			this.button_planarize.Location = new System.Drawing.Point(3, 104);
			this.button_planarize.Margin = new System.Windows.Forms.Padding(1);
			this.button_planarize.Name = "button_planarize";
			this.button_planarize.Size = new System.Drawing.Size(143, 21);
			this.button_planarize.TabIndex = 96;
			this.button_planarize.Text = "Combine Planar Polys";
			this.tool_tip.SetToolTip(this.button_planarize, "Combine all triangles into 4+ vert polygons if planar");
			this.button_planarize.UseVisualStyleBackColor = true;
			this.button_planarize.Click += new System.EventHandler(this.button_planarize_Click);
			// 
			// button_create_quad
			// 
			this.button_create_quad.Location = new System.Drawing.Point(3, 359);
			this.button_create_quad.Margin = new System.Windows.Forms.Padding(1);
			this.button_create_quad.Name = "button_create_quad";
			this.button_create_quad.Size = new System.Drawing.Size(143, 21);
			this.button_create_quad.TabIndex = 78;
			this.button_create_quad.Text = "Create Quad";
			this.tool_tip.SetToolTip(this.button_create_quad, "Create a default quad (of Size X/Y)");
			this.button_create_quad.UseVisualStyleBackColor = true;
			this.button_create_quad.Click += new System.EventHandler(this.button_create_quad_Click);
			// 
			// button_create_box
			// 
			this.button_create_box.Location = new System.Drawing.Point(3, 382);
			this.button_create_box.Margin = new System.Windows.Forms.Padding(1);
			this.button_create_box.Name = "button_create_box";
			this.button_create_box.Size = new System.Drawing.Size(143, 21);
			this.button_create_box.TabIndex = 101;
			this.button_create_box.Text = "Create Box";
			this.tool_tip.SetToolTip(this.button_create_box, "Create a default box (or Size X/Y/Height)");
			this.button_create_box.UseVisualStyleBackColor = true;
			this.button_create_box.Click += new System.EventHandler(this.button_create_box_Click);
			// 
			// button_poly_flip
			// 
			this.button_poly_flip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_poly_flip.Location = new System.Drawing.Point(1321, 931);
			this.button_poly_flip.Margin = new System.Windows.Forms.Padding(1);
			this.button_poly_flip.Name = "button_poly_flip";
			this.button_poly_flip.Size = new System.Drawing.Size(143, 21);
			this.button_poly_flip.TabIndex = 109;
			this.button_poly_flip.Text = "Flip Poly Normal";
			this.tool_tip.SetToolTip(this.button_poly_flip, "Flip the marked polygon normals - [Ctrl + F]");
			this.button_poly_flip.UseVisualStyleBackColor = true;
			this.button_poly_flip.Click += new System.EventHandler(this.button_poly_flip_Click);
			// 
			// button_align_vert_x
			// 
			this.button_align_vert_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_align_vert_x.Location = new System.Drawing.Point(1322, 370);
			this.button_align_vert_x.Margin = new System.Windows.Forms.Padding(1);
			this.button_align_vert_x.Name = "button_align_vert_x";
			this.button_align_vert_x.Size = new System.Drawing.Size(45, 21);
			this.button_align_vert_x.TabIndex = 71;
			this.button_align_vert_x.Text = "VertX";
			this.tool_tip.SetToolTip(this.button_align_vert_x, "Align all marked verts with the selected one");
			this.button_align_vert_x.UseVisualStyleBackColor = true;
			this.button_align_vert_x.Click += new System.EventHandler(this.button_align_vert_x_Click);
			// 
			// button_align_vert_y
			// 
			this.button_align_vert_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_align_vert_y.Location = new System.Drawing.Point(1369, 370);
			this.button_align_vert_y.Margin = new System.Windows.Forms.Padding(1);
			this.button_align_vert_y.Name = "button_align_vert_y";
			this.button_align_vert_y.Size = new System.Drawing.Size(47, 21);
			this.button_align_vert_y.TabIndex = 123;
			this.button_align_vert_y.Text = "VertY";
			this.tool_tip.SetToolTip(this.button_align_vert_y, "Align all marked verts with the selected one");
			this.button_align_vert_y.UseVisualStyleBackColor = true;
			this.button_align_vert_y.Click += new System.EventHandler(this.button_align_vert_y_Click);
			// 
			// button_align_vert_z
			// 
			this.button_align_vert_z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_align_vert_z.Location = new System.Drawing.Point(1419, 370);
			this.button_align_vert_z.Margin = new System.Windows.Forms.Padding(1);
			this.button_align_vert_z.Name = "button_align_vert_z";
			this.button_align_vert_z.Size = new System.Drawing.Size(45, 21);
			this.button_align_vert_z.TabIndex = 124;
			this.button_align_vert_z.Text = "VertZ";
			this.tool_tip.SetToolTip(this.button_align_vert_z, "Align all marked verts with the selected one");
			this.button_align_vert_z.UseVisualStyleBackColor = true;
			this.button_align_vert_z.Click += new System.EventHandler(this.button_align_vert_z_Click);
			// 
			// slider_smooth_angle_diff
			// 
			this.slider_smooth_angle_diff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_smooth_angle_diff.Location = new System.Drawing.Point(1321, 392);
			this.slider_smooth_angle_diff.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_smooth_angle_diff.Name = "slider_smooth_angle_diff";
			this.slider_smooth_angle_diff.RightMouseMultiplier = 5;
			this.slider_smooth_angle_diff.Size = new System.Drawing.Size(143, 19);
			this.slider_smooth_angle_diff.SlideText = "Smooth Diff";
			this.slider_smooth_angle_diff.SlideTol = 15;
			this.slider_smooth_angle_diff.TabIndex = 90;
			this.tool_tip.SetToolTip(this.slider_smooth_angle_diff, "SEE BELOW");
			this.slider_smooth_angle_diff.ToolTop = "Smoothing tolerance (on Save) between two polys with different textures";
			this.slider_smooth_angle_diff.ValueText = "0";
			this.slider_smooth_angle_diff.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_angle_diff_Feedback);
			// 
			// slider_smooth_angle_same
			// 
			this.slider_smooth_angle_same.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_smooth_angle_same.Location = new System.Drawing.Point(1321, 411);
			this.slider_smooth_angle_same.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_smooth_angle_same.Name = "slider_smooth_angle_same";
			this.slider_smooth_angle_same.RightMouseMultiplier = 5;
			this.slider_smooth_angle_same.Size = new System.Drawing.Size(143, 19);
			this.slider_smooth_angle_same.SlideText = "Smooth Same";
			this.slider_smooth_angle_same.SlideTol = 15;
			this.slider_smooth_angle_same.TabIndex = 126;
			this.tool_tip.SetToolTip(this.slider_smooth_angle_same, "SEE BELOW");
			this.slider_smooth_angle_same.ToolTop = "Smoothing tolerance (on Save) between two polys with the same texture";
			this.slider_smooth_angle_same.ValueText = "0";
			this.slider_smooth_angle_same.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_angle_same_Feedback);
			// 
			// label_poly_filter
			// 
			this.label_poly_filter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_poly_filter.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_poly_filter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_poly_filter.Location = new System.Drawing.Point(1321, 433);
			this.label_poly_filter.Margin = new System.Windows.Forms.Padding(0);
			this.label_poly_filter.Name = "label_poly_filter";
			this.label_poly_filter.Size = new System.Drawing.Size(143, 19);
			this.label_poly_filter.TabIndex = 117;
			this.label_poly_filter.Text = "PolyFilter: ALL";
			this.label_poly_filter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tool_tip.SetToolTip(this.label_poly_filter, "Cycle polygon view filter (F1)");
			this.label_poly_filter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_poly_filter_MouseDown);
			// 
			// button_mark_untextured
			// 
			this.button_mark_untextured.Location = new System.Drawing.Point(4, 617);
			this.button_mark_untextured.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_untextured.Name = "button_mark_untextured";
			this.button_mark_untextured.Size = new System.Drawing.Size(143, 21);
			this.button_mark_untextured.TabIndex = 128;
			this.button_mark_untextured.Text = "Mark Untextured";
			this.tool_tip.SetToolTip(this.button_mark_untextured, "Mark all untextured polygons");
			this.button_mark_untextured.UseVisualStyleBackColor = true;
			this.button_mark_untextured.Click += new System.EventHandler(this.button_mark_untextured_Click);
			// 
			// slider_inset_length
			// 
			this.slider_inset_length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_inset_length.Location = new System.Drawing.Point(1320, 564);
			this.slider_inset_length.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_inset_length.Name = "slider_inset_length";
			this.slider_inset_length.RightMouseMultiplier = 4;
			this.slider_inset_length.Size = new System.Drawing.Size(143, 19);
			this.slider_inset_length.SlideText = "Inset Length";
			this.slider_inset_length.SlideTol = 15;
			this.slider_inset_length.TabIndex = 129;
			this.tool_tip.SetToolTip(this.slider_inset_length, "SEE BELOW");
			this.slider_inset_length.ToolTop = "";
			this.slider_inset_length.ValueText = "0.25";
			this.slider_inset_length.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_inset_length_Feedback);
			// 
			// slider_inset_bevel
			// 
			this.slider_inset_bevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_inset_bevel.Location = new System.Drawing.Point(1320, 583);
			this.slider_inset_bevel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_inset_bevel.Name = "slider_inset_bevel";
			this.slider_inset_bevel.RightMouseMultiplier = 5;
			this.slider_inset_bevel.Size = new System.Drawing.Size(143, 19);
			this.slider_inset_bevel.SlideText = "Inset Percent";
			this.slider_inset_bevel.SlideTol = 15;
			this.slider_inset_bevel.TabIndex = 130;
			this.tool_tip.SetToolTip(this.slider_inset_bevel, "SEE BELOW");
			this.slider_inset_bevel.ToolTop = "";
			this.slider_inset_bevel.ValueText = "25";
			this.slider_inset_bevel.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_inset_bevel_Feedback);
			// 
			// button_extrude_selected
			// 
			this.button_extrude_selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_extrude_selected.Location = new System.Drawing.Point(1320, 477);
			this.button_extrude_selected.Margin = new System.Windows.Forms.Padding(1);
			this.button_extrude_selected.Name = "button_extrude_selected";
			this.button_extrude_selected.Size = new System.Drawing.Size(143, 21);
			this.button_extrude_selected.TabIndex = 131;
			this.button_extrude_selected.Text = "Extrude Selected";
			this.tool_tip.SetToolTip(this.button_extrude_selected, "Extrude the selected polygon");
			this.button_extrude_selected.UseVisualStyleBackColor = true;
			this.button_extrude_selected.Click += new System.EventHandler(this.button_extrude_selected_Click);
			// 
			// button_extrude_marked
			// 
			this.button_extrude_marked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_extrude_marked.Location = new System.Drawing.Point(1320, 500);
			this.button_extrude_marked.Margin = new System.Windows.Forms.Padding(1);
			this.button_extrude_marked.Name = "button_extrude_marked";
			this.button_extrude_marked.Size = new System.Drawing.Size(143, 21);
			this.button_extrude_marked.TabIndex = 132;
			this.button_extrude_marked.Text = "Extrude Marked";
			this.tool_tip.SetToolTip(this.button_extrude_marked, "Extrude the marked polygons");
			this.button_extrude_marked.UseVisualStyleBackColor = true;
			this.button_extrude_marked.Click += new System.EventHandler(this.button_extrude_marked_Click);
			// 
			// button_inset_bevel
			// 
			this.button_inset_bevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_inset_bevel.Location = new System.Drawing.Point(1320, 542);
			this.button_inset_bevel.Margin = new System.Windows.Forms.Padding(1);
			this.button_inset_bevel.Name = "button_inset_bevel";
			this.button_inset_bevel.Size = new System.Drawing.Size(143, 21);
			this.button_inset_bevel.TabIndex = 133;
			this.button_inset_bevel.Text = "Inset/Bevel Marked";
			this.tool_tip.SetToolTip(this.button_inset_bevel, "Extrude the marked polygons");
			this.button_inset_bevel.UseVisualStyleBackColor = true;
			this.button_inset_bevel.Click += new System.EventHandler(this.button_inset_bevel_Click);
			// 
			// button_split_edge
			// 
			this.button_split_edge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_split_edge.Location = new System.Drawing.Point(1320, 603);
			this.button_split_edge.Margin = new System.Windows.Forms.Padding(1);
			this.button_split_edge.Name = "button_split_edge";
			this.button_split_edge.Size = new System.Drawing.Size(143, 21);
			this.button_split_edge.TabIndex = 134;
			this.button_split_edge.Text = "Split Edge";
			this.tool_tip.SetToolTip(this.button_split_edge, "Add an extra vert between the two marked verts (applies to all connected polygons" +
		")");
			this.button_split_edge.UseVisualStyleBackColor = true;
			this.button_split_edge.Click += new System.EventHandler(this.button_split_edge_Click);
			// 
			// button_duplicate_3way
			// 
			this.button_duplicate_3way.Location = new System.Drawing.Point(78, 594);
			this.button_duplicate_3way.Margin = new System.Windows.Forms.Padding(1);
			this.button_duplicate_3way.Name = "button_duplicate_3way";
			this.button_duplicate_3way.Size = new System.Drawing.Size(69, 21);
			this.button_duplicate_3way.TabIndex = 137;
			this.button_duplicate_3way.Text = "Dup3";
			this.tool_tip.SetToolTip(this.button_duplicate_3way, "Duplicate the marked polygons 120 and 240 degrees around the Y axis");
			this.button_duplicate_3way.UseVisualStyleBackColor = true;
			this.button_duplicate_3way.Click += new System.EventHandler(this.button_duplicate_3way_Click);
			// 
			// button_duplicate_4way
			// 
			this.button_duplicate_4way.Location = new System.Drawing.Point(78, 571);
			this.button_duplicate_4way.Margin = new System.Windows.Forms.Padding(1);
			this.button_duplicate_4way.Name = "button_duplicate_4way";
			this.button_duplicate_4way.Size = new System.Drawing.Size(69, 21);
			this.button_duplicate_4way.TabIndex = 136;
			this.button_duplicate_4way.Text = "Dup4";
			this.tool_tip.SetToolTip(this.button_duplicate_4way, "Duplicate the marked polygons 90/180/270 degrees around Y-axis");
			this.button_duplicate_4way.UseVisualStyleBackColor = true;
			this.button_duplicate_4way.Click += new System.EventHandler(this.button_duplicate_4way_Click);
			// 
			// button_duplicate_z
			// 
			this.button_duplicate_z.Location = new System.Drawing.Point(5, 594);
			this.button_duplicate_z.Margin = new System.Windows.Forms.Padding(1);
			this.button_duplicate_z.Name = "button_duplicate_z";
			this.button_duplicate_z.Size = new System.Drawing.Size(70, 21);
			this.button_duplicate_z.TabIndex = 135;
			this.button_duplicate_z.Text = "DupZ";
			this.tool_tip.SetToolTip(this.button_duplicate_z, "Duplicate the marked polygons along the Z-axis");
			this.button_duplicate_z.UseVisualStyleBackColor = true;
			this.button_duplicate_z.Click += new System.EventHandler(this.button_duplicate_z_Click);
			// 
			// button_split_poly
			// 
			this.button_split_poly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_split_poly.Location = new System.Drawing.Point(1320, 626);
			this.button_split_poly.Margin = new System.Windows.Forms.Padding(1);
			this.button_split_poly.Name = "button_split_poly";
			this.button_split_poly.Size = new System.Drawing.Size(143, 21);
			this.button_split_poly.TabIndex = 138;
			this.button_split_poly.Text = "Split Polygon";
			this.tool_tip.SetToolTip(this.button_split_poly, "Split a polygon along the two marked verts");
			this.button_split_poly.UseVisualStyleBackColor = true;
			this.button_split_poly.Click += new System.EventHandler(this.button_split_poly_Click);
			// 
			// button_duplicate_x
			// 
			this.button_duplicate_x.Location = new System.Drawing.Point(5, 571);
			this.button_duplicate_x.Margin = new System.Windows.Forms.Padding(1);
			this.button_duplicate_x.Name = "button_duplicate_x";
			this.button_duplicate_x.Size = new System.Drawing.Size(70, 21);
			this.button_duplicate_x.TabIndex = 139;
			this.button_duplicate_x.Text = "DupX";
			this.tool_tip.SetToolTip(this.button_duplicate_x, "Duplicate the marked polygons along the X-axis");
			this.button_duplicate_x.UseVisualStyleBackColor = true;
			this.button_duplicate_x.Click += new System.EventHandler(this.button_duplicate_x_Click);
			// 
			// button_bevel_edge
			// 
			this.button_bevel_edge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_bevel_edge.Location = new System.Drawing.Point(1320, 672);
			this.button_bevel_edge.Margin = new System.Windows.Forms.Padding(1);
			this.button_bevel_edge.Name = "button_bevel_edge";
			this.button_bevel_edge.Size = new System.Drawing.Size(143, 21);
			this.button_bevel_edge.TabIndex = 140;
			this.button_bevel_edge.Text = "Bevel Edge";
			this.tool_tip.SetToolTip(this.button_bevel_edge, "Add polygons along pairs of marked verts");
			this.button_bevel_edge.UseVisualStyleBackColor = true;
			this.button_bevel_edge.Click += new System.EventHandler(this.button1_Click);
			// 
			// button_combine_verts
			// 
			this.button_combine_verts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_combine_verts.Location = new System.Drawing.Point(1320, 725);
			this.button_combine_verts.Margin = new System.Windows.Forms.Padding(1);
			this.button_combine_verts.Name = "button_combine_verts";
			this.button_combine_verts.Size = new System.Drawing.Size(143, 21);
			this.button_combine_verts.TabIndex = 141;
			this.button_combine_verts.Text = "Combine Marked Verts";
			this.tool_tip.SetToolTip(this.button_combine_verts, "Combine all marked verts into one (and fix/delete polygons)");
			this.button_combine_verts.UseVisualStyleBackColor = true;
			this.button_combine_verts.Click += new System.EventHandler(this.button_combine_verts_Click);
			// 
			// button_uv_quarter1
			// 
			this.button_uv_quarter1.Location = new System.Drawing.Point(43, 246);
			this.button_uv_quarter1.Margin = new System.Windows.Forms.Padding(1);
			this.button_uv_quarter1.Name = "button_uv_quarter1";
			this.button_uv_quarter1.Size = new System.Drawing.Size(29, 21);
			this.button_uv_quarter1.TabIndex = 91;
			this.button_uv_quarter1.Text = "1";
			this.tool_tip.SetToolTip(this.button_uv_quarter1, "Center and squeeze the UV coordinates in quarter of the texture");
			this.button_uv_quarter1.UseVisualStyleBackColor = true;
			this.button_uv_quarter1.Click += new System.EventHandler(this.button_uv_quarter1_Click);
			// 
			// button_uv_quarter2
			// 
			this.button_uv_quarter2.Location = new System.Drawing.Point(75, 246);
			this.button_uv_quarter2.Margin = new System.Windows.Forms.Padding(1);
			this.button_uv_quarter2.Name = "button_uv_quarter2";
			this.button_uv_quarter2.Size = new System.Drawing.Size(29, 21);
			this.button_uv_quarter2.TabIndex = 92;
			this.button_uv_quarter2.Text = "2";
			this.tool_tip.SetToolTip(this.button_uv_quarter2, "Center and squeeze the UV coordinates in quarter of the texture");
			this.button_uv_quarter2.UseVisualStyleBackColor = true;
			this.button_uv_quarter2.Click += new System.EventHandler(this.button_uv_quarter2_Click);
			// 
			// button_uv_quarter4
			// 
			this.button_uv_quarter4.Location = new System.Drawing.Point(75, 269);
			this.button_uv_quarter4.Margin = new System.Windows.Forms.Padding(1);
			this.button_uv_quarter4.Name = "button_uv_quarter4";
			this.button_uv_quarter4.Size = new System.Drawing.Size(29, 21);
			this.button_uv_quarter4.TabIndex = 94;
			this.button_uv_quarter4.Text = "4";
			this.tool_tip.SetToolTip(this.button_uv_quarter4, "Center and squeeze the UV coordinates in quarter of the texture");
			this.button_uv_quarter4.UseVisualStyleBackColor = true;
			this.button_uv_quarter4.Click += new System.EventHandler(this.button_uv_quarter4_Click);
			// 
			// button_uv_button3
			// 
			this.button_uv_button3.Location = new System.Drawing.Point(43, 269);
			this.button_uv_button3.Margin = new System.Windows.Forms.Padding(1);
			this.button_uv_button3.Name = "button_uv_button3";
			this.button_uv_button3.Size = new System.Drawing.Size(29, 21);
			this.button_uv_button3.TabIndex = 93;
			this.button_uv_button3.Text = "3";
			this.tool_tip.SetToolTip(this.button_uv_button3, "Center and squeeze the UV coordinates in quarter of the texture");
			this.button_uv_button3.UseVisualStyleBackColor = true;
			this.button_uv_button3.Click += new System.EventHandler(this.button_uv_button3_Click);
			// 
			// button_subdivide_mesh
			// 
			this.button_subdivide_mesh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_subdivide_mesh.Location = new System.Drawing.Point(1321, 761);
			this.button_subdivide_mesh.Margin = new System.Windows.Forms.Padding(1);
			this.button_subdivide_mesh.Name = "button_subdivide_mesh";
			this.button_subdivide_mesh.Size = new System.Drawing.Size(143, 21);
			this.button_subdivide_mesh.TabIndex = 142;
			this.button_subdivide_mesh.Text = "Subdivide Marked Polys";
			this.tool_tip.SetToolTip(this.button_subdivide_mesh, "Subdivide the marked polygons into quads");
			this.button_subdivide_mesh.UseVisualStyleBackColor = true;
			this.button_subdivide_mesh.Click += new System.EventHandler(this.button_subdivide_mesh_Click);
			// 
			// button_triangle_marked_nonplanar
			// 
			this.button_triangle_marked_nonplanar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_triangle_marked_nonplanar.Location = new System.Drawing.Point(1321, 839);
			this.button_triangle_marked_nonplanar.Margin = new System.Windows.Forms.Padding(1);
			this.button_triangle_marked_nonplanar.Name = "button_triangle_marked_nonplanar";
			this.button_triangle_marked_nonplanar.Size = new System.Drawing.Size(143, 21);
			this.button_triangle_marked_nonplanar.TabIndex = 143;
			this.button_triangle_marked_nonplanar.Text = "TriA - Marked Non-Planar";
			this.tool_tip.SetToolTip(this.button_triangle_marked_nonplanar, "Triangulate any marked polygons that are non-planar");
			this.button_triangle_marked_nonplanar.UseVisualStyleBackColor = true;
			this.button_triangle_marked_nonplanar.Click += new System.EventHandler(this.button_triangle_marked_nonplanar_Click);
			// 
			// button_triangulate_all_nonplanar
			// 
			this.button_triangulate_all_nonplanar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_triangulate_all_nonplanar.Location = new System.Drawing.Point(1321, 816);
			this.button_triangulate_all_nonplanar.Margin = new System.Windows.Forms.Padding(1);
			this.button_triangulate_all_nonplanar.Name = "button_triangulate_all_nonplanar";
			this.button_triangulate_all_nonplanar.Size = new System.Drawing.Size(143, 21);
			this.button_triangulate_all_nonplanar.TabIndex = 144;
			this.button_triangulate_all_nonplanar.Text = "TriA - All Non-Planar";
			this.tool_tip.SetToolTip(this.button_triangulate_all_nonplanar, "Triangulate all polygons that are non-planar");
			this.button_triangulate_all_nonplanar.UseVisualStyleBackColor = true;
			this.button_triangulate_all_nonplanar.Click += new System.EventHandler(this.button_triangulate_all_nonplanar_Click);
			// 
			// button_scale_dn1
			// 
			this.button_scale_dn1.Location = new System.Drawing.Point(2, 200);
			this.button_scale_dn1.Margin = new System.Windows.Forms.Padding(1);
			this.button_scale_dn1.Name = "button_scale_dn1";
			this.button_scale_dn1.Size = new System.Drawing.Size(70, 21);
			this.button_scale_dn1.TabIndex = 98;
			this.button_scale_dn1.Text = "Scale 0.9";
			this.tool_tip.SetToolTip(this.button_scale_dn1, "Scale the marked poly UVs");
			this.button_scale_dn1.UseVisualStyleBackColor = true;
			this.button_scale_dn1.Click += new System.EventHandler(this.button_scale_dn1_Click);
			// 
			// button_scale_up1
			// 
			this.button_scale_up1.Location = new System.Drawing.Point(75, 200);
			this.button_scale_up1.Margin = new System.Windows.Forms.Padding(1);
			this.button_scale_up1.Name = "button_scale_up1";
			this.button_scale_up1.Size = new System.Drawing.Size(70, 21);
			this.button_scale_up1.TabIndex = 97;
			this.button_scale_up1.Text = "Scale 1.1";
			this.tool_tip.SetToolTip(this.button_scale_up1, "Scale the marked poly UVs");
			this.button_scale_up1.UseVisualStyleBackColor = true;
			this.button_scale_up1.Click += new System.EventHandler(this.button_scale_up1_Click);
			// 
			// button_scale_dn2
			// 
			this.button_scale_dn2.Location = new System.Drawing.Point(2, 223);
			this.button_scale_dn2.Margin = new System.Windows.Forms.Padding(1);
			this.button_scale_dn2.Name = "button_scale_dn2";
			this.button_scale_dn2.Size = new System.Drawing.Size(70, 21);
			this.button_scale_dn2.TabIndex = 96;
			this.button_scale_dn2.Text = "Scale 0.5";
			this.tool_tip.SetToolTip(this.button_scale_dn2, "Scale the marked poly UVs");
			this.button_scale_dn2.UseVisualStyleBackColor = true;
			this.button_scale_dn2.Click += new System.EventHandler(this.button_scale_dn2_Click);
			// 
			// button_scale_up2
			// 
			this.button_scale_up2.Location = new System.Drawing.Point(75, 223);
			this.button_scale_up2.Margin = new System.Windows.Forms.Padding(1);
			this.button_scale_up2.Name = "button_scale_up2";
			this.button_scale_up2.Size = new System.Drawing.Size(70, 21);
			this.button_scale_up2.TabIndex = 95;
			this.button_scale_up2.Text = "Scale 2.0";
			this.tool_tip.SetToolTip(this.button_scale_up2, "Scale the marked poly UVs");
			this.button_scale_up2.UseVisualStyleBackColor = true;
			this.button_scale_up2.Click += new System.EventHandler(this.button_scale_up2_Click);
			// 
			// slider_bevel_width
			// 
			this.slider_bevel_width.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_bevel_width.Location = new System.Drawing.Point(1320, 694);
			this.slider_bevel_width.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_bevel_width.Name = "slider_bevel_width";
			this.slider_bevel_width.RightMouseMultiplier = 4;
			this.slider_bevel_width.Size = new System.Drawing.Size(143, 19);
			this.slider_bevel_width.SlideText = "Bevel Width";
			this.slider_bevel_width.SlideTol = 15;
			this.slider_bevel_width.TabIndex = 145;
			this.tool_tip.SetToolTip(this.slider_bevel_width, "SEE BELOW");
			this.slider_bevel_width.ToolTop = "";
			this.slider_bevel_width.ValueText = "0.125";
			this.slider_bevel_width.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_bevel_width_Feedback);
			// 
			// button_color4
			// 
			this.button_color4.BackColor = System.Drawing.Color.White;
			this.button_color4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_color4.Location = new System.Drawing.Point(118, 20);
			this.button_color4.Margin = new System.Windows.Forms.Padding(1);
			this.button_color4.Name = "button_color4";
			this.button_color4.Size = new System.Drawing.Size(37, 21);
			this.button_color4.TabIndex = 93;
			this.button_color4.Text = "4";
			this.tool_tip.SetToolTip(this.button_color4, "Color (for lights)");
			this.button_color4.UseVisualStyleBackColor = false;
			this.button_color4.Click += new System.EventHandler(this.button_color4_Click);
			// 
			// button_color3
			// 
			this.button_color3.BackColor = System.Drawing.Color.White;
			this.button_color3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_color3.Location = new System.Drawing.Point(79, 20);
			this.button_color3.Margin = new System.Windows.Forms.Padding(1);
			this.button_color3.Name = "button_color3";
			this.button_color3.Size = new System.Drawing.Size(37, 21);
			this.button_color3.TabIndex = 92;
			this.button_color3.Text = "3";
			this.tool_tip.SetToolTip(this.button_color3, "Color (for lights)");
			this.button_color3.UseVisualStyleBackColor = false;
			this.button_color3.Click += new System.EventHandler(this.button_color3_Click);
			// 
			// button_color2
			// 
			this.button_color2.BackColor = System.Drawing.Color.White;
			this.button_color2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_color2.Location = new System.Drawing.Point(40, 20);
			this.button_color2.Margin = new System.Windows.Forms.Padding(1);
			this.button_color2.Name = "button_color2";
			this.button_color2.Size = new System.Drawing.Size(37, 21);
			this.button_color2.TabIndex = 91;
			this.button_color2.Text = "2";
			this.tool_tip.SetToolTip(this.button_color2, "Color (for lights)");
			this.button_color2.UseVisualStyleBackColor = false;
			this.button_color2.Click += new System.EventHandler(this.button_color2_Click);
			// 
			// button_color1
			// 
			this.button_color1.BackColor = System.Drawing.Color.White;
			this.button_color1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_color1.Location = new System.Drawing.Point(1, 20);
			this.button_color1.Margin = new System.Windows.Forms.Padding(1);
			this.button_color1.Name = "button_color1";
			this.button_color1.Size = new System.Drawing.Size(37, 21);
			this.button_color1.TabIndex = 90;
			this.button_color1.Text = "1";
			this.tool_tip.SetToolTip(this.button_color1, "Color (for lights)");
			this.button_color1.UseVisualStyleBackColor = false;
			this.button_color1.Click += new System.EventHandler(this.button_color1_Click);
			// 
			// button_bisect_poly
			// 
			this.button_bisect_poly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_bisect_poly.Location = new System.Drawing.Point(1320, 649);
			this.button_bisect_poly.Margin = new System.Windows.Forms.Padding(1);
			this.button_bisect_poly.Name = "button_bisect_poly";
			this.button_bisect_poly.Size = new System.Drawing.Size(143, 21);
			this.button_bisect_poly.TabIndex = 139;
			this.button_bisect_poly.Text = "Bisect Polygon";
			this.tool_tip.SetToolTip(this.button_bisect_poly, "Split a polygon along the plane that lies on the three marked verts");
			this.button_bisect_poly.UseVisualStyleBackColor = true;
			this.button_bisect_poly.Click += new System.EventHandler(this.button_bisect_poly_Click);
			// 
			// slider_sizeheight
			// 
			this.slider_sizeheight.Location = new System.Drawing.Point(4, 465);
			this.slider_sizeheight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_sizeheight.Name = "slider_sizeheight";
			this.slider_sizeheight.RightMouseMultiplier = 4;
			this.slider_sizeheight.Size = new System.Drawing.Size(143, 19);
			this.slider_sizeheight.SlideText = "Height";
			this.slider_sizeheight.SlideTol = 15;
			this.slider_sizeheight.TabIndex = 107;
			this.slider_sizeheight.ToolTop = "Size of the default objects (Height)";
			this.slider_sizeheight.ValueText = "1.0";
			this.slider_sizeheight.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_sizeheight_Feedback);
			// 
			// slider_sizesegments
			// 
			this.slider_sizesegments.Location = new System.Drawing.Point(4, 484);
			this.slider_sizesegments.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_sizesegments.Name = "slider_sizesegments";
			this.slider_sizesegments.RightMouseMultiplier = 4;
			this.slider_sizesegments.Size = new System.Drawing.Size(143, 19);
			this.slider_sizesegments.SlideText = "Segments";
			this.slider_sizesegments.SlideTol = 15;
			this.slider_sizesegments.TabIndex = 108;
			this.slider_sizesegments.ToolTop = "Number of segments in the default cylinder";
			this.slider_sizesegments.ValueText = "16";
			this.slider_sizesegments.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_sizesegments_Feedback);
			// 
			// label_vert_display
			// 
			this.label_vert_display.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_vert_display.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_vert_display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_vert_display.Location = new System.Drawing.Point(1321, 452);
			this.label_vert_display.Margin = new System.Windows.Forms.Padding(0);
			this.label_vert_display.Name = "label_vert_display";
			this.label_vert_display.Size = new System.Drawing.Size(143, 19);
			this.label_vert_display.TabIndex = 127;
			this.label_vert_display.Text = "Vert Normals: HIDE";
			this.label_vert_display.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_vert_display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_vert_display_MouseDown);
			// 
			// panel_view
			// 
			this.panel_view.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panel_view.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_view.Controls.Add(this.label_lighting);
			this.panel_view.Controls.Add(this.label_gimbal);
			this.panel_view.Controls.Add(this.label_view_dark);
			this.panel_view.Controls.Add(this.label_view_layout);
			this.panel_view.Controls.Add(this.label_view_ortho);
			this.panel_view.Controls.Add(this.label_view);
			this.panel_view.Controls.Add(this.label_view_persp);
			this.panel_view.Location = new System.Drawing.Point(0, 901);
			this.panel_view.Margin = new System.Windows.Forms.Padding(1);
			this.panel_view.Name = "panel_view";
			this.panel_view.Size = new System.Drawing.Size(149, 144);
			this.panel_view.TabIndex = 48;
			// 
			// label_view
			// 
			this.label_view.BackColor = System.Drawing.SystemColors.Control;
			this.label_view.Location = new System.Drawing.Point(2, 0);
			this.label_view.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view.Name = "label_view";
			this.label_view.Size = new System.Drawing.Size(143, 19);
			this.label_view.TabIndex = 45;
			this.label_view.Text = "VIEW";
			this.label_view.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel_texturing
			// 
			this.panel_texturing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_texturing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_texturing.Controls.Add(this.button_scale_dn1);
			this.panel_texturing.Controls.Add(this.button_scale_up1);
			this.panel_texturing.Controls.Add(this.button_scale_dn2);
			this.panel_texturing.Controls.Add(this.button_scale_up2);
			this.panel_texturing.Controls.Add(this.button_uv_quarter4);
			this.panel_texturing.Controls.Add(this.button_uv_button3);
			this.panel_texturing.Controls.Add(this.button_uv_quarter2);
			this.panel_texturing.Controls.Add(this.button_uv_quarter1);
			this.panel_texturing.Controls.Add(this.button_texture_center_v);
			this.panel_texturing.Controls.Add(this.button_texture_center_u);
			this.panel_texturing.Controls.Add(this.button_texture_show_list);
			this.panel_texturing.Controls.Add(this.label_texture_name);
			this.panel_texturing.Controls.Add(this.button_texture_default_map);
			this.panel_texturing.Controls.Add(this.button_texture_snap4);
			this.panel_texturing.Controls.Add(this.button_texture_snap8);
			this.panel_texturing.Controls.Add(this.button_texture_planar_z);
			this.panel_texturing.Controls.Add(this.button_texture_planar_y);
			this.panel_texturing.Controls.Add(this.button_texture_planar_x);
			this.panel_texturing.Controls.Add(this.button_align_marked);
			this.panel_texturing.Controls.Add(this.button_texture_box_map);
			this.panel_texturing.Controls.Add(this.label_textures);
			this.panel_texturing.Location = new System.Drawing.Point(1317, 27);
			this.panel_texturing.Margin = new System.Windows.Forms.Padding(1);
			this.panel_texturing.Name = "panel_texturing";
			this.panel_texturing.Size = new System.Drawing.Size(149, 299);
			this.panel_texturing.TabIndex = 49;
			// 
			// label_textures
			// 
			this.label_textures.BackColor = System.Drawing.SystemColors.Control;
			this.label_textures.Location = new System.Drawing.Point(2, 0);
			this.label_textures.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_textures.Name = "label_textures";
			this.label_textures.Size = new System.Drawing.Size(143, 19);
			this.label_textures.TabIndex = 45;
			this.label_textures.Text = "TEXTURING";
			this.label_textures.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel3
			// 
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel3.Controls.Add(this.label_geometry);
			this.panel3.Controls.Add(this.button_flip_u);
			this.panel3.Controls.Add(this.button_edge_left);
			this.panel3.Controls.Add(this.button_center);
			this.panel3.Controls.Add(this.button_edge_right);
			this.panel3.Controls.Add(this.button_rotate90);
			this.panel3.Controls.Add(this.button_flip_v);
			this.panel3.Controls.Add(this.button_edge_bottom);
			this.panel3.Controls.Add(this.button_edge_top);
			this.panel3.Location = new System.Drawing.Point(0, 173);
			this.panel3.Margin = new System.Windows.Forms.Padding(1);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(149, 184);
			this.panel3.TabIndex = 93;
			// 
			// label_geometry
			// 
			this.label_geometry.BackColor = System.Drawing.SystemColors.Control;
			this.label_geometry.Location = new System.Drawing.Point(2, 0);
			this.label_geometry.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_geometry.Name = "label_geometry";
			this.label_geometry.Size = new System.Drawing.Size(141, 19);
			this.label_geometry.TabIndex = 45;
			this.label_geometry.Text = "GEOMETRY";
			this.label_geometry.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.button_color_paste_all);
			this.panel4.Controls.Add(this.button_color_copy_all);
			this.panel4.Controls.Add(this.button_color_paste);
			this.panel4.Controls.Add(this.button_color_copy);
			this.panel4.Controls.Add(this.slider_color_brightness);
			this.panel4.Controls.Add(this.slider_color_saturation);
			this.panel4.Controls.Add(this.slider_color_hue);
			this.panel4.Controls.Add(this.slider_color_blue);
			this.panel4.Controls.Add(this.slider_color_green);
			this.panel4.Controls.Add(this.slider_color_red);
			this.panel4.Controls.Add(this.button_color4);
			this.panel4.Controls.Add(this.button_color3);
			this.panel4.Controls.Add(this.button_color2);
			this.panel4.Controls.Add(this.button_color1);
			this.panel4.Controls.Add(this.label6);
			this.panel4.Location = new System.Drawing.Point(1157, 27);
			this.panel4.Margin = new System.Windows.Forms.Padding(1);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(158, 261);
			this.panel4.TabIndex = 120;
			// 
			// button_color_paste_all
			// 
			this.button_color_paste_all.Enabled = false;
			this.button_color_paste_all.Location = new System.Drawing.Point(1, 235);
			this.button_color_paste_all.Margin = new System.Windows.Forms.Padding(1);
			this.button_color_paste_all.Name = "button_color_paste_all";
			this.button_color_paste_all.Size = new System.Drawing.Size(154, 21);
			this.button_color_paste_all.TabIndex = 103;
			this.button_color_paste_all.Text = "Paste ALL Colors";
			this.button_color_paste_all.UseVisualStyleBackColor = true;
			this.button_color_paste_all.Click += new System.EventHandler(this.button_color_paste_all_Click);
			// 
			// button_color_copy_all
			// 
			this.button_color_copy_all.Location = new System.Drawing.Point(1, 212);
			this.button_color_copy_all.Margin = new System.Windows.Forms.Padding(1);
			this.button_color_copy_all.Name = "button_color_copy_all";
			this.button_color_copy_all.Size = new System.Drawing.Size(154, 21);
			this.button_color_copy_all.TabIndex = 102;
			this.button_color_copy_all.Text = "Copy ALL Colors";
			this.button_color_copy_all.UseVisualStyleBackColor = true;
			this.button_color_copy_all.Click += new System.EventHandler(this.button_color_copy_all_Click);
			// 
			// button_color_paste
			// 
			this.button_color_paste.Enabled = false;
			this.button_color_paste.Location = new System.Drawing.Point(1, 189);
			this.button_color_paste.Margin = new System.Windows.Forms.Padding(1);
			this.button_color_paste.Name = "button_color_paste";
			this.button_color_paste.Size = new System.Drawing.Size(154, 21);
			this.button_color_paste.TabIndex = 101;
			this.button_color_paste.Text = "Paste Color";
			this.button_color_paste.UseVisualStyleBackColor = true;
			this.button_color_paste.Click += new System.EventHandler(this.button_color_paste_Click);
			// 
			// button_color_copy
			// 
			this.button_color_copy.Location = new System.Drawing.Point(1, 166);
			this.button_color_copy.Margin = new System.Windows.Forms.Padding(1);
			this.button_color_copy.Name = "button_color_copy";
			this.button_color_copy.Size = new System.Drawing.Size(154, 21);
			this.button_color_copy.TabIndex = 100;
			this.button_color_copy.Text = "Copy Color";
			this.button_color_copy.UseVisualStyleBackColor = true;
			this.button_color_copy.Click += new System.EventHandler(this.button_color_copy_Click);
			// 
			// slider_color_brightness
			// 
			this.slider_color_brightness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_brightness.Location = new System.Drawing.Point(5, 146);
			this.slider_color_brightness.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_brightness.Name = "slider_color_brightness";
			this.slider_color_brightness.RightMouseMultiplier = 6;
			this.slider_color_brightness.Size = new System.Drawing.Size(143, 19);
			this.slider_color_brightness.SlideText = "Brightness";
			this.slider_color_brightness.SlideTol = 6;
			this.slider_color_brightness.TabIndex = 99;
			this.slider_color_brightness.ToolTop = "";
			this.slider_color_brightness.ValueText = "100";
			this.slider_color_brightness.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_brightness_Feedback);
			// 
			// slider_color_saturation
			// 
			this.slider_color_saturation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_saturation.Location = new System.Drawing.Point(5, 127);
			this.slider_color_saturation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_saturation.Name = "slider_color_saturation";
			this.slider_color_saturation.RightMouseMultiplier = 6;
			this.slider_color_saturation.Size = new System.Drawing.Size(143, 19);
			this.slider_color_saturation.SlideText = "Saturation";
			this.slider_color_saturation.SlideTol = 6;
			this.slider_color_saturation.TabIndex = 98;
			this.slider_color_saturation.ToolTop = "";
			this.slider_color_saturation.ValueText = "0";
			this.slider_color_saturation.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_saturation_Feedback);
			// 
			// slider_color_hue
			// 
			this.slider_color_hue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_hue.Location = new System.Drawing.Point(5, 108);
			this.slider_color_hue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_hue.Name = "slider_color_hue";
			this.slider_color_hue.RightMouseMultiplier = 3;
			this.slider_color_hue.Size = new System.Drawing.Size(143, 19);
			this.slider_color_hue.SlideText = "Hue";
			this.slider_color_hue.SlideTol = 3;
			this.slider_color_hue.TabIndex = 97;
			this.slider_color_hue.ToolTop = "";
			this.slider_color_hue.ValueText = "0";
			this.slider_color_hue.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_hue_Feedback);
			// 
			// slider_color_blue
			// 
			this.slider_color_blue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_blue.Location = new System.Drawing.Point(5, 80);
			this.slider_color_blue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_blue.Name = "slider_color_blue";
			this.slider_color_blue.RightMouseMultiplier = 5;
			this.slider_color_blue.Size = new System.Drawing.Size(143, 19);
			this.slider_color_blue.SlideText = "Blue";
			this.slider_color_blue.SlideTol = 5;
			this.slider_color_blue.TabIndex = 96;
			this.slider_color_blue.ToolTop = "";
			this.slider_color_blue.ValueText = "255";
			this.slider_color_blue.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_blue_Feedback);
			// 
			// slider_color_green
			// 
			this.slider_color_green.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_green.Location = new System.Drawing.Point(5, 61);
			this.slider_color_green.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_green.Name = "slider_color_green";
			this.slider_color_green.RightMouseMultiplier = 5;
			this.slider_color_green.Size = new System.Drawing.Size(143, 19);
			this.slider_color_green.SlideText = "Green";
			this.slider_color_green.SlideTol = 5;
			this.slider_color_green.TabIndex = 95;
			this.slider_color_green.ToolTop = "";
			this.slider_color_green.ValueText = "255";
			this.slider_color_green.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_green_Feedback);
			// 
			// slider_color_red
			// 
			this.slider_color_red.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_color_red.Location = new System.Drawing.Point(5, 42);
			this.slider_color_red.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_color_red.Name = "slider_color_red";
			this.slider_color_red.RightMouseMultiplier = 5;
			this.slider_color_red.Size = new System.Drawing.Size(143, 19);
			this.slider_color_red.SlideText = "Red";
			this.slider_color_red.SlideTol = 5;
			this.slider_color_red.TabIndex = 94;
			this.slider_color_red.ToolTop = "";
			this.slider_color_red.ValueText = "255";
			this.slider_color_red.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_color_red_Feedback);
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.SystemColors.Control;
			this.label6.Location = new System.Drawing.Point(2, 0);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(153, 19);
			this.label6.TabIndex = 45;
			this.label6.Text = "DECAL COLORS";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel5
			// 
			this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel5.Controls.Add(this.button_face_mark_same);
			this.panel5.Controls.Add(this.button_face_clear);
			this.panel5.Controls.Add(this.checklist_face);
			this.panel5.Controls.Add(this.button_face_copy);
			this.panel5.Controls.Add(this.label_face);
			this.panel5.Location = new System.Drawing.Point(1157, 290);
			this.panel5.Margin = new System.Windows.Forms.Padding(1);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(158, 222);
			this.panel5.TabIndex = 121;
			// 
			// button_face_mark_same
			// 
			this.button_face_mark_same.Location = new System.Drawing.Point(1, 165);
			this.button_face_mark_same.Margin = new System.Windows.Forms.Padding(1);
			this.button_face_mark_same.Name = "button_face_mark_same";
			this.button_face_mark_same.Size = new System.Drawing.Size(154, 21);
			this.button_face_mark_same.TabIndex = 90;
			this.button_face_mark_same.Text = "Mark Polys With Flags";
			this.button_face_mark_same.UseVisualStyleBackColor = true;
			this.button_face_mark_same.Click += new System.EventHandler(this.button_face_mark_same_Click);
			// 
			// button_face_clear
			// 
			this.button_face_clear.Location = new System.Drawing.Point(1, 119);
			this.button_face_clear.Margin = new System.Windows.Forms.Padding(1);
			this.button_face_clear.Name = "button_face_clear";
			this.button_face_clear.Size = new System.Drawing.Size(154, 21);
			this.button_face_clear.TabIndex = 89;
			this.button_face_clear.Text = "Clear Marked Properties";
			this.button_face_clear.UseVisualStyleBackColor = true;
			this.button_face_clear.Click += new System.EventHandler(this.button_face_clear_Click);
			// 
			// checklist_face
			// 
			this.checklist_face.CheckOnClick = true;
			this.checklist_face.FormattingEnabled = true;
			this.checklist_face.Location = new System.Drawing.Point(1, 23);
			this.checklist_face.Margin = new System.Windows.Forms.Padding(1);
			this.checklist_face.Name = "checklist_face";
			this.checklist_face.Size = new System.Drawing.Size(154, 94);
			this.checklist_face.TabIndex = 88;
			this.checklist_face.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checklist_face_ItemCheck);
			this.checklist_face.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checklist_face_MouseUp);
			// 
			// button_face_copy
			// 
			this.button_face_copy.Location = new System.Drawing.Point(1, 142);
			this.button_face_copy.Margin = new System.Windows.Forms.Padding(1);
			this.button_face_copy.Name = "button_face_copy";
			this.button_face_copy.Size = new System.Drawing.Size(154, 21);
			this.button_face_copy.TabIndex = 86;
			this.button_face_copy.Text = "Copy Selected To Marked";
			this.button_face_copy.UseVisualStyleBackColor = true;
			this.button_face_copy.Click += new System.EventHandler(this.button_face_copy_Click);
			// 
			// label_face
			// 
			this.label_face.BackColor = System.Drawing.SystemColors.Control;
			this.label_face.Location = new System.Drawing.Point(2, 0);
			this.label_face.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_face.Name = "label_face";
			this.label_face.Size = new System.Drawing.Size(153, 19);
			this.label_face.TabIndex = 45;
			this.label_face.Text = "POLY PROPERTIES";
			this.label_face.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.checklist_lights);
			this.panel2.Controls.Add(this.button_lights_paste);
			this.panel2.Controls.Add(this.button_lights_copy);
			this.panel2.Controls.Add(this.slider_light_rot_pitch);
			this.panel2.Controls.Add(this.slider_light_rot_yaw);
			this.panel2.Controls.Add(this.slider_light_posz);
			this.panel2.Controls.Add(this.slider_light_posy);
			this.panel2.Controls.Add(this.slider_light_posx);
			this.panel2.Controls.Add(this.label_light_flare);
			this.panel2.Controls.Add(this.label_light_color);
			this.panel2.Controls.Add(this.label_light_type);
			this.panel2.Controls.Add(this.slider_light_angle);
			this.panel2.Controls.Add(this.slider_light_range);
			this.panel2.Controls.Add(this.slider_light_intensity);
			this.panel2.Controls.Add(this.button_light_reset_rot);
			this.panel2.Controls.Add(this.button_light_reset_pos);
			this.panel2.Controls.Add(this.label_lights);
			this.panel2.Location = new System.Drawing.Point(1157, 514);
			this.panel2.Margin = new System.Windows.Forms.Padding(1);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(158, 393);
			this.panel2.TabIndex = 93;
			// 
			// checklist_lights
			// 
			this.checklist_lights.CheckOnClick = true;
			this.checklist_lights.FormattingEnabled = true;
			this.checklist_lights.Location = new System.Drawing.Point(1, 20);
			this.checklist_lights.Margin = new System.Windows.Forms.Padding(1);
			this.checklist_lights.Name = "checklist_lights";
			this.checklist_lights.Size = new System.Drawing.Size(154, 64);
			this.checklist_lights.TabIndex = 117;
			this.checklist_lights.SelectedIndexChanged += new System.EventHandler(this.checklist_lights_SelectedIndexChanged);
			this.checklist_lights.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checklist_lights_MouseUp);
			// 
			// button_lights_paste
			// 
			this.button_lights_paste.Location = new System.Drawing.Point(79, 348);
			this.button_lights_paste.Margin = new System.Windows.Forms.Padding(1);
			this.button_lights_paste.Name = "button_lights_paste";
			this.button_lights_paste.Size = new System.Drawing.Size(76, 21);
			this.button_lights_paste.TabIndex = 116;
			this.button_lights_paste.Text = "Paste Lights";
			this.button_lights_paste.UseVisualStyleBackColor = true;
			this.button_lights_paste.Click += new System.EventHandler(this.button_lights_paste_Click);
			// 
			// button_lights_copy
			// 
			this.button_lights_copy.Location = new System.Drawing.Point(1, 348);
			this.button_lights_copy.Margin = new System.Windows.Forms.Padding(1);
			this.button_lights_copy.Name = "button_lights_copy";
			this.button_lights_copy.Size = new System.Drawing.Size(76, 21);
			this.button_lights_copy.TabIndex = 115;
			this.button_lights_copy.Text = "Copy Lights";
			this.button_lights_copy.UseVisualStyleBackColor = true;
			this.button_lights_copy.Click += new System.EventHandler(this.button_lights_copy_Click);
			// 
			// slider_light_rot_pitch
			// 
			this.slider_light_rot_pitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_rot_pitch.Location = new System.Drawing.Point(5, 328);
			this.slider_light_rot_pitch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_rot_pitch.Name = "slider_light_rot_pitch";
			this.slider_light_rot_pitch.RightMouseMultiplier = 3;
			this.slider_light_rot_pitch.Size = new System.Drawing.Size(143, 19);
			this.slider_light_rot_pitch.SlideText = "Rotation Pitch";
			this.slider_light_rot_pitch.SlideTol = 3;
			this.slider_light_rot_pitch.TabIndex = 114;
			this.slider_light_rot_pitch.ToolTop = "";
			this.slider_light_rot_pitch.ValueText = "0.0";
			this.slider_light_rot_pitch.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_rot_pitch_Feedback);
			// 
			// slider_light_rot_yaw
			// 
			this.slider_light_rot_yaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_rot_yaw.Location = new System.Drawing.Point(5, 309);
			this.slider_light_rot_yaw.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_rot_yaw.Name = "slider_light_rot_yaw";
			this.slider_light_rot_yaw.RightMouseMultiplier = 3;
			this.slider_light_rot_yaw.Size = new System.Drawing.Size(143, 19);
			this.slider_light_rot_yaw.SlideText = "Rotation Yaw";
			this.slider_light_rot_yaw.SlideTol = 3;
			this.slider_light_rot_yaw.TabIndex = 113;
			this.slider_light_rot_yaw.ToolTop = "";
			this.slider_light_rot_yaw.ValueText = "0.0";
			this.slider_light_rot_yaw.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_rot_yaw_Feedback);
			// 
			// slider_light_posz
			// 
			this.slider_light_posz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_posz.Location = new System.Drawing.Point(5, 267);
			this.slider_light_posz.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_posz.Name = "slider_light_posz";
			this.slider_light_posz.RightMouseMultiplier = 4;
			this.slider_light_posz.Size = new System.Drawing.Size(143, 19);
			this.slider_light_posz.SlideText = "Position Z";
			this.slider_light_posz.SlideTol = 4;
			this.slider_light_posz.TabIndex = 112;
			this.slider_light_posz.ToolTop = "";
			this.slider_light_posz.ValueText = "0.0";
			this.slider_light_posz.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_posz_Feedback);
			// 
			// slider_light_posy
			// 
			this.slider_light_posy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_posy.Location = new System.Drawing.Point(5, 248);
			this.slider_light_posy.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_posy.Name = "slider_light_posy";
			this.slider_light_posy.RightMouseMultiplier = 4;
			this.slider_light_posy.Size = new System.Drawing.Size(143, 19);
			this.slider_light_posy.SlideText = "Position Y";
			this.slider_light_posy.SlideTol = 4;
			this.slider_light_posy.TabIndex = 111;
			this.slider_light_posy.ToolTop = "";
			this.slider_light_posy.ValueText = "0.0";
			this.slider_light_posy.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_posy_Feedback);
			// 
			// slider_light_posx
			// 
			this.slider_light_posx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_posx.Location = new System.Drawing.Point(5, 229);
			this.slider_light_posx.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_posx.Name = "slider_light_posx";
			this.slider_light_posx.RightMouseMultiplier = 4;
			this.slider_light_posx.Size = new System.Drawing.Size(143, 19);
			this.slider_light_posx.SlideText = "Position X";
			this.slider_light_posx.SlideTol = 4;
			this.slider_light_posx.TabIndex = 110;
			this.slider_light_posx.ToolTop = "";
			this.slider_light_posx.ValueText = "0.0";
			this.slider_light_posx.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_posx_Feedback);
			// 
			// label_light_flare
			// 
			this.label_light_flare.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_light_flare.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_light_flare.Location = new System.Drawing.Point(1, 107);
			this.label_light_flare.Margin = new System.Windows.Forms.Padding(0);
			this.label_light_flare.Name = "label_light_flare";
			this.label_light_flare.Size = new System.Drawing.Size(154, 19);
			this.label_light_flare.TabIndex = 109;
			this.label_light_flare.Text = "Security: NONE";
			this.label_light_flare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_light_flare.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_light_flare_MouseDown);
			// 
			// label_light_color
			// 
			this.label_light_color.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_light_color.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_light_color.Location = new System.Drawing.Point(1, 126);
			this.label_light_color.Margin = new System.Windows.Forms.Padding(0);
			this.label_light_color.Name = "label_light_color";
			this.label_light_color.Size = new System.Drawing.Size(154, 19);
			this.label_light_color.TabIndex = 108;
			this.label_light_color.Text = "Color: 1";
			this.label_light_color.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_light_color.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_light_color_MouseDown);
			// 
			// label_light_type
			// 
			this.label_light_type.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_light_type.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_light_type.Location = new System.Drawing.Point(1, 88);
			this.label_light_type.Margin = new System.Windows.Forms.Padding(0);
			this.label_light_type.Name = "label_light_type";
			this.label_light_type.Size = new System.Drawing.Size(154, 19);
			this.label_light_type.TabIndex = 107;
			this.label_light_type.Text = "Style: SPOT";
			this.label_light_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_light_type.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_light_type_MouseDown);
			// 
			// slider_light_angle
			// 
			this.slider_light_angle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_angle.Location = new System.Drawing.Point(5, 187);
			this.slider_light_angle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_angle.Name = "slider_light_angle";
			this.slider_light_angle.RightMouseMultiplier = 5;
			this.slider_light_angle.Size = new System.Drawing.Size(143, 19);
			this.slider_light_angle.SlideText = "Angle";
			this.slider_light_angle.SlideTol = 5;
			this.slider_light_angle.TabIndex = 106;
			this.slider_light_angle.ToolTop = "";
			this.slider_light_angle.ValueText = "30";
			this.slider_light_angle.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_angle_Feedback);
			// 
			// slider_light_range
			// 
			this.slider_light_range.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_range.Location = new System.Drawing.Point(5, 168);
			this.slider_light_range.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_range.Name = "slider_light_range";
			this.slider_light_range.RightMouseMultiplier = 5;
			this.slider_light_range.Size = new System.Drawing.Size(143, 19);
			this.slider_light_range.SlideText = "Range";
			this.slider_light_range.SlideTol = 5;
			this.slider_light_range.TabIndex = 105;
			this.slider_light_range.ToolTop = "";
			this.slider_light_range.ValueText = "10.0";
			this.slider_light_range.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_range_Feedback);
			// 
			// slider_light_intensity
			// 
			this.slider_light_intensity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_light_intensity.Location = new System.Drawing.Point(5, 149);
			this.slider_light_intensity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_light_intensity.Name = "slider_light_intensity";
			this.slider_light_intensity.RightMouseMultiplier = 5;
			this.slider_light_intensity.Size = new System.Drawing.Size(143, 19);
			this.slider_light_intensity.SlideText = "Intensity";
			this.slider_light_intensity.SlideTol = 5;
			this.slider_light_intensity.TabIndex = 104;
			this.slider_light_intensity.ToolTop = "";
			this.slider_light_intensity.ValueText = "1.0";
			this.slider_light_intensity.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_light_intensity_Feedback);
			// 
			// button_light_reset_rot
			// 
			this.button_light_reset_rot.Location = new System.Drawing.Point(1, 287);
			this.button_light_reset_rot.Margin = new System.Windows.Forms.Padding(1);
			this.button_light_reset_rot.Name = "button_light_reset_rot";
			this.button_light_reset_rot.Size = new System.Drawing.Size(154, 21);
			this.button_light_reset_rot.TabIndex = 95;
			this.button_light_reset_rot.Text = "Reset Rotation";
			this.button_light_reset_rot.UseVisualStyleBackColor = true;
			this.button_light_reset_rot.Click += new System.EventHandler(this.button_light_reset_rot_Click);
			// 
			// button_light_reset_pos
			// 
			this.button_light_reset_pos.Location = new System.Drawing.Point(1, 207);
			this.button_light_reset_pos.Margin = new System.Windows.Forms.Padding(1);
			this.button_light_reset_pos.Name = "button_light_reset_pos";
			this.button_light_reset_pos.Size = new System.Drawing.Size(154, 21);
			this.button_light_reset_pos.TabIndex = 92;
			this.button_light_reset_pos.Text = "Reset Position";
			this.button_light_reset_pos.UseVisualStyleBackColor = true;
			this.button_light_reset_pos.Click += new System.EventHandler(this.button_light_reset_pos_Click);
			// 
			// label_lights
			// 
			this.label_lights.BackColor = System.Drawing.SystemColors.Control;
			this.label_lights.Location = new System.Drawing.Point(2, 0);
			this.label_lights.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_lights.Name = "label_lights";
			this.label_lights.Size = new System.Drawing.Size(153, 19);
			this.label_lights.TabIndex = 45;
			this.label_lights.Text = "LIGHTS";
			this.label_lights.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_geometry2
			// 
			this.label_geometry2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_geometry2.BackColor = System.Drawing.SystemColors.Control;
			this.label_geometry2.Location = new System.Drawing.Point(1321, 350);
			this.label_geometry2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_geometry2.Name = "label_geometry2";
			this.label_geometry2.Size = new System.Drawing.Size(143, 19);
			this.label_geometry2.TabIndex = 122;
			this.label_geometry2.Text = "- MORE GEOMETRY -";
			this.label_geometry2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// outputTextBox
			// 
			this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.outputTextBox.Location = new System.Drawing.Point(306, 987);
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.outputTextBox.Size = new System.Drawing.Size(851, 58);
			this.outputTextBox.TabIndex = 125;
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1466, 1047);
			this.Controls.Add(this.button_bisect_poly);
			this.Controls.Add(this.slider_bevel_width);
			this.Controls.Add(this.button_triangulate_all_nonplanar);
			this.Controls.Add(this.button_triangle_marked_nonplanar);
			this.Controls.Add(this.button_subdivide_mesh);
			this.Controls.Add(this.button_combine_verts);
			this.Controls.Add(this.button_bevel_edge);
			this.Controls.Add(this.button_duplicate_x);
			this.Controls.Add(this.button_split_poly);
			this.Controls.Add(this.button_duplicate_3way);
			this.Controls.Add(this.button_duplicate_4way);
			this.Controls.Add(this.button_duplicate_z);
			this.Controls.Add(this.button_split_edge);
			this.Controls.Add(this.button_inset_bevel);
			this.Controls.Add(this.button_extrude_marked);
			this.Controls.Add(this.button_extrude_selected);
			this.Controls.Add(this.slider_inset_bevel);
			this.Controls.Add(this.slider_inset_length);
			this.Controls.Add(this.button_mark_untextured);
			this.Controls.Add(this.label_vert_display);
			this.Controls.Add(this.label_poly_filter);
			this.Controls.Add(this.slider_smooth_angle_same);
			this.Controls.Add(this.slider_smooth_angle_diff);
			this.Controls.Add(this.outputTextBox);
			this.Controls.Add(this.button_align_vert_z);
			this.Controls.Add(this.button_align_vert_y);
			this.Controls.Add(this.button_align_vert_x);
			this.Controls.Add(this.label_geometry2);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.button_browser);
			this.Controls.Add(this.button_mark_connected);
			this.Controls.Add(this.button_mark_dup_polys);
			this.Controls.Add(this.button_paste_polys);
			this.Controls.Add(this.button_copy_marked);
			this.Controls.Add(this.button_uveditor);
			this.Controls.Add(this.button_flip_edge);
			this.Controls.Add(this.button_triangulate_vert);
			this.Controls.Add(this.button_triangulate_fan);
			this.Controls.Add(this.button_combine_two);
			this.Controls.Add(this.button_poly_flip);
			this.Controls.Add(this.slider_sizesegments);
			this.Controls.Add(this.slider_sizeheight);
			this.Controls.Add(this.slider_sizey);
			this.Controls.Add(this.slider_sizex);
			this.Controls.Add(this.button_create_cyl_z);
			this.Controls.Add(this.button_create_cyl_y);
			this.Controls.Add(this.button_create_cyl_x);
			this.Controls.Add(this.button_create_box);
			this.Controls.Add(this.button_create_quad);
			this.Controls.Add(this.label_scalemode);
			this.Controls.Add(this.label_pivotmode);
			this.Controls.Add(this.button_planarize);
			this.Controls.Add(this.button_import_replace);
			this.Controls.Add(this.button_import);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.slider_coplanar_angle);
			this.Controls.Add(this.button_merge_verts);
			this.Controls.Add(this.slider_extrude_length);
			this.Controls.Add(this.panel_texturing);
			this.Controls.Add(this.label_count_selected);
			this.Controls.Add(this.label_count_marked);
			this.Controls.Add(this.button_mark_coplanar);
			this.Controls.Add(this.label_count_total);
			this.Controls.Add(this.panel_view);
			this.Controls.Add(this.panel_grid);
			this.Controls.Add(this.label_editmode);
			this.Controls.Add(this.gl_panel);
			this.Controls.Add(this.menu_strip);
			this.MainMenuStrip = this.menu_strip;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "Editor";
			this.Text = "Overload DMesh Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
			this.Load += new System.EventHandler(this.Editor_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
			this.Resize += new System.EventHandler(this.Editor_Resize);
			this.menu_strip.ResumeLayout(false);
			this.menu_strip.PerformLayout();
			this.panel_grid.ResumeLayout(false);
			this.panel_view.ResumeLayout(false);
			this.panel_texturing.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		  private System.Windows.Forms.MenuStrip menu_strip;
		  private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		  private System.Windows.Forms.Panel gl_panel;
		  private System.Windows.Forms.Label label_editmode;
		  private System.Windows.Forms.Label label_grid_display;
		  private System.Windows.Forms.Panel panel_grid;
		  private System.Windows.Forms.Label label_grid;
		  private System.Windows.Forms.Button button_snap_marked;
		  private System.Windows.Forms.ToolTip tool_tip;
		  private System.Windows.Forms.Panel panel_view;
		  private System.Windows.Forms.Label label_view_ortho;
		  private System.Windows.Forms.Label label_view;
		  private System.Windows.Forms.Label label_view_persp;
		  private System.Windows.Forms.Label label_count_total;
		  private System.Windows.Forms.Label label_count_marked;
		  private System.Windows.Forms.Label label_count_selected;
		  private System.Windows.Forms.Label label_view_layout;
		  private System.Windows.Forms.Button button_mark_coplanar;
		  private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		  private System.Windows.Forms.Panel panel_texturing;
		  private System.Windows.Forms.Label label_textures;
		  private System.Windows.Forms.Button button_align_marked;
		  private System.Windows.Forms.Button button_texture_box_map;
		  private System.Windows.Forms.Button button_texture_planar_z;
		  private System.Windows.Forms.Button button_texture_planar_y;
		  private System.Windows.Forms.Button button_texture_planar_x;
		  private System.Windows.Forms.Button button_texture_snap4;
		  private System.Windows.Forms.Button button_texture_snap8;
		  private System.Windows.Forms.Button button_texture_default_map;
		  private System.Windows.Forms.Label label_view_dark;
		  private System.Windows.Forms.Button button_texture_show_list;
		  private System.Windows.Forms.Label label_texture_name;
		  private SliderLabel slider_grid_snap;
		  private SliderLabel slider_grid_spacing;
		  private SliderLabel slider_coplanar_angle;
		  private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		  private System.Windows.Forms.Button button_texture_center_v;
		  private System.Windows.Forms.Button button_texture_center_u;
		  private System.Windows.Forms.Label label_gimbal;
		  private System.Windows.Forms.ToolStripMenuItem recent1ToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem recent2ToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem recent3ToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem recent4ToolStripMenuItem;
		  private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		  private System.Windows.Forms.Label label_lighting;
		  private System.Windows.Forms.Button button_merge_verts;
		  private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		  private SliderLabel slider_extrude_length;
		  private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		  private SliderLabel slider_grid_width;
		  private System.Windows.Forms.Panel panel3;
		  private System.Windows.Forms.Label label_geometry;
		  private System.Windows.Forms.Button button_flip_u;
		  private System.Windows.Forms.Button button_edge_left;
		  private System.Windows.Forms.Button button_center;
		  private System.Windows.Forms.Button button_edge_right;
		  private System.Windows.Forms.Button button_rotate90;
		  private System.Windows.Forms.Button button_flip_v;
		  private System.Windows.Forms.Button button_edge_bottom;
		  private System.Windows.Forms.Button button_edge_top;
		  private System.Windows.Forms.Button button_import_replace;
		  private System.Windows.Forms.Button button_import;
		  private System.Windows.Forms.Button button_planarize;
		  private System.Windows.Forms.Label label_pivotmode;
		  private System.Windows.Forms.Label label_scalemode;
		  private System.Windows.Forms.Button button_create_quad;
		  private System.Windows.Forms.Button button_create_box;
		  private System.Windows.Forms.Button button_create_cyl_z;
		  private System.Windows.Forms.Button button_create_cyl_y;
		  private System.Windows.Forms.Button button_create_cyl_x;
		  private SliderLabel slider_sizex;
		  private SliderLabel slider_sizey;
		  private SliderLabel slider_sizeheight;
		  private SliderLabel slider_sizesegments;
		  private System.Windows.Forms.Button button_poly_flip;
		  private System.Windows.Forms.Button button_combine_two;
		  private System.Windows.Forms.Button button_triangulate_fan;
		  private System.Windows.Forms.Button button_triangulate_vert;
		  private System.Windows.Forms.Button button_flip_edge;
		  private System.Windows.Forms.Button button_uveditor;
		  private System.Windows.Forms.Button button_copy_marked;
		  private System.Windows.Forms.Button button_paste_polys;
		  private System.Windows.Forms.Button button_mark_dup_polys;
		  private System.Windows.Forms.Button button_mark_connected;
		private System.Windows.Forms.ToolStripMenuItem checkPolygonsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem forceLowResTexturesToolStripMenuItem;
		private System.Windows.Forms.Button button_browser;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem exportToOBJToolStripMenuItem;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Button button_color_paste_all;
		private System.Windows.Forms.Button button_color_copy_all;
		private System.Windows.Forms.Button button_color_paste;
		private System.Windows.Forms.Button button_color_copy;
		private SliderLabel slider_color_brightness;
		private SliderLabel slider_color_saturation;
		private SliderLabel slider_color_hue;
		private SliderLabel slider_color_blue;
		private SliderLabel slider_color_green;
		private SliderLabel slider_color_red;
		private System.Windows.Forms.Button button_color4;
		private System.Windows.Forms.Button button_color3;
		private System.Windows.Forms.Button button_color2;
		private System.Windows.Forms.Button button_color1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Button button_face_clear;
		private System.Windows.Forms.CheckedListBox checklist_face;
		private System.Windows.Forms.Button button_face_copy;
		private System.Windows.Forms.Label label_face;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button_lights_paste;
		private System.Windows.Forms.Button button_lights_copy;
		private SliderLabel slider_light_rot_pitch;
		private SliderLabel slider_light_rot_yaw;
		private SliderLabel slider_light_posz;
		private SliderLabel slider_light_posy;
		private SliderLabel slider_light_posx;
		private System.Windows.Forms.Label label_light_flare;
		private System.Windows.Forms.Label label_light_color;
		private System.Windows.Forms.Label label_light_type;
		private SliderLabel slider_light_angle;
		private SliderLabel slider_light_range;
		private SliderLabel slider_light_intensity;
		private System.Windows.Forms.Button button_light_reset_rot;
		private System.Windows.Forms.Button button_light_reset_pos;
		private System.Windows.Forms.Label label_lights;
		private System.Windows.Forms.Label label_geometry2;
		private System.Windows.Forms.Button button_align_vert_x;
		private System.Windows.Forms.Button button_align_vert_y;
		private System.Windows.Forms.Button button_align_vert_z;
		private System.Windows.Forms.TextBox outputTextBox;
		private System.Windows.Forms.Button button_face_mark_same;
		private SliderLabel slider_smooth_angle_diff;
		private SliderLabel slider_smooth_angle_same;
		private System.Windows.Forms.Label label_poly_filter;
		private System.Windows.Forms.Label label_vert_display;
		private System.Windows.Forms.Button button_mark_untextured;
		private SliderLabel slider_inset_length;
		private SliderLabel slider_inset_bevel;
		private System.Windows.Forms.Button button_extrude_selected;
		private System.Windows.Forms.Button button_extrude_marked;
		private System.Windows.Forms.Button button_inset_bevel;
		private System.Windows.Forms.Button button_split_edge;
		private System.Windows.Forms.Button button_duplicate_3way;
		private System.Windows.Forms.Button button_duplicate_4way;
		private System.Windows.Forms.Button button_duplicate_z;
		private System.Windows.Forms.CheckedListBox checklist_lights;
		private System.Windows.Forms.Button button_split_poly;
		private System.Windows.Forms.Button button_duplicate_x;
		private System.Windows.Forms.Button button_bevel_edge;
		private System.Windows.Forms.Button button_combine_verts;
		private System.Windows.Forms.ToolStripMenuItem showShortcutsToolStripMenuItem;
		private System.Windows.Forms.Button button_uv_quarter4;
		private System.Windows.Forms.Button button_uv_button3;
		private System.Windows.Forms.Button button_uv_quarter2;
		private System.Windows.Forms.Button button_uv_quarter1;
		private System.Windows.Forms.ToolStripMenuItem filePathLocationsToolStripMenuItem;
		private System.Windows.Forms.Button button_subdivide_mesh;
		private System.Windows.Forms.Button button_triangle_marked_nonplanar;
		private System.Windows.Forms.Button button_triangulate_all_nonplanar;
		private System.Windows.Forms.Button button_scale_dn1;
		private System.Windows.Forms.Button button_scale_up1;
		private System.Windows.Forms.Button button_scale_dn2;
		private System.Windows.Forms.Button button_scale_up2;
		private SliderLabel slider_bevel_width;
		private System.Windows.Forms.ToolStripMenuItem autoCopyFaceFlagsToMarkedToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private System.Windows.Forms.Button button_bisect_poly;
	}
}

