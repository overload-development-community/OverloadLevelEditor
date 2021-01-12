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
using Overload;
using System.Runtime.InteropServices;

namespace OverloadLevelExport
{
	public class SceneBroker : ISceneBroker, IDisposable
	{
		#region IDisposable
		bool m_disposed = false;

		~SceneBroker()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (m_disposed)
				return;

			if (disposing) {
				// Free any managed objects here.
				//
				if (m_physics != null) {
					m_physics.Dispose();
					m_physics = null;
				}
			}

			// Free any unmanaged objects here.
			//
			m_disposed = true;
		}

		public void Dispose()
		{
			// Dispose of unmanaged resources.
			Dispose(true);
			// Suppress finalization.
			GC.SuppressFinalize(this);
		}
		#endregion

		Dictionary<Guid, Dictionary<string, List<Guid>>> m_gameObjectUniverseWithComponents = new Dictionary<Guid, Dictionary<string, List<Guid>>>();
		Dictionary<Guid, Guid> m_componentToParentGameObject = new Dictionary<Guid, Guid>();
		Dictionary<Guid, string> m_componentToType = new Dictionary<Guid, string>();
		Dictionary<Guid, Dictionary<string, object>> m_gameObjectProperties = new Dictionary<Guid, Dictionary<string, object>>();
		Dictionary<Guid, Dictionary<string, object>> m_componentProperties = new Dictionary<Guid, Dictionary<string, object>>();
		Dictionary<Guid, List<Guid>> m_childLinksByParentTransformComponent = new Dictionary<Guid, List<Guid>>();
		Dictionary<Guid, List<KeyValuePair<Guid, UnityEngine.Object>>> m_assetDatabaseObjects = new Dictionary<Guid, List<KeyValuePair<Guid, UnityEngine.Object>>>();
		Dictionary<string, Guid> m_assetPathToAssetDatabaseFileGuid = new Dictionary<string, Guid>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<UnityEngine.Object, Guid> m_assetObjectToAssetDatabaseFileGuid = new Dictionary<UnityEngine.Object, Guid>();
		Dictionary<object, Guid> m_assetObjectToAssetGuid = new Dictionary<object, Guid>();
		Dictionary<UnityEngine.Material, Guid> m_assetMaterials = new Dictionary<UnityEngine.Material, Guid>();
		List<UnityEngine.Object> m_assetsToSerializeOnSave = new List<UnityEngine.Object>();
		OverloadLevelConvertSerializer m_serializer;
		PhysicsScene m_physics = null;

		public static SceneBroker ActiveSceneBrokerInstance = null;

		public SceneBroker(OverloadLevelConvertSerializer serializer, LevelType levelType)
		{
			this.LevelExportType = levelType;
			Serializers.RegisterSerializers();

			System.Diagnostics.Debug.Assert(serializer.IsWriting);
			m_serializer = serializer;
		}

		public LevelType LevelExportType
		{
			get;
			private set;
		}

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern int editor_physics_initialize();

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern void editor_physics_shutdown();

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern void editor_deregister_all_trianglemeshes();

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern void editor_deregister_trianglemesh(UInt32 id);

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern UInt32 editor_register_trianglemesh(UInt32 numVerts, [In] float[] verts, UInt32 numTris, [In] UInt32[] indices, UInt32 layerMask);

		[DllImport("EditorPhysics.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern int editor_do_linecast(float ox, float oy, float oz, float dx, float dy, float dz, float max_dist, UInt32 layerMask);

		public class PhysicsScene : IDisposable
		{
			public PhysicsScene(UnityEngine.Vector3 _worldMin, UnityEngine.Vector3 _worldMax)
			{
				editor_physics_initialize();
			}

			~PhysicsScene()
			{
				Dispose(false);
			}

			bool m_disposed = false;
			protected virtual void Dispose(bool disposing)
			{
				if (m_disposed)
					return;

				if (disposing) {
					// Free any managed objects here.
					//
				}

				// Free any unmanaged objects here.
				//
				editor_physics_shutdown();
				m_disposed = true;
			}

			public void Dispose()
			{
				// Dispose of unmanaged resources.
				Dispose(true);

				// Suppress finalization.
				GC.SuppressFinalize(this);
			}

			public void AddCollisionMesh(UnityEngine.Mesh mesh, int layer)
			{
				int meshNumSubmeshes = mesh.subMeshCount;
				List<uint> meshIndices = new List<uint>();
				for (int i = 0; i < meshNumSubmeshes; ++i) {
					meshIndices.AddRange(mesh.GetTriangles(i).Select(id => (uint)id));
				}
				uint[] indices = meshIndices.ToArray();

				var meshVerts = mesh.vertices;
				float[] verts = new float[meshVerts.Length * 3];
				for (int i = 0; i < meshVerts.Length; ++i) {
					var v = meshVerts[i];
					int baseI = i * 3;
					verts[baseI + 0] = v.x;
					verts[baseI + 1] = v.y;
					verts[baseI + 2] = v.z;
				}

				uint layerMask = 1U << layer;
				editor_register_trianglemesh((uint)meshVerts.Length, verts, (uint)indices.Length / 3, indices, layerMask);
			}

			public uint AddMultiCollisionMesh(int layer, UnityEngine.Mesh[] meshes)
			{
				int numMeshes = meshes.Length;

				List<float> allMeshVerts = new List<float>();
				List<uint> meshIndices = new List<uint>();
				uint meshVertOffset = 0;
				for (int i = 0; i < numMeshes; ++i) {
					meshVertOffset = (uint)allMeshVerts.Count / 3;
					allMeshVerts.AddRange(meshes[i].vertices.SelectMany(v => new float[] { v.x, v.y, v.z }));

					int smCount = meshes[i].subMeshCount;
					for (int sm = 0; sm < smCount; ++sm) {
						int[] tris = meshes[i].GetTriangles(sm);
						meshIndices.AddRange(tris.Select(idx => (uint)idx + meshVertOffset));
					}
				}

				uint layerMask = 1U << layer;
				return editor_register_trianglemesh((uint)allMeshVerts.Count / 3, allMeshVerts.ToArray(), (uint)meshIndices.Count / 3, meshIndices.ToArray(), layerMask);
			}

			public bool Linecast(UnityEngine.Vector3 p1, UnityEngine.Vector3 p2, int layerMask)
			{
				var dir = p2 - p1;
				float dirLen = dir.magnitude;
				dir.Normalize();
				bool hasHit = OverloadLevelExport.SceneBroker.editor_do_linecast(p1.x, p1.y, p1.z, dir.x, dir.y, dir.z, dirLen, (UInt32)layerMask) != 0;
				return hasHit;
			}
		}

		public PhysicsScene GetPhysicsScene()
		{
			if (m_physics == null) {
				m_physics = GeneratePhysicsScene();
			}
			return m_physics;
		}

		public void MarkPhysicsSceneDirty()
		{
			if (m_physics != null) {
				m_physics.Dispose();
				m_physics = null;
			}
		}

		PhysicsScene GeneratePhysicsScene()
		{
			Action<IGameObjectBroker> processChildHierarchy = null;

			List<Tuple<int, UnityEngine.Mesh>> collisionList = new List<Tuple<int, UnityEngine.Mesh>>();
			UnityEngine.Vector3 worldMinAABB = new UnityEngine.Vector3(System.Single.MaxValue, System.Single.MaxValue, System.Single.MaxValue);
			UnityEngine.Vector3 worldMaxAABB = new UnityEngine.Vector3(-worldMinAABB.x, -worldMinAABB.y, -worldMinAABB.z);

			processChildHierarchy = (go) => {
				// Look for a mesh collider
				var meshColliderComponent = go.GetComponentOnlyValidInExport("MeshCollider");
				if (meshColliderComponent != null) {
					// There is a mesh collider on this object
					bool isTrigger;
					if (!meshColliderComponent.TryGetPropertyDuringExport<bool>("isTrigger", out isTrigger) || isTrigger == false) {
						// Not a trigger, get the mesh
						UnityEngine.Mesh mesh;
						if (meshColliderComponent.TryGetPropertyDuringExport<UnityEngine.Mesh>("sharedMesh", out mesh)) {
							collisionList.Add(new Tuple<int, UnityEngine.Mesh>(go.Layer, mesh));
							var meshMin = mesh.bounds.min;
							var meshMax = mesh.bounds.max;
							worldMinAABB = UnityEngine.Vector3.Min(worldMinAABB, meshMin);
							worldMaxAABB = UnityEngine.Vector3.Max(worldMaxAABB, meshMax);
						};
					}
				}

				// Dive into the children
				var transform = go.Transform;
				int numChildren = transform.ChildCount;
				for (int child = 0; child < numChildren; ++child) {
					var childTransform = transform.GetChild(child);
					var childGo = childTransform.ownerGameObject;
					processChildHierarchy(childGo);
				}
			};
			
			// Collect all of the collision meshes
			var rootLevelGOs = GetRootGameObjects();
			foreach (var go in rootLevelGOs) {
				processChildHierarchy(go);
			}

			PhysicsScene scene = new PhysicsScene(worldMinAABB, worldMaxAABB);

			//var bodies = collisionList.GroupBy(x => x.Item1)
			//	.Select(group => {
			//		return scene.AddMultiCollisionMesh(group.Key, group.Select(x => x.Item2).ToArray());
			//	})
			//	.ToArray();

			foreach (var datum in collisionList) {
				scene.AddCollisionMesh(datum.Item2, datum.Item1);
			}

			return scene;
		}

		Guid GetComponentParentGameObject(Guid componentUID)
		{
			Guid uid;
			if (!m_componentToParentGameObject.TryGetValue(componentUID, out uid)) {
				return Guid.Empty;
			}
			return uid;
		}

		Guid[] LookupGameObjectComponentsByType(Guid gameObjectUID, string typeName)
		{
			Dictionary<string, List<Guid>> componentMap;
			if (!m_gameObjectUniverseWithComponents.TryGetValue(gameObjectUID, out componentMap)) {
				return null;
			}

			List<Guid> guids;
			if (!componentMap.TryGetValue(typeName, out guids)) {
				return new Guid[0];
			}

			return guids.ToArray();
		}

		enum CreateGameObjectSerializeMode
		{
			NoCommand,
			CreateNewGameObject,
			CreateInlinePrefab,
		}

		Guid CreateNewGameObject(CreateGameObjectSerializeMode mode, Guid uidToUse)
		{
			if (mode != CreateGameObjectSerializeMode.CreateInlinePrefab) {
				MarkPhysicsSceneDirty();
			}

			Guid uid = uidToUse;
			Guid transform = Guid.NewGuid();
			if (uid == Guid.Empty) {
				uid = Guid.NewGuid();
			}

			List<Guid> components = new List<Guid>();
			components.Add(transform);
			Dictionary<string, List<Guid>> componentMap = new Dictionary<string, List<Guid>>();
			componentMap.Add("Transform", components);
			var transformProps = new Dictionary<string, object>();

			transformProps["position"] = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
			transformProps["rotation"] = new UnityEngine.Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

			m_gameObjectUniverseWithComponents.Add(uid, componentMap);
			m_gameObjectProperties.Add(uid, new Dictionary<string, object>());
			m_componentToParentGameObject.Add(transform, uid);
			m_componentProperties.Add(transform, transformProps);
			m_componentToType.Add(transform, "Transform");
			m_childLinksByParentTransformComponent.Add(transform, new List<Guid>());

			if (mode == CreateGameObjectSerializeMode.CreateNewGameObject) {
				var cmd = new CmdCreateNewGameObject();
				cmd.gameObjectGuid = uid;
				cmd.transformGuid = transform;
				cmd.Serialize(m_serializer);
			} else if (mode == CreateGameObjectSerializeMode.CreateInlinePrefab) {
				var cmd = new CmdCreateInlinePrefab();
				cmd.prefabGuid = uid;
				cmd.transformGuid = transform;
				cmd.Serialize(m_serializer);
			}

			return uid;
		}

		Guid CreateComponentOnGameObject(Guid gameObjectUID, string typeName)
		{
			MarkPhysicsSceneDirty();

			Dictionary<string, List<Guid>> componentMap;
			if (!m_gameObjectUniverseWithComponents.TryGetValue(gameObjectUID, out componentMap)) {
				throw new Exception("GameObject is invalid");
			}

			List<Guid> guids;
			if (!componentMap.TryGetValue(typeName, out guids)) {
				guids = new List<Guid>();
				componentMap.Add(typeName, guids);
			}

			Guid uid = Guid.NewGuid();
			guids.Add(uid);

			m_componentToParentGameObject.Add(uid, gameObjectUID);
			m_componentProperties.Add(uid, new Dictionary<string, object>());
			m_componentToType.Add(uid, typeName);

			var cmd = new CmdGameObjectAddComponent();
			cmd.thisGameObjectGuid = gameObjectUID;
			cmd.newComponentGuid = uid;
			cmd.componentTypeName = typeName;
			cmd.Serialize(m_serializer);

			return uid;
		}

		public Guid RegisterMaterialIfNeeded(UnityEngine.Material mat)
		{
			Guid guid;
			if (m_assetMaterials.TryGetValue(mat, out guid)) {
				return guid;
			}

			guid = Guid.NewGuid();
			m_assetMaterials.Add(mat, guid);

			var cmd = new CmdAssetRegisterMaterial();
			cmd.materialAssetGuid = guid;
			cmd.geometryType = (int)mat.m_geomType;
			cmd.materialLookupName = mat.m_materialLookupName;
			cmd.Serialize(m_serializer);

			return guid;
		}


		void DeleteComponent(Guid componentUid)
		{
			Guid parentGameObjectUid;
			if (!m_componentToParentGameObject.TryGetValue(componentUid, out parentGameObjectUid)) {
				return;
			}

			m_componentToParentGameObject.Remove(componentUid);
			m_componentProperties.Remove(componentUid);
			m_componentToType.Remove(componentUid);

			Dictionary<string, List<Guid>> componentMap = m_gameObjectUniverseWithComponents[parentGameObjectUid];
			foreach (var kvp in componentMap) {
				List<Guid> guids = kvp.Value;
				guids.Remove(componentUid);
			}

			MarkPhysicsSceneDirty();
		}

		void DeleteGameObject(Guid gameObjectUid)
		{
			// Make sure it is a valid GameObject
			Dictionary<string, List<Guid>> componentMap;
			if (!m_gameObjectUniverseWithComponents.TryGetValue(gameObjectUid, out componentMap)) {
				return;
			}

			MarkPhysicsSceneDirty();

			// Unlink all of the children
			Guid transformUID = componentMap["Transform"][0];
			foreach (Guid childTransformUID in m_childLinksByParentTransformComponent[transformUID]) {
				UnlinkTransformComponents(transformUID, childTransformUID);
			}

			// Unlink from the parent
			object parentTransformObj;
			if (m_gameObjectProperties[gameObjectUid].TryGetValue("parent", out parentTransformObj)) {
				Guid parentTransformUid = (Guid)parentTransformObj;
				if (parentTransformUid != Guid.Empty) {
					UnlinkTransformComponents(parentTransformUid, transformUID);
				}
			}

			// Get the list of components we'll need to remove
			List<Guid> allComponents = new List<Guid>();
			foreach (var kvp in componentMap) {
				allComponents.AddRange(kvp.Value);
			}

			// Cleanup the GameObject
			m_gameObjectUniverseWithComponents.Remove(gameObjectUid);
			m_gameObjectProperties.Remove(gameObjectUid);

			// Cleanup the Components
			foreach (var componentUid in allComponents) {
				m_componentToParentGameObject.Remove(componentUid);
				m_componentProperties.Remove(componentUid);
				m_componentToType.Remove(componentUid);
			}
		}

		Dictionary<string, object> GetGameObjectProperties(Guid gameObjectUid)
		{
			Dictionary<string, object> props;
			if (!m_gameObjectProperties.TryGetValue(gameObjectUid, out props)) {
				throw new Exception("Unknown GameObject");
			}
			return props;
		}

		Dictionary<string, object> GetComponentProperties(Guid componentUid)
		{
			Dictionary<string, object> props;
			if (!m_componentProperties.TryGetValue(componentUid, out props)) {
				throw new Exception("Unknown Component");
			}
			return props;
		}

		void LinkTransformComponents(Guid parentUID, Guid childUID)
		{
			MarkPhysicsSceneDirty();
			List<Guid> childs = m_childLinksByParentTransformComponent[parentUID];
			if (!childs.Contains(childUID)) {
				childs.Add(childUID);
			}
		}

		void UnlinkTransformComponents(Guid parentUID, Guid childUID)
		{
			MarkPhysicsSceneDirty();
			List<Guid> childs = m_childLinksByParentTransformComponent[parentUID];
			childs.Remove(childUID);
		}

		private class ComponentBroker : IComponentBroker
		{
			protected Guid m_componentUid;
			protected SceneBroker m_parentScene;
			protected bool m_isVirtual;

			public Guid InternalUID
			{
				get { return m_componentUid; }
			}

			public bool IsVirtual
			{
				get { return m_isVirtual; }
			}

			public ComponentBroker(SceneBroker scene, Guid componentUid, bool isVirtual)
			{
				m_componentUid = componentUid;
				m_parentScene = scene;
				m_isVirtual = isVirtual;
			}

			public IGameObjectBroker ownerGameObject
			{
				get { return new GameObjectBroker(m_parentScene, m_parentScene.GetComponentParentGameObject(m_componentUid)); }
			}

			static object PrepSpecialPropertyValue<TSource,TDest>(object value, System.Type originalType, System.Func<TSource,TDest> convert, out CmdGameObjectSetComponentProperty.TargetType tt)
			{
				if (originalType.IsArray) {
					tt = CmdGameObjectSetComponentProperty.TargetType.Array;
					TSource[] sourceValueArray = (TSource[])value;
					TDest[] outputValue = new TDest[sourceValueArray.Length];
					for (int i = 0; i < sourceValueArray.Length; ++i) {
						outputValue[i] = convert(sourceValueArray[i]);
					}
					return outputValue;
				}

				if (originalType.IsGenericType && originalType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)) {
					tt = CmdGameObjectSetComponentProperty.TargetType.List;
					List<TSource> sourceValueList = (List<TSource>)value;
					TDest[] outputValue = new TDest[sourceValueList.Count];
					for (int i = 0; i < sourceValueList.Count; ++i) {
						outputValue[i] = convert(sourceValueList[i]);
					}
					return outputValue;
				}

				tt = CmdGameObjectSetComponentProperty.TargetType.Singular;
				return convert((TSource)value);
			}

			public bool TryGetPropertyDuringExport<T>(string name, out T result)
			{
				if (m_isVirtual) {
					result = default(T);
					return false;
				}

				Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
				if (properties == null) {
					result = default(T);
					return false;
				}

				object resultObj;
				if( !properties.TryGetValue( name, out resultObj ) ) {
					result = default(T);
					return false;
				}

				result = (T)resultObj;
				return true;
			}

			public void SetProperty<T>(string name, T value)
			{
				m_parentScene.MarkPhysicsSceneDirty();

				if (!m_isVirtual) {
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
					properties[name] = value;
				}

				CmdGameObjectSetComponentProperty.ValueNamespace ns = CmdGameObjectSetComponentProperty.ValueNamespace.None;
				CmdGameObjectSetComponentProperty.TargetType tt = CmdGameObjectSetComponentProperty.TargetType.Singular;

				Type originalType = typeof(T);
				Type elementType = originalType;
				if (elementType.IsArray) {
					elementType = elementType.GetElementType();
				} else if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)) {
					elementType = elementType.GetGenericArguments()[0];
				}

				object valueToWrite = value;
				if (elementType == typeof(UnityEngine.Mesh) || elementType == typeof(LevelGeometry)) {
					// We have an asset reference
					ns = CmdGameObjectSetComponentProperty.ValueNamespace.Asset;
					valueToWrite = PrepSpecialPropertyValue<object, Guid>(value, originalType, (object asset) => {
						Guid assetGuid;
						if (m_parentScene.m_assetObjectToAssetGuid.TryGetValue(asset, out assetGuid)) {
							return assetGuid;
						}
						throw new SerializationFailureException();
					}, out tt);
				} else if (elementType == typeof(UnityEngine.Material)) {
					// We have a Material reference
					ns = CmdGameObjectSetComponentProperty.ValueNamespace.Asset;
					valueToWrite = PrepSpecialPropertyValue<UnityEngine.Material, Guid>(value, originalType, mat => m_parentScene.RegisterMaterialIfNeeded(mat), out tt);
				} else if (elementType == typeof(IGameObjectBroker) || elementType.IsSubclassOf(typeof(IGameObjectBroker))) {
					// We have some form of GameObject reference
					ns = CmdGameObjectSetComponentProperty.ValueNamespace.GameObject;
					valueToWrite = PrepSpecialPropertyValue<IGameObjectBroker, Guid>(value, originalType, x => x.InternalUID, out tt);
				} else if (elementType == typeof(IComponentBroker) || elementType == typeof(ITransformBroker) || elementType.IsSubclassOf(typeof(IComponentBroker))) {
					// We have some form of Component reference
					ns = CmdGameObjectSetComponentProperty.ValueNamespace.Component;
					valueToWrite = PrepSpecialPropertyValue<IComponentBroker, Guid>(value, originalType, x => x.InternalUID, out tt);
				} else if (OverloadLevelConvertSerializer.HasRegisteredSerializer(elementType)) {
					// We'll be able to serialize this with a registered serializer
				} else if (!value.GetType().IsValueType && value.GetType() != typeof(string)) {
					throw new SerializationFailureException();
				}

				var cmd = new CmdGameObjectSetComponentProperty();
				cmd.thisComponentGuid = m_componentUid;
				cmd.propertyName = name;
				cmd.propertyValue = valueToWrite;
				cmd.targetType = tt;
				cmd.propertyValueNamespace = ns;
				cmd.Serialize(m_parentScene.m_serializer);
			}
		}

		private class TransformBroker : ComponentBroker, ITransformBroker
		{
			public TransformBroker(SceneBroker scene, Guid transformComponentUid)
				 : base(scene, transformComponentUid, false)
			{
			}

			public ITransformBroker Parent
			{
				get
				{
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);

					object parentProp;
					if (!properties.TryGetValue("parent", out parentProp)) {
						parentProp = Guid.Empty;
					}

					Guid parentTransformUID = (Guid)parentProp;
					if (parentTransformUID == Guid.Empty) {
						return null;
					}

					return new TransformBroker(m_parentScene, parentTransformUID);
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();

					var cmd = new CmdTransformSetParent();
					cmd.thisTransformGuid = m_componentUid;

					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);

					object oldParent;
					if (properties.TryGetValue("parent", out oldParent)) {
						Guid oldParentUID = (Guid)oldParent;
						if (oldParentUID != Guid.Empty) {
							m_parentScene.UnlinkTransformComponents(oldParentUID, m_componentUid);
						}
					}

					if (value == null) {
						properties["parent"] = Guid.Empty;
						cmd.newParentGuid = Guid.Empty;
					} else {
						Guid newParentUID = ((TransformBroker)value).m_componentUid;
						cmd.newParentGuid = newParentUID;
						properties["parent"] = newParentUID;
						m_parentScene.LinkTransformComponents(newParentUID, m_componentUid);
					}

					cmd.Serialize(m_parentScene.m_serializer);
				}
			}

			public int ChildCount
			{
				get { return m_parentScene.m_childLinksByParentTransformComponent[m_componentUid].Count; }
			}

			public UnityEngine.Vector3 Position
			{
				get
				{
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
					return (UnityEngine.Vector3)properties["position"];
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();
					SetProperty<UnityEngine.Vector3>("position", value);
				}
			}

			public UnityEngine.Quaternion Rotation
			{
				get
				{
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
					return (UnityEngine.Quaternion)properties["rotation"];
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();
					SetProperty<UnityEngine.Quaternion>("rotation", value);
				}
			}

			public UnityEngine.Vector3 LocalPosition
			{
				get
				{
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
					return (UnityEngine.Vector3)properties["localPosition"];
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();
					SetProperty<UnityEngine.Vector3>("localPosition", value);
				}
			}

			public UnityEngine.Quaternion LocalRotation
			{
				get
				{
					Dictionary<string, object> properties = m_parentScene.GetComponentProperties(m_componentUid);
					return (UnityEngine.Quaternion)properties["localRotation"];
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();
					SetProperty<UnityEngine.Quaternion>("localRotation", value);
				}
			}

			public ITransformBroker GetChild(int child)
			{
				List<Guid> childs = m_parentScene.m_childLinksByParentTransformComponent[m_componentUid];
				if (child < 0 || child >= childs.Count) {
					throw new Exception("Out of bounds getting a child");
				}

				Guid childTransformUID = childs[child];
				return new TransformBroker(m_parentScene, childTransformUID);
			}
		}

		private class GameObjectBroker : IGameObjectBroker
		{
			private Guid m_gameObjectUID;
			private Guid m_transformUID; // every GameObject will have a transform
			private SceneBroker m_parentScene;

			public Guid InternalUID
			{
				get { return m_gameObjectUID; }
			}

			public GameObjectBroker(SceneBroker scene, Guid uid)
			{
				m_parentScene = scene;
				m_gameObjectUID = uid;
				m_transformUID = scene.LookupGameObjectComponentsByType(uid, "Transform")[0];
			}

			public GameObjectBroker(SceneBroker scene, Guid uid, Guid transformUid)
			{
				m_parentScene = scene;
				m_gameObjectUID = uid;
				m_transformUID = transformUid;
			}

			public ITransformBroker Transform
			{
				get { return new TransformBroker(this.m_parentScene, this.m_transformUID); }
			}

			public string Tag
			{
				get
				{
					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);

					object val;
					if (!props.TryGetValue("tag", out val)) {
						return string.Empty;
					}
					return (string)val;
				}

				set
				{
					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);
					props["tag"] = value;

					var cmd = new CmdGameObjectSetTag();
					cmd.thisGameObjectGuid = this.m_gameObjectUID;
					cmd.tag = value;
					cmd.Serialize(m_parentScene.m_serializer);
				}
			}

			public string Name
			{
				get
				{
					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);

					object val;
					if (!props.TryGetValue("name", out val)) {
						return string.Empty;
					}
					return (string)val;
				}

				set
				{
					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);
					props["name"] = value;

					var cmd = new CmdGameObjectSetName();
					cmd.thisGameObjectGuid = this.m_gameObjectUID;
					cmd.name = value;
					cmd.Serialize(m_parentScene.m_serializer);
				}
			}

			public int Layer
			{
				get
				{
					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);

					object val;
					if (!props.TryGetValue("layer", out val)) {
						return 0;
					}
					return (int)val;
				}

				set
				{
					m_parentScene.MarkPhysicsSceneDirty();

					var props = m_parentScene.GetGameObjectProperties(this.m_gameObjectUID);
					props["layer"] = value;

					var cmd = new CmdGameObjectSetLayer();
					cmd.thisGameObjectGuid = this.m_gameObjectUID;
					cmd.layer = value;
					cmd.Serialize(m_parentScene.m_serializer);
				}
			}

			public bool ActiveInHierarchy
			{
				get { return true; } // ????????
			}

			private IEnumerable<IGameObjectBroker> EnumerateChildren(bool includeDescendents)
			{
				List<Guid> children = m_parentScene.m_childLinksByParentTransformComponent[m_transformUID];
				foreach (Guid childTransformUID in children) {
					Guid childGameObjectUID = m_parentScene.GetComponentParentGameObject(childTransformUID);
					GameObjectBroker goBroker = new GameObjectBroker(m_parentScene, childGameObjectUID);
					yield return goBroker;
					if (includeDescendents) {
						// Need to go depth-first to keep same behavior as Unity
						foreach (var gchild in goBroker.EnumerateChildren(includeDescendents)) {
							yield return gchild;
						}
					}
				}
			}

			private IEnumerable<Guid> EnumerateComponentsInChildren(string componentTypeName, bool includeInactive)
			{
				// I don't think we care about includeInactive, since everything we
				// create is activeSelf=true by default, and there is no way to set something
				// as inactive.

				// First the current GameObject
				Guid[] components = m_parentScene.LookupGameObjectComponentsByType(m_gameObjectUID, componentTypeName);
				if (components != null && components.Length > 0) {
					foreach (var compUID in components) {
						yield return compUID;
					}
				}

				// Then all of the children
				foreach (var childGO in EnumerateChildren(true)) {
					components = m_parentScene.LookupGameObjectComponentsByType(((GameObjectBroker)childGO).m_gameObjectUID, componentTypeName);
					if (components != null && components.Length > 0) {
						foreach (var compUID in components) {
							yield return compUID;
						}
					}
				}
			}

			public IGameObjectBroker GetChildByName(string name)
			{
				string[] namePath = name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				if (namePath.Length == 0) {
					return null;
				}

				GameObjectBroker currObject = this;
				for (int currIndex = 0; currIndex < namePath.Length; ++currIndex) {
					currObject = currObject.EnumerateChildren(false)
						 .Where(go => go.Name == namePath[currIndex])
						 .Cast<GameObjectBroker>()
						 .DefaultIfEmpty(null)
						 .FirstOrDefault();
					if (currObject == null) {
						return null;
					}
				}

				return currObject;
			}

			public IComponentBroker AddComponent(string componentTypeName)
			{
				Guid componentUID = m_parentScene.CreateComponentOnGameObject(m_gameObjectUID, componentTypeName);
				if (componentUID == Guid.Empty) {
					return null;
				}
				m_parentScene.MarkPhysicsSceneDirty();

				return new ComponentBroker(m_parentScene, componentUID, false);
			}

			// NOTE: This is only supported functionality for the Unity Editor - NOT for the Overload Editor
			public IComponentBroker[] GetComponentsInChildren(string componentTypeName, bool includeInactive)
			{
				throw new Exception("GetComponentsInChildren is not supported in the Overload Editor");
			}

			public IComponentBroker GetComponentOnlyValidInExport(string componentTypeName)
			{
				Guid[] components = m_parentScene.LookupGameObjectComponentsByType(m_gameObjectUID, componentTypeName);
				if (components == null || components.Length == 0) {
					return null;
				}
				return new ComponentBroker(m_parentScene, components[0], false);
			}

			public IComponentBroker GetComponent(string componentTypeName)
			{
				Guid[] components = m_parentScene.LookupGameObjectComponentsByType(m_gameObjectUID, componentTypeName);
				if (components == null || components.Length == 0) {
					var guid = Guid.NewGuid();
					var cmd = new CmdGetComponentAtRuntime();
					cmd.includeChildren = false;
					cmd.componentName = componentTypeName;
					cmd.gameObjectGuid = this.m_gameObjectUID;
					cmd.searchResultGuid = guid;
					cmd.Serialize(m_parentScene.m_serializer);
					return new ComponentBroker(m_parentScene, guid, true);
				}

				return new ComponentBroker(m_parentScene, components[0], false);
			}

			public IComponentBroker GetComponentInChildren(string componentTypeName)
			{
				Guid[] foundComponents = EnumerateComponentsInChildren(componentTypeName, true).ToArray();
				if (foundComponents.Length == 0) {
					var guid = Guid.NewGuid();
					var cmd = new CmdGetComponentAtRuntime();
					cmd.includeChildren = true;
					cmd.componentName = componentTypeName;
					cmd.gameObjectGuid = this.m_gameObjectUID;
					cmd.searchResultGuid = guid;
					cmd.Serialize(m_parentScene.m_serializer);
					return new ComponentBroker(m_parentScene, guid, true);
				}

				return new ComponentBroker(m_parentScene, foundComponents[0], false);
			}
		}

		public IGameObjectBroker CreateRootGameObject(string name = "")
		{
			Guid goUID = CreateNewGameObject(CreateGameObjectSerializeMode.CreateNewGameObject, Guid.NewGuid());
			if (goUID == Guid.Empty) {
				return null;
			}

			var go = new GameObjectBroker(this, goUID);
			if (!string.IsNullOrEmpty(name)) {
				go.Name = name;
			}

			return go;
		}

		public IGameObjectBroker CreateInlinePrefab(Guid _prefabUID, string name = "")
		{
			Guid prefabUID = CreateNewGameObject(CreateGameObjectSerializeMode.CreateInlinePrefab, _prefabUID);
			if (prefabUID == Guid.Empty) {
				return null;
			}

			var go = new GameObjectBroker(this, prefabUID);
			if (!string.IsNullOrEmpty(name)) {
				go.Name = name;
			}

			return go;
		}

		public IGameObjectBroker[] GetRootGameObjects()
		{
			List<IGameObjectBroker> result = new List<IGameObjectBroker>();
			foreach (var kvp in m_gameObjectUniverseWithComponents) {
				var componentMap = kvp.Value;
				List<Guid> transformComponents;
				if (!componentMap.TryGetValue("Transform", out transformComponents)) {
					continue;
				}

				var transform = new TransformBroker(this, transformComponents[0]);
				if (transform.Parent != null) {
					// This is not a root GameObject
					continue;
				}

				result.Add(new GameObjectBroker(this, kvp.Key));
			}

			return result.ToArray();
		}

		// Consider removing this from the interface, it is only used during re-export
		//
		// Consider removing this.
		public void DestroyGameObject(IGameObjectBroker obj)
		{
			if (obj == null) {
				return;
			}

			Guid goUID = ((GameObjectBroker)obj).InternalUID;
			DeleteGameObject(goUID);
		}

		// Consider removing this from the interface, it is only used during re-export
		//
		// Consider removing this.
		public void DestroyComponent(IComponentBroker comp)
		{
			if (comp == null) {
				return;
			}

			Guid compUID = ((ComponentBroker)comp).InternalUID;
			DeleteComponent(compUID);
		}

		// Consider removing this from the interface, it is really only
		// used for checking on re-export, which we never handle.
		//
		// Consider removing this.
		public IGameObjectBroker FindGameObject(string name)
		{
			// 'Hand' - find any object by that name
			// '/Hand' - find a root level object
			// '/Monster/Arm/Hand'
			// 'Monster/Arm/Hand'
			string[] namePath = name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			Guid[] candidateList;
			if (name.Length > 0 && name[0] == '/') {
				// Search root-only GameObjects
				candidateList = m_gameObjectUniverseWithComponents
					 .Select(kvp => new { GOUID = kvp.Key, GameObjectProps = m_gameObjectProperties[kvp.Key], TransformProps = m_componentProperties[kvp.Value["Transform"][0]] })
					 .Where(x => (x.GameObjectProps.ContainsKey("name") && ((string)x.GameObjectProps["name"]) == namePath[0]) && (x.TransformProps.ContainsKey("parent") == false || ((Guid)x.TransformProps["parent"]) == Guid.Empty))
					 .Select(x => x.GOUID)
					 .ToArray();
			} else {
				// Search all GameObjects
				candidateList = m_gameObjectUniverseWithComponents
					 .Select(kvp => new { GOUID = kvp.Key, GameObjectProps = m_gameObjectProperties[kvp.Key] })
					 .Where(x => (x.GameObjectProps.ContainsKey("name") && ((string)x.GameObjectProps["name"]) == namePath[0]))
					 .Select(x => x.GOUID)
					 .ToArray();
			}

			// Continue through the remainder of the path (if one specified)
			for (int subPathIdx = 1; subPathIdx < namePath.Length; ++subPathIdx) {
				// Filter the candidateList to those that have children matching the next subPath
				string subPathName = namePath[subPathIdx];
				candidateList = candidateList
					 // Extract Transform GUID for this GameObject
					 .Select(gameObjectUID => m_gameObjectUniverseWithComponents[gameObjectUID]["Transform"][0])
					 // Select all of the children (Transforms) of the Transform
					 .SelectMany(parentTransformUID => m_childLinksByParentTransformComponent[parentTransformUID])
					 // Go from the Transform back to the GameObject
					 .Select(childTransformUID => m_componentToParentGameObject[childTransformUID])
					 // Lookup the GameObject properties
					 .Select(childGameObjectUID => new { GOUID = childGameObjectUID, GameObjectProps = m_gameObjectProperties[childGameObjectUID] })
					 // Filter to those that match the subPath name
					 .Where(x => x.GameObjectProps.ContainsKey("name") && ((string)x.GameObjectProps["name"]) == subPathName)
					 // Back to the GameObject GUID
					 .Select(x => x.GOUID)
					 .ToArray();
			}

			if (candidateList.Length == 0) {
				// No matches
				return null;
			}

			return new GameObjectBroker(this, candidateList[0]);
		}

		// Note: This function really isn't used in an important way for our purposes as
		// it is currently only used to see if we need to create the top level "Level" GameObject
		// or if it already exists. Since we are exporting fresh, it will never exist.
		//
		// If we ever really care about the results, we might want to abstract this into
		// the command stream and return proxy IGameObjectBroker objects that wouldn't actually
		// resolve until runtime. Lets not worry about that right now.
		//
		// Consider removing this.
		public IGameObjectBroker[] FindGameObjectsWithTag(string tag)
		{
			return m_gameObjectUniverseWithComponents
				 .Select(kvp => new { GOUID = kvp.Key, GameObjectProps = m_gameObjectProperties[kvp.Key] })
				 .Where(x => (x.GameObjectProps.ContainsKey("tag") && ((string)x.GameObjectProps["tag"]) == tag))
				 .Select(x => new GameObjectBroker(this, x.GOUID))
				 .Cast<IGameObjectBroker>()
				 .ToArray();
		}

		public void InitializeGameManager(string gm_name, IComponentBroker levelObjectInitializer)
		{
			// TODO: This whole function will get wrapped up in a command stream command which will
			// do this work at runtime instead of export time.
			var cmd = new CmdInitializeGameManager();
			cmd.gameManagerName = gm_name;
			cmd.levelObjectInitializerComponentGuid = ((ComponentBroker)levelObjectInitializer).InternalUID;
			cmd.Serialize(m_serializer);
		}

		public IGameObjectBroker InstantiatePrefab(IGameObjectBroker prefabObject)
		{
			Guid gameObjectUID = CreateNewGameObject(CreateGameObjectSerializeMode.NoCommand, Guid.NewGuid());
			var go = new GameObjectBroker(this, gameObjectUID);

			var cmd = new CmdInstantiatePrefab();
			cmd.prefabAssetReferenceGuid = prefabObject.InternalUID;
			cmd.resultGameObjectGuid = gameObjectUID;
			cmd.resultGameObjectTransformGuid = go.Transform.InternalUID;
			cmd.Serialize(m_serializer);

			return go;
		}

		public IGameObjectBroker FindAndLoadPrefabAsset(string prefabName)
		{
			// Fixup some badly named prefabs
			if (prefabName.Equals("entity_item_hunter", StringComparison.InvariantCultureIgnoreCase)) {
				prefabName = "entity_item_hunter4pack";
			} else if (prefabName.Equals("entity_item_falcon", StringComparison.InvariantCultureIgnoreCase)) {
				prefabName = "entity_item_falcon4pack";
			}

			Guid prefabAssetUID = Guid.NewGuid();

			var cmd = new CmdFindPrefabReference();
			cmd.prefabName = prefabName;
			cmd.prefabAssetReferenceGuid = prefabAssetUID;
			cmd.Serialize(m_serializer);

			// TODO: Maybe this GameObjectBroker should be flagged to not allow modification (it wouldn't make sense)
			return new GameObjectBroker(this, prefabAssetUID, Guid.Empty);
		}

		#region AssetDatabase functionality
		public void AssetDatabase_StartAssetEditing()
		{
			// I don't think we need to do anything here
		}

		public void AssetDatabase_StopAssetEditing()
		{
			// I don't think we need to do anything here
		}

		public void AssetDatabase_SaveAssets()
		{
			foreach (var obj in m_assetsToSerializeOnSave) {
				Guid assetId;
				if (!m_assetObjectToAssetGuid.TryGetValue(obj, out assetId)) {
					// TODO: Report error
					throw new SerializationFailureException();
				}

				var cmd = new CmdSaveAsset(assetId, obj);
				cmd.Serialize(m_serializer);
			}

			// Clear the list of assets to save
			m_assetsToSerializeOnSave.Clear();
		}

		public void AssetDatabase_Refresh()
		{
			// I don't think we need to do anything here
		}

		public void AssetDatabase_ImportAsset(string path)
		{
			// I don't think we need to do anything here
		}

		public void AssetDatabase_CreateAsset(UnityEngine.Object obj, string path)
		{
			if (m_assetPathToAssetDatabaseFileGuid.ContainsKey(path)) {
				throw new Exception(string.Format("The Asset \"{0}\" was already created", path));
			}

			// Create a Guid for the asset file
			Guid assetFileGuid = Guid.NewGuid();
			// Create a Guid for the asset
			Guid assetGuid = Guid.NewGuid();

			// Register
			m_assetObjectToAssetDatabaseFileGuid.Add(obj, assetFileGuid);
			m_assetPathToAssetDatabaseFileGuid.Add(path, assetFileGuid);
			m_assetObjectToAssetGuid.Add(obj, assetGuid);

			List<KeyValuePair<Guid, UnityEngine.Object>> objList = new List<KeyValuePair<Guid, UnityEngine.Object>>();
			m_assetDatabaseObjects.Add(assetFileGuid, objList);

			objList.Add(new KeyValuePair<Guid, UnityEngine.Object>(assetGuid, obj));
			m_assetsToSerializeOnSave.Add(obj);

			var cmdCreate = new CmdCreateAssetFile(path, assetFileGuid);
			cmdCreate.Serialize(m_serializer);

			Type objectType = obj.GetType();
			var cmdAdd = new CmdAddAssetToAssetFile(assetFileGuid, assetGuid, objectType);
			cmdAdd.Serialize(m_serializer);
		}

		public void AssetDatabase_AddObjectToAsset(UnityEngine.Object obj, UnityEngine.Object asset)
		{
			// Lookup the asset's file guid
			Guid assetFileGuid;
			if (!m_assetObjectToAssetDatabaseFileGuid.TryGetValue(asset, out assetFileGuid)) {
				throw new Exception("The asset has not been created via AssetDatabase_CreateAsset");
			}

			List<KeyValuePair<Guid, UnityEngine.Object>> objList = m_assetDatabaseObjects[assetFileGuid];

			// Create a Guid for the asset
			Guid assetGuid = Guid.NewGuid();

			objList.Add(new KeyValuePair<Guid, UnityEngine.Object>(assetGuid, obj));
			m_assetObjectToAssetGuid.Add(obj, assetGuid);
			m_assetsToSerializeOnSave.Add(obj);

			Type objectType = obj.GetType();
			var cmdAdd = new CmdAddAssetToAssetFile(assetFileGuid, assetGuid, objectType);
			cmdAdd.Serialize(m_serializer);
		}
		#endregion
	}
}
