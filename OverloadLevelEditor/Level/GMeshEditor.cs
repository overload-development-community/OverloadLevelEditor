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

using System.Collections.Generic;

// GMESH - Editor
// Editor-specific functions for a GMesh

namespace OverloadLevelEditor
{
	// GMESH - Generated (decal) mesh (output form for level)
	// - Takes a DMesh + Decal data to tile and clip to create a mesh for a single side of a segment
	// - Saved in world space
	public partial class GMesh
	{
		public List<int> m_tex_gl_id;
		
		public void UpdateGLTextures(TextureManager tm, TextureManager backup_tm)
		{
			for (int i = 0; i < m_tex_name.Count; i++) {
				m_tex_gl_id[i] = tm.FindTextureIDByName(m_tex_name[i]);
				if (m_tex_gl_id[i] == -1) {
					m_tex_gl_id[i] = backup_tm.FindTextureIDByName(m_tex_name[i]);
				}
			}
		}

		public void AddTexture(int id, string name)
		{
			m_tex_gl_id.Add(id);
			m_tex_name.Add(name);
		}
	}
}