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
		SEGMENT,
		SIDE,
		VERTEX,
		ENTITY,

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
		CHUNKS,

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
		ALL
	}

	public enum ViewLayout
	{
		FOUR_WAY,
		RIGHT,
		TOP,
		DOUBLE,
		BOTTOM,

		NUM,
	}

	public enum BGColor
	{
		LIGHT,
		DARK,

		NUM,
	}

	public enum PivotMode
	{
		ORIGIN,
		LOCAL,
		MARKED,
		SELECTED_SEG,
		SELECTED_SIDE,
		SELECTED_VERT,
		SELECTED_ENTITY,

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

	public enum InsertDecal
	{
		ALL,
		SELECTED,
		NONE,

		NUM,
	}

	public enum LightingType
	{
		OFF,
		BRIGHT,
		DARK,

		NUM,
	}

	public enum ShowTextType
	{
		NONE,
		ALL_SEGS,
		MARK_SEGS,
		POWERUPS,
		ROBOTS,
		MARK_ENTS,

		NUM
	}

	public enum CutterDisplay
	{
		HIDE,
		SHOW,

		NUM
	}

	public enum OperationMode
	{
		TOGGLE,
		ADD,
		REMOVE
	}

	public partial class Editor : EditorDockContent
	{
		public EditMode m_edit_mode = EditMode.SEGMENT;

		public int m_coplanar_tol = 15;
		public int m_rotate_angle = 15;

		public GridDisplay m_grid_display = GridDisplay.ORTHO;
		public int m_grid_spacing = 4;
		public int m_grid_lines = 40; // This is doubled while drawing the grid
		public float m_grid_snap = 2f;

		public int m_extrude_length = 4;

		public SideSelect m_side_select = SideSelect.VISIBLE;
		public bool m_insert_advance = true;

		public DragMode m_drag_mode = DragMode.ALL;

		public ViewMode m_view_mode_ortho = ViewMode.WIREFRAME;
		public ViewMode m_view_mode_persp = ViewMode.WIRE_TEXTURE;
		public int m_view_persp_fov = 60;

		public ViewLayout m_view_layout = ViewLayout.FOUR_WAY;
		public BGColor m_bg_color = BGColor.LIGHT;
		public GimbalDisplay m_gimbal_display = GimbalDisplay.HIDE;
		public ClipPlaneDisplay m_cp_display = ClipPlaneDisplay.SHOW;
		public InsertDecal m_insert_decal = InsertDecal.ALL;
		public LightingType m_lighting_type = LightingType.BRIGHT;
		public ShowTextType m_show_3d_text_type = ShowTextType.NONE;
		public CutterDisplay m_cutter_display = CutterDisplay.SHOW;
		public bool m_auto_center = false;

		public PivotMode m_pivot_mode = PivotMode.MARKED;

		public Point m_decal_list_loc;
		public Point m_decal_list_sz;
		public bool m_decal_list_visible;
		public Point m_tex_list_loc;
		public Point m_tex_list_sz;
		public bool m_texture_list_visible;
		public Point m_texture_set_list_loc;
		public Point m_texture_set_list_sz;
		public bool m_texture_set_list_visible;
		public Point m_uv_editor_loc;
		public Point m_uv_editor_sz;
		public bool m_uv_editor_visible;

		public EditMode m_mm_edit_type = EditMode.SEGMENT;
		public OperationMode m_mm_op_mode = OperationMode.ADD;

		public string m_editor_layout = string.Empty;

		public class TextureCollection
		{
			private string m_name;
			private List<string> m_textures;

			public string Name { get { return m_name; } }

			public TextureCollection(string name, bool deserialize = false)
			{
				m_textures = new List<string>();

				if (deserialize) {
					string[] names = name.Split(':');
					m_name = names[0];
					for (int n = 1; n < names.Length; n++) {
						m_textures.Add(names[n]);
					}
				} else {
					m_name = name;
				}
			}

			public void AddTexture(string texture_name)
			{
				m_textures.Add(texture_name);
			}

			public void RemoveTexture(string texture_name)
			{
				m_textures.Remove(texture_name);
			}

			public string Serialize()
			{
				string s = m_name;
				foreach (string texture in m_textures) {
					s += ":" + texture;
				}
				return s;
			}

			public IEnumerable<string> EnumerateTexturesSorted()
			{
				m_textures.Sort();
				foreach (string texture in m_textures) {
					yield return texture;
				}
			}

			public bool Contains(string texture_name)
			{
				return (m_textures.Find(t => t == texture_name) != null);
			}
		}

		public List<TextureCollection> TextureCollections = new List<TextureCollection>();

		public void UpdateDirectories()
		{
			const string folder_decals = "Decals";
			const string folder_decal_textures = "DecalTextures";
			const string folder_levels = "Levels";
			const string folder_level_textures = "LevelTextures";

			var subfolder_to_find = new string[]
				                      {
					                      folder_decals,
					                      folder_decal_textures,
					                      folder_levels,
					                      folder_level_textures,
				                      };

            var executable_dir = Program.m_datadir;

			// Pop up folders until we find some of the subfolders we need
			var test_folder = executable_dir;
			while (true) {
				int count = subfolder_to_find
					// Get the fullpath for this subfolder
					.Select(subfolder => System.IO.Path.Combine(test_folder, subfolder))
					// Check if it exists or not
					.Select(System.IO.Directory.Exists)
					// Count up the ones that exist
					.Count(dirExists => dirExists == true);
				if (count > 1) {
					// Found more than one folder, go with it
					break;
				}

				// Try popping up a folder to the parent
				var parent_folder = System.IO.Path.GetDirectoryName(test_folder);
				if (string.IsNullOrEmpty(parent_folder) || parent_folder == test_folder) {
					// Can't go any further
					test_folder = executable_dir;
					break;
				}
				test_folder = parent_folder;
			}

			this.m_filepath_root = test_folder;
			this.m_filepath_decals = System.IO.Path.Combine(m_filepath_root, folder_decals);
			this.m_filepath_decal_textures = System.IO.Path.Combine(m_filepath_root, folder_decal_textures);
			this.m_filepath_levels = System.IO.Path.Combine(m_filepath_root, folder_levels);
			this.m_filepath_level_textures = System.IO.Path.Combine(m_filepath_root, folder_level_textures);
		}

		public void LoadPreferences()
		{
			Directory.SetCurrentDirectory(m_filepath_root);

			m_grid_display = (GridDisplay)UserPrefs.GetInt("grid_display", (int)m_grid_display);
			m_grid_spacing = UserPrefs.GetInt("grid_spacing", m_grid_spacing);
			m_grid_snap = UserPrefs.GetFloat("grid_snap", m_grid_snap);

			m_coplanar_tol = UserPrefs.GetInt("coplanar_tol", m_coplanar_tol);
			m_rotate_angle = UserPrefs.GetInt("rotate_angle", m_rotate_angle);
			m_side_select = (SideSelect)UserPrefs.GetInt("side_select", (int)m_side_select);
			m_insert_advance = UserPrefs.GetBool("insert_advance", m_insert_advance);
			m_drag_mode = (DragMode)UserPrefs.GetInt("drag_mode", (int)m_drag_mode);

			m_view_mode_ortho = (ViewMode)UserPrefs.GetInt("view_mode_ortho", (int)m_view_mode_ortho);
			m_view_mode_persp = (ViewMode)UserPrefs.GetInt("view_mode_persp", (int)m_view_mode_persp);
			m_view_persp_fov = UserPrefs.GetInt("view_persp_fov", m_view_persp_fov);
			m_view_layout = (ViewLayout)UserPrefs.GetInt("view_layout", (int)m_view_layout);
			m_bg_color = (BGColor)UserPrefs.GetInt("bg_color", (int)m_bg_color);
			m_gimbal_display = (GimbalDisplay)UserPrefs.GetInt("gimbal_display", (int)m_gimbal_display);
			m_lighting_type = (LightingType)UserPrefs.GetInt("lighting_type", (int)m_lighting_type);
			m_cp_display = (ClipPlaneDisplay)UserPrefs.GetInt("clip_decal", (int)m_cp_display);
			m_insert_decal = (InsertDecal)UserPrefs.GetInt("insert_decal", (int)m_insert_decal);
			m_show_3d_text_type = (ShowTextType)UserPrefs.GetInt("show_segment_numbers", (int)m_show_3d_text_type);
			m_auto_center = UserPrefs.GetBool("auto_center", m_auto_center);

			m_decal_list_loc = UserPrefs.GetPoint("decal_list_loc", decal_list.Location);
			m_decal_list_sz = UserPrefs.GetPoint("decal_list_sz", (Point)decal_list.Size);
			m_tex_list_loc = UserPrefs.GetPoint("tex_list_loc", texture_list.Location);
			m_tex_list_sz = UserPrefs.GetPoint("tex_list_sz", (Point)texture_list.Size);
			m_texture_set_list_loc = UserPrefs.GetPoint("texture_set_list_loc", texture_list.Location);
			m_texture_set_list_sz = UserPrefs.GetPoint("texture_set_list_sz", (Point)texture_list.Size);
			m_uv_editor_loc = UserPrefs.GetPoint("uv_editor_loc", uv_editor.Location);
			m_uv_editor_sz = UserPrefs.GetPoint("uv_editor_sz", (Point)uv_editor.Size);

			decal_list.Size = (Size)m_decal_list_sz;
			texture_list.Size = (Size)m_tex_list_sz;
			uv_editor.Size = (Size)m_uv_editor_sz;

			for (int i = 0; i < NumRecentFiles; i++) {
				string recent_file = GetRecentFile( i );
				SetRecentFile( i, UserPrefs.GetString( "recent" + i.ToString(), recent_file ) );
			}
			Shell.UpdateRecentFileMenu();

			m_pivot_mode = (PivotMode)UserPrefs.GetInt("entity_pivot", (int)m_pivot_mode);

			m_editor_layout = UserPrefs.GetString( "layout", string.Empty );

			//Save whether pop-up windows are open at startup
			m_decal_list_visible = UserPrefs.GetBool("decal_list_vis", false);
			m_texture_list_visible = UserPrefs.GetBool("tex_list_vis", false);
			m_texture_set_list_visible = UserPrefs.GetBool("texture_set_list_vis", false);
			m_uv_editor_visible = UserPrefs.GetBool("uv_editor_vis", false);

#if !PUBLIC_RELEASE
			Overload.Perforce.m_cached_username = UserPrefs.GetString("m_cached_username", string.Empty);
			Overload.Perforce.m_cached_clientname = UserPrefs.GetString("m_cached_clientname", string.Empty);
#endif // !PUBLIC_RELEASE

			m_mm_edit_type = (EditMode)UserPrefs.GetInt("mm_edit_type", (int)m_mm_edit_type);
			m_mm_op_mode = (OperationMode)UserPrefs.GetInt("mm_op_mode", (int)m_mm_op_mode);

			//Read texture collections
			int num_texture_collections = UserPrefs.GetInt("num_texture_collections", 0);
			for (int c=0;c<num_texture_collections;c++) {
				TextureCollections.Add(new TextureCollection(UserPrefs.GetString("texture_collection_" + c), true));
			}
		}

		public void StoreLayoutToPreferences( string layout )
		{
			if( string.IsNullOrWhiteSpace( layout ) ) {
				m_editor_layout = string.Empty;
			} else {
				m_editor_layout = layout;
			}

			UserPrefs.SetString( "layout", m_editor_layout );

			//Save whether pop-up windows are open
			UserPrefs.SetBool("decal_list_vis", decal_list.Visible);
			UserPrefs.SetBool("tex_list_vis", texture_list.Visible);
			UserPrefs.SetBool("texture_set_list_vis", texture_set_list.Visible);
			UserPrefs.SetBool("uv_editor_vis", uv_editor.Visible);
		}

		public void SavePreferences()
		{
			UserPrefs.SetInt("grid_display", (int)m_grid_display);
			UserPrefs.SetInt("grid_spacing", m_grid_spacing);
			UserPrefs.SetFloat("grid_snap", m_grid_snap);

			UserPrefs.SetInt("coplanar_tol", m_coplanar_tol);
			UserPrefs.SetInt("rotate_angle", m_rotate_angle);
			UserPrefs.SetInt("side_select", (int)m_side_select);
			UserPrefs.SetBool("insert_advance", m_insert_advance);
			UserPrefs.SetInt("drag_mode", (int)m_drag_mode);

			UserPrefs.SetInt("view_mode_ortho", (int)m_view_mode_ortho);
			UserPrefs.SetInt("view_mode_persp", (int)m_view_mode_persp);
			UserPrefs.SetInt("view_persp_fov", m_view_persp_fov);
			UserPrefs.SetInt("view_layout", (int)m_view_layout);
			UserPrefs.SetInt("bg_color", (int)m_bg_color);
			UserPrefs.SetInt("gimbal_display", (int)m_gimbal_display);
			UserPrefs.SetInt("lighting_type", (int)m_lighting_type);
			UserPrefs.SetInt("clip_decal", (int)m_cp_display);
			UserPrefs.SetInt("insert_decal", (int)m_insert_decal);
			UserPrefs.SetInt("show_segment_numbers", (int)m_show_3d_text_type);
			UserPrefs.SetBool("auto_center", m_auto_center);

			UserPrefs.SetInt("entity_pivot", (int)m_pivot_mode);

			UserPrefs.SetPoint("decal_list_loc", m_decal_list_loc);
			UserPrefs.SetPoint("decal_list_sz", (Point)decal_list.Size);
			UserPrefs.SetPoint("tex_list_loc", m_tex_list_loc);
			UserPrefs.SetPoint("tex_list_sz", (Point)texture_list.Size);
			UserPrefs.SetPoint("texture_set_list_loc", m_texture_set_list_loc);
			UserPrefs.SetPoint("texture_set_list_sz", (Point)texture_set_list.Size);
			UserPrefs.SetPoint("uv_editor_loc", m_uv_editor_loc);
			UserPrefs.SetPoint("uv_editor_sz", (Point)uv_editor.RestoreBounds.Size);

			UserPrefs.SetInt("mm_edit_type", (int)m_mm_edit_type);
			UserPrefs.SetInt("mm_op_mode", (int)m_mm_op_mode);

			for (int i = 0; i < NumRecentFiles; i++) {
				UserPrefs.SetString( string.Format( "recent{0}", i ), GetRecentFile( i ) );
			}

#if !PUBLIC_RELEASE
			UserPrefs.SetString("m_cached_username", Overload.Perforce.m_cached_username);
			UserPrefs.SetString("m_cached_clientname", Overload.Perforce.m_cached_clientname);
#endif // !PUBLIC_RELEASE

			//Pack texture collections into string
			if (TextureCollections.Count > 0) {
				UserPrefs.SetInt("num_texture_collections", TextureCollections.Count);
				int num = 0;
				foreach (TextureCollection collection in TextureCollections) {
					UserPrefs.SetString("texture_collection_" + num, collection.Serialize());
					num++;
				}
			}

			Directory.SetCurrentDirectory(m_filepath_root);
			UserPrefs.Flush();
		}

		public void UpdateOptionLabels()
		{
			label_editmode.Text = "Mode: " + m_edit_mode.ToString();

			ViewOptionsPane.UpdateOptionLabels();
			GeometryPane.UpdateOptionsLabels();
			EntityEditPane.UpdateOptionLabels();
			GeometryDecalsPane.UpdateOptionsLabels();
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

		public void CycleDecalClipDisplay()
		{
			m_cp_display = (ClipPlaneDisplay)(((int)m_cp_display + 1) % (int)ClipPlaneDisplay.NUM);
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleInsertDecal()
		{
			m_insert_decal = (InsertDecal)(((int)m_insert_decal + 1) % (int)InsertDecal.NUM);
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void CycleEntityPivot(bool reverse)
		{
			m_pivot_mode = (PivotMode)(((int)m_pivot_mode + (reverse ? (int)PivotMode.NUM - 1 : 1)) % (int)PivotMode.NUM);
			UpdateOptionLabels();
		}

		public void CycleBGColor()
		{
			m_bg_color = (BGColor)(((int)m_bg_color + 1) % (int)BGColor.NUM);
			UpdateOptionLabels();
			RefreshGrid();
			this.Refresh();
			decal_list.RefreshGrid();
			decal_list.Refresh();
		}

		public void CycleShowSegmentNumbers(bool reverse)
		{
			if (reverse) {
				m_show_3d_text_type = (ShowTextType)(((int)m_show_3d_text_type + (int)ShowTextType.NUM -  1) % (int)ShowTextType.NUM);
			} else {
				m_show_3d_text_type = (ShowTextType)(((int)m_show_3d_text_type + 1) % (int)ShowTextType.NUM);
			}
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void ToggleAutoCenter()
		{
			m_auto_center = !m_auto_center;
			UpdateOptionLabels();
			if (m_auto_center) {
				RefreshGeometry();
			}
		}

		public void SetEditModeSilent(EditMode em, bool update_label = true)
		{
			m_edit_mode = em;
			if (update_label) {
				UpdateOptionLabels();
			}
		}

		public void ChangeRotateAngle(int amt)
		{
			m_rotate_angle = Utility.Clamp(m_rotate_angle + amt, 5, 90);
			UpdateOptionLabels();
		}

		public void ChangeCoplanarAngle(int amt)
		{
			m_coplanar_tol = Utility.Clamp(m_coplanar_tol + amt, 5, 50);
			UpdateOptionLabels();
		}

		public void ChangeExtrudeLength(int amt)
		{
			m_extrude_length = Utility.Clamp(m_extrude_length + amt, 1, 16);
			UpdateOptionLabels();
		}

		public void CycleEditMode(bool reverse)
		{
			if (reverse) {
				m_edit_mode = (EditMode)(((int)m_edit_mode + (int)EditMode.NUM - 1) % (int)EditMode.NUM);
			} else {
				m_edit_mode = (EditMode)(((int)m_edit_mode + 1) % (int)EditMode.NUM);
			}
			ClearAllMarked();
			UpdateOptionLabels();
			RefreshGeometry();
		}

		public void ChangeGridSpacing(int increase)
		{
			if (increase > 0) {
				m_grid_spacing = Math.Min(32, m_grid_spacing * 2);
			} else if (increase < 0) {
				m_grid_spacing = Math.Max(1, m_grid_spacing / 2);
			} else {
				return;
			}
			UpdateOptionLabels();
			RefreshGrid();
			this.Refresh();
		}

		public void ChangeGridSnap(int increase)
		{
			if (increase > 0) {
				m_grid_snap = Math.Min(32f, m_grid_snap * 2f);
			} else if (increase < 0) {
				m_grid_snap = Math.Max(0.125f, m_grid_snap / 2f);
			} else {
				return;
			}
			m_grid_snap = (float)Math.Round(m_grid_snap, 3);
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

		public void CycleInsertAdvance()
		{
			m_insert_advance = !m_insert_advance;
			UpdateOptionLabels();
		}

		public void CycleSideSelect()
		{
			m_side_select = (SideSelect)(((int)m_side_select + 1) % (int)SideSelect.NUM);
			UpdateOptionLabels();
		}

		public void RefreshGrid()
		{
			gl_panel.RefreshGrid();
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
			gl_panel.ResetViews();
		}

		public void AdjustPerspFOV(int amt)
		{
			m_view_persp_fov += amt;
			m_view_persp_fov = Math.Max(30, Math.Min(120, m_view_persp_fov));
		}

		// Global level settings
		public void UpdateLevelGlobalDataLabels()
		{
			LevelGlobalPane.UpdateLabels();
		}

		public void ChangeCaveSimplify(float amt)
		{
			m_level.global_data.simplify_strength = Utility.Clamp(m_level.global_data.simplify_strength + amt * 0.05f, 0f, 1f);
			m_level.global_data.simplify_strength = Utility.SnapValue(m_level.global_data.simplify_strength, 0.05f);
			UpdateLevelGlobalDataLabels();
		}

		public void CycleCaveGrid(bool prev)
		{
			m_level.global_data.grid_size = (m_level.global_data.grid_size + (prev ? -1 : 1));
			if (m_level.global_data.grid_size > 12) {
				m_level.global_data.grid_size = 6;
			} else if (m_level.global_data.grid_size < 6) {
				m_level.global_data.grid_size = 12;
         }
			UpdateLevelGlobalDataLabels();
		}

		public void CycleCavePreSmooth(bool prev)
		{
			m_level.global_data.pre_smooth = (m_level.global_data.pre_smooth + (prev ? -1 : 1));
			if (m_level.global_data.pre_smooth > 5) {
				m_level.global_data.pre_smooth = 0;
			} else if (m_level.global_data.pre_smooth < 0) {
				m_level.global_data.pre_smooth = 5;
			}
			UpdateLevelGlobalDataLabels();
		}

		public void CycleCavePostSmooth(bool prev)
		{
			m_level.global_data.post_smooth = (m_level.global_data.post_smooth + (prev ? -1 : 1));
			if (m_level.global_data.post_smooth > 5) {
				m_level.global_data.post_smooth = 0;
			} else if (m_level.global_data.post_smooth < 0) {
				m_level.global_data.post_smooth = 5;
			}
			UpdateLevelGlobalDataLabels();
		}

		public void CycleCavePreset(int idx, bool prev)
		{
			m_level.global_data.deform_presets[idx] = (m_level.global_data.deform_presets[idx] + (prev ? -1 : 1));
			if (m_level.global_data.deform_presets[idx] >= DeformPreset.NUM) {
				m_level.global_data.deform_presets[idx] = DeformPreset.NONE;
			} else if (m_level.global_data.deform_presets[idx] < DeformPreset.NONE) {
				m_level.global_data.deform_presets[idx] = DeformPreset.NUM - 1;
			}
			UpdateLevelGlobalDataLabels();
		}

		public void CycleCutterDisplay()
		{
			m_cutter_display = (CutterDisplay)(((int)m_cutter_display + 1) % (int)CutterDisplay.NUM);
			UpdateOptionLabels();
			RefreshGeometry();
		}
	}
}
