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
	static class Serializers
	{
		public static void RegisterSerializers()
		{
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(SegmentLightInfo), Serializers.Serialize_SegmentLightInfo, 0x1A6A41130A1A026F);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(SegmentReflectionProbeInfo), Serializers.Serialize_SegmentReflectionProbeInfo, 0x544F641426210104);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(LevelData.PortalDoorConnection), Serializers.Serialize_PortalDoorConnection, 0x7A1A461C0338151A);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(LevelData.SpawnPoint), Serializers.Serialize_SpawnPoint, 0x4048456A0F034C1B);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(LevelGeometry), Serializers.Serialize_LevelGeometry, 0x4F1D0629010A9502);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(UnityEngine.Mesh), Serializers.Serialize_UnityMesh, 0x1625382F0C613911);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(PortalPolygonData), Serializers.Serialize_PortalPolygonData, 0x8A188B0EB600690B);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(PortalData), Serializers.Serialize_PortalData, 0x081B0F4622022E04);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(SegmentData), Serializers.Serialize_SegmentData, 0x175B612B03657695);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(BSPTreeNode), Serializers.Serialize_BSPTreeNode, 0x458B012301C00016);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(AABB), Serializers.Serialize_AABB, 0x3F492A0C1E022C05);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(AABBTreeNode), Serializers.Serialize_AABBTreeNode, 0x002A050200321E0E);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(ChunkData), Serializers.Serialize_ChunkData, 0x470116442D051D07);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(ChunkPortal), Serializers.Serialize_ChunkPortal, 0xAD3C0D0200150210);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(PortalGeomTriangle), Serializers.Serialize_PortalGeomTriangle, 0x1A7C981C322F3DD3);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(PortalGeomData), Serializers.Serialize_PortalGeomData, 0x025CCD9E09040B00);
			OverloadLevelConvertSerializer.RegisterSerializer(typeof(PathDistanceData), Serializers.Serialize_PathDistanceData, 0x5019100A0A0E0365);
		}

		public static object Serialize_SegmentLightInfo(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			SegmentLightInfo obj = (SegmentLightInfo)_obj;
			serializer.SerializeX(obj, x => x.LightType);
			serializer.SerializeX(obj, x => x.SegmentIndex);
#if OVERLOAD_LEVEL_EDITOR
			serializer.SerializeOut_guid(obj.LightObject.InternalUID);
#else
			Guid lightObjectGoGuid = serializer.SerializeIn_guid();
			UserLevelLoader loader = (UserLevelLoader)serializer.Context;
			obj.LightObject = loader.ResolveGameObject(lightObjectGoGuid);
#endif
			return obj;
		}

		public static object Serialize_SegmentReflectionProbeInfo(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			SegmentReflectionProbeInfo obj = (SegmentReflectionProbeInfo)_obj;
			serializer.SerializeX(obj, x => x.ProbeType);
			serializer.SerializeX(obj, x => x.SegmentIndex);
#if OVERLOAD_LEVEL_EDITOR
			serializer.SerializeOut_guid(obj.ProbeObject.InternalUID);
#else
			Guid probeObjectGoGuid = serializer.SerializeIn_guid();
			UserLevelLoader loader = (UserLevelLoader)serializer.Context;
			obj.ProbeObject = loader.ResolveGameObject(probeObjectGoGuid);
#endif
			return obj;
		}

		public static object Serialize_PortalDoorConnection(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			LevelData.PortalDoorConnection obj = (LevelData.PortalDoorConnection)_obj;
			serializer.SerializeX(obj, x => x.PortalIndex);
#if OVERLOAD_LEVEL_EDITOR
			serializer.SerializeOut_guid(obj.ReferenceDoor.InternalUID);
#else
			Guid doorBaseRefGuid = serializer.SerializeIn_guid();
			UserLevelLoader loader = (UserLevelLoader)serializer.Context;
			obj.ReferenceDoor = (DoorBase)loader.ResolveComponent(doorBaseRefGuid);
#endif
			return obj;
		}

		public static object Serialize_SpawnPoint(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			LevelData.SpawnPoint obj = (LevelData.SpawnPoint)_obj;
			serializer.SerializeX(obj, x => x.position);
			serializer.SerializeX(obj, x => x.orientation);
			serializer.SerializeX(obj, x => x.m_current_segment);
			serializer.SerializeX(obj, x => x.multiplayer_team_association_mask);
			return obj;
		}

		public static object Serialize_PortalPolygonData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			PortalPolygonData obj = (PortalPolygonData)_obj;
			if (serializer.IsWriting) {
				serializer.SerializeOut_vector3(obj.Normal);
				serializer.SerializeOut_float(obj.PlaneEqD);
				serializer.SerializeOut_array(typeof(int), obj.VertIndices);
			} else {
				obj.Normal = serializer.SerializeIn_vector3();
				obj.PlaneEqD = serializer.SerializeIn_float();
				obj.VertIndices = (int[])serializer.SerializeIn_array(typeof(int));
			}
			return obj;
		}

		public static object Serialize_PortalData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			PortalData obj = (PortalData)_obj;
			serializer.SerializeX(obj, x => x.MasterSegmentIndex);
			serializer.SerializeX(obj, x => x.MasterSideIndex);
			serializer.SerializeX(obj, x => x.SlaveSegmentIndex);
			serializer.SerializeX(obj, x => x.SlaveSideIndex);
			serializer.SerializeX(obj, x => x.Polygons);
			// Note: not serializing .DoorData, which is only used in the runtime
			return obj;
		}

		public static object Serialize_SegmentData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			SegmentData obj = (SegmentData)_obj;
			serializer.SerializeX(obj, x => x.VertIndices);
			serializer.SerializeX(obj, x => x.Center);
			serializer.SerializeX(obj, x => x.MinCornerPos);
			serializer.SerializeX(obj, x => x.MaxCornerPos);
			serializer.SerializeX(obj, x => x.ApproxSidePlaneEquations);
			serializer.SerializeX(obj, x => x.Portals);
			serializer.SerializeX(obj, x => x.ChunkNum);
			serializer.SerializeX(obj, x => x.DecalFlags);
			serializer.SerializeX(obj, x => x.DoorFlags);
			serializer.SerializeX(obj, x => x.Dark);
			serializer.SerializeX(obj, x => x.Pathfinding);
			serializer.SerializeX(obj, x => x.ExitSegment);
			// Note: skip .Lights           -- it isn't used in export, only at runtime
			// Note: skip .ReflectionProbes -- it isn't used in export, only at runtime
			serializer.SerializeX(obj, x => x.DeformationHeights);
			serializer.SerializeX(obj, x => x.WarpDestinationSegs);
			return obj;
		}

		public static object Serialize_BSPTreeNode(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			BSPTreeNode obj = (BSPTreeNode)_obj;
			if (serializer.IsWriting) {
				serializer.SerializeOut_vector4(obj.PlaneEq);
				serializer.SerializeOut_int32(obj.BackNodeIndex);
				serializer.SerializeOut_int32(obj.FrontNodeIndex);
			} else {
				obj.PlaneEq = serializer.SerializeIn_vector4();
				obj.BackNodeIndex = serializer.SerializeIn_int32();
				obj.FrontNodeIndex = serializer.SerializeIn_int32();
			}
			return obj;
		}

		public static object Serialize_AABB(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			AABB obj = (AABB)_obj;
			if (serializer.IsWriting) {
				serializer.SerializeOut_vector3(obj.MinXYZ);
				serializer.SerializeOut_vector3(obj.MaxXYZ);
			} else {
				obj.MinXYZ = serializer.SerializeIn_vector3();
				obj.MaxXYZ = serializer.SerializeIn_vector3();
			}
			return obj;
		}

		public static object Serialize_AABBTreeNode(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			AABBTreeNode obj = (AABBTreeNode)_obj;
			if (serializer.IsWriting) {
				serializer.SerializeOut_object(obj.Bounds);
				serializer.SerializeOut_int32(obj.MinChildIndex);
				serializer.SerializeOut_int32(obj.MaxChildIndex);
				serializer.SerializeOut_int32(obj.SegmentIndex);
			} else {
				obj.Bounds = (AABB)serializer.SerializeIn_object(typeof(AABB));
				obj.MinChildIndex = serializer.SerializeIn_int32();
				obj.MaxChildIndex = serializer.SerializeIn_int32();
				obj.SegmentIndex = serializer.SerializeIn_int32();
			}
			return obj;
		}

		public static object Serialize_ChunkData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			ChunkData obj = (ChunkData)_obj;
			serializer.SerializeX(obj, x => x.PortalIndices);
			serializer.SerializeX(obj, x => x.Segnums);
			serializer.SerializeX(obj, x => x.IsEnergyCenter);
			return obj;
		}

		public static object Serialize_ChunkPortal(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			ChunkPortal obj = (ChunkPortal)_obj;
			serializer.SerializeX(obj, x => x.Num);
			serializer.SerializeX(obj, x => x.Chunknum);
			serializer.SerializeX(obj, x => x.Segnum);
			serializer.SerializeX(obj, x => x.Sidenum);
			serializer.SerializeX(obj, x => x.ConnectedChunk);
			serializer.SerializeX(obj, x => x.ConnectedPortal);
			serializer.SerializeX(obj, x => x.PortalGeomNum);
			return obj;
		}

		public static object Serialize_PortalGeomTriangle(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			PortalGeomTriangle obj = (PortalGeomTriangle)_obj;
			serializer.SerializeX(obj, x => x.FirstVertIndex);
			return obj;
		}

		public static object Serialize_PortalGeomData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			PortalGeomData obj = (PortalGeomData)_obj;
			serializer.SerializeX(obj, x => x.NumTriangles);
			serializer.SerializeX(obj, x => x.StartIndex);
			return obj;
		}

		public static object Serialize_PathDistanceData(object _obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			PathDistanceData obj = (PathDistanceData)_obj;
			if (serializer.IsWriting) {
				serializer.SerializeOut_float(obj.Distance);
				serializer.SerializeOut_int32(obj.PathLength);
				serializer.SerializeOut_int32(obj.SecondSegment);
				serializer.SerializeOut_int32(obj.SecondLastSegment);
			} else {
				obj.Distance = serializer.SerializeIn_float();
				obj.PathLength = serializer.SerializeIn_int32();
				obj.SecondSegment = serializer.SerializeIn_int32();
				obj.SecondLastSegment = serializer.SerializeIn_int32();
			}
			return obj;
		}

		public static object Serialize_LevelGeometry(object obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			LevelGeometry geom = (LevelGeometry)obj;

			serializer.SerializeX(geom, x => x.name);
			serializer.SerializeX(geom, x => x.FileName);
			serializer.SerializeX(geom, x => x.Segments);
			serializer.SerializeX(geom, x => x.Portals);
			serializer.SerializeX(geom, x => x.SegmentVerts);
			serializer.SerializeX(geom, x => x.SegmentRootIndicesIntoPerSegmentBSPData);
			serializer.SerializeX(geom, x => x.PerSegmentBSPData);
			serializer.SerializeX(geom, x => x.SegmentAABBTree);
			serializer.SerializeX(geom, x => x.Chunks);
			serializer.SerializeX(geom, x => x.ChunkPortals);
			serializer.SerializeX(geom, x => x.PortalGeomVerts);
			serializer.SerializeX(geom, x => x.PortalGeomTriangles);
			serializer.SerializeX(geom, x => x.PortalGeomDatas);
			serializer.SerializeX(geom, x => x.ChallengeModeDataText);
			serializer.SerializeX(geom, x => x.SegmentToSegmentVisibility);
			serializer.SerializeX(geom, x => x.PathDistances);
			serializer.SerializeX(geom, x => x.GeometryHash);
			serializer.SerializeX(geom, x => x.RobotSpawnPointsHash);

			return geom;
		}

		public static object Serialize_UnityMesh(object obj, System.Type type, OverloadLevelConvertSerializer serializer)
		{
			uint version = serializer.Version;

			UnityEngine.Mesh mesh = (UnityEngine.Mesh)obj;

			int flags;
			if (version <= 3) {
				// old version, we just had 'colors'
				flags = 1;
			} else {
				// new version, use a flags value to report what we have
				if (serializer.IsWriting) {
					flags = 0;
					if (mesh.colors != null && mesh.colors.Length > 0) {
						flags |= 1;
					}
					if (mesh.colors32 != null && mesh.colors32.Length > 0) {
						flags |= 2;
					}
					serializer.SerializeOut_int32(flags);
				} else {
					flags = serializer.SerializeIn_int32();
				}
			}

			serializer.SerializeX(mesh, x => x.name);
			serializer.SerializeX(mesh, x => x.vertices); // Note: vertices must come first since the other fields may check size invariants
			serializer.SerializeX(mesh, x => x.uv);
			serializer.SerializeX(mesh, x => x.uv2);
			serializer.SerializeX(mesh, x => x.uv3);
			serializer.SerializeX(mesh, x => x.normals);
			serializer.SerializeX(mesh, x => x.tangents);
			if ((flags & 1) != 0) {
				serializer.SerializeX(mesh, x => x.colors);
			}
			if ((flags & 2) != 0) {
				serializer.SerializeX(mesh, x => x.colors32);
			}
			if (version > 3) {
				serializer.SerializeX(mesh, x => x.boneWeights);
				serializer.SerializeX(mesh, x => x.bindposes);
			}

			if (serializer.IsWriting) {
				Int32 numSubmeshes = mesh.subMeshCount;
				serializer.SerializeOut_int32(numSubmeshes);

				for (Int32 i = 0; i < numSubmeshes; ++i) {
					Int32[] tris = mesh.GetTriangles(i);
					serializer.SerializeOut_array(typeof(Int32), tris);
				}
			} else {
				Int32 numSubmeshes = serializer.SerializeIn_int32();
				mesh.subMeshCount = numSubmeshes;

				for (Int32 i = 0; i < numSubmeshes; ++i) {
					Int32[] tris = (Int32[])serializer.SerializeIn_array(typeof(Int32));
					mesh.SetTriangles(tris, i);
				}
			}

			return mesh;
		}
	}
}