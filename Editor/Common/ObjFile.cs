using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ObjFileLib
{
	public class ObjFile
	{
		public struct FaceVert
		{
			public int VertIdx;
			public int UVIdx;
			public int NormIdx;
			public FaceVert(int vertIdx, int uvIdx, int normIdx)
			{
				VertIdx = vertIdx;
				UVIdx = uvIdx;
				NormIdx = normIdx;
			}
		}

		public struct Face
		{
			public FaceVert[] FaceVerts;
			public int MatIdx;

			public Face(FaceVert[] faceVerts, int matIdx)
			{
				FaceVerts = faceVerts;
				MatIdx = matIdx;
			}
		}

		public struct MaterialChannel
		{
			public Color Color;
			public string Texture;
		}

		public class Material
		{
			public string Name;
			public MaterialChannel Diffuse;
		}

		public List<Vector3> Vertices = new List<Vector3>();
		public List<Vector3> Normals = new List<Vector3>();
		public List<Vector2> UVs = new List<Vector2>();
		public List<Face> Faces = new List<Face>();
		public List<Material> Materials = new List<Material>();

		private static float ToFloat(string s)
		{
			return float.Parse(s, CultureInfo.InvariantCulture);
		}

		private static int ToIdx(string s, int count)
		{
			var n = s == "" ? 0 : int.Parse(s, CultureInfo.InvariantCulture);
			return n < 0 ? n + count : n - 1;
		}

		private int NextNonWhiteSpaceIdx(string s, int i)
		{
			while (i < s.Length && Char.IsWhiteSpace(s[i]))
				i++;
			return i;
		}

		private int NextWhiteSpaceIdx(string s, int i)
		{
			while (i < s.Length && !Char.IsWhiteSpace(s[i]))
				i++;
			return i;
		}

		private bool SplitLine(string line, out string keyword, out string arg)
		{
			var i = NextNonWhiteSpaceIdx(line, 0);
			if (i == line.Length || line[i] == '#') {
				keyword = arg = null;
				return false;
			}
			var j = NextWhiteSpaceIdx(line, i);
			keyword = line.Substring(i, j - i);
			i = NextNonWhiteSpaceIdx(line, j);
			arg = line.Substring(i).TrimEnd();
			return true;
		}

		private Dictionary<string, Material> LoadMtl(string filename)
		{
			Material cur = null;
			var materials = new Dictionary<string, Material>();
			foreach (var line in File.ReadLines(filename))
			{
				string keyword, arg;
				if (!SplitLine(line, out keyword, out arg))
					continue;
				switch (keyword)
				{
					case "newmtl":
						cur = new Material();
						cur.Name = arg;
						materials[cur.Name] = cur;
						break;
					case "map_Kd":
						if (cur != null)
							cur.Diffuse.Texture = arg;
						break;
					case "Kd":
						if (cur != null) {
							var parts = arg.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
							cur.Diffuse.Color = Color.FromArgb((int)(ToFloat(parts[0]) * 255), (int)(ToFloat(parts[1]) * 255), (int)(ToFloat(parts[2]) * 255));
						}
						break;
				}
			}
			return materials;
		}

		public void Load(string filename)
		{
			var sepSlash = new char[] { '/' };
			var mtlIdx = new Dictionary<string, int>();
			int curMtl = 0;
			var materials = new Dictionary<string, Material>();

			foreach (var line in File.ReadLines(filename))
			{
				string keyword, arg;
				if (!SplitLine(line, out keyword, out arg))
					continue;
				var parts = arg.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
				switch (keyword)
				{
					case "mtllib":
						var mtlFile = Path.Combine(Path.GetDirectoryName(filename), arg);
						if (File.Exists(mtlFile))
							materials = LoadMtl(mtlFile);
						break;
					case "usemtl":
						string mtl = arg;
						Material m;
						if (!mtlIdx.TryGetValue(mtl, out curMtl) &&
							materials.TryGetValue(mtl, out m)) {
							curMtl = Materials.Count;
							mtlIdx.Add(mtl, curMtl);
							Materials.Add(m);
						}
						break;
					case "v":
						Vertices.Add(new Vector3(ToFloat(parts[0]), ToFloat(parts[1]), ToFloat(parts[2])));
						break;
					case "vn":
						Normals.Add(new Vector3(ToFloat(parts[0]), ToFloat(parts[1]), ToFloat(parts[2])));
						break;
					case "vt":
						UVs.Add(new Vector2(ToFloat(parts[0]), ToFloat(parts[1])));
						break;
					case "f":
						Faces.Add(new Face(parts.Select(x => {
								var ns = x.Split(sepSlash);
								return new FaceVert(ToIdx(ns[0], Vertices.Count), ToIdx(ns[1], UVs.Count), ToIdx(ns[2], Normals.Count));
							}).ToArray(), curMtl));
						break;
					case "s":
					case "g":
					case "o":
					case "l":
						break;
					default:
						throw new Exception("Unknown line " + line);
				}
			}
		}
	}

	public class TGAFile
	{
		public static Bitmap Read(BinaryReader r)
		{
			byte image_id_len = r.ReadByte();
			byte color_map_type = r.ReadByte();
			byte image_type = r.ReadByte();

			if (color_map_type != 0 || image_type != 10)
				return null;

			// color map spec + x / y origin
			for (int i = 0; i < 9; i++)
				r.ReadByte();

			int width = r.ReadInt16();
			int height = r.ReadInt16();
			int pixdepth = r.ReadByte();

			if (pixdepth != 24)
				return null;

			int descriptor = r.ReadByte();
			if (descriptor != 0)
				return null;

			for (int i = 0; i < image_id_len; i++)
				r.ReadByte();

			int[] img = new int[width * height];

			int total = width * height, idx = 0;
			if (image_type == 10) {
				while (idx < total) {
					int cmd = r.ReadByte();
					if ((cmd & 0x80) != 0) {
						int pixel = r.ReadUInt16() | unchecked((int)0xff000000);
						pixel |= r.ReadByte() << 16;
						int n = (cmd & 0x7f) + 1;
						while (n-- != 0)
							img[idx++] = pixel;
					} else {
						int n = cmd + 1;
						while (n-- != 0) {
							int pixel = r.ReadUInt16() | unchecked((int)0xff000000);
							pixel |= r.ReadByte() << 16;
							img[idx++] = pixel;
						}
					}
				}
			} else
				throw new Exception("Invalid image file type");

			Rectangle rect = new Rectangle(0, 0, width, height);
			PixelFormat fmt = PixelFormat.Format32bppArgb;
			Bitmap bmp = new Bitmap(width, height, fmt);
			BitmapData d = bmp.LockBits(rect, ImageLockMode.ReadWrite, fmt);
			for (int p = 0; p < total; p += width)
				Marshal.Copy(img, total - p - width, d.Scan0 + p * 4, width);
			bmp.UnlockBits(d);
			return bmp;
		}
	}
}
