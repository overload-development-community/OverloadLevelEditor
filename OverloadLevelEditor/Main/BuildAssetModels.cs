using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AssetStudio;
using Vector3 = OpenTK.Vector3;
using Vector2 = OpenTK.Vector2;
using System.IO;
using Texture2DDecoder;

namespace OverloadLevelEditor
{
	static class AssetStudioExtensions
	{
		public static OpenTK.Vector3 ToOpenTK(this AssetStudio.Vector3 v)
		{
			return new OpenTK.Vector3(v.X, v.Y, v.Z);
		}

		public static OpenTK.Vector2 ToOpenTK(this AssetStudio.Vector2 v)
		{
			return new OpenTK.Vector2(v.X, v.Y);
		}

		public static OpenTK.Quaternion ToOpenTK(this AssetStudio.Quaternion q)
		{
			return new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W);
		}
	}

	public struct MaterialInfo
	{
		public int tex_width, tex_height;
		public byte[] tex_data;
		public int tex_id;
		public bool no_draw;
		public bool emissive;
		public bool no_cull; // draw on both sides
	}

	class MeshBuilder
	{
		private Dictionary<string, MaterialInfo> mat_infos;
		Matrix4 transform;
		List<Material[]> load_materials;
		bool is_load;
		public int index_value;
		Vector2 monitor_uv1, monitor_uv2;
		bool monitor_sideways;
		public bool is_monitor;

		public MeshBuilder(Dictionary<string, MaterialInfo> mat_infos, bool is_load)
		{
			this.mat_infos = mat_infos;
			transform = Matrix4.Identity;
			if (is_load)
				load_materials = new List<Material[]>();
			this.is_load = is_load;
		}

		public static Mesh GetMesh(MeshRenderer meshR)
		{
			GameObject gameObject;
			Mesh mesh;
			if (meshR.m_GameObject.TryGet(out gameObject) &&
				gameObject.m_MeshFilter != null &&
				gameObject.m_MeshFilter.m_Mesh.TryGet(out mesh))
				return mesh;
			return null;
		}

		private static byte[] GetDataOfs(StreamingInfo m_StreamData, SerializedFile assetsFile, int ofs, int len)
		{
			ResourceReader resourceReader;
			if (!string.IsNullOrEmpty(m_StreamData?.path)) {
				resourceReader = new ResourceReader(m_StreamData.path, assetsFile, m_StreamData.offset + (long)ofs, len);
				return resourceReader.GetData();
			} else {
				//resourceReader = new ResourceReader(reader, reader.BaseStream.Position, image_data_size);
				throw new Exception("embedded image data not supported");
			}
		}

		private static byte[] DecodeRGBA32(byte[] image_data, int w, int h, byte[] buff)
		{
			for (var i = 0; i < buff.Length; i += 4) {
				buff[i] = image_data[i + 2];
				buff[i + 1] = image_data[i + 1];
				buff[i + 2] = image_data[i + 0];
				buff[i + 3] = image_data[i + 3];
			}
			return buff;
		}

		private static byte[] TextureConverter(Texture2D tex, out int width, out int height)
		{
			bool is_dxt_crunch = tex.m_TextureFormat == TextureFormat.DXT1Crunched || tex.m_TextureFormat == TextureFormat.DXT5Crunched;
			bool is_32 = tex.m_TextureFormat == TextureFormat.RGBA32;
			int mip_count = tex.m_MipCount, w = tex.m_Width, h = tex.m_Height, ofs = 0, level = 0;
			int mip_size = tex.m_TextureFormat == TextureFormat.DXT1 || tex.m_TextureFormat == TextureFormat.DXT1Crunched ? w * h / 2 :
				is_32 ? w * h * 4 : w * h;
			while (mip_count > 0 && w > 256) {
				mip_count--;
				ofs += mip_size;
				w >>= 1;
				h >>= 1;
				mip_size >>= 2;
				level++;
			}

			width = w;
			height = h;

			byte[] data;
			if (is_dxt_crunch) {
				byte[] hdr = GetDataOfs(tex.m_StreamData, tex.assetsFile, 0, 65536); // start with max 64K header
				uint dataOfs, dataSize, resultSize;
				IntPtr context = TextureDecoder.UnpackUnityCrunchInit(hdr, level, out dataOfs, out dataSize, out resultSize);
				if (context.Equals(IntPtr.Zero)) {
					if (resultSize == 0) // invalid header
						return null;
					// retry with larger header
					hdr = GetDataOfs(tex.m_StreamData, tex.assetsFile, 0, (int)resultSize);
					context = TextureDecoder.UnpackUnityCrunchInit(hdr, level, out dataOfs, out dataSize, out resultSize);
					if (context.Equals(IntPtr.Zero))
						return null;
				}
				byte[] level_data = GetDataOfs(tex.m_StreamData, tex.assetsFile, (int)dataOfs, (int)dataSize);
				data = TextureDecoder.UnpackUnityCrunchLevelData(context, level, level_data, resultSize);
				TextureDecoder.UnpackUnityCrunchDone(context);
			} else {
				int len = mip_size;
				data = GetDataOfs(tex.m_StreamData, tex.assetsFile, ofs, len);
			}

			var imageBuff = new byte[w * h * 4];
			if (tex.m_TextureFormat == TextureFormat.DXT5 || tex.m_TextureFormat == TextureFormat.DXT5Crunched)
				TextureDecoder.DecodeDXT5(data, w, h, imageBuff);
			else if (tex.m_TextureFormat == TextureFormat.BC7)
				TextureDecoder.DecodeBC7(data, w, h, imageBuff);
			else if (tex.m_TextureFormat == TextureFormat.DXT1 || tex.m_TextureFormat == TextureFormat.DXT1Crunched)
				TextureDecoder.DecodeDXT1(data, w, h, imageBuff);
			else if (tex.m_TextureFormat == TextureFormat.RGBA32)
				DecodeRGBA32(data, w, h, imageBuff);
			else {
				System.Diagnostics.Debug.WriteLine(tex.m_Name + ": unknown texture format " + tex.m_TextureFormat);
				return null;
			}

			return imageBuff;
		}

		public void BuildObj(MeshRenderer meshR)
		{
			var materials = meshR.m_Materials.Select(matPtr => {
					Material mat;
					matPtr.TryGet(out mat);
					return mat;
				}).ToArray();
			if (is_load) {
				load_materials.Add(materials);
				return;
			}
			var mesh = GetMesh(meshR);
			if (mesh == null)
				return;
			AddMesh(materials, mesh);
		}

		private static UnityTexEnv MaterialGetTexEnv(Material mat, string name)
		{
			return mat.m_SavedProperties.m_TexEnvs.Where(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
		}

		private static Texture2D TexEnvTexture(UnityTexEnv texEnv)
		{
			Texture2D tex = null;
			if (texEnv != null)
				texEnv.m_Texture.TryGet(out tex);
			return tex;
		}

		private static Vector3? MaterialGetColor(Material mat, string name)
		{
			var colorSeq = mat.m_SavedProperties.m_Colors.Where(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
			if (colorSeq.Any()) {
				var uColor = colorSeq.First().Value;
				var c = new Vector3(uColor.R, uColor.G, uColor.B);
				return c;
			}
			return null;
		}

		private static byte ClampByte(int n)
		{
			return n < 0 ? (byte)0 : n > 255 ? (byte)255 : (byte)n;
		}

		private static void MergeTex(byte[] tex, byte[] etex, Vector3 c)
		{
			int n = tex.Length;
			for (int i = 0; i < n; i += 4) {
				if (etex[i + 3] >= 128) { // visible alpha
					tex[i] = ClampByte(tex[i] + (int)(etex[i] * c.Z));
					tex[i + 1] = ClampByte(tex[i + 1] + (int)(etex[i + 1] * c.Y));
					tex[i + 2] = ClampByte(tex[i + 2] + (int)(etex[i + 2] * c.X));
				}
			}
		}

		private static void AddTex(byte[] tex, byte[] c)
		{
			int n = tex.Length;
			for (int i = 0; i < n; i += 4) {
				tex[i] = ClampByte(tex[i] + c[0]);
				tex[i + 1] = ClampByte(tex[i + 1] + c[1]);
				tex[i + 2] = ClampByte(tex[i + 2] + c[2]);
			}
		}

		private static void MergeTexChan(byte[] tex, byte[] etex, int chan_idx, Vector3 c)
		{
			int n = tex.Length;
			for (int i = 0; i < n; i += 4) {
				byte v = etex[i + chan_idx];
				if (v != 0) {
					tex[i] = ClampByte(tex[i] + (int)(v * c.Z));
					tex[i + 1] = ClampByte(tex[i + 1] + (int)(v * c.Y));
					tex[i + 2] = ClampByte(tex[i + 2] + (int)(v * c.X));
				}
			}
		}

		private HashSet<string> ignore_material = new HashSet<string> { "forcefield5", "kineticfield_subtle4",
			"kineticfield_subtle", "kineticfield_subtle3", "forcefield6",
			"automap_solid_door", "automap_solid_door_security", "automap_solid" };

		// simulate just enough of the shaders to make models recognizable
		private MaterialInfo ConvertMaterial(Material mat)
		{
			MaterialInfo ret = new MaterialInfo();
			if (mat == null)
				return ret;
			if (mat.m_Name.Contains("thruster") || ignore_material.Contains(mat.m_Name)) {
				ret.no_draw = true;
				return ret;
			}
			var tex = TexEnvTexture(MaterialGetTexEnv(mat, "_MainTex") ?? MaterialGetTexEnv(mat, "_Diffuse"));

			var etexEnv = MaterialGetTexEnv(mat, "_Emission") ?? MaterialGetTexEnv(mat, "_EmissionMap");
			var etex = TexEnvTexture(etexEnv);
			int etex_width = 0, etex_height = 0;
			var etex_data = etex != null ? TextureConverter(etex, out etex_width, out etex_height) : null;
			if (tex == null && etex != null) {
				Vector3 c = MaterialGetColor(mat, "_GlowColor") ?? new Vector3(0f, 0f, 0f);
				ret.tex_width = etex_width; ret.tex_height = etex_height;
				int size = etex_width * etex_height * 4;
				byte[] data = ret.tex_data = new byte[size];
				bool forcefield = mat.m_Name.StartsWith("forcefield");
				byte alpha = forcefield ? (byte)192 : (byte)255;
				for (int i = 0; i < size; i += 4) {
					data[i] = (byte)(c.Z * 255);
					data[i + 1] = (byte)(c.Y * 255);
					data[i + 2] = (byte)(c.X * 255);
					data[i + 3] = alpha;
				}
				ret.emissive = true;
				ret.no_cull = forcefield;
			} else if (tex != null) {
				ret.tex_data = TextureConverter(tex, out ret.tex_width, out ret.tex_height);
			}
			if (ret.tex_data != null && etex_data != null && etex_width == ret.tex_width && etex_height == ret.tex_height) {
				Vector3? eye = MaterialGetColor(mat, "_Color_eye");
				if (eye != null) { // robot
					MergeTexChan(ret.tex_data, etex_data, 1, eye.Value * 1.5f);
					Vector3? internalCol = MaterialGetColor(mat, "_Color_internal");
					if (internalCol.HasValue)
						MergeTexChan(ret.tex_data, etex_data, 2, internalCol.Value);
					Vector3? thruster = MaterialGetColor(mat, "_Color_thruster");
					if (thruster.HasValue)
						MergeTexChan(ret.tex_data, etex_data, 0, thruster.Value);
					Vector3? headlight = MaterialGetColor(mat, "_Color_headlight");
					if (headlight.HasValue)
						MergeTexChan(ret.tex_data, etex_data, 3, headlight.Value);
					return ret;
				} else {
					var c = MaterialGetColor(mat, "_EmissionColor") ?? new Vector3(1f, 1f, 1f);
					if (etex_data != null && etex_width == ret.tex_width && etex_height == ret.tex_height) {
						MergeTex(ret.tex_data, etex_data, c);
					}
					return ret;
				}
			}
			return ret;
		}

		private int GLAddTexture(byte[] tex_data, int tex_width, int tex_height)
		{
			int tex_id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, tex_id);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, tex_width, tex_height, 0,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, tex_data);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return tex_id;
		}

		public void LoadMaterials(Material[] materials)
		{
			foreach (var mat in materials) {
				if (mat == null)
					continue;
				if (mat_infos.ContainsKey(mat.m_Name))
					continue;

				var info = ConvertMaterial(mat);

				if (info.tex_data != null)
					info.tex_id = GLAddTexture(info.tex_data, info.tex_width, info.tex_height);

				mat_infos.Add(mat.m_Name, info);
			}
		}

		public void AddMesh(Material[] materials, Mesh mesh)
		{
			var hasNormal = mesh.m_Normals?.Length > 0;
			var hasUV = mesh.m_UV0?.Length > 0;

			var vertC = mesh.m_Vertices.Length == mesh.m_VertexCount * 4 ? 4 : 3;
			var normC = hasNormal && mesh.m_Normals.Length == mesh.m_VertexCount * 4 ? 4 : 3;
			var uvC = hasUV && mesh.m_UV0.Length == mesh.m_VertexCount * 3 ? 3 : 2;

			var vertices = new Vector3[mesh.m_VertexCount];
			var normals = new Vector3[mesh.m_VertexCount];
			var uvs = new Vector2[mesh.m_VertexCount];
			for (var j = 0; j < mesh.m_VertexCount; j++) {
				vertices[j] = new Vector3(mesh.m_Vertices[j * vertC], mesh.m_Vertices[j * vertC + 1], mesh.m_Vertices[j * vertC + 2]);
				if (hasNormal) {
					normals[j] = new Vector3(mesh.m_Normals[j * normC], mesh.m_Normals[j * normC + 1], mesh.m_Normals[j * normC + 2]);
				}
				if (hasUV) {
					uvs[j] = new Vector2(mesh.m_UV0[j * uvC], mesh.m_UV0[j * uvC + 1]);
				}
			}
			int idx = 0;
			for (int i = 0; i < mesh.m_SubMeshes.Length; i++) {
				Material mat = materials[i];
				MaterialInfo info;
				if (mat != null)
					mat_infos.TryGetValue(mat.m_Name, out info);
				else
					info = new MaterialInfo();
				if (info.no_draw)
					continue;
				if (info.no_cull)
					GL.Disable(EnableCap.CullFace);
				GL.BindTexture(TextureTarget.Texture2D, info.tex_id);

				if (hasUV && info.emissive && is_monitor) {
					Vector2 uv_scale = monitor_uv1;
					Vector2 uv_offset = monitor_uv2 +
						(monitor_sideways ?
							new Vector2((index_value % 3) / 3f, (index_value % 2) / 2f) :
							new Vector2((index_value % 2) / 2f, (index_value % 3) / 3f));
					for (int ii = 0; ii < mesh.m_VertexCount; ii++)
						uvs[ii] = (uvs[ii] * uv_scale) + uv_offset;
				}

				int endIdx = idx + (int)mesh.m_SubMeshes[i].indexCount;
				GL.Begin(PrimitiveType.Triangles);
				Vector3 normal = default(Vector3);
				for (int j = idx; j < endIdx; j += 3) {
					if (!hasNormal)
						normal = Utility.FindNormal(vertices[(int)mesh.m_Indices[j]], vertices[(int)mesh.m_Indices[j + 1]], vertices[(int)mesh.m_Indices[j + 2]]);
					for (int l = 0; l < 3; l++) {
						int k = (int)mesh.m_Indices[j + l];
						if (hasUV)
							GL.TexCoord2(uvs[k]);
						GL.Normal3(hasNormal ? normals[k] : normal);
						GL.Vertex3(vertices[k]);
					}
				}
				GL.End();
				idx = endIdx;
				if (info.no_cull)
					GL.Enable(EnableCap.CullFace);
			}
		}

		public void BuildObj(Transform transform)
		{
			GameObject gameObject;
			transform.m_GameObject.TryGet(out gameObject);
			if (!gameObject.m_IsActive) {
				return;
			}

			if (!is_load) {
				var m = Matrix4.CreateScale(transform.m_LocalScale.ToOpenTK()) *
					Matrix4.CreateFromQuaternion(transform.m_LocalRotation.ToOpenTK()) *
					Matrix4.CreateTranslation(transform.m_LocalPosition.ToOpenTK());
				GL.PushMatrix();
				GL.MultMatrix(ref m);
			}

			if (gameObject.m_MeshRenderer != null) {
				BuildObj(gameObject.m_MeshRenderer);
			}
			foreach (var childPtr in transform.m_Children) {
				Transform child;
				if (childPtr.TryGet(out child))
					BuildObj(child);
			}

			if (!is_load) {
				GL.PopMatrix();
			}
		}

		private static Guid prop_monitor_script_id = new Guid(new byte[] { 0x23, 0x78, 0x45, 0x1e, 0x02, 0x6c, 0x00, 0x14, 0x8f, 0xd2, 0x59, 0x76, 0xad, 0x34, 0x2b, 0x82 });
		
		public void BuildObj(GameObject gameObject)
		{
			if (gameObject.m_Components.Length == 3) { // extract monitor data
				Component comp;
				gameObject.m_Components[1].TryGet(out comp);
				if (comp != null && new Guid(comp.serializedType.m_ScriptID).Equals(prop_monitor_script_id)) {
					is_monitor = true;
					var monitor_data = comp.GetRawData();
					monitor_uv1 = new Vector2(BitConverter.ToSingle(monitor_data, 0x78), BitConverter.ToSingle(monitor_data, 0x7c));
					monitor_uv2 = new Vector2(BitConverter.ToSingle(monitor_data, 0x80), BitConverter.ToSingle(monitor_data, 0x84));
					monitor_sideways = monitor_data[0x88] != 0;
					//System.Diagnostics.Debug.WriteLine(string.Format("uv1 {0} uv2 {1} sideways {2}", monitor_uv1, monitor_uv2, monitor_sideways));
				}
			}
			if (gameObject.m_MeshRenderer != null) {
				BuildObj(gameObject.m_MeshRenderer);
			}
			foreach (var childPtr in gameObject.m_Transform.m_Children) {
				Transform child;
				if (childPtr.TryGet(out child))
					BuildObj(child);
			}
		}

		public void LoadAllMaterials()
		{
			foreach (var materials in load_materials)
				LoadMaterials(materials);
		}
	}

	class BuildAssetModels
	{
		public static AssetsManager assetsManager = new AssetsManager();
		public static Dictionary<string, MaterialInfo> mat_infos = new Dictionary<string, MaterialInfo>();

		private static SerializedFile file;
		private static Dictionary<Tuple<int, int>, GameObject> EntityGameObjects = new Dictionary<Tuple<int, int>, GameObject>();
		private static Dictionary<Tuple<int, int, int>, int> EntityDisplayLists = new Dictionary<Tuple<int, int, int>, int>();

		private static int ListNext = 32000;

		public static void Reset()
		{
			foreach (var x in EntityDisplayLists)
				GL.DeleteLists(x.Value, 1);
			EntityDisplayLists.Clear();
			ListNext = 32000;
			foreach (var x in mat_infos)
				GL.DeleteTexture(x.Value.tex_id);
			mat_infos.Clear();
		}

		private static Dictionary<string, string> EntityRename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
			{ "entity_item_hunter4pack", "entity_item_hunter" },
			{ "entity_item_falcon4pack", "entity_item_falcon" },
			{ "entity_special_player_ship", "entity_special_player_start" },
			{ "entity_special_player_start", null },
		};

		// retrieve or create an OpenGL list for the entity
		public static int GetEntityList(EntityType type, int subType, int? index)
		{
			int list;
			if (EntityDisplayLists.TryGetValue(new Tuple<int, int, int>((int)type, subType, -1), out list))
				return list;
			if (index != null &&
				EntityDisplayLists.TryGetValue(new Tuple<int, int, int>((int)type, subType, (int)index % 6), out list))
				return list;

			GameObject go;
			if (!EntityGameObjects.TryGetValue(new Tuple<int, int>((int)type, subType), out go))
				return -1;

			// OpenGL list does not yet exist, build it

			// first phase: collect materials
			var builder = new MeshBuilder(mat_infos, true);
			builder.BuildObj(go);
			builder.LoadAllMaterials();

			int index_val = builder.is_monitor && index != null ? (int)index % 6 : -1;
			
			list = ++ListNext;

			GL.DeleteLists(list, 1);
			GL.NewList(list, ListMode.Compile);

			// second phase: draw meshes
			builder = new MeshBuilder(mat_infos, false) { index_value = index_val };
			builder.BuildObj(go);

			GL.EndList();

			EntityDisplayLists.Add(new Tuple<int, int, int>((int)type, subType, index_val), list);
			return list;
		}

		public static System.Type GetEntityTypeEnumType(EntityType type)
		{
			switch (type) {
				case EntityType.ENEMY:
					return typeof(Overload.EnemyType);
				case EntityType.PROP:
					return typeof(PropSubType);
				case EntityType.ITEM:
					return typeof(ItemSubType);
				case EntityType.DOOR:
					return typeof(DoorSubType);
				case EntityType.SCRIPT:
					return typeof(ScriptSubType);
				case EntityType.TRIGGER:
					return typeof(TriggerSubType);
				case EntityType.LIGHT:
					return typeof(LightSubType);
				case EntityType.SPECIAL:
					return typeof(SpecialSubType);
				default:
					return null;
			}
		}

		public static void Prepare()
		{
			if (file != null || String.IsNullOrEmpty(Program.m_gamedir))
				return;

            var st = new System.Diagnostics.Stopwatch();
            st.Start();
			assetsManager.LoadFiles(new [] { Path.Combine(Program.m_gamedir, "Overload_Data", "resources.assets") });
            st.Stop();
            System.Diagnostics.Debug.WriteLine("BuildAssetModels.Prepare Reading assets: " + st.Elapsed);

			var EntityIds = new Dictionary<string, Tuple<int, int>>(StringComparer.OrdinalIgnoreCase);
			foreach (var type in new [] { EntityType.DOOR, EntityType.ENEMY, EntityType.ITEM, EntityType.PROP, EntityType.SPECIAL }) {
				string pre = "entity_" + type.ToString() + "_";
				foreach (var sub_type_field in GetEntityTypeEnumType(type).GetFields()) {
					if (sub_type_field.IsLiteral) {
						EntityIds.Add(pre + sub_type_field.Name, new Tuple<int, int>((int)type, (int)sub_type_field.GetValue(null)));
					}
				}
			}

            file = assetsManager.assetsFileList[0];
			foreach (var obj in file.Objects) {
				if (obj.type == ClassIDType.GameObject) {
					var go = (GameObject)obj;
					if (go.m_Name.StartsWith("entity_")) {
						string name = go.m_Name, new_name;
						if (EntityRename.TryGetValue(name, out new_name)) {
							if (new_name == null)
								continue;
							name = new_name;
						}
						Tuple<int, int> entityId;
						if (EntityIds.TryGetValue(name, out entityId)) {
							EntityGameObjects.Add(entityId, go);
						}
					}
				}
			}
			
			System.Diagnostics.Debug.WriteLine($"Finished loading {assetsManager.assetsFileList.Count} files with {file.Objects.Count} exportable assets.");
        }
    }
}
