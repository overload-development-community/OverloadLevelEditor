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
using OpenTK;
using Newtonsoft.Json.Linq;

namespace OverloadLevelEditor
{
	public enum DeformPreset
	{
		NONE,
		PLAIN_NOISE,
		FRACTAL_NOISE,
		BILLOW_NOISE,
		RIDGED_NOISE,
		V_STRIPES,
		H_RIDGES,
		POINTY,
      
		NUM,
	}

	public class LevelGlobalData
	{
		public int grid_size = 6;
		public int pre_smooth = 4;
		public int post_smooth = 1;
		public float simplify_strength = 0f;

		public const int MAX_DEFORM_PRESETS = 4;
		public DeformPreset[] deform_presets = new DeformPreset[MAX_DEFORM_PRESETS];

		public LevelGlobalData()
		{
			for (int i = 0; i < MAX_DEFORM_PRESETS; i++) {
				deform_presets[i] = DeformPreset.NONE;
			}

			deform_presets[0] = DeformPreset.PLAIN_NOISE;
		}

		public void SerializeLevelGlobalData(JObject root)
		{
			root["grid_size"] = this.grid_size;
			root["pre_smooth"] = this.pre_smooth;
			root["post_smooth"] = this.post_smooth;
			root["simplify_strength"] = this.simplify_strength;
			root["deform_presets0"] = this.deform_presets[0].ToString();
			root["deform_presets1"] = this.deform_presets[1].ToString();
			root["deform_presets2"] = this.deform_presets[2].ToString();
			root["deform_presets3"] = this.deform_presets[3].ToString();
		}

		public void DeserializeLevelGlobalData(JObject root)
		{
			this.grid_size = root["grid_size"].GetInt(8);
			this.pre_smooth = root["pre_smooth"].GetInt(3);
			this.post_smooth = root["post_smooth"].GetInt(0);
			this.simplify_strength = root["simplify_strength"].GetFloat(0f);
			this.deform_presets[0] = (DeformPreset)Enum.Parse(typeof(DeformPreset), root["deform_presets0"].GetString("PLAIN_NOISE"));
			this.deform_presets[1] = (DeformPreset)Enum.Parse(typeof(DeformPreset), root["deform_presets1"].GetString("NONE"));
			this.deform_presets[2] = (DeformPreset)Enum.Parse(typeof(DeformPreset), root["deform_presets2"].GetString("NONE"));
			this.deform_presets[3] = (DeformPreset)Enum.Parse(typeof(DeformPreset), root["deform_presets3"].GetString("NONE"));
		}
	}
}