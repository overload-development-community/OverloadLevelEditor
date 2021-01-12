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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OverloadLevelEditor;
using Microsoft.VisualBasic.Devices;

// INSTALLATION NOTE: Install OpenTK (http://www.opentk.com/) before using/editing

// Mirror selected (X/Y/Z)
// Smoothing groups

// Edit colors
// Edit lights
// Edit vert colors
//


// Lathe? (extrude with verts/polys, use tunnel builder like modification)
// - Probably not needed for items of this scale

// Add option to disable grid snap (only really relevant to mouse move/rotate/scale)

// Enable pivot functionality (selected poly, selected vert, etc)


namespace OverloadLevelEditor
{
	public enum ViewType
	{
		TOP,
		RIGHT,
		FRONT,
		PERSP,

		NUM,
	}

	public partial class Editor : Form
	{
		public TextureManager tm_decal;

		public TextureList texture_list;
		public UVEditor uv_editor;
		public TunnelBuilder tunnel_builder;
		public DMeshBrowser dmesh_browser;
		public Label[] label_viewport = new Label[(int)ViewType.NUM];
		public GLView[] gl_view = new GLView[(int)ViewType.NUM];

		public DMesh m_dmesh;
		public DMesh m_dmesh_buffer;
		public string m_filepath_decals = "";
		public string m_filepath_current_decal = "";
		public string m_filepath_decal_textures = "";
		public string m_filepath_level_textures = "";
		public string m_filepath_root = "C:\\projects\\overload\\Editor";

		public Editor()
		{
			InitializeComponent();
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			m_dmesh = new DMesh("default_dmesh");
			tm_decal = new TextureManager(this);
			texture_list = new TextureList(this);
			uv_editor = new UVEditor(this);
			tunnel_builder = new TunnelBuilder(this);
			dmesh_browser = new DMeshBrowser(this);
			NewDMesh();
			UndoInit();
			KeyPreview = true;

			// Init views and view labels
			Size sz = gl_panel.Size;
			sz.Height -= 5;
			sz.Width -= 5;

			// Create GL views and labels
			for (int i = 0; i < (int)ViewType.NUM; i++) {
				gl_view[i] = new GLView((ViewType)i, this);
				gl_view[i].Parent = gl_panel;
				switch (i) {
					case 0: gl_view[i].Location = new Point(0, 0); break;
					case 1: gl_view[i].Location = new Point(sz.Width / 2, 0); break;
					case 2: gl_view[i].Location = new Point(0, sz.Height / 2); break;
					case 3: gl_view[i].Location = new Point(sz.Width / 2, sz.Height / 2); break;
				}
				gl_view[i].Size = new Size(sz.Width / 2, sz.Height / 2);
				gl_view[i].Show();
			}

			for (int i = 0; i < (int)ViewType.NUM; i++) {
				label_viewport[i] = new Label();
				label_viewport[i].Parent = gl_panel;
				label_viewport[i].Location = new Point(gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2);
				label_viewport[i].AutoSize = true;
				label_viewport[i].Text = ((ViewType)i).ToString();
				label_viewport[i].BackColor = GLView.C_bg;
				label_viewport[i].ForeColor = Color.White;
				label_viewport[i].Show();
				gl_panel.Controls.Add(label_viewport[i]);
				label_viewport[i].BringToFront();
			}

			// Could make this save/load?
			WindowState = FormWindowState.Maximized;

			ResetViews();

			UpdateDirectories();
			LoadPreferences();
			UpdateOptionLabels();
			RefreshGrid();
			ResetViews();
			InitChecklists();

			ComputerInfo ci = new ComputerInfo();
			ulong total_memory = (ulong)ci.TotalPhysicalMemory;
			bool low_mem = total_memory < 5000000000; // 5 GB
			if (m_low_res_force || low_mem) {
				m_low_res_textures = true;
			}
			tm_decal.LoadTexturesInDir(m_filepath_decal_textures, false, false);
			tm_decal.LoadTexturesInDir(m_filepath_level_textures, false, false);
			texture_list.InitImageLists();
			texture_list.Hide();

			uv_editor.Hide();
			tunnel_builder.Hide();
		}

		public void RefreshGeometry(bool refresh_editor = true)
		{
			// This is a dumb way of doing things, but until it's too slow, it should be ok
			m_dmesh.RecalculateNormals();
			gl_view[0].BuildDMeshAll();
			if (refresh_editor) {
				this.Refresh();
			}
			UpdateFaceCheckListFromSelected();
			UpdateEditorColorsFromSelected();
			UpdateLightCheckList();
			UpdateLightProperties(m_dmesh.light[m_selected_light]);
			
			UpdateCountLabels();
			UpdateTextureLabels();
		}

		public void CycleVisibilityType()
		{
			m_vis_type = (VisibilityType)(((int)m_vis_type + 1) % (int)VisibilityType.NUM);
			AddOutputText("Cycle visibility of polygons, now showing: " + m_vis_type.ToString());
			UpdateOptionLabels();
			RefreshGeometry(true);
		}

		public void ToggleVertDisplay()
		{
			m_vert_display = (VertDisplay)(((int)m_vert_display + 1) % (int)VertDisplay.NUM);
			UpdateOptionLabels();
			RefreshGeometry(true);
		}

		private void gl_panel_Resize(object sender, EventArgs e)
		{
			ResetViews();
		}

		private void Editor_Resize(object sender, EventArgs e)
		{
			if (!IsViewInitialized())
				return;

			for (int i = 0; i < (int)ViewType.NUM; i++) {
				gl_view[i].SetupViewport();
				gl_view[i].Invalidate();
			}
		}

		private void Editor_KeyDown(object sender, KeyEventArgs e)
		{
			// See EditorKeyboard.cs (which is triggered by GLView)
		}

		private void label_grid_display_MouseDown(object sender, MouseEventArgs e)
		{
			CycleGridDisplay();
		}

		private void label_drag_select_MouseDown(object sender, MouseEventArgs e)
		{
			CycleDragMode();
		}

		private void label_side_select_MouseDown(object sender, MouseEventArgs e)
		{
			CycleSideSelect();
		}

		private void button_snap_marked_Click(object sender, EventArgs e)
		{
			SnapMarkedElementsToGrid();
		}

		private void label_view_ortho_MouseDown(object sender, MouseEventArgs e)
		{
			CycleViewModeOrtho(e.Button == MouseButtons.Right);
		}

		private void label_view_persp_MouseDown(object sender, MouseEventArgs e)
		{
			CycleViewModePersp(e.Button == MouseButtons.Right);
		}

		private void label_view_layout_MouseDown(object sender, MouseEventArgs e)
		{
			CycleViewLayout(e.Button == MouseButtons.Right);
		}

		private void button_mark_coplanar_Click(object sender, EventArgs e)
		{
			MarkCoplanarPolys();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RestoreUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RestoreRedo();
		}

		private void button_texture_box_map_Click(object sender, EventArgs e)
		{
			//m_level.UVBoxMapMarkedSides();
			RefreshGeometry();
		}

		private void button_texture_planar_x_Click(object sender, EventArgs e)
		{
			m_dmesh.UVPlanarMapMarkedSides(Axis.X);
			RefreshGeometry();
		}

		private void button_texture_planar_y_Click(object sender, EventArgs e)
		{
			m_dmesh.UVPlanarMapMarkedSides(Axis.Y);
			RefreshGeometry();
		}

		private void button_texture_planar_z_Click(object sender, EventArgs e)
		{
			m_dmesh.UVPlanarMapMarkedSides(Axis.Z);
			RefreshGeometry();
		}

		private void button_texture_snap4_Click(object sender, EventArgs e)
		{
			m_dmesh.UVSnapToFraction(32);
			RefreshGeometry();
		}

		private void button_texture_snap8_Click(object sender, EventArgs e)
		{
			m_dmesh.UVSnapToFraction(8);
			RefreshGeometry();
		}

		private void button_texture_default_map_Click(object sender, EventArgs e)
		{
			m_dmesh.UVDefaultMapMarkedPolys();
			RefreshGeometry();
		}

		private void button_align_marked_Click(object sender, EventArgs e)
		{
			m_dmesh.UVAlignToPoly();
			RefreshGeometry();
		}

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			//decal_list.SaveAllDecalMeshesToDir(m_filepath_decals);
			SavePreferences();
			e.Cancel = false;
		}

		private void button_mark_walls_Click(object sender, EventArgs e)
		{
			//MarkWallSides();
		}

		private void label_view_dark_MouseDown(object sender, MouseEventArgs e)
		{
			CycleBGColor();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (m_dmesh.dirty) {
				DialogResult dr = MessageBox.Show("Level has changed.  Save it before starting a new level?", "Save Level?", MessageBoxButtons.YesNoCancel);
				if (dr == DialogResult.Yes) {
					saveToolStripMenuItem_Click(this, e);
				} else if (dr == DialogResult.Cancel) {
					// Nevermind!
					return;
				}
			}

			NewDMesh();
			RefreshGeometry();
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (m_dmesh.dirty) {
				DialogResult dr = MessageBox.Show("DMesh has changed.  Save it before loading another decal?", "Save DMesh?", MessageBoxButtons.YesNoCancel);
				if (dr == DialogResult.Yes) {
					saveToolStripMenuItem_Click(this, e);
				} else if (dr == DialogResult.Cancel) {
					// Nevermind!
					return;
				}
			}

			using (var od = new OpenFileDialog()) {
				od.AddExtension = true;
				od.CheckFileExists = true;
				od.CheckPathExists = true;
				od.DefaultExt = ".dmesh";
				od.Filter = "Overload DMesh (*.dmesh) | *.dmesh";
				od.Multiselect = false;
				od.Title = "Load an Overload DMesh";
				od.InitialDirectory = m_filepath_decals;

				var res = od.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;
				LoadDecalMesh(m_dmesh, od.FileName);

				RefreshGeometry();
			}
		}

		public void CheckMeshBeforeSaving()
		{
			m_dmesh.RemoveUnusedVerts();
			m_dmesh.DeconvertPolysToTris();

			string issues;
			if (m_dmesh.CheckAndCleanMeshIssues(false, out issues)) {
				AddOutputText(issues);
				Utility.DebugPopup("Mesh has issues.  See output window for details.");
			}
		}

		public void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckMeshBeforeSaving();

			// Save a backup
			DateTime date = DateTime.Now;
			string backup = m_filepath_decals +
				"\\Backup\\DMeshBackup" +
				"_" + date.Month +
				"_" + date.Day +
				"_" + (date.Hour * 3600 + date.Minute * 60 + date.Second).ToString() +
				".dmesh";
			SaveDecalMesh(m_dmesh, backup, true);

			if (this.m_filepath_current_decal == null) {
				saveasToolStripMenuItem_Click(sender, e);
			} else {
				SaveDecalMesh(m_dmesh, this.m_filepath_current_decal);
			}
		}

		private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckMeshBeforeSaving();

			using (var sd = new SaveFileDialog()) {
				sd.AddExtension = true;
				sd.CheckPathExists = true;
				sd.DefaultExt = ".dmesh";
				sd.FileName = this.m_filepath_current_decal;
				sd.Filter = "Overload DMesh (*.dmesh) | *.dmesh";
				sd.OverwritePrompt = true;
				sd.Title = "Save an Overload DMesh";
				sd.InitialDirectory = m_filepath_decals;

				var res = sd.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;
				SaveDecalMesh(m_dmesh, sd.FileName);
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (m_dmesh.dirty) {
				DialogResult dr = MessageBox.Show("DMesh has changed.  Save it before closing the editor?", "Save DMesh?", MessageBoxButtons.YesNoCancel);
				if (dr == DialogResult.Yes) {
					saveToolStripMenuItem_Click(this, EventArgs.Empty);
				} else if (dr == DialogResult.Cancel) {
					// Nevermind!
					return;
				}
			}

			texture_list.Owner = null;
			tunnel_builder.Owner = null;
			Close();
		}

		private void label_editmode_MouseDown(object sender, MouseEventArgs e)
		{
			CycleEditMode(e.Button == MouseButtons.Right);
		}

		private void slider_coplanar_angle_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeCoplanarAngle(e.Increment * 5);
		}

		private void slider_grid_spacing_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeGridSpacing(e.Increment);
		}

		private void slider_grid_snap_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeGridSnap(e.Increment);
		}

		private void button_texture_center_u_Click(object sender, EventArgs e)
		{
			m_dmesh.UVCenterU();
			RefreshGeometry();
		}

		private void button_texture_center_v_Click(object sender, EventArgs e)
		{
			m_dmesh.UVCenterV();
			RefreshGeometry();
		}

		private void button_texture_show_list_Click(object sender, EventArgs e)
		{
			texture_list.TopLevel = true;
			if (!texture_list.Visible) {
				texture_list.Show(this);
				texture_list.Location = m_tex_list_loc;
			}
		}

		private void label_gimbal_MouseDown(object sender, MouseEventArgs e)
		{
			CycleGimbalDisplay();
		}

		private void recent1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(0);
		}

		private void recent2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(1);
		}

		private void recent3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(2);
		}

		private void recent4ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(3);
		}

		private void label_lighting_MouseDown(object sender, MouseEventArgs e)
		{
			CycleLightingType();
		}

		private void button_merge_verts_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Merge overlapping verts");
			m_dmesh.MergeOverlappingVerts();
			m_dmesh.RemoveUnusedVerts();
			RefreshGeometry();
		}

		private void button_tunnel_builder_Click(object sender, EventArgs e)
		{
			tunnel_builder.TopLevel = true;
			if (!tunnel_builder.Visible) {
				tunnel_builder.Show(this);
				tunnel_builder.Location = m_tunnel_builder_loc;
			}
		}

		private void slider_extrude_length_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeExtrudeLength(e.Increment);
		}

		private void slider_grid_width_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeGridWidth(e.Increment);
		}

		private void button_flip_u_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Flip mesh");
			m_dmesh.FlipMesh(-1f, 1f);
			RefreshGeometry();
		}

		private void button_flip_v_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Flip mesh");
			m_dmesh.FlipMesh(1f, -1f);
			RefreshGeometry();
		}

		private void button_center_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Center mesh");
			m_dmesh.CenterMesh();
			RefreshGeometry();
		}

		private void button_rotate90_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Rotate mesh");
			m_dmesh.RotateMesh90();
			RefreshGeometry();
		}

		private void button_edge_top_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Align mesh");
			bool[] edges = { false, false, false, true };
			m_dmesh.AlignMeshEdges(edges);
			RefreshGeometry();
		}

		private void button_edge_left_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Align mesh");
			bool[] edges = { false, false, true, false };
			m_dmesh.AlignMeshEdges(edges);
			RefreshGeometry();
		}

		private void button_edge_right_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Align mesh");
			bool[] edges = { false, true, false, false };
			m_dmesh.AlignMeshEdges(edges);
			RefreshGeometry();
		}

		private void button_edge_bottom_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Align mesh");
			bool[] edges = { true, false, false, false };
			m_dmesh.AlignMeshEdges(edges);
			RefreshGeometry();
		}

		private void button_planarize_Click(object sender, EventArgs e)
		{
			m_dmesh.CombineMatchingPolys();
			RefreshGeometry();
		}

		private void label_pivot_MouseDown(object sender, MouseEventArgs e)
		{
			CyclePivotMode(e.Button == MouseButtons.Right);
		}

		private void label_scalemode_MouseDown(object sender, MouseEventArgs e)
		{
			CycleScaleMode(e.Button == MouseButtons.Right);
		}

		private void button_create_quad_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Create quad");
			m_dmesh.ClearAllMarked();
			Vector2 v = new Vector2(m_size_x, m_size_y);
			m_dmesh.CreateQuad(v);
			RefreshGeometry();
		}

		private void button_create_box_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Create box");
			m_dmesh.ClearAllMarked();
			Vector3 v = new Vector3(m_size_x, m_size_height, m_size_y);
			m_dmesh.CreateBox(v);
			RefreshGeometry();
		}

		private void button_create_cyl_x_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Create cylinder");
			m_dmesh.ClearAllMarked();
			m_dmesh.CreateCylinder(Axis.X, m_size_x, m_size_height, m_size_segments);
			RefreshGeometry();
		}

		private void slider_sizex_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSizeX(e.Increment);
		}

		private void slider_sizey_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSizeY(e.Increment);
		}

		private void slider_sizeheight_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSizeHeight(e.Increment);
		}

		private void slider_sizesegments_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSizeSegments(e.Increment);
		}

		private void button_create_cyl_y_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Create cylinder");
			m_dmesh.ClearAllMarked();
			m_dmesh.CreateCylinder(Axis.Y, m_size_x, m_size_height, m_size_segments);
			RefreshGeometry();
		}

		private void button_create_cyl_z_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Create cylinder");
			m_dmesh.ClearAllMarked();
			m_dmesh.CreateCylinder(Axis.Z, m_size_x, m_size_height, m_size_segments);
			RefreshGeometry();
		}

		private void button_poly_flip_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Flip normal");
			m_dmesh.ReverseMarkedPolys();
			RefreshGeometry();
		}

		private void button_combine_two_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Combine polys");
			m_dmesh.CombineTwoPolys();
			RefreshGeometry();
		}

		private void button_triangulate_fan_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Triangulate poly");
			m_dmesh.PolysTriangulateFan();
			RefreshGeometry();
		}

		private void button_triangulate_vert_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Triangulate poly");
			m_dmesh.PolysTriangulateVert();
			RefreshGeometry();
		}

		private void button_flip_edge_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Switch triangle edge");
			m_dmesh.SwitchTriEdge();
			RefreshGeometry();
		}

		private void button_uveditor_Click(object sender, EventArgs e)
		{
			uv_editor.TopLevel = true;
			if (!uv_editor.Visible) {
				uv_editor.Show(this);
				//uv_editor.Location = m_tunnel_builder_loc;
			}
		}

		private void button_copy_marked_Click(object sender, EventArgs e)
		{
			CopyMarkedPolys();
		}

		private void button_paste_polys_Click(object sender, EventArgs e)
		{
			PasteBufferPolys();
		}

		private void button_mark_dup_polys_Click(object sender, EventArgs e)
		{
			MarkDuplicatePolys();
		}

		private void button_mark_connected_Click(object sender, EventArgs e)
		{
			MarkConnectedPolys();
		}

		private void checkPolygonsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_dmesh.DeconvertPolysToTris();

			string issues;
            if(m_dmesh.CheckAndCleanMeshIssues(false, out issues)) { 
				AddOutputText(issues);
				Utility.DebugPopup("Mesh has issues.  See output window for details.");
			} else {
				Utility.DebugPopup("No issues found.", "Message");
			}

		}

		private void forceLowResTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			forceLowResTexturesToolStripMenuItem.Checked = !forceLowResTexturesToolStripMenuItem.Checked;
			m_low_res_force = forceLowResTexturesToolStripMenuItem.Checked;
      }

		private void button_browser_Click(object sender, EventArgs e)
		{
			dmesh_browser.TopLevel = true;
			if (!dmesh_browser.Visible) {
				dmesh_browser.Show(this);
			}
      }

		private void exportToOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var sd = new SaveFileDialog()) {
				sd.AddExtension = true;
				sd.CheckPathExists = true;
				sd.DefaultExt = ".obj";
				sd.FileName = this.m_filepath_current_decal;
				sd.Filter = "OBJ file (*.obj) | *.obj";
				sd.OverwritePrompt = true;
				sd.Title = "Export to OBJ Format";
				sd.InitialDirectory = m_filepath_decals;

				var res = sd.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;
				ExportDecalMeshToOBJ(m_dmesh, sd.FileName);
			}
		}

		private void button_color1_Click(object sender, EventArgs e)
		{
			SetButtonColorActive(button_color1, 0);
		}

		private void button_color2_Click(object sender, EventArgs e)
		{
			SetButtonColorActive(button_color2, 1);
		}

		private void button_color3_Click(object sender, EventArgs e)
		{
			SetButtonColorActive(button_color3, 2);
		}

		private void button_color4_Click(object sender, EventArgs e)
		{
			SetButtonColorActive(button_color4, 3);
		}

		private void button_color_copy_Click(object sender, EventArgs e)
		{
			CopySelectedColor();
		}

		private void button_color_paste_Click(object sender, EventArgs e)
		{
			PasteToSelectedColor();
		}

		private void button_color_copy_all_Click(object sender, EventArgs e)
		{
			CopyAllColors();
		}

		private void button_color_paste_all_Click(object sender, EventArgs e)
		{
			PasteAllColors();
		}

		private void checklist_face_MouseUp(object sender, MouseEventArgs e)
		{
			UpdateFaceFlagsFromCheckList();
		}

		private void label_light_type_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.CycleStyle();
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		private void label_light_flare_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.CycleFlare();
				UpdateLightProperties(dl);
			}
		}

		private void label_light_color_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.CycleColorIndex();
				UpdateLightProperties(dl);
			}
		}

		private void slider_light_intensity_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				float inc = (dl.intensity < 2f ? 0.1f : 0.5f);
				dl.intensity = Utility.SnapValue(dl.intensity + e.Increment * inc, inc);
				dl.intensity = Utility.Clamp(dl.intensity, 0f, 50f);
				UpdateLightProperties(dl);
			}
		}

		private void slider_light_range_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.range = Utility.SnapValue(dl.range + e.Increment * 0.5f, 0.5f);
				dl.range = Utility.Clamp(dl.range, 0f, 50f);
				UpdateLightProperties(dl);
			}
		}

		private void slider_light_angle_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.angle = Utility.SnapValue(dl.angle + e.Increment * 5, 5f);
				dl.angle = Utility.Clamp(dl.angle, 0f, 160f);
				UpdateLightProperties(dl);
			}
		}

		private void checklist_lights_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_selected_light = Utility.Clamp(checklist_lights.SelectedIndex, 0, 3);
			if (m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				UpdateLightProperties(dl);
				RefreshGeometry(false);
			}
		}

		private void checklist_lights_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_dmesh != null) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					m_dmesh.light[i].enabled = checklist_lights.GetItemChecked(i);
				}
				RefreshGeometry(true);
			}
		}

		private void checklist_face_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			// Clear the other color flags if setting a color
			// WARNING: This is a hack that needs to be updated if FaceFlags are changed
			if (e.Index > 1 && e.NewValue == CheckState.Checked) {
				checklist_face.SetItemChecked(2, false);
				checklist_face.SetItemChecked(3, false);
				checklist_face.SetItemChecked(4, false);
				checklist_face.SetItemChecked(5, false);
			}
		}

		public void UpdateMarkedFaceFlags(int flags)
		{
			for (int i = 0; i < m_dmesh.polygon.Count; i++) {
				if (m_dmesh.polygon[i].marked) {
					m_dmesh.polygon[i].flags = flags;
				}
			}
		}

		private void button_face_clear_Click(object sender, EventArgs e)
		{
			ClearMarkedFaceFlags();
		}

		public bool[] m_face_buffer;

		private void button_face_copy_Click(object sender, EventArgs e)
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				CopyMarkedToFaceFlags(m_dmesh.polygon[m_dmesh.selected_poly].flags);
				RefreshGeometry(true);
			}
		}

		private void button_face_paste_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checklist_face.Items.Count; i++) {
				checklist_face.SetItemChecked(i, m_face_buffer[i]);
			}
			UpdateFaceFlagsFromCheckList();
		}

		private void slider_color_red_Feedback(object sender, SliderLabelArgs e)
		{
			byte R = (byte)Utility.Clamp(m_editor_colors[m_selected_color].R + e.Increment, 0, 255);
			Color c = m_editor_colors[m_selected_color];
			m_editor_colors[m_selected_color] = Color.FromArgb(R, c.G, c.B);
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			NotifyEditorColorUpdated(m_selected_color);
		}

		private void slider_color_green_Feedback(object sender, SliderLabelArgs e)
		{
			byte G = (byte)Utility.Clamp(m_editor_colors[m_selected_color].G + e.Increment, 0, 255);
			Color c = m_editor_colors[m_selected_color];
			m_editor_colors[m_selected_color] = Color.FromArgb(c.R, G, c.B);
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			NotifyEditorColorUpdated(m_selected_color);
		}

		private void slider_color_blue_Feedback(object sender, SliderLabelArgs e)
		{
			byte B = (byte)Utility.Clamp(m_editor_colors[m_selected_color].B + e.Increment, 0, 255);
			Color c = m_editor_colors[m_selected_color];
			m_editor_colors[m_selected_color] = Color.FromArgb(c.R, c.G, B);
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			NotifyEditorColorUpdated(m_selected_color);
		}

		private void slider_color_hue_Feedback(object sender, SliderLabelArgs e)
		{
			int h = Utility.Clamp((int)(m_hsb_selected.h * 360f + 0.1f) + e.Increment, 0, 360);
			m_hsb_selected.h = (float)h / 360f;
			UpdateColorFromHSB();
			UpdateSlidersFromSelectedColor();
		}

		private void slider_color_saturation_Feedback(object sender, SliderLabelArgs e)
		{
			int s = Utility.Clamp((int)(m_hsb_selected.s * 100f + 0.1f) + e.Increment, 0, 100);
			m_hsb_selected.s = (float)s / 100f;
			UpdateColorFromHSB();
			UpdateSlidersFromSelectedColor();
		}

		private void slider_color_brightness_Feedback(object sender, SliderLabelArgs e)
		{
			int b = Utility.Clamp((int)(m_hsb_selected.b * 100f + 0.1f) + e.Increment, 0, 100);
			m_hsb_selected.b = (float)b / 100f;
			UpdateColorFromHSB();
			UpdateSlidersFromSelectedColor();
		}

		private void button_light_reset_pos_Click(object sender, EventArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.position = Vector3.Zero;
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		private void button_light_reset_rot_Click(object sender, EventArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.rot_yaw = 0f;
				dl.rot_pitch = 0f;
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		public float POS_SNAP = 0.03125f;

		private void slider_light_posx_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.position.X = Utility.SnapValue(dl.position.X + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		// NOTE: Z and Y are flipped on purpose (because of how decals are oriented vs. displayed)
		private void slider_light_posy_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.position.Z = Utility.SnapValue(dl.position.Z + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		// NOTE: Z and Y are flipped on purpose (because of how decals are oriented vs. displayed)
		private void slider_light_posz_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.position.Y = Utility.SnapValue(dl.position.Y + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		public float ROT_SNAP = Utility.RAD_180 / 36f; // 5 degrees

		private void slider_light_rot_yaw_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.rot_yaw = Utility.SnapValue(dl.rot_yaw + e.Increment * ROT_SNAP, ROT_SNAP);
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		private void slider_light_rot_pitch_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_dmesh != null) {
				DLight dl = m_dmesh.light[m_selected_light];
				dl.rot_pitch = Utility.SnapValue(dl.rot_pitch + e.Increment * ROT_SNAP, ROT_SNAP);
				UpdateLightProperties(dl);
				RefreshGeometry();
			}
		}

		private void button_lights_copy_Click(object sender, EventArgs e)
		{
			CopyLights();
		}

		private void button_lights_paste_Click(object sender, EventArgs e)
		{
			PasteLights();
			RefreshGeometry();
		}

		private void button_align_vert_x_Click(object sender, EventArgs e)
		{
			if (m_dmesh.selected_vert > -1) {
				m_dmesh.AlignVerts(m_dmesh.vertex[m_dmesh.selected_vert], Axis.X);
				RefreshGeometry();
			}
		}

		private void button_align_vert_y_Click(object sender, EventArgs e)
		{
			if (m_dmesh.selected_vert > -1) {
				m_dmesh.AlignVerts(m_dmesh.vertex[m_dmesh.selected_vert], Axis.Y);
				RefreshGeometry();
			}
		}

		private void button_align_vert_z_Click(object sender, EventArgs e)
		{
			if (m_dmesh.selected_vert > -1) {
				m_dmesh.AlignVerts(m_dmesh.vertex[m_dmesh.selected_vert], Axis.Z);
				RefreshGeometry();
			}
		}

		private void button_face_mark_same_Click(object sender, EventArgs e)
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				MarkFacesWithSameFlags(m_dmesh.polygon[m_dmesh.selected_poly].flags);
				RefreshGeometry();
			}
		}

		private void slider_smooth_angle_diff_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSmoothAngleDiff(e.Increment * 5);
		}

		private void slider_smooth_angle_same_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeSmoothAngleSame(e.Increment * 5);
		}

		private void label_poly_filter_MouseDown(object sender, MouseEventArgs e)
		{
			CycleVisibilityType();
		}

		private void label_vert_display_MouseDown(object sender, MouseEventArgs e)
		{
			ToggleVertDisplay();
		}

		private void button_mark_untextured_Click(object sender, EventArgs e)
		{
			MarkFacesInvalidTexture();
			RefreshGeometry();
		}

		private void slider_inset_length_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeInsetLength(e.Increment);
		}

		private void slider_inset_bevel_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeInsetPct(e.Increment);
		}

		private void button_extrude_selected_Click(object sender, EventArgs e)
		{
			ExtrudeSelectedPoly();
		}

		private void button_extrude_marked_Click(object sender, EventArgs e)
		{
			if (m_edit_mode == EditMode.POLY) {
				ExtrudeMarkedPolys();
			} else {
				ExtrudeMarkedVerts();
			}
		}

		private void button_inset_bevel_Click(object sender, EventArgs e)
		{
			InsetBevelMarkedPolys();
		}

		private void button_split_edge_Click(object sender, EventArgs e)
		{
			SplitEdge();
		}

		private void button_split_poly_Click(object sender, EventArgs e)
		{
			SplitPolygon();
		}

		private void button_bisect_poly_Click(object sender, EventArgs e)
		{
			BisectPolygon();
		}

		private void button_duplicate_x_Click(object sender, EventArgs e)
		{
			DuplicateMarkedPolysAlongAxis(true);
		}

		private void button_duplicate_z_Click(object sender, EventArgs e)
		{
			DuplicateMarkedPolysAlongAxis(false);
		}

		private void button_duplicate_3way_Click(object sender, EventArgs e)
		{
			DuplicateMarkedPolysAroundYAxis(3);
		}

		private void button_duplicate_4way_Click(object sender, EventArgs e)
		{
			DuplicateMarkedPolysAroundYAxis(4);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			BevelEdge();
		}

		private void button_combine_verts_Click(object sender, EventArgs e)
		{
			CombineMarkedVerts();
		}

		private void showShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShortcutKeys skform = new ShortcutKeys();
			skform.Show();
		}

		private void button_uv_quarter1_Click(object sender, EventArgs e)
		{
			FitUVsToQuarter(0.5f, 0.5f);
		}

		private void button_uv_quarter2_Click(object sender, EventArgs e)
		{
			FitUVsToQuarter(0f, 0.5f);
		}

		private void button_uv_button3_Click(object sender, EventArgs e)
		{
			FitUVsToQuarter(0.5f, 0f);
		}

		private void button_uv_quarter4_Click(object sender, EventArgs e)
		{
			FitUVsToQuarter(0f, 0f);
		}

		private void filePathLocationsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PathLocation pl_form = new PathLocation(this);
			pl_form.Show();
		}

		private void button_subdivide_mesh_Click(object sender, EventArgs e)
		{
			SubdivideMarkedPolys();
		}

		private void button_triangle_marked_nonplanar_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Triangulate marked polys that are non-planar");
			m_dmesh.PolysTriangulateNonPlanar();
			RefreshGeometry();
		}

		private void button_triangulate_all_nonplanar_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Triangulate all polys that are non-planar");
			m_dmesh.MarkAllPolys();
			m_dmesh.PolysTriangulateNonPlanar();
			RefreshGeometry();
		}

		private void button_scale_dn1_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Scale UVs");
			m_dmesh.ScaleUVs(1f / 1.1f);
			RefreshGeometry();
		}

		private void button_scale_up1_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Scale UVs");
			m_dmesh.ScaleUVs(1.1f);
			RefreshGeometry();
		}

		private void button_scale_dn2_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Scale UVs");
			m_dmesh.ScaleUVs(0.5f);
			RefreshGeometry();
		}

		private void button_scale_up2_Click(object sender, EventArgs e)
		{
			SaveStateForUndo("Scale UVs");
			m_dmesh.ScaleUVs(2f);
			RefreshGeometry();
		}

		private void slider_bevel_width_Feedback(object sender, SliderLabelArgs e)
		{
			ChangeBevelWidth(e.Increment);
		}

		private void autoCopyFaceFlagsToMarkedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			auto_copy_face_flags = !auto_copy_face_flags;
			autoCopyFaceFlagsToMarkedToolStripMenuItem.Checked = auto_copy_face_flags;
		}

		private void button_import_Click(object sender, EventArgs e)
		{
			if (m_dmesh.dirty) {
				DialogResult dr = MessageBox.Show("DMesh has changed.  Save it before importing OBJ file?", "Save DMesh?", MessageBoxButtons.YesNoCancel);
				if (dr == DialogResult.Yes) {
					saveToolStripMenuItem_Click(this, e);
				} else if (dr == DialogResult.Cancel) {
					// Nevermind!
					return;
				}
			}

			using (OpenFileDialog od = new OpenFileDialog()) {
				od.AddExtension = true;
				od.CheckFileExists = true;
				od.CheckPathExists = true;
				od.DefaultExt = ".obj";
				od.Filter = "OBJ mesh files (*.obj) | *.obj";
				od.Multiselect = false;
				od.Title = "Import an OBJ mesh file";

				var res = od.ShowDialog();
				if (res != DialogResult.OK)
					return;
				ImportOBJ(m_dmesh, od.FileName);

				RefreshGeometry();
			}
		}
	}
}
