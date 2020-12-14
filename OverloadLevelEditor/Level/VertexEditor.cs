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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Newtonsoft.Json.Linq;

// VERTEX - Editor
// Editor-specific functions for modifying/etc a segment

namespace OverloadLevelEditor
{
	public partial class Vertex
	{
		public bool m_tag; // For side/segment functions
		
		public void Delete()
		{
			alive = false;
			marked = false;
			m_tag = false;
		}

		public void Create(Vector3 pos, bool mrk = false)
		{
			alive = true;
			marked = mrk;
			m_tag = true;
			position = pos;
		}

		public void SnapToGrid(float grid_snap)
		{
			position = Utility.SnapValue(position, grid_snap);
		}

		public void Copy(Vertex src, bool full)
		{
			position = src.position;
			alive = src.alive;
			marked = (full ? src.marked : false);
		}
	}
}