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

namespace OverloadLevelExport
{
	public interface ICmdExecutor
	{
		void ExecuteCmdPacket<TCmdPacket>(TCmdPacket packet);
	}

	public interface ICmdPacket
	{
		void Serialize(OverloadLevelConvertSerializer serializer);
		void Execute(ICmdExecutor executor); // need to pass some sort of 'state'
	}

	public class CmdDone : ICmdPacket
	{
		public const UInt16 CommandId = 0;

		public CmdDone()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdCreateAssetFile : ICmdPacket
	{
		public const UInt16 CommandId = 1;

		public string RelativeAssetPath;
		public Guid AssetFileGuid;

		public CmdCreateAssetFile()
		{
		}

		public CmdCreateAssetFile(string relativeAssetPath, Guid assetFileGuid)
		{
			this.RelativeAssetPath = relativeAssetPath;
			this.AssetFileGuid = assetFileGuid;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.RelativeAssetPath);
			serializer.SerializeX(this, x => x.AssetFileGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdAddAssetToAssetFile : ICmdPacket
	{
		public const UInt16 CommandId = 2;

		public Type AssetObjectType;
		public Guid AssetGuid;
		public Guid AssetFileGuid;

		public CmdAddAssetToAssetFile()
		{
		}

		public CmdAddAssetToAssetFile(Guid assetFileGuid, Guid assetGuid, Type assetObjectType)
		{
			this.AssetFileGuid = assetFileGuid;
			this.AssetGuid = assetGuid;
			this.AssetObjectType = assetObjectType;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.AssetFileGuid);
			serializer.SerializeX(this, x => x.AssetGuid);

			if (serializer.IsWriting) {
				// Write out the name of the type
				string assetDataTypeName = AssetObjectType.FullName;
				serializer.SerializeOut_string(assetDataTypeName);
			} else {
				// Read in the name of the type
				string assetDataTypeName = serializer.SerializeIn_string();
				this.AssetObjectType = Type.GetType(assetDataTypeName);
#if !OVERLOAD_LEVEL_EDITOR
				if (this.AssetObjectType == null) {
					// Look for the type in UnityEngine, as not all the Unity types resolve on Type.GetType
					var unityEngineAsm = System.Reflection.Assembly.GetAssembly(typeof(UnityEngine.Mesh));
					this.AssetObjectType = unityEngineAsm.GetType(assetDataTypeName);
				}
#endif
			}
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdInitializeGameManager : ICmdPacket
	{
		public const UInt16 CommandId = 3;

		public string gameManagerName;
		public Guid levelObjectInitializerComponentGuid;

		public CmdInitializeGameManager()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.gameManagerName);
			serializer.SerializeX(this, x => x.levelObjectInitializerComponentGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdCreateNewGameObject : ICmdPacket
	{
		public const UInt16 CommandId = 4;

		public Guid gameObjectGuid;
		public Guid transformGuid;

		public CmdCreateNewGameObject()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.gameObjectGuid);
			serializer.SerializeX(this, x => x.transformGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdTransformSetParent : ICmdPacket
	{
		public const UInt16 CommandId = 5;

		public Guid thisTransformGuid;
		public Guid newParentGuid;

		public CmdTransformSetParent()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisTransformGuid);
			serializer.SerializeX(this, x => x.newParentGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGameObjectSetName : ICmdPacket
	{
		public const UInt16 CommandId = 6;

		public Guid thisGameObjectGuid;
		public string name;

		public CmdGameObjectSetName()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisGameObjectGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGameObjectSetTag : ICmdPacket
	{
		public const UInt16 CommandId = 7;

		public Guid thisGameObjectGuid;
		public string tag;

		public CmdGameObjectSetTag()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisGameObjectGuid);
			serializer.SerializeX(this, x => x.tag);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGameObjectSetLayer : ICmdPacket
	{
		public const UInt16 CommandId = 8;

		public Guid thisGameObjectGuid;
		public int layer;

		public CmdGameObjectSetLayer()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisGameObjectGuid);
			serializer.SerializeX(this, x => x.layer);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGameObjectAddComponent : ICmdPacket
	{
		public const UInt16 CommandId = 9;

		public Guid thisGameObjectGuid;
		public Guid newComponentGuid;
		public string componentTypeName;

		public CmdGameObjectAddComponent()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisGameObjectGuid);
			serializer.SerializeX(this, x => x.newComponentGuid);
			serializer.SerializeX(this, x => x.componentTypeName);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGameObjectSetComponentProperty : ICmdPacket
	{
		public const UInt16 CommandId = 10;

		public enum ValueNamespace : byte
		{
			None = 0,
			Asset = 1,
			GameObject = 3,
			Component = 4,
		}

		public enum TargetType : byte
		{
			Singular = 0,
			Array = 1,
			List = 2,
		}

		public Guid thisComponentGuid;
		public string propertyName;
		public object propertyValue;
		public TargetType targetType;
		public ValueNamespace propertyValueNamespace;

		public CmdGameObjectSetComponentProperty()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.thisComponentGuid);
			serializer.SerializeX(this, x => x.propertyName);
			if (serializer.IsWriting) {
				serializer.SerializeOut_byte((byte)propertyValueNamespace);
				serializer.SerializeOut_byte((byte)targetType);
				serializer.SerializeOut_unknown(propertyValue, propertyValue.GetType());
			} else {
				propertyValueNamespace = (ValueNamespace)serializer.SerializeIn_byte();
				targetType = (TargetType)serializer.SerializeIn_byte();
				propertyValue = serializer.SerializeIn_unknown();
			}

			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdAssetRegisterMaterial : ICmdPacket
	{
		public const UInt16 CommandId = 11;

		public Guid materialAssetGuid;
		public string materialLookupName;
		public Int32 geometryType;

		public CmdAssetRegisterMaterial()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialAssetGuid);
			serializer.SerializeX(this, x => x.geometryType);
			serializer.SerializeX(this, x => x.materialLookupName);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdFindPrefabReference : ICmdPacket
	{
		public const UInt16 CommandId = 12;

		public string prefabName;
		public Guid prefabAssetReferenceGuid;

		public CmdFindPrefabReference()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.prefabName);
			serializer.SerializeX(this, x => x.prefabAssetReferenceGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdInstantiatePrefab : ICmdPacket
	{
		public const UInt16 CommandId = 13;

		public Guid prefabAssetReferenceGuid;
		public Guid resultGameObjectGuid;
		public Guid resultGameObjectTransformGuid;

		public CmdInstantiatePrefab()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.prefabAssetReferenceGuid);
			serializer.SerializeX(this, x => x.resultGameObjectGuid);
			serializer.SerializeX(this, x => x.resultGameObjectTransformGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdGetComponentAtRuntime : ICmdPacket
	{
		public const UInt16 CommandId = 14;

		public string componentName;
		public bool includeChildren;
		public Guid gameObjectGuid;
		public Guid searchResultGuid;

		public CmdGetComponentAtRuntime()
		{
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.includeChildren);
			serializer.SerializeX(this, x => x.componentName);
			serializer.SerializeX(this, x => x.gameObjectGuid);
			serializer.SerializeX(this, x => x.searchResultGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdSaveAsset : ICmdPacket
	{
		public const UInt16 CommandId = 15;

		public UnityEngine.Object AssetData;
		public Guid AssetGuid;

		public CmdSaveAsset()
		{
		}

		public CmdSaveAsset(Guid assetGuid, UnityEngine.Object assetData)
		{
			this.AssetGuid = assetGuid;
			this.AssetData = assetData;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.AssetGuid);
			if (serializer.IsWriting) {
				serializer.SerializeOut_ExistingObject(this.AssetGuid, this.AssetData);
			} else {
				// Lookup this object in the asset database
				this.AssetData = null;
#if !OVERLOAD_LEVEL_EDITOR
				// Lookup the UnityEngine.Object from the Guid
				this.AssetData = ((UserLevelLoader)serializer.Context).ResolveAsset(this.AssetGuid);
				if (this.AssetData == null) {
					// TODO: Report error
				}
#endif
				serializer.SerializeIn_ExistingObject(this.AssetData);
			}
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdLoadAssetBundle : ICmdPacket
	{
		public const UInt16 CommandId = 16;

		public string relativeBundleFolder;
		public string bundleName;
		public Guid bundleRefGuid;

		public CmdLoadAssetBundle()
		{
		}

		public CmdLoadAssetBundle(string _relativeBundleFolder, string _bundleName, Guid _bundleRefGuid)
		{
			relativeBundleFolder = _relativeBundleFolder;
			bundleName = _bundleName;
			bundleRefGuid = _bundleRefGuid;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.relativeBundleFolder);
			serializer.SerializeX(this, x => x.bundleName);
			serializer.SerializeX(this, x => x.bundleRefGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdLoadAssetFromAssetBundle : ICmdPacket
	{
		public const UInt16 CommandId = 17;

		public string assetName;
		public Guid bundleRefGuid;
		public Guid loadedAssetGuid;

		public CmdLoadAssetFromAssetBundle()
		{
		}

		public CmdLoadAssetFromAssetBundle(Guid _bundleRefGuid, string _assetName, Guid _loadedAssetGuid)
		{
			bundleRefGuid = _bundleRefGuid;
			assetName = _assetName;
			loadedAssetGuid = _loadedAssetGuid;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.assetName);
			serializer.SerializeX(this, x => x.bundleRefGuid);
			serializer.SerializeX(this, x => x.loadedAssetGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdCreateMaterial : ICmdPacket
	{
		public const UInt16 CommandId = 18;

		public Guid shaderGuid;
		public Guid materialGuid;
		public UnityEngine.Color color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 1.0f);
		public bool enableInstancing = false;
		public Guid mainTexture = Guid.Empty;
		public UnityEngine.Vector2 mainTextureOffset = new UnityEngine.Vector2(0.0f, 0.0f);
		public UnityEngine.Vector2 mainTextureScale = new UnityEngine.Vector2(1.0f, 1.0f);
		public int renderQueue = 2450;
		public string[] shaderKeywords = null;
		public string name = null;

		public CmdCreateMaterial()
		{
		}

		public CmdCreateMaterial(Guid _materialGuid, Guid _shaderGuid)
		{
			materialGuid = _materialGuid;
			shaderGuid = _shaderGuid;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.shaderGuid);
			serializer.SerializeX(this, x => x.color);
			serializer.SerializeX(this, x => x.enableInstancing);
			serializer.SerializeX(this, x => x.mainTexture);
			serializer.SerializeX(this, x => x.mainTextureOffset);
			serializer.SerializeX(this, x => x.mainTextureScale);
			serializer.SerializeX(this, x => x.renderQueue);
			serializer.SerializeX(this, x => x.shaderKeywords);
			serializer.SerializeX(this, x => x.name);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialDisableKeyword : ICmdPacket
	{
		public const UInt16 CommandId = 19;

		public Guid materialGuid;
		public string keyword;

		public CmdMaterialDisableKeyword()
		{
		}

		public CmdMaterialDisableKeyword(Guid _materialGuid, string _keyword)
		{
			materialGuid = _materialGuid;
			keyword = _keyword;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.keyword);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialEnableKeyword : ICmdPacket
	{
		public const UInt16 CommandId = 20;

		public Guid materialGuid;
		public string keyword;

		public CmdMaterialEnableKeyword()
		{
		}

		public CmdMaterialEnableKeyword(Guid _materialGuid, string _keyword)
		{
			materialGuid = _materialGuid;
			keyword = _keyword;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.keyword);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetColor : ICmdPacket
	{
		public const UInt16 CommandId = 21;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Color color;

		public CmdMaterialSetColor()
		{
		}

		public CmdMaterialSetColor(Guid _materialGuid, string _name, UnityEngine.Color _color)
		{
			materialGuid = _materialGuid;
			name = _name;
			color = _color;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.color);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetColorArray : ICmdPacket
	{
		public const UInt16 CommandId = 22;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Color[] value;

		public CmdMaterialSetColorArray()
		{
		}

		public CmdMaterialSetColorArray(Guid _materialGuid, string _name, UnityEngine.Color[] _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetFloat : ICmdPacket
	{
		public const UInt16 CommandId = 23;

		public Guid materialGuid;
		public string name;
		public float value;

		public CmdMaterialSetFloat()
		{
		}

		public CmdMaterialSetFloat(Guid _materialGuid, string _name, float _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetFloatArray : ICmdPacket
	{
		public const UInt16 CommandId = 24;

		public Guid materialGuid;
		public string name;
		public float[] value;

		public CmdMaterialSetFloatArray()
		{
		}

		public CmdMaterialSetFloatArray(Guid _materialGuid, string _name, float[] _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetInt : ICmdPacket
	{
		public const UInt16 CommandId = 25;

		public Guid materialGuid;
		public string name;
		public int value;

		public CmdMaterialSetInt()
		{
		}

		public CmdMaterialSetInt(Guid _materialGuid, string _name, int _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetMatrix : ICmdPacket
	{
		public const UInt16 CommandId = 26;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Matrix4x4 value;

		public CmdMaterialSetMatrix()
		{
		}

		public CmdMaterialSetMatrix(Guid _materialGuid, string _name, UnityEngine.Matrix4x4 _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetMatrixArray : ICmdPacket
	{
		public const UInt16 CommandId = 27;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Matrix4x4[] value;

		public CmdMaterialSetMatrixArray()
		{
		}

		public CmdMaterialSetMatrixArray(Guid _materialGuid, string _name, UnityEngine.Matrix4x4[] _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetOverrideTag : ICmdPacket
	{
		public const UInt16 CommandId = 28;

		public Guid materialGuid;
		public string tag;
		public string value;

		public CmdMaterialSetOverrideTag()
		{
		}

		public CmdMaterialSetOverrideTag(Guid _materialGuid, string _tag, string _value)
		{
			materialGuid = _materialGuid;
			tag = _tag;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.tag);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetTexture : ICmdPacket
	{
		public const UInt16 CommandId = 29;

		public Guid materialGuid;
		public string name;
		public Guid value;

		public CmdMaterialSetTexture()
		{
		}

		public CmdMaterialSetTexture(Guid _materialGuid, string _name, Guid _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetTextureOffset : ICmdPacket
	{
		public const UInt16 CommandId = 30;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Vector2 value;

		public CmdMaterialSetTextureOffset()
		{
		}

		public CmdMaterialSetTextureOffset(Guid _materialGuid, string _name, UnityEngine.Vector2 _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetTextureScale : ICmdPacket
	{
		public const UInt16 CommandId = 30;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Vector2 value;

		public CmdMaterialSetTextureScale()
		{
		}

		public CmdMaterialSetTextureScale(Guid _materialGuid, string _name, UnityEngine.Vector2 _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetVector : ICmdPacket
	{
		public const UInt16 CommandId = 31;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Vector4 value;

		public CmdMaterialSetVector()
		{
		}

		public CmdMaterialSetVector(Guid _materialGuid, string _name, UnityEngine.Vector4 _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdMaterialSetVectorArray : ICmdPacket
	{
		public const UInt16 CommandId = 32;

		public Guid materialGuid;
		public string name;
		public UnityEngine.Vector4[] value;

		public CmdMaterialSetVectorArray()
		{
		}

		public CmdMaterialSetVectorArray(Guid _materialGuid, string _name, UnityEngine.Vector4[] _value)
		{
			materialGuid = _materialGuid;
			name = _name;
			value = _value;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.materialGuid);
			serializer.SerializeX(this, x => x.name);
			serializer.SerializeX(this, x => x.value);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdCreateTexture2D : ICmdPacket
	{
		public const UInt16 CommandId = 33;

		public Guid texture2DGuid;
		public UnityEngine.Texture2D texture;

		public CmdCreateTexture2D()
		{
		}

		public CmdCreateTexture2D(Guid _texture2DGuid, UnityEngine.Texture2D _texture)
		{
			texture2DGuid = _texture2DGuid;
			texture = _texture;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);

			int width = 0;
			int height = 0;
			string formatStr = UnityEngine.TextureFormat.RGB24.ToString();
			string filterModeStr = UnityEngine.FilterMode.Bilinear.ToString();
			string name = string.Empty;
			bool mipmap = false;

			if (serializer.IsWriting) {
				width = texture.width;
				height = texture.height;
				formatStr = texture.format.ToString();
				filterModeStr = texture.filterMode.ToString();
				name = texture.name;
#if OVERLOAD_LEVEL_EDITOR
				mipmap = texture.mipmap;
#endif
			}

			serializer.SerializeX(this, x => x.texture2DGuid);
			serializer.Serialize(ref width);
			serializer.Serialize(ref height);
			serializer.Serialize(ref formatStr);
			serializer.Serialize(ref mipmap);
			serializer.Serialize(ref filterModeStr);
			serializer.Serialize(ref name);

			UnityEngine.Color32[] pixelData = null;
			if (serializer.IsWriting) {
				serializer.SerializeOut_array(typeof(UnityEngine.Color32), texture.GetPixels32());
			} else {
				pixelData = (UnityEngine.Color32[])serializer.SerializeIn_array(typeof(UnityEngine.Color32));
			}

			if (!serializer.IsWriting) {
				UnityEngine.TextureFormat format = (UnityEngine.TextureFormat)Enum.Parse(typeof(UnityEngine.TextureFormat), formatStr);
				this.texture = new UnityEngine.Texture2D(width, height, format, mipmap);

				UnityEngine.FilterMode filterMode = (UnityEngine.FilterMode)Enum.Parse(typeof(UnityEngine.FilterMode), filterModeStr);
				this.texture.filterMode = filterMode;

				this.texture.name = name;

				this.texture.SetPixels32(pixelData);
				this.texture.Apply();
			}

			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdLoadAssemblyFromAssetBundle : ICmdPacket
	{
		public const UInt16 CommandId = 34;

		public string assemblyAssetName;
		public Guid bundleRefGuid;

		public CmdLoadAssemblyFromAssetBundle()
		{
		}

		public CmdLoadAssemblyFromAssetBundle(Guid _bundleRefGuid, string _assemblyAssetName)
		{
			bundleRefGuid = _bundleRefGuid;
			assemblyAssetName = _assemblyAssetName;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.assemblyAssetName);
			serializer.SerializeX(this, x => x.bundleRefGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

	public class CmdCreateInlinePrefab : ICmdPacket
	{
		public const UInt16 CommandId = 35;

		public Guid prefabGuid;
		public Guid transformGuid;

		public CmdCreateInlinePrefab()
		{
		}

		public CmdCreateInlinePrefab(Guid _prefabGuid, Guid _transformGuid)
		{
			prefabGuid = _prefabGuid;
			transformGuid = _transformGuid;
		}

		public void Serialize(OverloadLevelConvertSerializer serializer)
		{
			serializer.StartCmd(CommandId);
			serializer.SerializeX(this, x => x.prefabGuid);
			serializer.SerializeX(this, x => x.transformGuid);
			serializer.FinishCmd(CommandId);
		}

		public void Execute(ICmdExecutor executor)
		{
			executor.ExecuteCmdPacket(this);
		}
	}

}

