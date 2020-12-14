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

// DECAL - Editor
// Editor-specific functions for decals (changing stuff, basically)

namespace OverloadLevelEditor
{
	public partial class Decal
	{
		public void CycleAlign(bool reverse)
		{
			if (reverse) {
				align = (DecalAlign)(((int)align + (int)DecalAlign.NUM - 1) % (int)DecalAlign.NUM);
			} else {
				align = (DecalAlign)(((int)align + 1) % (int)DecalAlign.NUM);
			}
		}

		public void CycleMirror()
		{
			mirror = (DecalMirror)(((int)mirror + 1) % (int)DecalMirror.NUM);
		}

		public void CycleClip(int idx, bool reverse)
		{
			if (reverse) {
				clip[idx] = (DecalClip)(((int)clip[idx] + (int)DecalClip.NUM - 1) % (int)DecalClip.NUM);
			} else {
				clip[idx] = (DecalClip)(((int)clip[idx] + 1) % (int)DecalClip.NUM);
			}
			UpdateClipNormals();
		}

		public void Set4Clips(DecalClip dc1, DecalClip dc2, DecalClip dc3, DecalClip dc4)
		{
			clip[(int)EdgeOrder.RIGHT] = dc1;
			clip[(int)EdgeOrder.DOWN] = dc2;
			clip[(int)EdgeOrder.LEFT] = dc3;
			clip[(int)EdgeOrder.UP] = dc4;
			UpdateClipNormals();
		}

		public void ChangeRepeat(int u, int v)
		{
			repeat_u = MathHelper.Clamp(repeat_u + u, 0, 15);
			repeat_v = MathHelper.Clamp(repeat_v + v, 0, 15);
		}

		public void ChangeOffset(int u, int v)
		{
			offset_u = MathHelper.Clamp(offset_u + u, -8, 8);
			offset_v = MathHelper.Clamp(offset_v + v, -8, 8);
		}

		public void ChangeRotation(int inc)
		{
			rotation = rotation + inc;
			if (rotation < 0) {
				rotation = 7;
			} else if (rotation > 7) {
				rotation = 0;
			}
		}

		public void ResetSettings()
		{
			repeat_u = repeat_v = 1;
			offset_u = offset_v = 0;
			rotation = 0;
		}
	}
}