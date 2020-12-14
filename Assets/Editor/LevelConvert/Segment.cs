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

using OpenTK;
using Newtonsoft.Json.Linq;

namespace OverloadLevelEditor
{
	public enum SideOrder
	{
		// Left (7623)
		LEFT,
		// Top (0473)
		TOP,
		// Right (0154)
		RIGHT,
		// Bottom (2651)
		BOTTOM,
		// Front (3210)
		FRONT,
		// Back (4567)
		BACK,
	}

    //This must mirror the Pathfinding enum in LevelMetadata.cs
    public enum PathfindingType { All, GB, None, NUM };
	public enum ExitSegmentType { Start, End, None, NUM };

	public partial class Segment
	{
		public const int NUM_SIDES = 6;
		public const int NUM_VERTS = 8;

		public int num;
		private bool m_alive;
		public bool marked;
		public bool m_dark;
		public PathfindingType m_pathfinding;
		public ExitSegmentType m_exit_segment_type;

#if OVERLOAD_LEVEL_EDITOR
		public bool m_tag;
		public bool m_hidden;
#endif

		public int[] neighbor = new int[NUM_SIDES];
		public Side[] side = new Side[NUM_SIDES];

		public int[] vert = new int[NUM_VERTS];

		public Level level;

		public int m_chunk_num;

		public Segment(Level lvl, int n)
		{
			level = lvl;
			num = n;

			for (int i = 0; i < NUM_SIDES; i++) {
				neighbor[i] = -1;
				side[i] = new Side(this, i);
			}

			Init();
		}

		public bool Alive 
		{
			get { return m_alive; }
			set { m_alive = value; }
		}

		public bool Visible 
		{
#if OVERLOAD_LEVEL_EDITOR
			get { return m_alive && !m_hidden; }
#else
			get { return m_alive; }
#endif
		}

		public void Init()
		{
			m_alive = false;
			marked = false;
            m_dark = false;
            m_pathfinding = PathfindingType.All;
			m_exit_segment_type = ExitSegmentType.None;
#if OVERLOAD_LEVEL_EDITOR
			m_tag = false;
			m_hidden = false;
#endif
		}

		public void Copy(Segment src, bool full)
		{
			m_alive = src.m_alive;
			marked = (full ? src.marked : true);
            m_pathfinding = src.m_pathfinding;
            m_dark = src.m_dark;

			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = src.vert[i];
			}

			for (int i = 0; i < NUM_SIDES; i++) {
				side[i].Copy(src.side[i], full);
				neighbor[i] = src.neighbor[i];
			}
		}

		public void Serialize(JObject root)
		{
			// NOTE: alive is skipped, as it is implied as true
			root["marked"] = this.marked;
			root["pathfinding"] = this.m_pathfinding.ToString();
			root["exitsegment"] = this.m_exit_segment_type.ToString();
			root["dark"] = this.m_dark;

			var jVerts = new JArray();
			for (int i = 0; i < NUM_VERTS; ++i) {
				jVerts.Add(this.vert[i]);
			}
			root["verts"] = jVerts;

			var jSides = new JArray();
			var jNeighbors = new JArray();
			for (int i = 0; i < NUM_SIDES; ++i) {
				jNeighbors.Add(this.neighbor[i]);

				var jSide = new JObject();
				jSides.Add(jSide);
				this.side[i].Serialize(jSide);
			}
			root["sides"] = jSides;
			root["neighbors"] = jNeighbors;
		}

		public void Deserialize(JObject root)
		{
			this.m_alive = true;
			this.marked = root["marked"].GetBool(false);
			JToken token = root["pathfinding"];
			if (!token.IsValid()) {
				this.m_pathfinding = PathfindingType.All;
			} else if (token.Type == JTokenType.Boolean) {
				this.m_pathfinding = token.GetBool(true) ? PathfindingType.All : PathfindingType.None;
			} else {
				this.m_pathfinding = token.GetEnum<PathfindingType>(PathfindingType.All);
			}

			token = root["exitsegment"];
			if (!token.IsValid()) {
				this.m_exit_segment_type = ExitSegmentType.None;
			} else if (token.Type == JTokenType.Boolean) {
				this.m_exit_segment_type = token.GetBool(true) ? ExitSegmentType.None: ExitSegmentType.End;
			} else {
				this.m_exit_segment_type = token.GetEnum<ExitSegmentType>(ExitSegmentType.None);
			}

			this.m_dark = root["dark"].GetBool(false);

			var jVerts = root["verts"].GetArray();
			for (int i = 0; i < NUM_VERTS; ++i) {
				this.vert[i] = jVerts[i].GetInt(0);
			}

			var jSides = root["sides"].GetArray();
			var jNeighbors = root["neighbors"].GetArray();
			for (int i = 0; i < NUM_SIDES; ++i) {
				this.neighbor[i] = jNeighbors[i].GetInt(-1);
				this.side[i].Deserialize(jSides[i].GetObject());
			}
		}

		public void DeserializeComplete()
		{
			for (int i = 0; i < NUM_SIDES; ++i) {
				this.side[i].DeserializeComplete();
			}
		}

		public Vector3 FindCenter()
		{
			Vector3 center = Vector3.Zero;
			for (int i = 0; i < NUM_VERTS; i++) {
				center += level.vertex[vert[i]].position;
			}

			center /= (NUM_VERTS);

			return center;
		}

	}
}