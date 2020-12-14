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

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using OverloadLevelEditor;


// Establish a baseline class for editor (necessary for Level class)
// This is the most minimal IEditor interface for the Level class
public partial class EditorWrapper : OverloadLevelEditor.IEditor
{
	const string kFolderDecals = "Decals";
	const string kFolderDecalTextures = "DecalTextures";
	const string kFolderLevels = "Levels";
	const string kFolderLevelTextures = "LevelTextures";

	string m_editorRootFolder;
	Dictionary<string, DMesh> m_dmeshMap;

	public EditorWrapper( string editorRootFolder )
	{
		this.m_editorRootFolder = editorRootFolder;
		this.m_dmeshMap = new Dictionary<string, DMesh>(StringComparer.InvariantCultureIgnoreCase);
		this.ActiveEditMode = EditMode.SEGMENT;
	}

    public bool IsHeadlessProxyEditor { get { return true; } }

    #region Dummy Implementation
    public Level LoadedLevel { get { return null; } }
    public TextureManager tm_decal { get { return null; } }
    public TextureManager tm_level { get { return null; } }
    public List<TextureSet> TextureSets { get { return null; } }
    public EditMode ActiveEditMode { get; set; }
    public SideSelect ActiveSideSelect { get { return SideSelect.ALL; } }
    public PivotMode ActivePivotMode { get { return PivotMode.LOCAL; } }
    public DragMode ActiveDragMode { get { return DragMode.ALL; } }
	 public InsertDecal ActiveInsertDecal { get { return InsertDecal.ALL; } }
    public float CurrGridSnap { get { return 0.000001f; } }
    public bool IsAutoCenter { get { return false; } }
    public int CurrExtrudeLength { get { return 0; } }
    public bool ShouldInsertAdvance { get { return false; } }
    public OpenTK.Matrix4 SourceSideRotation { get { return OpenTK.Matrix4.Identity; } }
    public OpenTK.Matrix4 DestSideRotation { get { return OpenTK.Matrix4.Identity; } }
    public void AddOutputText(string text)
    {
    }
    public void EntityListUpdateEntity(Entity entity)
    {
    }
    public void EntityListRemoveEntity(Entity entity)
    {
    }
    public void EntityListSetSelectedEntity(int index)
    {
    }
    public void SetProjOffsetAllViews(OpenTK.Vector3 pos)
    {
    }
    public void SetEditModeSilent(EditMode mode, bool update_label)
    {
        this.ActiveEditMode = mode;
    }
    public OpenTK.Vector3 AlignPasteVert(OpenTK.Vector3 v)
    {
        return v;
    }
    public void RefreshGeometry(bool refresh_editor)
    {
    }
    public void Refresh()
    {
    }
    #endregion

    public DMesh GetDMeshByName(string dmeshName)
	{
		// Adjust the given dmesh name to handle subfolders
		dmeshName = dmeshName.Replace('\\', '/');

		// Has it already been loaded?
		DMesh res = null;
		if (!this.m_dmeshMap.TryGetValue(dmeshName, out res)) {
			// This is a new DMesh, get the full path to the DMesh
			string dmeshFullPath = Path.ChangeExtension(Path.Combine(Path.Combine(this.m_editorRootFolder, kFolderDecals), dmeshName), ".dmesh");
			if (!File.Exists(dmeshFullPath)) {
				Debug.LogError(string.Format("Unable to find DMesh '{0}'", dmeshName));
				this.m_dmeshMap.Add(dmeshName, null); // prevent future errors
				return null;
			}

			// Attempt to load the DMesh
			try {
				string dmeshFileData = System.IO.File.ReadAllText(dmeshFullPath);
				JObject root = JObject.Parse(dmeshFileData);

				res = new DMesh(dmeshName);
				res.Deserialize(root);
			}
			catch (Exception ex) {
				Debug.LogError(string.Format("Error loading DMesh '{0}': {1}", dmeshName, ex.Message));
				res = null;
			}

			// Update cache
			this.m_dmeshMap.Add(dmeshName, res);
		}

		return res;
	}
}
