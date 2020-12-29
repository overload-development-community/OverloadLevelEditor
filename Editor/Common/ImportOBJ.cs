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

using System.IO;
using ObjLoader.Loader.Loaders;
using OpenTK;

namespace OverloadLevelEditor
{
	public static class ImportOBJ {
		public static bool CopyImportTextureToDecalFolder(string name, string filepath_decal_textures)
		{
			string dest_file = Path.Combine( filepath_decal_textures, name );
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

		public static void ConvertLoadResultToDMesh(LoadResult load_result, DMesh dm, bool units_inches, Editor editor, TextureManager tex_manager)
		{
			// Convert the verts
			ObjLoader.Loader.Data.VertexData.Vertex v;
			for (int i = 0; i < load_result.Vertices.Count; i++) {
				v = load_result.Vertices[i];
				#if DMESH_EDITOR
				dm.AddVertexEditor(new Vector3(v.X, v.Y, v.Z), false);
				#else
				dm.AddVertex(v.X, v.Y, v.Z);
				#endif
			}

			if (units_inches) {
				for (int i = 0; i < dm.vertex.Count; i++) {
					dm.vertex[i] *= 0.0254f;
				}
			}

			// Rotate all the verts 180
			dm.RotateMesh180();

			// Convert the faces
			int[] vrt_idx = new int[3];
			Vector3[] nrml = new Vector3[3];
			Vector2[] uv = new Vector2[3];
			string tex_name;

			int tex_idx = 0;

			ObjLoader.Loader.Data.VertexData.Normal n;
			ObjLoader.Loader.Data.VertexData.Texture t;
			ObjLoader.Loader.Data.Elements.FaceVertex fv;

			foreach (ObjLoader.Loader.Data.Elements.Group g in load_result.Groups) {
				tex_name = g.Material.DiffuseTextureMap;
				if (!CopyImportTextureToDecalFolder(tex_name, editor.m_filepath_decal_textures)) {
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

				dm.AddTexture(tex_idx, tex_id_name);
				foreach (ObjLoader.Loader.Data.Elements.Face f in g.Faces) {
					// Only works for 3 vert faces (currently)
					for (int i = 0; i < f.Count; i++) {
						if (i < 3) {
							fv = f[i];
							vrt_idx[i] = fv.VertexIndex - 1;
							n = load_result.Normals[fv.NormalIndex - 1];
							t = load_result.Textures[fv.TextureIndex - 1];
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

					dm.AddFace(vrt_idx, nrml, uv, tex_idx);
				}

				tex_idx += 1;
			}
		}

		public static bool ImportOBJToDMesh(DMesh dm, string obj_file_name, bool units_inches, Editor editor, TextureManager tex_manager)
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
				LoadResult load_result;
				ObjLoaderFactory obj_loader_factory = new ObjLoaderFactory();
				IObjLoader obj_loader = obj_loader_factory.Create();

				// Have to set working directory so loading of the material file will work
				Directory.SetCurrentDirectory(obj_working_dir);
				using (FileStream file_stream = new FileStream(obj_file_name, FileMode.Open, FileAccess.Read)) {
					load_result = obj_loader.Load(file_stream);
					file_stream.Close();
				}

				ConvertLoadResultToDMesh(load_result, dm, units_inches, editor, tex_manager);
				//m_active_dmesh.CenterMesh();
				dm.FlipMesh(-1f, 1f);
				dm.FlipVertNormalsZ();
				//active_dmesh.RotateMesh90();
			}
			finally {
				// Restore the working directory
				Directory.SetCurrentDirectory( current_working_directory );
			}

			return true;
		}
	}
}
