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
   public partial class Editor : Form
   {
		public void KeyboardDown(KeyEventArgs e, GLView view)
		{
			//Utility.DebugLog("Got keycode " + e.KeyCode.ToString() + " - data: " + e.KeyData.ToString() + " - value: " + e.KeyValue.ToString() + " - alt: " + e.Alt.ToString());

			switch (e.KeyCode) {
				case Keys.NumPad1:
					RotateMarkedElements(view, -1f, e.Alt);
					break;
				case Keys.NumPad3:
					RotateMarkedElements(view, 1f, e.Alt);
					break;
				case Keys.NumPad7:
					ScaleMarkedElements(view, false, e.Alt);
					break;
				case Keys.NumPad9:
					ScaleMarkedElements(view, true, e.Alt);
					break;
				case Keys.NumPad4:
					MoveMarkedElements(view, -Vector3.UnitX, e.Alt);
					break;
				case Keys.NumPad6:
					MoveMarkedElements(view, Vector3.UnitX, e.Alt);
					break;
				case Keys.NumPad8:
					MoveMarkedElements(view, Vector3.UnitY, e.Alt);
					break;
				case Keys.NumPad2:
					MoveMarkedElements(view, -Vector3.UnitY, e.Alt);
					break;
				case Keys.Subtract:
					MoveMarkedElements(view, Vector3.UnitZ, e.Alt);
					break;
				case Keys.A:
					if (!e.Control) {
						if (e.Shift) {
							AddPolygonFromMarkedVerts(); 
						} else {
							AddVertAtMouse(view);
						}
					}
					break;
				case Keys.Add:
					break;
				case Keys.R:
					CycleScaleMode(e.Shift);
					UpdateOptionLabels();
					break;
				case Keys.Tab:
				case Keys.Oemtilde:
					CycleEditMode(e.Shift);
					break;
				case Keys.Insert:
					if (e.Control) {
						ExtrudeMarkedPolys();
					} else if (e.Shift) {
						ExtrudeMarkedVerts();
					} else {
						ExtrudeSelectedPoly();
					}
					break;
				case Keys.I:
					if (e.Control) {
						ExtrudeMarkedPolys();
					} else if (e.Shift) {
						ExtrudeMarkedVerts();
					} else {
						ExtrudeSelectedPoly();
					}
					break;
				case Keys.Delete:
					DeleteMarked();
					break;
				case Keys.Space:
					if (e.Modifiers == Keys.Control) {
						ClearAllMarked();
					} else if (e.Modifiers == Keys.Shift) {
						ToggleMarkAll();
					} else {
						ToggleMarkSelected();
					}
					break;
				case Keys.F:
					if (e.Control) {
						SaveStateForUndo("Flip normal");
						m_dmesh.ReverseMarkedPolys();
						RefreshGeometry();
					} else {
						FitFrameAll(e.Shift, view);
					}
					break;
				case Keys.G:
					if (e.Modifiers == Keys.Shift) {
						CycleGridDisplay();
					} else if (e.Modifiers == Keys.Control) {
						SnapMarkedElementsToGrid();
					}
					break;
				case Keys.Q:
					if (e.Modifiers == Keys.Shift) {
						MarkCoplanarPolys();
					}
					break;
				case Keys.T:
					if (e.Modifiers == Keys.Shift) {
						m_dmesh.UVAlignToPoly();
						RefreshGeometry();
					} else if (e.Modifiers == Keys.Control) {
						m_dmesh.UVDefaultMapMarkedPolys();
						RefreshGeometry();
					}
					break;
				case Keys.J:
					break;
				case Keys.OemCloseBrackets:
					if (e.Modifiers == Keys.Shift) {
						ChangeGridSpacing(1);
					} else {
						ChangeGridSnap(1);
					}
					break;
				case Keys.OemOpenBrackets:
					if (e.Modifiers == Keys.Shift) {
						ChangeGridSpacing(-1);
					} else {
						ChangeGridSnap(-1);
					}
					break;
				case Keys.N:
					break;
				case Keys.C:
					if (e.Modifiers == Keys.Control) {
						CopyMarkedPolys();
					} else {
						m_dmesh.CycleSelectedPoly(e.Shift);
						RefreshGeometry(true);
					}
					break;
				case Keys.V:
					if (e.Modifiers == Keys.Control) {
						PasteBufferPolys();
					} else {
						m_dmesh.CycleSelectedVert(e.Shift);
						RefreshGeometry(true);
					}
					break;
				case Keys.X:
					//CycleSelectedSide(e.Shift);
					if (e.Modifiers == Keys.Shift || e.Modifiers == (Keys.Shift | Keys.Control)) {
						MaybeCutPolygon(view, (e.Modifiers != Keys.Shift));
					}
					break;
				case Keys.Up:
					MoveMarkedSideTextures(Vector2.UnitY, e.Alt);
					break;
				case Keys.Down:
					MoveMarkedSideTextures(-Vector2.UnitY, e.Alt);
					break;
				case Keys.Left:
					if (e.Control) {
						RotateMarkedSideTextures(1f, e.Alt);
					} else {
						MoveMarkedSideTextures(Vector2.UnitX, e.Alt);
					}
					break;
				case Keys.Right:
					if (e.Control) {
						RotateMarkedSideTextures(-1f, e.Alt);
					} else {
						MoveMarkedSideTextures(-Vector2.UnitX, e.Alt);
					}
					break;
				case Keys.F1:
					CycleVisibilityType();
               break;
				case Keys.F2:
					break;
				case Keys.F3:
					break;
				default:
					//Utility.DebugLog("Got keydown " + e.KeyCode.ToString());
					break;
			}
		}
   }
}
