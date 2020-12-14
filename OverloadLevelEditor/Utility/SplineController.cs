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
using System.Collections;
///using System.Collections.Generic;
///
/// can't use generics in Unity3D iPhone, so must make these changes:
/// List<Component> -- Component[], ArrayList of Components, or a custom class 


/// see http://www.unifycommunity.com/wiki/index.php?title=Spline_Controller for usage notes.

/// adapted for use in Unity3D iPhone by Matt Maker.

public enum eOrientationMode { NODE = 0, TANGENT }

[AddComponentMenu("Splines/Spline Controller")]
[RequireComponent(typeof(SplineInterpolator))]
public class SplineController : MonoBehaviour
{
	public GameObject SplineRoot;
	public float Duration = 10;
	public eOrientationMode OrientationMode = eOrientationMode.NODE;
	public eWrapMode WrapMode = eWrapMode.ONCE;
	public bool useDeltaTime = true;
	public bool AutoStart = true;
	public bool AutoClose = true;
	public bool HideOnExecute = true;


	SplineInterpolator mSplineInterp;
	public Transform[] mTransforms;

	void OnDrawGizmos()
	{
		Transform[] trans = GetTransforms();
		
		if (trans == null)
			return;
		
		if (trans.Length < 2)
			return;

		// This was semi-incorrectly changed in porting this to Unity's C#:
		// SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
		// that's not entirely correct; we need a *new* SplineInterpolator to draw the gizmos with.
		// The one that we have as a Component is the one used in game itself.
		// trying to use the same one for both means that the object just twitches in place,
		// since it then gets reset continuously by the gizmo drawing.
		// we might need to make SplineInterpolator into something other than a Monobehaviour --
		// or create an empty gameobject to AddComponent() it to (only in Editor)  --Matt M.
		GameObject GizmoSplineInterpolator = new GameObject();
		SplineInterpolator interp = (SplineInterpolator)GizmoSplineInterpolator.AddComponent(typeof(SplineInterpolator)); 
		SetupSplineInterpolator(interp, trans);
		interp.StartInterpolation(null, false, WrapMode);


		Vector3 prevPos = trans[0].position;
		for (int c = 1; c <= 100; c++)
		{
			float currTime = c * Duration / 100;
			Vector3 currPos = interp.GetHermiteAtTime(currTime);
			//Debug.Log("currPos = " + currPos + " at " + currTime);
			float mag = (currPos-prevPos).magnitude * 2;
			Gizmos.color = new Color(mag, 0, 0, 1);
			Gizmos.DrawLine(prevPos, currPos);
			prevPos = currPos;
		}
		
		DestroyImmediate(GizmoSplineInterpolator);
	}


	void Start()
	{
		mSplineInterp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;

		mTransforms = GetTransforms();

		if (HideOnExecute)
			DisableTransforms();

		if (AutoStart)
			FollowSpline();
	}

	void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		//Debug.Log(Time.realtimeSinceStartup + " " + this + " SetupSplineInterpolator " + interp + " " + trans);
		interp.Reset();

		float step = (AutoClose) ? Duration / trans.Length :
			Duration / (trans.Length - 1);

		//Debug.Log(Time.realtimeSinceStartup + " " + this + " step = " + step);
		int c;
		for (c = 0; c < trans.Length; c++)
		{
			if (OrientationMode == eOrientationMode.NODE)
			{
				interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
			}
			else if (OrientationMode == eOrientationMode.TANGENT)
			{
				Quaternion rot;
				if (c != trans.Length - 1)
					rot = Quaternion.LookRotation(trans[c + 1].position - trans[c].position, trans[c].up);
				else if (AutoClose)
					rot = Quaternion.LookRotation(trans[0].position - trans[c].position, trans[c].up);
				else
					rot = trans[c].rotation;

				interp.AddPoint(trans[c].position, rot, step * c, new Vector2(0, 1));
			}
		}

		if (AutoClose)
			interp.SetAutoCloseMode(step * c);
	}


	/// <summary>
	/// Returns children transforms, sorted by name.
	/// </summary>
	Transform[] GetTransforms()
	{
		if (SplineRoot != null)
		{
			SortedList mySL = new SortedList();
			
			foreach (Transform child in SplineRoot.transform)
				mySL.Add(child.name, child);

			mySL.Remove(SplineRoot.transform.name);
			Transform[] ret = new Transform[mySL.Count];
			for (int i=0;i<mySL.Count;i++)
				ret[i] = (Transform)mySL.GetByIndex(i);
			
			return ret;
		}

		return null;
	}

	/// <summary>
	/// Disables the spline objects, we don't need them outside design-time.
	/// </summary>
	void DisableTransforms()
	{
		if (SplineRoot != null)
		{
			SplineRoot.SetActiveRecursively(false);
		}
	}


	/// <summary>
	/// Starts the interpolation
	/// </summary>
	void FollowSpline()
	{
		//Debug.Log("FollowSpline");
		if (mTransforms.Length > 0)
		{
			SetupSplineInterpolator(mSplineInterp, mTransforms);
			mSplineInterp.StartInterpolation(null, true, WrapMode);
		}
	}
}