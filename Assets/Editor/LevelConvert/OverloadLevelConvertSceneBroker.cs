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
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace OverloadLevelExport
{
	public static class Extensions
	{
		//Extension method to find a child object by name
		public static GameObject GetChildByName(this GameObject go, string name)
		{
			if (!go) {
				return null;
			}

			var child = go.transform.Find(name);
			if (!child) {
				return null;
			}

			return child.gameObject;
		}
	}

	public class SceneBroker : ISceneBroker
	{
		private class ComponentBroker : IComponentBroker
		{
			private Component m_component;

#if !OVERLOAD_LEVEL_EDITOR
			public Component InternalObject
			{
				get { return m_component; }
			}
#endif

			public ComponentBroker(Component comp)
			{
				m_component = comp;
			}

			public IGameObjectBroker ownerGameObject
			{
				get { return new GameObjectBroker(m_component.gameObject); }
			}

			public void SetProperty<T>(string name, T value)
			{
				if (m_component == null) {
					// This is null for ComponentBrokers returned by GetComponent* where the component wasn't found
					return;
				}

				Type componentType = m_component.GetType();

				var asProp = componentType.GetProperty(name, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty);
				if (asProp != null) {
					asProp.SetValue(m_component, value, null);
					return;
				}

				var asField = componentType.GetField(name, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetField);
				if (asField != null) {
					asField.SetValue(m_component, value);
					return;
				}

				throw new Exception(string.Format("Unable to set the property/field {0}", name));
			}
		}

		private class TransformBroker : ComponentBroker, ITransformBroker
		{
			private GameObject m_gameObject;

			public TransformBroker(GameObject go)
				 : base(go.transform)
			{
				m_gameObject = go;
			}

			public ITransformBroker Parent
			{
				get { return m_gameObject.transform.parent == null ? null : new TransformBroker(m_gameObject.transform.parent.gameObject); }
				set { m_gameObject.transform.parent = value == null ? null : ((TransformBroker)value).m_gameObject.transform; }
			}

			public int ChildCount
			{
				get { return m_gameObject.transform.childCount; }
			}

			public Vector3 Position
			{
				get { return m_gameObject.transform.position; }
				set { m_gameObject.transform.position = value; }
			}

			public Quaternion Rotation
			{
				get { return m_gameObject.transform.rotation; }
				set { m_gameObject.transform.rotation = value; }
			}

			public Vector3 LocalPosition
			{
				get { return m_gameObject.transform.localPosition; }
				set { m_gameObject.transform.localPosition = value; }
			}

			public Quaternion LocalRotation
			{
				get { return m_gameObject.transform.localRotation; }
				set { m_gameObject.transform.localRotation = value; }
			}

			public ITransformBroker GetChild(int child)
			{
				return new TransformBroker(m_gameObject.transform.GetChild(child).gameObject);
			}
		}

		private class GameObjectBroker : IGameObjectBroker
		{
			private GameObject m_gameObject;

#if !OVERLOAD_LEVEL_EDITOR
			public GameObject InternalObject
			{
				get { return m_gameObject; }
			}
#endif

			public GameObjectBroker(GameObject go)
			{
				if (go == null) {
					Debug.LogErrorFormat("Setting a null GameObjectBroker");
				}
				m_gameObject = go;
			}

			public ITransformBroker Transform
			{
				get { return new TransformBroker(this.m_gameObject); }
			}

			public string Tag
			{
				get { return m_gameObject.tag; }
				set { m_gameObject.tag = value; }
			}

			public string Name
			{
				get { return m_gameObject.name; }
				set { m_gameObject.name = value; }
			}

			public int Layer
			{
				get { return m_gameObject.layer; }
				set { m_gameObject.layer = value; }
			}

			public bool ActiveInHierarchy
			{
				get { return m_gameObject.activeInHierarchy; }
			}

			public IGameObjectBroker GetChildByName(string name)
			{
				var go = m_gameObject.GetChildByName(name);
				if (go == null) {
					return null;
				}
				return new GameObjectBroker(go);
			}

			public IComponentBroker AddComponent(string componentTypeName)
			{
				Type type = SceneBroker.LookupType(componentTypeName);
				var comp = m_gameObject.AddComponent(type) as Component;
				if (comp == null) {
					return null;
				}
				return new ComponentBroker(comp);
			}

			public IComponentBroker[] GetComponentsInChildren(string componentTypeName, bool includeInactive)
			{
				Type type = SceneBroker.LookupType(componentTypeName);
				Component[] comps = m_gameObject.GetComponentsInChildren(type, includeInactive);
				if (comps == null) {
					return null;
				}
				return comps.Select(co => new ComponentBroker(co)).ToArray();
			}

			public IComponentBroker GetComponent(string componentTypeName)
			{
				Type type = SceneBroker.LookupType(componentTypeName);
				var comp = m_gameObject.GetComponent(type);
				if (comp == null) {
					// NOTE: We NEVER return a null ComponentBroker here because we need to keep it consistent
					// with the Overload Editor version
					return new ComponentBroker(null);
				}
				return new ComponentBroker(comp);
			}

			public IComponentBroker GetComponentInChildren(string componentTypeName)
			{
				Type type = SceneBroker.LookupType(componentTypeName);
				var comp = m_gameObject.GetComponentInChildren(type);
				if (comp == null) {
					// NOTE: We NEVER return a null ComponentBroker here because we need to keep it consistent
					// with the Overload Editor version
					return new ComponentBroker(null);
				}
				return new ComponentBroker(comp);
			}
		}

		public static Type GetTypeByName(string className)
		{
			var returnVal = new System.Collections.Generic.List<Type>();

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
					Type[] assemblyTypes = a.GetTypes();
					for (int j = 0; j < assemblyTypes.Length; j++) {
						if (assemblyTypes[j].Name == className) {
							returnVal.Add(assemblyTypes[j]);
						}
					}
				}
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
		private static System.Collections.Generic.Dictionary<string, Type> s_typeLookup = new System.Collections.Generic.Dictionary<string, Type>();

		public static Type LookupType(string type)
		{
			Type res;
			if (s_typeLookup.TryGetValue(type, out res)) {
				return res;
			}

			Type choiceType = GetTypeByName(type);
			if (choiceType == null) {
				return null;
			}

			s_typeLookup.Add(type, choiceType);
			return choiceType;
		}

		public SceneBroker(LevelType exportLevelType)
		{
			this.LevelExportType = exportLevelType;
		}

		public LevelType LevelExportType
		{
			get;
			private set;
		}

		public IGameObjectBroker CreateRootGameObject(string name = "")
		{
			var go = new GameObject(name);
			return new GameObjectBroker(go);
		}

		public IGameObjectBroker InstantiatePrefab(IGameObjectBroker prefabObject)
		{
#if !OVERLOAD_LEVEL_EDITOR
			var go = UnityEditor.PrefabUtility.InstantiatePrefab(prefabObject.InternalObject) as GameObject;
			if (!go) {
				return null;
			}
			return new GameObjectBroker(go);
#else
            return null;
#endif
		}

		public void DestroyGameObject(IGameObjectBroker obj)
		{
#if !OVERLOAD_LEVEL_EDITOR
			GameObject.DestroyImmediate(obj.InternalObject);
#endif
		}

		public void DestroyComponent(IComponentBroker comp)
		{
#if !OVERLOAD_LEVEL_EDITOR
			UnityEngine.Object.DestroyImmediate(comp.InternalObject);
#endif
		}

		public IGameObjectBroker FindGameObject(string name)
		{
			var go = UnityEngine.GameObject.Find(name);
			if (go == null) {
				return null;
			}
			return new GameObjectBroker(go);
		}

		public IGameObjectBroker[] FindGameObjectsWithTag(string tag)
		{
			var gos = GameObject.FindGameObjectsWithTag(tag);
			if (gos == null) {
				return null;
			}

			return gos.Select(go => new GameObjectBroker(go)).ToArray();
		}

		public void InitializeGameManager(string gm_name, IComponentBroker levelObjectInitializer)
		{
			IGameObjectBroker gms = FindGameObject(gm_name);
			if (gms != null) {
				// Assume already has a bound game manager prefab
				return;
			}

			var prefab_match_guids = AssetDatabase.FindAssets(gm_name + " t:GameObject");
			if (prefab_match_guids == null || prefab_match_guids.Length == 0) {
				return;
			}

			var entity_prefab_path = AssetDatabase.GUIDToAssetPath(prefab_match_guids[0]);
			var gm_prefab = AssetDatabase.LoadAssetAtPath(entity_prefab_path, typeof(GameObject)) as GameObject;
#if !OVERLOAD_LEVEL_EDITOR
			((LevelData)levelObjectInitializer.InternalObject).m_game_manager_prefab = gm_prefab;
#endif
		}

		public IGameObjectBroker FindAndLoadPrefabAsset(string prefabName)
		{
			if (string.Compare(prefabName, "entity_item_hunter", true) == 0) {
				prefabName = "entity_item_hunter4pack";
			} else if (string.Compare(prefabName, "entity_item_falcon", true) == 0) {
				prefabName = "entity_item_falcon4pack";
			}

			var prefabMatchGUIDs = AssetDatabase.FindAssets(prefabName + " t:GameObject");
			if (prefabMatchGUIDs == null || prefabMatchGUIDs.Length == 0) {
				return null;
			}

			string prefabPath = null;
			foreach (string guid in prefabMatchGUIDs) {
				var asset_path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.Compare(System.IO.Path.GetFileNameWithoutExtension(asset_path), prefabName, true) == 0) {
					prefabPath = asset_path;
					break;
				}
			}
			if (prefabPath == null) {
				return null;
			}

			var prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			return new GameObjectBroker(prefabAsset);
		}
		public void AssetDatabase_StartAssetEditing()
		{
			AssetDatabase.StartAssetEditing();
		}
		public void AssetDatabase_StopAssetEditing()
		{
			AssetDatabase.StopAssetEditing();
		}
		public void AssetDatabase_SaveAssets()
		{
			AssetDatabase.SaveAssets();
		}
		public void AssetDatabase_Refresh()
		{
			AssetDatabase.Refresh();
		}
		public void AssetDatabase_ImportAsset(string path)
		{
			AssetDatabase.ImportAsset(path);
		}
		public void AssetDatabase_CreateAsset(UnityEngine.Object obj, string path)
		{
			AssetDatabase.CreateAsset(obj, path);
		}
		public void AssetDatabase_AddObjectToAsset(UnityEngine.Object obj, UnityEngine.Object asset)
		{
			AssetDatabase.AddObjectToAsset(obj, asset);
		}
	}
}
