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

namespace OverloadLevelEditor
{
	class MathUtils
	{
		public static float Remap(float original_value, float old_range_min, float old_range_max, float new_range_min, float new_range_max)
		{
			if (old_range_min == old_range_max) {
				// This is basically saying to scale by infinity, since a single point value is being expanded into a continuous range.
				//  Just fail out and return the original value.
				return original_value;
			} else {
				// Setting the new range to be a single value is fine - it just remaps any input value to that one new value.
				float range_position = (original_value - old_range_min) / (old_range_max - old_range_min);
				return new_range_min + range_position * (new_range_max - new_range_min);
			}
		}
	}
}