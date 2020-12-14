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
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace OverloadLevelEditor
{
	public partial class UVEditor : Form
	{
		public static float GRID_SNAP = 0.125f;

		// Get X component of unit vector from an angle
		public float AngleToX(float angle)
		{
			return (float)Math.Sin(angle);
		}

		// Get Y component of unit vector from an angle
		public float AngleToY(float angle)
		{
			return -(float)Math.Cos(angle);
		}

		public Color AdjustColor(Color c, int amt)
		{
			return Color.FromArgb(Clamp(c.R + amt, 0, 255), Clamp(c.G + amt, 0, 255), Clamp(c.B + amt, 0, 255));
		}

		public int Clamp(int val, int min, int max)
		{
			return Math.Min(max, Math.Max(min, val));
		}

		public float Clamp(float val, float min, float max)
		{
			return Math.Min(max, Math.Max(min, val));
		}

		// Change mouse coordinates to world coordinates
		public float TranslateXCoordinate(float x)
		{
			x = x - view.Width / 2;
			x = (x * zoom[zoom_level]) + cam_x;
			return x;
		}

		public float TranslateYCoordinate(float y)
		{
			y = y - view.Height / 2;
			y = (y * zoom[zoom_level]) + cam_y;
			return y;
		}

		// Change world coordinates to mouse coordinates
		public float ReverseX(float x)
		{
			x = ((x - cam_x) / zoom[zoom_level]);
			x = x + view.Width / 2;
			return x;
		}

		public float ReverseY(float y)
		{
			y = ((y - cam_y) / zoom[zoom_level]);
			y = y + view.Height / 2;
			return y;
		}

		public float Unscale(float v)
		{
			v = (v / zoom[zoom_level]);
			return v;
		}

		public void IncreaseZoom()
		{
			zoom_level = Math.Min(zoom_level + 1, zoom.GetUpperBound(0));
			view.Refresh();
		}

		public void DecreaseZoom()
		{
			zoom_level = Math.Max(zoom_level - 1, 0);
			view.Refresh();
		}

		public void DrawCircle(Graphics g, Pen p, float x, float y, float w, float h)
		{
			g.DrawEllipse(p, ReverseX(x - w), ReverseY(y - h), Unscale(w * 2), Unscale(h * 2));
		}

		public void DrawBox(Graphics g, Pen p, float x, float y, float w, float h)
		{
			g.DrawRectangle(p, ReverseX(x - w), ReverseY(y - h), Unscale(w * 2), Unscale(h * 2));
		}

		public void DrawBoxCorners(Graphics g, Pen p, float x1, float y1, float x2, float y2)
		{
			float xx = x2;
			float yy = y2;
			if (x1 > x2) {
				x2 = x1;
				x1 = xx;
			}
			if (y1 > y2) {
				y2 = y1;
				y1 = yy;
			}
			g.DrawRectangle(p, ReverseX(x1), ReverseY(y1), Unscale(x2 - x1), Unscale(y2 - y1));
		}

		public void DrawText(Graphics g, Pen p, float x, float y, String s)
		{
			g.DrawString(s, Font, p.Brush, (float)ReverseX(x), (float)ReverseY(y));
		}

		public void DrawLine(Graphics g, Pen p, float x1, float y1, float x2, float y2)
		{
			g.DrawLine(p, ReverseX(x1), ReverseY(y1), ReverseX(x2), ReverseY(y2));
		}

		public void DrawCross(Graphics g, Pen p, float x, float y)
		{
			DrawLine(g, p, x - 8, y - 8, x + 8, y + 8);
			DrawLine(g, p, x + 8, y - 8, x - 8, y + 8);
		}

		public void DrawDirection(Graphics g, Pen p, float x, float y, float a)
		{
			float ax = x + (int)(6.0f * AngleToX(Utility.RAD_90 * (float)a));
			float ay = y + (int)(6.0f * AngleToY(Utility.RAD_90 * (float)a));
			DrawLine(g, p, x, y, ax, ay);
			DrawBox(g, p, ax, ay, 1, 1);
		}

		public const float GRID_W = 1;
		public const float GRID_H = 1;

		public void DrawGrid(Graphics g)
		{
			Pen p = Pens.Gray;

			for (int i = -16; i < 16; i++) {
				if (Math.Abs(i) % 4 != 0) {
					DrawLine(g, p_grid[0], i * 0.0625f, -1f, i * 0.0625f, 1f);
				}
			}
			for (int i = -16; i < 16; i++) {
				if (Math.Abs(i) % 4 != 0) {
					DrawLine(g, p_grid[0], -1f, i * 0.0625f, 1f, i * 0.0625f);
				}
			}

			for (int i = -32; i < 32; i++) {
				if (Math.Abs(i) % 4 != 0) {
					DrawLine(g, p_grid[1], i * 0.25f, -8f, i * 0.25f, 8f);
				}
			}
			for (int i = -32; i < 32; i++) {
				if (Math.Abs(i) % 4 != 0) {
					DrawLine(g, p_grid[1], -8f, i * 0.25f, 8f, i * 0.25f);
				}
			}

			for (int i = -8; i < 9; i++) {
				if (i != 0) {
					DrawLine(g, p_grid[2], i, -8f, i, 8f);
				}
			}
			for (int i = -8; i < 9; i++) {
				if (i != 0) {
					DrawLine(g, p_grid[2], -8f, i, 8f, i);
				}
			}

			DrawBox(g, p_grid[3], 0f, 0f, 8f, 8f);
			DrawLine(g, p_grid[4], -8f, 0f, 8f, 0f);
			DrawLine(g, p_grid[4], 0f, -8f, 0f, 8f);
		}

		public Image cur_texture = null;

		public void SetTexture(Image tex, bool from_poly = false)
		{
			if (!from_poly || tex_from_sel_poly) {
				if (tex != null) {
					cur_texture = tex;
					this.Refresh();
				}
			}
		}

		Pen[] p_grid;
		Pen[] p_uv;
		Pen p_drag_box = new Pen(Color.FromArgb(100, 50, 150, 250));
		
		public void DrawFilledBox(Graphics g, Color c, int a, float x, float y, float w, float h)
		{
			g.FillRectangle(new SolidBrush(Color.FromArgb(a, c.R, c.G, c.B)), ReverseX(x - w), ReverseY(y - h), Unscale(w * 2), Unscale(h * 2));
		}

		public void InitPens()
		{
			p_grid = new Pen[5];
			p_grid[0] = new Pen(Color.FromArgb(12, 0, 0, 0));
			p_grid[1] = new Pen(Color.FromArgb(22, 0, 0, 0));
			p_grid[2] = new Pen(Color.FromArgb(38, 0, 0, 0));
			p_grid[3] = new Pen(Color.FromArgb(54, 0, 0, 0));
			p_grid[4] = new Pen(Color.FromArgb(54, 0, 0, 0));

			p_uv = new Pen[4];
			p_uv[0] = new Pen(Color.FromArgb(128, 128, 0));
			p_uv[1] = new Pen(Color.FromArgb(0, 255, 255));
			p_uv[2] = new Pen(Color.FromArgb(255, 128, 0));
			p_uv[3] = new Pen(Color.FromArgb(0, 0, 0));
		}

		public void DrawImage(Graphics g, Image image, float sx, float sy, float ex, float ey)
		{
			if (image != null) {
				PointF[] tile_pts = new PointF[3];
				tile_pts[0].X = ReverseX(sx);
				tile_pts[0].Y = ReverseY(sy);
				tile_pts[1].X = ReverseX(ex);
				tile_pts[1].Y = ReverseY(sy);
				tile_pts[2].X = ReverseX(sx);
				tile_pts[2].Y = ReverseY(ey);

				g.DrawImage(image, tile_pts);
			}
		}

		public void DrawUVs(Graphics g)
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();
			for (int i = 0; i < polys.Count; i++) {
				DrawUVsForPoly(g, polys[i], i);
			}
		}

		public void DrawUVsForPoly(Graphics g, DPoly p, int p_num)
		{
			for (int i = 0; i < p.num_verts; i++) {
				int j = (i + 1) % p.num_verts;
				if (p.uv_mark[i]) {
					DrawBox(g, p_uv[1], p.tex_uv[i].X, p.tex_uv[i].Y, 0.006f, 0.006f);
				} else {
					DrawBox(g, p_uv[0], p.tex_uv[i].X, p.tex_uv[i].Y, 0.006f, 0.006f);
				}
				if (IsSelected(p_num, i)) {
					DrawBox(g, p_uv[2], p.tex_uv[i].X, p.tex_uv[i].Y, 0.008f, 0.008f);
				}

				DrawLine(g, p_uv[0], p.tex_uv[i].X, p.tex_uv[i].Y, p.tex_uv[j].X, p.tex_uv[j].Y);
			}
		}

		public bool IsSelected(int poly, int p_vert)
		{
			return (selected_uv[0] == poly && selected_uv[1] == p_vert);
		}

		public void SelectPolyVert(int poly, int p_vert)
		{
			selected_uv[0] = poly;
			selected_uv[1] = p_vert;
		}

		public void SelectVert(Vector2 mouse_pos)
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			int close_poly = -1;
			int close_p_vert = -1;
			float close_dist_sq = 1f;
			float dist_sq;

			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					dist_sq = (polys[i].tex_uv[j] - mouse_pos).LengthSquared;
					if (dist_sq < close_dist_sq) {
						close_poly = i;
						close_p_vert = j;
						close_dist_sq = dist_sq;
					}
				}
			}

			if (close_poly > -1) {
				SelectPolyVert(close_poly, close_p_vert);
				editor.RefreshGeometry();
			}
		}

		public void UnMarkAllUVs()
		{
			foreach (DPoly p in editor.m_dmesh.polygon) {
				for (int i = 0; i < p.num_verts; i++) {
					p.uv_mark[i] = false;
				}
			}
		}

		public void MarkAllUVs()
		{
			foreach (DPoly p in editor.m_dmesh.polygon) {
				for (int i = 0; i < p.num_verts; i++) {
					p.uv_mark[i] = true;
				}
			}
		}

		public void ToggleMarkAll()
		{
			bool any_marked = false;
			foreach (DPoly p in editor.m_dmesh.polygon) {
				for (int i = 0; i < p.num_verts; i++) {
					any_marked = true;
					break;
				}
			}

			if (any_marked) {
				UnMarkAllUVs();
			} else {
				MarkAllUVs();
			}
		}

		public void ToggleMarkSelected()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					if (IsSelected(i, j)) {
						polys[i].uv_mark[j] = !polys[i].uv_mark[j];
					}
				}
			}
		}

		public void DragMark(Vector2 start_pos, Vector2 end_pos, bool add_to_selection)
		{
			if (!add_to_selection) {
				UnMarkAllUVs();
			}

			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();
			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					if (Utility.PointInsideAABBSort(polys[i].tex_uv[j], start_pos, end_pos)) {
						polys[i].uv_mark[j] = true;
					}
				}
			}
		}

		public void MoveUVs(Vector2 move_amt)
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();
			
			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					if (polys[i].uv_mark[j]) {
						polys[i].tex_uv[j] += move_amt;
					}
				}
			}
		}

		public void MoveMarked(Vector2 move_dir, bool large)
		{
			MoveUVs(move_dir * GRID_SNAP * (large ? 0.25f : 1f));
			this.Refresh();
			editor.RefreshGeometry();
		}

		public void SnapMarkedVerts()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					if (polys[i].uv_mark[j]) {
						polys[i].tex_uv[j] = Utility.SnapValue(polys[i].tex_uv[j], GRID_SNAP);
					}
				}
			}
		}

		public void SnapMarkedPairs()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			Vector2 avg;
			int[] closest_pvert = { -1, -1 };
			float closest_dist = 1f;
			float dist;

			for (int i = 0; i < polys.Count; i++) {
				for (int j = 0; j < polys[i].num_verts; j++) {
					if (polys[i].uv_mark[j]) {
						// Find the closest marked vert not in our polygon, and average our UV positions
						closest_dist = 1f;
						closest_pvert[0] = closest_pvert[1] = -1;

						for (int k = 0; k < polys.Count; k++) {
							if (k != i) {
								for (int h = 0; h < polys[k].num_verts; h++) {
									if (polys[k].uv_mark[h]) {
										dist = (polys[i].tex_uv[j] - polys[k].tex_uv[h]).Length;
										if (dist < closest_dist) {
											closest_dist = dist;
											closest_pvert[0] = k;
											closest_pvert[1] = h;
										}
									}
								}
							}
						}

						if (closest_pvert[0] > -1) {
							avg = (polys[i].tex_uv[j] + polys[closest_pvert[0]].tex_uv[closest_pvert[1]]) * 0.5f;
							polys[i].tex_uv[j] = avg;
							polys[closest_pvert[0]].tex_uv[closest_pvert[1]] = avg;
						}
					}
				}
			}
		}

		public void SnapToSelected()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			if (selected_uv[0] > -1) {
				for (int i = 0; i < polys.Count; i++) {
					for (int j = 0; j < polys[i].num_verts; j++) {
						if (polys[i].uv_mark[j]) {
							polys[i].tex_uv[j] = (polys[selected_uv[0]].tex_uv[selected_uv[1]]);
						}
					}
				}
			}
		}

		public List<Vector2> copy_uvs = new List<Vector2>();

		public void CopyUVs()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();

			if (polys.Count > 0) {
				copy_uvs.Clear();
				for (int i = 0; i < polys.Count; i++) {
					for (int j = 0; j < polys[i].num_verts; j++) {
						copy_uvs.Add(polys[i].tex_uv[j]);
					}
				}
			}
		}

		public void PasteUVs()
		{
			List<DPoly> polys = editor.m_dmesh.GetMarkedPolys();
			int count = 0;
			for (int i = 0; i < polys.Count; i++) {
				count += polys[i].num_verts;
			}

			if (polys.Count > 0 && copy_uvs.Count >= count) {
				count = 0;
				for (int i = 0; i < polys.Count; i++) {
					for (int j = 0; j < polys[i].num_verts; j++) {
						polys[i].tex_uv[j] = copy_uvs[count];
						count++;
					}
				}
			}

			this.Refresh();
			editor.RefreshGeometry();
		}
	}
}