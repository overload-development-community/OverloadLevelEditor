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

using UnityEngine;
using System;

public class Mathfx
{
	public static float Hermite(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
	}

	public static float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
	}

	public static float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}

	public static float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static float SmoothStep(float x, float min, float max)
	{
		x = Mathf.Clamp(x, min, max);
		float v1 = (x - min) / (max - min);
		float v2 = (x - min) / (max - min);
		return -2 * v1 * v1 * v1 + 3 * v2 * v2;
	}

	public static float Lerp(float start, float end, float value)
	{
		return ((1.0f - value) * start) + (value * end);
	}

	public static Vector3 NearestPoint(Vector3 line_start, Vector3 line_end, Vector3 point)
	{
		Vector3 lineDirection = Vector3.Normalize(line_end - line_start);
		float closestPoint = Vector3.Dot((point - line_start), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
		return line_start + (closestPoint * lineDirection);
	}

	public static Vector3 NearestPointStrict(Vector3 line_start, Vector3 line_end, Vector3 point)
	{
		Vector3 fullDirection = line_end - line_start;
		Vector3 lineDirection = Vector3.Normalize(fullDirection);
		float closestPoint = Vector3.Dot((point - line_start), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
		return line_start + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
	}
	public static float Bounce(float x)
	{
		return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
	}

	// test for value that is near specified float (due to floating point inprecision)
	// all thanks to Opless for this!
	public static bool Approx(float val, float about, float range)
	{
		return ((Mathf.Abs(val - about) < range));
	}

	// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
	// compares the square of the distance to the square of the range as this
	// avoids calculating a square root which is much slower than squaring the range
	public static bool Approx(Vector3 val, Vector3 about, float range)
	{
		return ((val - about).sqrMagnitude < range * range);
	}

	/*
	  * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
	  * This is useful when interpolating eulerAngles and the object
	  * crosses the 0/360 boundary.  The standard Lerp function causes the object
	  * to rotate in the wrong direction and looks stupid. Clerp fixes that.
	  */
	public static float Clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
		float retval = 0.0f;
		float diff = 0.0f;

		if ((end - start) < -half) {
			diff = ((max - start) + end) * value;
			retval = start + diff;
		} else if ((end - start) > half) {
			diff = -((max - end) + start) * value;
			retval = start + diff;
		} else retval = start + (end - start) * value;

		// if (Debug.isDebugBuild) Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
		return retval;
	}
}