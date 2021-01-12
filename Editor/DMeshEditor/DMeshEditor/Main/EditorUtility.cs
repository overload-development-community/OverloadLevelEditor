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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// EDITOR - Utility functions
// Mostly used for consolidating or redirecting editor commands to the specific steps
// Fairly random collection, so moving groups of functions to new files/etc is fine

namespace OverloadLevelEditor
{
	public partial class Editor : Form
	{
		public Vector3 m_src_side_pos = Vector3.Zero;
		public Vector3 m_src_side_normal = Vector3.Zero;
		public int m_src_seg_count = 0;
		public Vector3 m_dest_side_pos = Vector3.Zero;
		public Vector3 m_dest_side_normal = Vector3.Zero;

		public void CopyMarkedPolys()
		{
			m_dmesh_buffer = new DMesh("buffer_dmesh");
			m_dmesh_buffer.CopyMarkedPolys(m_dmesh);
		}

		public void PasteBufferPolys()
		{
			SaveStateForUndo("Paste buffer polys");
			m_dmesh.CopyMarkedPolys(m_dmesh_buffer, true);
			RefreshGeometry();
		}

		public void FlipTaggedPolysAlongAxis(bool x_axis)
		{
			m_dmesh.TagAllVerts(false);
			m_dmesh.TagTaggedPolyVerts();
			Vector3 flip;
			if (x_axis) {
				flip = new Vector3(-1f, 1f, 1f);
			} else {
				flip = new Vector3(1f, 1f, -1f);
			}
			for (int i = 0; i < m_dmesh.vert_info.Count; i++) {
				if (m_dmesh.vert_info[i].tag) {
					m_dmesh.vertex[i] *= flip;
				}
			}

			m_dmesh.ReverseTaggedPolys();
		}

		public void RotateTaggedPolysAroundYAxis(float rotate_amt)
		{
			m_dmesh.TagAllVerts(false);
			m_dmesh.TagTaggedPolyVerts();
			
			for (int i = 0; i < m_dmesh.vert_info.Count; i++) {
				if (m_dmesh.vert_info[i].tag) {
					m_dmesh.vertex[i] = Utility.RotateAroundPivot(m_dmesh.vertex[i], Vector3.Zero, Axis.Y, rotate_amt);
				}
			}
		}

		public void MarkCoplanarPolys()
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				SaveStateForUndo("Mark coplanar polys", false);
				SetEditModeSilent(EditMode.POLY);

				m_dmesh.TagAllPolys(false);
				m_dmesh.polygon[m_dmesh.selected_poly].tag = true;
				m_dmesh.TagCoplanarConnectedPolys(m_dmesh.polygon[m_dmesh.selected_poly], ((float)m_coplanar_tol) * Utility.RAD_90 / 90f, true);
				m_dmesh.MarkTaggedPolys();
				
				AddOutputText("Marked coplanar polys");
				RefreshGeometry();
			}
		}

		public void MarkConnectedPolys()
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				SaveStateForUndo("Mark connected polys", false);
				SetEditModeSilent(EditMode.POLY);

				m_dmesh.TagAllPolys(false);
				m_dmesh.polygon[m_dmesh.selected_poly].tag = true;
				m_dmesh.TagCoplanarConnectedPolys(m_dmesh.polygon[m_dmesh.selected_poly], Utility.RAD_360, true);
				m_dmesh.MarkTaggedPolys();

				AddOutputText("Marked connected polys");
				RefreshGeometry();
			}
		}

		public void MarkDuplicatePolys()
		{
			// Find all polygons that have 3 or more verts very close to each other
			// Tag one poly, mark the other
			SaveStateForUndo("Mark duplicate polys", false);
			SetEditModeSilent(EditMode.POLY);

			m_dmesh.MarkDuplicatePolys();

			RefreshGeometry();
		}

		public void FitFrameAll(bool all, GLView view)
		{
			if (all) {
				List<Vector3> pos_list = new List<Vector3>();
				for (int i = 0; i < 4; i++) {
					pos_list.Clear();
					pos_list = m_dmesh.AllVertexPositions();
					gl_view[i].FitFrame(pos_list);
				}
			} else {
				view.FitFrame(m_dmesh.AllVertexPositions());
			}
		}

		public void FitFrameMarkedSelected(bool all, GLView view)
		{
			if (all) {
				List<Vector3> pos_list = new List<Vector3>();
				for (int i = 0; i < 4; i++) {
					pos_list = m_dmesh.AllMarkedVertexPositions();
					gl_view[i].FitFrame(pos_list);
				}
			} else {
				view.FitFrame(m_dmesh.AllMarkedVertexPositions());
			}
		}

		public const float UV_SHIFT = 0.0625f;
		public const float UV_SHIFT_MINOR = UV_SHIFT / 16f;

		public void MoveMarkedSideTextures(Vector2 dir, bool minor)
		{
			SaveStateForUndo("Move textures");
			m_dmesh.UVMoveMarkedPoly(dir * (minor ? UV_SHIFT_MINOR : UV_SHIFT));
			RefreshGeometry();
		}

		public const float UV_ROT = Utility.RAD_45;
		public const float UV_ROT_MINOR = UV_ROT / 16f;

		public void RotateMarkedSideTextures(float angle, bool minor)
		{
			SaveStateForUndo("Rotate textures");
			m_dmesh.UVRotateMarkedPoly(angle * (minor ? UV_ROT_MINOR : UV_ROT));
			RefreshGeometry();
		}

		public Axis AxisFromViewType(ViewType vt)
		{
			switch (vt) {
				case ViewType.FRONT:
					return Axis.Z;
				case ViewType.RIGHT:
					return Axis.X;
				default:
				case ViewType.TOP:
					return Axis.Y;
			}
		}


		public void SnapMarkedElementsToGrid()
		{
			SaveStateForUndo("Snap marked elements to grid");
			m_dmesh.SnapMarkedToGrid();
			RefreshGeometry();
		}

		public void ScaleMarkedElements(GLView view, bool up, bool minor)
		{
			SaveStateForUndo("Scale marked elements");
			float amt = 1f;
			if (up) {
				amt = (minor ? 1.1f : 2f);
			} else {
				amt = (minor ? 1f / 1.1f : 0.5f);
			}
			m_dmesh.ScaleMarked(view, m_scale_mode, amt);
			RefreshGeometry();
		}

		public void ScaleMarkedElementsRaw(GLView view, float amtx = 1f, float amty = 1f)
		{
			if (m_scale_mode == ScaleMode.FREE_XY) {
				m_dmesh.ScaleMarked(view, ScaleMode.VIEW_X, amtx);
				m_dmesh.ScaleMarked(view, ScaleMode.VIEW_X, amty);
			} else {
				m_dmesh.ScaleMarked(view, m_scale_mode, amtx);
			}
			RefreshGeometry();
		}

		public void RotateMarkedElements(GLView view, float amt, bool minor)
		{
			SaveStateForUndo("Rotate marked elements");
			m_dmesh.RotateMarked(view, amt * (minor ? Utility.RAD_15 : Utility.RAD_45 ));
			RefreshGeometry();
		}

		public void RotateMarkedElementsRaw(GLView view, float amt)
		{
			m_dmesh.RotateMarked(view, amt);
			RefreshGeometry();
		}

		public void MoveMarkedElements(GLView view, Vector3 dir, bool minor)
		{
			SaveStateForUndo("Move marked elements");
			m_dmesh.MoveMarked(view, dir, m_grid_snap * (minor ? 0.125f : 1f));
			RefreshGeometry();
		}

		public void MoveMarkedElementsRaw(GLView view, Vector3 amt_dir)
		{
			m_dmesh.MoveMarkedRaw(view, amt_dir);
			RefreshGeometry();
		}

		public void AddPolygonFromMarkedVerts()
		{
			SaveStateForUndo("Add new polygon");
			m_dmesh.AddPolyFromMarkedVerts();
			RefreshGeometry();
		}

		public void AddVertAtMouse(GLView view)
		{
			SaveStateForUndo("Add new vertex");
			Vector3 pos = ScreenToWorldPos(view.m_mouse_pos, view);
			m_dmesh.AddVertexEditor(pos.X, pos.Y, pos.Z);
			RefreshGeometry();
		}

		public int m_cut_poly_state = 0;
		public int m_cut_polyvert_start = -1;
		public int m_cut_polyvert_end = -1;
		public static Vector3 POLYCUT_POS = Vector3.Zero;

		public void MaybeCutPolygon(GLView view, bool dont_cut = false)
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				DPoly dp = m_dmesh.polygon[m_dmesh.selected_poly];
				SaveStateForUndo("Maybe cut polygon");
				Vector3 pos = ScreenToWorldPos(view.m_mouse_pos, view, false);

				if (m_cut_poly_state == 0) {
					m_cut_polyvert_start = dp.GetClosestEdgeFV(view, m_dmesh, pos, -1, true);
					m_cut_poly_state = 1;
				} else {
					m_cut_polyvert_end = dp.GetClosestEdgeFV(view, m_dmesh, pos, m_cut_polyvert_start);
					
					int v1 = dp.vert[m_cut_polyvert_start];
					int v2 = dp.vert[(m_cut_polyvert_start + 1) % dp.num_verts];
					int v3 = dp.vert[m_cut_polyvert_end];
					int v4 = dp.vert[(m_cut_polyvert_end + 1) % dp.num_verts];

					// Add the verts
					DPoly.AddedVert = -1;
					dp.MaybeAddVertBetween(v1, v2, m_dmesh);
					int new_v0 = DPoly.AddedVert;
					DPoly.AddedVert = -1;
					dp.MaybeAddVertBetween(v3, v4, m_dmesh);
					int new_v1 = DPoly.AddedVert;

					m_cut_poly_state = 0;

					if (!dont_cut) {
						m_dmesh.SplitPolygonByVerts(dp, new_v0, new_v1);
					}

					AddOutputText("Cut a polygon");
				}
				RefreshGeometry(true);
			} else {
				AddOutputText("Select a polygon to cut");
			}
		}

		public void DuplicateMarkedPolysAlongAxis(bool x_axis)
		{
			SaveStateForUndo("Duplicate along axis");
			CopyMarkedPolys();
			m_dmesh.TagAllPolys(false);
			m_dmesh.CopyMarkedPolys(m_dmesh_buffer, true, true);
			FlipTaggedPolysAlongAxis(x_axis);
			RefreshGeometry();
		}

		public void DuplicateMarkedPolysAroundYAxis(int steps)
		{
			SaveStateForUndo("Duplicate along axis");
			CopyMarkedPolys();
			for (int i = 0; i < steps - 1; i++) {
				m_dmesh.TagAllPolys(false);
				m_dmesh.CopyMarkedPolys(m_dmesh_buffer, true, true);
				RotateTaggedPolysAroundYAxis((i + 1) * Utility.RAD_360 / steps);
			}
			RefreshGeometry();
		}

		public void FitUVsToQuarter(float offset_x, float offset_y)
		{
			SaveStateForUndo("Fit UVs to quarter");
			m_dmesh.FitUVsToQuarter(offset_x, offset_y);
			RefreshGeometry();
		}

		public void SubdivideMarkedPolys()
		{
			SaveStateForUndo("Subdivide marked polygons");
			m_dmesh.SubdividMarkedPolys();
			RefreshGeometry();
		}

		public Vector3 ScreenToWorldPos(Vector2 mouse_pos, GLView view, bool snap = true)
		{
			Vector3 pos = new Vector3(mouse_pos.X, mouse_pos.Y, 0f);

			pos.X = (2.0f * pos.X / (float)view.m_control_sz.X) - 1f;
			pos.Y = (2.0f * pos.Y / (float)view.m_control_sz.Y) - 1f;
			pos.Y *= -1f;

			pos = Vector3.Transform(pos, view.m_persp_mat.Inverted());
			pos = Vector3.Transform(pos, view.m_cam_mat.Inverted());
			switch (view.m_view_type) {
				case ViewType.TOP:
					pos = Vector3.Transform(pos, Matrix4.CreateRotationX(Utility.RAD_90));
					break;
				case ViewType.RIGHT:
					pos = Vector3.Transform(pos, Matrix4.CreateRotationY(-Utility.RAD_90));
					break;
				case ViewType.FRONT:
					break;
				case ViewType.PERSP:
					pos.Z *= -1f;
					// Project it forward, then project it to the XY plane
					Vector3 orig_pos = pos;
					Vector3 cam_forward = view.m_cam_mat.Column2.Xyz;
					cam_forward.X *= -1f;
					cam_forward.Y *= -1f;
					pos += cam_forward * (-view.m_cam_mat.Row3.Z - 1f);
					pos *= -view.m_cam_mat.Row3.Z;
					cam_forward = (pos - orig_pos).Normalized();
					pos -= cam_forward * (pos.Y / cam_forward.Y);
					break;
			}

			if (snap) {
				pos = Utility.SnapValue(pos, m_grid_snap);
			}
			
			return pos;
		}

		public void DeleteMarked()
		{
			SaveStateForUndo("Delete marked");
			//m_level.DeleteMarked();
			m_dmesh.DeleteMarked();
			RefreshGeometry();
		}

		public void ClearAllMarked()
		{
			SaveStateForUndo("Clear all marked", false);
			m_dmesh.ClearAllMarked();
			RefreshGeometry();
		}

		public void ToggleMarkAll()
		{
			SaveStateForUndo("Toggle mark all", false);
			m_dmesh.ToggleMarkAll();
			RefreshGeometry();
		}

		public void ToggleMarkSelected()
		{
			SaveStateForUndo("Toggle mark selected", false);
			m_dmesh.ToggleMarkSelected();
			RefreshGeometry();
		}

		public void ApplyTexture(string s, int idx)
		{
			SaveStateForUndo("Apply texture to poly(s)");
			m_dmesh.ApplyTexture(s, idx);
			RefreshGeometry();
		}

		public void MarkPolysWithTexture(string s)
		{
			SaveStateForUndo("Mark polys with texture");
			m_dmesh.MarkPolysWithTexture(s);
			RefreshGeometry();
		}

		public string GetSelectedPolyTexture()
		{
			return m_dmesh.GetSelectedPolyTexture();
		}

		public void ExtrudeSelectedPoly()
		{
			SaveStateForUndo("Extrude poly");
			m_dmesh.ExtrudeSelectedPoly(m_extrude_length);
			RefreshGeometry();
		}

		public void ExtrudeMarkedPolys()
		{
			SaveStateForUndo("Extrude polys");
			m_dmesh.ExtrudeMarkedPolys(m_extrude_length);
			RefreshGeometry();
		}

		public void ExtrudeMarkedVerts()
		{
			SaveStateForUndo("Extrude verts");
			m_dmesh.ExtrudeMarkedVerts(m_extrude_length);
			RefreshGeometry();
		}

		public void InsetBevelMarkedPolys()
		{
			SaveStateForUndo("Inset/bevel polys");
			m_dmesh.InsetExtrudeMarkedPolys(m_inset_pct * 0.01f, m_inset_length);
			RefreshGeometry();
		}

		public void SplitEdge()
		{
			SaveStateForUndo("Split edge(s)");
			m_dmesh.SplitEdge();
			RefreshGeometry();
		}

		public void BevelEdge()
		{
			SaveStateForUndo("Bevel edge(s)");
			//m_dmesh.BevelEdge();
			m_dmesh.BevelEdgeVerts();
			RefreshGeometry();
		}

		public void CombineMarkedVerts()
		{
			SaveStateForUndo("Combine marked verts");
			m_dmesh.CombineMarkedVerts();
			RefreshGeometry();
		}

		public void SplitPolygon()
		{
			SaveStateForUndo("Split polygon");
			m_dmesh.SplitPolygonWithMarkedVerts();
			RefreshGeometry();
		}

		public void BisectPolygon()
		{
			List<DPoly> markedPolys = m_dmesh.GetMarkedPolys();
			if (markedPolys.Count > 0)
			{
				Clipping.ClipPlane plane = null;
				if (m_dmesh.num_marked_verts == 3)
				{
					var markedVerts = m_dmesh.GetMarkedVerts(false).ConvertAll(vert => m_dmesh.vertex[vert]);
					if (Clipping.Clipper.CheckColinear(markedVerts[0], markedVerts[1], markedVerts[2]) == -1)
					{
						plane = Clipping.ClipPlane.CreateFrom3Points(markedVerts[0], markedVerts[1], markedVerts[2]);
					}
				}

				if (plane != null)
				{
					SaveStateForUndo("Bisect polygon");
					foreach (var poly in markedPolys)
					{
						m_dmesh.SplitPolygonByPlane(poly, plane);
					}
					RefreshGeometry();
				}
				else
				{
					AddOutputText("Bisect Polygon requires three non-collinear marked vertices");
				}
			}
		}

		public void UpdateTextureLabels()
		{
			label_texture_name.Text = m_dmesh.GetSelectedPolyTexture();
		}

		public void UpdateCountLabels()
		{
			label_count_total.Text = "Total: " + m_dmesh.polygon.Count.ToString() + "/" + m_dmesh.vertex.Count.ToString();
			label_count_marked.Text = "Marked: " + m_dmesh.num_marked_polys.ToString() + "/" + m_dmesh.num_marked_verts.ToString();
			label_count_selected.Text = "Selected: " + (m_dmesh.selected_poly < 0 ? "--" : m_dmesh.selected_poly.ToString()) + "/" +
				(m_dmesh.selected_vert < 0 ? "--" : m_dmesh.selected_vert.ToString());
		}

		public string[] m_output_strings = { "", "", "", "" };

		public void AddOutputText(string s)
		{
			string s2 = s.Replace("\n", Environment.NewLine);


			if (!s.EndsWith(Environment.NewLine)) {
				outputTextBox.AppendText(s2 + Environment.NewLine);
			} else {
				outputTextBox.AppendText(s2);
			}

			outputTextBox.Select(outputTextBox.Text.Length - 1, 0);
			outputTextBox.ScrollToCaret();
		}

		void NewDMesh()
		{
			this.m_filepath_current_decal = null;
			this.Text = "Overload DMesh Editor - " + "<New DMesh>";
			this.m_dmesh = new DMesh("default");
			this.m_dmesh.Init(this);
			AddOutputText("Created new dmesh");
		}

		public bool LoadDecalMesh(DMesh dm, string path_to_file)
		{
			dm.Init(this);
			try {
				string file_data = System.IO.File.ReadAllText(path_to_file);
				JObject root = JObject.Parse(file_data);

				dm.Deserialize(root);
				dm.UpdateGLTextures(tm_decal);

				this.m_filepath_current_decal = path_to_file;
				this.Text = "Overload DMesh Editor - " + path_to_file;

				AddOutputText(string.Format("Loaded dmesh: {0}", path_to_file));

				AddRecentFile(path_to_file);
				dm.dirty = false;

				if (!dm.WasConverted()) {
					m_dmesh.ConvertTrisToPolysRaw();
					dm.dirty = true;
				}
				UpdateOptionLabels();

				return true;
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to load decal mesh: " + ex.Message);
				return false;
			}
		}

		public bool SaveDecalMesh(DMesh dm, string path_to_file, bool silent = false)
		{
			try {
				JObject root = new JObject();
				dm.DeconvertPolysToTris();

				dm.Serialize(root);

				string new_file_data = root.ToString(Formatting.Indented);

				File.WriteAllText(path_to_file, new_file_data);

				if (!silent) {
					this.m_filepath_current_decal = path_to_file;
					this.Text = "Overload DMesh Editor - " + path_to_file;

					AddOutputText(string.Format("Saved decal: {0}", path_to_file));

					AddRecentFile(path_to_file);
					dm.dirty = false;
				}

				return true;
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to save decal mesh: " + ex.Message);
				return false;
			}
		}

		public const int RECENT_NUM = 4;
		public string[] m_recent_files = { "", "", "", "" };

		public void LoadRecentFile(int idx)
		{
			if (m_dmesh.dirty) {
				DialogResult dr = MessageBox.Show("DMesh has changed.  Save it before loading another DMesh?", "Save DMesh?", MessageBoxButtons.YesNoCancel);
				if (dr == DialogResult.Yes) {
					saveToolStripMenuItem_Click(this, EventArgs.Empty);
				} else if (dr == DialogResult.Cancel) {
					// Nevermind!
					return;
				}
			}

			string filename = m_recent_files[idx];
			if (File.Exists(filename)) {
				if (LoadDecalMesh(m_dmesh, filename)) {
					RefreshGeometry();
				}
			} else {
				Utility.DebugPopup("Could not find file.  Loading aborted", "LOADING ERROR");
			}
		}

		public void AddRecentFile(string filename)
		{
			if (MaybeRecentFilesSwap(filename)) {
				// Already swapped inside MaybeRecentFilesSwap
			} else {
				for (int i = RECENT_NUM - 1; i > 0; i--) {
					m_recent_files[i] = m_recent_files[i - 1];
				}
				m_recent_files[0] = filename;
			}

			UpdateRecentFileMenu();
		}

		public bool MaybeRecentFilesSwap(string filename)
		{
			for (int i = 0; i < RECENT_NUM; i++) {
				if (m_recent_files[i] == filename) {
					// Swap with the first item if it's in the list
					string s = m_recent_files[0];
					m_recent_files[0] = filename;
					m_recent_files[i] = s;

					return true;
				}
			}

			return false;
		}

		public void UpdateRecentFileMenu()
		{
			recent1ToolStripMenuItem.Enabled = recent1ToolStripMenuItem.Visible = (m_recent_files[0] != "");
			recent2ToolStripMenuItem.Enabled = recent2ToolStripMenuItem.Visible = (m_recent_files[1] != "");
			recent3ToolStripMenuItem.Enabled = recent3ToolStripMenuItem.Visible = (m_recent_files[2] != "");
			recent4ToolStripMenuItem.Enabled = recent4ToolStripMenuItem.Visible = (m_recent_files[3] != "");

			recent1ToolStripMenuItem.Text = m_recent_files[0];
			recent2ToolStripMenuItem.Text = m_recent_files[1];
			recent3ToolStripMenuItem.Text = m_recent_files[2];
			recent4ToolStripMenuItem.Text = m_recent_files[3];
		}

		public bool ExportDecalMeshToOBJ(DMesh dm, string path_to_file, bool silent = false)
		{
			try {
				string new_file_data = DMeshToString(dm, path_to_file);

				File.WriteAllText(path_to_file, new_file_data);

				if (!silent) {
					AddOutputText(string.Format("Exported decal to OBJ: {0}", path_to_file));
				}

				return true;
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to export decal mesh: " + ex.Message);
				return false;
			}
		}

		public static string DMeshToString(DMesh dm, string file_name)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("g ").Append(Utility.GetPathlessFilename(file_name).Split('.')[0]).Append("\n");
			foreach (Vector3 v in dm.vertex) {
				sb.Append(string.Format("v {0} {1} {2}\n", v.X, v.Y, v.Z));
			}
			sb.Append("\n");

			// Output the vert normals/UVs in order
			int idx = 0;
			foreach (DPoly dp in dm.polygon) {
				dp.export_list_idx.Clear();
				for (int i = 0; i < dp.num_verts; i++) {
					sb.Append(string.Format("vn {0} {1} {2}\n", dp.normal[i].X, dp.normal[i].Y, dp.normal[i].Z));
					sb.Append(string.Format("vt {0} {1}\n", dp.tex_uv[i].X, dp.tex_uv[i].Y));
					dp.export_list_idx.Add(idx);
					idx += 1;
				}	
			}

			idx = 0;
			foreach (String tx_name in dm.tex_name) {
				sb.Append("\n");
				string short_tex_name = Utility.GetPathlessFilename(tx_name);
				sb.Append("usemtl ").Append(short_tex_name + "\n");
				sb.Append("usemap ").Append(short_tex_name + "\n");

				foreach (DPoly dp in dm.polygon) {
					if (dp.tex_index == idx) {
						sb.Append("f ");
						for (int i = 0; i < dp.num_verts; i++) {
							sb.Append(string.Format("{0}/{1}/{1} ", dp.vert[i] + 1, dp.export_list_idx[i] + 1));
						}
						sb.Append("\n");
					}
				}
				idx += 1;
			}
			return sb.ToString();
		}

		// COLOR EDITING

		public int m_selected_color = 0;
		public Color[] m_editor_colors = new Color[DMesh.NUM_COLORS];
		public HSBColor m_hsb_selected;
		public Button m_button_selected = null;

		void NotifyEditorColorUpdated(int index = -1)
		{
			if (m_dmesh != null) {
				if (index == -1) {
					m_dmesh.color.Clear();
					m_dmesh.color = new List<Color>(m_editor_colors);
				} else if (m_dmesh.color.Count > index) {
					m_dmesh.color[index] = this.m_editor_colors[index];
				}
			}
		}

		public void UpdateSlidersFromSelectedColor()
		{
			Color c = m_editor_colors[m_selected_color];
			slider_color_red.ValueText = c.R.ToString();
			slider_color_green.ValueText = c.G.ToString();
			slider_color_blue.ValueText = c.B.ToString();

			slider_color_hue.ValueText = Utility.ConvertFloatTo1Dec(360f * m_hsb_selected.h);
			slider_color_saturation.ValueText = Utility.ConvertFloatTo1Dec(100f * m_hsb_selected.s);
			slider_color_brightness.ValueText = Utility.ConvertFloatTo1Dec(100f * m_hsb_selected.b);

			if (m_button_selected != null) {
				m_button_selected.BackColor = c;
			}
		}

		public void UpdateColorFromHSB()
		{
			m_editor_colors[m_selected_color] = m_hsb_selected.ToSystemColor();
			NotifyEditorColorUpdated();
		}

		public void UpdateHSBFromColor()
		{
			m_hsb_selected = HSBColor.FromColor(m_editor_colors[m_selected_color]);
		}

		public void SetButtonColorActive(Button b, int idx)
		{
			button_color1.ForeColor = SystemColors.ControlText;
			button_color2.ForeColor = SystemColors.ControlText;
			button_color3.ForeColor = SystemColors.ControlText;
			button_color4.ForeColor = SystemColors.ControlText;
			b.ForeColor = Color.Green;
			m_button_selected = b;

			m_selected_color = idx;
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
		}

		public void InitColors()
		{
			for (int i = 0; i < DMesh.NUM_COLORS; i++) {
				m_editor_colors[i] = Color.White;
			}
			UpdateColorButtons();
		}

		public Color m_copy_color;
		public Color[] m_copy_colors = new Color[DMesh.NUM_COLORS];

		public void CopySelectedColor()
		{
			m_copy_color = m_editor_colors[m_selected_color];
			button_color_paste.Enabled = true;
		}

		public void PasteToSelectedColor()
		{
			m_editor_colors[m_selected_color] = m_copy_color;
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			NotifyEditorColorUpdated(m_selected_color);
		}

		public void CopyAllColors()
		{
			for (int i = 0; i < (int)DMesh.NUM_COLORS; i++) {
				m_copy_colors[i] = m_editor_colors[i];
			}
			button_color_paste_all.Enabled = true;
		}

		public void PasteAllColors()
		{
			for (int i = 0; i < (int)DMesh.NUM_COLORS; i++) {
				m_editor_colors[i] = m_copy_colors[i];
			}
			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			UpdateColorButtons();
			NotifyEditorColorUpdated();
		}

		public void UpdateColorButtons()
		{
			button_color1.BackColor = m_editor_colors[0];
			button_color2.BackColor = m_editor_colors[1];
			button_color3.BackColor = m_editor_colors[2];
			button_color4.BackColor = m_editor_colors[3];
		}

		// LIGHT EDITING

		public Color Vec3ToColor(Vector3 vec)
		{
			Color c = Color.FromArgb(255, (int)vec.X, (int)vec.Y, (int)vec.Z);
			return c;
		}

		public Vector3 ColorToVec3(Color c)
		{
			Vector3 vec = new Vector3((float)c.R / 255f, (float)c.G / 255f, (float)c.B / 255f);
			return vec;
		}

		public int m_selected_light = 0;

		public void UpdateEditorColorsFromSelected()
		{
			for (int i = 0; i < m_dmesh.color.Count && i < DMesh.NUM_COLORS; ++i) {
				m_editor_colors[i] = m_dmesh.color[i];
			}

			for (int i = m_dmesh.color.Count; i < DMesh.NUM_COLORS; ++i) {
				m_editor_colors[i] = Color.Black;
			}

			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			UpdateColorButtons();
		}

		public void UpdateFaceCheckListFromSelected()
		{
			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
					bool flag_set = m_dmesh.polygon[m_dmesh.selected_poly].IsFlagSet((FaceFlags)Utility.Pow2(i));
					checklist_face.SetItemChecked(i, flag_set);
				}
			}
		}

		public void UpdateFaceFlagsFromCheckList()
		{
			int flags = 0;
			for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
				if (checklist_face.GetItemChecked(i)) {
					flags += Utility.Pow2(i);
				}
			}

			if (m_dmesh.selected_poly > -1 && m_dmesh.selected_poly < m_dmesh.polygon.Count) {
				m_dmesh.polygon[m_dmesh.selected_poly].flags = flags;
			}
			if (auto_copy_face_flags) {
				UpdateMarkedFaceFlags(flags);
			}

			RefreshGeometry(true);
		}

		public void ClearMarkedFaceFlags()
		{
			for (int i = 0; i < m_dmesh.polygon.Count; i++) {
				if (m_dmesh.polygon[i].marked) {
					m_dmesh.polygon[i].flags = 0;
				}
			}
		}

		public void CopyMarkedToFaceFlags(int flags)
		{
			for (int i = 0; i < m_dmesh.polygon.Count; i++) {
				if (m_dmesh.polygon[i].marked) {
					m_dmesh.polygon[i].flags = flags;
				}
			}
		}

		public void MarkFacesWithSameFlags(int flags)
		{
			for (int i = 0; i < m_dmesh.polygon.Count; i++) {
				if (m_dmesh.polygon[i].flags == flags) {
					m_dmesh.polygon[i].marked = true;
				}
			}
		}

		public void MarkFacesInvalidTexture()
		{
			for (int i = 0; i < m_dmesh.polygon.Count; i++) {
				if (m_dmesh.polygon[i].tex_index == -1 || m_dmesh.polygon[i].tex_index >= m_dmesh.m_tex_gl_id.Count) {
               m_dmesh.polygon[i].marked = true;
				}
			}
		}

		public void UpdateLightCheckList()
		{
			if (checklist_lights.Items.Count > 0) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					checklist_lights.SetItemChecked(i, m_dmesh.light[i].enabled);
				}
			}
		}

		public void UpdateLightProperties(DLight dl)
		{
			label_light_type.Text = "Style: " + dl.style.ToString();
			label_light_flare.Text = "Security: " + dl.flare.ToString();
			label_light_color.Text = "Color: " + (dl.color_index + 1).ToString();

			slider_light_intensity.ValueText = Utility.ConvertFloatTo1Dec(dl.intensity);
			slider_light_range.ValueText = Utility.ConvertFloatTo1Dec(dl.range);
			slider_light_angle.ValueText = ((int)dl.angle).ToString();

			// Z and Y are flipped on purpose due to how Decals are oriented
			slider_light_posx.ValueText = Utility.ConvertFloatTo1Thru3Dec(dl.position.X);
			slider_light_posy.ValueText = Utility.ConvertFloatTo1Thru3Dec(dl.position.Z);
			slider_light_posz.ValueText = Utility.ConvertFloatTo1Thru3Dec(dl.position.Y);

			slider_light_rot_yaw.ValueText = Utility.ConvertFloatTo1Dec(Utility.SnapValue(dl.rot_yaw / Utility.RAD_180 * 180, 1f));
			slider_light_rot_pitch.ValueText = Utility.ConvertFloatTo1Dec(Utility.SnapValue(dl.rot_pitch / Utility.RAD_180 * 180, 1f));
		}

		public DLight[] copy_lights = new DLight[DMesh.NUM_LIGHTS];
		public bool copied_lights = false;

		public void CopyLights()
		{
			for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
				copy_lights[i] = new DLight(m_dmesh.light[i]);
			}
			copied_lights = true;
		}

		public void PasteLights()
		{
			if (copied_lights) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					m_dmesh.light[i].Copy(copy_lights[i]);
				}
			}
		}

		public void InitChecklists()
		{
			for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
				checklist_face.Items.Add((FaceFlags)Utility.Pow2(i));
			}
			for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
				checklist_lights.Items.Add("Light " + i.ToString());
			}

			InitColors();
		}

		public bool ImportOBJ(DMesh dm, string path_to_file)
		{
			dm.Init(this);
			try {
				if (!OverloadLevelEditor.ImportOBJ.ImportOBJToDMesh(dm, path_to_file, false, this, tm_decal))
                    return false;

				dm.UpdateGLTextures(tm_decal);

				dm.dirty = false;

				if (!dm.WasConverted()) {
					m_dmesh.ConvertTrisToPolysRaw();
					dm.dirty = true;
				}
				UpdateOptionLabels();

				return true;
			}
			catch (Exception ex) {
				Utility.DebugPopup("Failed to load OBJ: " + ex.Message, "Error");
				return false;
			}
		}
	}
}
