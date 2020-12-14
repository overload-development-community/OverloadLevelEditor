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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

// TEXTUREMANAGER
// Manager for textures in the editor
// Uses OpenGL and the .NET Bitmap class

namespace OverloadLevelEditor
{
	public partial class TextureManager
	{
		public Editor editor;

		public int m_total_tex_count;
		public List<int> m_gl_id;
		public List<string> m_name;
		public List<Bitmap> m_bitmap;
		
		public TextureManager(Editor e)
		{
			m_total_tex_count = 0;
			editor = e;
			m_gl_id = new List<int>();
			m_name = new List<string>();
			m_bitmap = new List<Bitmap>();
		}

		public void LoadTexturesInDir(string dir, bool all_dir = false, bool dispose_bmp = false)
		{
			string[] files = Directory.GetFiles(dir, "*.png", (all_dir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

			string png_name;
			Bitmap bmp_large;
			Bitmap bmp_small;

			for (int i = 0; i < files.Length; i++) {
				// Remove all the extra stuff to get the names
				png_name = Utility.GetRelativeExtensionlessFilenameFromDirectory(dir, files[i]);
				if (!m_name.Contains(png_name)) {
					m_name.Add(png_name);

					// Load the bitmaps
					if (editor.m_low_res_textures) {
						bmp_large = new Bitmap(files[i]);
						bmp_small = ResizeBitmap(bmp_large, 128, 128);
						m_bitmap.Add(bmp_small);
						bmp_large.Dispose();

						m_gl_id.Add(LoadTexture(bmp_small, dispose_bmp));
					} else {
						bmp_large = new Bitmap(files[i]);
						bmp_small = ResizeBitmap(bmp_large, 128, 128);
						m_bitmap.Add(bmp_small);

						m_gl_id.Add(LoadTexture(bmp_large, dispose_bmp));
						bmp_large.Dispose();
					}

					m_total_tex_count += 1;
				} else {
					Utility.DebugLog("Found a duplicate copy of " + png_name);
				}
         }
      }

		private Bitmap ResizeBitmap(Bitmap source_bmp, int width, int height)
		{
			Bitmap result = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(result))
				g.DrawImage(source_bmp, 0, 0, width, height);
			return result;
		}

		public int LoadTexture(Bitmap bmp, bool dispose_bmp)
		{
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			//BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
				 OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			if (dispose_bmp) {
				bmp.Dispose();
			}

			return id;
		}

		public int LoadTexture(string bmp_name, bool dispose_bmp)
		{
			Bitmap bmp = new Bitmap(bmp_name);
			return LoadTexture(bmp, dispose_bmp); // NOTE(Jeff): shouldn't this always dispose this bmp?
		}

		public int FindTextureIDByName(string tex_name)
		{
			if (tex_name == "") {
				return -1;
			}

			string tex_name_raw = Utility.GetPathlessFilename(tex_name);

			for (int i = 0; i < m_name.Count; i++) {
				// Ignore filename case differences
				string this_texture_name = m_name[i];
				if( this_texture_name.Equals( tex_name, System.StringComparison.InvariantCultureIgnoreCase ) ||
				    this_texture_name.Equals( tex_name_raw, System.StringComparison.InvariantCultureIgnoreCase ) ) {
					return m_gl_id[i];
				}
			}

			// No texture found
			Utility.DebugLog("Texture NOT found in the texture manager: " + tex_name);
			return -1;
		}
	}
}