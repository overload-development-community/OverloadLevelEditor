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

public static class Assert
{
    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void True(bool condition)
    {
        DoTrue(condition, null);
    }

    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void True(bool condition, string message)
    {
        DoTrue(condition, message);
    }

    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    private static void DoTrue(bool condition, string message)
    {
        if (Debug.isDebugBuild && !condition)
        {
            System.Diagnostics.StackFrame f = new System.Diagnostics.StackTrace(true).GetFrame(2);
            string new_message = "Assertion failed in " + f.GetMethod() + " (" + f.GetFileName() + ":" + f.GetFileLineNumber() + ")";
            if (! string.IsNullOrEmpty(message))
            {
                new_message += "\n" + message;
            }

            Debug.Assert(condition, new_message);
            HaltWhen(!condition);
        }
    }

    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void HaltWhen(bool condition)
    {
        if (condition)
        {
            if (!Application.isEditor)
            {
                Application.Quit();
            }
            else
            {
                Debug.Break();
            }
        }
    }
}
