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

namespace OverloadLevelEditor
{
	partial class EntityLinkLabel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label_name = new System.Windows.Forms.Label();
			this.label_value = new System.Windows.Forms.Label();
			this.tooltip_link = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// label_name
			// 
			this.label_name.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_name.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label_name.Location = new System.Drawing.Point(0, 0);
			this.label_name.Margin = new System.Windows.Forms.Padding(0);
			this.label_name.Name = "label_name";
			this.label_name.Size = new System.Drawing.Size(50, 19);
			this.label_name.TabIndex = 0;
			this.label_name.Text = "Label";
			this.label_name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_name.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_name_MouseDown);
			// 
			// label_value
			// 
			this.label_value.BackColor = System.Drawing.SystemColors.Control;
			this.label_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_value.Location = new System.Drawing.Point(50, 0);
			this.label_value.Margin = new System.Windows.Forms.Padding(0);
			this.label_value.Name = "label_value";
			this.label_value.Size = new System.Drawing.Size(97, 19);
			this.label_value.TabIndex = 1;
			this.label_value.Text = "-";
			this.label_value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label_value.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_value_MouseDown);
			// 
			// EntityLinkLabel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label_value);
			this.Controls.Add(this.label_name);
			this.Name = "EntityLinkLabel";
			this.Size = new System.Drawing.Size(147, 19);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label_name;
		private System.Windows.Forms.Label label_value;
		private System.Windows.Forms.ToolTip tooltip_link;
	}
}
