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


namespace OverloadLevelEditor
{
	public partial class EntityLinkLabel : UserControl
	{
		private string m_label_value = "";
		private string m_label_text = "";
		private string m_tool_top = "";
		private int m_index = 0;

		public string ValueText
		{
			get { return m_label_value; }
			set { m_label_value = value; label_value.Text = m_label_value; Invalidate(); }
		}

		public string LabelText
		{
			get { return m_label_text; }
			set { m_label_text = value; label_name.Text = m_label_text; Invalidate(); }
		}

		public int Index
		{
			get { return m_index; }
			set { m_index = value;}
		}

		public string ToolTop
		{
			get { return m_tool_top; }
			set { m_tool_top = value; tooltip_link.SetToolTip(label_name, m_tool_top); tooltip_link.SetToolTip(label_value, m_tool_top); }
		}

		public EntityLinkLabel()
		{
			InitializeComponent();
		}

		private void label_name_MouseDown(object sender, MouseEventArgs e)
		{
			RaiseFeedback(e.Button, ModifierKeys == Keys.Shift);
		}

		public event EventHandler<EntityLinkLabelArgs> Feedback;

		private void RaiseFeedback(MouseButtons mb, bool shft)
		{
			EventHandler<EntityLinkLabelArgs> handler = Feedback;
			if (handler != null) {
				handler(null, new EntityLinkLabelArgs(mb, shft, m_index));
			}
		}

		private void label_value_MouseDown(object sender, MouseEventArgs e)
		{
			RaiseFeedback(e.Button, ModifierKeys == Keys.Shift);
		}
	}
}

public class EntityLinkLabelArgs : EventArgs
{
	private MouseButtons button;
	private bool shift;
	private int index;

	public EntityLinkLabelArgs(MouseButtons mb, bool shft, int idx)
	{
		button = mb;
		index = idx;
		shift = shft;
	}

	public bool Shift
	{
		get { return shift; }
		set { shift = value; }
	}

	public MouseButtons Button
	{
		get { return button; }
		set { button = value; }
	}

	public int Index
	{
		get { return index; }
		set { index = value; }
	}
}

