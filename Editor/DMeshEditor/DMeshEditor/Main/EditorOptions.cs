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
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OverloadLevelEditor
{
	public enum GridDisplay
	{
		OFF,
		ORTHO,
		ALL,

		NUM,
	}

	public enum EditMode
	{
		POLY,
		VERT,
		
		NUM,
	}

	public enum ScaleMode
	{
		VIEW_X,
		VIEW_Y,
		VIEW_XY,
		ALL,
		FREE_XY,

		NUM,
	}

	public enum PivotMode
	{
		ORIGIN,
		ALL_MARKED,
		SEL_POLY,
		SEL_VERT,

		NUM,
	}

	public enum DragMode
	{
		ANY,
		ALL,

		NUM,
	}

	public enum SideSelect
	{
		VISIBLE,
		SOLID,
		FRONT,
		ALL,

		NUM,
	}

	public enum ViewMode
	{
		WIREFRAME,
		SOLID,
		WIRE_SOLID,
		TEXTURED,
		WIRE_TEXTURE,

		NUM,
	}

	public enum Axis
	{
		X,
		Y,
		Z,

		// For scaling
		XY,
		XZ,
		YZ,
		ALL,
	}

	public enum ViewLayout
	{
		FOUR_WAY,
		RIGHT,
		TOP,
		DOUBLE,
		
		NUM,
	}

	public enum BGColor
	{
		LIGHT,
		DARK,

		NUM,
	}

	public enum GimbalDisplay
	{
		HIDE,
		SHOW,

		NUM,
	}

	public enum ClipPlaneDisplay
	{
		HIDE,
		SHOW,

		NUM,
	}

	public enum VertDisplay
	{
		HIDE,
		SHOW,

		NUM,
	}

	public enum LightingType
	{
		NONE,
		BRIGHT,
		DARK,

		NUM,
	}

	public enum VisibilityType
	{
		ALL,
		NO_RENDER,
		NO_COLLIDE,
		NORMAL_ONLY,

		NUM,
	}

	public partial class Editor : Form
   {
		public EditMode m_edit_mode = EditMode.POLY;
		public ScaleMode m_scale_mode = ScaleMode.VIEW_XY;
		public PivotMode m_pivot_mode = PivotMode.ORIGIN;
		
		public int m_coplanar_tol = 15;

		public GridDisplay m_grid_display = GridDisplay.ALL;
		public float m_grid_spacing = 0.5f;
		public int m_grid_width = 2;
		public int m_grid_lines = 8; // This is doubled while drawing the grid
		public float m_grid_snap = 0.25f;

		public float m_size_x = 1.0f;
		public float m_size_y = 1.0f;
		public float m_size_height = 1.0f;
		public int m_size_segments = 16;

		public float m_extrude_length = 0.25f;
		public float m_inset_length = 0.25f;
		public int m_inset_pct = 25;
		public float m_bevel_width = 0.125f;
		//public int m_default_uv_scalar = 4;

		public SideSelect m_side_select = SideSelect.VISIBLE;
		
		public DragMode m_drag_mode = DragMode.ALL;

		public ViewMode m_view_mode_ortho = ViewMode.WIREFRAME;
		public ViewMode m_view_mode_persp = ViewMode.WIRE_TEXTURE;
		public int m_view_persp_fov = 50;

		public ViewLayout m_view_layout = ViewLayout.FOUR_WAY;
		public BGColor m_bg_color = BGColor.LIGHT;
		public GimbalDisplay m_gimbal_display = GimbalDisplay.HIDE;
		public ClipPlaneDisplay m_cp_display = ClipPlaneDisplay.HIDE;
		public LightingType m_lighting_type = LightingType.NONE;
		public VertDisplay m_vert_display = VertDisplay.HIDE;

		public VisibilityType m_vis_type = VisibilityType.ALL;

		public bool auto_copy_face_flags = true;

		public bool m_low_res_textures = false;
		public bool m_low_res_force = false;

		public Point m_tex_list_loc;
		public Point m_tex_list_sz;
		public Point m_tunnel_builder_loc;

		public void UpdateDirectories()
		{
			const string folder_decals = "Decals";
			const string folder_decal_textures = "DecalTextures";
			const string folder_level_textures = "LevelTextures";

			var subfolder_to_find = new string[]
				                      {
					                      folder_decals,
					                      folder_decal_textures,
				                      };

            var executable_dir = Program.m_datadir;

			// Pop up folders until we find some of the subfolders we need
			var test_folder = executable_dir;
			while( true ) {
				int count = subfolder_to_find
					// Get the fullpath for this subfolder
					.Select( subfolder => System.IO.Path.Combine( test_folder, subfolder ) )
					// Check if it exists or not
					.Select( System.IO.Directory.Exists )
					// Count up the ones that exist
					.Count( dirExists => dirExists == true );
				if( count > 1 ) {
					// Found more than one folder, go with it
					break;
				}

				// Try popping up a folder to the parent
				var parent_folder = System.IO.Path.GetDirectoryName( test_folder );
				if( string.IsNullOrEmpty( parent_folder ) || parent_folder == test_folder ) {
					// Can't go any further
					test_folder = executable_dir;
					break;
				}
				test_folder = parent_folder;
			}

			this.m_filepath_root = test_folder;
			this.m_filepath_decals = System.IO.Path.Combine( m_filepath_root, folder_decals );
			this.m_filepath_decal_textures = System.IO.Path.Combine( m_filepath_root, folder_decal_textures );
			this.m_filepath_level_textures = System.IO.Path.Combine(m_filepath_root, folder_level_textures);
		}

	   public void LoadPreferences()
		{
			Directory.SetCurrentDirectory(m_filepath_root);

			//m_filepath_root = UserPrefs.GetString("filepath_root", m_filepath_root);
			m_filepath_decal_textures = UserPrefs.GetString("filepath_decal_textures", m_filepath_decal_textures);
			m_filepath_level_textures = UserPrefs.GetString("filepath_level_textures", m_filepath_level_textures);
			m_filepath_decals = UserPrefs.GetString("filepath_decals", m_filepath_decals);

			m_grid_display = (GridDisplay)UserPrefs.GetInt("grid_display", (int)m_grid_display);
			m_grid_spacing = UserPrefs.GetFloat("grid_spacing", m_grid_spacing);
			SetGridLines();
			m_grid_snap = UserPrefs.GetFloat("grid_snap", m_grid_snap);

			m_extrude_length = UserPrefs.GetFloat("extrude_length", m_extrude_length);
			m_inset_pct = UserPrefs.GetInt("inset_pct", m_inset_pct);
			m_inset_length = UserPrefs.GetFloat("inset_length", m_inset_length);
			m_bevel_width= UserPrefs.GetFloat("bevel_width", m_bevel_width);

			m_coplanar_tol = UserPrefs.GetInt("coplanar_tol", m_coplanar_tol);
			m_side_select = (SideSelect)UserPrefs.GetInt("side_select", (int)m_side_select);
			m_drag_mode = (DragMode)UserPrefs.GetInt("drag_mode", (int)m_drag_mode);
			
			m_view_mode_ortho = (ViewMode)UserPrefs.GetInt("view_mode_ortho", (int)m_view_mode_ortho);
			m_view_mode_persp = (ViewMode)UserPrefs.GetInt("view_mode_persp", (int)m_view_mode_persp);
			m_view_persp_fov = UserPrefs.GetInt("view_persp_fov", m_view_persp_fov);
			m_view_layout = (ViewLayout)UserPrefs.GetInt("view_layout", (int)m_view_layout);
			m_bg_color = (BGColor)UserPrefs.GetInt("bg_color", (int)m_bg_color);
			m_gimbal_display = (GimbalDisplay)UserPrefs.GetInt("gimbal_display", (int)m_gimbal_display);
			m_lighting_type = (LightingType)UserPrefs.GetInt("lighting_type", (int)m_lighting_type);
			m_pivot_mode = (PivotMode)UserPrefs.GetInt("pivot_mode", (int)m_pivot_mode);
			
			m_low_res_force = UserPrefs.GetBool("low_res_force", m_low_res_force);
			forceLowResTexturesToolStripMenuItem.Checked = m_low_res_force;

			m_tex_list_loc = UserPrefs.GetPoint("tex_list_loc", texture_list.Location);
			m_tex_list_sz = UserPrefs.GetPoint("tex_list_sz", (Point)texture_list.Size);
			m_tunnel_builder_loc = UserPrefs.GetPoint("tunnel_builder_loc", tunnel_builder.Location);
			texture_list.Size = (Size)m_tex_list_sz;

			for (int i = 0; i < RECENT_NUM; i++) {
				m_recent_files[i] = UserPrefs.GetString("recent" + i.ToString(), m_recent_files[i]);
			}
			UpdateRecentFileMenu();
		}
		
		public void SavePreferences()
		{
			//UserPrefs.SetString("filepath_root", m_filepath_root);
			UserPrefs.SetString("filepath_decal_textures", m_filepath_decal_textures);
			UserPrefs.SetString("filepath_level_textures", m_filepath_level_textures);
			UserPrefs.SetString("filepath_decals", m_filepath_decals);

			UserPrefs.SetInt("grid_display", (int)m_grid_display);
			UserPrefs.SetFloat("grid_spacing", m_grid_spacing);
			UserPrefs.SetFloat("grid_snap", m_grid_snap);

			UserPrefs.SetFloat("extrude_length", m_extrude_length);
			UserPrefs.SetInt("inset_pct", m_inset_pct);
			UserPrefs.SetFloat("inset_length", m_inset_length);
			UserPrefs.SetFloat("bevel_width", m_bevel_width);

			UserPrefs.SetInt("coplanar_tol", m_coplanar_tol);
			UserPrefs.SetInt("side_select", (int)m_side_select);
			UserPrefs.SetInt("drag_mode", (int)m_drag_mode);

			UserPrefs.SetInt("view_mode_ortho", (int)m_view_mode_ortho);
			UserPrefs.SetInt("view_mode_persp", (int)m_view_mode_persp);
			UserPrefs.SetInt("view_persp_fov", m_view_persp_fov);
			UserPrefs.SetInt("view_layout", (int)m_view_layout);
			UserPrefs.SetInt("bg_color", (int)m_bg_color);
			UserPrefs.SetInt("gimbal_display", (int)m_gimbal_display);
			UserPrefs.SetInt("lighting_type", (int)m_lighting_type);
			UserPrefs.SetInt("pivot_mode", (int)m_pivot_mode);

			UserPrefs.SetBool("low_res_force", m_low_res_force);

			UserPrefs.SetPoint("tex_list_loc", m_tex_list_loc);
			UserPrefs.SetPoint("tex_list_sz", (Point)texture_list.Size);
			UserPrefs.SetPoint("tunnel_builder_loc", m_tunnel_builder_loc);
			
			for (int i = 0; i < RECENT_NUM; i++) {
				UserPrefs.SetString("recent" + i.ToString(), m_recent_files[i]);
			}

			Directory.SetCurrentDirectory(m_filepath_root);
			UserPrefs.Flush();
		}

		public void UpdateOptionLabels()
		{
			slider_coplanar_angle.ValueText = m_coplanar_tol.ToString();
			slider_extrude_length.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_extrude_length);
			slider_inset_bevel.ValueText = m_inset_pct.ToString();
			slider_inset_length.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_inset_length);
			slider_bevel_width.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_bevel_width);
			slider_sizex.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_size_x);
			slider_sizey.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_size_y);
			slider_sizeheight.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_size_height);
			slider_sizesegments.ValueText = m_size_segments.ToString();
			label_editmode.Text = "Mode: " + m_edit_mode.ToString();
			label_pivotmode.Text = "Pivot: " + m_pivot_mode.ToString();
			label_scalemode.Text = "Scale: " + m_scale_mode.ToString();
			slider_grid_spacing.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_grid_spacing);
			slider_grid_snap.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_grid_snap);
			slider_grid_width.ValueText = m_grid_width.ToString();
			label_grid_display.Text = "Display: " + m_grid_display.ToString();
			label_view_layout.Text = "Layout: " + m_view_layout.ToString();
			label_view_persp.Text = "Persp: " + m_view_mode_persp.ToString();
			label_view_ortho.Text = "Ortho: " + m_view_mode_ortho.ToString();
			label_view_dark.Text = "Background: " + m_bg_color.ToString();
			label_gimbal.Text = "Gimbal: " + m_gimbal_display.ToString();
			label_lighting.Text = "Lighting: " + m_lighting_type.ToString();

			label_vert_display.Text = "Vert Normals: " + m_vert_display.ToString();
			label_poly_filter.Text = "PolyFilter: " + m_vis_type.ToString();
         slider_smooth_angle_diff.ValueText = m_dmesh.smooth_angle_diff.ToString();
			slider_smooth_angle_same.ValueText = m_dmesh.smooth_angle_same.ToString();
		}

		public void CycleLightingType()
		{
			m_lighting_type = (LightingType)(((int)m_lighting_type + 1) % (int)LightingType.NUM);
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleGimbalDisplay()
		{
			m_gimbal_display = (GimbalDisplay)(((int)m_gimbal_display + 1) % (int)GimbalDisplay.NUM);
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleBGColor()
		{
			m_bg_color = (BGColor)(((int)m_bg_color + 1) % (int)BGColor.NUM);
			UpdateOptionLabels();
			RefreshGrid();
			this.Refresh();
		}

		public void SetEditModeSilent(EditMode em, bool update_label = true)
		{
			m_edit_mode = em;
			if (update_label) {
				UpdateOptionLabels();
			}
		}

		public void ChangeCoplanarAngle(int amt)
		{
			m_coplanar_tol = Utility.Clamp(m_coplanar_tol + amt, 5, 50); 
			UpdateOptionLabels();
		}

		public void ChangeSmoothAngleSame(int amt)
		{
			m_dmesh.smooth_angle_same = Utility.Clamp(m_dmesh.smooth_angle_same + amt, 0, 95);
			RefreshGeometry();
			UpdateOptionLabels();
		}

		public void ChangeSmoothAngleDiff(int amt)
		{
			m_dmesh.smooth_angle_diff = Utility.Clamp(m_dmesh.smooth_angle_diff + amt, 0, 95);
			RefreshGeometry();
			UpdateOptionLabels();
		}

		public void ChangeExtrudeLength(int amt)
		{
			m_extrude_length = Utility.Clamp(m_extrude_length + amt * SIZE_INC, SIZE_INC, 2f);
			m_extrude_length = Utility.SnapValue(m_extrude_length, SIZE_INC);
			UpdateOptionLabels();
		}

		public void ChangeInsetLength(int amt)
		{
			m_inset_length = Utility.Clamp(m_inset_length + amt * SIZE_INC_SM, SIZE_INC_SM, 2f);
			m_inset_length = Utility.SnapValue(m_inset_length, SIZE_INC_SM);
			UpdateOptionLabels();
		}

		public void ChangeInsetPct(int amt)
		{
			m_inset_pct = Utility.Clamp(m_inset_pct + amt * 5, -100, 95);
			UpdateOptionLabels();
		}

		public void ChangeBevelWidth(int amt)
		{
			m_bevel_width = Utility.Clamp(m_bevel_width + amt * SIZE_INC_SM * 0.25f, SIZE_INC_SM * 0.25f, 0.25f);
			m_bevel_width = Utility.SnapValue(m_bevel_width, SIZE_INC_SM * 0.25f);
			UpdateOptionLabels();
		}

		/*public void ChangeUVScalar(int amt)
		{
			m_default_uv_scalar = Utility.Clamp(m_default_uv_scalar + amt, 1, 16);
			UpdateOptionLabels();
		}*/

		public const float SIZE_INC = 0.125f;
		public const float SIZE_INC_SM = 0.0625f;

		public void ChangeSizeX(int amt)
		{
			m_size_x = Utility.Clamp(m_size_x + amt * SIZE_INC, SIZE_INC, 2f);
			m_size_x = Utility.SnapValue(m_size_x, SIZE_INC);
			UpdateOptionLabels();
		}

		public void ChangeSizeY(int amt)
		{
			m_size_y = Utility.Clamp(m_size_y + amt * SIZE_INC, SIZE_INC, 2f);
			m_size_y = Utility.SnapValue(m_size_y, SIZE_INC);
			UpdateOptionLabels();
		}

		public void ChangeSizeHeight(int amt)
		{
			m_size_height = Utility.Clamp(m_size_height + amt * SIZE_INC, SIZE_INC, 2f);
			m_size_height = Utility.SnapValue(m_size_height, SIZE_INC);
			UpdateOptionLabels();
		}

		public void ChangeSizeSegments(int amt)
		{
			m_size_segments = Utility.Clamp(m_size_segments + amt, 4, 64);
			UpdateOptionLabels();
		}

		public void CycleEditMode(bool reverse)
		{
			if (reverse) {
				m_edit_mode = (EditMode)(((int)m_edit_mode + (int)EditMode.NUM - 1) % (int)EditMode.NUM);
			} else {
				m_edit_mode = (EditMode)(((int)m_edit_mode + 1) % (int)EditMode.NUM);
			}
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CyclePivotMode(bool reverse)
		{
			if (reverse) {
				m_pivot_mode = (PivotMode)(((int)m_pivot_mode + (int)PivotMode.NUM - 1) % (int)PivotMode.NUM);
			} else {
				m_pivot_mode = (PivotMode)(((int)m_pivot_mode + 1) % (int)PivotMode.NUM);
			}
			UpdateOptionLabels();
		}

		public void CycleScaleMode(bool reverse)
		{
			if (reverse) {
				m_scale_mode = (ScaleMode)(((int)m_scale_mode + (int)ScaleMode.NUM - 1) % (int)ScaleMode.NUM);
			} else {
				m_scale_mode = (ScaleMode)(((int)m_scale_mode + 1) % (int)ScaleMode.NUM);
			}
			UpdateOptionLabels();
		}

		public void ChangeGridSpacing(int increase)
		{
			if (increase > 0) {
				m_grid_spacing = Math.Min(4f, m_grid_spacing * 2f);
			} else if (increase < 0) {
				m_grid_spacing = Math.Max(0.03125f, m_grid_spacing / 2f);
			} else {
				return;
			}
			m_grid_spacing = (float)Math.Round(m_grid_spacing, 5);
			SetGridLines();
			UpdateOptionLabels();
			RefreshGrid();
			this.Refresh();
		}

		public void ChangeGridWidth(int increase)
		{
			if (increase > 0) {
				m_grid_width = Math.Min(32, m_grid_width * 2);
			} else if (increase < 0) {
				m_grid_width = Math.Max(1, m_grid_width / 2);
			} else {
				return;
			}
			SetGridLines();
			UpdateOptionLabels();
			RefreshGrid();
			this.Refresh();
		}

		public void SetGridLines()
		{
			m_grid_lines = (int)(m_grid_width / m_grid_spacing + 0.001f);
		}

		public void ChangeGridSnap(int increase)
		{
			if (increase > 0) {
				m_grid_snap = Math.Min(4f, m_grid_snap * 2f);
			} else if (increase < 0) {
				m_grid_snap = Math.Max(1f / 64f, m_grid_snap / 2f);
			} else {
				return;
			}
			//m_grid_snap = (float)Math.Round(m_grid_snap, 6);
			m_grid_snap = Utility.SnapValue(m_grid_snap, 1f / 64f);
			UpdateOptionLabels();
		}

		public void CycleGridDisplay()
		{
			m_grid_display = (GridDisplay)(((int)m_grid_display + 1) % (int)GridDisplay.NUM);
			UpdateOptionLabels();
			this.Refresh();
		}

		public void CycleDragMode()
		{
			m_drag_mode = (DragMode)(((int)m_drag_mode + 1) % (int)DragMode.NUM);
			UpdateOptionLabels();
		}

		public void CycleSideSelect()
		{
			m_side_select = (SideSelect)(((int)m_side_select + 1) % (int)SideSelect.NUM);
			UpdateOptionLabels();
		}

		public void RefreshGrid()
		{
			gl_view[0].UpdateBGColor(m_bg_color);
			gl_view[0].UpdateClearColor();
			gl_view[0].BuildGridGeometry(m_grid_lines, m_grid_spacing);
		}

		public void CycleViewModeOrtho(bool reverse)
		{
			if (reverse) {
				m_view_mode_ortho = (ViewMode)(((int)m_view_mode_ortho + (int)ViewMode.NUM - 1) % (int)ViewMode.NUM);
			} else {
				m_view_mode_ortho = (ViewMode)(((int)m_view_mode_ortho + 1) % (int)ViewMode.NUM);
			}
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleViewModePersp(bool reverse)
		{
			if (reverse) {
				m_view_mode_persp = (ViewMode)(((int)m_view_mode_persp + (int)ViewMode.NUM - 1) % (int)ViewMode.NUM);
			} else {
				m_view_mode_persp = (ViewMode)(((int)m_view_mode_persp + 1) % (int)ViewMode.NUM);
			}
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleViewLayout(bool reverse)
		{
			if (reverse) {
				m_view_layout = (ViewLayout)(((int)m_view_layout + (int)ViewLayout.NUM - 1) % (int)ViewLayout.NUM);
			} else {
				m_view_layout = (ViewLayout)(((int)m_view_layout + 1) % (int)ViewLayout.NUM);
			}
			UpdateOptionLabels();
			ResetViews();
		}

		public void AdjustPerspFOV(int amt)
		{
			m_view_persp_fov += amt;
			m_view_persp_fov = Math.Max(20, Math.Min(90, m_view_persp_fov));
		}

		public bool IsViewInitialized()
		{
			if (gl_view == null || gl_view.Length == 0 || gl_view[0] == null)
				return false;
			return true;
		}

		public void ResetViews()
		{
			if (!IsViewInitialized())
				return;

			Size sz = gl_panel.Size;
			sz.Height -= 5;
			sz.Width -= 5;

			switch (m_view_layout) {
				case ViewLayout.FOUR_WAY:
					for (int i = 0; i < (int)ViewType.NUM; i++) {
						switch (i) {
							case 0: gl_view[i].Location = new Point(0, 0); break;
							case 1: gl_view[i].Location = new Point(sz.Width / 2, 0); break;
							case 2: gl_view[i].Location = new Point(0, sz.Height / 2); break;
							case 3: gl_view[i].Location = new Point(sz.Width / 2, sz.Height / 2); break;
						}
						gl_view[i].Size = new Size(sz.Width / 2, sz.Height / 2);
						label_viewport[i].Location = new Point(gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2);
					}
					break;
				case ViewLayout.RIGHT:
					for (int i = 0; i < (int)ViewType.NUM; i++) {
						switch (i) {
							case 0:
								gl_view[i].Location = new Point(0, 0);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height/ 3);
								break;
							case 1:
								gl_view[i].Location = new Point(0, sz.Height / 3);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 2:
								gl_view[i].Location = new Point(0, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 3:
								gl_view[i].Location = new Point(sz.Width / 3, 0);
								gl_view[i].Size = new Size(sz.Width * 2 / 3, sz.Height);
								break;
						}

						label_viewport[i].Location = new Point(gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2);
					}
					break;
				case ViewLayout.TOP:
					for (int i = 0; i < (int)ViewType.NUM; i++) {
						switch (i) {
							case 0: 
								gl_view[i].Location = new Point(0, 0);
								gl_view[i].Size = new Size(sz.Width, sz.Height * 2 / 3);
								break;
							case 1:
								gl_view[i].Location = new Point(0, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 2:
								gl_view[i].Location = new Point(sz.Width / 3, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 3:
								gl_view[i].Location = new Point(sz.Width * 2 / 3, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
						}
						
						label_viewport[i].Location = new Point(gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2);
					}
					break;
				case ViewLayout.DOUBLE:
					for (int i = 0; i < (int)ViewType.NUM; i++) {
						switch (i) {
							case 0:
								gl_view[i].Location = new Point(0, 0);
								gl_view[i].Size = new Size(sz.Width / 2, sz.Height * 2 / 3);
								break;
							case 1:
								gl_view[i].Location = new Point(0, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 4, sz.Height / 3);
								break;
							case 2:
								gl_view[i].Location = new Point(sz.Width / 4, sz.Height * 2 / 3);
								gl_view[i].Size = new Size(sz.Width / 4, sz.Height / 3);
								break;
							case 3:
								gl_view[i].Location = new Point(sz.Width / 2, 0);
								gl_view[i].Size = new Size(sz.Width / 2, sz.Height);
								break;
						}

						label_viewport[i].Location = new Point(gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2);
					}
					break;
			}
		}
   }
}
