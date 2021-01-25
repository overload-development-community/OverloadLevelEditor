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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using ObjLoader.Loader.Loaders;

// DECALLIST - Primary
// The window that shows the list of decals
// Could be cleaned up and separated more (see DecalListUtility too)

namespace OverloadLevelEditor
{
	public enum DViewMode
	{
		SOLID,
		TEXTURE,
		WIRE,

		NUM,
	}

	public partial class DecalList : Form
	{
		// PInvoke
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, [MarshalAs(UnmanagedType.LPWStr)] string lp);

		public Editor editor;
		public TextureManager tex_manager;

		public bool m_gl_loaded = false;
		public Vector2 m_control_sz;

		public const int GL_DECAL = 50;
		public const int GL_GRID = 51;
		public const int GL_SELECTED = 52;
		public const int GL_LIGHT = 53;
		public const int GL_LIGHT_CONE = 54;
		public const int GL_VERT_NORMALS = 55;
		public DViewMode m_view_mode = DViewMode.TEXTURE;
		public float m_nudge_amount = 0.125f;
		public bool m_units_inches = true;

		public List<DMesh> m_dmesh;
		public int m_cur_dmesh = -1;
		
		public int m_selected_face = -1;
		public int m_selected_color = 0;
		public int m_selected_light = 0;

		private List<string> m_decal_list = new List<string>();
		private List<bool> m_decal_readonly = new List<bool>();

		public DecalList(Editor e)
		{
			editor = e;
			InitializeComponent();
			MinimumSize = new Size(0, 0);
			//MaximumSize = new Size(Width, 2048);
		}

		public void DecalList_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSelectedDecalMesh();
			if (e.CloseReason != CloseReason.FormOwnerClosing) {
				this.Hide();
				e.Cancel = true;
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			UpdateActiveDMesh();
		}

		private void DecalList_MouseEnter(object sender, EventArgs e)
		{
			gl_custom.Focus();
		}

		private void gl_custom_MouseEnter(object sender, EventArgs e)
		{
			gl_custom.Focus();
		}

		private void listbox_MouseEnter(object sender, EventArgs e)
		{
			//Disabled this because it was tending to take away focus from the filter textbox
			//listbox.Focus();
		}

		private void ListboxUpdate(string selected_item = null)
		{
			string filter_text = textBox_filter.Text;
			if (selected_item == null) {
				selected_item = (string)listbox.SelectedItem;
			}

			listbox.BeginUpdate();

			listbox.Items.Clear();

			if (string.IsNullOrEmpty(filter_text)) {
				foreach (string s in m_decal_list) {
					listbox.Items.Add(s);
				}
			}
			else {
            foreach (string s in m_decal_list) {
					if (System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, filter_text, System.Globalization.CompareOptions.IgnoreCase) >= 0) { 
						listbox.Items.Add(s);
					}
				}
			}

			//Try to select the old selected
			listbox.SelectedItem = selected_item;
			if (listbox.SelectedItem == null) {    //Failed to find old item
				if (listbox.Items.Count == 0) {
					listbox_SelectedIndexChanged(null, null);
            }
				else {
					listbox.SelectedIndex = 0;
				}
			}

			listbox.EndUpdate();
		}

		public void InitTexturesAndDecals()
		{
			// Load all the decals (has to wait for OpenGL to init)
			tex_manager = new TextureManager(editor);
			tex_manager.LoadTexturesInDir(editor.m_filepath_decal_textures, false, true);

			// Add decals from the decal directory
			LoadDecalsInDir(editor.m_filepath_decals, false);

			//Update listbox
			ListboxUpdate();
		}

		private void DecalList_Load(object sender, EventArgs e)
		{
			for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
				checklist_face.Items.Add((FaceFlags)Utility.Pow2(i));
			}
			for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
				checklist_lights.Items.Add("Light " + i.ToString());
			}

			if (listbox.Items.Count > 0)
			{
				listbox.SelectedIndex = 0;
			}

			InitColors();

			//Set cue text
			if (textBox_filter.IsHandleCreated) {
				SendMessage(textBox_filter.Handle, 0x1501, (IntPtr)1, "Filter Text");
			}
		}

		float m_cam_distance = 7f;
		Vector2 m_cam_angles = new Vector2();
		Matrix4 m_persp_mat;
		Matrix4 m_persp_mat2;
		Matrix4 m_cam_mat;
		Matrix4 m_cam_mat2;
		public bool m_show_vert_normals = false;

		private void gl_custom_Paint(object sender, PaintEventArgs e)
		{
			if (!m_gl_loaded) {
				return;
			}

			// GL Setup
			gl_custom.MakeCurrent();
			GL.ClearColor(GLView.C_bg);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Viewport(0, 0, (int)m_control_sz.X, (int)m_control_sz.Y);

			// Perspective mode
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			m_persp_mat = Matrix4.CreatePerspectiveFieldOfView(Utility.RAD_45 * 1.0f, m_control_sz.X / m_control_sz.Y, 0.3f, 2000f);
			m_persp_mat2 = Matrix4.CreatePerspectiveFieldOfView(Utility.RAD_45 * 1.0f, m_control_sz.X / m_control_sz.Y, 0.3f, 2000f);
			GL.LoadMatrix(ref m_persp_mat);

			// Camera position
			Vector3 cam_pos = Vector3.Transform(Vector3.UnitZ * m_cam_distance, Matrix4.CreateRotationX(m_cam_angles.X) * Matrix4.CreateRotationY(m_cam_angles.Y));
			m_cam_mat = Matrix4.LookAt(cam_pos, Vector3.Zero, Vector3.UnitY);
			Vector3 cam_pos2 = Vector3.Transform(Vector3.UnitZ * m_cam_distance * 1.0f, Matrix4.CreateRotationX(m_cam_angles.X) * Matrix4.CreateRotationY(m_cam_angles.Y));
			m_cam_mat2 = Matrix4.LookAt(cam_pos2, Vector3.Zero, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref m_cam_mat);

			GL.Scale(1f, 1f, -1f);

			// Draw the grid
			if (m_view_mode == DViewMode.WIRE) {
				GL.Disable(EnableCap.DepthTest);
			}
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.Texture2D);
			GL.CallList(GL_GRID);

			// Rotate the decal to be XY for viewing (XZ is better for editing)
			GL.Rotate(-90, Vector3.UnitX);

			GL.CullFace(CullFaceMode.Front);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.PolygonOffsetFill);
			GL.Enable(EnableCap.PolygonOffsetLine);
			GL.PolygonOffset(1f, 1f);
			if (m_view_mode != DViewMode.WIRE) {
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
				GL.Enable(EnableCap.DepthTest);
				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.Light0);
				if (m_view_mode == DViewMode.TEXTURE) {
					GL.Enable(EnableCap.Texture2D);
				}
			} else {
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.Disable(EnableCap.Lighting);
			}

			if (m_decal_loaded) {
				GL.CallList(GL_DECAL);
			}
			GL.Disable(EnableCap.PolygonOffsetLine); 
			GL.Disable(EnableCap.PolygonOffsetFill);

			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.Texture2D);

			if (m_decal_loaded) {
				// Lights
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					if (m_active_dmesh.light[i].enabled) {
						DLight dl = m_active_dmesh.light[i];
						if (i == m_selected_light) {
							GL.Color3(Color.GreenYellow);
						} else {
							GL.Color3(Color.LightYellow);
						}
						GL.PushMatrix();
						GL.Translate(dl.position);
						GL.CallList(GL_LIGHT);
						if (dl.style == LightStyle.SPOT) {
							var light_rotation = dl.rotation;
							GL.MultMatrix(ref light_rotation);
							GL.CallList(GL_LIGHT_CONE);
						}
						GL.PopMatrix();
					}
				}
			}

			if (m_selected_face > -1) {
				// Selected face
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_SELECTED);
			}

			if (m_show_vert_normals) {
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_VERT_NORMALS);
			}
			
			GL.End();

			gl_custom.SwapBuffers();
		}

		private void gl_custom_Load(object sender, EventArgs e)
		{
			m_gl_loaded = true;
			GraphicsContext.ShareContexts = false;

			InitTexturesAndDecals();

			// Lighting setup
			float[] light_pos = { 5f, 25f, 100f, 0f };

			GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, 0f);
			GL.Light(LightName.Light0, LightParameter.Position, light_pos);
			GL.Light(LightName.Light0, LightParameter.Diffuse, GLView.C_light);
			GL.Light(LightName.Light0, LightParameter.Ambient, GLView.C_ambient_solid);

			// Smooth lines (too many weird artificacts)
			/*GL.Enable(EnableCap.LineSmooth);
			GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);*/
			
			m_control_sz = new Vector2(gl_custom.Width, gl_custom.Height);

			RefreshGrid();
			BuildLight();
			BuildLightCone();
		}

		Vector2 m_mouse_pos;
		Vector2 m_mouse_pos_prev;
		
		private void gl_custom_MouseDown(object sender, MouseEventArgs e)
		{
			m_mouse_pos.X = e.X + 1f;
			m_mouse_pos.Y = e.Y + 1f;
			m_mouse_pos_prev = m_mouse_pos;
			if (e.Button == MouseButtons.Left && m_active_dmesh != null) {
				// Pick a face
				Utility.DebugLog(m_mouse_pos.ToString()); 
				m_selected_face = m_active_dmesh.SelectFace(m_mouse_pos, m_cam_mat2, m_persp_mat2, m_control_sz);
				BuildDecalSelected(m_active_dmesh, m_selected_face);
				gl_custom.Invalidate();
				UpdateFaceCheckListFromSelected();
			}	
		}

		private void gl_custom_MouseMove(object sender, MouseEventArgs e)
		{
			m_mouse_pos.X = e.X + 2f;
			m_mouse_pos.Y = e.Y + 2f;

			switch (e.Button) {
				case MouseButtons.Middle:
				case MouseButtons.Right:
					// Limit the rotation amount (mostly because of importing dialogs)
					Vector2 diff = m_mouse_pos - m_mouse_pos_prev;
					if ((diff).Length > 20f) {
						diff = diff.Normalized() * 20f;
					}
					m_cam_angles.X += (diff.Y) * -0.005f;
					m_cam_angles.Y += (diff.X) * -0.005f;
					m_cam_angles.X = Math.Min(Utility.PI * 0.49f, Math.Max(Utility.PI * -0.49f, m_cam_angles.X));
					gl_custom.Invalidate();
					break;
			}
			m_mouse_pos_prev = m_mouse_pos;
		}

		private void gl_custom_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				m_cam_distance = Math.Max(3f, m_cam_distance - 0.5f);
			} else {
				m_cam_distance = Math.Min(12f, m_cam_distance + 0.5f);
			}
			gl_custom.Invalidate();
		}

		private void label_view_mode_MouseDown(object sender, MouseEventArgs e)
		{
			CycleViewMode();
		}

		private void button_import_Click(object sender, EventArgs e)
		{
			ImportOBJToDecalDialog(false);
		}

		public void ImportOBJToDecalDialog(bool replace)
		{
			int old_active_dmesh_index = m_decal_list.IndexOf((string)m_active_dmesh.name);

			using (OpenFileDialog od = new OpenFileDialog()) {
				od.AddExtension = true;
				od.CheckFileExists = true;
				od.CheckPathExists = true;
				od.DefaultExt = ".obj";
				od.Filter = "OBJ mesh files (*.obj) | *.obj";
				od.Multiselect = false;
				od.Title = "Import an OBJ mesh file";
				od.InitialDirectory = editor.m_filepath_decals + "\\OBJ";

				DialogResult res = od.ShowDialog();
				if (res != DialogResult.OK) {
					return;
				}

				string name = m_active_dmesh.name;
				if (!replace)
					m_active_dmesh = new DMesh("");

				bool saved = ImportOBJToDecal(od.FileName);
				if (!saved) {
					listbox.SetSelected(old_active_dmesh_index, true);
					return;
				}

				if (replace) {
					m_active_dmesh.name = name;
					SaveDecalMesh(m_active_dmesh);
					editor.RefreshAllGMeshes();
					editor.Refresh();
				} else { 
					name = InputBox.GetInput("Import Decal Mesh", "Enter a name for this decal (must be valid filename)", "");
					if (name == null) {
						listbox.SetSelected(old_active_dmesh_index, true);
						return;
					}
					if (m_decal_list.IndexOf((string)name) != -1) {
						MessageBox.Show("Name '" + name + "' already used for a decal.");
						listbox.SetSelected(old_active_dmesh_index, true);
						return;
					}
					m_active_dmesh.name = name;
					SaveDecalMesh(m_active_dmesh);
					m_dmesh.Add(m_active_dmesh);
					m_decal_list.Add(m_active_dmesh.name);
					m_decal_readonly.Add(false);
					ListboxUpdate(m_active_dmesh.name);
				}

				UpdateActiveDMesh();
				gl_custom.Invalidate();
			}
		}

		private void button_apply1_Click(object sender, EventArgs e)
		{
			editor.ApplyDecal(m_active_dmesh, 0);
		}

		private void button_apply2_Click(object sender, EventArgs e)
		{
			editor.ApplyDecal(m_active_dmesh, 1);
		}

		private void button_flip_u_Click(object sender, EventArgs e)
		{
			m_active_dmesh.FlipMesh(-1f, 1f);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_flip_v_Click(object sender, EventArgs e)
		{
			m_active_dmesh.FlipMesh(1f, -1f);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_center_Click(object sender, EventArgs e)
		{
			m_active_dmesh.CenterMesh();
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_edge_top_Click(object sender, EventArgs e)
		{
			bool[] edges = { false, false, false, true };
			m_active_dmesh.AlignMeshEdges(edges);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_edge_left_Click(object sender, EventArgs e)
		{
			bool[] edges = { false, false, true, false };
			m_active_dmesh.AlignMeshEdges(edges);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_edge_bottom_Click(object sender, EventArgs e)
		{
			bool[] edges = { false, true, false, false };
			m_active_dmesh.AlignMeshEdges(edges);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_edge_right_Click(object sender, EventArgs e)
		{
			bool[] edges = { true, false, false, false };
			m_active_dmesh.AlignMeshEdges(edges);
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void button_rotate90_Click(object sender, EventArgs e)
		{
			m_active_dmesh.RotateMesh90();
			BuildDecalMesh(m_active_dmesh);
			this.Refresh();
		}

		private void listbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_cur_dmesh = m_decal_list.IndexOf((string)listbox.SelectedItem);
			if (m_cur_dmesh > -1) {
				SaveSelectedDecalMesh();
				m_active_dmesh = m_dmesh[m_cur_dmesh];
				UpdateActiveDMesh();

				//Enable/disable edit controls based on read-only status
				panel2.Enabled = panel4.Enabled = panel5.Enabled = !m_decal_readonly[m_cur_dmesh];
			} else {
				m_decal_loaded = false;
				this.Refresh();
			}

			//Update buttons enabled
			button_apply1.Enabled = button_apply2.Enabled = (listbox.SelectedIndex != -1);
		}

		public void UpdateActiveDMesh()
		{
			if (m_active_dmesh == null)
            {
				return;
            }
			m_active_dmesh.UpdateGLTextures(tex_manager, editor.tm_level);
			BuildDecalMesh(m_active_dmesh);
			m_selected_face = -1;
			BuildDecalSelected(m_active_dmesh, m_selected_face);
			BuildDecalVertNormals(m_active_dmesh);
			this.Refresh();
			UpdateFaceCheckListFromSelected();
			UpdateEditorColorsFromSelected();
			UpdateLightCheckList();
			if (checklist_lights.Items.Count > 0) {
				checklist_lights.SetSelected(0, true);
			}
			//UpdateLightProperties(active_dmesh.light[0]);
			m_decal_loaded = true;
		}

		private void button_import_replace_Click(object sender, EventArgs e)
		{
			m_active_dmesh = m_dmesh[m_cur_dmesh];
			ImportOBJToDecalDialog(true);
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

		private void button_face_clear_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checklist_face.Items.Count; i++) {
				checklist_face.SetItemChecked(i, false);
			}
			UpdateFaceFlagsFromCheckList();
		}

		public bool[] m_face_buffer;

		private void button_face_copy_Click(object sender, EventArgs e)
		{
			m_face_buffer = new bool[checklist_face.Items.Count];

			for (int i = 0; i < checklist_face.Items.Count; i++) {
				m_face_buffer[i] = checklist_face.GetItemChecked(i);
			}
			button_face_paste.Enabled = true;
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

		private void button_select_adjacent_Click(object sender, EventArgs e)
		{
			SelectAdjacentFace();
		}

		private void checklist_face_MouseUp(object sender, MouseEventArgs e)
		{
			UpdateFaceFlagsFromCheckList();
		}

		
		private void label_light_type_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.CycleStyle();
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		private void label_light_flare_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.CycleFlare();
				UpdateLightProperties(dl);
			}
		}

		private void label_light_color_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.CycleColorIndex();
				UpdateLightProperties(dl);
			}
		}

		private void slider_light_intensity_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				float inc = (dl.intensity < 2f ? 0.1f : 0.5f);
				dl.intensity = Utility.SnapValue(dl.intensity + e.Increment * inc, inc);
				dl.intensity = Utility.Clamp(dl.intensity, 0f, 50f);
				UpdateLightProperties(dl);
			}
		}
		
		private void slider_light_range_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.range = Utility.SnapValue(dl.range + e.Increment * 0.5f, 0.5f);
				dl.range = Utility.Clamp(dl.range, 0f, 50f);
				UpdateLightProperties(dl);
			}
		}

		private void slider_light_angle_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.angle = Utility.SnapValue(dl.angle + e.Increment * 5, 5f);
				dl.angle = Utility.Clamp(dl.angle, 0f, 160f);
				UpdateLightProperties(dl);
			}
		}

		private void checklist_lights_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_selected_light = Utility.Clamp(checklist_lights.SelectedIndex, 0, 3);
			if (m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		private void checklist_lights_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_active_dmesh != null) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					m_active_dmesh.light[i].enabled = checklist_lights.GetItemChecked(i);
				}
				gl_custom.Invalidate();
			}
		}

		private void DecalList_KeyDown(object sender, KeyEventArgs e)
		{
			ProcessKeyboard(e);
		}

		private void gl_custom_KeyDown(object sender, KeyEventArgs e)
		{
			ProcessKeyboard(e);
		}

		private void button_light_reset_pos_Click(object sender, EventArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.position = Vector3.Zero;
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		private void button_light_reset_rot_Click(object sender, EventArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.rot_yaw = 0f;
				dl.rot_pitch = 0f;
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		public float POS_SNAP = 0.03125f;

		private void slider_light_posx_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.position.X = Utility.SnapValue(dl.position.X + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		// NOTE: Z and Y are flipped on purpose (because of how decals are oriented vs. displayed)
		private void slider_light_posy_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.position.Z = Utility.SnapValue(dl.position.Z + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		// NOTE: Z and Y are flipped on purpose (because of how decals are oriented vs. displayed)
		private void slider_light_posz_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.position.Y = Utility.SnapValue(dl.position.Y + e.Increment * POS_SNAP, POS_SNAP);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		public float ROT_SNAP = Utility.RAD_180 / 36f; // 5 degrees

		private void slider_light_rot_yaw_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.rot_yaw = Utility.SnapValue(dl.rot_yaw + e.Increment * ROT_SNAP, ROT_SNAP);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		private void slider_light_rot_pitch_Feedback(object sender, SliderLabelArgs e)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.rot_pitch = Utility.SnapValue(dl.rot_pitch + e.Increment * ROT_SNAP, ROT_SNAP);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		private void DecalList_LocationChanged(object sender, EventArgs e)
		{
			if (Visible) {
				editor.m_decal_list_loc = Location;
			}
		}

		private void slider_nudge_amount_Feedback(object sender, SliderLabelArgs e)
		{
			if (e.Increment > 0) {
				m_nudge_amount = Math.Min(1f, m_nudge_amount * 2f);
			} else {
				m_nudge_amount = Math.Max(1f / 128f, m_nudge_amount * 0.5f);
			}

			slider_nudge_amount.ValueText = Utility.ConvertFloatTo1Thru3Dec(m_nudge_amount);
		}

		private void button_nudge_left_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(-Vector3.UnitX * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		private void button_nudge_right_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(Vector3.UnitX * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		// Z/Y switched on purpose
		private void button_nudge_up_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(Vector3.UnitZ * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		// Z/Y switched on purpose
		private void button_down_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(-Vector3.UnitZ * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		// Z/Y switched on purpose
		private void button_nudge_fore_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(Vector3.UnitY * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		// Z/Y switched on purpose
		private void button_nudge_back_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.NudgeMesh(-Vector3.UnitY * m_nudge_amount);
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		private void label_units_MouseDown(object sender, MouseEventArgs e)
		{
			m_units_inches = !m_units_inches;
			label_units.Text = "OBJ Units: " + (m_units_inches ? "INCHES" : "METERS");
		}

		private void button_scale_to_borders_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				ScaleMeshToBorders();
				BuildDecalMesh(m_active_dmesh);
				this.Refresh();
			}
		}

		private void button_mark_sides1_Click(object sender, EventArgs e)
		{
			editor.SaveStateForUndo("Mark decal sides");
			editor.m_level.MarkSidesWithDecal(0, m_active_dmesh.name);
			editor.RefreshGeometry();
		}

		private void button_mark_sides2_Click(object sender, EventArgs e)
		{
			editor.SaveStateForUndo("Mark decal sides");
			editor.m_level.MarkSidesWithDecal(1, m_active_dmesh.name);
			editor.RefreshGeometry();
		}

		private void button_get_decal1_Click(object sender, EventArgs e)
		{
			string decal_name = editor.m_level.GetDecalFromSide(0);
			if (!string.IsNullOrEmpty(decal_name)) {
				int decal_idx = listbox.Items.IndexOf(decal_name);
				if (decal_idx > -1 && decal_idx < listbox.Items.Count) {
					listbox.SelectedIndex = decal_idx;
				} else {
					MessageBox.Show("Can't get decal because it's not in filtered list.");
				}
			}
		}

		private void button_get_decal2_Click(object sender, EventArgs e)
		{
			string decal_name = editor.m_level.GetDecalFromSide(1);
			if (! string.IsNullOrEmpty(decal_name)) {
				int decal_idx = listbox.Items.IndexOf(decal_name);
				if (decal_idx > -1 && decal_idx < listbox.Items.Count) {
					listbox.SelectedIndex = decal_idx;
				} else {
					MessageBox.Show("Can't get decal because it's not in filtered list.");
				}
			}
		}

		private void button_clear_filter_Click(object sender, EventArgs e)
		{
			this.textBox_filter.Text = "";
		}

		private void textBox_filter_TextChanged(object sender, EventArgs e)
		{
			ListboxUpdate();

		}

		private void textBox_filter_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			//Clear text if escape pressed
			if (e.KeyChar == 27) {
				this.textBox_filter.Text = "";
				e.Handled = true;
			}
		}

		private void button_flip_vert_normals_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.FlipVertNormalsZ();
				BuildDecalMesh(m_active_dmesh);
				BuildDecalVertNormals(m_active_dmesh);
				this.Refresh();
			}
		}

		private void button_toggle_vert_normals_Click(object sender, EventArgs e)
		{
			m_show_vert_normals = !m_show_vert_normals;
		}

		private void button_lights_copy_Click(object sender, EventArgs e)
		{
			CopyLights();
      }

		private void button_lights_paste_Click(object sender, EventArgs e)
		{
			PasteLights();
			this.Refresh();
		}

		private void button_flip_vert_normals_X_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.FlipVertNormalsX();
				BuildDecalMesh(m_active_dmesh);
				BuildDecalVertNormals(m_active_dmesh);
				this.Refresh();
			}
		}

		private void button_flip_vert_normals_Y_Click(object sender, EventArgs e)
		{
			if (m_active_dmesh != null) {
				m_active_dmesh.FlipVertNormalsY();
				BuildDecalMesh(m_active_dmesh);
				BuildDecalVertNormals(m_active_dmesh);
				this.Refresh();
			}
		}

		private void button_copy_Click(object sender, EventArgs e)
		{
			string name = InputBox.GetInput("Copy Decal", "Enter a name for this decal (must be valid filename)", "");
			if (name == null) {
				return;
			}
			if (m_decal_list.IndexOf((string)name) != -1) {
				MessageBox.Show("Name '" + name + "' already used for a decal.");
				return;
			}

			DMesh src = m_active_dmesh;
			m_active_dmesh = new DMesh("");
			m_active_dmesh.CopyDMesh(src);

			m_active_dmesh.name = name;
			SaveDecalMesh(m_active_dmesh);
			m_dmesh.Add(m_active_dmesh);
			m_decal_list.Add(m_active_dmesh.name);
			m_decal_readonly.Add(false);
			ListboxUpdate(m_active_dmesh.name);

			UpdateActiveDMesh();
			gl_custom.Invalidate();
		}
	}
}
