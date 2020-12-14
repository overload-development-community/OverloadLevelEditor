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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Diagnostics;


namespace OverloadLevelEditor
{
	public partial class SliderLabel : UserControl
	{
		private string m_label_value = "";
		private string m_label_text = "";
		private int m_scroll_tol = 15;
		private string m_tool_top = "";
		private int m_rm_multiplier = 5;

		public int SlideTol
		{
			get { return m_scroll_tol; }
			set { m_scroll_tol = value; Invalidate(); }
		}

		public string ValueText
		{
			get { return m_label_value; }
			set { m_label_value = value; label_value.Text = m_label_value; Invalidate(); }
		}

		public string SlideText
		{
			get { return m_label_text; }
			set { m_label_text = value; label_name.Text = m_label_text; Invalidate(); }
		}

		public string ToolTop
		{
			get { return m_tool_top; }
			set { m_tool_top = value; tooltip_slider.SetToolTip(label_name, m_tool_top); Invalidate(); }
		}

		public int RightMouseMultiplier
		{
			get { return m_rm_multiplier; }
			set { m_rm_multiplier = value; Invalidate(); }
		}

		public SliderLabel()
		{
			InitializeComponent();
		}

		private int slide_prev = 0;
		public Stopwatch mouse_down_sw = new Stopwatch();
		
		private void label_name_MouseDown(object sender, MouseEventArgs e)
		{
			mouse_down_sw.Restart();
			slide_prev = e.X;
		}

		private void label_name_MouseMove(object sender, MouseEventArgs e)
		{
			// Trigger event based on this:
			int inc = MaybeMouseSlide(e);
			if (inc != 0) {
				RaiseFeedback(inc * (e.Button == MouseButtons.Right ? m_rm_multiplier : 1));
			}
		}

		private void label_name_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				RaiseFeedback(1);
			} else if (e.Delta < 0) {
				RaiseFeedback(-1);
			}
		}

		public int MaybeMouseSlide(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
				int diff = slide_prev - e.X;
				if (diff < -m_scroll_tol) {
					slide_prev += m_scroll_tol;
					return 1;
				} else if (diff > m_scroll_tol) {
					slide_prev -= m_scroll_tol;
					return -1;
				}
			}
			return 0;
		}

		public event EventHandler<SliderLabelArgs> Feedback;

		private void RaiseFeedback(int p, bool reset = false)
		{
			EventHandler<SliderLabelArgs> handler = Feedback;
			if (handler != null) {
				handler(null, new SliderLabelArgs(p, reset));
			}
		}

		public Color C_text_active = SystemColors.HighlightText;
		public Color C_text_inactive = SystemColors.ControlText;

		private void label_name_MouseHover(object sender, EventArgs e)
		{
			if (!Focused) {
				label_name.ForeColor = C_text_active;
			}
			label_name.Focus();
		}

		private void label_name_Leave(object sender, EventArgs e)
		{
			label_name.ForeColor = C_text_inactive;
		}

		private void label_value_MouseDown(object sender, MouseEventArgs e)
		{
			mouse_down_sw.Restart();
			slide_prev = e.X;
		}

		private void label_value_MouseMove(object sender, MouseEventArgs e)
		{
			int inc = MaybeMouseSlide(e);
			if (inc != 0) {
				RaiseFeedback(inc * (e.Button == MouseButtons.Right ? m_rm_multiplier : 1));
			}
		}

		private void label_value_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				RaiseFeedback(1);
			} else if (e.Delta < 0) {
				RaiseFeedback(-1);
			}
		}

		private void label_name_MouseUp(object sender, MouseEventArgs e)
		{
			MouseUp(e);
		}

		private void label_value_MouseUp(object sender, MouseEventArgs e)
		{
			MouseUp(e);
		}

		public new void MouseUp(MouseEventArgs e)
		{
			int drag_time = (int)mouse_down_sw.ElapsedMilliseconds;
			if (drag_time < 500) {
				switch (e.Button) {
					case MouseButtons.Left:
						RaiseFeedback(1);
						break;
					case MouseButtons.Right:
						RaiseFeedback(-1);
						break;
					case MouseButtons.Middle:
						RaiseFeedback(0, true);
						break;
				}
			}
		}
	}
}

public class SliderLabelArgs : EventArgs
{
	private int inc;
	private bool reset;

	public SliderLabelArgs(int i, bool r)
	{
		inc = i;
		reset = r;
	}

	public int Increment
	{
		get { return inc; }
		set { inc = value; }
	}

	public bool Reset
	{
		get
		{
			return reset;
		}
		set
		{
			reset = value;
		}
	}
}

