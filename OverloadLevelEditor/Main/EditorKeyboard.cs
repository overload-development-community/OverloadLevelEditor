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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// EDITOR - Keyboard
// Consolidates all the keyboard commands into one function
// Called by the views (they have focus while the main window is up)
// - The editor window doesn't actually capture keyboard events

namespace OverloadLevelEditor
{
	public partial class Editor : EditorDockContent
	{
		private struct KeyboardAssignment
		{
			Keys m_keydata;
			Keys m_optional;
			Action<KeyEventArgs, GLView> m_function;

			public KeyboardAssignment(Keys keydata, Keys optional, Action<KeyEventArgs, GLView> function)
			{
				m_keydata = keydata;
				m_optional = optional;
				m_function = function;
			}

			public Action<KeyEventArgs, GLView> Function { get { return m_function; } }

			public bool Matches(Keys keydata)
			{
				return ((keydata & ~m_optional) == m_keydata);
			}
		}

		private List<KeyboardAssignment> KeyboardAssignments;

		private void SetupKeyboard()
		{
			KeyboardAssignments = new List<KeyboardAssignment>() {
												//Key + required				 			Optional
												//modifier(s)								modifiers				//Code to call
				new KeyboardAssignment(Keys.F5,										Keys.None,		(e, v) => { RefreshAllGMeshes(); AddOutputText("Updated GMeshes"); this.Refresh(); }),
				new KeyboardAssignment(Keys.Return,									Keys.None,		(e, v) => { RefreshAllGMeshes(); AddOutputText("Updated GMeshes"); this.Refresh(); }),
				new KeyboardAssignment(Keys.NumPad4,								Keys.Alt,		(e, v) => MoveMarkedElements(v, -Vector3.UnitX, e.Alt)),
				new KeyboardAssignment(Keys.NumPad4 | Keys.Control,			Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.Y, -1f, e.Alt)),
				new KeyboardAssignment(Keys.NumPad6,								Keys.Alt,		(e, v) => MoveMarkedElements(v, Vector3.UnitX, e.Alt)),
				new KeyboardAssignment(Keys.NumPad6 | Keys.Control,			Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.Y, 1f, e.Alt)),
				new KeyboardAssignment(Keys.NumPad8,								Keys.Alt,		(e, v) => MoveMarkedElements(v, Vector3.UnitY, e.Alt)),
				new KeyboardAssignment(Keys.NumPad8 | Keys.Control,			Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.X, 1f, e.Alt)),
				new KeyboardAssignment(Keys.NumPad2,								Keys.Alt,		(e, v) => MoveMarkedElements(v, -Vector3.UnitY, e.Alt)),
				new KeyboardAssignment(Keys.NumPad2 | Keys.Control,			Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.X, -1f, e.Alt)),
				new KeyboardAssignment(Keys.NumPad7,								Keys.Control,	(e, v) => MaybeRotateMarkedSegments(AxisFromViewType(v.m_view_type), -Utility.RAD_90, e.Control)),
				new KeyboardAssignment(Keys.NumPad9,								Keys.Control,	(e, v) => MaybeRotateMarkedSegments(AxisFromViewType(v.m_view_type), Utility.RAD_90, e.Control)),
				new KeyboardAssignment(Keys.F | Keys.Shift,						Keys.Control,	(e, v) => FitFrameLevel(!e.Control, v)),
				new KeyboardAssignment(Keys.F,										Keys.Control,	(e, v) => FitFrameMarkedSelected(!e.Control, v)),
				new KeyboardAssignment(Keys.G,                              Keys.None,     (e, v) => FitFrameMarkedSelected(!e.Control, v, true)),
				new KeyboardAssignment(Keys.Subtract,								Keys.Alt,		(e, v) => MoveMarkedElements(v, Vector3.UnitZ, e.Alt)),
				new KeyboardAssignment(Keys.Subtract | Keys.Control,			Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.Z, 1f, e.Alt)),
				new KeyboardAssignment(Keys.Add,										Keys.Alt,		(e, v) => MoveMarkedElements(v, -Vector3.UnitZ, e.Alt)),
				new KeyboardAssignment(Keys.Add | Keys.Control,					Keys.Alt,		(e, v) => RotatedMarkedEntities(Axis.Z, -1f, e.Alt)),
				new KeyboardAssignment(Keys.Tab,										Keys.Shift,		(e, v) => CycleEditMode(e.Shift)),
				new KeyboardAssignment(Keys.Oemtilde,								Keys.Shift,		(e, v) => CycleEditMode(e.Shift)),
				new KeyboardAssignment(Keys.Insert,									Keys.Shift,		(e, v) => InsertSelectedSide(e.Shift)),
				new KeyboardAssignment(Keys.Insert | Keys.Control,				Keys.None,		(e, v) => InsertMarkedSides()),
				new KeyboardAssignment(Keys.I,										Keys.Shift,		(e, v) => InsertSelectedSide(e.Shift)),
				new KeyboardAssignment(Keys.I | Keys.Control,               Keys.None,     (e, v) => InsertMarkedSides()),
				new KeyboardAssignment(Keys.Delete,									Keys.None,		(e, v) => DeleteMarked()),
				new KeyboardAssignment(Keys.Space,									Keys.None,		(e, v) => ToggleMarkSelected()),
				new KeyboardAssignment(Keys.Space | Keys.Shift,					Keys.None,		(e, v) => ToggleMarkAll()),
				new KeyboardAssignment(Keys.Space | Keys.Control,				Keys.None,		(e, v) => ClearAllMarked()),
				new KeyboardAssignment(Keys.G | Keys.Shift,						Keys.None,		(e, v) => CycleGridDisplay()),
				new KeyboardAssignment(Keys.G | Keys.Control,					Keys.None,		(e, v) => SnapMarkedElementsToGrid()),
				new KeyboardAssignment(Keys.Q,										Keys.None,     (e, v) => MarkCoTextureSides()),
				new KeyboardAssignment(Keys.Q | Keys.Shift,						Keys.None,		(e, v) => MarkCoplanarSides()),
				new KeyboardAssignment(Keys.Q | Keys.Control,					Keys.None,		(e, v) => MarkWallSidesStraight()),
				new KeyboardAssignment(Keys.T | Keys.Shift,						Keys.None,		(e, v) => { m_level.UVAlignToSide(); RefreshGeometry(); }),
				new KeyboardAssignment(Keys.T | Keys.Control,					Keys.None,		(e, v) => { m_level.UVDefaultMapMarkedSides(); RefreshGeometry(); }),
				new KeyboardAssignment(Keys.J | Keys.Shift,						Keys.None,		(e, v) => JoinSelectedSideToMarked()),
				new KeyboardAssignment(Keys.L,                              Keys.None,     (e, v) => SetMarkedRobotsStation(!e.Shift)),
				new KeyboardAssignment(Keys.K,                              Keys.None,     (e, v) => SetMarkedRobotsNGP(!e.Shift)),
				new KeyboardAssignment(Keys.N,										Keys.None,		(e, v) => SelectAdjacentSegment()),
				new KeyboardAssignment(Keys.N | Keys.Shift,						Keys.None,		(e, v) => SelectOppositeSegment()),
				new KeyboardAssignment(Keys.C,										Keys.Shift,		(e, v) => CycleSelectedSegment(e.Shift)),
				new KeyboardAssignment(Keys.C | Keys.Control,					Keys.None,		(e, v) => CopyMarkedSegments(false)),
				new KeyboardAssignment(Keys.V | Keys.Control,					Keys.Shift,		(e, v) => PasteBufferSegments(true, e.Shift)),
				new KeyboardAssignment(Keys.X,										Keys.Shift,		(e, v) => CycleSelectedSide(e.Shift)),
				new KeyboardAssignment(Keys.E,										Keys.Shift,		(e, v) => CycleSelectedEntity(e.Shift)),
				new KeyboardAssignment(Keys.Up,										Keys.Alt,		(e, v) => MoveMarkedSideTextures(Vector2.UnitY, e.Alt)),
				new KeyboardAssignment(Keys.Down,									Keys.Alt,		(e, v) => MoveMarkedSideTextures(-Vector2.UnitY, e.Alt)),
				new KeyboardAssignment(Keys.Left,									Keys.Alt,		(e, v) => MoveMarkedSideTextures(Vector2.UnitX, e.Alt)),
				new KeyboardAssignment(Keys.Left | Keys.Control,				Keys.Alt,		(e, v) => RotateMarkedSideTextures(1f, e.Alt)),
				new KeyboardAssignment(Keys.Right,									Keys.Alt,		(e, v) => MoveMarkedSideTextures(-Vector2.UnitX, e.Alt)),
				new KeyboardAssignment(Keys.Right | Keys.Control,				Keys.Alt,		(e, v) => RotateMarkedSideTextures(-1f, e.Alt)),
				new KeyboardAssignment(Keys.F1,										Keys.None,		(e, v) => ShowDecalList()),
				new KeyboardAssignment(Keys.F2,										Keys.None,		(e, v) => ShowTextureList()),
				new KeyboardAssignment(Keys.F3,										Keys.None,		(e, v) => ShowTunnelBuilder()),
				new KeyboardAssignment(Keys.OemCloseBrackets,					Keys.None,		(e, v) => ChangeGridSnap(1)),
				new KeyboardAssignment(Keys.OemCloseBrackets | Keys.Shift,	Keys.None,		(e, v) => ChangeGridSpacing(1)),
				new KeyboardAssignment(Keys.OemOpenBrackets,						Keys.None,		(e, v) => ChangeGridSnap(-1)),
				new KeyboardAssignment(Keys.OemOpenBrackets | Keys.Shift,	Keys.None,		(e, v) => ChangeGridSpacing(-1)),
			};
		}

		public void KeyboardDown(KeyEventArgs e, GLView view)
		{
			//Utility.DebugLog("Got keycode " + e.KeyCode.ToString() + " - data: " + e.KeyData.ToString() + " - value: " + e.KeyValue.ToString() + " - alt: " + e.Alt.ToString());

			//Look for keycode
			foreach (KeyboardAssignment ka in KeyboardAssignments) {
				if (ka.Matches(e.KeyData))
					ka.Function(e, view);
			}
		}
	}
}
