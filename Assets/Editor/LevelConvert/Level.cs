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
using System.Collections.Generic;
using OpenTK;
using Newtonsoft.Json.Linq;

namespace OverloadLevelEditor
{
#if !OVERLOAD_LEVEL_EDITOR
    public class TextureManager
    {
    }

    public class TextureSet
    {
    }

    public enum EditMode
    {
        SEGMENT
    }

    public enum PivotMode
    {
        LOCAL
    }

    public enum SideSelect
    {
        ALL
    }

    public enum DragMode
    {
        ALL
    }

	public enum InsertDecal
	{
		ALL
	}
#endif

    public interface IEditor
    {
        bool IsHeadlessProxyEditor { get; }
        Level LoadedLevel { get; }
        TextureManager tm_decal { get; }
        TextureManager tm_level { get; }
        List<TextureSet> TextureSets { get; }
        EditMode ActiveEditMode { get; set; }
        SideSelect ActiveSideSelect { get; }
        PivotMode ActivePivotMode { get; }
        DragMode ActiveDragMode { get; }
		InsertDecal ActiveInsertDecal { get; }
        float CurrGridSnap { get; }
        bool IsAutoCenter { get; }
        int CurrExtrudeLength { get; }
        bool ShouldInsertAdvance { get; }
        Matrix4 SourceSideRotation { get; }
        Matrix4 DestSideRotation { get; }
        void AddOutputText(string text);
        void EntityListUpdateEntity(Entity entity);
        void EntityListRemoveEntity(Entity entity);
        void EntityListSetSelectedEntity(int index);
        void SetProjOffsetAllViews(Vector3 pos);
        void SetEditModeSilent(EditMode mode, bool update_label = true);
        Vector3 AlignPasteVert(Vector3 v);
        void RefreshGeometry(bool refresh_editor = true);
        void Refresh();
        DMesh GetDMeshByName(string dmeshName);
    }

	public partial class Level
	{
		public const float DEF_SEG_SZ = 4f;
		public const float DEF_SEG_SZH = DEF_SEG_SZ / 2f;

		public const int MAX_SEGMENTS = 4000;
		public const int MAX_VERTICES = MAX_SEGMENTS * 4; // 2048 * 4 = 8192 - All segments share at least 4 verts, making this is a reasonable limit
		public const int MAX_ENTITIES = 1024; // Unrelated to other level elements

		public Segment[] segment = new Segment[MAX_SEGMENTS];
		public int next_segment;
		public Vertex[] vertex = new Vertex[MAX_VERTICES];
		public int next_vertex;
		public Entity[] entity = new Entity[MAX_ENTITIES];
		public int next_entity;

		public int m_num_chunks = -1;

		//For rendering chunks
		public System.Drawing.Color[] ChunkColor = new System.Drawing.Color[MAX_SEGMENTS];

		private int m_selected_segment;
		public int selected_side;
		public int selected_vertex;
		private int m_selected_entity;

		public int num_segments;
		public int num_sides;	// Number of visible sides
		public int num_vertices;
		public int num_entities;
		public int num_visible_segments;
		public int num_marked_segments;
		public int num_marked_sides;
		public int num_marked_vertices;
		public int num_marked_entities;

		public LevelGlobalData global_data = new LevelGlobalData();

		//The name of the default texture set for this level
		public string m_texture_set_name;

		public bool dirty;

		public enum CustomLevelInfoObjective
		{
			NORMAL,        // 10 levels
			DESTROY_BOTS,  // Level 1
			SECURE_CRASH,  // Level 12
			ALIEN_WARP,    // Level 13/14/15
			LEVEL_16,      // Level 16
			TRAINING,      // Training level
		}

		public class CustomLevelInfo
		{
			public float m_exit_music_start_time = 0.0f;
			public bool m_exit_no_explosions = false;
			public bool m_alien_lava = false;
			public int m_custom_count = 0;
			public CustomLevelInfoObjective m_objective = CustomLevelInfoObjective.NORMAL;
			public bool m_include_default_reflection_probes = true;

			public void Serialize(JObject root)
			{
				root["exit_music_start_time"] = m_exit_music_start_time;
				root["exit_no_explosions"] = m_exit_no_explosions;
				root["alien_lava"] = m_alien_lava;
				root["custom_count"] = m_custom_count;
				root["objective"] = m_objective.ToString();
				root["reflection_probes"] = m_include_default_reflection_probes ? "Default" : "None";
			}

			public void Deserialize(JObject root)
			{
				this.m_exit_music_start_time = root["exit_music_start_time"].GetFloat(0.0f);
				this.m_exit_no_explosions = root["exit_no_explosions"].GetBool(false);
				this.m_alien_lava = root["alien_lava"].GetBool(false);
				this.m_custom_count = root["custom_count"].GetInt(0);
				this.m_objective = (CustomLevelInfoObjective)Enum.Parse(typeof(CustomLevelInfoObjective), root["objective"].GetString("NORMAL"));
				this.m_include_default_reflection_probes = root["reflection_probes"].GetString("Default") == "Default";
			}
		}

		public CustomLevelInfo custom_level_info = new CustomLevelInfo();


		public IEditor editor = null;

        public Level(IEditor e)
        {
            this.editor = e;

            dirty = false;

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				segment[i] = new Segment(this, i);
			}

			for (int i = 0; i < MAX_VERTICES; i++) {
				vertex[i] = new Vertex(i);
			}

			for (int i = 0; i < MAX_ENTITIES; i++) {
				entity[i] = new Entity(this, i);
			}

			selected_segment = -1;
			selected_side = -1;
			selected_vertex = -1;
			selected_entity = -1;
			m_texture_set_name = "Default";
		}

		// Compacts the segments down so all alive segments are together
		// at the start of the array, with no gaps in between.
		public void CompactLevelSegments()
		{
			// We want to minimize the number of "moves" done, so instead of
			// just sliding things down, we want to find gaps and take from the rear
			// to fill in the gaps.
			Func<int, int> get_last_alive_segment = (int start_seg) => {

				int curr = start_seg - 1;
				while (curr >= 0) {
					if (this.segment[curr].Alive == true) {
						return curr;
					}
					--curr;
				}

				return -1;
			};

			Func<int, int> get_next_dead_segment = (int start_seg) => {

				int curr = start_seg + 1;
				while (curr < MAX_SEGMENTS) {
					if (this.segment[curr].Alive == false) {
						return curr;
					}
					++curr;
				}

				return MAX_SEGMENTS;
			};

			Action<int, int> move_segment = (int dead_slot, int alive_slot) => {

				// First copy the alive segment into the dead slot
				this.segment[dead_slot].Copy(this.segment[alive_slot], true);

				// Patch up the neighbors so that they point to the next segment index
				Segment new_segment = this.segment[dead_slot];
				for (int side_idx = 0; side_idx < Segment.NUM_SIDES; ++side_idx) {
					int neighbor_segment_idx = new_segment.neighbor[side_idx];
					if (neighbor_segment_idx == -1) {
						// its a wall...
						continue;
					}

					// Patch the neighbor so that its links to [alive_slot] are now to [dead_slot]
					Segment neighbor_segment = this.segment[neighbor_segment_idx];
					for (int neighbor_side_idx = 0; neighbor_side_idx < Segment.NUM_SIDES; ++neighbor_side_idx) {
						if (neighbor_segment.neighbor[neighbor_side_idx] == alive_slot) {
							neighbor_segment.neighbor[neighbor_side_idx] = dead_slot;
						}
					}
				}

				// The [alive_slot] has been moved, so clear its flags...
				// NOTE: This doesn't adjust counts because we just moved
				this.segment[alive_slot].Alive = false;
				this.segment[alive_slot].marked = false;

				// Patch up the editor variables that reference segment indices
				if (this.next_segment == dead_slot) {
					// next_segment points to the next free segment -- swap it around
					this.next_segment = alive_slot;
				}
				if (this.selected_segment == alive_slot) {
					// selected_segment points to the currently selected segment -- swap it around
					this.selected_segment = dead_slot;
				}
			};

			int tail_alive_segment = get_last_alive_segment(MAX_SEGMENTS);
			int head_dead_segment = get_next_dead_segment(-1);
			Dictionary<int, int> segment_remapping = new Dictionary<int, int>();

			while (tail_alive_segment > head_dead_segment) {
				// There is a gap at [head_dead_segment], move [tail_alive_segment] to take its slot
				move_segment(head_dead_segment, tail_alive_segment);

				segment_remapping.Add(tail_alive_segment, head_dead_segment);

				// Find the next indices to move around
				tail_alive_segment = get_last_alive_segment(tail_alive_segment);
				head_dead_segment = get_next_dead_segment(head_dead_segment);
			}

			// adjust the next free segment to be the one after the last alive segment
			this.next_segment = tail_alive_segment + 1;

			//Patch up entities
			foreach (Entity entity in EnumerateAliveEntities()) {
				int new_segnum;
				if (segment_remapping.TryGetValue(entity.m_segnum, out new_segnum)) {
					entity.m_segnum = new_segnum;
				}
			}

			//////////////////
			// Validate
			//////////////////
			/*
			int num_found_alive = 0;
			bool found_dead = false;
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].alive) {
					System.Diagnostics.Debug.Assert(found_dead == false);
					++num_found_alive;
				} else {
					found_dead = true;
				}
			}
			System.Diagnostics.Debug.Assert(num_found_alive == this.num_segments);
			System.Diagnostics.Debug.Assert(this.next_segment == num_found_alive);
			System.Diagnostics.Debug.Assert(this.selected_segment == -1 || this.selected_segment < num_found_alive);
			*/
		}

		public int selected_segment {
			get { return m_selected_segment; }

			set {
				m_selected_segment = value;
#if OVERLOAD_LEVEL_EDITOR
				if (this == editor.LoadedLevel) {		//Don't update view when changing the undo_level
					if (editor.IsAutoCenter && (value != -1)) {
						editor.SetProjOffsetAllViews(editor.LoadedLevel.GetSelectedSegmentPos());
					}
				}
#endif
			}
		}

		public int selected_entity {
			get { return m_selected_entity; }

			set {
				m_selected_entity = value;
#if OVERLOAD_LEVEL_EDITOR
				if (this == editor.LoadedLevel) {		//Don't update view when changing the undo_level
					editor.EntityListSetSelectedEntity(value);
				}
#endif
			}
		}

		public void SerializeLevelProperties(JObject root)
		{
			root["next_segment"] = this.next_segment;
			root["next_vertex"] = this.next_vertex;
			root["next_entity"] = this.next_entity;
			root["selected_segment"] = this.selected_segment;
			root["selected_side"] = this.selected_side;
			root["selected_vertex"] = this.selected_vertex;
			root["selected_entity"] = this.selected_entity;
			root["num_segments"] = this.num_segments;
			root["num_vertices"] = this.num_vertices;
			root["num_entities"] = this.num_entities;
			root["num_marked_segments"] = this.num_marked_segments;
			root["num_marked_sides"] = this.num_marked_sides;
			root["num_marked_vertices"] = this.num_marked_vertices;
			root["num_marked_entities"] = this.num_marked_entities;
			root["texture_set"] = this.m_texture_set_name;
		}

		public void DeserializeLevelProperties(JObject root)
		{
			this.next_segment = root["next_segment"].GetInt(0);
			this.next_vertex = root["next_vertex"].GetInt(0);
			this.next_entity = root["next_entity"].GetInt(0);
			this.selected_segment = root["selected_segment"].GetInt(-1);
			this.selected_side = root["selected_side"].GetInt(-1);
			this.selected_vertex = root["selected_vertex"].GetInt(-1);
			this.selected_entity = root["selected_entity"].GetInt(-1);
			this.num_segments = root["num_segments"].GetInt(0);
			this.num_vertices = root["num_vertices"].GetInt(0);
			this.num_entities = root["num_entities"].GetInt(0);
			this.num_marked_segments = root["num_marked_segments"].GetInt(0);
			this.num_marked_sides = root["num_marked_sides"].GetInt(0);
			this.num_marked_vertices = root["num_marked_vertices"].GetInt(0);
			this.num_marked_entities = root["num_marked_entities"].GetInt(0);
			this.m_texture_set_name = root["texture_set"].GetString("Default");
		}

		public void Serialize(JObject root)
		{
			var levelProps = new JObject();
			SerializeLevelProperties(levelProps);
			root["properties"] = levelProps;

			var levelGlobalData = new JObject();
			global_data.SerializeLevelGlobalData(levelGlobalData);
			root["global_data"] = levelGlobalData;

			var cli_obj = new JObject();
			root["custom_level_info"] = cli_obj;
			custom_level_info.Serialize(cli_obj);

			var jVerts = new JObject();
			root["verts"] = jVerts;
			for (int i = 0; i < MAX_VERTICES; ++i) {
				if (this.vertex[i].alive == false) {
					// Skip verts that are not alive to save space
					continue;
				}

				var jVert = new JObject();
				this.vertex[i].Serialize(jVert);
				jVerts[i.ToString()] = jVert;
			}

			var jSegments = new JObject();
			root["segments"] = jSegments;
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Alive == false) {
					// Skip segments that are not alive to save space
					continue;
				}

				var jSegment = new JObject();
				this.segment[i].Serialize(jSegment);
				jSegments[i.ToString()] = jSegment;
			}

			var jEntities = new JObject();
			root["entities"] = jEntities;
			for (int i = 0; i < MAX_ENTITIES; ++i) {
				if (this.entity[i].alive == false) {
					// Skip entities that are not alive to save space
					continue;
				}

				var jEntity = new JObject();
				this.entity[i].Serialize(jEntity);
				jEntities[i.ToString()] = jEntity;
			}
		}

		public void Deserialize(JObject root)
		{
			// Only alive verts, entities, and segments are serialized, so mark
			// the current state of them to not alive, so when they deserialize
			// in, only those alive are deserialized and only those are marked
			// alive
			for (int i = 0; i < MAX_VERTICES; ++i) {
				this.vertex[i].alive = false;
			}
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				this.segment[i].Alive = false;
			}
			for (int i = 0; i < MAX_ENTITIES; ++i) {
				this.entity[i].alive = false;
			}

			var jVerts = root["verts"].GetObject();
			foreach (var kvp in jVerts) {
				int vertexIndex = int.Parse(kvp.Key);
				var jVert = kvp.Value.GetObject();
				this.vertex[vertexIndex].Deserialize(jVert);
			}

			var jSegments = root["segments"].GetObject();
			foreach (var kvp in jSegments) {
				int segmentIndex = int.Parse(kvp.Key);
				var jSegment = kvp.Value.GetObject();
				this.segment[segmentIndex].Deserialize(jSegment);
			}

			var jEntities = root["entities"].GetObject();
			foreach (var kvp in jEntities) {
				int entityIndex = int.Parse(kvp.Key);
				var jEntity = kvp.Value.GetObject();
				this.entity[entityIndex].Deserialize(jEntity);
			}

			DeserializeLevelProperties(root["properties"].GetObject());

			global_data.DeserializeLevelGlobalData(root["global_data"].GetObject());

			this.custom_level_info = new CustomLevelInfo();
			var cli_obj = root["custom_level_info"].GetObject();
			if (cli_obj != null) {
				custom_level_info.Deserialize(cli_obj);
			}

			DeserializeComplete();
		}

		void DeserializeComplete()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Alive) {
					this.segment[i].DeserializeComplete();
				}
			}

			for (int i = 0; i < MAX_ENTITIES; ++i) {
				if (this.entity[i].alive) {
					this.entity[i].DeserializeComplete();
				}
			}

#if OVERLOAD_LEVEL_EDITOR
			UpdateSideTextures();
#endif
		}

#if OVERLOAD_LEVEL_EDITOR
		public void TagConnectedSegments(Segment start_seg)
		{
			List<Segment> tagged = new List<Segment>(Level.MAX_SEGMENTS);

			tagged.Add(start_seg);
			start_seg.m_tag = true;
			int current = 0;

			//Build list of neighbors, and keep processing them until there aren't any more
			while (current < tagged.Count) {
				Segment seg = tagged[current];

				for (int s = 0; s < Segment.NUM_SIDES; s++) {
					int neighbor = seg.neighbor[s];
					if (neighbor != -1) {
						Segment neighbor_seg = segment[neighbor];
						if (!neighbor_seg.m_tag) {
							tagged.Add(segment[neighbor]);
							neighbor_seg.m_tag = true;
						}
					}
				}
				current++;
			}
		}

		public void UnTagAllSegments()
		{
			foreach (Segment s in EnumerateAliveSegments()) {
				s.m_tag = false;
			}
		}

		public IEnumerable<Segment> EnumerateTaggedSegments()
		{
			foreach (Segment s in EnumerateAliveSegments()) {
				if (s.m_tag) {
					yield return s;
				}
			}
		}

		public IEnumerable<Segment> EnumerateUntaggedSegments()
		{
			foreach (Segment s in EnumerateAliveSegments()) {
				if (!s.m_tag) {
					yield return s;
				}
			}
		}
#endif

		public IEnumerable<Vertex> EnumerateAliveVertices()
		{
			foreach (Vertex vert in vertex) {
				if (vert.alive) {
					yield return vert;
				}
			}
		}

#if OVERLOAD_LEVEL_EDITOR
		public IEnumerable<Vertex> EnumerateMarkedVertices()
		{
			foreach (Vertex vert in EnumerateAliveVertices()) {
				if (vert.marked) {
					yield return vert;
				}
			}
		}

		public IEnumerable<Vertex> EnumerateTaggedVertices()
		{
			foreach (Vertex vert in EnumerateAliveVertices()) {
				if (vert.m_tag) {
					yield return vert;
				}
			}
		}
#endif

		public IEnumerable<Segment> EnumerateAliveSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Alive) {
					yield return this.segment[i];
				}
			}
		}

		public IEnumerable<int> EnumerateAliveSegmentIndices()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Alive) {
					yield return i;
				}
			}
		}

		public IEnumerable<Segment> EnumerateVisibleSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Visible) {
					yield return this.segment[i];
				}
			}
		}

		public IEnumerable<Segment> EnumerateMarkedSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Visible && this.segment[i].marked) {
					yield return this.segment[i];
				}
			}
		}

		public IEnumerable<Segment> EnumerateUnmarkedSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Visible && !this.segment[i].marked) {
					yield return this.segment[i];
				}
			}
		}

#if OVERLOAD_LEVEL_EDITOR
		public IEnumerable<Segment> EnumerateHiddenSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; ++i) {
				if (this.segment[i].Alive && this.segment[i].m_hidden) {
					yield return this.segment[i];
				}
			}
		}
#endif

		public IEnumerable<Entity> EnumerateAliveEntities()
		{
			foreach (Entity entity in this.entity) {
				if (entity.alive) {
					yield return entity;
				}
			}
		}

		public IEnumerable<Entity> EnumerateMarkedEntities()
		{
			foreach (Entity entity in this.entity) {
				if (entity.alive && entity.marked) {
					yield return entity;
				}
			}
		}

		//Return all alive entities of specified type
		public IEnumerable<Entity> EnumerateAliveEntities(EntityType type)
		{
			foreach (Entity entity in EnumerateAliveEntities()) {
				if (entity.Type == type) {
					yield return entity;
				}
			}
		}

		public IEnumerable<int> EnumerateAliveEntityIndices()
		{
			for (int i = 0; i < MAX_ENTITIES; ++i) {
				if (this.entity[i].alive) {
					yield return i;
				}
			}
		}

		public Vector3 GetSegmentCenter(int segmentIdx)
		{
			if (segmentIdx < 0 || segmentIdx >= MAX_SEGMENTS || this.segment[segmentIdx].Alive == false) {
				return Vector3.Zero;
			}

			var segment = this.segment[segmentIdx];

			Vector3 centroid = Vector3.Zero;
			for (int i = 0; i < 8; ++i) {
				var vIdx = segment.vert[i];
				var vData = this.vertex[vIdx];
				centroid += vData.position;
			}
			const float scale = 1.0f / 8.0f;
			centroid *= scale;

			return centroid;
		}

		//Refresh the GMeshes on the specified side
		public void RefreshSideGMeshes(Side side, bool check_for_decal_issues)
		{
			System.Diagnostics.Debug.Assert(side != null);

			for (int k = 0; k < Side.NUM_DECALS; k++) {
				string this_decal_issues;
				side.decal[k].MaybeUpdateGMesh( check_for_decal_issues, out this_decal_issues );

#if !OVERLOAD_LEVEL_EDITOR
				if( check_for_decal_issues && this_decal_issues.Length > 0 ) {
					UnityEngine.Debug.LogError( this_decal_issues );
				}
#endif
			}
		}

		public void RefreshAllGMeshes( bool check_for_decal_issues )
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (!segment[i].Alive) {
					continue;
				}

				for (int j = 0; j < Segment.NUM_SIDES; j++) {
					RefreshSideGMeshes( segment[i].side[j], check_for_decal_issues );
				}
			}
		}

		public interface IHasher
		{
			void Hash( int data );
			void Hash( float data );
			void Hash( bool data );
			void Hash( string data );
		}

		public void HashGeometry( IHasher hasher, bool considerNonGeometryData )
		{
			for( int i = 0; i < MAX_VERTICES; ++i ) {
				if( !this.vertex[i].alive ) {
					continue;
				}

				Vertex v = this.vertex[i];

				hasher.Hash( i );
				hasher.Hash( v.position.X );
				hasher.Hash( v.position.Y );
				hasher.Hash( v.position.Z );
			}

			for( int i = 0; i < MAX_SEGMENTS; ++i ) {
				if( !this.segment[i].Alive ) {
					continue;
				}

				Segment s = this.segment[i];

				hasher.Hash( i );
				foreach( int vidx in s.vert ) {
					hasher.Hash( vidx );
				}
				foreach( int nidx in s.neighbor ) {
					hasher.Hash( nidx );
				}

				for( int sidx = 0; sidx < 6; ++sidx ) {
					Side side = s.side[sidx];

					if( s.neighbor[sidx] == -1 ) {
						if( considerNonGeometryData ) {
							hasher.Hash( side.tex_name );
						}
						hasher.Hash( side.is_lava );
						hasher.Hash( side.deformation_height );
						hasher.Hash( side.deformation_preset );

						if( considerNonGeometryData ) {
							foreach (var uv in side.uv) {
								hasher.Hash(uv.X);
								hasher.Hash(uv.Y);
							}
							foreach (var uv2 in side.uv2) {
								hasher.Hash(uv2.X);
								hasher.Hash(uv2.Y);
							}
							foreach (var uv3 in side.uv3) {
								hasher.Hash(uv3.X);
								hasher.Hash(uv3.Y);
							}
						}
					}

					foreach( var d in side.decal ) {
						if( d == null || string.IsNullOrEmpty(d.mesh_name) || d.gmesh == null ) {
							continue;
						}

						var decal = d.gmesh;

						if( considerNonGeometryData ) {
							foreach( var c in decal.m_color ) {
								hasher.Hash( c.X );
								hasher.Hash( c.Y );
								hasher.Hash( c.Z );
							}

							foreach( var t in decal.m_tex_name ) {
								hasher.Hash( t );
							}
						}

						foreach( var v in decal.m_vertex ) {
							hasher.Hash( v.X );
							hasher.Hash( v.Y );
							hasher.Hash( v.Z );
						}

						foreach( var t in decal.m_triangle ) {
							if( considerNonGeometryData ) {
								hasher.Hash( t.flags );
								hasher.Hash( t.tex_index );
							}
							for( int ti = 0; ti < 3; ++ti ) {
								var n = t.normal[ti];
								var v = t.vert[ti];
								hasher.Hash( v );
								if( considerNonGeometryData ) {
									var uv = t.tex_uv[ti];
									hasher.Hash( uv.X );
									hasher.Hash( uv.Y );
								}
								hasher.Hash( n.X );
								hasher.Hash( n.Y );
								hasher.Hash( n.Z );
							}
						}
					}
				}
			}
		}

		public class LocatorInfo
		{
			public LocatorInfo(OpenTK.Vector3 pos, OpenTK.Quaternion rot)
			{
				this.Pos = pos;
				this.Rotation = rot;
			}

			public OpenTK.Vector3 Pos;
			public OpenTK.Quaternion Rotation;
		}

		public LocatorInfo[] ExtractRobotSpawnPoints()
		{
			LocatorInfo[] robot_spawn_points = EnumerateAliveEntityIndices()
				 .Select(entity_idx => this.entity[entity_idx])
				 .Where(entity_src => (entity_src.Type == OverloadLevelEditor.EntityType.SPECIAL) && (entity_src.SubType == (int)OverloadLevelEditor.SpecialSubType.ROBOT_SPAWN_POINT))
				 .Select(entity_src => new LocatorInfo(entity_src.position, entity_src.rotation.ExtractRotation()))
				 .ToArray();
			return robot_spawn_points;
		}

		public void HashRobotSpawnPoints(IHasher hasher)
		{
			var locators = ExtractRobotSpawnPoints();
			hasher.Hash(locators.Length);
			for (int i = 0, cnt = locators.Length; i < cnt; ++i) {
				hasher.Hash(locators[i].Pos.X);
				hasher.Hash(locators[i].Pos.Y);
				hasher.Hash(locators[i].Pos.Z);
				hasher.Hash(locators[i].Rotation.X);
				hasher.Hash(locators[i].Rotation.Y);
				hasher.Hash(locators[i].Rotation.Z);
				hasher.Hash(locators[i].Rotation.W);
			}
		}

		public DMesh GetDMeshByName(string dmeshName)
		{
			return this.editor.GetDMeshByName(dmeshName);
		}
    }
}