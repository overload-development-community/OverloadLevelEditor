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
using System.Collections.Generic;
using System.Linq;
using OverloadLevelExport;

public class UserLevelLoader : UnityEngine.MonoBehaviour, ICmdExecutor
{
	static bool AllowScriptAssemblies = false;

	public UnityEngine.GameObject gameManagerPrefab;

	Dictionary<UInt16, Type> m_commandPacketHandlers = new Dictionary<UInt16, Type>();
	Dictionary<Type, System.Reflection.MethodInfo> m_commandExecutor = new Dictionary<Type, System.Reflection.MethodInfo>();
	bool m_isExecuting = true;
	List<Action> m_postLoadCallbacks = new List<Action>();

	string m_filesystemLevelFilename;
	IUserFileSystem m_fileSystem;


	////////////////////////////////////////////////////////////////
	static Dictionary<string, System.Reflection.Assembly> s_allLoadedAssemblies = new Dictionary<string, System.Reflection.Assembly>();
	Dictionary<string, Type> m_typeLookup = new Dictionary<string, Type>();
	Dictionary<Guid, UnityEngine.Object> m_assetDatabaseAssets = new Dictionary<Guid, UnityEngine.Object>();
	Dictionary<Guid, UnityEngine.GameObject> m_gameObjectMap = new Dictionary<Guid, UnityEngine.GameObject>();
	Dictionary<Guid, UnityEngine.Component> m_componentMap = new Dictionary<Guid, UnityEngine.Component>();
	Dictionary<Guid, UnityEngine.GameObject> m_prefabSearchReferences = new Dictionary<Guid, UnityEngine.GameObject>();
	Dictionary<Guid, UnityEngine.AssetBundle> m_loadedBundlesByGuid = new Dictionary<Guid, UnityEngine.AssetBundle>();
	Dictionary<string, System.Reflection.Assembly> m_loadedAssemblies = new Dictionary<string, System.Reflection.Assembly>();
	Dictionary<Guid, UnityEngine.GameObject> m_inlinePrefabs = new Dictionary<Guid, UnityEngine.GameObject>();
	List<UnityEngine.GameObject> m_objectsToActivate = new List<UnityEngine.GameObject>();
	UnityEngine.GameObject m_topLevelContainer = null;
	////////////////////////////////////////////////////////////////

	void ClearAllInternalBookkeeping()
	{
		m_typeLookup.Clear();
		m_assetDatabaseAssets.Clear();
		m_gameObjectMap.Clear();
		m_componentMap.Clear();
		m_prefabSearchReferences.Clear();
		m_loadedBundlesByGuid.Clear();
		m_objectsToActivate.Clear();
		m_loadedAssemblies.Clear();
		m_inlinePrefabs.Clear();
	}

	void PostLoadCleanup()
	{
		// Remove the inline prefabs objects that we created
		foreach (var kvp in m_inlinePrefabs) {
			UnityEngine.GameObject.DestroyImmediate(kvp.Value);
		}

		// Call unload on any AssetBundles we loaded. This will trigger Unity to release the
		// header from memory for the bundle. We'll ensure assets from the bundle stay in 
		// memory. The next time a scene is loaded (non-additively) or UnloadUnusedResources
		// is called, memory will be released.
		foreach (var kvp in m_loadedBundlesByGuid) {
			kvp.Value.Unload(false);
		}

		// Cleanup references hanging out in code
		ClearAllInternalBookkeeping();
	}

	public UnityEngine.GameObject ResolveGameObject(Guid uid)
	{
		UnityEngine.GameObject go;
		if (!m_gameObjectMap.TryGetValue(uid, out go)) {
			// TODO: Report error
			return null;
		}
		return go;
	}

	public UnityEngine.Component ResolveComponent(Guid uid)
	{
		UnityEngine.Component comp;
		if (!m_componentMap.TryGetValue(uid, out comp)) {
			// TODO: Report error
			return null;
		}
		return comp;
	}

	public UnityEngine.Object ResolveAsset(Guid uid)
	{
		UnityEngine.Object obj;
		if (!m_assetDatabaseAssets.TryGetValue(uid, out obj)) {
			// TODO: Report error
			return null;
		}
		return obj;
	}

	private static UserLevelLoader FindInstance()
	{
		UnityEngine.GameObject loaderGo = UnityEngine.GameObject.Find("Loader");
		if (loaderGo == null) {
			return null;
		}

		return loaderGo.GetComponent<UserLevelLoader>();
	}

	public static bool IsLoadingUserLevel
	{
		get
		{
			var loader = FindInstance();
			if (loader == null) {
				return false;
			}
			return loader.m_isExecuting;
		}
	}

	public static void ExecuteAfterLoading(System.Action callback)
	{
		var loader = FindInstance();
		if (loader != null) {
			loader.m_postLoadCallbacks.Add(callback);
		}
	}

	static bool m_firstGetAssembliesProcess = true;

	Type GetTypeByName(string className)
	{
		var returnVal = new List<Type>();

		// Check the executing assembly first before anything else
		var thisAsm = System.Reflection.Assembly.GetExecutingAssembly();
		foreach (var assemblyType in thisAsm.GetTypes()) {
			if (assemblyType.Name == className) {
				returnVal.Add(assemblyType);
			}
		}

		if (returnVal.Count == 0) {
			// Check all of the assemblies
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					Type[] assemblyTypes = a.GetTypes();
					for (int j = 0; j < assemblyTypes.Length; j++) {
						if (assemblyTypes[j].Name == className) {
							returnVal.Add(assemblyTypes[j]);
						}
					}
				} catch {
					if (m_firstGetAssembliesProcess) {
						UnityEngine.Debug.LogFormat("Failed to get types for {0}", a.ToString());
					}
				}
			}
			m_firstGetAssembliesProcess = false;
		}

		// Look for a full name match
		foreach (var t in returnVal) {
			if (t.FullName == className) {
				return t;
			}
		}

		// Pick the first I guess
		if (returnVal.Count == 0) {
			return null;
		}
		return returnVal[0];
	}

	Type LookupType(string type)
	{
		Type res;
		if (m_typeLookup.TryGetValue(type, out res)) {
			return res;
		}

		Type choiceType = GetTypeByName(type);
		if (choiceType == null) {
			return null;
		}

		m_typeLookup.Add(type, choiceType);
		return choiceType;
	}

	void Awake()
	{
		m_commandPacketHandlers[CmdDone.CommandId] = typeof(CmdDone);
		m_commandPacketHandlers[CmdCreateAssetFile.CommandId] = typeof(CmdCreateAssetFile);
		m_commandPacketHandlers[CmdAddAssetToAssetFile.CommandId] = typeof(CmdAddAssetToAssetFile);
		m_commandPacketHandlers[CmdInitializeGameManager.CommandId] = typeof(CmdInitializeGameManager);
		m_commandPacketHandlers[CmdCreateNewGameObject.CommandId] = typeof(CmdCreateNewGameObject);
		m_commandPacketHandlers[CmdTransformSetParent.CommandId] = typeof(CmdTransformSetParent);
		m_commandPacketHandlers[CmdGameObjectSetName.CommandId] = typeof(CmdGameObjectSetName);
		m_commandPacketHandlers[CmdGameObjectSetTag.CommandId] = typeof(CmdGameObjectSetTag);
		m_commandPacketHandlers[CmdGameObjectSetLayer.CommandId] = typeof(CmdGameObjectSetLayer);
		m_commandPacketHandlers[CmdGameObjectAddComponent.CommandId] = typeof(CmdGameObjectAddComponent);
		m_commandPacketHandlers[CmdGameObjectSetComponentProperty.CommandId] = typeof(CmdGameObjectSetComponentProperty);
		m_commandPacketHandlers[CmdAssetRegisterMaterial.CommandId] = typeof(CmdAssetRegisterMaterial);
		m_commandPacketHandlers[CmdFindPrefabReference.CommandId] = typeof(CmdFindPrefabReference);
		m_commandPacketHandlers[CmdInstantiatePrefab.CommandId] = typeof(CmdInstantiatePrefab);
		m_commandPacketHandlers[CmdGetComponentAtRuntime.CommandId] = typeof(CmdGetComponentAtRuntime);
		m_commandPacketHandlers[CmdSaveAsset.CommandId] = typeof(CmdSaveAsset);
		m_commandPacketHandlers[CmdLoadAssetBundle.CommandId] = typeof(CmdLoadAssetBundle);
		m_commandPacketHandlers[CmdLoadAssetFromAssetBundle.CommandId] = typeof(CmdLoadAssetFromAssetBundle);
		m_commandPacketHandlers[CmdCreateMaterial.CommandId] = typeof(CmdCreateMaterial);
		m_commandPacketHandlers[CmdMaterialDisableKeyword.CommandId] = typeof(CmdMaterialDisableKeyword);
		m_commandPacketHandlers[CmdMaterialEnableKeyword.CommandId] = typeof(CmdMaterialEnableKeyword);
		m_commandPacketHandlers[CmdMaterialSetColor.CommandId] = typeof(CmdMaterialSetColor);
		m_commandPacketHandlers[CmdMaterialSetColorArray.CommandId] = typeof(CmdMaterialSetColorArray);
		m_commandPacketHandlers[CmdMaterialSetFloat.CommandId] = typeof(CmdMaterialSetFloat);
		m_commandPacketHandlers[CmdMaterialSetFloatArray.CommandId] = typeof(CmdMaterialSetFloatArray);
		m_commandPacketHandlers[CmdMaterialSetInt.CommandId] = typeof(CmdMaterialSetInt);
		m_commandPacketHandlers[CmdMaterialSetMatrix.CommandId] = typeof(CmdMaterialSetMatrix);
		m_commandPacketHandlers[CmdMaterialSetMatrixArray.CommandId] = typeof(CmdMaterialSetMatrixArray);
		m_commandPacketHandlers[CmdMaterialSetOverrideTag.CommandId] = typeof(CmdMaterialSetOverrideTag);
		m_commandPacketHandlers[CmdMaterialSetTexture.CommandId] = typeof(CmdMaterialSetTexture);
		m_commandPacketHandlers[CmdMaterialSetTextureOffset.CommandId] = typeof(CmdMaterialSetTextureOffset);
		m_commandPacketHandlers[CmdMaterialSetTextureScale.CommandId] = typeof(CmdMaterialSetTextureScale);
		m_commandPacketHandlers[CmdMaterialSetVector.CommandId] = typeof(CmdMaterialSetVector);
		m_commandPacketHandlers[CmdMaterialSetVectorArray.CommandId] = typeof(CmdMaterialSetVectorArray);
		m_commandPacketHandlers[CmdCreateTexture2D.CommandId] = typeof(CmdCreateTexture2D);
		m_commandPacketHandlers[CmdLoadAssemblyFromAssetBundle.CommandId] = typeof(CmdLoadAssemblyFromAssetBundle);
		m_commandPacketHandlers[CmdCreateInlinePrefab.CommandId] = typeof(CmdCreateInlinePrefab);

		Type thisType = GetType();
		System.Reflection.BindingFlags searchBindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
		m_commandExecutor[typeof(CmdDone)] = thisType.GetMethod("ExecuteCmdDone", searchBindingFlags);
		m_commandExecutor[typeof(CmdCreateAssetFile)] = thisType.GetMethod("ExecuteCmdCreateAssetFile", searchBindingFlags);
		m_commandExecutor[typeof(CmdAddAssetToAssetFile)] = thisType.GetMethod("ExecuteCmdAddAssetToAssetFile", searchBindingFlags);
		m_commandExecutor[typeof(CmdInitializeGameManager)] = thisType.GetMethod("ExecuteCmdInitializeGameManager", searchBindingFlags);
		m_commandExecutor[typeof(CmdCreateNewGameObject)] = thisType.GetMethod("ExecuteCmdCreateNewGameObject", searchBindingFlags);
		m_commandExecutor[typeof(CmdTransformSetParent)] = thisType.GetMethod("ExecuteCmdTransformSetParent", searchBindingFlags);
		m_commandExecutor[typeof(CmdGameObjectSetName)] = thisType.GetMethod("ExecuteCmdGameObjectSetName", searchBindingFlags);
		m_commandExecutor[typeof(CmdGameObjectSetTag)] = thisType.GetMethod("ExecuteCmdGameObjectSetTag", searchBindingFlags);
		m_commandExecutor[typeof(CmdGameObjectSetLayer)] = thisType.GetMethod("ExecuteCmdGameObjectSetLayer", searchBindingFlags);
		m_commandExecutor[typeof(CmdGameObjectAddComponent)] = thisType.GetMethod("ExecuteCmdGameObjectAddComponent", searchBindingFlags);
		m_commandExecutor[typeof(CmdGameObjectSetComponentProperty)] = thisType.GetMethod("ExecuteCmdGameObjectSetComponentProperty", searchBindingFlags);
		m_commandExecutor[typeof(CmdAssetRegisterMaterial)] = thisType.GetMethod("ExecuteCmdAssetRegisterMaterial", searchBindingFlags);
		m_commandExecutor[typeof(CmdFindPrefabReference)] = thisType.GetMethod("ExecuteCmdFindPrefabReference", searchBindingFlags);
		m_commandExecutor[typeof(CmdInstantiatePrefab)] = thisType.GetMethod("ExecuteCmdInstantiatePrefab", searchBindingFlags);
		m_commandExecutor[typeof(CmdGetComponentAtRuntime)] = thisType.GetMethod("ExecuteCmdGetComponentAtRuntime", searchBindingFlags);
		m_commandExecutor[typeof(CmdSaveAsset)] = thisType.GetMethod("ExecuteCmdSaveAsset", searchBindingFlags);
		m_commandExecutor[typeof(CmdLoadAssetBundle)] = thisType.GetMethod("ExecuteCmdLoadAssetBundle", searchBindingFlags);
		m_commandExecutor[typeof(CmdLoadAssetFromAssetBundle)] = thisType.GetMethod("ExecuteCmdLoadAssetFromAssetBundle", searchBindingFlags);
		m_commandExecutor[typeof(CmdCreateMaterial)] = thisType.GetMethod("ExecuteCmdCreateMaterial", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialDisableKeyword)] = thisType.GetMethod("ExecuteCmdMaterialDisableKeyword", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialEnableKeyword)] = thisType.GetMethod("ExecuteCmdMaterialEnableKeyword", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetColor)] = thisType.GetMethod("ExecuteCmdMaterialSetColor", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetColorArray)] = thisType.GetMethod("ExecuteCmdMaterialSetColorArray", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetFloat)] = thisType.GetMethod("ExecuteCmdMaterialSetFloat", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetFloatArray)] = thisType.GetMethod("ExecuteCmdMaterialSetFloatArray", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetInt)] = thisType.GetMethod("ExecuteCmdMaterialSetInt", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetMatrix)] = thisType.GetMethod("ExecuteCmdMaterialSetMatrix", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetMatrixArray)] = thisType.GetMethod("ExecuteCmdMaterialSetMatrixArray", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetOverrideTag)] = thisType.GetMethod("ExecuteCmdMaterialSetOverrideTag", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetTexture)] = thisType.GetMethod("ExecuteCmdMaterialSetTexture", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetTextureOffset)] = thisType.GetMethod("ExecuteCmdMaterialSetTextureOffset", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetTextureScale)] = thisType.GetMethod("ExecuteCmdMaterialSetTextureScale", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetVector)] = thisType.GetMethod("ExecuteCmdMaterialSetVector", searchBindingFlags);
		m_commandExecutor[typeof(CmdMaterialSetVectorArray)] = thisType.GetMethod("ExecuteCmdMaterialSetVectorArray", searchBindingFlags);
		m_commandExecutor[typeof(CmdCreateTexture2D)] = thisType.GetMethod("ExecuteCmdCreateTexture2D", searchBindingFlags);
		m_commandExecutor[typeof(CmdLoadAssemblyFromAssetBundle)] = thisType.GetMethod("ExecuteCmdLoadAssemblyFromAssetBundle", searchBindingFlags);
		m_commandExecutor[typeof(CmdCreateInlinePrefab)] = thisType.GetMethod("ExecuteCmdCreateInlinePrefab", searchBindingFlags);

		Serializers.RegisterSerializers();

		// Create the top level container which will be set to be non-active
		// Every GameObject created will be placed underneath this initially. This will
		// keep Awake from being called on components until we have deserialized everything
		m_topLevelContainer = new UnityEngine.GameObject("Root");
		m_topLevelContainer.SetActive(false);

		string zipFilePath = Overload.GameplayManager.Level.ZipPath;
		string path = Overload.GameplayManager.Level.FilePath;

		if (zipFilePath != null) {
			// Assume it to be a ZIP file system
			string internalFolder = System.IO.Path.GetDirectoryName(path);
			m_filesystemLevelFilename = System.IO.Path.GetFileName(path);
			m_fileSystem = new ZipUserFileSystem(zipFilePath, internalFolder);
		} else {
			// Assume it is to use the raw file system
			string folder = System.IO.Path.GetDirectoryName(path);
			m_filesystemLevelFilename = System.IO.Path.GetFileName(path);
			m_fileSystem = new RawUserFileSystem(folder);
		}

		var stopWatch = new System.Diagnostics.Stopwatch();
		try {

			stopWatch.Start();
			using (var serializer = new OverloadLevelConvertSerializer(m_fileSystem.OpenFileStream(m_filesystemLevelFilename))) {

				// Magic number 'Rev1'
				uint magicNumber = serializer.SerializeIn_uint32();
				if (magicNumber != 0x52657631) {
					throw new SerializationFailureException();
				}

				// Version (4)
				const uint minSupportedVersion = 3;
				const uint expectedVersion = 4;
				uint version = serializer.SerializeIn_uint32();
				if (version < minSupportedVersion || version > expectedVersion) {
					UnityEngine.Debug.LogErrorFormat("User level file version is {0}, expected {1}", version, expectedVersion);
					throw new SerializationFailureException();
				}
				serializer.SetVersion(version);

				// Level type
				/*uint levelType = */
				serializer.SerializeIn_uint32();

				serializer.Context = this;
				//UInt16 lastPackedId = 0;

				while (m_isExecuting) {
					UInt16 packetId = serializer.SerializeIn_uint16();
					if (!m_commandPacketHandlers.ContainsKey(packetId)) {
						// TODO: Report error
						UnityEngine.Debug.LogError("Uh oh corrupt file");
					}

					Type packetHandlerType = m_commandPacketHandlers[packetId];
					ICmdPacket packetHandler = (ICmdPacket)Activator.CreateInstance(packetHandlerType);
					packetHandler.Serialize(serializer);
					packetHandler.Execute(this);
					//lastPackedId = packetId;

				}
				serializer.Context = null;
			}

			stopWatch.Stop();
			UnityEngine.Debug.LogFormat("USERLEVEL: Deserialization time: {0} ms", stopWatch.ElapsedMilliseconds);
		} catch(System.Exception ex) {
			UnityEngine.Debug.LogErrorFormat("USERLEVEL ERROR: {0}", ex.Message);

			// Start doing some cleanup
			if (m_postLoadCallbacks != null) {
				m_postLoadCallbacks.Clear();
			}

			// Call the normal post-load cleanup
			try {
				PostLoadCleanup();
			} catch {
			}

			// All of the GameObjects will be created under m_topLevelContainer
			// So, let's cleanup everything under that object
			try {
				UnityEngine.Transform[] allChildren = m_topLevelContainer.GetComponentsInChildren<UnityEngine.Transform>(true);
				if (allChildren != null) {
					for (int i = 0, cnt = allChildren.Length; i < cnt; ++i) {
						UnityEngine.GameObject.Destroy(allChildren[i].gameObject);
					}
				}

				UnityEngine.GameObject.Destroy(m_topLevelContainer);
				m_topLevelContainer = null;
			} catch {
			}

			// Destroy ourselves, this level is not good
			UnityEngine.GameObject.DestroyImmediate(this.gameObject);

			// Get out of here
			return;
		} finally {
			// Cleanup
			m_fileSystem.Dispose();
			m_fileSystem = null;
		}

		// Find all of the Reflection Probes and configure them to
		// be good (we can't use 'Baked' because they aren't)
		stopWatch.Reset();
		stopWatch.Start();

		int cullingMask = -1; // default enable all layers?
		cullingMask ^= (1 << (int)Overload.UnityObjectLayers.ENEMY_MESH); // Ignore enemies when rendering
		cullingMask ^= (1 << (int)Overload.UnityObjectLayers.ITEMS); // Ignore items when rendering
		cullingMask ^= (1 << (int)Overload.UnityObjectLayers.RP_IGNORE); // Ignore items when rendering
		cullingMask ^= (1 << (int)Overload.UnityObjectLayers.DOOR); // Ignore items when rendering

		foreach (var rp in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(UnityEngine.ReflectionProbe)) as UnityEngine.ReflectionProbe[]) {
			rp.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
			rp.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
			rp.intensity = 1.5f;
			rp.cullingMask = cullingMask;
		}
		stopWatch.Stop();
		UnityEngine.Debug.LogFormat("USERLEVEL: Reflection Probe fixup time: {0} ms", stopWatch.ElapsedMilliseconds);

		// Activate all the deferred objects
		stopWatch.Reset();
		stopWatch.Start();
		for (int i = m_objectsToActivate.Count - 1; i >= 0; --i) {
			m_objectsToActivate[i].SetActive(true);
		}
		m_topLevelContainer.SetActive(true);
		stopWatch.Stop();
		UnityEngine.Debug.LogFormat("USERLEVEL: Activation time: {0} ms", stopWatch.ElapsedMilliseconds);

		// Call the deferred callbacks
		stopWatch.Reset();
		stopWatch.Start();
		foreach (var action in m_postLoadCallbacks.ToArray()) {
			action();
		}
		stopWatch.Stop();
		UnityEngine.Debug.LogFormat("USERLEVEL: Deferred post-load callbacks time: {0} ms", stopWatch.ElapsedMilliseconds);

		// Cleanup after load
		PostLoadCleanup();
		UnityEngine.Debug.LogFormat("USERLEVEL: Cleanup of asset references complete");
	}

	public void ExecuteCmdPacket<TCmdPacket>(TCmdPacket packet)
	{
		// Call to the real processing function
		m_commandExecutor[typeof(TCmdPacket)].Invoke(this, new object[1] { packet });
	}

	private void ExecuteCmdDone(CmdDone cmd)
	{
		m_isExecuting = false;
	}

	private void ExecuteCmdCreateAssetFile(CmdCreateAssetFile cmd)
	{
	}

	private void ExecuteCmdAddAssetToAssetFile(CmdAddAssetToAssetFile cmd)
	{
		Type assetObjType = cmd.AssetObjectType;
		bool isScriptableObject = assetObjType.IsSubclassOf(typeof(UnityEngine.ScriptableObject));
		UnityEngine.Object resultObject;
		if (isScriptableObject) {
			// Object derived from ScriptableObject
			resultObject = UnityEngine.ScriptableObject.CreateInstance(assetObjType);
		} else {
			// Normal object
			resultObject = (UnityEngine.Object)Activator.CreateInstance(assetObjType);
		}

		// Register the object into our database by the AssetGuid
		m_assetDatabaseAssets.Add(cmd.AssetGuid, resultObject);
	}

	private void ExecuteCmdInitializeGameManager(CmdInitializeGameManager cmd)
	{
		var go = UnityEngine.GameObject.Find(cmd.gameManagerName);
		if (go != null) {
			// Assume already has a bound game manager prefab
			return;
		}

		((LevelData)m_componentMap[cmd.levelObjectInitializerComponentGuid]).m_game_manager_prefab = gameManagerPrefab;
	}

	private void ExecuteCmdCreateNewGameObject(CmdCreateNewGameObject cmd)
	{
		var go = new UnityEngine.GameObject();
		go.transform.parent = m_topLevelContainer.transform;

		m_gameObjectMap.Add(cmd.gameObjectGuid, go);
		m_componentMap.Add(cmd.transformGuid, go.transform);
	}

	private void ExecuteCmdTransformSetParent(CmdTransformSetParent cmd)
	{
		var thisTransform = (UnityEngine.Transform)m_componentMap[cmd.thisTransformGuid];
		if (cmd.newParentGuid == Guid.Empty) {
			thisTransform.parent = null;
		} else {
			var newParentTransform = (UnityEngine.Transform)m_componentMap[cmd.newParentGuid];
			thisTransform.parent = newParentTransform;
		}
	}

	private UnityEngine.GameObject LookupGameObjectOrInlinePrefab(Guid guid)
	{
		UnityEngine.GameObject go;
		if (m_gameObjectMap.TryGetValue(guid, out go)) {
			return go;
		}

		if (m_inlinePrefabs.TryGetValue(guid, out go)) {
			return go;
		}

		// TODO: Report an error
		return null;
	}

	private void ExecuteCmdGameObjectSetName(CmdGameObjectSetName cmd)
	{
		UnityEngine.GameObject go = LookupGameObjectOrInlinePrefab(cmd.thisGameObjectGuid);
		if (go == null) {
			return;
		}
		go.name = cmd.name;
	}

	private void ExecuteCmdGameObjectSetTag(CmdGameObjectSetTag cmd)
	{
		UnityEngine.GameObject go = LookupGameObjectOrInlinePrefab(cmd.thisGameObjectGuid);
		if (go == null) {
			return;
		}
		go.tag = cmd.tag;
	}

	private void ExecuteCmdGameObjectSetLayer(CmdGameObjectSetLayer cmd)
	{
		UnityEngine.GameObject go = LookupGameObjectOrInlinePrefab(cmd.thisGameObjectGuid);
		if (go == null) {
			return;
		}

		go.layer = cmd.layer;
	}

	private void ExecuteCmdGameObjectAddComponent(CmdGameObjectAddComponent cmd)
	{
		UnityEngine.GameObject go = LookupGameObjectOrInlinePrefab(cmd.thisGameObjectGuid);
		if (go == null) {
			return;
		}

		Type type = LookupType(cmd.componentTypeName);

		var comp = go.AddComponent(type) as UnityEngine.Component;

		m_componentMap.Add(cmd.newComponentGuid, comp);
	}

	static object ReconstituteSpecialPropertyValue<TSource, TDest>(CmdGameObjectSetComponentProperty.TargetType targetType, object propertyValue, System.Func<TSource, TDest> convert)
	{
		switch (targetType) {
			case CmdGameObjectSetComponentProperty.TargetType.Singular: {
					return convert((TSource)propertyValue);
				}
			case CmdGameObjectSetComponentProperty.TargetType.Array: {
					TSource[] asArray = (TSource[])propertyValue;
					return asArray.Select(x => convert(x)).ToArray();
				}
			case CmdGameObjectSetComponentProperty.TargetType.List: {
					TSource[] asArray = (TSource[])propertyValue;
					return asArray.Select(x => convert(x)).ToList();
				}
		}

		throw new System.Exception("Unexpected TargetType");
	}

	private object CheckAndUpConvertObjectArray(object sourceObject, Type destType)
	{
		var sourceObjectType = sourceObject.GetType();
		if (sourceObjectType.IsArray && destType.IsArray && sourceObjectType != destType && destType.GetElementType().IsSubclassOf(sourceObjectType.GetElementType())) {
			Array sourceObjectArray = (Array)sourceObject;
			int numElements = sourceObjectArray.Length;
			Type destElementType = destType.GetElementType();
			Array resolvedConvertedArray = Array.CreateInstance(destElementType, numElements);
			for (int ai = 0; ai < numElements; ++ai) {
				object origVal = sourceObjectArray.GetValue(ai);
				resolvedConvertedArray.SetValue(Convert.ChangeType(origVal, destElementType), ai);
			}
			return resolvedConvertedArray;
		}

		return sourceObject;
	}

	private void ExecuteCmdGameObjectSetComponentProperty(CmdGameObjectSetComponentProperty cmd)
	{
		UnityEngine.Component comp;
		if (!m_componentMap.TryGetValue(cmd.thisComponentGuid, out comp)) {
			UnityEngine.Debug.LogErrorFormat("Unable to resolve component {0}", cmd.thisComponentGuid);
			return;
		}

		if (comp == null) {
			// This component is the result of a call to GetComponent/GetComponentInChildren and
			// that component doesn't exist on this GameObject/prefab instance. No reason to do anything
			// else in here.
			return;
		}

		object resolvedValue = cmd.propertyValue;
		switch (cmd.propertyValueNamespace) {
			case CmdGameObjectSetComponentProperty.ValueNamespace.None:
				// Already resolved
				break;
			case CmdGameObjectSetComponentProperty.ValueNamespace.Asset:
			case (CmdGameObjectSetComponentProperty.ValueNamespace)2: { // For older versions when we used to consider 'Material' as a different namespace
					resolvedValue = ReconstituteSpecialPropertyValue<Guid, UnityEngine.Object>(cmd.targetType, cmd.propertyValue, x => m_assetDatabaseAssets[x]);
				}
				break;
			case CmdGameObjectSetComponentProperty.ValueNamespace.GameObject: {
					resolvedValue = ReconstituteSpecialPropertyValue<Guid, UnityEngine.GameObject>(cmd.targetType, cmd.propertyValue, x => {
						UnityEngine.GameObject go;
						if (m_gameObjectMap.TryGetValue(x, out go)) {
							return go;
						}
						// Fallback to inline prefab
						return m_inlinePrefabs[x];
					});
				}
				break;
			case CmdGameObjectSetComponentProperty.ValueNamespace.Component: {
					resolvedValue = ReconstituteSpecialPropertyValue<Guid, UnityEngine.Component>(cmd.targetType, cmd.propertyValue, x => m_componentMap[x]);
				}
				break;
		}

		string propertyName = cmd.propertyName;

		Type componentType = comp.GetType();
		var asProp = componentType.GetProperty(propertyName, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty);
		if (asProp != null) {
			var resolvedValueConverted = CheckAndUpConvertObjectArray(resolvedValue, asProp.PropertyType);
			asProp.SetValue(comp, resolvedValueConverted, null);
			return;
		}

		var asField = componentType.GetField(propertyName, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetField);
		if (asField != null) {
			var resolvedValueConverted = CheckAndUpConvertObjectArray(resolvedValue, asField.FieldType);
			asField.SetValue(comp, resolvedValueConverted);
			return;
		}

		// TODO: Report error
		throw new Exception(string.Format("Unable to set the property/field {0}", propertyName));
	}

	private void ExecuteCmdAssetRegisterMaterial(CmdAssetRegisterMaterial cmd)
	{
		UnityEngine.Material material = null;

		if (cmd.materialLookupName.StartsWith("$INTERNAL$:")) {
			string materialGuidStr = cmd.materialLookupName.Substring("$INTERNAL$:".Length);
			Guid materialGuid = new Guid(materialGuidStr);

			UnityEngine.Object materialObj;
			if (!m_assetDatabaseAssets.TryGetValue(materialGuid, out materialObj)) {
				// TODO: Report error
				// Create a new asset as a placeholder
				material = new UnityEngine.Material(UnityEngine.Shader.Find("Diffuse"));
				material.color = UnityEngine.Color.green;
			} else {
				material = (UnityEngine.Material)materialObj;
			}
		} else {
			string loadPath = ResourceDatabase.LookupMaterial(cmd.materialLookupName);
			material = UnityEngine.Resources.Load(loadPath, typeof(UnityEngine.Material)) as UnityEngine.Material;
			if (material == null) {
				if (cmd.materialLookupName.EndsWith("_diffuse", StringComparison.InvariantCultureIgnoreCase)) {
					// Some texture names have '_diffuse' at the end - remove and look for a material again
					var adjustedMaterialName = cmd.materialLookupName.Substring(0, cmd.materialLookupName.Length - "_diffuse".Length);
					loadPath = ResourceDatabase.LookupMaterial(adjustedMaterialName);
					material = UnityEngine.Resources.Load(loadPath, typeof(UnityEngine.Material)) as UnityEngine.Material;
				}

				if (material == null) {
					// TODO: Report error
					// Create a new asset as a placeholder
					material = new UnityEngine.Material(UnityEngine.Shader.Find("Diffuse"));
					material.color = UnityEngine.Color.green;
				}
			}
		}

		m_assetDatabaseAssets.Add(cmd.materialAssetGuid, material);
	}

	private void ExecuteCmdFindPrefabReference(CmdFindPrefabReference cmd)
	{
		UnityEngine.GameObject prefab;

		if (cmd.prefabName.StartsWith("$INTERNAL$:")) {
			// Special case handler for an inline defined prefab
			string prefabGuidStr = cmd.prefabName.Substring("$INTERNAL$:".Length);
			if (prefabGuidStr.Length != 36) {
				// TODO: Report error
				prefab = new UnityEngine.GameObject();
			} else {
				Guid prefabGuid = new Guid(prefabGuidStr);
				if (!m_inlinePrefabs.TryGetValue(prefabGuid, out prefab)) {
					// TODO: Report error
					prefab = new UnityEngine.GameObject();
				}
			}
		} else {
			string loadPath = ResourceDatabase.LookupPrefab(cmd.prefabName);
			prefab = UnityEngine.Resources.Load(loadPath, typeof(UnityEngine.GameObject)) as UnityEngine.GameObject;
			if (prefab == null) {
				// TODO: Report error
				prefab = new UnityEngine.GameObject();
			}
		}

		m_prefabSearchReferences.Add(cmd.prefabAssetReferenceGuid, prefab);
	}

	private void ExecuteCmdInstantiatePrefab(CmdInstantiatePrefab cmd)
	{
		UnityEngine.GameObject prefab;

		// First check to see if this is a prefab from a prefab search
		if (!m_prefabSearchReferences.TryGetValue(cmd.prefabAssetReferenceGuid, out prefab)) {
			// Now check the inline prefab references
			if (!m_inlinePrefabs.TryGetValue(cmd.prefabAssetReferenceGuid, out prefab)) {
				// No, so now check our generic asset database
				UnityEngine.Object genericObject;
				if (!m_assetDatabaseAssets.TryGetValue(cmd.prefabAssetReferenceGuid, out genericObject)) {
					// No idea what this is
					// TODO: Report error
					throw new Exception(string.Format("Unable to resolve prefab reference {0}", cmd.prefabAssetReferenceGuid.ToString()));
				}

				// Convert it to a prefab
				prefab = (UnityEngine.GameObject)genericObject;
			}
		}

#if !UNITY_EDITOR // It would be nice to do this in the Editor also, but it ends up checking out the prefab even though we don't make a change
        var wasPrefabActive = prefab.activeSelf;
        if (wasPrefabActive)
        {
            // Temporarily disable the prefab to prevent Awake from being called
            prefab.SetActive(false);
        }
#endif

		// Instantiate and register the new instance into our tables
		UnityEngine.GameObject go = UnityEngine.GameObject.Instantiate<UnityEngine.GameObject>(prefab);
		go.transform.parent = m_topLevelContainer.transform;
		// Furthermore, explicitly disable it in case the command stream later re-parents this outside of our top level container
		go.SetActive(false);
		m_objectsToActivate.Add(go);

#if !UNITY_EDITOR
        if (wasPrefabActive)
        {
            // Revert the change to the prefab
            prefab.SetActive(true);
        }
#endif

		m_gameObjectMap.Add(cmd.resultGameObjectGuid, go);
		m_componentMap.Add(cmd.resultGameObjectTransformGuid, go.transform);
	}

	private void ExecuteCmdGetComponentAtRuntime(CmdGetComponentAtRuntime cmd)
	{
		UnityEngine.GameObject go = LookupGameObjectOrInlinePrefab(cmd.gameObjectGuid);
		if (go == null) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find GameObject"));
		}

		Type componentTypeToFind = LookupType(cmd.componentName);

		UnityEngine.Component comp;
		if (cmd.includeChildren) {
			comp = go.GetComponentInChildren(componentTypeToFind);
		} else {
			comp = go.GetComponent(componentTypeToFind);
		}

		// Update the component lookup map
		m_componentMap.Add(cmd.searchResultGuid, comp);
	}

	private void ExecuteCmdSaveAsset(CmdSaveAsset cmd)
	{
		// Nothing to do here -- the work was serialized into an existing asset
	}

	private void ExecuteCmdLoadAssetBundle(CmdLoadAssetBundle cmd)
	{

#if UNITY_STANDALONE_WIN
		string platform = "windows";
#elif UNITY_STANDALONE_LINUX
		string platform = "linux";
#elif UNITY_STANDALONE_OSX
		string platform = "osx";
#else
		string platform = "unknown";
#endif

		string relativePathToBundle = System.IO.Path.Combine(System.IO.Path.Combine(cmd.relativeBundleFolder, platform), cmd.bundleName);
		using (var fs = m_fileSystem.OpenFileStreamToMemory(relativePathToBundle)) {
			if (fs == null) {
				// TODO: Report error
				throw new Exception(string.Format("Unable to find AssetBundle at: {0}", relativePathToBundle));
			}

			long bundleFileSize = fs.Length;
			byte[] bundleFileData = new byte[bundleFileSize];
			fs.Read(bundleFileData, 0, (int)bundleFileSize);

			// https://docs.unity3d.com/Manual/AssetBundles-Native.html
			var bundle = UnityEngine.AssetBundle.LoadFromMemory(bundleFileData);
			if (bundle == null) {
				// TODO: Report error
				throw new Exception(string.Format("Unable to load AssetBundle: {0}", relativePathToBundle));
			}

			m_loadedBundlesByGuid.Add(cmd.bundleRefGuid, bundle);
		}
	}

	private void ExecuteCmdLoadAssetFromAssetBundle(CmdLoadAssetFromAssetBundle cmd)
	{
		UnityEngine.AssetBundle bundle;
		if (!m_loadedBundlesByGuid.TryGetValue(cmd.bundleRefGuid, out bundle)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to reference AssetBundle: {0}", cmd.bundleRefGuid.ToString()));
		}

		UnityEngine.Object obj = bundle.LoadAsset(cmd.assetName);
		if (obj == null) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to load Asset ({0}) from AssetBundle {1}", cmd.assetName, cmd.bundleRefGuid.ToString()));
		}

		m_assetDatabaseAssets.Add(cmd.loadedAssetGuid, obj);
	}

	private void ExecuteCmdCreateMaterial(CmdCreateMaterial cmd)
	{
		UnityEngine.Object shaderObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.shaderGuid, out shaderObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Shader ({0})", cmd.shaderGuid.ToString()));
		}
		UnityEngine.Shader shader = (UnityEngine.Shader)shaderObj;

		var mat = new UnityEngine.Material(shader);
		mat.color = cmd.color;
		mat.enableInstancing = cmd.enableInstancing;
		mat.mainTextureOffset = cmd.mainTextureOffset;
		mat.mainTextureScale = cmd.mainTextureScale;
		if (!string.IsNullOrEmpty(cmd.name)) {
			mat.name = cmd.name;
		}
		mat.renderQueue = cmd.renderQueue;
		if (cmd.shaderKeywords != null) {
			mat.shaderKeywords = cmd.shaderKeywords;
		}

		if (cmd.mainTexture != Guid.Empty) {
			UnityEngine.Object textureObj;
			if (m_assetDatabaseAssets.TryGetValue(cmd.mainTexture, out textureObj)) {
				UnityEngine.Texture texture = (UnityEngine.Texture)textureObj;
				mat.mainTexture = texture;
			}
		}

		m_assetDatabaseAssets.Add(cmd.materialGuid, mat);
	}

	private void ExecuteCmdMaterialDisableKeyword(CmdMaterialDisableKeyword cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.DisableKeyword(cmd.keyword);
	}

	private void ExecuteCmdMaterialEnableKeyword(CmdMaterialEnableKeyword cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.EnableKeyword(cmd.keyword);
	}

	private void ExecuteCmdMaterialSetColor(CmdMaterialSetColor cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetColor(cmd.name, cmd.color);
	}

	private void ExecuteCmdMaterialSetColorArray(CmdMaterialSetColorArray cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetColorArray(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetFloat(CmdMaterialSetFloat cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetFloat(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetFloatArray(CmdMaterialSetFloatArray cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetFloatArray(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetInt(CmdMaterialSetInt cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetInt(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetMatrix(CmdMaterialSetMatrix cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetMatrix(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetMatrixArray(CmdMaterialSetMatrixArray cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetMatrixArray(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetOverrideTag(CmdMaterialSetOverrideTag cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetOverrideTag(cmd.tag, cmd.value);
	}

	private void ExecuteCmdMaterialSetTexture(CmdMaterialSetTexture cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		UnityEngine.Texture texture = null;
		UnityEngine.Object textureObj;
		if (m_assetDatabaseAssets.TryGetValue(cmd.value, out textureObj)) {
			texture = (UnityEngine.Texture)textureObj;
		}
		material.SetTexture(cmd.name, texture);
	}

	private void ExecuteCmdMaterialSetTextureOffset(CmdMaterialSetTextureOffset cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetTextureOffset(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetTextureScale(CmdMaterialSetTextureScale cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetTextureScale(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetVector(CmdMaterialSetVector cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetVector(cmd.name, cmd.value);
	}

	private void ExecuteCmdMaterialSetVectorArray(CmdMaterialSetVectorArray cmd)
	{
		UnityEngine.Object materialObj;
		if (!m_assetDatabaseAssets.TryGetValue(cmd.materialGuid, out materialObj)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Material ({0})", cmd.materialGuid.ToString()));
		}
		UnityEngine.Material material = (UnityEngine.Material)materialObj;

		material.SetVectorArray(cmd.name, cmd.value);
	}

	private void ExecuteCmdCreateTexture2D(CmdCreateTexture2D cmd)
	{
		m_assetDatabaseAssets.Add(cmd.texture2DGuid, cmd.texture);
	}

	private void ExecuteCmdLoadAssemblyFromAssetBundle(CmdLoadAssemblyFromAssetBundle cmd)
	{
		if (!AllowScriptAssemblies) {
			return;
		}

		UnityEngine.AssetBundle bundle;
		if (!m_loadedBundlesByGuid.TryGetValue(cmd.bundleRefGuid, out bundle)) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to reference AssetBundle: {0}", cmd.bundleRefGuid.ToString()));
		}

		UnityEngine.TextAsset txt = bundle.LoadAsset(cmd.assemblyAssetName, typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
		if (!txt) {
			// TODO: Report error
			throw new Exception(string.Format("Unable to find Assembly ({0}) in AssetBundle", cmd.assemblyAssetName));
		}

		// Retrieve the binary data as an array of bytes
		byte[] bytes = txt.bytes;

		// Calculate a SHA1 hash of the bytes
		string assemblyHash;
		using (var sha1 = new System.Security.Cryptography.SHA1Managed()) {
			var hash = sha1.ComputeHash(bytes);
			assemblyHash = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
		}

		System.Reflection.Assembly asm;
		if (s_allLoadedAssemblies.TryGetValue(assemblyHash, out asm)) {
			// The assembly is already loaded into the process
			m_loadedAssemblies[cmd.assemblyAssetName] = asm;
			return;
		}

		// Load the Assembly
		asm = System.Reflection.Assembly.Load(txt.bytes);
		s_allLoadedAssemblies.Add(assemblyHash, asm);
		m_loadedAssemblies[cmd.assemblyAssetName] = asm;
	}

	private void ExecuteCmdCreateInlinePrefab(CmdCreateInlinePrefab cmd)
	{
		var go = new UnityEngine.GameObject(string.Format("$INTERNAL$:{0}", cmd.prefabGuid.ToString()));
		go.SetActive(false);
		go.transform.parent = m_topLevelContainer.transform;
		m_inlinePrefabs.Add(cmd.prefabGuid, go);
		m_componentMap.Add(cmd.transformGuid, go.transform);
	}
}
