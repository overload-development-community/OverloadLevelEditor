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
	partial class AboutBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this.label_ProgramName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label_VersionNum = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.label_Copyright = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label_ProgramName
			// 
			this.label_ProgramName.AutoSize = true;
			this.label_ProgramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ProgramName.Location = new System.Drawing.Point(13, 13);
			this.label_ProgramName.Name = "label_ProgramName";
			this.label_ProgramName.Size = new System.Drawing.Size(242, 26);
			this.label_ProgramName.TabIndex = 0;
			this.label_ProgramName.Text = "Overload Level Editor";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(18, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Version";
			// 
			// label_VersionNum
			// 
			this.label_VersionNum.AutoSize = true;
			this.label_VersionNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_VersionNum.Location = new System.Drawing.Point(77, 46);
			this.label_VersionNum.Name = "label_VersionNum";
			this.label_VersionNum.Size = new System.Drawing.Size(100, 20);
			this.label_VersionNum.TabIndex = 2;
			this.label_VersionNum.Text = "Version Num";
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(24, 128);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(622, 209);
			this.richTextBox1.TabIndex = 5;
			this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
			// 
			// label_Copyright
			// 
			this.label_Copyright.AutoSize = true;
			this.label_Copyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Copyright.Location = new System.Drawing.Point(21, 81);
			this.label_Copyright.Name = "label_Copyright";
			this.label_Copyright.Size = new System.Drawing.Size(99, 17);
			this.label_Copyright.TabIndex = 3;
			this.label_Copyright.Text = "Copyright Text";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(21, 99);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(603, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "This tool may be used and distributed freely for non-commercial use.  All other r" +
    "ights reserved.";
			// 
			// AboutBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(680, 362);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label_Copyright);
			this.Controls.Add(this.label_VersionNum);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label_ProgramName);
			this.Controls.Add(this.richTextBox1);
			this.Name = "AboutBox";
			this.Text = "AboutBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RichTextBox richTextBox1;
		public System.Windows.Forms.Label label_ProgramName;
		public System.Windows.Forms.Label label_VersionNum;
		public System.Windows.Forms.Label label_Copyright;
		private System.Windows.Forms.Label label3;
	}
}