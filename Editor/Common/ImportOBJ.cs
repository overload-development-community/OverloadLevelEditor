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
using OpenTK;
using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using ObjFileLib;
using System.Collections.Generic;

namespace OverloadLevelEditor
{
	public static class ImportOBJ {
		public static bool CopyImportTextureToDecalFolder(string obj_file_name, string name, string filepath_decal_textures)
		{
			string dest_file = Path.Combine( filepath_decal_textures, Path.GetFileNameWithoutExtension(name) + ".png" );
			string obj_dir = Path.GetDirectoryName(obj_file_name);
			string src_file = Path.Combine( obj_dir, name );

			if ( !File.Exists( src_file ) ) {
				string[] parts = name.Split( new [] { '/', '\\' } );
				for ( int i = parts.Length - 1; i > 0; i-- ) {
					src_file = Path.Combine( obj_dir, string.Join( new string(Path.DirectorySeparatorChar, 1), parts.Skip(i) ) );
					if ( File.Exists( src_file ) )
						break;
				}
			}

			if (dest_file.Equals(src_file, StringComparison.InvariantCultureIgnoreCase) || File.Exists( dest_file ) ) {
				// No need to do anything
				return false;
			}

			if( !File.Exists( src_file ) ) {
				Utility.DebugPopup( string.Format( "The file \"{0}\" does not exist", name ) );
				return false;
			}

			try {
				string ext = Path.GetExtension( src_file );
				if ( ext.Equals( ".png", StringComparison.InvariantCultureIgnoreCase ) ) {
					File.Copy( src_file, dest_file );
				} else {
					var bmp = ext.Equals( ".tga", StringComparison.InvariantCultureIgnoreCase ) ?
						TGAFile.Read( new BinaryReader( new BufferedStream( File.OpenRead( src_file ) ) ) ) :
						new Bitmap( src_file );
					bmp.Save( dest_file, ImageFormat.Png );
					bmp.Dispose();
				}
				return true;
			} catch (Exception ex) {
				Utility.DebugLog( "Failed to import texture " + src_file + ": " + ex.Message );
				return false;
			}
		}

		public static void ConvertObjFileToDMesh(ObjFile obj_file, DMesh dm, bool units_inches, string obj_file_name, Editor editor, TextureManager tex_manager)
		{
			dm.Init(editor);

			// Convert the verts
			foreach (var v in obj_file.Vertices) {
				// Flip Z for coordinate system translation
				#if DMESH_EDITOR
				dm.AddVertexEditor(new Vector3(v.X, v.Y, -v.Z), false);
				#else
				dm.AddVertex(v.X, v.Y, -v.Z);
				#endif
			}

			if (units_inches) {
				for (int i = 0; i < dm.vertex.Count; i++) {
					dm.vertex[i] *= 0.0254f;
				}
			}

			// Convert the textures
			for (var i = 0; i < obj_file.Materials.Count; i++) {
				var material = obj_file.Materials[i];
				string tex_name = material.Diffuse.Texture;
				string tex_id_name = tex_name != null ? Path.GetFileNameWithoutExtension(tex_name) : material.Name;
				string decal_tex_name = Path.Combine(editor.m_filepath_decal_textures, tex_id_name + ".png");

				if (tex_name != null) {
					if (!CopyImportTextureToDecalFolder(obj_file_name, tex_name, editor.m_filepath_decal_textures)) {
						Utility.DebugLog("No texture imported/updated: " + tex_name);
					} else {
						Utility.DebugLog("Imported/updated a texture: " + tex_name);
					}
				} else if (!File.Exists(decal_tex_name)) {
					using (var bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb)) {
						bmp.SetPixel(0, 0, material.Diffuse.Color);
						bmp.Save(decal_tex_name, ImageFormat.Png);
					}
				}

				if (tex_manager.FindTextureIDByName(tex_id_name) < 0) {
					// Get the texture (if it exists)
					if (File.Exists(decal_tex_name)) {
						tex_manager.AddTexture(decal_tex_name);
					} else {
						// Try to steal it from the level directory instead
						string level_tex_name = Path.Combine(editor.m_filepath_level_textures, tex_id_name + ".png");
						if (File.Exists(level_tex_name)) {
							// Copy it to the decal textures directory, then load it
							if (!File.Exists(decal_tex_name)) {
								File.Copy(level_tex_name, decal_tex_name);

								tex_manager.AddTexture(decal_tex_name);
							}
						} else {
							editor.AddOutputText("IMPORT WARNING: No PNG file could be found matching the name: " + tex_id_name);
						}
					}
				}

				dm.AddTexture(i, tex_id_name);
			}

			// Convert the faces
			var vrt_idx = new int[3];
			var uv = new Vector2[3];
			var nrml = new Vector3[3];
			var tris = new List<ObjFile.FaceVert[]>();
			foreach (var f in obj_file.Faces) {
				// Triangulate face
				tris.Clear();
				var fv = f.FaceVerts;
				if (fv.Length == 3) {
					tris.Add(fv);
				} else if (fv.Length == 4 &&
					Vector3.Subtract(obj_file.Vertices[fv[0].VertIdx], obj_file.Vertices[fv[2].VertIdx]).LengthSquared >
					Vector3.Subtract(obj_file.Vertices[fv[1].VertIdx], obj_file.Vertices[fv[3].VertIdx]).LengthSquared) {
					tris.Add(new [] { fv[0], fv[1], fv[3] });
					tris.Add(new [] { fv[1], fv[2], fv[3] });
				} else { // assume convex...
					for (int i = 1; i < fv.Length - 1; i++)
						tris.Add(new [] { fv[0], fv[i], fv[i + 1] });
				}

				foreach (var tri in tris) {
					for (var i = 0; i < 3; i++) {
						// Flip Z/V and reverse vertex order for coordinate system translation
						int fi = 2 - i;
						vrt_idx[i] = tri[fi].VertIdx;
						nrml[i] = obj_file.Normals[tri[fi].NormIdx];
						nrml[i].Z = -nrml[i].Z;
						uv[i] = tri[fi].UVIdx >= 0 ? obj_file.UVs[tri[fi].UVIdx] : new Vector2();
						uv[i].Y = 1.0f - uv[i].Y;
					}
					dm.AddFace(vrt_idx, nrml, uv, f.MatIdx);
				}
			}
		}

		public static bool ImportOBJToDMesh(DMesh dm, string obj_file_name, bool units_inches, Editor editor, TextureManager tex_manager)
		{
			if( !File.Exists( obj_file_name ) ) {
				return false;
			}

			ObjFile obj_file = new ObjFile();
			obj_file.Load(obj_file_name);

			ConvertObjFileToDMesh(obj_file, dm, units_inches, obj_file_name, editor, tex_manager);

			return true;
		}
	}
}
