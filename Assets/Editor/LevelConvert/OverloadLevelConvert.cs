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
using System;
using System.Collections.Generic;
using System.Linq;
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using OverloadLevelExport;

public partial class OverloadLevelConverter
{
	class CancelExportException : Exception
	{
		public CancelExportException()
			: base( "Conversion Canceled" )
		{
		}
	}

	class ActionDisposable : IDisposable
	{
		Func<bool> m_action;

		public ActionDisposable( Func<bool> action )
		{
			m_action = action;
		}

		public void Dispose()
		{
			if( !m_action() ) {
				throw new CancelExportException();
			}
		}
	}

	class LevelGeomHasher : OverloadLevelEditor.Level.IHasher
	{
		System.Security.Cryptography.SHA1CryptoServiceProvider m_sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();

		public string FinishHashing()
		{
			byte[] block = new byte[0];
			m_sha1.TransformFinalBlock( block, 0, block.Length );

			byte[] hashAsBytes = m_sha1.Hash;
			return Convert.ToBase64String( hashAsBytes );
		}

		public void Hash( byte[] data )
		{
			m_sha1.TransformBlock( data, 0, data.Length, data, 0 );
		}

		public void Hash( int data )
		{
			Hash( BitConverter.GetBytes( data ) );
		}

		public void Hash( float data )
		{
			Hash( BitConverter.GetBytes( data ) );
		}

		public void Hash( bool data )
		{
			Hash( BitConverter.GetBytes( data ) );
		}

		public void Hash( string data )
		{
			Hash( System.Text.UTF8Encoding.UTF8.GetBytes( data ) );
		}
	}

	#region LogCodeExecutionTime
	public static IDisposable LogCodeExecutionTime( string description )
	{
		var stopWatch = new System.Diagnostics.Stopwatch();
		stopWatch.Start();
		return new ActionDisposable( () => {
			stopWatch.Stop();
			Debug.Log( string.Format( "TIME: {0} ({1} seconds)", description, stopWatch.Elapsed.TotalSeconds ) );
			return true;
		} );
	}
	#endregion

	/// <summary>
	/// Helper function to get the path to the root of the Editor resource files
	/// </summary>
	/// <returns></returns>
	public static string FindEditorRootFolder()
	{
		string pathToAssetsFolder = Application.dataPath;
		string pathToEditorFolder = IOPath.GetFullPath(IOPath.Combine(IOPath.Combine(pathToAssetsFolder, ".."), "Editor"));
		if (IODirectory.Exists(pathToEditorFolder)) {
			return pathToEditorFolder;
		}

		// Um...
		return IODirectory.GetCurrentDirectory();
	}

	/// <summary>
	/// Helper function to ensure a given folder exists in the Unity project. It will create
	/// all necessary subfolders to make the folder exist
	/// </summary>
	/// <param name="folderPath"></param>
	public static void EnsureUnityAssetFolderExists(string folderPath)
	{
#if !OVERLOAD_LEVEL_EDITOR
        var assetFolder = Application.dataPath;

		if (IOPath.IsPathRooted(folderPath)) {
			folderPath = OverloadLevelEditor.Utility.GetRelativeFilenameFromDirectory(assetFolder, folderPath + IOPath.DirectorySeparatorChar);
		}

		// Cleanup the path
		folderPath = folderPath.Replace('\\', '/');
		while (folderPath.StartsWith("/")) {
			folderPath = folderPath.Substring(1);
		}
		if (folderPath.StartsWith("Assets/", StringComparison.InvariantCultureIgnoreCase)) {
			folderPath = folderPath.Substring("Assets/".Length);
		}
		while (folderPath.Contains("//")) {
			folderPath = folderPath.Replace("//", "/");
		}

		// folderPath is now relative to the 'Assets' folder and uses / for separators
		var allSubfolders = folderPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		var parentFolder = "";
		for (int i = 0; i < allSubfolders.Length; ++i) {
			var currSubfolder = allSubfolders[i];
			var currFolder = parentFolder + currSubfolder;
			var fullFolderPath = IOPath.Combine(assetFolder, currFolder);
			if (System.IO.Directory.Exists(fullFolderPath)) {
				// Already exists
				parentFolder = currFolder + "/";
				continue;
			}

			// Make this folder
			string parentFolderAssetName = "Assets/" + parentFolder;
			parentFolderAssetName = parentFolderAssetName.Substring(0, parentFolderAssetName.Length - 1); // Strip off the / separator
			AssetDatabase.CreateFolder(parentFolderAssetName, currSubfolder);
			parentFolder = currFolder + "/";
		}
#endif
	}

	public partial class LevelConvertState
	{
		public const string s_specialLightContainerObjectName = "_container_lights";
		public const string s_specialEntityContainerObjectName = "_container_placed_entities";
		public const string s_specialReflectionProbesContainerObjectName = "_container_reflection_probes";
		public const string s_specialLevelMeshesContainerObjectName = "_container_level_meshes";
		public const string s_specialLavaMeshesContainerObjectName = "_container_level_lava_meshes";
		public const string s_specialReflectionProbesUserContainerObjectName = "!probes";
		public const string s_specialLightsUserContainerObjectName = "!lights";
		public const string s_specialLevelTagName = "Level";
		public const string s_specialLevelChunkTagName = "LevelChunk";

		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		// Add any container objects you want to preserve during reconvert
		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		string[] protectedChildObjectNames = new string[]{
								s_specialLevelMeshesContainerObjectName,
								s_specialLavaMeshesContainerObjectName,
								s_specialLightContainerObjectName,
								s_specialReflectionProbesContainerObjectName,
								s_specialEntityContainerObjectName,
							};

		const bool debugAddPortalsToRenderGeometry = false; // Set this to true to add portal geometry to the render mesh for debug visualization purposes
		const bool flipTextureVCoordinateLevelGeom = true;  // Set this to true to flip the V (of UVs) for level geometry
		const bool flipTextureVCoordinateDecalGeom = true; // Set this to true to flip the V (of UVs) for decal geometry
		const float segmentToSegmentVisibilitySecondaryDistance = 55.0f; // Note: might want to bump deformationCodeVersion if you are changing this, or cached data will be used

		public LevelConvertState()
		{
			this.FlagUpdateGeometry = true;
			this.FlagUpdateEntities = true;
			this.FlagUpdateLights = true;
			this.FlagUpdateReflectionProbes = true;
			this.FlagSplitIntoChunks = true;
			this.FlagEnableDeformation = true;
			this.FlagForceVisibility = false;
			this.FlagForcePathDistances = false;
			this.FlagDecalGeomChecks = false;
			this.FlagDecalNormalSmooth = true;
			this.SmoothingAngleSameMaterial = 40.0f;
			this.SmoothingAngleDifferentMaterial = 20.0f;
		}

		/// <summary>
		/// True if geometry (render mesh, collision mesh, and level meta data) should be updated on an existing level scene
		/// Default: true
		/// </summary>
		public bool FlagUpdateGeometry { get; set; }

		/// <summary>
		/// True if entities should be updated in an existing scene
		/// </summary>
		public bool FlagUpdateEntities { get; set; }

		/// <summary>
		/// True if [decal] lights should be updated in an existing scene
		/// Default: true
		/// </summary>
		public bool FlagUpdateLights { get; set; }

		/// <summary>
		/// True if reflection probes should be re-created based on chunks
		/// </summary>
		public bool FlagUpdateReflectionProbes { get; set; }

		/// <summary>
		/// True if placed the render mesh should be split into smaller chunks
		/// Default: true
		/// </summary>
		public bool FlagSplitIntoChunks { get; set; }

		/// <summary>
		/// True if deformation should be done
		/// </summary>
		public bool FlagEnableDeformation { get; set; }

		/// <summary>
		/// True if visibility should be forced to update, even if geometry matches the latest version
		/// </summary>
		public bool FlagForceVisibility { get; set; }

		/// <summary>
		/// True if path distances should be forced to update, even if geometry matches the latest version
		/// </summary>
		public bool FlagForcePathDistances { get; set; }

		/// <summary>
		/// True if decal geometry should be checked for erros
		/// </summary>
		public bool FlagDecalGeomChecks { get; set; }

        /// <summary>
        /// True if decal normals should be smoothed on shared segment boundaries
        /// </summary>
        public bool FlagDecalNormalSmooth { get; set; }

        /// <summary>
        /// The vertex normal smoothing angle (degrees) to use for neighboring triangles of the same material
        /// </summary>
        public float SmoothingAngleSameMaterial { get; set; }

		/// <summary>
		/// The vertex normal smoothing angle (degrees) to use for neighboring triangles of a different material
		/// </summary>
		public float SmoothingAngleDifferentMaterial { get; set; }

#region Internal members
		static System.Globalization.TextInfo s_text_info = new System.Globalization.CultureInfo("en-US", false).TextInfo;

#endregion

		class LevelConvertStateManager
		{
			Func<string, bool> m_pushProgressStatusUpdate;
			Func<bool> m_popProgressStatusUpdate;

            public Func<string, bool> PushProgressStatusUpdate
            {
                get { return m_pushProgressStatusUpdate; }
            }

            public Func<bool> PopProgressStatusUpdate
            {
                get { return m_popProgressStatusUpdate; }
            }

			public LevelConvertStateManager( ISceneBroker scene, string pathToFile, Newtonsoft.Json.Linq.JObject overloadLevelFileData, string levelDataBaseName, string editorRootFolder,
				bool flagUpdateGeometry, bool flagUpdateEntities, bool flagSplitIntoChunks, bool flagEnableDeformation, bool flagForceVisibility, bool flagForcePathDistances, bool flagDecalGeomChecks, bool flagDecalNormalSmooth, bool flagUpdateLights, bool flagUpdateReflectionProbes, float smoothingAngleSameMaterial, float smoothingAngleDifferentMaterial,
				Func<string, bool> pushProgressStatusUpdate, Func<bool> popProgressStatusUpdate)
			{
				this.LevelDatabaseName = levelDataBaseName;
				this.PathToOverloadFile = pathToFile;
				this.EditorRootFolder = editorRootFolder;
				this.FlagUpdateGeometry = flagUpdateGeometry;
				this.FlagUpdateEntities = flagUpdateEntities;
				this.FlagSplitIntoChunks = flagSplitIntoChunks;
				this.FlagEnableDeformation = flagEnableDeformation;
				this.FlagForceVisibility = flagForceVisibility;
				this.FlagForcePathDistances = flagForcePathDistances;
				this.FlagDecalGeomChecks = flagDecalGeomChecks;
				this.FlagDecalNormalSmooth = flagDecalNormalSmooth;
				this.FlagUpdateLights = flagUpdateLights;
				this.FlagUpdateReflectionProbes = flagUpdateReflectionProbes;
				this.SmoothingAngleSameMaterial = smoothingAngleSameMaterial;
				this.SmoothingAngleDifferentMaterial = smoothingAngleDifferentMaterial;
				m_pushProgressStatusUpdate = pushProgressStatusUpdate != null ? pushProgressStatusUpdate : (_x) => { return false; };
				m_popProgressStatusUpdate = popProgressStatusUpdate != null ? popProgressStatusUpdate : () => { return false; };

				OverloadLevelConverter.EnsureUnityAssetFolderExists(this.LevelDataFolder);

                if( overloadLevelFileData != null ) {
                    const bool silent = true;
                    InitializeLevelDocDataCache( overloadLevelFileData, silent );
                }

                // Find the level object in the scene
                IGameObjectBroker sceneLevelContainerObject = scene.FindGameObject(levelDataBaseName);
				if (sceneLevelContainerObject == null) {
					// Didn't find the level container by name, look using the tag
					foreach (var go in scene.FindGameObjectsWithTag(s_specialLevelTagName)) {
						// Older levels tagged a child of the level container with the "Level" tag, while
						// later levels tagged the container itself
						sceneLevelContainerObject = (go.Transform.Parent != null) ? go.Transform.Parent.ownerGameObject : go;
						break;
					}
				}

				this.IsNewLevel = false;
				if (sceneLevelContainerObject == null) {
                    // Didn't find the level container - so make it now
                    sceneLevelContainerObject = scene.CreateRootGameObject( levelDataBaseName );
					this.IsNewLevel = true;
				}

#if !OVERLOAD_LEVEL_EDITOR
                // If the level object was a prefab instance, then we need to disconnect the prefab.
                // Level conversion no longer uses prefabs but instead works directly with the scene.
                if (PrefabUtility.GetPrefabType(sceneLevelContainerObject.InternalObject) == PrefabType.PrefabInstance) {
					PrefabUtility.DisconnectPrefabInstance(sceneLevelContainerObject.InternalObject);
				}
#endif

				this.SceneLevelContainerObject = sceneLevelContainerObject;
				this.SceneLevelContainerObject.Tag = s_specialLevelTagName;
				PrepareLevelSceneObject( scene );

#if !OVERLOAD_LEVEL_EDITOR
                //
                // Cleanup old files
                //
                {
					List<string> filesToCleanup = new List<string>();

					// We no longer generate a prefab for the level
					string levelPrefabPath = IOPath.Combine(LevelDataFolder, LevelDatabaseName + ".prefab");
					if (System.IO.File.Exists(levelPrefabPath)) {
						filesToCleanup.Add(levelPrefabPath);
					}

					// This is an old asset that we don't need anymore...
					string levelInitializerPath = IOPath.Combine(LevelDataFolder, LevelDatabaseName + "_levelinit.asset");
					if (System.IO.File.Exists(levelInitializerPath)) {
						filesToCleanup.Add(levelInitializerPath);
					}

					if (ShouldUpdateGeometry) {
						foreach (var file in IODirectory.GetFiles(LevelDataFolder, LevelDatabaseName + "_rendermesh*.asset")) {
							filesToCleanup.Add(file);
						}

						foreach (var file in IODirectory.GetFiles(LevelDataFolder, LevelDatabaseName + "_collisionmesh*.asset")) {
							filesToCleanup.Add(file);
						}
					}

					if (filesToCleanup.Count > 0) {
                        if (UnityEditor.VersionControl.Provider.enabled && UnityEditor.VersionControl.Provider.isActive) {
							// Use the VersionControl Provider to delete all of these assets
							UnityEditor.VersionControl.AssetList assetList = new UnityEditor.VersionControl.AssetList();
							assetList.AddRange(filesToCleanup.Select(path => new UnityEditor.VersionControl.Asset(path)));

							UnityEditor.VersionControl.Task deleteTask = UnityEditor.VersionControl.Provider.Delete(assetList);
							deleteTask.Wait();
						} else {
							// Fallback to just using the AssetDatabase
							foreach (var file in filesToCleanup) {
								AssetDatabase.DeleteAsset(file);
							}
						}
					}
				}
#endif // !OVERLOAD_LEVEL_EDITOR
            }

			void PrepareLevelSceneObject(ISceneBroker scene)
			{
				// Assign the LevelObjectInitializer script if it doesn't have one
				IComponentBroker levelObjectInitializer = null;
#if !OVERLOAD_LEVEL_EDITOR
				levelObjectInitializer = this.SceneLevelContainerObject.GetComponent("LevelData");
				if (levelObjectInitializer.InternalObject == null) {
					levelObjectInitializer = null;
				}
#endif
				if (levelObjectInitializer == null) {
					levelObjectInitializer = this.SceneLevelContainerObject.AddComponent("LevelData");
				}

				// Create and configure the GameObject representing the level meshes
				IGameObjectBroker levelMeshContainer = null;
				IGameObjectBroker lavaCollisionContainer = null;

				if (!IsNewLevel) {
					// Search the children for LevelObject
					int numChildren = SceneLevelContainerObject.Transform.ChildCount;
					for (int childIdx = 0; childIdx < numChildren; ++childIdx) {
						var testObject = SceneLevelContainerObject.Transform.GetChild(childIdx).ownerGameObject;

						if (testObject.Name == s_specialLevelMeshesContainerObjectName) {
							// Found it
							levelMeshContainer = testObject;

							// can't break after finding anymore because we need to find two things
						}

						if (testObject.Name == s_specialLavaMeshesContainerObjectName) {
							// Found it
							lavaCollisionContainer = testObject;

							// can't break after finding anymore because we need to find two things
						}
					}
				}

				// Save reference to the game manager prefab in LevelObjectInitializer
				scene.InitializeGameManager("_game_manager", levelObjectInitializer);

				if (levelMeshContainer == null) {
					levelMeshContainer = scene.CreateRootGameObject(s_specialLevelMeshesContainerObjectName);

					// Assign the mesh container as a child of the level object
					levelMeshContainer.Transform.Parent = SceneLevelContainerObject.Transform;
				}

				if (lavaCollisionContainer == null) {
					lavaCollisionContainer = scene.CreateRootGameObject(s_specialLavaMeshesContainerObjectName);

					// Assign the lava container as a child of the level object
					lavaCollisionContainer.Transform.Parent = SceneLevelContainerObject.Transform;
				}

				levelMeshContainer.Tag = "Untagged";// Old instances will have "Level" tag set
				SetStaticFlags(levelMeshContainer);

				lavaCollisionContainer.Tag = "Untagged";// Old instances will have "Level" tag set
				SetStaticFlags(lavaCollisionContainer);

				this.LevelMeshContainer = levelMeshContainer;
				this.LavaCollisionContainer = lavaCollisionContainer;

#if !OVERLOAD_LEVEL_EDITOR
				// Cleanup the containers
				if (!IsNewLevel && ShouldUpdateGeometry) {
					// Delete stale components from level container if it has one

					//
					// Level Container
					//

					IComponentBroker meshRenderer = levelMeshContainer.GetComponent("MeshRenderer");
					if (meshRenderer != null && meshRenderer.InternalObject != null) {
						scene.DestroyComponent(meshRenderer);
					}
					IComponentBroker meshFilter = levelMeshContainer.GetComponent("MeshFilter");
					if (meshFilter != null && meshFilter.InternalObject != null) {
						scene.DestroyComponent(meshFilter);
					}
					IComponentBroker meshCollider = levelMeshContainer.GetComponent("MeshCollider");
					if (meshCollider != null && meshCollider.InternalObject != null) {
						scene.DestroyComponent(meshCollider);
					}

					//
					// Lava Container
					//

					meshCollider = lavaCollisionContainer.GetComponent("MeshCollider");
					if (meshCollider != null && meshCollider.InternalObject != null) {
						scene.DestroyComponent(meshCollider);
					}

					//
					// Delete the current children of the level object
					//

					var childrenToRemove = new List<IGameObjectBroker>();
					for (int i = 0; i < levelMeshContainer.Transform.ChildCount; ++i) {
						childrenToRemove.Add(levelMeshContainer.Transform.GetChild(i).ownerGameObject);
					}

					for (int i = 0; i < lavaCollisionContainer.Transform.ChildCount; ++i) {
						childrenToRemove.Add(lavaCollisionContainer.Transform.GetChild(i).ownerGameObject);
					}

					foreach (var child in childrenToRemove) {
						scene.DestroyGameObject(child);
					}
				}
#endif // !OVERLOAD_LEVEL_EDITOR
			}

#region IDisposable DoProgressUpdate(string update)
			public IDisposable DoProgressUpdate(string update)
			{
				if (!m_pushProgressStatusUpdate(update)) {
					throw new CancelExportException();
				}
				return new ActionDisposable(m_popProgressStatusUpdate);
			}
#endregion

			/// <summary>
			/// Path to the .overload file being converted
			/// </summary>
			public string PathToOverloadFile
			{
				get;
				private set;
			}

			/// <summary>
			/// Path to the root of the Editor folder
			/// </summary>
			public string EditorRootFolder
			{
				get;
				private set;
			}

			/// <summary>
			/// The name of the level being converted
			/// </summary>
			public string LevelDatabaseName
			{
				get;
				private set;
			}

			/// <summary>
			/// Path on disk to where level data will be exported to
			/// </summary>
			public string LevelDataFolder
			{
				get
				{
					return "Assets/Levels/";
				}
			}

			/// <summary>
			/// The name of the level metadata asset file
			/// </summary>
			public string LevelAssetName
			{
				get
				{
					return LevelDatabaseName + "_metadata";
				}
			}

			/// <summary>
			/// Conversion flag of whether or not geometry should be updated
			/// </summary>
			public bool FlagUpdateGeometry
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag of whether or not entities should be updated
			/// </summary>
			public bool FlagUpdateEntities
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag of whether or not the level should be split into chunks
			/// </summary>
			public bool FlagSplitIntoChunks
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag of whether or not deformation should be done
			/// </summary>
			public bool FlagEnableDeformation
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag of whether or not to force visibility generation
			/// </summary>
			public bool FlagForceVisibility
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag of whether or not to force path distance calculation
			/// </summary>
			public bool FlagForcePathDistances
			{
				get;
				private set;
			}

			/// <summary>
			/// Check decal geometry for errors
			/// </summary>
			public bool FlagDecalGeomChecks
			{
				get;
				private set;
			}

            /// <summary>
            /// Smooth decal normals on shared segment boundaries
            /// </summary>
            public bool FlagDecalNormalSmooth
            {
                get;
                private set;
            }

            /// <summary>
            /// Whether or not the lights in the scene should be updated
            /// </summary>
            public bool FlagUpdateLights
			{
				get;
				private set;
			}

			/// <summary>
			/// Whether or not the reflection probes in the scene should be updated
			/// </summary>
			public bool FlagUpdateReflectionProbes
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag for smoothing angle (normal generation) across the same material
			/// </summary>
			public float SmoothingAngleSameMaterial
			{
				get;
				private set;
			}

			/// <summary>
			/// Conversion flag for smoothing angle (normal generation) across a different material
			/// </summary>
			public float SmoothingAngleDifferentMaterial
			{
				get;
				private set;
			}

			/// <summary>
			/// Returns true if level geometry should be processed and updated
			/// </summary>
			public bool ShouldUpdateGeometry
			{
				get
				{
					return IsNewLevel || FlagUpdateGeometry;
				}
			}

			/// <summary>
			/// Returns true if level entities should be processed and updated
			/// </summary>
			public bool ShouldUpdateEntities
			{
				get
				{
					return IsNewLevel || FlagUpdateEntities;
				}
			}

			/// <summary>
			/// Returns true if level lights should be processed and updated
			/// </summary>
			public bool ShouldUpdateDecalLights
			{
				get
				{
					return IsNewLevel || FlagUpdateLights;
				}
			}

			/// <summary>
			/// Returns true if reflection probes should be processed and updated
			/// </summary>
			public bool ShouldUpdateReflectionProbes
			{
				get
				{
					return IsNewLevel || FlagUpdateReflectionProbes;
				}
			}

			/// <summary>
			/// The GameObject under which all exported level data should go
			/// </summary>
			public IGameObjectBroker SceneLevelContainerObject
			{
				get;
				private set;
			}

			/// <summary>
			/// The GameObject under which all of the chunk meshes should go
			/// </summary>
			public IGameObjectBroker LevelMeshContainer
			{
				get;
				private set;
			}

			/// <summary>
			/// The GameObject under which the lava collision meshes should go
			/// </summary>
			public IGameObjectBroker LavaCollisionContainer
			{
				get;
				private set;
			}

			/// <summary>
			/// Is this a fresh conversion into the scene
			/// </summary>
			public bool IsNewLevel
			{
				get;
				private set;
			}

            private void InitializeLevelDocDataCache( Newtonsoft.Json.Linq.JObject file, bool silent )
            {
                IDisposable d;

                d = null;
                if( !silent ) {
                    d = DoProgressUpdate( "Loading Overload file" );
                }
                try {
                    m_cached_level_doc_data = new OverloadLevelEditor.Level( new EditorWrapper( this.EditorRootFolder ) );
                    m_cached_level_doc_data.Deserialize( file );
                    m_cached_level_doc_data.CompactLevelSegments();
                    m_cached_level_doc_data.RefreshAllGMeshes( FlagDecalGeomChecks );
                } finally {
                    if( !silent ) {
                        d.Dispose();
                    }
                    d = null;
                }

                if( !silent ) {
                    d = DoProgressUpdate( "Calculating chunks" );
                }
                try {
                    if( FlagSplitIntoChunks ) {
                        OverloadLevelEditor.Chunking.DetermineLevelChunking( m_cached_level_doc_data );
                    } else {
                        // Assign all of the segments to a single chunk
                        foreach( OverloadLevelEditor.Segment seg in m_cached_level_doc_data.EnumerateAliveSegments() ) {
                            seg.m_chunk_num = 0;
                        }
                        m_cached_level_doc_data.m_num_chunks = 1;
                    }
                }finally {
                    if( !silent ) {
                        d.Dispose();
                    }
                }
            }

            /// <summary>
            /// Access the loaded Overload file
            /// </summary>
            public OverloadLevelEditor.Level OverloadLevelData
			{
				get
				{
					if (m_cached_level_doc_data == null) {
                        const bool silent = false;
                        InitializeLevelDocDataCache( Newtonsoft.Json.Linq.JObject.Parse( System.IO.File.ReadAllText( this.PathToOverloadFile ) ), silent );
                    }
					return m_cached_level_doc_data;
				}
			}
			OverloadLevelEditor.Level m_cached_level_doc_data = null;

			/// <summary>
			/// Returns a hash of the level's segment and decal geometry (no deformation applied, but, with deformation params)
			/// </summary>
			public string OverloadLevelGeometryHash
			{
				get {
					if( m_cached_level_geometry_hash != null ) {
						return m_cached_level_geometry_hash;
					}

					using( DoProgressUpdate( "Generating Overload file geometry hash" ) ) {
						const bool considerNonGeometry = false;
						var hasher = new LevelGeomHasher();
						OverloadLevelData.HashGeometry( hasher, considerNonGeometry );
						hasher.Hash( deformationCodeVersion );
						m_cached_level_geometry_hash = hasher.FinishHashing();
					}

					return m_cached_level_geometry_hash;
				}
			}
			string m_cached_level_geometry_hash = null;
			const int deformationCodeVersion = 2; // If the deformation logic is changed and you want to reflect that in the hash, bump this.

			public string OverloadLevelRobotSpawnPointsHash
			{
				get
				{
					if (m_cached_level_robot_spawn_points_hash != null) {
						return m_cached_level_robot_spawn_points_hash;
					}

					using (DoProgressUpdate("Generating Overload file robot spawn points hash")) {
						var hasher = new LevelGeomHasher();
						OverloadLevelData.HashRobotSpawnPoints(hasher);
						m_cached_level_robot_spawn_points_hash = hasher.FinishHashing();
					}

					return m_cached_level_robot_spawn_points_hash;
				}
			}
			string m_cached_level_robot_spawn_points_hash = null;

			/// <summary>
			/// Get the lights defined by decals from the Overload scene
			/// </summary>
			public List<DecalLightInfo> OverloadLevelDecalLights
			{
				get
				{
					if (m_cachedSegmentDecalLights == null) {
						m_cachedSegmentDecalLights = new List<DecalLightInfo>();

						var overloadLevel = this.OverloadLevelData;
						foreach (var segmentIndex in overloadLevel.EnumerateAliveSegmentIndices()) {
							var segmentData = overloadLevel.segment[segmentIndex];

							for (int sideIdx = 0; sideIdx < 6; ++sideIdx) {
								var side = segmentData.side[sideIdx];

								foreach (var currDecal in side.decal) {
									if (currDecal == null || string.IsNullOrEmpty(currDecal.mesh_name) || currDecal.gmesh == null) {
										continue;
									}

									var decalMesh = currDecal.gmesh;

									// Process decal lights
									foreach (var dl in decalMesh.m_light) {
										var light = new DecalLightInfo(dl, segmentIndex);
										if (currDecal.dmesh.color != null && dl.m_color_index >= 0 && dl.m_color_index < currDecal.dmesh.color.Count) {
											light.Color = currDecal.dmesh.color[dl.m_color_index].ToUnity();
										}
										m_cachedSegmentDecalLights.Add(light);
									}
								}
							}
						}
					}

					return m_cachedSegmentDecalLights;
				}
			}

			List<DecalLightInfo> m_cachedSegmentDecalLights = null;

			/// <summary>
			/// Access the structure builder which is used to define, connect, and specify
			/// the "structure" of the level. The segments, the portals, and their links.
			/// </summary>
			public StructureBuilder LevelStructureBuilder
			{
				get
				{
					if (m_level_structure_builder == null) {
						ConstructGeometry();
					}
					return m_level_structure_builder;
				}
			}

			/// <summary>
			/// Collision mesh MeshBuilder objects for each chunk
			/// </summary>
			public MeshBuilder[] BuilderCollisionsByChunk
			{
				get
				{
					if (m_builder_collisions == null) {
						ConstructGeometry();
					}
					return m_builder_collisions;
				}
			}

			/// <summary>
			/// Lava collision mesh MeshBuilder objects for each chunk
			/// </summary>
			public MeshBuilder[] BuilderLavaCollisionsByChunk
			{
				get
				{
					if (m_builder_lava_collisions == null) {
						ConstructGeometry();
					}
					return m_builder_lava_collisions;
				}
			}

			/// <summary>
			/// Render mesh MeshBuilder objects for each chunk
			/// </summary>
			public MeshBuilder[] BuilderGeometryByChunk
			{
				get
				{
					if (m_builder_chunks == null) {
						ConstructGeometry();
					}
					return m_builder_chunks;
				}
			}

			/// <summary>
			/// The textures referenced during level conversion
			/// </summary>
			public List<SubmeshKey> TextureList
			{
				get
				{
					if (m_texture_list == null) {
						ConstructGeometry();
					}
					return m_texture_list;
				}
			}

            /// <summary>
            /// A map from a segment and side index for a portal to a list of all the included triangles.
            /// </summary>
            public Dictionary<PortalSideKey, List<Vector3[]>> PortalTriangles {
                get {
                    if (m_portal_triangles == null) {
                        ConstructGeometry();
                    }
                    return m_portal_triangles;
                }
            }

            /// <summary>
            /// A map (for each chunk), to map from a material to the submesh index
            /// </summary>
            public Dictionary<SubmeshKey, int>[] MaterialToSubmeshIndexByChunk
			{
				get
				{
					if (m_material_to_submesh_index == null) {
						ConstructGeometry();
					}
					return m_material_to_submesh_index;
				}
			}

			/// <summary>
			/// For any chunk index, get the list of segments in that chunk
			/// </summary>
			public Dictionary<int, List<int>> ChunkToLevelSegmentIndices
            {
                get {
                    if( m_chunk_to_level_docsegment_indices == null ) {
                        if( ShouldUpdateGeometry ) {
                            ConstructGeometry();
                        } else {
                            m_chunk_to_level_docsegment_indices = new Dictionary<int, List<int>>();
                            foreach( var segmentIndex in OverloadLevelData.EnumerateAliveSegmentIndices() ) {
                                var segment_base_data = OverloadLevelData.segment[segmentIndex];
                                int segment_chunk_index = segment_base_data.m_chunk_num;

                                // Update the chunk index -> segment index list
                                List<int> segmentListForChunk;
                                if( !m_chunk_to_level_docsegment_indices.TryGetValue( segment_chunk_index, out segmentListForChunk ) ) {
                                    segmentListForChunk = new List<int>();
                                    m_chunk_to_level_docsegment_indices.Add( segment_chunk_index, segmentListForChunk );
                                }
                                segmentListForChunk.Add( segmentIndex );

                            }
                        }
                    }
                    return m_chunk_to_level_docsegment_indices;
                }
            }

			StructureBuilder m_level_structure_builder;
			MeshBuilder[] m_builder_collisions;    //one for each chunk
			MeshBuilder[] m_builder_lava_collisions;    // one for each chunk, even though a lot of these might be empty (no lava in that chunk)
			MeshBuilder[] m_builder_chunks;     //one for each chunk
			List<SubmeshKey> m_texture_list;
			Dictionary<SubmeshKey, int>[] m_material_to_submesh_index;     //One for each chunk
			Dictionary<int, List<int>> m_chunk_to_level_docsegment_indices;//Map chunk index to level doc segment indices
            Dictionary<PortalSideKey, List<Vector3[]>> m_portal_triangles;

			void ConstructGeometry()
			{
				if (!ShouldUpdateGeometry) {
					throw new Exception("Internal Error: Geometry needs to be generated though the user requested it not to be");
				}

				using (DoProgressUpdate("Level construction")) {
					// First allocate all of the data (this prevents this function from being called again)
					m_level_structure_builder = new StructureBuilder(OverloadLevelData, EditorSegmentIndexToPackedSegmentIndex);
					m_builder_collisions = new MeshBuilder[OverloadLevelData.m_num_chunks];
					m_builder_lava_collisions = new MeshBuilder[OverloadLevelData.m_num_chunks];
					m_builder_chunks = new MeshBuilder[OverloadLevelData.m_num_chunks];
					m_texture_list = new List<SubmeshKey>();
					m_material_to_submesh_index = new Dictionary<SubmeshKey, int>[OverloadLevelData.m_num_chunks];
					m_chunk_to_level_docsegment_indices = new Dictionary<int, List<int>>();// Construct a map of chunk -> segment[] for later use
                    m_portal_triangles = new Dictionary<PortalSideKey, List<Vector3[]>>();

					// Generate the data
					LevelConvertState.ConstructGeometryInternal(this);
				}
			}

			/// <summary>
			/// Access the meta data object, which is stored out as an asset file - generic level asset data
			/// </summary>
			public LevelGeometry MetaData
			{
				get
				{
					if (m_levelStructureMetaData == null) {
#if !OVERLOAD_LEVEL_EDITOR
						// Look for existing level metadata, we might reuse some/all of it
						IComponentBroker levelObjectInitizer = SceneLevelContainerObject.GetComponent("LevelData");
						if (levelObjectInitizer != null && levelObjectInitizer.InternalObject != null) {
							m_levelStructureMetaData = ((LevelData)levelObjectInitizer.InternalObject).m_geometry;
						} else if (!ShouldUpdateGeometry) {
							throw new Exception("The scene is missing the LevelMetaData component on the level container - you must update geometry");
						}
#else
						if(!ShouldUpdateGeometry) {
							throw new Exception("The scene is missing the LevelMetaData component on the level container - you must update geometry");
						}
#endif

						if( ShouldUpdateGeometry) {
							// If we are updating geometry then we'll use the LevelStructureBuilder to make the meta data
							// Note: We use a hash of the geometry to see if we need to re-run visibility or can just use
							// the cached data.
							string currGeometryHash = OverloadLevelGeometryHash;
							string currRobotSpawnPointsHash = OverloadLevelRobotSpawnPointsHash;
							int[] cachedSegmentToSegmentVisibility = null;
							PathDistanceData[] cachedPathDisances = null;
							if (m_levelStructureMetaData != null) {
								if (m_levelStructureMetaData.GeometryHash == currGeometryHash) {
									// The geometry looks to be the same, we can copy over some data (all??)
									// TODO: Bring over all the data we can from the old m_levelStructureMetaData to the new to save work
									Debug.Log("Overload file level data looks to be the same");
									cachedSegmentToSegmentVisibility = m_levelStructureMetaData.SegmentToSegmentVisibility;
									if (m_levelStructureMetaData.RobotSpawnPointsHash == currRobotSpawnPointsHash) {
										cachedPathDisances = m_levelStructureMetaData.PathDistances;
									}
								} else {
									Debug.Log("Overload file level data looks to be different");
								}
							}

							using (DoProgressUpdate("Generating level meta-data")) {
								StructureBuilder.BuildFlags buildFlags = 0;

								m_levelStructureMetaData = this.LevelStructureBuilder.Build(DoProgressUpdate, buildFlags, PortalTriangles);
								m_levelStructureMetaData.name = LevelAssetName;
								m_levelStructureMetaData.FileName = IOPath.GetFileNameWithoutExtension(this.PathToOverloadFile);
								m_levelStructureMetaData.GeometryHash = currGeometryHash;
								m_levelStructureMetaData.RobotSpawnPointsHash = currRobotSpawnPointsHash;
								m_levelStructureMetaData.SegmentToSegmentVisibility = cachedSegmentToSegmentVisibility;
								m_levelStructureMetaData.PathDistances = cachedPathDisances;
							}
						}
					}
					return m_levelStructureMetaData;
				}
			}
			LevelGeometry m_levelStructureMetaData;

			/// <summary>
			/// Access the map to map from an Overload material to a Unity material
			/// </summary>
			public Dictionary<SubmeshKey, Material> TextureToUnityMaterialMap
			{
				get
				{
					if (m_texture_to_material == null) {
						// Create Substance material lookup maps - they have priority when resolving
						using (DoProgressUpdate("Resolving Substance assets")) {
#if !OVERLOAD_LEVEL_EDITOR
                            var substance_level_lookup_map = TextureExportMenu.EnumerateSubstanceMaterials("Level").SafeConvertToDictionary(item => item.Value.name, item => item.Value, item => item.Key);
							var substance_decal_lookup_map = TextureExportMenu.EnumerateSubstanceMaterials("Decal").SafeConvertToDictionary(item => item.Value.name, item => item.Value, item => item.Key);
							if (substance_level_lookup_map == null || substance_decal_lookup_map == null) {
								throw new Exception("Error building the Substance Materials map; see the log for details.");
							}
#endif

                            using (DoProgressUpdate("Binding materials")) {
#if OVERLOAD_LEVEL_EDITOR
                                m_texture_to_material = LevelConvertState.BuildMaterialsDictionaryFromTextureList(TextureList);
#else
                                m_texture_to_material = LevelConvertState.BuildMaterialsDictionaryFromTextureList(TextureList, substance_level_lookup_map, substance_decal_lookup_map);
#endif
                            }
						}
					}
					return m_texture_to_material;
				}
			}
			Dictionary<SubmeshKey, Material> m_texture_to_material;        //Global list of textures, mapped to materials

            public struct SubmeshObject
            {
                public int m_original_submesh_index;
                public Mesh m_mesh_object;

                public SubmeshObject(int submesh_index, Mesh mesh_object) { m_original_submesh_index = submesh_index; m_mesh_object = mesh_object; }
            }

            public List<SubmeshObject>[] UnityRenderMeshByChunk
			{
				get
				{
					if (renderMeshObject == null) {
						using (DoProgressUpdate("Generating Unity render mesh per chunk")) {
							renderMeshObject = GenerateMeshObject(BuilderGeometryByChunk, MeshBuilder.GenerateMode.RenderMesh, LevelDatabaseName + "__RenderMesh");
						}
					}
					return renderMeshObject;
				}
			}
			List<SubmeshObject>[] renderMeshObject = null;

			public List<SubmeshObject>[] UnityCollisionMeshByChunk
			{
				get
				{
					if (collisionMeshObject == null) {
						using (DoProgressUpdate("Generating Unity collision mesh per chunk")) {
							collisionMeshObject = GenerateMeshObject(BuilderCollisionsByChunk, MeshBuilder.GenerateMode.CollisionMesh, LevelDatabaseName + "__CollisionMesh");
						}
					}
					return collisionMeshObject;
				}
			}
			List<SubmeshObject>[] collisionMeshObject = null;

			public List<SubmeshObject>[] UnityLavaCollisionMeshByChunk
			{
				get
				{
					if (lavaCollisionMeshObject == null) {
						using (DoProgressUpdate("Generating Unity lava collision mesh per chunk")) {
							lavaCollisionMeshObject = GenerateMeshObject(BuilderLavaCollisionsByChunk, MeshBuilder.GenerateMode.CollisionMesh, LevelDatabaseName + "__LavaCollisionMesh");
						}
					}
					return lavaCollisionMeshObject;
				}
			}
			List<SubmeshObject>[] lavaCollisionMeshObject = null;

			/// <summary>
			/// Access a mapping for editor segments to packed segments
			/// NOTE: Levels should be packed now, meaning this mapping should be 1:1 now
			/// </summary>
			public Dictionary<int, int> EditorSegmentIndexToPackedSegmentIndex
			{
				get
				{
					if (m_cachedEditorSegmentIndexToPackedSegmentIndex == null) {
						m_cachedEditorSegmentIndexToPackedSegmentIndex = new Dictionary<int, int>();

						int packedIndex = 0;
						for (int editorSegmentIndex = 0; editorSegmentIndex < OverloadLevelEditor.Level.MAX_SEGMENTS; ++editorSegmentIndex) {
							if (!OverloadLevelData.segment[editorSegmentIndex].Alive) {
								continue;
							}

							if (packedIndex != editorSegmentIndex) {
								Debug.LogWarning(string.Format("Editor segment {0} mapped to game segment {1} -- THIS IS NOT EXPECTED", editorSegmentIndex, packedIndex));
							}
							m_cachedEditorSegmentIndexToPackedSegmentIndex.Add(editorSegmentIndex, packedIndex);
							packedIndex++;
						}
					}
					return m_cachedEditorSegmentIndexToPackedSegmentIndex;
				}
			}
			Dictionary<int, int> m_cachedEditorSegmentIndexToPackedSegmentIndex = null;

			/// <summary>
			/// Access a mapping of door numbers (from the level) to which portal is associated for the door
			/// </summary>
			public Dictionary<int, int> DooNumToPortalMap
			{
				get
				{
					if (m_cachedDoorNumToPortalMap == null) {
						m_cachedDoorNumToPortalMap = BuildDoorNumToPortalMap(this.OverloadLevelData, this.MetaData);
					}
					return m_cachedDoorNumToPortalMap;
				}
			}
			Dictionary<int, int> m_cachedDoorNumToPortalMap = null;

			List<SubmeshObject>[] GenerateMeshObject(MeshBuilder[] builders, MeshBuilder.GenerateMode mode, string name_prefix)
			{
				int num_builders = builders.Length;

				List<Mesh>[] meshes_per_chunk = new List<Mesh>[num_builders];
                List<int>[] submesh_indices_per_chunk = new List<int>[num_builders];
                List<SubmeshObject>[] combined_submesh_entries = new List<SubmeshObject>[num_builders];

                for (int i = 0; i < num_builders; i++) {
					meshes_per_chunk[i] = new List<Mesh>();
                    submesh_indices_per_chunk[i] = new List<int>();
                    combined_submesh_entries[i] = new List<SubmeshObject>();

					if (builders[i] == null) {
						continue;
					}

					builders[i].GenerateMeshObject(mode, SmoothingAngleSameMaterial, SmoothingAngleDifferentMaterial, meshes_per_chunk[i], submesh_indices_per_chunk[i]);
                    Assert.True(meshes_per_chunk[i].Count == submesh_indices_per_chunk[i].Count);

					int num_submeshes_for_chunk = meshes_per_chunk[i].Count;
					for (int sm_idx = 0; sm_idx < num_submeshes_for_chunk; ++sm_idx) {
						meshes_per_chunk[i][sm_idx].name = name_prefix + i.ToString("D4") + "_" + submesh_indices_per_chunk[i][sm_idx].ToString("D3");
                        combined_submesh_entries[i].Add(new SubmeshObject(submesh_indices_per_chunk[i][sm_idx], meshes_per_chunk[i][sm_idx]));

                    }
                }

				return combined_submesh_entries;
			}

			public string SaveLevelAssetFile(ISceneBroker scene)
			{
				using (DoProgressUpdate("Saving level metadata asset")) {
					string levelMetaDataBaseName = LevelAssetName;
					string levelMetaDataPath = IOPath.Combine(LevelDataFolder, levelMetaDataBaseName + ".asset");

					scene.AssetDatabase_CreateAsset(MetaData, levelMetaDataPath);

					// Level Render Mesh asset
					using (DoProgressUpdate("Saving render mesh assets")) {
						SaveMeshAssets(scene, UnityRenderMeshByChunk, MetaData);
					}

					// Level Collision Mesh asset
					using (DoProgressUpdate("Saving collision mesh assets")) {
						SaveMeshAssets(scene, UnityCollisionMeshByChunk, MetaData);
					}

					// Level Lava Collision Mesh asset
					using (DoProgressUpdate("Saving lava collision mesh assets")) {
						SaveMeshAssets(scene, UnityLavaCollisionMeshByChunk, MetaData);
					}

					return levelMetaDataPath;
				}
			}

			static Dictionary<int, int> BuildDoorNumToPortalMap(OverloadLevelEditor.Level levelData, LevelGeometry metaData)
			{
				Dictionary<int, int> doorNumToPortalIndex = new Dictionary<int, int>();

				// Find the doors
				foreach (int segmentIndex in levelData.EnumerateAliveSegmentIndices()) {

					OverloadLevelEditor.Segment levelSegmentData = levelData.segment[segmentIndex];

					// Run through the sides in this segment
					int numSides = levelSegmentData.neighbor.Length;
					System.Diagnostics.Debug.Assert(numSides == 6);

					for (int sideIdx = 0; sideIdx < numSides; ++sideIdx) {
						if (levelSegmentData.neighbor[sideIdx] == -1) {
							continue;
						}

						int portalIndex = metaData.Segments[segmentIndex].Portals[sideIdx];
						System.Diagnostics.Debug.Assert(portalIndex != -1);
						PortalData portalData = metaData.Portals[portalIndex];

						// We only want to process portals once - so only if we are on the master side
						if (portalData.MasterSegmentIndex != segmentIndex) {
							continue;
						}

						int doorNumMasterSide = levelData.segment[portalData.MasterSegmentIndex].side[portalData.MasterSideIndex].Door;
						int doorNumSlaveSide = levelData.segment[portalData.SlaveSegmentIndex].side[portalData.SlaveSideIndex].Door;
						if (doorNumMasterSide != -1) {
							if (doorNumSlaveSide != -1) {
								throw new Exception("Both sides of a portal have doors (segments " + portalData.MasterSegmentIndex + " and " + portalData.SlaveSegmentIndex + ").");
							}
							doorNumToPortalIndex.Add(doorNumMasterSide, portalIndex);
						} else if (doorNumSlaveSide != -1) {
							doorNumToPortalIndex.Add(doorNumSlaveSide, portalIndex);
						}
					}
				}

				return doorNumToPortalIndex;
			}
		}

#region Scene Preparation and Initialization

		static LevelConvertStateManager InitializeAndPrepareLevelConversion(ISceneBroker scene, string pathToFile, Newtonsoft.Json.Linq.JObject overloadLevelFileData, string levelDataBaseName, string editorRootFolder,
			bool FlagUpdateGeometry, bool FlagUpdateEntities, bool FlagSplitIntoChunks, bool FlagEnableDeformation, bool FlagForceVisibility, bool FlagForcePathDistances, bool FlagDecalGeomChecks, bool FlagDecalNormalSmooth, bool FlagUpdateLights, bool FlagUpdateReflectionProbes, float SmoothingAngleSameMaterial, float SmoothingAngleDifferentMaterial,
			Func<string, bool> pushProgressStatusUpdate, Func<bool> popProgressStatusUpdate)
		{
#if UNITY_EDITOR
            UnityEngine.SceneManagement.Scene active_scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
			if (string.IsNullOrEmpty(active_scene.path)) {
				// New scenes in Unity have two default objects, remove them as we'll have our own
				if (active_scene.rootCount == 2) {
					GameObject main_camera = GameObject.Find("Main Camera");
					GameObject directional_light = GameObject.Find("Directional Light");
					if (main_camera && directional_light) {
						Component[] camera_components = main_camera.GetComponents<Component>();
						Component[] dirlight_components = directional_light.GetComponents<Component>();

						if (camera_components.Length == 5 && dirlight_components.Length == 2) {

							bool is_camera = camera_components.Where(comp => comp.GetType() == typeof(Camera)).Count() == 1;
							bool is_light = dirlight_components.Where(comp => comp.GetType() == typeof(Light)).Count() == 1;

							if (is_camera && is_light) {
								// Remove the two default objects from the scene
								GameObject.DestroyImmediate(main_camera);
								GameObject.DestroyImmediate(directional_light);
							}
						}
					}
				}
			}
#endif
			return new LevelConvertStateManager(scene, pathToFile, overloadLevelFileData, levelDataBaseName, editorRootFolder,
				FlagUpdateGeometry, FlagUpdateEntities, FlagSplitIntoChunks, FlagEnableDeformation, FlagForceVisibility, FlagForcePathDistances, FlagDecalGeomChecks, FlagDecalNormalSmooth, FlagUpdateLights, FlagUpdateReflectionProbes, SmoothingAngleSameMaterial, SmoothingAngleDifferentMaterial,
				pushProgressStatusUpdate, popProgressStatusUpdate);
		}

#endregion

		public void ConvertLevel(string pathToFile, string levelDataBaseName, string editorRootFolder, ISceneBroker scene, Newtonsoft.Json.Linq.JObject overloadLevelFileData )
		{
#if UNITY_EDITOR
            var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
			var pb = new ProgressBarManager(string.Format("Converting and Adding Level to Scene ({0})", sceneName));
			bool sceneAssetUpdated = false;
			bool sceneUpdated = false;
#endif
            bool isChallengeModeLevel = scene.LevelExportType == LevelType.ChallengeMode;

            try {

				//
				// Ready the Unity scene for level conversion, prepare the file system, and find (or create) the
				// level container object in the scene.
				//
				int numProgressPhases = 0;
				int progressPhaseDepth = 0;
				string progressInfoPhaseText = string.Empty;
				Stack<string> progressInfoStack = new Stack<string>();
				Stack<System.Diagnostics.Stopwatch> progressInfoStopwatchStack = new Stack<System.Diagnostics.Stopwatch>();
				Dictionary<int, List<string>> timingsByDepth = new Dictionary<int, List<string>>();

				LevelConvertStateManager convertState = InitializeAndPrepareLevelConversion(scene, pathToFile, overloadLevelFileData, levelDataBaseName, editorRootFolder,
				FlagUpdateGeometry, FlagUpdateEntities, FlagSplitIntoChunks, FlagEnableDeformation, FlagForceVisibility, FlagForcePathDistances, FlagDecalGeomChecks, FlagDecalNormalSmooth, FlagUpdateLights, FlagUpdateReflectionProbes, SmoothingAngleSameMaterial, SmoothingAngleDifferentMaterial,

				// Push progress update
				(string progress) => {
					if (numProgressPhases == 0) {
						// Not initialized / nothing to do
						return false;
					}

#if !OVERLOAD_LEVEL_EDITOR
                    bool continueProcess = false;
					if (progressPhaseDepth == 0) {
						// Starting a root level phase, tick the progress bar
						continueProcess = pb.Tick(progress, true);
						progressInfoPhaseText = progress;
					} else {
						// Nested phase - just update some text
						string fullText = string.Format("{0}: {1}", progressInfoPhaseText, progress);
						continueProcess = pb.NonTick(fullText, true);
					}
#else
                    bool continueProcess = false;
#endif

                    var stopWatch = new System.Diagnostics.Stopwatch();
					stopWatch.Start();
					progressInfoStack.Push(progress);
					progressInfoStopwatchStack.Push( stopWatch );
					++progressPhaseDepth;

					return !continueProcess;
				},

				// Pop progress update
				() => {
					if (numProgressPhases == 0 || progressPhaseDepth == 0) {
						// Not initialized / nothing to do
						progressInfoPhaseText = string.Empty;
#if UNITY_EDITOR
                        return !pb.NonTick("Converting...", true);
#else
                        return false;
#endif
                    }

					var title = progressInfoStack.Pop();
					var sw = progressInfoStopwatchStack.Pop();
					sw.Stop();
					string performanceInfo = string.Format( "{2}-{0} [ {1} seconds ]", title, sw.Elapsed.TotalSeconds, new string( '|', progressPhaseDepth ) );

					List<string> timingsAtDepth;
					if( !timingsByDepth.TryGetValue( progressPhaseDepth, out timingsAtDepth ) ) {
						timingsAtDepth = new List<string>();
						timingsByDepth.Add( progressPhaseDepth, timingsAtDepth );
					}
					timingsAtDepth.Add( performanceInfo );
					// Consume all the text at deeper depth, folding it into this depth
					List<string> timingsAtHigherDepth;
					if( timingsByDepth.TryGetValue( progressPhaseDepth + 1, out timingsAtHigherDepth ) ) {
						timingsAtDepth.AddRange( timingsAtHigherDepth );
						timingsAtHigherDepth.Clear();
					}
					--progressPhaseDepth;

					string fullText;
					if (progressPhaseDepth == 0) {
						fullText = "Converting...";
					} else if (progressPhaseDepth == 1) {
						fullText = progressInfoPhaseText;
					} else {
						string progressInfo = progressInfoStack.Peek();
						fullText = string.Format("{0}: {1}", progressInfoPhaseText, progressInfo);
					}

#if UNITY_EDITOR
                    return !pb.NonTick(fullText, true);
#else
                    return true;
#endif
                });

#region Calculate progress bar phases
				if (convertState.ShouldUpdateGeometry) {
					// Construct scene geometry
					numProgressPhases++;
                    numProgressPhases++;
                }
				if (convertState.ShouldUpdateDecalLights) {
					// Place lights
					numProgressPhases++;
				}
				if (convertState.ShouldUpdateReflectionProbes) {
					// Place reflection probes
					numProgressPhases++;
				}
				if (convertState.ShouldUpdateEntities) {
					// Place entities
					numProgressPhases++;
				}
                numProgressPhases += 2; //save & reconcile
#if UNITY_EDITOR
                pb.BeginPhase("Level conversion", numProgressPhases);
#endif
#endregion

				// Construct scene geometry
#region Scene geometry construction
				if (convertState.ShouldUpdateGeometry) {
					string levelMetaDataPath = null;

					using (convertState.DoProgressUpdate("Building scene geometry")) {

						// Start editing assets
						scene.AssetDatabase_StartAssetEditing();
						try {
							// The level asset file must be saved first before we build the Unity scene
							levelMetaDataPath = convertState.SaveLevelAssetFile(scene);

							using (convertState.DoProgressUpdate("Create scene geometry objects")) {
#if UNITY_EDITOR
								sceneUpdated = true;
#endif
								IGameObjectBroker levelMeshContainer = convertState.LevelMeshContainer;
								IGameObjectBroker lavaCollisionContainer = convertState.LavaCollisionContainer;

								// Create children for each render/collision mesh
								for (int chunkIdx = 0, numChunks = convertState.UnityRenderMeshByChunk.Length; chunkIdx < numChunks; ++chunkIdx) {
									List<LevelConvertStateManager.SubmeshObject> renderSubmeshMeshesForChunk = convertState.UnityRenderMeshByChunk[chunkIdx];
									List<LevelConvertStateManager.SubmeshObject> collisionSubmeshMeshesForChunk = convertState.UnityCollisionMeshByChunk[chunkIdx];
									List<LevelConvertStateManager.SubmeshObject> lavaCollisionSubmeshMeshesForChunk = convertState.UnityLavaCollisionMeshByChunk[chunkIdx];
									Assert.True(collisionSubmeshMeshesForChunk.Count <= 1);
									Assert.True(lavaCollisionSubmeshMeshesForChunk.Count <= 1);

									int numRenderSubmeshes = renderSubmeshMeshesForChunk.Count;

									IGameObjectBroker collisionMeshObject;
									IGameObjectBroker lavaCollisionMeshObject = null;
									List<IGameObjectBroker> renderMeshObjects = new List<IGameObjectBroker>();

									if (FlagSplitIntoChunks) {
										// Create a child GameObject for the collision chunk (and the lava collision -- if there is one)
										// Create a child GameObject for each render submesh (each one being a texture for the chunk)

										string baseMeshName = "Chunk" + chunkIdx.ToString("D4");
										IGameObjectBroker chunkGO = scene.CreateRootGameObject(baseMeshName);
										chunkGO.Transform.Parent = levelMeshContainer.Transform;
										collisionMeshObject = chunkGO;

										if (lavaCollisionSubmeshMeshesForChunk.Count > 0) {
											string baseLavaMeshName = "ChunkLava" + chunkIdx.ToString("D4");
											IGameObjectBroker lavaChunkGO = scene.CreateRootGameObject(baseLavaMeshName);
											lavaChunkGO.Transform.Parent = lavaCollisionContainer.Transform;
											lavaCollisionMeshObject = lavaChunkGO;
										}

										if (numRenderSubmeshes <= 1) {
											// Only one rendermesh, share the game object
											renderMeshObjects.Add(chunkGO);
										} else {
											for (int smIdx = 0; smIdx < numRenderSubmeshes; ++smIdx) {
												string childName = string.Format("sm_{0}_{1}", chunkIdx.ToString("D4"), renderSubmeshMeshesForChunk[smIdx].m_original_submesh_index.ToString("D3"));
												IGameObjectBroker smMesh = scene.CreateRootGameObject(childName);
												smMesh.Transform.Parent = chunkGO.Transform;
												renderMeshObjects.Add(smMesh);
											}
										}
									} else {
										// Not using chunks. Put the collision data on the level container, and if
										// there is only one mesh, put it on the level container, otherwise, we need
										// to make child game objects
										collisionMeshObject = levelMeshContainer;
										if (lavaCollisionSubmeshMeshesForChunk.Count > 0) {
											lavaCollisionMeshObject = lavaCollisionContainer;
										}
										if (numRenderSubmeshes <= 1) {
											renderMeshObjects.Add(levelMeshContainer);
										} else {
											// Need multiple render meshes
											for (int smIdx = 0; smIdx < numRenderSubmeshes; ++smIdx) {
												string childName = string.Format("sm_{0}_{1}", chunkIdx.ToString("D4"), renderSubmeshMeshesForChunk[smIdx].m_original_submesh_index.ToString("D3"));
												IGameObjectBroker smMesh = scene.CreateRootGameObject(childName);
												smMesh.Transform.Parent = levelMeshContainer.Transform;
												renderMeshObjects.Add(smMesh);
											}
										}
									}

									Action<IGameObjectBroker, Overload.UnityObjectLayers, bool> setCommonLevelGameObjectFlags = (IGameObjectBroker meshObject, Overload.UnityObjectLayers layer, bool isRenderSubmesh) => {
										// Mark the mesh GameObject as static so it can be a part of optimizations
										SetStaticFlags(meshObject);
										meshObject.Layer = (int)layer;
										if (isRenderSubmesh == false) {
											// Only set the tag for collision-based GameObjects. For now nothing
											// in the game checks tags on render meshes, only for collidable stuff.
											meshObject.Tag = s_specialLevelChunkTagName;
										}
									};

									// Setup the collision mesh
									IComponentBroker meshCollider = collisionMeshObject.AddComponent("MeshCollider");
									meshCollider.SetProperty("isTrigger", false);
									meshCollider.SetProperty("sharedMesh", collisionSubmeshMeshesForChunk[0].m_mesh_object);
									setCommonLevelGameObjectFlags(collisionMeshObject, Overload.UnityObjectLayers.LEVEL, false);

									// Setup the lava collision mesh
									if (lavaCollisionMeshObject != null) {

										// Assign the collision mesh to the level object
										IComponentBroker lavaMeshCollider = lavaCollisionMeshObject.AddComponent("MeshCollider");
										lavaMeshCollider.SetProperty("isTrigger", false);
										lavaMeshCollider.SetProperty("sharedMesh", lavaCollisionSubmeshMeshesForChunk[0].m_mesh_object);

										setCommonLevelGameObjectFlags(lavaCollisionMeshObject, Overload.UnityObjectLayers.LAVA, false);
									}

									// Setup the render meshes
									Material[] chunkMaterials = BindMaterialsChunk(convertState, chunkIdx);
									//Assert.True(chunkMaterials.Length == numRenderSubmeshes);     // This is not necessarily true anymore
									for (int smIdx = 0; smIdx < numRenderSubmeshes; ++smIdx) {
										IGameObjectBroker submeshRenderGameObject = renderMeshObjects[smIdx];

										IComponentBroker meshFilter = submeshRenderGameObject.AddComponent("MeshFilter");
										meshFilter.SetProperty("sharedMesh", renderSubmeshMeshesForChunk[smIdx].m_mesh_object);

										IComponentBroker renderer = submeshRenderGameObject.AddComponent("MeshRenderer");
										renderer.SetProperty("sharedMaterial", chunkMaterials[renderSubmeshMeshesForChunk[smIdx].m_original_submesh_index]);

										// If this is the collision mesh object, this data has already been set
										if (submeshRenderGameObject != collisionMeshObject) {
											setCommonLevelGameObjectFlags(submeshRenderGameObject, Overload.UnityObjectLayers.LEVEL, true);
										}
									}
								}

								//Attach level meta data to LevelObjectInitializer
								IComponentBroker initializer = convertState.SceneLevelContainerObject.GetComponent("LevelData");
								initializer.SetProperty("m_geometry", convertState.MetaData);
							}

							using (convertState.DoProgressUpdate("Computing segment visibility")) {
								// !!!!!!!!!!!
								// This must happen *after* the scene geometry is laid out
								// Which must happen *after* mesh assets are generated
								// !!!!!!!!!!!
								//
								// Once the level object is setup we can compute visibility (which needs to cast rays against the geometry).
								// Using 30 meters for the indirect visibility
								if( FlagForceVisibility || convertState.MetaData.SegmentToSegmentVisibility == null || convertState.MetaData.SegmentToSegmentVisibility.Length == 0 ) {
									Segment2SegmentVis.ComputeSegmentVisibility( convertState.MetaData, segmentToSegmentVisibilitySecondaryDistance );
								} else {
									Debug.Log( "Using previous Segment-to-segment visibility" );
								}
							}

							using (convertState.DoProgressUpdate("Computing path distances")) {
								// Path distances are only used in challenge mode
								if (isChallengeModeLevel) {
									if (FlagForcePathDistances || convertState.MetaData.PathDistances == null || convertState.MetaData.PathDistances.Length == 0) {
										bool robotsCanOpenDoors = isChallengeModeLevel; // yes, this will always be true, initially we did path distances for non-CM levels
										StructureBuilder.ComputePathDistances(convertState.MetaData, convertState.OverloadLevelData, convertState.DooNumToPortalMap, robotsCanOpenDoors);
									} else {
										Debug.Log("Using previous Path Distances");
									}
								} else {
									// No Path Distances
									convertState.MetaData.PathDistances = null;
								}
							}
#if UNITY_EDITOR
							sceneAssetUpdated = true;
#endif
						} finally {
							// Done editing
							scene.AssetDatabase_StopAssetEditing();

							using (convertState.DoProgressUpdate("Refreshing Asset Database")) {
								// Save unsaved assets out
								scene.AssetDatabase_SaveAssets();

								// Finally, ensure the AssetDatabase reflects the changes
								scene.AssetDatabase_Refresh();

								// Re-import the asset after adding an object.
								// Otherwise the change only shows up when saving the project
								scene.AssetDatabase_ImportAsset(levelMetaDataPath);
							}
						}
					}
				}
#endregion

				// Place lights
#region Update Lights
				// Generate all of the decal-based lights
				if (convertState.ShouldUpdateDecalLights) {
#if UNITY_EDITOR
					sceneUpdated = true;
#endif
					using (convertState.DoProgressUpdate("Placing decal-based lights")) {
						IComponentBroker levelObjectInitializer = convertState.SceneLevelContainerObject.GetComponent("LevelData");
						levelObjectInitializer.SetProperty("m_level_lights", PlaceAndGatherSegmentLights(scene, convertState.SceneLevelContainerObject, convertState.IsNewLevel,
							 convertState.MetaData, convertState.OverloadLevelDecalLights, convertState.EditorSegmentIndexToPackedSegmentIndex));
					}
				}
#endregion

				// Place reflection probes
#region Update Reflection Probes
				if (convertState.ShouldUpdateReflectionProbes) {
#if UNITY_EDITOR
					sceneUpdated = true;
#endif
					using (convertState.DoProgressUpdate("Placing reflection probes")) {
						IComponentBroker levelObjectInitializer = convertState.SceneLevelContainerObject.GetComponent("LevelData");
						levelObjectInitializer.SetProperty("m_level_reflection_probes", PlaceAndGatherSceneReflectionProbes(scene, convertState.SceneLevelContainerObject, convertState.IsNewLevel,
							 convertState.MetaData, convertState.OverloadLevelData, convertState.EditorSegmentIndexToPackedSegmentIndex, convertState.ChunkToLevelSegmentIndices));
					}
				}
#endregion

				// Place entities
#region Update Entities
				if (convertState.ShouldUpdateEntities) {
#if UNITY_EDITOR
                    sceneUpdated = true;
#endif
					using (convertState.DoProgressUpdate("Placing entities")) {

						if (!convertState.IsNewLevel) {
							// Clean out all of the child objects except some special
							// ones -- everything else is a placed entity
							var childrenToRemove = new List<IGameObjectBroker>();
							for (int childIdx = 0, numChildren = convertState.SceneLevelContainerObject.Transform.ChildCount; childIdx < numChildren; ++childIdx) {
								var testObject = convertState.SceneLevelContainerObject.Transform.GetChild(childIdx).ownerGameObject;
								if (protectedChildObjectNames.Contains(testObject.Name)) {
									// A protected child object
									continue;
								}

								childrenToRemove.Add(testObject);
							}

							// clean them out
							foreach (var child in childrenToRemove) {
								scene.DestroyGameObject(child);
							}
						}

						PlaceEntities(scene, convertState.OverloadLevelData, convertState.IsNewLevel, convertState.SceneLevelContainerObject, convertState.DooNumToPortalMap);
					}
				}
				#endregion

				// Custom level info
				if (scene.LevelExportType == LevelType.SinglePlayer) {
					string levelInfoObjName = "!level_info";
					IGameObjectBroker levelInfoObj = scene.FindGameObject(levelInfoObjName);
					if (levelInfoObj == null) {
						// We are only going to fill this in if it doesn't exist
						// For Revival/Unity export we don't want to bash over it, but for Overload Level Editor export we always set it
						levelInfoObj = scene.CreateRootGameObject(levelInfoObjName);
						IComponentBroker levelInfoData = levelInfoObj.AddComponent("LevelCustomInfo");

						// Access the data from the loaded level
						var cli = convertState.OverloadLevelData.custom_level_info ?? new OverloadLevelEditor.Level.CustomLevelInfo();

						// Transfer over the properties
						levelInfoData.SetProperty("m_exit_music_start_time", cli.m_exit_music_start_time);
						levelInfoData.SetProperty("m_exit_no_explosions", cli.m_exit_no_explosions);
						levelInfoData.SetProperty("m_alien_lava", cli.m_alien_lava);
						levelInfoData.SetProperty("m_custom_count", cli.m_custom_count);
						levelInfoData.SetProperty("m_objective", (Int32)cli.m_objective);
					}
				}

				Action finalize_work = () => {
                    try {
#if UNITY_EDITOR
                        // Save the scene
                        var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                        using( convertState.DoProgressUpdate( "Saving scene" ) ) {
                            SaveSceneWithVersionControl( activeScene );
                        }

                        // Reconcile changes to work around Unity/Perforce issues
                        using( convertState.DoProgressUpdate( "Updating Perforce" ) ) {
                            P4ReconcileLevelFiles( activeScene );
                        }
#endif

                        // Display timings
                        Debug.Log( "Timings:" );
                        foreach( var kvp in timingsByDepth ) {
                            List<string> timings = kvp.Value;
                            foreach( var timingStr in timings ) {
                                Debug.Log( timingStr );
                            }
                        }
                    } catch(Exception e) {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog( "Error converting level", e.Message, "Ok" );
#endif
                        Debug.LogErrorFormat( "Exception: {0}", e.Message );
                        Debug.LogError( e.StackTrace );
                    } finally {
#if UNITY_EDITOR
                        if( pb != null ) {
                            pb.Clear();
                            pb = null;
                        }
#endif
                    }
                };


                    // No occlusion baking work to do, just finalize the work now
                    finalize_work();
			} catch (CancelExportException) {
#if UNITY_EDITOR
                string message = "Conversion canceled by user";
				if (sceneAssetUpdated) {
					if (sceneUpdated) {
						message += ". WARNING: Your scene and scene assets have been [partially] modified.";
					} else {
						message += ". WARNING: Your scene assets have been modified.";
					}
				}
				if (sceneUpdated) {
					message += ". WARNING: Your scene has been modified.";
				}
				EditorUtility.DisplayDialog("Canceled", message, "Ok");
#endif
				return;
			} catch (Exception e) {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Error converting level", e.Message, "Ok");
#endif
				Debug.LogErrorFormat("Exception: {0}", e.Message);
				Debug.LogError(e.StackTrace);
				return;
			} finally {
#if UNITY_EDITOR
                if( pb != null ) {
                    pb.Clear();
                    pb = null;
                }
#endif
			}
		}

#if !OVERLOAD_LEVEL_EDITOR
        public static void SetStaticFlags(UnityEngine.GameObject go)
        {
            GameObjectUtility.SetStaticEditorFlags(go, (StaticEditorFlags)(
                (int)StaticEditorFlags.LightmapStatic |
                (int)StaticEditorFlags.NavigationStatic |
                (int)StaticEditorFlags.BatchingStatic |
                (int)StaticEditorFlags.OccludeeStatic |
                (int)StaticEditorFlags.OccluderStatic |
                (int)StaticEditorFlags.OffMeshLinkGeneration |
                (int)StaticEditorFlags.ReflectionProbeStatic
                ));
        }
#endif

        public static void SetStaticFlags( IGameObjectBroker go )
		{
#if !OVERLOAD_LEVEL_EDITOR
            SetStaticFlags(go.InternalObject);
#endif
        }

		static void SaveMeshAssets(ISceneBroker scene, List<LevelConvertStateManager.SubmeshObject>[] submeshes_per_chunk, UnityEngine.Object assetCollectionObject)
		{
			int num_chunks = submeshes_per_chunk.Length;
			for (int chunk_index = 0; chunk_index < num_chunks; ++chunk_index) {

				List<LevelConvertStateManager.SubmeshObject> submeshes_for_chunk = submeshes_per_chunk[chunk_index];

				int num_submeshes = submeshes_for_chunk.Count;
				for (int sm_index = 0; sm_index < num_submeshes; ++sm_index) {
					Mesh meshObj = submeshes_for_chunk[sm_index].m_mesh_object;
					scene.AssetDatabase_AddObjectToAsset(meshObj, assetCollectionObject);
				}
			}
		}

		static IEnumerable<int> EnumerateSegmentIndicesUnderSegmentAABBTreeIndex(int aabbTreeIndex, LevelGeometry levelData)
		{
			if (aabbTreeIndex < 0 || aabbTreeIndex >= levelData.SegmentAABBTree.Length) {
				yield break;
			}

			if (levelData.SegmentAABBTree[aabbTreeIndex].SegmentIndex != -1) {
				yield return levelData.SegmentAABBTree[aabbTreeIndex].SegmentIndex;
			}

			foreach (int child in EnumerateSegmentIndicesUnderSegmentAABBTreeIndex(levelData.SegmentAABBTree[aabbTreeIndex].MinChildIndex, levelData)) {
				yield return child;
			}

			foreach (int child in EnumerateSegmentIndicesUnderSegmentAABBTreeIndex(levelData.SegmentAABBTree[aabbTreeIndex].MaxChildIndex, levelData)) {
				yield return child;
			}
		}

		static int FindSegmentForPoint(Vector3 point, LevelGeometry levelData)
		{
			Func<AABB, bool> isInBounds = (bounds) => {
				if (point.x < bounds.MinXYZ.x)
					return false;
				if (point.y < bounds.MinXYZ.y)
					return false;
				if (point.z < bounds.MinXYZ.z)
					return false;

				if (point.x > bounds.MaxXYZ.x)
					return false;
				if (point.y > bounds.MaxXYZ.y)
					return false;
				if (point.z > bounds.MaxXYZ.z)
					return false;

				return true;
			};

			// NOTE: If the point is outside of a segment this might not
			// select the segment with the closest center in the situation
			// where the closest segment isn't in the deepest containing
			// bounding box.
			int lastTreeNodeContainingPoint = 0;
			if (isInBounds(levelData.SegmentAABBTree[lastTreeNodeContainingPoint].Bounds)) {
				while (true) {

					int minChildIdx = levelData.SegmentAABBTree[lastTreeNodeContainingPoint].MinChildIndex;
					int maxChildIdx = levelData.SegmentAABBTree[lastTreeNodeContainingPoint].MaxChildIndex;

					if (minChildIdx == -1 && maxChildIdx == -1) {
						// All the way down to a segment, just go with it
						Assert.True(levelData.SegmentAABBTree[lastTreeNodeContainingPoint].SegmentIndex != -1);
						return levelData.SegmentAABBTree[lastTreeNodeContainingPoint].SegmentIndex;
					}

					Assert.True(minChildIdx != -1);
					Assert.True(maxChildIdx != -1);

					// Choose the best child
					AABB minBounds = levelData.SegmentAABBTree[minChildIdx].Bounds;
					AABB maxBounds = levelData.SegmentAABBTree[maxChildIdx].Bounds;

					bool minGood = isInBounds(minBounds);
					bool maxGood = isInBounds(maxBounds);

					if (minGood == maxGood) {
						// Ambiguous of where to go from here, have to check individuals
						break;
					}

					if (minGood) {
						lastTreeNodeContainingPoint = minChildIdx;
					} else {
						lastTreeNodeContainingPoint = maxChildIdx;
					}
				}
			}

			// Check all the segments under this node
			float bestDistance = 0.0f;
			int bestSegmentIndex = -1;
			foreach (int segmentIndex in EnumerateSegmentIndicesUnderSegmentAABBTreeIndex(lastTreeNodeContainingPoint, levelData)) {
				Vector3 center = levelData.Segments[segmentIndex].Center;
				float distToCenter = Vector3.SqrMagnitude(center - point);
				if (bestSegmentIndex == -1 || distToCenter < bestDistance) {
					bestDistance = distToCenter;
					bestSegmentIndex = segmentIndex;
				}
			}

			return bestSegmentIndex;
		}

#region Decal lights
		static int GetSegmentIndexForLight(IComponentBroker light, LevelGeometry levelData)
		{
			return FindSegmentForPoint(light.ownerGameObject.Transform.Position, levelData);
		}

		// Generates all of the decal lights given by segmentDecalLights, also processes over user-placed lights under !lights
		// Returns all of the lights for the scene.
		// Side-effect: Wipes out all of the old lights under the light container and refreshes it
		static SegmentLightInfo[] PlaceAndGatherSegmentLights( ISceneBroker scene, IGameObjectBroker levelConversionPrefabObject, bool isNewScene, LevelGeometry levelData, IEnumerable<DecalLightInfo> segmentDecalLights, Dictionary<int,int> editorSegmentIndexToPackedSegmentIndex)
		{
            List<SegmentLightInfo> light_metadata = new List<SegmentLightInfo>();

#if !OVERLOAD_LEVEL_EDITOR
			// Dig through all the user placed lights and create metadata for them
			{
				IGameObjectBroker user_container_object = scene.FindGameObject(s_specialLightsUserContainerObjectName);
				if (user_container_object != null && user_container_object.Transform.Parent == null) {
					// Gather all the reflection probes under "!lights"
					light_metadata.AddRange(user_container_object.GetComponentsInChildren("Light", false)
								.Select(light => new SegmentLightInfo() {
									LightObject = light.ownerGameObject.InternalObject,
									LightType = SegmentLightType.UserPlaced,
									SegmentIndex = GetSegmentIndexForLight(light, levelData)
								}));
				}
			}
#endif

			// Create a container object to reduce clutter
			const string containerName = s_specialLightContainerObjectName;
			IGameObjectBroker lightContainerObject = null;

			if (!isNewScene) {
				// Look for the existing container object
				for (int childIdx = 0, numChildren = levelConversionPrefabObject.Transform.ChildCount; childIdx < numChildren; ++childIdx) {
					var testObject = levelConversionPrefabObject.Transform.GetChild(childIdx).ownerGameObject;
					if (testObject.Name == containerName) {
						lightContainerObject = testObject;
						break;
					}
				}

				if (lightContainerObject != null) {
					// Clean out all the old lights
					while (lightContainerObject.Transform.ChildCount > 0) {
						scene.DestroyGameObject(lightContainerObject.Transform.GetChild(0).ownerGameObject);
					}
				}
			}

			if (lightContainerObject == null) {
				lightContainerObject = scene.CreateRootGameObject(containerName);
				lightContainerObject.Transform.Parent = levelConversionPrefabObject.Transform;
			}

			//Per-chunk containers for lights
			Dictionary<int, IGameObjectBroker> chunk_containers = new Dictionary<int, IGameObjectBroker>();

			bool addLightsToMetadata = lightContainerObject.ActiveInHierarchy;
			int lightnum = 0;
			foreach (var decalSegmentLight in segmentDecalLights) {
				var decalLight = decalSegmentLight.Light;
				string lightName;
				if (decalSegmentLight.Light.m_flare != OverloadLevelEditor.LightFlare.NONE) {
					lightName = "sec" + ((int)decalSegmentLight.Light.m_flare).ToString() + "_light";
            } else {
					lightName = string.Format("Decal{0}Light", s_text_info.ToTitleCase(decalLight.m_style.ToString().ToLowerInvariant()));
				}
				lightName += lightnum.ToString("D3");
				lightnum++;

				var lightGameObject = scene.CreateRootGameObject(lightName);
                var lightComp = lightGameObject.AddComponent( "Light" );

				lightGameObject.Transform.Position = decalLight.m_position.ToUnity();
				lightGameObject.Transform.Rotation = OpenTKExtensions.OpenTKQuaternion.ExtractRotation(decalLight.m_orientation).ToUnity();

				//lightGameObject.transform.parent = lightContainerObject.transform; // parent to the prefab
				IGameObjectBroker chunk_container;
				int chunknum = levelData.Segments[decalSegmentLight.EditorSegmentIndex].ChunkNum;
				if (!chunk_containers.TryGetValue(chunknum, out chunk_container)) {
					chunk_container = scene.CreateRootGameObject("Chunk" + chunknum.ToString("D3"));
					chunk_container.Transform.Parent = lightContainerObject.Transform; // parent to the prefab
					chunk_containers.Add(chunknum, chunk_container);
				}
				lightGameObject.Transform.Parent = chunk_container.Transform;

                lightComp.SetProperty( "type", ( ( decalLight.m_style == OverloadLevelEditor.LightStyle.SPOT || decalLight.m_style == OverloadLevelEditor.LightStyle.SPOT_NO_SHADOW ) ? LightType.Spot : LightType.Point ) );
                lightComp.SetProperty( "color", new Color( decalSegmentLight.Color.x, decalSegmentLight.Color.y, decalSegmentLight.Color.z ) );
                lightComp.SetProperty( "enabled", true );
                //lightComp.flare
                lightComp.SetProperty( "intensity", decalLight.m_intensity );
                lightComp.SetProperty( "range", decalLight.m_range );
                lightComp.SetProperty( "spotAngle", decalLight.m_spot_angle );
                lightComp.SetProperty( "shadowBias", 0.015f );
                lightComp.SetProperty( "shadowNearPlane", 0.3f );
                lightComp.SetProperty( "shadows", ( ( decalLight.m_style == OverloadLevelEditor.LightStyle.POINT_NO_SHADOW || decalLight.m_style == OverloadLevelEditor.LightStyle.SPOT_NO_SHADOW ) ? LightShadows.None : LightShadows.Hard ) );
                lightComp.SetProperty( "shadowStrength", 0.35f );	// 9/27/2018: Shadow strength is now a measure of blur

                if (addLightsToMetadata) {
					// Add to the metadata
					light_metadata.Add(new SegmentLightInfo() {
#if OVERLOAD_LEVEL_EDITOR
                        LightObject = lightComp.ownerGameObject,
#else
                        LightObject = lightComp.ownerGameObject.InternalObject,
#endif
                        LightType = SegmentLightType.Decal,
						SegmentIndex = editorSegmentIndexToPackedSegmentIndex[decalSegmentLight.EditorSegmentIndex],
					});
				}
			}

            return light_metadata.ToArray();
		}
#endregion

#region Reflection probes

		static int GetSegmentIndexForProbe(IComponentBroker probe, LevelGeometry levelData)
		{
			return FindSegmentForPoint(probe.ownerGameObject.Transform.Position, levelData);
		}

		// Generates a reflection probe at the center of every chunk, also processes over user-placed probes under !probes
		// Returns all of the probes for the scene.
		// Side-effect: Wipes out all of the old probes under the probe container and refreshes it
		static SegmentReflectionProbeInfo[] PlaceAndGatherSceneReflectionProbes( ISceneBroker scene, IGameObjectBroker levelConversionSceneObject, bool isNewScene, LevelGeometry levelData, OverloadLevelEditor.Level overloadLevel, Dictionary<int,int> editorSegmentIndexToPackedSegmentIndex, Dictionary<int,List<int>> chunkToLevelDocsegmentIndices)
		{
			List<SegmentReflectionProbeInfo> probeMetadata = new List<SegmentReflectionProbeInfo>();

#if !OVERLOAD_LEVEL_EDITOR
			// Dig through all the user placed probes and create metadata for them
			{
				IGameObjectBroker userContainerObject = scene.FindGameObject(s_specialReflectionProbesUserContainerObjectName);
				if (userContainerObject != null && userContainerObject.Transform.Parent == null) {
					// Gather all the reflection probes under "!probes"
					probeMetadata.AddRange(userContainerObject.GetComponentsInChildren("ReflectionProbe", false)
				 .Select(probe => new SegmentReflectionProbeInfo() {
					 ProbeObject = probe.ownerGameObject.InternalObject,

					 ProbeType = SegmentReflectionProbeType.UserPlaced,
					 SegmentIndex = GetSegmentIndexForProbe(probe, levelData)
				 }));
				}
			}
#endif

			// Create a container object to reduce clutter
			const string containerName = s_specialReflectionProbesContainerObjectName;
			IGameObjectBroker containerObject = null;

			if (!isNewScene) {
				// Look for the existing container object
				for (int childIdx = 0, numChildren = levelConversionSceneObject.Transform.ChildCount; childIdx < numChildren; ++childIdx) {
					var testObject = levelConversionSceneObject.Transform.GetChild(childIdx).ownerGameObject;
					if (testObject.Name == containerName) {
						containerObject = testObject;
						break;
					}
				}

                if( containerObject != null) {
					// Clean out all the probes
					while (containerObject.Transform.ChildCount > 0) {
						scene.DestroyGameObject(containerObject.Transform.GetChild(0).ownerGameObject);
					}
				}
			}

			if (containerObject == null) {
				containerObject = scene.CreateRootGameObject(containerName);
				containerObject.Transform.Parent = levelConversionSceneObject.Transform;
				containerObject.AddComponent( "RPRefresher" );
			}

			if (chunkToLevelDocsegmentIndices == null) {
				// No auto probes to place...
				return probeMetadata.ToArray();
			}

			bool addProbesToMetadata = containerObject.ActiveInHierarchy;

			// We are adding a reflection probe per chunk. Go through each chunk and find
			// the best (hopefully) suitable place for the probe. This is all just a shot
			// in the dark and probably error prone
			foreach (var kvp in chunkToLevelDocsegmentIndices) {

				var chunkNum = kvp.Key;
				var segmentIndices = kvp.Value;
				var segmentsInChunk = segmentIndices
					.Select(seg_index => new KeyValuePair<int, OverloadLevelEditor.Segment>(seg_index, overloadLevel.segment[seg_index]))
					.ToArray();

				// Find the centroid of the chunk
				const float hugeFloat = 99999999.9f;
				Vector3 chunkCentroidPosition = Vector3.zero;
				Vector3 chunkMinPos = new Vector3(hugeFloat, hugeFloat, hugeFloat);
				Vector3 chunkMaxPos = -chunkMinPos;
				{
					int numVertsToAverage = 0;
					foreach (var vertPos in segmentsInChunk
						.SelectMany(seg => seg.Value.vert)
						.Select(vert_idx => overloadLevel.vertex[vert_idx].position.ToUnity())) {
						chunkCentroidPosition += vertPos;
						++numVertsToAverage;

						// also update the AABB of the chunk
						chunkMinPos = Vector3.Min(chunkMinPos, vertPos);
						chunkMaxPos = Vector3.Max(chunkMaxPos, vertPos);
					}

					float scale = 1.0f / (float)numVertsToAverage;
					chunkCentroidPosition.x *= scale;
					chunkCentroidPosition.y *= scale;
					chunkCentroidPosition.z *= scale;
				}

				// Find the segment center closest to the chunk centroid
				Vector3 bestSegmentCentroid = Vector3.zero;
				int bestSegmentIndex = -1;
				{
					float bestSegmentCentroidDist = hugeFloat;
					foreach (var segmentCentroidPositionKVP in segmentsInChunk
						.Select(segKVP => new { SegIndex = segKVP.Key, Verts = segKVP.Value.vert.Select(vert_idx => overloadLevel.vertex[vert_idx].position.ToUnity()).ToArray() } )
						.Select(segVertsKVP =>
						{
							// Calculate the centroid for the segment verts
							Vector3 centroid = Vector3.zero;
							int numVerts = 0;
							foreach (var vert_pos in segVertsKVP.Verts) {
								centroid += vert_pos;
								++numVerts;
							}

							float scale = 1.0f / (float)numVerts;
							centroid.x *= scale;
							centroid.y *= scale;
							centroid.z *= scale;
							return new { SegIndex = segVertsKVP.SegIndex, Centroid = centroid };
						})) {
						// given this segment centroid, find the distance to the chunk centroid
						// if it is closer, then use this
						float distToChunkCentroid = Vector3.Distance(segmentCentroidPositionKVP.Centroid, chunkCentroidPosition);
						if (distToChunkCentroid < bestSegmentCentroidDist) {
							// This is a better segment center
							bestSegmentIndex = segmentCentroidPositionKVP.SegIndex;
							bestSegmentCentroid = segmentCentroidPositionKVP.Centroid;
							bestSegmentCentroidDist = distToChunkCentroid;
						}
					}
				}

				// Create the reflection probe
				Vector3 chunkBoundsCenter = (chunkMaxPos + chunkMinPos) * 0.5f;
				var reflectionGO = scene.CreateRootGameObject(string.Format("refl_probe{0}", chunkNum.ToString("D4")));
				reflectionGO.Transform.Position = bestSegmentCentroid;
				reflectionGO.Transform.Parent = containerObject.Transform;

                int cullingMask = -1; // default enable all layers?
                cullingMask ^= (1 << (int)Overload.UnityObjectLayers.ENEMY_MESH); // Ignore enemies when rendering
                cullingMask ^= (1 << (int)Overload.UnityObjectLayers.ITEMS); // Ignore items when rendering
                cullingMask ^= (1 << (int)Overload.UnityObjectLayers.RP_IGNORE); // Ignore items when rendering

                var reflProbeComp = reflectionGO.AddComponent( "ReflectionProbe" );
                reflProbeComp.SetProperty( "mode", UnityEngine.Rendering.ReflectionProbeMode.Baked );
                reflProbeComp.SetProperty( "refreshMode", UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting );
                reflProbeComp.SetProperty( "boxProjection", true );
                reflProbeComp.SetProperty( "resolution", 256 );
                reflProbeComp.SetProperty( "intensity", 1.5f );
                reflProbeComp.SetProperty( "clearFlags", UnityEngine.Rendering.ReflectionProbeClearFlags.SolidColor );
                reflProbeComp.SetProperty( "backgroundColor", Color.black );
                reflProbeComp.SetProperty( "cullingMask", cullingMask );
                reflProbeComp.SetProperty( "center", chunkBoundsCenter - bestSegmentCentroid );
                reflProbeComp.SetProperty( "size", chunkMaxPos - chunkMinPos + Vector3.one * 2f );
                reflProbeComp.SetProperty( "farClipPlane", 100f );
                reflProbeComp.SetProperty( "nearClipPlane", 0.3f );

                if (addProbesToMetadata) {
					// Add to the metadata
					probeMetadata.Add(new SegmentReflectionProbeInfo() {
#if OVERLOAD_LEVEL_EDITOR
                        ProbeObject = reflProbeComp.ownerGameObject,
#else
                        ProbeObject = reflProbeComp.ownerGameObject.InternalObject,
#endif
                        ProbeType = SegmentReflectionProbeType.ConverterGenerated,
						SegmentIndex = editorSegmentIndexToPackedSegmentIndex[bestSegmentIndex],
					});
				}
			}

			return probeMetadata.ToArray();
		}
#endregion
	}

	public static void WriteSerializationLevelHeader(OverloadLevelConvertSerializer serializer, uint levelType)
	{
		const uint version = 4;
		serializer.SetVersion(version);

		serializer.SerializeOut_uint32(0x52657631);            // Magic number 'Rev1'
		serializer.SerializeOut_uint32(version);               // Version
		serializer.SerializeOut_uint32(levelType);             // Level type
	}

	public static void ConvertLevel(string pathToFile, string prefabName, string editorRootFolder, ISceneBroker scene)
	{
		LevelConvertState lcs = new LevelConvertState();
		lcs.ConvertLevel(pathToFile, prefabName, editorRootFolder, scene, null);
	}

    public static void ConvertLevel( string pathToFile, Newtonsoft.Json.Linq.JObject overloadLevelFileData, string prefabName, string editorRootFolder, ISceneBroker scene )
    {
        LevelConvertState lcs = new LevelConvertState();
        lcs.ConvertLevel( pathToFile, prefabName, editorRootFolder, scene, overloadLevelFileData );
    }

    public static Mesh CombineMeshVerts(Mesh m)
	{
		// PER MATERIAL:
		//  - If position + normal + uv_fraction of 2 triangles match:
		//    - If neither exists in a list, start a new triangle list
		//		- If one exists in another list, add the other to that list
		//    - If other also exists in another list, combine both lists (and delete the partial one)
		//  - Once done, start with first "unaligned" triangle in list, and find any *adjacent* triangles (in the list) and "align" those (aka offset the UVs)
		//    - Continue recursively in this manner, marking the triangles as "aligned" as you get to them
		//    - Repeat until all triangles are aligned

		//  - Now use the same combining algorithm from import to remove verts in the optimized mesh

		// NOTE FROM JEFF: WHILE WE CAN DO ALL OF THIS AS A POST-PROCESS, IT SHOULD BE ABLE
		// TO BE DONE AS WE ARE BUILDING UP THE MESH TO BEGIN WITH.
		return m;
	}

#if UNITY_EDITOR

    public static bool SaveSceneWithVersionControl( UnityEngine.SceneManagement.Scene active_scene )
    {
        if( string.IsNullOrEmpty( active_scene.path ) ) {
            return false;
        }

        bool complete_save = true;

        // Check out the .unity file
        if( UnityEditor.VersionControl.Provider.enabled && UnityEditor.VersionControl.Provider.isActive ) {

            UnityEditor.VersionControl.Asset asset = UnityEditor.VersionControl.Provider.GetAssetByPath( active_scene.path );
            if( asset != null ) {

                // First you have to query the status (*sigh*)
                UnityEditor.VersionControl.Task status_task = UnityEditor.VersionControl.Provider.Status( asset );
                status_task.Wait();

                // Now see if checking out is a valid operation
                if( UnityEditor.VersionControl.Provider.CheckoutIsValid( status_task.assetList[0] ) ) {

                    // Only bother to check it out if it isn't already
                    if( !UnityEditor.VersionControl.Provider.IsOpenForEdit( status_task.assetList[0] ) ) {

                        // Now check it out
                        UnityEditor.VersionControl.Task checkout_task = UnityEditor.VersionControl.Provider.Checkout( status_task.assetList[0], UnityEditor.VersionControl.CheckoutMode.Asset );
                        checkout_task.Wait();

                        // Update the status (again)
                        status_task = UnityEditor.VersionControl.Provider.Status( asset );
                        status_task.Wait();

                        // Save only if we could check out the file
                        if( !UnityEditor.VersionControl.Provider.IsOpenForEdit( status_task.assetList[0] ) ) {
                            complete_save = false;
                            Debug.LogErrorFormat( "Unable to checkout ({0}), someone else may have it checked out. !!! YOUR CHANGES WILL NOT BE REFLECTED IN PERFORCE !!!", active_scene.path );
                        }
                    }
                }
            }
        }

        if( !complete_save ) {
            return false;
        }

        return UnityEditor.SceneManagement.EditorSceneManager.SaveScene( active_scene );
    }

    public static bool BakeOcclusion( System.Action done_callback )
    {
        if( StaticOcclusionCulling.isRunning ) {
            // Already running
            return false;
        }

        // Callback to poll for when Occlusion baking is complete
        EditorApplication.CallbackFunction on_update = null;
        on_update = new EditorApplication.CallbackFunction( () => {
            if( StaticOcclusionCulling.isRunning ) {
                return;
            }

            // No longer running - unhook the callback and call to the callback
            EditorApplication.update -= on_update;
            if( done_callback != null ) {
                try {
                    done_callback();
                } catch(Exception ex ) {
                    Debug.LogError( ex.Message );
                }
            }
        } );

        // Start generation in the background
        if( StaticOcclusionCulling.GenerateInBackground() ) {
            // Hook up to get a callback regularly
            EditorApplication.update += on_update;
            return true;
        }

        return false;
    }

    public static string GetP4Path()
    {
        const string hardcodedX64Path = @"C:\Program Files\Perforce\p4.exe";
        const string hardcodedX86Path = @"C:\Program Files (x86)\Perforce\p4.exe";
        const string relativeP4Exe = @"..\Other\Perforce\p4.exe";

        if( System.IO.File.Exists( hardcodedX64Path ) ) {
            return hardcodedX64Path;
        }

        if( System.IO.File.Exists( hardcodedX86Path ) ) {
            return hardcodedX86Path;
        }

        string localPath = IOPath.GetFullPath( IOPath.Combine( Application.dataPath.Replace( '/', '\\' ), relativeP4Exe ) );
        if( System.IO.File.Exists( localPath ) ) {
            return localPath;
        }

        return null;
    }

    public static string GetActiveLevelPerforceResolveQueryString()
    {
        foreach( var go in GameObject.FindGameObjectsWithTag( LevelConvertState.s_specialLevelTagName ) ) {

            GameObject currLevelPrefabInst = go;

            var parentTransform = go.transform.parent;
            if( go.name == "LevelObject" && parentTransform ) {
                currLevelPrefabInst = parentTransform.gameObject;
            } else if( parentTransform ) {
                continue;
            }

            if( currLevelPrefabInst == null ) {
                // Don't clear out the currently set value
                continue;
            }

            string baseFilename = IOPath.GetFileNameWithoutExtension( currLevelPrefabInst.name );
            string levelFolder = IOPath.Combine( Application.dataPath.Replace( '/', '\\' ), "Levels" );
            string queryStr = IOPath.Combine( levelFolder, baseFilename ) + "_metadata.asset*";
            return queryStr;
        }

        return null;
    }

    // Call this after all level files should be saved to disk to make sure Perforce reflects the correct state
    public static bool P4ReconcileLevelFiles( UnityEngine.SceneManagement.Scene active_scene )
    {
        if( string.IsNullOrEmpty( active_scene.path ) ) {
            return false;
        }

        string scenePath = active_scene.path.Replace( '/', '\\' );
        string rootFolder = IOPath.Combine( Application.dataPath.Replace( '/', '\\' ), ".." );
        string sceneFolder = IOPath.GetDirectoryName( scenePath );
        string sceneNameRoot = IOPath.GetFileNameWithoutExtension( scenePath );
        string sceneWorkingFolder = IOPath.GetFullPath( IOPath.Combine( IOPath.Combine( rootFolder, sceneFolder ), sceneNameRoot ) );
        string subfolderQuery = sceneWorkingFolder + "\\...";
        string sceneAbsolutePath = IOPath.GetFullPath( IOPath.Combine( rootFolder, scenePath ) );

        // First the .unity file
        if( !P4Reconcile( sceneAbsolutePath ) ) {
            return false;
        }

        // Next the level metadata files
        string levelData = GetActiveLevelPerforceResolveQueryString();
        if( levelData != null && !P4Reconcile( levelData ) ) {
            return false;
        }

        // Now all of the scene asset files (lighting, occlusion, reflection probes)
        return P4Reconcile( subfolderQuery );
    }

    public static bool P4Execute( string commandAndQuery )
    {
        string p4exe = GetP4Path();
        if( p4exe == null ) {
            return false;
        }

        string p4ServerAndPort = EditorUserSettings.GetConfigValue( "vcPerforceServer" );
        string p4User = EditorUserSettings.GetConfigValue( "vcPerforceUsername" );
        string p4Workspace = EditorUserSettings.GetConfigValue( "vcPerforceWorkspace" );

        if( string.IsNullOrEmpty( p4ServerAndPort ) || string.IsNullOrEmpty( p4User ) || string.IsNullOrEmpty( p4Workspace ) ) {
            return false;
        }

        string args = string.Format( "-p \"{0}\" -u \"{1}\" -c \"{2}\" {3}", p4ServerAndPort, p4User, p4Workspace, commandAndQuery );

        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        try {
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = p4exe;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.WorkingDirectory = Application.dataPath.Replace( '/', '\\' );
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            string err = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            if( output.Length > 0 ) {
                Debug.Log( output );
            }
            if( err.Length > 0 ) {
                if( proc.ExitCode != 0 ) {
                    Debug.LogError( err );
                } else {
                    Debug.LogWarning( err );
                }
            }
            return proc.ExitCode == 0;
        } catch( System.Exception e ) {
            Debug.LogError( e.Message );
            return false;
        }
    }

    public static bool P4Reconcile( string query )
    {
        string commandAndQuery = string.Format( "reconcile -f \"{0}\"", query );
        return P4Execute( commandAndQuery );
    }

#endif
}
