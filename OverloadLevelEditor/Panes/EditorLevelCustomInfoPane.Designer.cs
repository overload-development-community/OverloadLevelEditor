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
	partial class EditorLevelCustomInfoPane
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
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxObjectiveCount = new System.Windows.Forms.TextBox();
			this.comboBoxObjective = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxAlienLava = new System.Windows.Forms.CheckBox();
			this.checkBoxNoExplosionsOnExit = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxExitMusicStartTime = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(1, 85);
			this.label3.Margin = new System.Windows.Forms.Padding(1);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(142, 18);
			this.label3.TabIndex = 16;
			this.label3.Text = "OBJECTIVE TYPE";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(-2, 43);
			this.label4.Margin = new System.Windows.Forms.Padding(1);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(145, 18);
			this.label4.TabIndex = 15;
			this.label4.Text = "OBJECTIVE COUNT";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxObjectiveCount
			// 
			this.textBoxObjectiveCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxObjectiveCount.Location = new System.Drawing.Point(86, 63);
			this.textBoxObjectiveCount.Margin = new System.Windows.Forms.Padding(1);
			this.textBoxObjectiveCount.Name = "textBoxObjectiveCount";
			this.textBoxObjectiveCount.Size = new System.Drawing.Size(57, 20);
			this.textBoxObjectiveCount.TabIndex = 2;
			this.textBoxObjectiveCount.Leave += new System.EventHandler(this.textBoxObjectiveCount_Leave);
			// 
			// comboBoxObjective
			// 
			this.comboBoxObjective.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxObjective.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxObjective.FormattingEnabled = true;
			this.comboBoxObjective.Location = new System.Drawing.Point(1, 106);
			this.comboBoxObjective.Margin = new System.Windows.Forms.Padding(1);
			this.comboBoxObjective.Name = "comboBoxObjective";
			this.comboBoxObjective.Size = new System.Drawing.Size(142, 21);
			this.comboBoxObjective.TabIndex = 1;
			this.comboBoxObjective.SelectedIndexChanged += new System.EventHandler(this.comboBoxObjective_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(21, 25);
			this.label2.Margin = new System.Windows.Forms.Padding(1);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "[seconds]";
			// 
			// checkBoxAlienLava
			// 
			this.checkBoxAlienLava.AutoSize = true;
			this.checkBoxAlienLava.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBoxAlienLava.Location = new System.Drawing.Point(1, 151);
			this.checkBoxAlienLava.Margin = new System.Windows.Forms.Padding(1);
			this.checkBoxAlienLava.Name = "checkBoxAlienLava";
			this.checkBoxAlienLava.Size = new System.Drawing.Size(142, 17);
			this.checkBoxAlienLava.TabIndex = 4;
			this.checkBoxAlienLava.Text = "Alien Level (affects lava)";
			this.checkBoxAlienLava.UseVisualStyleBackColor = true;
			this.checkBoxAlienLava.CheckedChanged += new System.EventHandler(this.checkBoxAlienLava_CheckedChanged);
			// 
			// checkBoxNoExplosionsOnExit
			// 
			this.checkBoxNoExplosionsOnExit.AutoSize = true;
			this.checkBoxNoExplosionsOnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBoxNoExplosionsOnExit.Location = new System.Drawing.Point(1, 130);
			this.checkBoxNoExplosionsOnExit.Margin = new System.Windows.Forms.Padding(1);
			this.checkBoxNoExplosionsOnExit.Name = "checkBoxNoExplosionsOnExit";
			this.checkBoxNoExplosionsOnExit.Size = new System.Drawing.Size(130, 17);
			this.checkBoxNoExplosionsOnExit.TabIndex = 3;
			this.checkBoxNoExplosionsOnExit.Text = "No Explosions On Exit";
			this.checkBoxNoExplosionsOnExit.UseVisualStyleBackColor = true;
			this.checkBoxNoExplosionsOnExit.CheckedChanged += new System.EventHandler(this.checkBoxNoExplosionsOnExit_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(-2, 1);
			this.label1.Margin = new System.Windows.Forms.Padding(1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 18);
			this.label1.TabIndex = 8;
			this.label1.Text = "EXIT MUSIC OFFSET";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxExitMusicStartTime
			// 
			this.textBoxExitMusicStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxExitMusicStartTime.Location = new System.Drawing.Point(86, 20);
			this.textBoxExitMusicStartTime.Margin = new System.Windows.Forms.Padding(1);
			this.textBoxExitMusicStartTime.Name = "textBoxExitMusicStartTime";
			this.textBoxExitMusicStartTime.Size = new System.Drawing.Size(57, 20);
			this.textBoxExitMusicStartTime.TabIndex = 0;
			this.textBoxExitMusicStartTime.Leave += new System.EventHandler(this.textBoxExitMusicStartTime_Leave);
			// 
			// EditorLevelCustomInfoPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(148, 168);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxObjectiveCount);
			this.Controls.Add(this.comboBoxObjective);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkBoxAlienLava);
			this.Controls.Add(this.checkBoxNoExplosionsOnExit);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxExitMusicStartTime);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.HideOnClose = true;
			this.MinimumSize = new System.Drawing.Size(143, 39);
			this.Name = "EditorLevelCustomInfoPane";
			this.Text = "CUSTOM INFO";
			this.Load += new System.EventHandler(this.EditorLevelCustomInfoPane_Load);
			this.VisibleChanged += new System.EventHandler(this.EditorLevelCustomInfoPane_VisibleChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxExitMusicStartTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxNoExplosionsOnExit;
		private System.Windows.Forms.CheckBox checkBoxAlienLava;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxObjective;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxObjectiveCount;
		private System.Windows.Forms.Label label3;
	}
}