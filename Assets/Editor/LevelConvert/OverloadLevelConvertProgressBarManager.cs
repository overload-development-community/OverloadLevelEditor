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
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class OverloadLevelConverter
{
#if !OVERLOAD_LEVEL_EDITOR
    class ProgressBarManager
	{
		public ProgressBarManager(string title)
		{
			this.m_title = title ?? "Progress";
			this.m_phaseTickDelta = new Stack<float>();
			this.m_phaseTicksRemaining = new Stack<int>();
			this.m_phaseInfo = new Stack<string>();
			this.m_currPhaseDelta = 1.0f;
			this.m_currPhaseTicksRemaining = 1;
			this.m_currPhaseInfo = null;
			this.m_progress = 0.0f;
		}

		// Note: Each nested BeginPhase counts as a tick for a phase
		public void BeginPhase(string info, int numTicks)
		{
			Assert.True(numTicks >= 1);
			this.m_phaseTickDelta.Push(this.m_currPhaseDelta);
			this.m_phaseTicksRemaining.Push(this.m_currPhaseTicksRemaining);
			this.m_phaseInfo.Push(this.m_currPhaseInfo);

			this.m_currPhaseTicksRemaining = numTicks;
			this.m_currPhaseInfo = info ?? string.Empty;
			this.m_currPhaseDelta *= 1.0f / (float)numTicks;
		}

		public bool Tick(string info, bool cancellable)
		{
			Assert.True(this.m_currPhaseTicksRemaining > 0);
			--this.m_currPhaseTicksRemaining;
			this.m_progress += this.m_currPhaseDelta;

			if (info == null) {
				info = this.m_currPhaseInfo;
			}

			return NonTick(info, cancellable);
		}

		public bool NonTick(string info, bool cancellable)
		{
			bool res = false;
			if (cancellable) {
				res = EditorUtility.DisplayCancelableProgressBar(this.m_title, info, this.m_progress);
			} else {
				EditorUtility.DisplayProgressBar(this.m_title, info, this.m_progress);
			}

			while (this.m_currPhaseTicksRemaining == 0 && this.m_phaseTickDelta.Count > 0) {
				// Pop up a phase
				this.m_currPhaseDelta = this.m_phaseTickDelta.Pop();
				this.m_currPhaseInfo = this.m_phaseInfo.Pop();
				this.m_currPhaseTicksRemaining = this.m_phaseTicksRemaining.Pop();
			}

			return res;
		}

		public void Clear()
		{
			EditorUtility.ClearProgressBar();
		}

		string m_title;
		Stack<float> m_phaseTickDelta;
		Stack<int> m_phaseTicksRemaining;
		Stack<string> m_phaseInfo;
		float m_currPhaseDelta;
		int m_currPhaseTicksRemaining;
		string m_currPhaseInfo;
		float m_progress;
	}
#endif
}
