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
using System.Collections.Generic;

// DMESH - Editor
// Editor-specific functions/variables for DMesh geometry

namespace OverloadLevelEditor
{
	// DMESH - A decal mesh object (the base piece that gets tiled)
	// - Saved in local space, facing Y up, tiles every 4 in XZ
	public partial class DMesh
	{
		public const int MAX_VERTS = 1024; // Arbitrary, increase as needed

		// NEW for decal editor (not used in LevelEditor)
		public bool dirty = false;
		Editor editor = null;

		public List<DPoly> polygon = new List<DPoly>();
		public List<DVert> vert_info = new List<DVert>();

		public int selected_poly;
		public int selected_vert;
		
		public int num_marked_polys;
		public int num_marked_verts;
		
		public void Init(Editor e)
		{
			editor = e;
			flags = 0;
			name = "default";
			tex_name.Clear();
			//light.Clear();
			color.Clear();
			for (int i = 0; i < NUM_COLORS; i++) {
				color.Add(System.Drawing.Color.White);
			}
			triangle.Clear();
			vertex.Clear();
			vert_info.Clear();
			polygon.Clear();

			selected_poly = -1;
			selected_vert = -1;
			num_marked_polys = 0;
			num_marked_verts = 0;
		}

		public void CopyDMesh(DMesh src, bool ignore_light = false)
		{
			Init(editor);
			Copy(src, ignore_light);
		}

		public List<int> m_tex_gl_id;
		
		public void UpdateGLTextures(TextureManager tm)
		{
			for (int i = 0; i < tex_name.Count; i++) {
				m_tex_gl_id[i] = tm.FindTextureIDByName(tex_name[i]);
				if (m_tex_gl_id[i] == -1) {
					m_tex_gl_id[i] = tm.FindTextureIDByName("_default");
				}
			}
		}

		public void AddTexture(int id, string name)
		{
			m_tex_gl_id.Add(id);
			tex_name.Add(name);
		}

		public int FindTextureIndex(string name)
		{
			name = name.ToLower();
			for (int i = 0; i < tex_name.Count; i++) {
				if (tex_name[i].ToLower() == name) {
					return i;
				}
			}
			return -1;
		}

		public void SerializePolys(JObject root)
		{
			// No real need to save vert info

			JObject j_polys = new JObject();
			root["polys"] = j_polys;
			for (int i = 0; i < polygon.Count; i++) {
				JObject j_poly = new JObject();
				polygon[i].Serialize(j_poly);
				j_polys[i.ToString()] = j_poly;
			}
		}

		public void DeserializePolys(JObject root)
		{
			// Just initialize vert info
			for (int i = 0; i < vertex.Count; i++) {
				DVert d_vert = new DVert();
				vert_info.Add(d_vert);
			}

			JObject j_polys = root["polys"].GetObject();
			foreach (var kvp in j_polys) {
				JObject j_poly = kvp.Value.GetObject();
				DPoly d_poly = new DPoly(j_poly);
				polygon.Add(d_poly);
			}
		}

		public bool WasConverted()
		{
			return (polygon.Count > 0);
		}

		public void DeconvertPolysToTris()
		{
			//Number polygons
			for (int polynum = 0; polynum < polygon.Count; polynum++) {
				polygon[polynum].num = polynum;
			}

			// Go through all polygons and output triangles with same flags/etc
			triangle.Clear();
			foreach (DPoly p in polygon) {
				triangle.AddRange(p.DeconvertPoly());
			}

			int num_collide = 0;
			int num_render = 0;
			int num_useless = 0;
			foreach (DTriangle t in triangle) {
				if (t.flags == (int)FaceFlags.NO_COLLIDE + (int)FaceFlags.NO_RENDER) {
					num_useless += 1;
				}
				if (t.flags == (int)FaceFlags.NO_RENDER || t.flags == 0) {
					num_collide += 1;
				}
				if (t.flags == (int)FaceFlags.NO_COLLIDE || t.flags == 0) {
					num_render += 1;
				}
			}

			editor.AddOutputText("Deconverted triangles: " + triangle.Count.ToString() + " from polygons: " + polygon.Count.ToString() + "  (COLLIDE: " + num_collide + " RENDER: " + num_render + ")");
			if (num_useless > 0) {
				editor.AddOutputText("Mesh has some useless faces!!!");
			}
		}

		public void ConvertTrisToPolysRaw()
		{
			polygon.Clear();
			foreach (DTriangle t in triangle) {
				DPoly p = new DPoly(t);
				polygon.Add(p);
			}
		}
	}

	// Extra info for vertices (DMesh editor only)
	public class DVert
	{
		public bool marked = false;
		public bool tag = false;

		public DVert()
		{
		}

		public DVert(bool mrk)
		{
			marked = mrk;
		}

		public DVert(DVert src)
		{
			marked = src.marked;
			tag = src.tag;
		}
	}

	public partial class DPoly
	{
		public const int MAX_VERTS = 32;
		public int num_verts = 0;
		public List<int> vert = new List<int>();
		public List<Vector3> normal = new List<Vector3>();
		public List<Vector2> tex_uv = new List<Vector2>();
		public int tex_index = 0;
		public byte smoothing = 0;
		public int flags = 0;
		public bool marked = false;
		public bool tag = false;
		public bool[] uv_mark = new bool[MAX_VERTS]; // This is for UV editing only, never saved
		public int num;   //Which polygon is this
		public Vector3 face_normal;

		public DPoly(int num_vrts, List<int> vrt_idx, List<Vector3> nrml, List<Vector2> uv, int tex_idx)
		{
			num_verts = num_vrts;
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(vrt_idx[i]);
				normal.Add(nrml[i]);
				tex_uv.Add(uv[i]);
			}

			tex_index = tex_idx;
			flags = 0;
			smoothing = 0;
			marked = false;
		}

		public void ClearLists()
		{
			vert.Clear();
			normal.Clear();
			tex_uv.Clear();
		}

		public DPoly(DPoly src)
		{
			num_verts = src.num_verts;
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(src.vert[i]);
				normal.Add(src.normal[i]);
				tex_uv.Add(src.tex_uv[i]);
			}

			tex_index = src.tex_index;
			flags = src.flags;
			smoothing = src.smoothing;
			marked = src.marked;
		}

		public bool IsFlagSet(FaceFlags ff)
		{
			return ((flags & (int)ff) == (int)ff);
		}

		public void ReverseWindingOrder()
		{
			DPoly flip_tri = new DPoly(this);
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(flip_tri.vert[num_verts - 1 - i]);
				normal.Add(flip_tri.normal[num_verts - 1 - i]);
				tex_uv.Add(flip_tri.tex_uv[num_verts - 1 - i]);
			}
		}

		public List<DTriangle> DeconvertPoly()
		{
			// Triange for each vert beyond 2 (0-1-2, 0-2-3, 0-3-4, etc)
			List<DTriangle> tri_list = new List<DTriangle>();

			int[] vrt = new int[3];
			Vector3[] nrml = new Vector3[3];
			Vector2[] uv = new Vector2[3];
			for (int i = 0; i < num_verts - 2; i++) {
				vrt[0] = vert[0];
				vrt[1] = vert[i + 1];
				vrt[2] = vert[i + 2];
				nrml[0] = normal[0];
				nrml[1] = normal[i + 1];
				nrml[2] = normal[i + 2];
				uv[0] = tex_uv[0];
				uv[1] = tex_uv[i + 1];
				uv[2] = tex_uv[i + 2];
				
				DTriangle dtri = new DTriangle(vrt, nrml, uv, tex_index);
				dtri.poly_num = num;
				dtri.flags = flags;
				smoothing = 0;
				tri_list.Add(dtri);
			}

			return tri_list;
		}

		public DPoly(DTriangle dtri)
		{
			num_verts = 3;
			flags = dtri.flags;
			smoothing = 0;
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(dtri.vert[i]);
				normal.Add(dtri.normal[i]);
				tex_uv.Add(dtri.tex_uv[i]);
				tex_index = dtri.tex_index;
			}
		}

		public DPoly(JObject root)
		{
			tex_index = root["tex_index"].GetInt(0);
			flags = root["flags"].GetInt(0);
			smoothing = (byte)root["smoothing"].GetInt(0);
			num_verts = root["num_verts"].GetInt(0);

			JArray j_verts = root["verts"].GetArray();
			JArray j_normals = root["normals"].GetArray();
			JArray j_uvs = root["uvs"].GetArray();
			Vector3 v3;
			Vector2 v2;
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(j_verts[i].GetInt(0));
				v3.X = j_normals[i]["x"].GetFloat(0.0f);
				v3.Y = j_normals[i]["y"].GetFloat(0.0f);
				v3.Z = j_normals[i]["z"].GetFloat(0.0f);
				normal.Add(v3);
				v2.X = j_uvs[i]["u"].GetFloat(0.0f);
				v2.Y = j_uvs[i]["v"].GetFloat(0.0f);
				tex_uv.Add(v2);
			}
		}

		public void Serialize(JObject root)
		{
			root["tex_index"] = tex_index;
			root["flags"] = flags;
			root["smoothing"] = (int)smoothing;
			root["num_verts"] = num_verts;

			JArray j_verts = new JArray();
			JArray j_normals = new JArray();
			JArray j_uvs = new JArray();
			for (int i = 0; i < num_verts; i++) {
				j_verts.Add(vert[i]);

				JObject j_normal = new JObject();
				j_normal["x"] = normal[i].X;
				j_normal["y"] = normal[i].Y;
				j_normal["z"] = normal[i].Z;
				j_normals.Add(j_normal);

				JObject j_uv = new JObject();
				j_uv["u"] = tex_uv[i].X;
				j_uv["v"] = tex_uv[i].Y;
				j_uvs.Add(j_uv);
			}

			root["verts"] = j_verts;
			root["normals"] = j_normals;
			root["uvs"] = j_uvs;
		}
	}
}
