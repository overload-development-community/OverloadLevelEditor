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

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class TextureExportMenu : ScriptableObject
{
	[MenuItem("Custom/Texture Export/Export Level Materials In Selected Directory")]
	public static void ExportSelectLevelMaterialsMenu()
	{
		foreach (Object obj in Selection.objects) {
			if (IsAssetAFolder(obj)) {
				ExportAllMaterialsInDirectory(m_last_path, true, false, true);
			}
		}
	}

	[MenuItem("Custom/Texture Export/Export Decal Materials In Selected Directory")]
	public static void ExportSelectDecalMaterialsMenu()
	{
		foreach (Object obj in Selection.objects) {
			if (IsAssetAFolder(obj)) {
				ExportAllMaterialsInDirectory(m_last_path, true, true, true);
			}
		}
	}

	private static string m_last_path = "";
	private static bool IsAssetAFolder(Object obj)
	{
		string path = "";

		if (obj == null) {
			return false;
		}

		path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

		if (path.Length > 0) {
			if (Directory.Exists(path)) {
				m_last_path = path;
            return true;
			} else {
				return false;
			}
		}

		return false;
	}

	[MenuItem("Custom/Texture Export/Export ALL Level Materials")]
	public static void ExportLevelMaterialsMenu()
	{
		ExportAllMaterialsInDirectory("Level");
	}

	[MenuItem("Custom/Texture Export/Export ALL Decal Materials")]
	public static void ExportDecalMaterialsMenu()
	{
		ExportAllMaterialsInDirectory("Decal");
	}

	[MenuItem("Custom/Texture Export/Export ALL Level Substances")]
	public static void ExportLevelSubstancesMenu()
	{
		ExportAllSubstancesInDirectory("Level");
	}

	[MenuItem("Custom/Texture Export/Export ALL Decal Substances")]
	public static void ExportDecalSubstancesMenu()
	{
		ExportAllSubstancesInDirectory("Decal");
	}
	
	public static void ExportAllMaterialsInDirectory(string dir, bool full_dir = false, bool decals = false, bool combine_normal_and_rough = false)
	{
		string unity_assets_path = Application.dataPath;
		string unity_export_texture_dir = (full_dir ? dir : Path.Combine(Path.Combine(unity_assets_path, "Textures"), dir));
		string overload_editor_root_dir = OverloadLevelConverter.FindEditorRootFolder();
		string overload_texture_export_dir = Path.Combine(overload_editor_root_dir, (full_dir ? (decals ? "DecalTextures" : "LevelTextures") : dir + "Textures"));

		int export_count = 0;
		int actual_count = 0;
		string[] files = Directory.GetFiles(unity_export_texture_dir, "*.png", SearchOption.AllDirectories);
		int total_files = files.Length;
		
		try {
			foreach (string filename in files) {
				if (filename.ToLower().Contains("basecolor") || filename.ToLower().Contains("diffuse")) {
					// Get a usable path for LoadAsset
					string unity_texture_location = Path.Combine("Assets", OverloadLevelEditor.Utility.GetRelativeFilenameFromDirectory(unity_assets_path, filename)).Replace('\\', '/');
					string[] file_split_name = unity_texture_location.Split('/');
					string file_plain_name = file_split_name[file_split_name.Length - 1].ToLower().Replace("_basecolor", "").Replace("_diffuse", "");
					string overload_export_location = (overload_texture_export_dir + "/" + file_plain_name).Replace('\\', '/');

					// Show progress so far
					float prog_perc = (float)export_count / (float)total_files;
					if (EditorUtility.DisplayCancelableProgressBar("Exporting Materials", unity_texture_location, prog_perc)) {
						Debug.LogWarning("User cancelled Material export");
						break;
					}

					// Shrink the texture
					Texture2D new_tex = new Texture2D(1, 1);
					new_tex.LoadImage(System.IO.File.ReadAllBytes(unity_texture_location));
					new_tex.Apply();
					TextureScale.Bilinear(new_tex, 512, 512);
					new_tex.Apply();

					Color32[] pixels = new_tex.GetPixels32();
					int count = pixels.Length;
					for (int i = 0; i < pixels.Length; i++) {
						pixels[i].a = 255;
					}

					if (combine_normal_and_rough) {
						string normal_texture_location = unity_texture_location.Replace("basecolor", "normal");
						Texture2D normal_tex = new Texture2D(1, 1);
						if (normal_tex.LoadImage(System.IO.File.ReadAllBytes(normal_texture_location))) {
							normal_tex.Apply();
							TextureScale.Bilinear(normal_tex, 512, 512);
							normal_tex.Apply();

							Color32[] normal_pixels = normal_tex.GetPixels32();
							for (int i = 0; i < count; i++) {
								pixels[i].r = (byte)Mathf.Clamp(pixels[i].r + (128 - normal_pixels[i].r) / 8, 0, 255);
								pixels[i].g = (byte)Mathf.Clamp(pixels[i].g + (128 - normal_pixels[i].g) / 8, 0, 255);
							}
						}

						string rough_texture_location = unity_texture_location.Replace("basecolor", "metallic");
						Texture2D rough_tex = new Texture2D(1, 1);
						if (rough_tex.LoadImage(System.IO.File.ReadAllBytes(rough_texture_location))) {
							rough_tex.Apply();
							TextureScale.Bilinear(rough_tex, 512, 512);
							rough_tex.Apply();

							Color32[] rough_pixels = rough_tex.GetPixels32();
							for (int i = 0; i < count; i++) {
								pixels[i].r = (byte)Mathf.Clamp(pixels[i].r + (rough_pixels[i].a - 128) / 4, 0, 255);
								pixels[i].g = (byte)Mathf.Clamp(pixels[i].g + (rough_pixels[i].a - 128) / 4, 0, 255);
								pixels[i].b = (byte)Mathf.Clamp(pixels[i].b + (rough_pixels[i].a - 128) / 4, 0, 255);
							}
						}
					}

					new_tex.SetPixels32(pixels);
					new_tex.Apply();

					// Write out (delete first if necessary) the texture
					if (File.Exists(overload_export_location)) {
						File.Delete(overload_export_location);
					}
					File.WriteAllBytes(overload_export_location, new_tex.EncodeToPNG());

					actual_count += 1;
				}

				export_count += 1;
			}
		}
		finally {
			EditorUtility.ClearProgressBar();
			Debug.Log("Finished exporting " + actual_count.ToString() + " diffuse textures to " + overload_texture_export_dir);
		}
	}


	enum SubstanceTextureFormat : int
	{
		Compressed = 0,
		Raw = 1,
		CompressedNoAlpha = 2,
		RawNoAlpha = 3,
	}

	struct SubstanceTextureSettings
	{
		public ProceduralMaterial mat;
		public int maxTextureWidth;
		public int maxTextureHeight;
		public int textureFormat;
		public int loadBehavior;
	}

	public static IEnumerable<KeyValuePair<string, ProceduralMaterial>> EnumerateSubstanceMaterials(string folderName)
	{
		string unityAssetsPath = Application.dataPath;
		string unityExportTextureDir = Path.Combine(Path.Combine(Path.Combine(unityAssetsPath, "Textures"), folderName), "Resources");

		foreach (var substancePath in Directory.GetFiles(unityExportTextureDir, "*.sbsar", SearchOption.AllDirectories)) {
			var substanceAsset = Path.Combine("Assets", OverloadLevelEditor.Utility.GetRelativeFilenameFromDirectory(unityAssetsPath, substancePath)).Replace('\\', '/');
			var substanceImporter = (SubstanceImporter)AssetImporter.GetAtPath(substanceAsset);
			foreach (var mat in substanceImporter.GetMaterials()) {
				yield return new KeyValuePair<string, ProceduralMaterial>(substanceAsset, mat);
			}
		}
	}

	public static void ExportAllSubstancesInDirectory(string dir)
	{
		string unity_assets_path = Application.dataPath;
		string unity_export_texture_dir = Path.Combine(Path.Combine(unity_assets_path, "Textures"), dir);
		string overload_editor_root_dir = OverloadLevelConverter.FindEditorRootFolder();
		string overload_texture_export_dir = Path.Combine(overload_editor_root_dir, dir + "Textures");

		int export_count = 0;
		string[] files = Directory.GetFiles(unity_export_texture_dir, "*.sbsar", SearchOption.AllDirectories);
		int total_files = files.Length;
		int files_processed = 0;

		try {
			foreach (string filename in files) {
				// Get a usable path for LoadAsset
				string unity_asset_location = Path.Combine("Assets", OverloadLevelEditor.Utility.GetRelativeFilenameFromDirectory(unity_assets_path, filename)).Replace('\\', '/');

				// Show progress so far
				float prog_perc = (float)files_processed / (float)total_files;
				++files_processed;
				if (EditorUtility.DisplayCancelableProgressBar("Exporting Substances", unity_asset_location, prog_perc)) {
					Debug.LogWarning("User cancelled Substance export");
					break;
				}

				SubstanceImporter sub_imp = (SubstanceImporter)AssetImporter.GetAtPath(unity_asset_location);
				int sub_imp_count = sub_imp.GetMaterialCount();
				if (sub_imp_count <= 0) {
					continue;
				}

				// In order to call GetPixels32 the material must be set to Raw format and only Raw
				// Fixup those that need to be fixed, and then restore them back.
				var materials_to_restore_settings = new Dictionary<string, SubstanceTextureSettings>();
				ProceduralMaterial[] proc_mats = sub_imp.GetMaterials();
				foreach (ProceduralMaterial proc_mat in proc_mats) {
					int maxTextureWidth;
					int maxTextureHeight;
					int textureFormat;
					int loadBehavior;
					bool res = sub_imp.GetPlatformTextureSettings(proc_mat.name, string.Empty, out maxTextureWidth, out maxTextureHeight, out textureFormat, out loadBehavior);
					if (!res) {
						Debug.LogError(string.Format("Failed to read platform settings for '{0}'", proc_mat.name));
						continue;
					}

					if (textureFormat == (int)SubstanceTextureFormat.Raw) {
						continue;
					}

					// Convert to Raw format
					materials_to_restore_settings.Add(proc_mat.name, new SubstanceTextureSettings() { mat = proc_mat, maxTextureWidth = maxTextureWidth, maxTextureHeight = maxTextureHeight, textureFormat = textureFormat, loadBehavior = loadBehavior });
					sub_imp.SetPlatformTextureSettings(proc_mat, string.Empty, maxTextureWidth, maxTextureHeight, (int)SubstanceTextureFormat.Raw, loadBehavior);
				}
				if (materials_to_restore_settings.Count != 0) {
					AssetDatabase.ImportAsset(unity_asset_location);
					sub_imp = (SubstanceImporter)AssetImporter.GetAtPath(unity_asset_location);
					proc_mats = sub_imp.GetMaterials();
				}

				try {
					foreach (ProceduralMaterial proc_mat in proc_mats) {
						proc_mat.isReadable = true;
						proc_mat.RebuildTexturesImmediately();
						if (proc_mat.isProcessing) {
							Debug.LogError(string.Format("{0} is still processing", proc_mat.name));
							continue;
						}

						Texture[] mat_texs = proc_mat.GetGeneratedTextures();
						if (mat_texs.Length <= 0) {
							continue;
						}

						string material_name = proc_mat.name;

						// Only look for the Diffuse texture
						foreach (ProceduralTexture mat_tex in mat_texs.Where(m => m.name.EndsWith("_Diffuse", System.StringComparison.InvariantCultureIgnoreCase))) {
							// Copy the texture into a Texture2D, then write that out
							Texture2D new_tex = new Texture2D(mat_tex.width, mat_tex.height);

							new_tex.anisoLevel = mat_tex.anisoLevel;
							new_tex.filterMode = mat_tex.filterMode;
							new_tex.mipMapBias = mat_tex.mipMapBias;
							new_tex.wrapMode = mat_tex.wrapMode;

							Color32[] pixels = mat_tex.GetPixels32(0, 0, mat_tex.width, mat_tex.height);
							for (int i = 0; i < pixels.Length; i++) {
								pixels[i].a = 255;
							}

							new_tex.SetPixels32(pixels);
							new_tex.Apply();

							// Scale down to 512x512 before final export
							TextureScale.Bilinear(new_tex, 512, 512);

							string new_tex_name = Path.Combine(overload_texture_export_dir, material_name + ".png");

							File.WriteAllBytes(new_tex_name, new_tex.EncodeToPNG());

							export_count += 1;
						}
					}
				}
				finally {
					if (materials_to_restore_settings.Count != 0) {
						// Restore settings
						foreach (var kvp in materials_to_restore_settings) {
							//var name = kvp.Key;
							var settings = kvp.Value;
							sub_imp.SetPlatformTextureSettings(settings.mat, string.Empty, settings.maxTextureWidth, settings.maxTextureHeight, settings.textureFormat, settings.loadBehavior);
						}
						AssetDatabase.ImportAsset(unity_asset_location);
					}
				}
			}
		}
		finally {
			EditorUtility.ClearProgressBar();
			Debug.Log("Finished exporting " + export_count.ToString() + " diffuse textures to " + overload_texture_export_dir);
		}
	}
}