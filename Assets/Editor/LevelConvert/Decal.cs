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
using Newtonsoft.Json.Linq;

// DECAL
// Decals define the data for how a DMesh is repeated
// There can be multiple decals per side (2 per side)

namespace OverloadLevelEditor
{
	public enum DecalAlign
	{
		TEXTURE,
		CENTER,
		EDGE_RIGHT,
		EDGE_DOWN,
		EDGE_LEFT,
		EDGE_UP,
		
		NUM,
	}

	public enum DecalMirror
	{
		OFF,
		MIRROR_U,
		MIRROR_V,

		NUM,
	}

	public enum DecalClip
	{
		NONE, // Cannot use CAP (all others can)
		WHOLE,
		PERP,
		SEGMENT,
		AVERAGE,
		ADJACENT,

		NUM,
	}

	public enum DecalCap
	{
		NONE,
		DEFAULT_UV,
		ADJUSTED_UV,

		NUM,
	}

	public enum EdgeOrder
	{
		RIGHT,
		DOWN,
		LEFT,
		UP,
		
		NUM,
	}

	public partial class Decal
	{
		public string mesh_name;

		// Not saved, this is a reference to the decal mesh and the procedurally-generated geometry
		public DMesh dmesh;
		public GMesh gmesh;

		// Needed to easily hide/unhide decals without overwriting data (sides with neighbors default to hiding, for instance)
		public bool hidden;

		public DecalAlign align;
		public DecalMirror mirror;
		public int rotation; // Stored in 90 degree increments (may switch to 45 degree increments)

		public int repeat_u; // 0 = Max
		public int repeat_v; // 0 = Max
		public int offset_u; // Measured in 1 meter increments (may switch to 1/2 meter)
		public int offset_v; // Measured in 1 meter increments (may switch to 1/2 meter)

		public const int NUM_EDGES = 4;
		public DecalClip[] clip = new DecalClip[NUM_EDGES];
		public Vector3[] clip_normal = new Vector3[NUM_EDGES];

		// Not currently being used, may discard at some point
		public DecalCap[] cap = new DecalCap[NUM_EDGES];

		public Side side;
		public int num;

		public Decal(Side s, int n)
		{
			side = s;
			num = n;

			Reset();
		}

		public void Reset()
		{
			mesh_name = "";
			hidden = false;

			dmesh = null;
			gmesh = null;

			align = DecalAlign.CENTER;
			mirror = DecalMirror.OFF;
			rotation = 0;
			repeat_u = 1;
			repeat_v = 1;
			offset_u = 0;
			offset_v = 0;

			for (int i = 0; i < NUM_EDGES; i++) {
				clip[i] = DecalClip.NONE;
				cap[i] = DecalCap.NONE;
			}
		}

		public void Copy(Decal src)
		{
			mesh_name = src.mesh_name;

			dmesh = src.dmesh;
			gmesh = src.gmesh;

			hidden = src.hidden;
			align = src.align;
			mirror = src.mirror;
			rotation = src.rotation;
			repeat_u = src.repeat_u;
			repeat_v = src.repeat_v;
			offset_u = src.offset_u;
			offset_v = src.offset_v;

			for (int i = 0; i < NUM_EDGES; i++) {
				clip[i] = src.clip[i];
				cap[i] = src.cap[i];
			}
		}

		//Returns true if a decal has actually been applied
		public bool Applied
		{
			get { return this.mesh_name != ""; }
		}

		public void MaybeUpdateGMesh( bool report_mesh_issues, out string decal_issues )
		{
			if( string.IsNullOrEmpty( mesh_name ) || hidden ) {
				decal_issues = string.Empty;
				gmesh = null;		//Clear out rendered mesh
				return;
			}

			dmesh = side.level.GetDMeshByName( mesh_name );

			System.Text.StringBuilder issues_text = new System.Text.StringBuilder();

            const bool cleanup_bad_triangles = true;

			if( report_mesh_issues || cleanup_bad_triangles ) {
				string issues;
				if( dmesh != null && dmesh.CheckAndCleanMeshIssues( cleanup_bad_triangles, out issues ) ) {
                    if( report_mesh_issues ) {
                        issues_text.AppendFormat( "Decal \"{0}\" has issues in its source geometry:\n{1}", mesh_name, issues );
                    }
				}
			}

			if( dmesh == null ) {
				decal_issues = string.Empty;
				gmesh = null;     //Clear out rendered mesh
				return;
			}

			mesh_name = dmesh.name;

			gmesh = new GMesh();

			gmesh.SetColors(dmesh.color);
			UpdateClipNormals(); // This must be done before GenerateNonClippedGeometry because 
			gmesh.GenerateNonClippedGeometry(this, dmesh);
#if OVERLOAD_LEVEL_EDITOR
			{
				// Note: We may not be in the UNITY_EDITOR here, but not
				// have valid TextureManager for decal and level textures.
				// This is the case when we are doing an export for user
				// levels. We don't need GL texture data anyway.
				var texman_decal = side.level.editor.tm_decal;
				var texman_level = side.level.editor.tm_level;
				if (texman_decal != null && texman_level != null) {
					gmesh.UpdateGLTextures(texman_decal, texman_level);
				}
			}
#endif
			gmesh.ClipGeometry();

			if( report_mesh_issues || cleanup_bad_triangles ) {
				string issues;
				if( gmesh.CheckAndCleanMeshIssues( cleanup_bad_triangles, out issues ) ) {
                    if( report_mesh_issues ) {
                        issues_text.AppendFormat( "Decal \"{0}\" has issues after generation and clipping:\n{1}", mesh_name, issues );
                    }
				}
			}

			gmesh.OptimizeMesh();

			if( report_mesh_issues || cleanup_bad_triangles ) {
				string issues;
				if( gmesh.CheckAndCleanMeshIssues( cleanup_bad_triangles, out issues ) ) {
                    if( report_mesh_issues ) {
                        issues_text.AppendFormat( "Decal \"{0}\" has issues after optimization:\n{1}", mesh_name, issues );
                    }
				}
			}

			decal_issues = issues_text.ToString();
		}

		public void UpdateClipNormals()
		{
			for (int i = 0; i < NUM_EDGES; i++) {
				UpdateClipNormal(i);
			}
			if (gmesh != null) {
				for (int i = 0; i < (int)NUM_EDGES; i++) {
					gmesh.m_clip_plane[i] = new GClipPlane(this, i);
				}
			}
		}

		public bool IsClipActive(int idx)
		{
			switch (clip[idx]) {
				default:
				case DecalClip.NONE:
				case DecalClip.WHOLE:
					return false;
				case DecalClip.SEGMENT:
				case DecalClip.PERP:
				case DecalClip.AVERAGE:
				case DecalClip.ADJACENT:
					return true;
			}
		}

		public void UpdateClipNormal(int idx)
		{
			Vector3 v;
			Side s2;
			switch (clip[idx]) {
				case DecalClip.ADJACENT:
					// Average of this normal and the next non-neighbor side (falls back to this segment if no neighbor, which is the same as AVERAGE)
					s2 = side.FindAdjacentSideOtherSegment(idx);
					v = s2.FindNormal();
					v = v + side.FindNormal();
					v.Normalize(); // Will always be valid unless you have an infinitely thin wall (in which case that's bad anyway)

					clip_normal[idx] = v;
					break;
				case DecalClip.AVERAGE:
					// Average of this normal and the adjacent side normal
					s2 = side.FindAdjacentSideSameSegment(idx);
					v = s2.FindNormal();
					v = v + side.FindNormal();
					v.Normalize(); // Will always be valid because the segment can't have two coincident sides

					clip_normal[idx] = v;
					break;
				case DecalClip.WHOLE: // This could be changed to same as segment if it doesn't work as desired, or could add second WHOLE variant
				case DecalClip.PERP:
					// The side normal
					clip_normal[idx] = side.FindNormal();
					break;
				case DecalClip.SEGMENT:
					// The edge direction away (in the adjacent side)
					v = side.FindAdjacentSideEdgeDirAway(idx);
					
					clip_normal[idx] = v;
					break;
				default:
					clip_normal[idx] = Vector3.Zero;
					break;
			}
		}

		public float RotationAngle()
		{
			return (Utility.RAD_180 * rotation / 4f);
		}

		public void Serialize(JObject root)
		{
			root["mesh_name"] = this.mesh_name;
			root["align"] = this.align.ToString();
			root["mirror"] = this.mirror.ToString();
			root["rotation"] = this.rotation;
			root["repeat_u"] = this.repeat_u;
			root["repeat_v"] = this.repeat_v;
			root["offset_u"] = this.offset_u;
			root["offset_v"] = this.offset_v;
			root["hidden"] = this.hidden;

			var jClips = new JArray();
			var jCaps = new JArray();
			for (int i = 0; i < NUM_EDGES; ++i) {
				jClips.Add(this.clip[i].ToString());
				jCaps.Add(this.cap[i].ToString());
			}
			root["clips"] = jClips;
			root["caps"] = jCaps;
		}

		public void Deserialize(JObject root)
		{
			this.mesh_name = root["mesh_name"].GetString(string.Empty);
			this.align = root["align"].GetEnum<DecalAlign>(DecalAlign.TEXTURE);
			this.mirror = root["mirror"].GetEnum<DecalMirror>(DecalMirror.OFF);
			this.rotation = root["rotation"].GetInt(0);
			this.repeat_u = root["repeat_u"].GetInt(0);
			this.repeat_v = root["repeat_v"].GetInt(0);
			this.offset_u = root["offset_u"].GetInt(0);
			this.offset_v = root["offset_v"].GetInt(0);
			this.hidden = root["hidden"].GetBool(false);

			var jClips = root["clips"].GetArray();
			var jCaps = root["caps"].GetArray();
			for (int i = 0; i < NUM_EDGES; ++i) {
				this.clip[i] = jClips[i].GetEnum<DecalClip>(DecalClip.NONE);
				this.cap[i] = jCaps[i].GetEnum<DecalCap>(DecalCap.NONE);
			}
		}

		public void DeserializeComplete()
		{
			UpdateClipNormals();
		}


		public bool IsEdgeAlign()
		{
			switch (align) {
				case DecalAlign.EDGE_RIGHT:
				case DecalAlign.EDGE_DOWN:
				case DecalAlign.EDGE_LEFT:
				case DecalAlign.EDGE_UP:
					return true;
				default:
					return false;
			}
		}
	}
}
