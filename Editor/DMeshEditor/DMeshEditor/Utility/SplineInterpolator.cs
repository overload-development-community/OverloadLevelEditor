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
///can't use generics in Unity3D iPhone
///
/// List<SplineNode> -- ArrayList of SplineNode or custom class

public enum eEndPointsMode { AUTO, AUTOCLOSED, EXPLICIT }
public enum eWrapMode { ONCE, LOOP }
public delegate void OnEndCallback();

public class SplineInterpolator : MonoBehaviour
{
	eEndPointsMode mEndPointsMode = eEndPointsMode.AUTO;

	private float mCurrentTime;
	private float mStartTime = Time.realtimeSinceStartup;
	private float mCurrentTimePlusDelta = 0;
	private float mTimeSinceLevelStart = 0;	
	private int mCurrentIdx = 1;
	public bool useDeltaTime = true;
	public bool absoluteCoords = false;
	private bool mPaused = false;
	
	public GameObject relativeToTarget = null;
	
	internal class SplineNode
	{
		internal Vector3 Point;
		internal Quaternion Rot;
		internal float Time;
		internal Vector2 EaseIO;

		internal SplineNode(Vector3 p, Quaternion q, float t, Vector2 io) { Point = p; Rot = q; Time = t; EaseIO = io; }
		internal SplineNode(SplineNode o) { Point = o.Point; Rot = o.Rot; Time = o.Time; EaseIO = o.EaseIO; }
	}
	
	///List<SplineNode> mNodes = new List<SplineNode>();
	
	
	public class SplineNodeList : IList
	{
    	private SplineNode[] _contents = new SplineNode[255];//TODO make this number configurable or... something
    	private int _count;

    public SplineNodeList()
    {
        _count = 0;
    }

    // IList Members
    public int Add(object value)
    {
        if (_count < _contents.Length)
        {
            _contents[_count] = (SplineNode)value;
            _count++;

            return (_count - 1);
        }
        else
        {
            return -1;
        }
    }

    public void Clear()
    {
        _count = 0;
    }

    public bool Contains(object value)
    {
        bool inList = false;
        for (int i = 0; i < Count; i++)
        {
            if (_contents[i] == value)
            {
                inList = true;
                break;
            }
        }
        return inList;
    }

    public bool Contains(Vector3 value)
    {
        bool inList = false;
        for (int i = 0; i < Count; i++)
        {
            if (_contents[i].Point == value)
            {
                inList = true;
                break;
            }
        }
        return inList;
    }

    public bool Contains(Quaternion value)
    {
        bool inList = false;
        for (int i = 0; i < Count; i++)
        {
            if (_contents[i].Rot == value)
            {
                inList = true;
                break;
            }
        }
        return inList;
    }

    public bool Contains(float value)
    {
        bool inList = false;
        for (int i = 0; i < Count; i++)
        {
            if (_contents[i].Time == value)
            {
                inList = true;
                break;
            }
        }
        return inList;
    }

    public int IndexOf(object value)
    {
        int itemIndex = -1;
        for (int i = 0; i < Count; i++)
        {
            if (_contents[i] == (SplineNode)value)
            {
                itemIndex = i;
                break;
            }
        }
        return itemIndex;
    }

    public void Insert(int index, object value)
    {
        if ((_count + 1 <= _contents.Length) && (index < Count) && (index >= 0))
        {
            _count++;

            for (int i = Count - 1; i > index; i--)
            {
                _contents[i] = _contents[i - 1];
            }
            _contents[index] = (SplineNode)value;
        }
    }

    public bool IsFixedSize
    {
        get
        {
            return true;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public void Remove(object value)
    {
        RemoveAt(IndexOf(value));
    }

    public void RemoveAt(int index)
    {
        if ((index >= 0) && (index < Count))
        {
            for (int i = index; i < Count - 1; i++)
            {
                _contents[i] = _contents[i + 1];
            }
            _count--;
        }
    }

    public object this[int index]
    {
        get
        {
            return (object)_contents[index];
        }
        set
        {
            _contents[index] = (SplineNode)value;
        }
    }

    // ICollection Members

    public void CopyTo(System.Array array, int index)
    {
        int j = index;
        for (int i = 0; i < Count; i++)
        {
			array.SetValue(_contents[i], j);
            j++;
        }
    }

    public int Count
    {
        get
        {
            return _count;
        }
    }

    public bool IsSynchronized
    {
        get
        {
            return false;
        }
    }

    // Return the current instance since the underlying store is not
    // publicly available.
    public object SyncRoot
    {
        get
        {
            return this;
        }
    }

    // IEnumerable Members

    /*public IEnumerator GetEnumerator()
    {
        // Refer to the IEnumerator documentation for an example of
        // implementing an enumerator.
		do {
			yield return _contents[_index];
       		} while(++_index != _contents.Length);
       		
       		_index = 0;
   		}
	}*/
	
		//or you can do it this way, and let the compiler make the index for you
		public IEnumerator GetEnumerator() { 
			foreach (SplineNode nextspline in this._contents)
    			yield return nextspline;
  		}

    	public void PrintContents()
    	{
		Debug.Log("SplineNodeList has a capacity of " + _contents.Length + " and currently has " + _count + " elements.");
        	Debug.Log("SplineNodeList contents:");
        	for (int i = 0; i < Count; i++)
				Debug.Log(_contents[i]);
		}
	}

	private SplineNodeList mNodes = new SplineNodeList();
	private string mState = "";
	private bool mRotations;

	OnEndCallback mOnEndCallback;



	void Awake()
	{
		Reset();
	}

	public void StartInterpolation(OnEndCallback endCallback, bool bRotations, eWrapMode mode)
	{
		//Debug.Log(Time.realtimeSinceStartup + " " + this + " StartInterpolation " + endCallback + " " + bRotations + " " + mode);
		
		if (mState != "Reset")
			throw new System.Exception("First reset, add points and then call here");

		mState = mode == eWrapMode.ONCE ? "Once" : "Loop";
		mRotations = bRotations;
		mOnEndCallback = endCallback;

		SetInput();

		mPaused = false;

	}

	public void Reset()
	{
		//Debug.Log(Time.realtimeSinceStartup + " " + this + " Reset");
		mNodes.Clear();
		mState = "Reset";
		mCurrentIdx = 1;
		mCurrentTime = 0;
		mCurrentTimePlusDelta = 0;
		mTimeSinceLevelStart = 0;	
		mStartTime = Time.realtimeSinceStartup;
		mRotations = false;
		mEndPointsMode = eEndPointsMode.AUTO;
	}

	public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds, Vector2 easeInOut)
	{
		if (mState != "Reset")
			throw new System.Exception("Cannot add points after start");

		//Debug.Log("AddPoint " + pos + " " + timeInSeconds + " " + easeInOut);
		mNodes.Add(new SplineNode(pos, quat, timeInSeconds, easeInOut));
	}


	void SetInput()
	{
		if (mNodes.Count < 2)
			throw new System.Exception("Invalid number of points");

		if (mRotations)
		{
			for (int c = 1; c < mNodes.Count; c++)
			{
				SplineNode node = (SplineNode)mNodes[c];
				SplineNode prevNode = (SplineNode)mNodes[c - 1];

				// Always interpolate using the shortest path -> Selective negation
				if (Quaternion.Dot(node.Rot, prevNode.Rot) < 0)
				{
					node.Rot.x = -node.Rot.x;
					node.Rot.y = -node.Rot.y;
					node.Rot.z = -node.Rot.z;
					node.Rot.w = -node.Rot.w;
				}
			}
		}

		if (mEndPointsMode == eEndPointsMode.AUTO)
		{
			mNodes.Insert(0, mNodes[0]);
			mNodes.Add(mNodes[mNodes.Count - 1]);
		}
		else if (mEndPointsMode == eEndPointsMode.EXPLICIT && (mNodes.Count < 4))
			throw new System.Exception("Invalid number of points");
	}

	void SetExplicitMode()
	{
		if (mState != "Reset")
			throw new System.Exception("Cannot change mode after start");

		mEndPointsMode = eEndPointsMode.EXPLICIT;
	}

	public void SetAutoCloseMode(float joiningPointTime)
	{
		if (mState != "Reset")
			throw new System.Exception("Cannot change mode after start");

		mEndPointsMode = eEndPointsMode.AUTOCLOSED;

		
		mNodes.Add(new SplineNode(mNodes[0] as SplineNode));
		((SplineNode)mNodes[mNodes.Count - 1]).Time = joiningPointTime;

		Vector3 vInitDir =  (((SplineNode)(mNodes[1]               )).Point - ((SplineNode)(mNodes[0])).Point               ).normalized;
		Vector3 vEndDir =   (((SplineNode)(mNodes[mNodes.Count - 2])).Point - ((SplineNode)(mNodes[mNodes.Count - 1])).Point).normalized;
		float firstLength = (((SplineNode)(mNodes[1]               )).Point - ((SplineNode)(mNodes[0])).Point               ).magnitude;
		float lastLength =  (((SplineNode)(mNodes[mNodes.Count - 2])).Point - ((SplineNode)(mNodes[mNodes.Count - 1])).Point).magnitude;

		SplineNode firstNode = new SplineNode(mNodes[0] as SplineNode);
		firstNode.Point = ((SplineNode)(mNodes[0])).Point + vEndDir * firstLength;

		SplineNode lastNode = new SplineNode(mNodes[mNodes.Count - 1] as SplineNode);
		lastNode.Point = ((SplineNode)(mNodes[0])).Point + vInitDir * lastLength;

		mNodes.Insert(0, firstNode);
		mNodes.Add(lastNode);
	}

	void Update()
	{
		if (mState == "Reset" || mState == "Stopped" || mNodes.Count < 4) {
			return;
		}
		
		if (mPaused)
			return;
			
			mCurrentTimePlusDelta += Time.deltaTime;
			mTimeSinceLevelStart = Time.realtimeSinceStartup - mStartTime;

		if (useDeltaTime)
			mCurrentTime += mCurrentTimePlusDelta;
		else
			mCurrentTime = mTimeSinceLevelStart;
			
		//Debug.Log("+delta= " + mCurrentTimePlusDelta + " abs= " + mTimeSinceLevelStart );
			

		// We advance to next point in the path
		if (mCurrentTime >= ((SplineNode)(mNodes[mCurrentIdx + 1])).Time)
		{
			if (mCurrentIdx < mNodes.Count - 3)
			{
				mCurrentIdx++;
			}
			else
			{
				if (mState != "Loop")
				{
					mState = "Stopped";

					// We stop right in the end point
					if (absoluteCoords) {
						transform.position = ((SplineNode)(mNodes[mNodes.Count - 2])).Point;
					} else {
						if (relativeToTarget)
							transform.position = relativeToTarget.transform.position;

						transform.Translate( ((SplineNode)(mNodes[mNodes.Count - 2])).Point );
					}
					
					if (mRotations)
						transform.rotation = ((SplineNode)(mNodes[mNodes.Count - 2])).Rot;

					// We call back to inform that we are ended
					if (mOnEndCallback != null)
						mOnEndCallback();
				}
				else
				{
					mCurrentIdx = 1;
					mCurrentTime = 0;
					mCurrentTimePlusDelta = 0;
					mStartTime = Time.realtimeSinceStartup;
					mTimeSinceLevelStart = 0;	
				}
			}
		}

		if (mState != "Stopped")
		{
			// Calculates the t param between 0 and 1
			float param = (mCurrentTime - ((SplineNode)(mNodes[mCurrentIdx])).Time) / ( ((SplineNode)(mNodes[mCurrentIdx + 1])).Time - ((SplineNode)(mNodes[mCurrentIdx])).Time );

			// Smooth the param
			param = MathUtils.Ease(param, ((SplineNode)(mNodes[mCurrentIdx])).EaseIO.x, ((SplineNode)(mNodes[mCurrentIdx])).EaseIO.y);


			if (absoluteCoords)
				transform.position = GetHermiteInternal(mCurrentIdx, param);
			else
				if (relativeToTarget)
					transform.position = relativeToTarget.transform.position;
					
				transform.Translate( transform.position = GetHermiteInternal(mCurrentIdx, param) );
			
			if (mRotations)
			{
				transform.rotation = GetSquad(mCurrentIdx, param);
			}
		}
	}

	Quaternion GetSquad(int idxFirstPoint, float t)
	{
		Quaternion Q0 = ((SplineNode)(mNodes[idxFirstPoint - 1])).Rot;
		Quaternion Q1 = ((SplineNode)(mNodes[idxFirstPoint]    )).Rot;
		Quaternion Q2 = ((SplineNode)(mNodes[idxFirstPoint + 1])).Rot;
		Quaternion Q3 = ((SplineNode)(mNodes[idxFirstPoint + 2])).Rot;

		Quaternion T1 = MathUtils.GetSquadIntermediate(Q0, Q1, Q2);
		Quaternion T2 = MathUtils.GetSquadIntermediate(Q1, Q2, Q3);

		return MathUtils.GetQuatSquad(t, Q1, Q2, T1, T2);
	}



	public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		//Debug.Log(Time.realtimeSinceStartup + " " + this + " GetHermiteInternal " + idxFirstPoint + " " + t);
		float t2 = t * t;
		float t3 = t2 * t;

		Vector3 P0 = ((SplineNode)(mNodes[idxFirstPoint - 1])).Point;
		Vector3 P1 = ((SplineNode)(mNodes[idxFirstPoint]    )).Point;
		Vector3 P2 = ((SplineNode)(mNodes[idxFirstPoint + 1])).Point;
		Vector3 P3 = ((SplineNode)(mNodes[idxFirstPoint + 2])).Point;

		float tension = 0.5f;	// 0.5 equivale a catmull-rom

		Vector3 T1 = tension * (P2 - P0);
		Vector3 T2 = tension * (P3 - P1);

		float Blend1 = 2 * t3 - 3 * t2 + 1;
		float Blend2 = -2 * t3 + 3 * t2;
		float Blend3 = t3 - 2 * t2 + t;
		float Blend4 = t3 - t2;

		return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
	}


	public Vector3 GetHermiteAtTime(float timeParam)
	{
		if (timeParam >= ((SplineNode)(mNodes[mNodes.Count - 2])).Time)
			return ((SplineNode)(mNodes[mNodes.Count - 2])).Point;

		int c;
		for (c = 1; c < mNodes.Count - 2; c++)
		{
			if (((SplineNode)(mNodes[c])).Time > timeParam)
				break;
		}

		int idx = c - 1;
		float param = (timeParam - ((SplineNode)(mNodes[idx])).Time) / ( ((SplineNode)(mNodes[idx + 1])).Time - ((SplineNode)(mNodes[idx])).Time );
		param = MathUtils.Ease(param, ((SplineNode)(mNodes[idx])).EaseIO.x, ((SplineNode)(mNodes[idx])).EaseIO.y);

		return GetHermiteInternal(idx, param);
	}
	
	public void Pause (bool paused) {
		Debug.Log(this + " Pause " + paused);
		mPaused = !paused; // we want to be active when we get a pause message!
	}

}