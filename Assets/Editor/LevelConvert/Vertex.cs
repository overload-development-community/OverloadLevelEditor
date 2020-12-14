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
	public partial class Vertex
	{
		public Vector3 position;
		public bool alive;
		public bool marked;
		public int num;

		public Vertex(int n)
		{
			alive = false;
			marked = false;
#if OVERLOAD_LEVEL_EDITOR
			m_tag = false;
#endif
			num = n;
			position = Vector3.Zero;
		}

		public void Serialize(JObject root)
		{
			// NOTE: alive is implied as true, not serializing it
			root["marked"] = this.marked;
			root["x"] = this.position.X;
			root["y"] = this.position.Y;
			root["z"] = this.position.Z;
		}

		public void Deserialize(JObject root)
		{
#if OVERLOAD_LEVEL_EDITOR
			this.m_tag = false;
#endif
			this.alive = true;
			this.marked = root["marked"].GetBool(false);
			this.position.X = root["x"].GetFloat(0.0f);
			this.position.Y = root["y"].GetFloat(0.0f);
			this.position.Z = root["z"].GetFloat(0.0f);
		}
	}
}