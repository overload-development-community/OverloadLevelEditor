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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using ObjLoader.Loader.Loaders;

// DECALLIST - Utility
// Lots of various utility functions for the DecalList
// Could probably be split into saving/loading, drawing, and general utility at some point

namespace OverloadLevelEditor
{
	public partial class DecalList : Form
	{
		public bool m_decal_loaded = false;
		
		// Temporary variables for importing from OBJ
		public LoadResult m_i_load_result;
		public DMesh m_active_dmesh;

		public void LoadDecalsInDir(string dir, bool all_dir = false)
		{
			if (!Directory.Exists(dir))
            {
				return;
            }
			string[] files = Directory.GetFiles(dir, "*.dmesh", (all_dir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

			m_dmesh = new List<DMesh>();
			foreach (string file in files) {
				// Remove all the extra stuff to get the names
				string mesh_name = Utility.GetRelativeExtensionlessFilenameFromDirectory(dir, file);
				m_decal_list.Add(mesh_name);
				m_decal_readonly.Add(m_builtin_decal_list.Contains(mesh_name) ? true : false);

				// Load the decal into memory
				m_active_dmesh = new DMesh(mesh_name);
				LoadDecalMesh(m_active_dmesh, file);
				m_active_dmesh.UpdateGLTextures(tex_manager);
				m_dmesh.Add(m_active_dmesh);
			}

			UpdateActiveDMesh();
		}

		public bool ImportOBJToDecal(string obj_file_name)
		{
			if( !Path.IsPathRooted(obj_file_name) ) {
				// Make sure the file path is absolute
				obj_file_name = Path.GetFullPath(obj_file_name);
			}

			if( !File.Exists( obj_file_name ) ) {
				return false;
			}

			string obj_working_dir = Path.GetDirectoryName(obj_file_name);

			string current_working_directory = Directory.GetCurrentDirectory();
			try {
				ObjLoaderFactory obj_loader_factory = new ObjLoaderFactory();
				IObjLoader obj_loader = obj_loader_factory.Create();

				// Have to set working directory so loading of the material file will work
				Directory.SetCurrentDirectory(obj_working_dir);
				using (FileStream file_stream = new FileStream(obj_file_name, FileMode.Open, FileAccess.Read)) {
					m_i_load_result = obj_loader.Load(file_stream);
					file_stream.Close();
				}

				m_decal_loaded = true;

				ConvertLoadResultToDMesh();
				//m_active_dmesh.CenterMesh();
				m_active_dmesh.FlipMesh(-1f, 1f);
				m_active_dmesh.FlipVertNormalsZ();
				//active_dmesh.RotateMesh90();

				UpdateActiveDMesh();
			}
			finally {
				// Restore the working directory
				Directory.SetCurrentDirectory( current_working_directory );
			}

			gl_custom.Invalidate();

			return true;
		}

		public void ScaleMeshToBorders()
		{
			float max_scale = 2f;
			for (int i = 0; i < m_active_dmesh.vertex.Count; i++) {
				max_scale = Math.Max(max_scale, Math.Abs(m_active_dmesh.vertex[i].X));
				max_scale = Math.Max(max_scale, Math.Abs(m_active_dmesh.vertex[i].Y));
				max_scale = Math.Max(max_scale, Math.Abs(m_active_dmesh.vertex[i].Z));
			}

			if (max_scale > 2f) {
				for (int i = 0; i < m_active_dmesh.vertex.Count; i++) {
					m_active_dmesh.vertex[i] *= (2f / max_scale);
				}
			}
		}

		public void ConvertLoadResultToDMesh()
		{
			// Convert the verts
			ObjLoader.Loader.Data.VertexData.Vertex v;
			for (int i = 0; i < m_i_load_result.Vertices.Count; i++) {
				v = m_i_load_result.Vertices[i];
				m_active_dmesh.AddVertex(v.X, v.Y, v.Z);
			}

			if (m_units_inches) {
				for (int i = 0; i < m_active_dmesh.vertex.Count; i++) {
					m_active_dmesh.vertex[i] *= 0.0254f;
				}
			}

			// Rotate all the verts 180
			m_active_dmesh.RotateMesh180();
			
			// Convert the faces
			int[] vrt_idx = new int[3];
			Vector3[] nrml = new Vector3[3];
			Vector2[] uv = new Vector2[3];
			string tex_name;

			int tex_idx = 0;
			
			ObjLoader.Loader.Data.VertexData.Normal n;
			ObjLoader.Loader.Data.VertexData.Texture t;
			ObjLoader.Loader.Data.Elements.FaceVertex fv;

			foreach (ObjLoader.Loader.Data.Elements.Group g in m_i_load_result.Groups) {
				tex_name = g.Material.DiffuseTextureMap;
				if (!CopyImportTextureToDecalFolder(tex_name)) {
					Utility.DebugLog("No texture imported/updated: " + tex_name);
				} else {
					Utility.DebugLog("Imported/updated a texture: " + tex_name);
				}

				string tex_id_name = Path.ChangeExtension(tex_name, null);
				if (tex_manager.FindTextureIDByName(tex_id_name) < 0) {
					// Get the texture (if it exists)
					if (File.Exists(tex_id_name + ".png")) {
						tex_manager.LoadTexture(tex_id_name + ".png", true);
					} else {
						// Try to steal it from the level directory instead
						string level_tex_name = editor.m_filepath_level_textures + "\\" + tex_id_name + ".png";
						if (File.Exists(level_tex_name)) {
							// Copy it to the decal textures directory, then load it
							string decal_tex_name = editor.m_filepath_decal_textures + "\\" + tex_id_name + ".png";
							if (!File.Exists(decal_tex_name)) {
								File.Copy(level_tex_name, decal_tex_name);

								tex_manager.LoadTexture(decal_tex_name, true);
							}
						} else {
							Utility.DebugPopup("No PNG file could be found matching the name: " + tex_id_name, "IMPORT WARNING!");
						}
					}
				}

				m_active_dmesh.AddTexture(tex_idx, tex_id_name);
				foreach (ObjLoader.Loader.Data.Elements.Face f in g.Faces) {
					// Only works for 3 vert faces (currently)
					for (int i = 0; i < f.Count; i++) {
						if (i < 3) {
							fv = f[i];
							vrt_idx[i] = fv.VertexIndex - 1;
							n = m_i_load_result.Normals[fv.NormalIndex - 1];
							t = m_i_load_result.Textures[fv.TextureIndex - 1];
							nrml[i] = new Vector3(n.X, n.Y, n.Z);

                            // Since we use OpenGL for rendering, but don't load
                            // the texture images in upside down (as OpenGL expects)
                            // we must flip the V coordinate of the UVs so that decals
                            // render correctly.
                            //
                            // What I don't understand is why the U coordinates of geometry
                            // decals need to be flipped also. I wonder if this is the
                            // in the WaveFront OBJ specification or something else.
                            // uv[i] = new Vector2(1.0f - t.X, 1.0f - t.Y);
									 uv[i] = new Vector2(t.X, 1f - t.Y);
						}
					}

					m_active_dmesh.AddFace(vrt_idx, nrml, uv, tex_idx);
				}

				tex_idx += 1;
			}
		}

		public bool CopyImportTextureToDecalFolder(string name)
		{
			string dest_file = Path.Combine( editor.m_filepath_decal_textures, name );
			string src_file = Path.Combine( Directory.GetCurrentDirectory(), name );
			if (dest_file == src_file) {
				// No need to do anything
				return false;
			}

			if( !File.Exists( src_file ) ) {
				Utility.DebugPopup( string.Format( "The file \"{0}\" does not exist", src_file ) );
				return false;
			}

			try {
				if( File.Exists( dest_file ) ) {
					File.Delete( dest_file );
				}
				File.Copy( src_file, dest_file );
				return true;
			} catch {
				Utility.DebugLog( "Tried to import to a texture that is currently in use" );
				return false;
			}
		}

		public void BuildDecalSelected(DMesh dm, int idx)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_SELECTED, 1);
			GL.NewList(GL_SELECTED, ListMode.Compile);
			
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Color3(Color.Yellow);

				for (int i = 0; i < dm.triangle.Count; i++) {
					if (m_selected_face == i) {
						CreateLinesFromTriangle(dm.triangle[i], dm);
					}
				}

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDecalVertNormals(DMesh dm)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_VERT_NORMALS, 1);
			GL.NewList(GL_VERT_NORMALS, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Lines);
				GL.Color3(Color.Aqua);

				for (int i = 0; i < dm.triangle.Count; i++) {
					CreateLinesFromVertNormals(dm.triangle[i], dm);
				}

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		// Warning: You must have updated decal tex_gl_id before calling this
		public void BuildDecalMesh(DMesh dm)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_DECAL, 1);
			GL.NewList(GL_DECAL, ListMode.Compile);
			int tex_id = -1;
			
			{
				GL.Begin(PrimitiveType.Triangles);
				GL.Color3(Color.White);

				for (int i = 0; i < dm.triangle.Count; i++) {
					// Don't draw no_render triangles
					if ((dm.triangle[i].flags & (int)FaceFlags.NO_RENDER) == (int)FaceFlags.NO_RENDER) {
						continue;
					}

					if (tex_id != dm.m_tex_gl_id[dm.triangle[i].tex_index]) {
						GL.End();
						tex_id = dm.m_tex_gl_id[dm.triangle[i].tex_index];
						GL.BindTexture(TextureTarget.Texture2D, tex_id);
						GL.Begin(PrimitiveType.Triangles);
					}

					CreateTriangle(dm.triangle[i], dm);
				}

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void CreateTriangle(DTriangle tri, DMesh mesh)
		{
			for (int i = 0; i < 3; i++) {
				GL.TexCoord2(tri.tex_uv[i]);
				GL.Normal3(tri.normal[i]);
				GL.Vertex3(mesh.vertex[tri.vert[i]]);
			}
		}

		public void CreateLinesFromTriangle(DTriangle tri, DMesh mesh)
		{
			GL.Vertex3(mesh.vertex[tri.vert[0]]);
			GL.Vertex3(mesh.vertex[tri.vert[1]]);
			GL.Vertex3(mesh.vertex[tri.vert[1]]);
			GL.Vertex3(mesh.vertex[tri.vert[2]]);
			GL.Vertex3(mesh.vertex[tri.vert[2]]);
			GL.Vertex3(mesh.vertex[tri.vert[0]]);
		}

		public void CreateLinesFromVertNormals(DTriangle tri, DMesh mesh)
		{
			GL.Vertex3(mesh.vertex[tri.vert[0]]);
			GL.Vertex3(mesh.vertex[tri.vert[0]] + tri.normal[0] * 0.1f);
			GL.Vertex3(mesh.vertex[tri.vert[1]]);
			GL.Vertex3(mesh.vertex[tri.vert[1]] + tri.normal[1] * 0.1f);
			GL.Vertex3(mesh.vertex[tri.vert[2]]);
			GL.Vertex3(mesh.vertex[tri.vert[2]] + tri.normal[2] * 0.1f);
		}

		public const float LT_SZ = 0.1f;
		public const float LT_MD = 0.025f;
		public const float LT_AR = 0.5f;
		
		// Color for this is determined while drawing
		public void BuildLight()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT, 1);
			GL.NewList(GL_LIGHT, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(LT_SZ, 0f, 0f);
				GL.Vertex3(-LT_SZ, 0f, 0f);
				
				GL.Vertex3(0f, LT_SZ, 0f);
				GL.Vertex3(0f, -LT_SZ, 0f);
				
				GL.Vertex3(0f, 0f, LT_SZ);
				GL.Vertex3(0f, 0f, -LT_SZ);

				GL.Vertex3(LT_MD, LT_MD, LT_MD);
				GL.Vertex3(-LT_MD, -LT_MD, -LT_MD);

				GL.Vertex3(LT_MD, -LT_MD, LT_MD);
				GL.Vertex3(-LT_MD, LT_MD, -LT_MD);

				GL.Vertex3(-LT_MD, LT_MD, LT_MD);
				GL.Vertex3(LT_MD, -LT_MD, -LT_MD);

				GL.Vertex3(LT_MD, LT_MD, -LT_MD);
				GL.Vertex3(-LT_MD, -LT_MD, LT_MD);
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildLightCone()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT_CONE, 1);
			GL.NewList(GL_LIGHT_CONE, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(0f, 0f, 0f);
				GL.Vertex3(0f, 0f, LT_AR);

				GL.Vertex3(0f, 0f, LT_AR);
				GL.Vertex3(LT_SZ, 0f, LT_AR - LT_SZ);

				GL.Vertex3(0f, 0f, LT_AR);
				GL.Vertex3(-LT_SZ, 0f, LT_AR - LT_SZ);
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		public void RefreshGrid()
		{
			BuildGridGeometry(2, 1);
		}

		public void BuildGridGeometry(int lines, int spacing)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_GRID, 1);
			GL.NewList(GL_GRID, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				for (int i = -lines; i <= lines; i++) {
					if (i == 0) {
						GL.Color3(GLView.C_grid3);
					} else if (i % 4 == 0) {
						GL.Color3(GLView.C_grid2);
					} else {
						GL.Color3(GLView.C_grid1);
					}
					GL.Vertex2(i * spacing, -lines * spacing);
					GL.Vertex2(i * spacing, lines * spacing);
				}
				for (int i = -lines; i <= lines; i++) {
					if (i == 0) {
						GL.Color3(GLView.C_grid3);
					} else if (i % 4 == 0) {
						GL.Color3(GLView.C_grid2);
					} else {
						GL.Color3(GLView.C_grid1);
					}
					GL.Vertex2(-lines * spacing, i * spacing);
					GL.Vertex2(lines * spacing, i * spacing);
				}
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		// Load the passed in filename to a texture (working directory should be set)
		public int LoadTexture(string filename)
		{
			if (String.IsNullOrEmpty(filename)) {
				Utility.DebugLog("Could not find texture: " + filename);
				throw new ArgumentException(filename);
			}

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap(filename);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
				 OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
			// GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			bmp.Dispose();

			return id;
		}

		public bool LoadDecalMesh(DMesh dm, string path_to_file)
		{
			try {
				string file_data = System.IO.File.ReadAllText(path_to_file);
				JObject root = JObject.Parse(file_data);

				dm.Deserialize(root);

				return true;
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to load decal mesh: " + ex.Message);
				return false;
			}
		}

		public bool SaveDecalMesh(DMesh dm)
		{
			string path_to_file = Path.ChangeExtension(Path.Combine(editor.m_filepath_decals, dm.name), ".dmesh");

			try {
				JObject root = new JObject();
				dm.Serialize(root);

				string new_file_data = root.ToString(Formatting.Indented);

				if (File.Exists(path_to_file)) {
					// if the file already exists, and it is the same, do not write it
					// out - preserving file timestamps
					try {
						string old_file_data = File.ReadAllText(path_to_file);
						if (old_file_data == new_file_data) {
							// ignore - nothing changed
							return true;
						}
					}
					catch {
					}
				}

				File.WriteAllText(path_to_file, new_file_data);

				return true;
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to save decal mesh: " + ex.Message);
				return false;
			}
		}

		//Save the selected mesh, in case it's been changed
		public void SaveSelectedDecalMesh()
		{
			if (m_active_dmesh != null) {
				Utility.DebugLog("Saving decal meshes " + m_active_dmesh.name);
				SaveDecalMesh(m_active_dmesh);
			}
		}

		public void CycleViewMode()
		{
			m_view_mode = (DViewMode)(((int)m_view_mode + 1) % (int)DViewMode.NUM);
			label_view_mode.Text = "View: " + m_view_mode.ToString();
			this.Refresh();
		}

		public Color[] m_editor_colors = new Color[DMesh.NUM_COLORS];
		public HSBColor m_hsb_selected;
		public Button m_button_selected = null;

		void NotifyEditorColorUpdated(int index = -1)
		{
			if (m_active_dmesh != null) {
				if (index == -1) {
					m_active_dmesh.color.Clear();
					m_active_dmesh.color = new List<Color>(this.m_editor_colors);
				}
				else if( m_active_dmesh.color.Count > index ) {
					m_active_dmesh.color[index] = this.m_editor_colors[index];
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

		public void SelectAdjacentFace()
		{
			if (m_selected_face > -1) {
				m_selected_face = m_active_dmesh.GetFirstAdjacentFace(m_selected_face);
				BuildDecalSelected(m_active_dmesh, m_selected_face);
				this.Refresh();
				UpdateFaceCheckListFromSelected();
			}
		}

		public void UpdateEditorColorsFromSelected()
		{
			for (int i = 0; i < m_active_dmesh.color.Count && i < DMesh.NUM_COLORS; ++i) {
				m_editor_colors[i] = m_active_dmesh.color[i];
			}

			for (int i = m_active_dmesh.color.Count; i < DMesh.NUM_COLORS; ++i) {
				m_editor_colors[i] = Color.Black;
			}

			UpdateHSBFromColor();
			UpdateSlidersFromSelectedColor();
			UpdateColorButtons();
		}

		public void UpdateFaceCheckListFromSelected()
		{
			if (m_selected_face > -1) {
				for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
					bool flag_set = m_active_dmesh.triangle[m_selected_face].IsFlagSet((FaceFlags)Utility.Pow2(i));
					checklist_face.SetItemChecked(i, flag_set);
				}
			}
		}

		public void UpdateFaceFlagsFromCheckList()
		{
			if (m_selected_face > -1) {
				// Clear the flags, then add them
				m_active_dmesh.triangle[m_selected_face].flags = 0;
				for (int i = 0; i < Enum.GetNames(typeof(FaceFlags)).Length; i++) {
					if (checklist_face.GetItemChecked(i)) {
						m_active_dmesh.triangle[m_selected_face].flags += Utility.Pow2(i);
					}
				}
			}
		}

		public void UpdateLightCheckList()
		{
			if (checklist_lights.Items.Count > 0) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					checklist_lights.SetItemChecked(i, m_active_dmesh.light[i].enabled);
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
				copy_lights[i] = new DLight(m_active_dmesh.light[i]);
			}
			copied_lights = true;
		}

		public void PasteLights()
		{
			if (copied_lights) {
				for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
					m_active_dmesh.light[i].Copy(copy_lights[i]);
				}
			}
		}

		public void ProcessKeyboard(KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.NumPad2:
					MoveSelectedLight(-Vector3.UnitZ, e.Shift);
					break;
				case Keys.NumPad8:
					MoveSelectedLight(Vector3.UnitZ, e.Shift);
					break;
				case Keys.NumPad4:
					MoveSelectedLight(-Vector3.UnitX, e.Shift);
					break;
				case Keys.NumPad6:
					MoveSelectedLight(Vector3.UnitX, e.Shift);
					break;
				case Keys.Add:
					MoveSelectedLight(-Vector3.UnitY, e.Shift);
					break;
				case Keys.Subtract:
					MoveSelectedLight(Vector3.UnitY, e.Shift);
					break;
			}
			Utility.DebugLog("Processing keyboard for decal list: " + e.KeyCode.ToString());
		}

		public void MoveSelectedLight(Vector3 dir, bool fast)
		{
			if (m_selected_light >= 0 && m_active_dmesh != null) {
				DLight dl = m_active_dmesh.light[m_selected_light];
				dl.position += dir * (fast ? 0.25f : 0.03125f);
				UpdateLightProperties(dl);
				gl_custom.Invalidate();
			}
		}

		//The built-in (and thus read-only) decals
		private string[] m_builtin_decal_list = new string[] {
			"alien_bevel_01a",
			"alien_bevel_01a_cap1",
			"alien_bevel_01a_cap2",
			"alien_bevel_01b",
			"alien_bevel_01b_cap1",
			"alien_bevel_01b_cap2",
			"alien_bevel_01c",
			"alien_bevel_01c_cap1",
			"alien_bevel_01c_cap2",
			"alien_bevel_01d",
			"alien_bevel_01d_cap1",
			"alien_bevel_01d_cap2",
			"alien_bevel_01e",
			"alien_bevel_01e_cap1",
			"alien_bevel_01e_cap2",
			"alien_bigLight_01",
			"alien_bigLight_01off",
			"alien_brace_01",
			"alien_brace_01off",
			"alien_center_01a",
			"alien_center_01b",
			"alien_center_01c",
			"alien_cover_01a",
			"alien_cover_01b",
			"alien_cover_02a",
			"alien_cover_02b",
			"alien_cover_02c",
			"alien_crystal_02a",
			"alien_crystal_02a_b",
			"alien_crystal_02a_c",
			"alien_crystal_02b",
			"alien_crystal_02b_b",
			"alien_crystal_02b_c",
			"alien_crystal_02c",
			"alien_crystal_02c_b",
			"alien_crystal_02c_c",
			"alien_decal_rib_fat_01a",
			"alien_decal_rib_fat_01off",
			"alien_decal_rib_thin_01a",
			"alien_edge_01a",
			"alien_edge_01b",
			"alien_edge_01c",
			"alien_edge_01d",
			"alien_edge_01e",
			"alien_edge_02a",
			"alien_edge_02b",
			"alien_edge_02c",
			"alien_edge_02d",
			"alien_edge_03a",
			"alien_edge_03b",
			"alien_full_01a",
			"alien_full_01b",
			"alien_full_01c",
			"alien_full_01d",
			"alien_hbar_01a",
			"alien_hbar_01b",
			"alien_hbar_01c",
			"alien_hbar_01d",
			"alien_hbar_01e",
			"alien_hbar_01f",
			"alien_hbar_02a",
			"alien_hbar_02b",
			"alien_hbar_02c",
			"alien_hbar_02d",
			"alien_litebar_01a",
			"alien_litebar_01b",
			"alien_litebar_01off",
			"alien_litebar_02a",
			"alien_litebar_02b",
			"alien_litebar_02off",
			"alien_pipes_01a",
			"alien_pipes_01b",
			"alien_pipes_01c",
			"alien_pipes_01d",
			"alien_pipes_01e",
			"alien_pipes_01f",
			"alien_pipes_01g",
			"alien_sconce_01",
			"alien_sconce_01off",
			"alien_smallLight_01",
			"alien_smallLight_01off",
			"alien_stopper_01a",
			"alien_stopper_01b",
			"alien_stopper_01c",
			"alien_stopper_01d",
			"alien_stopper_01doff",
			"alien_stopper_01e",
			"alien_support_01a",
			"alien_support_01b",
			"alien_support_01c",
			"alien_support_01d",
			"alien_support_02a",
			"alien_support_02b",
			"alien_trim_01",
			"alien_vbar_01a",
			"alien_vbar_01b",
			"alien_vbar_01c",
			"alien_vbar_01d",
			"alien_vbar_01e",
			"alien_vbar_01f",
			"alien_vwide_01a",
			"alien_vwide_01b",
			"alien_widelight_01a",
			"alien_widelight_01off",
			"cc_bevel_01a",
			"cc_bevel_01b",
			"cc_bevel_01c",
			"cc_bevel_02a",
			"cc_edge_01",
			"cc_edge_02",
			"cc_edge_03",
			"cc_edge_04",
			"cc_edge_05",
			"cc_edge_06",
			"cc_edge_06a",
			"cc_fridge_01a",
			"cc_light_01",
			"cc_pod_skirt_01",
			"cc_roof_vent_01a",
			"cc_vbar_01",
			"cc_vbar_01a",
			"cc_vbar_02",
			"decal_orientation_test_01a",
			"decal_orientation_test_01a1",
			"decal_orientation_test_01b",
			"decal_orientation_test_01c",
			"decal_orientation_test_01d",
			"decal_orientation_test_01e",
			"ec_edge_01a",
			"ec_full_01a",
			"ec_hbar_01a",
			"ec_light_decal_01a",
			"ec_light_decal_01b",
			"ec_pod_skirt_01",
			"ec_sign1",
			"foundry_bar01a",
			"foundry_bar01b",
			"foundry_bar02a",
			"foundry_beam02",
			"foundry_beam03",
			"foundry_bevel01",
			"foundry_bevel01a",
			"foundry_bevel02",
			"foundry_bevel02a",
			"foundry_bevel03",
			"foundry_duct01",
			"foundry_edge01",
			"foundry_edge01a",
			"foundry_edge02a",
			"foundry_edge03a",
			"foundry_edge04",
			"foundry_fan01",
			"foundry_light01a",
			"foundry_light02a",
			"foundry_light03a",
			"foundry_light04a",
			"foundry_light04b",
			"foundry_light04c",
			"foundry_light04_edge",
			"foundry_overlay01a",
			"foundry_overlay02a",
			"foundry_pipes01a",
			"matcen_bevel_01",
			"matcen_bevel_02",
			"matcen_bevel_03",
			"matcen_bevel_04",
			"matcen_edge_01",
			"matcen_edge_02",
			"matcen_edge_02b",
			"matcen_edge_03",
			"matcen_edge_03b",
			"matcen_hbar_01",
			"matcen_hbar_01b",
			"om_bevel_01",
			"om_bevel_01b",
			"om_bevel_01c",
			"om_bevel_02",
			"om_bevel_02b",
			"om_bevel_02c",
			"om_bevel_03",
			"om_bevel_03b",
			"om_bevel_03c",
			"om_bevel_04-cyan",
			"om_bevel_04-white",
			"om_bevel_04-yellow",
			"om_bevel_04",
			"om_bevel_05",
			"om_bevel_06",
			"om_bevel_07",
			"om_bevel_08",
			"om_bevel_08a",
			"om_bevel_08b",
			"om_cavelightA_1",
			"om_cavelight_01",
			"om_cavelight_03",
			"om_corner_01",
			"om_corner_02",
			"om_corner_02a",
			"om_corner_03",
			"om_edge_01",
			"om_edge_02",
			"om_edge_03",
			"om_edge_04",
			"om_edge_05",
			"om_edge_06",
			"om_edge_07",
			"om_edge_08-white",
			"om_edge_08",
			"om_edge_09",
			"om_edge_10-cyan",
			"om_edge_10-white",
			"om_edge_10-yellow",
			"om_edge_10",
			"om_edge_11",
			"om_edge_12",
			"om_edge_12b",
			"om_edge_12c",
			"om_edge_13",
			"om_edge_13a",
			"om_edge_13b",
			"om_edge_14",
			"om_full_01a",
			"om_full_02a",
			"om_full_02b",
			"om_full_03a",
			"om_full_04a",
			"om_full_04b",
			"om_full_05a",
			"om_full_06",
			"om_full_06b",
			"om_full_06c",
			"om_full_06d",
			"om_full_06e",
			"om_full_07",
			"om_full_07a",
			"om_full_08",
			"om_full_08b",
			"om_full_08c",
			"om_full_09",
			"om_full_09b",
			"om_full_frame_01",
			"om_full_frame_02",
			"om_full_frame_02a",
			"om_full_frame_03",
			"om_full_frame_lg_01",
			"om_gas_storage_tank_01a",
			"om_gas_storage_tank_01b",
			"om_hbar_01",
			"om_hbar_02",
			"om_hbar_03",
			"om_hbar_04",
			"om_hbar_05",
			"om_hbar_06",
			"om_hbar_07",
			"om_hbar_08",
			"om_hbar_08a",
			"om_hbar_08b",
			"om_hbar_09",
			"om_hbar_10-cyan",
			"om_hbar_10-white",
			"om_hbar_10-yellow",
			"om_hbar_10",
			"om_hbar_10a",
			"om_hbar_11",
			"om_hbar_11a",
			"om_lightbar01",
			"om_lightbar01a",
			"om_lightbar01b",
			"om_lightbar01c",
			"om_lightbar01d",
			"om_lightbar01e",
			"om_lightbar01f",
			"om_lightbar01g",
			"om_lightbar02",
			"om_lightbar02a",
			"om_lightbar02b",
			"om_lightbar02c",
			"om_lightbar02d",
			"om_lightbar02e",
			"om_lightbar02f",
			"om_lightbar02g",
			"om_lightbar03",
			"om_lightbar03a",
			"om_lightbar03b",
			"om_lightbar03c",
			"om_lightbar03d",
			"om_lightbar03e",
			"om_lightbar03f",
			"om_lightbar03g",
			"om_lightbar04",
			"om_lightbar04b",
			"om_lightbar04c",
			"om_lightbar04g",
			"om_lightbevel01",
			"om_light_01a",
			"om_light_01b",
			"om_light_01c",
			"om_light_01d",
			"om_light_01e",
			"om_light_01f",
			"om_light_02a",
			"om_light_02b",
			"om_light_02c",
			"om_light_02d",
			"om_light_02e",
			"om_light_02f",
			"om_light_03a",
			"om_light_03b",
			"om_light_03c",
			"om_light_03d",
			"om_light_03e",
			"om_light_03f",
			"om_light_04a",
			"om_light_04b",
			"om_light_04c",
			"om_light_04d",
			"om_light_04e",
			"om_light_04f",
			"om_light_05a",
			"om_light_06a",
			"om_light_07a",
			"om_light_edge_01",
			"om_light_edge_01b",
			"om_light_edge_01c",
			"om_light_edge_01d",
			"om_light_edge_01e",
			"om_light_edge_01f",
			"om_light_edge_02",
			"om_light_edge_02b",
			"om_light_edge_02c",
			"om_light_edge_02d",
			"om_light_edge_02e",
			"om_light_edge_02f",
			"om_light_edge_03",
			"om_light_edge_03b",
			"om_light_edge_03c",
			"om_light_edge_03d",
			"om_light_edge_03e",
			"om_light_edge_03f",
			"om_light_edge_04",
			"om_light_edge_04b",
			"om_pillar_01a",
			"om_pipes01a",
			"om_pipes01b",
			"om_pipes01c",
			"om_pipes01d",
			"om_pipes02a",
			"om_pipes02b",
			"om_pipes02c",
			"om_pipes02d",
			"om_pipes03a",
			"om_pipes03b",
			"om_pipes04a",
			"om_pipes04b",
			"om_pipes05a",
			"om_pipes05b",
			"om_pipes05c",
			"om_pipes06a",
			"om_pipes06b",
			"om_pipes06c",
			"om_pipe_large_01a",
			"om_pipe_large_cap_01a",
			"om_pipe_large_corner_01a",
			"om_sec1_bar1",
			"om_sec2_bar1",
			"om_sec3_bar1",
			"om_sec4_bar1",
			"om_sec5_bar1",
			"om_security_bar01",
			"om_security_bar02",
			"om_sign_ambient_01a",
			"om_sign_ambient_01b",
			"om_sign_ambient_02a",
			"om_sign_ambient_02b",
			"om_sign_ambient_02c",
			"om_sign_ambient_02d",
			"om_sign_ambient_02e",
			"om_sign_ambient_02f",
			"om_sign_ambient_02g",
			"om_sign_ambient_02h",
			"om_sign_ambient_03a",
			"om_sign_ambient_04a",
			"om_sign_ambient_04b",
			"om_sign_ambient_04c",
			"om_sign_ambient_04d",
			"om_sign_ambient_04e",
			"om_sign_ambient_04f",
			"om_sign_ambient_04g",
			"om_sign_arrow_01a",
			"om_sign_danger_01a",
			"om_sign_ec_01a",
			"om_sign_exit_01a",
			"om_sign_nuclear_01a",
			"om_sign_reactor_01a",
			"om_sign_restricted_area_01a",
			"om_sign_restricted_area_02a",
			"om_sign_restricted_area_03a",
			"om_sign_restricted_area_level_01a",
			"om_sign_restricted_area_level_02a",
			"om_sign_restricted_area_level_03a",
			"om_sign_security_01a",
			"om_sign_security_02a",
			"om_sign_two_01a",
			"om_support_01",
			"om_support_01a",
			"om_support_02",
			"om_support_03",
			"om_support_04",
			"om_support_04b",
			"om_support_05",
			"om_test01",
			"om_test02",
			"om_vbar_01",
			"om_vbar_02",
			"om_vbar_02a",
			"om_vbar_03",
			"om_vwide01",
			"om_vwide02",
			"om_vwide03",
			"om_vwide03b",
			"om_vwide04",
			"om_vwide04b",
			"om_vwide05",
			"om_vwide06",
			"om_vwide07",
			"om_vwide07a",
			"om_vwide07b",
			"om_warning_stripe_01a",
			"powerCoreA_01a",
			"powerCoreA_01b",
			"powerCoreA_01c",
			"powerCoreA_01d",
			"powerCoreA_01e",
			"powerCoreA_Switch_01a",
			"power_extra01",
			"power_extra01a",
			"power_extra02",
			"power_extra02a",
			"power_extra02b",
			"rust_bevel01",
			"rust_bevel01a",
			"rust_bevel01b",
			"rust_bevel01c",
			"rust_bevel02",
			"rust_bevel02a",
			"rust_bevel02b",
			"rust_bevel03",
			"rust_bevel03b",
			"rust_bevel04",
			"rust_bevel05",
			"rust_bevel06",
			"rust_bevel07",
			"rust_bevel07a",
			"rust_bevel07b",
			"rust_bevel08a",
			"rust_bevel08b",
			"rust_bevel08c",
			"rust_bevel08d",
			"rust_cavelight01",
			"rust_cavelight02",
			"rust_cavelight03",
			"rust_cavelight04",
			"rust_corner01",
			"rust_corner02",
			"rust_corner02b",
			"rust_corner03",
			"rust_corner04",
			"rust_corner05",
			"rust_corner05a",
			"rust_corner06",
			"rust_corner06a",
			"rust_cover01",
			"rust_cover02",
			"rust_cover03",
			"rust_cover03a",
			"rust_door1_frame1",
			"rust_door1_sliding1",
			"rust_door2_frame1",
			"rust_door2_sliding1",
			"rust_edge01",
			"rust_edge02",
			"rust_edge02a",
			"rust_edge02b",
			"rust_edge02c",
			"rust_edge03",
			"rust_edge03a",
			"rust_edge04",
			"rust_edge04a",
			"rust_edge04b",
			"rust_edge04c",
			"rust_edge04d",
			"rust_edge05",
			"rust_edge05a",
			"rust_edge06",
			"rust_edge07",
			"rust_edge07a",
			"rust_edge08",
			"rust_edge08a",
			"rust_edge09",
			"rust_edge09a",
			"rust_edge10",
			"rust_edge10a",
			"rust_edge11",
			"rust_edge11a",
			"rust_edge12",
			"rust_edge12a",
			"rust_edge13",
			"rust_edge13a",
			"rust_edge13b",
			"rust_edge13d",
			"rust_edge14",
			"rust_edge14a",
			"rust_edge14b",
			"rust_edge14c",
			"rust_edge15",
			"rust_edge15a",
			"rust_edge16a",
			"rust_edge16b",
			"rust_edge16c",
			"rust_frame01",
			"rust_frame02",
			"rust_frame03",
			"rust_frame03a",
			"rust_frame_lg01",
			"rust_full01",
			"rust_full01a",
			"rust_full01b",
			"rust_full02",
			"rust_full02a",
			"rust_full03",
			"rust_full03a",
			"rust_full03b",
			"rust_full03c",
			"rust_full04",
			"rust_full04a",
			"rust_full04b",
			"rust_full05",
			"rust_full06",
			"rust_full06a",
			"rust_full06b",
			"rust_full06c",
			"rust_full07",
			"rust_full07a",
			"rust_full08",
			"rust_full09",
			"rust_full09a",
			"rust_hbar01",
			"rust_hbar01a",
			"rust_hbar01b",
			"rust_hbar02",
			"rust_hbar02a",
			"rust_hbar03",
			"rust_hbar03a",
			"rust_hbar03b",
			"rust_hbar05",
			"rust_hbar05a",
			"rust_hbar06",
			"rust_hbar06a",
			"rust_hbar06b",
			"rust_hbar07",
			"rust_hbar07a",
			"rust_hbar08",
			"rust_hbar08a",
			"rust_hbar08b",
			"rust_hbar08c",
			"rust_hbar08d",
			"rust_hbar09",
			"rust_hbar09a",
			"rust_hbar10",
			"rust_hbar10a",
			"rust_hbar11a",
			"rust_hbar11b",
			"rust_hbar12",
			"rust_hbar4",
			"rust_light01",
			"rust_light01a",
			"rust_light02",
			"rust_light02a",
			"rust_light03",
			"rust_light04",
			"rust_light04a",
			"rust_light05",
			"rust_light05a",
			"rust_litebar01a",
			"rust_litebar01b",
			"rust_litebar02a",
			"rust_litebar02b",
			"rust_litebar03a",
			"rust_litebar03b",
			"rust_litewide01a",
			"rust_litewide01b",
			"rust_litewide02a",
			"rust_litewide02b",
			"rust_notch01",
			"rust_notch02",
			"rust_sign01",
			"rust_vbar01",
			"rust_vbar02",
			"rust_vbar03",
			"rust_vbar03a",
			"rust_vbar04",
			"rust_vbar05",
			"rust_vbar05a",
			"rust_vbar06",
			"rust_vbar06a",
			"rust_vbar06b",
			"rust_vbar06c",
			"rust_vbar06e",
			"rust_vbar07",
			"rust_vbar08",
			"rust_vbar08a",
			"rust_vbar09",
			"rust_vbar09a",
			"rust_vwide01",
			"rust_vwide02",
			"rust_vwide02a",
			"rust_vwide03",
			"rust_vwide03a",
			"rust_vwide04",
			"rust_vwide05",
			"rust_vwide06",
			"rust_vwide06a",
			"rust_vwide07",
			"rust_vwide08",
			"rust_vwide08a",
			"rust_vwide08b",
			"rust_vwide09",
			"rust_vwide10",
			"rust_vwide10a",
			"test_titan01",
			"titan_bevel_01a",
			"titan_bevel_01b",
			"titan_bevel_01c",
			"titan_temple_cap01a",
			"titan_temple_cap01b",
			"titan_temple_cap02a",
			"titan_temple_panel01a",
			"titan_temple_panel01b",
			"titan_temple_panel01c",
			"titan_temple_panel01e",
			"tn_bevel_01a",
			"tn_bevel_01b",
			"tn_bevel_01c",
			"tn_bevel_02a",
			"tn_bevel_02b",
			"tn_bevel_02c",
			"tn_bevel_03a",
			"tn_bevel_03b",
			"tn_bevel_03c",
			"tn_cabling_01a",
			"tn_cabling_01b",
			"tn_cabling_01c",
			"tn_cabling_01d",
			"tn_cabling_01e",
			"tn_cabling_01f",
			"tn_cabling_01g",
			"tn_cabling_01h",
			"tn_cavelightA_1",
			"tn_cavelightA_1_f",
			"tn_edge_01a",
			"tn_edge_01b",
			"tn_edge_01c",
			"tn_edge_01d",
			"tn_edge_02a",
			"tn_edge_02b",
			"tn_edge_02c",
			"tn_edge_03a",
			"tn_edge_04a",
			"tn_edge_04b",
			"tn_edge_04c",
			"tn_edge_04d",
			"tn_edge_05a",
			"tn_edge_05b",
			"tn_edge_05c",
			"tn_edge_07a",
			"tn_edge_07b",
			"tn_edge_07c",
			"tn_edge_07d",
			"tn_edge_07e",
			"tn_edge_07f",
			"tn_gas_storage_tank_01a",
			"tn_gas_storage_tank_01b",
			"tn_gas_storage_tank_02a",
			"tn_gas_storage_tank_02b",
			"tn_light_01a",
			"tn_light_01b",
			"tn_light_01c",
			"tn_light_01d",
			"tn_light_01e",
			"tn_light_01f",
			"tn_light_02a",
			"tn_light_02b",
			"tn_light_02c",
			"tn_light_02d",
			"tn_light_02e",
			"tn_light_02f",
			"tn_light_03a",
			"tn_light_03b",
			"tn_light_03c",
			"tn_light_03d",
			"tn_light_03e",
			"tn_light_03f",
			"tn_light_04a",
			"tn_light_04b",
			"tn_light_04c",
			"tn_light_04d",
			"tn_light_04e",
			"tn_light_04f",
			"tn_light_05a",
			"tn_light_05b",
			"tn_light_05c",
			"tn_light_05d",
			"tn_light_05e",
			"tn_light_05f",
			"tn_light_06a",
			"tn_light_06b",
			"tn_light_06c",
			"tn_light_06d",
			"tn_light_06e",
			"tn_light_06f",
			"tn_light_07a",
			"tn_light_07b",
			"tn_light_07c",
			"tn_light_07d",
			"tn_light_07e",
			"tn_light_07f",
			"tn_light_08a",
			"tn_light_08b",
			"tn_light_08c",
			"tn_light_08d",
			"tn_light_08e",
			"tn_light_08f",
			"tn_light_edge01a",
			"tn_light_edge01b",
			"tn_light_edge01c",
			"tn_light_edge01d",
			"tn_light_edge01e",
			"tn_light_edge01f",
			"tn_light_edge02a",
			"tn_light_edge02b",
			"tn_light_edge02c",
			"tn_light_edge02d",
			"tn_light_edge02e",
			"tn_light_edge02f",
			"tn_light_edge02g",
			"tn_pipes01a",
			"tn_pipes01b",
			"tn_pipes01c",
			"tn_pipes01d",
			"tn_pipes02a",
			"tn_pipes02b",
			"tn_pipes02c",
			"tn_pipes02d",
			"tn_pipes03a",
			"tn_pipes03b",
			"tn_pipes03c",
			"tn_pipes03d",
			"tn_pipes04a",
			"tn_pipes04b",
			"tn_pipes05a",
			"tn_pipes05b",
			"tn_pipes05c",
			"tn_pipes06a",
			"tn_pipes06b",
			"tn_pipes06c",
			"tn_pipes06d",
			"tn_pipes07a",
			"tn_pipes07b",
			"tn_pipes07c",
			"tn_pipes07d",
			"tn_pipes07e",
			"tn_pipes07f",
			"tn_pipes07g",
			"tn_pipes07h",
			"tn_pipes08a",
			"tn_pipes08b",
			"tn_pipes08c",
			"tn_pipes08d",
			"tn_pipe_large_01a",
			"tn_pipe_large_02a",
			"tn_pipe_large_cap_01a",
			"tn_pipe_large_cap_02a",
			"tn_pipe_large_corner_01a",
			"tn_pipe_large_corner_02a",
			"tn_sign_decals_01a",
			"tn_sign_decals_02a",
			"tn_sign_decals_02b",
			"tn_sign_decals_03a",
			"tn_sign_decals_04a",
			"tn_sign_decals_04b",
			"tn_sign_decals_04c",
			"tn_sign_decals_04d",
			"tn_sign_decals_04e",
			"tn_sign_decals_05a",
			"tn_sign_decals_06a",
			"tn_sign_decals_06b",
			"tn_sign_decals_06b2",
			"tn_sign_decals_06b3",
			"tn_sign_decals_06b4",
			"tn_sign_decals_06b5",
			"tn_sign_decals_06c",
			"utility_box_01a",
			"utility_box_01b",
			"utility_box_01c",
			"utility_box_02a",
			"utility_box_02b",
			"utility_box_02c",
			"utility_box_03a",
			"utility_box_03b",
			"utility_box_03c",
			"wind_arrow01",
			"wind_edge01",
			"wind_hbar01",
			"wind_hbar01a",
			"wind_hbar01b",
			"wind_vwide_01",
		};
	}
}
