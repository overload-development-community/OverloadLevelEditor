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
	partial class EditMarkerForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxIndices = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.radioOpToggle = new System.Windows.Forms.RadioButton();
			this.radioOpSet = new System.Windows.Forms.RadioButton();
			this.radioOpClear = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioTypeSide = new System.Windows.Forms.RadioButton();
			this.radioTypeSegment = new System.Windows.Forms.RadioButton();
			this.radioTypeVertex = new System.Windows.Forms.RadioButton();
			this.radioTypeEntity = new System.Windows.Forms.RadioButton();
			this.buttonDoIt = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxIndices
			// 
			this.textBoxIndices.Location = new System.Drawing.Point(12, 32);
			this.textBoxIndices.Name = "textBoxIndices";
			this.textBoxIndices.Size = new System.Drawing.Size(465, 20);
			this.textBoxIndices.TabIndex = 1;
			this.textBoxIndices.TextChanged += new System.EventHandler(this.textBoxIndices_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(137, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Indices (comma seperated):";
			// 
			// radioOpToggle
			// 
			this.radioOpToggle.AutoSize = true;
			this.radioOpToggle.Location = new System.Drawing.Point(6, 19);
			this.radioOpToggle.Name = "radioOpToggle";
			this.radioOpToggle.Size = new System.Drawing.Size(58, 17);
			this.radioOpToggle.TabIndex = 3;
			this.radioOpToggle.TabStop = true;
			this.radioOpToggle.Text = "Toggle";
			this.radioOpToggle.UseVisualStyleBackColor = true;
			// 
			// radioOpSet
			// 
			this.radioOpSet.AutoSize = true;
			this.radioOpSet.Location = new System.Drawing.Point(6, 42);
			this.radioOpSet.Name = "radioOpSet";
			this.radioOpSet.Size = new System.Drawing.Size(41, 17);
			this.radioOpSet.TabIndex = 4;
			this.radioOpSet.TabStop = true;
			this.radioOpSet.Text = "Set";
			this.radioOpSet.UseVisualStyleBackColor = true;
			// 
			// radioOpClear
			// 
			this.radioOpClear.AutoSize = true;
			this.radioOpClear.Location = new System.Drawing.Point(6, 65);
			this.radioOpClear.Name = "radioOpClear";
			this.radioOpClear.Size = new System.Drawing.Size(49, 17);
			this.radioOpClear.TabIndex = 5;
			this.radioOpClear.TabStop = true;
			this.radioOpClear.Text = "Clear";
			this.radioOpClear.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioOpSet);
			this.groupBox1.Controls.Add(this.radioOpClear);
			this.groupBox1.Controls.Add(this.radioOpToggle);
			this.groupBox1.Location = new System.Drawing.Point(12, 58);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(95, 112);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Operation";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioTypeSide);
			this.groupBox2.Controls.Add(this.radioTypeSegment);
			this.groupBox2.Controls.Add(this.radioTypeVertex);
			this.groupBox2.Controls.Add(this.radioTypeEntity);
			this.groupBox2.Location = new System.Drawing.Point(113, 58);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(122, 112);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Type";
			// 
			// radioTypeSide
			// 
			this.radioTypeSide.AutoSize = true;
			this.radioTypeSide.Location = new System.Drawing.Point(6, 88);
			this.radioTypeSide.Name = "radioTypeSide";
			this.radioTypeSide.Size = new System.Drawing.Size(46, 17);
			this.radioTypeSide.TabIndex = 10;
			this.radioTypeSide.TabStop = true;
			this.radioTypeSide.Text = "Side";
			this.radioTypeSide.UseVisualStyleBackColor = true;
			// 
			// radioTypeSegment
			// 
			this.radioTypeSegment.AutoSize = true;
			this.radioTypeSegment.Location = new System.Drawing.Point(6, 65);
			this.radioTypeSegment.Name = "radioTypeSegment";
			this.radioTypeSegment.Size = new System.Drawing.Size(67, 17);
			this.radioTypeSegment.TabIndex = 9;
			this.radioTypeSegment.TabStop = true;
			this.radioTypeSegment.Text = "Segment";
			this.radioTypeSegment.UseVisualStyleBackColor = true;
			// 
			// radioTypeVertex
			// 
			this.radioTypeVertex.AutoSize = true;
			this.radioTypeVertex.Location = new System.Drawing.Point(6, 42);
			this.radioTypeVertex.Name = "radioTypeVertex";
			this.radioTypeVertex.Size = new System.Drawing.Size(55, 17);
			this.radioTypeVertex.TabIndex = 8;
			this.radioTypeVertex.TabStop = true;
			this.radioTypeVertex.Text = "Vertex";
			this.radioTypeVertex.UseVisualStyleBackColor = true;
			// 
			// radioTypeEntity
			// 
			this.radioTypeEntity.AutoSize = true;
			this.radioTypeEntity.Location = new System.Drawing.Point(6, 19);
			this.radioTypeEntity.Name = "radioTypeEntity";
			this.radioTypeEntity.Size = new System.Drawing.Size(51, 17);
			this.radioTypeEntity.TabIndex = 7;
			this.radioTypeEntity.TabStop = true;
			this.radioTypeEntity.Text = "Entity";
			this.radioTypeEntity.UseVisualStyleBackColor = true;
			// 
			// buttonDoIt
			// 
			this.buttonDoIt.Location = new System.Drawing.Point(340, 147);
			this.buttonDoIt.Name = "buttonDoIt";
			this.buttonDoIt.Size = new System.Drawing.Size(137, 23);
			this.buttonDoIt.TabIndex = 11;
			this.buttonDoIt.Text = "Do It!";
			this.buttonDoIt.UseVisualStyleBackColor = true;
			this.buttonDoIt.Click += new System.EventHandler(this.buttonDoIt_Click);
			// 
			// EditMarkerForm
			// 
			this.AcceptButton = this.buttonDoIt;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(489, 178);
			this.Controls.Add(this.buttonDoIt);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxIndices);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "EditMarkerForm";
			this.Text = "Marker Form";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.EditSelectorForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxIndices;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioOpToggle;
		private System.Windows.Forms.RadioButton radioOpSet;
		private System.Windows.Forms.RadioButton radioOpClear;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioTypeSide;
		private System.Windows.Forms.RadioButton radioTypeSegment;
		private System.Windows.Forms.RadioButton radioTypeVertex;
		private System.Windows.Forms.RadioButton radioTypeEntity;
		private System.Windows.Forms.Button buttonDoIt;
	}
}