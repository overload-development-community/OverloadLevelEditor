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

namespace OverloadLevelExport
{
    public enum LevelType
    {
        ChallengeMode,
        SinglePlayer,
        MultiPlayer,
    }

	public interface IComponentBroker
	{
#if OVERLOAD_LEVEL_EDITOR
		System.Guid InternalUID { get; }
		bool TryGetPropertyDuringExport<T>(string name, out T result);
#else
		UnityEngine.Component InternalObject { get; }
#endif

		IGameObjectBroker ownerGameObject { get; }
		void SetProperty<T>(string name, T value);
	}

	public interface ITransformBroker : IComponentBroker
	{
		ITransformBroker Parent { get; set; }
		int ChildCount { get; }
		UnityEngine.Vector3 Position { get; set; }
		UnityEngine.Quaternion Rotation { get; set; }
		UnityEngine.Vector3 LocalPosition { get; set; }
		UnityEngine.Quaternion LocalRotation { get; set; }

		ITransformBroker GetChild(int child);
	}

	public interface IGameObjectBroker
	{
#if OVERLOAD_LEVEL_EDITOR
		System.Guid InternalUID { get; }
#else
		UnityEngine.GameObject InternalObject { get; }
#endif

		ITransformBroker Transform { get; }
		string Tag { get; set; }
		string Name { get; set; }
		int Layer { get; set; }
		bool ActiveInHierarchy { get; }

		IGameObjectBroker GetChildByName(string name);
		IComponentBroker AddComponent(string componentTypeName);
		IComponentBroker[] GetComponentsInChildren(string componentTypeName, bool includeInactive); // NOTE: This is only supported in the Unity Editor - NOT the Overload Editor
#if OVERLOAD_LEVEL_EDITOR
		IComponentBroker GetComponentOnlyValidInExport(string componentTypeName);
#endif
		IComponentBroker GetComponent(string componentTypeName);
		IComponentBroker GetComponentInChildren(string componentTypeName);
	}

	public interface ISceneBroker
	{
		IGameObjectBroker CreateRootGameObject(string name = "");
		IGameObjectBroker InstantiatePrefab(IGameObjectBroker prefabObject);
		void DestroyGameObject(IGameObjectBroker obj);
		void DestroyComponent(IComponentBroker comp);
		IGameObjectBroker FindGameObject(string name);
		IGameObjectBroker[] FindGameObjectsWithTag(string tag);
		void InitializeGameManager(string gm_name, IComponentBroker levelObjectInitializer);
		IGameObjectBroker FindAndLoadPrefabAsset(string prefabName);

		LevelType LevelExportType { get; }

		void AssetDatabase_StartAssetEditing();
		void AssetDatabase_StopAssetEditing();
		void AssetDatabase_SaveAssets();
		void AssetDatabase_Refresh();
		void AssetDatabase_ImportAsset(string path);
		void AssetDatabase_CreateAsset(UnityEngine.Object obj, string path);
		void AssetDatabase_AddObjectToAsset(UnityEngine.Object obj, UnityEngine.Object asset);
	}
}