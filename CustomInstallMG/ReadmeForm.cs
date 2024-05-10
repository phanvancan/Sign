// This Code was created by Microgold Software Inc. for educational purposes
// Copyright Microgold Software Inc. Saturday, December 27, 2003

using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ReadMeDialogExtension
{
	/// <summary>
	/// Summary description for ReadmeForm.
	/// </summary>
	public class ReadmeForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button NextButton;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		string GetAppPath()
		{
			return Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));
		}

		void ReadRtfReadMe()
		{

			StreamReader sr = new StreamReader(MainDirectoryPath + "\\" + ReadmeFileName);
			richTextBox1.Rtf = sr.ReadToEnd();
			sr.Close();
		}

		string ReadmeFileName = "readme.txt";
		string ReadmeLink = @"http://www.c-sharpcorner.com";
		string MainDirectoryPath = "c:\\temp";
		public ReadmeForm(string fileName, string hyperlink, string pathDir)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (fileName.Length > 0)
				ReadmeFileName = fileName;

			if (hyperlink.Length > 0)
				ReadmeLink = hyperlink;

			if (pathDir.Length > 0)
				MainDirectoryPath = pathDir;

			ReadRtfReadMe();
			richTextBox1.ReadOnly = true;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.NextButton = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // NextButton
            // 
            this.NextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.NextButton.Location = new System.Drawing.Point(192, 360);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 0;
            this.NextButton.Text = "Next >";
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(184, 328);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(100, 23);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "View Artice Online";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Location = new System.Drawing.Point(40, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 288);
            this.panel1.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(392, 288);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "richTextBox1";
            // 
            // ReadmeForm
            // 
            this.AcceptButton = this.NextButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(458, 383);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.NextButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ReadmeForm";
            this.Text = "Read Me First";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("IExplore.exe", ReadmeLink);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Couldn't Show Page " + ReadmeLink);
			}
		}

        private void NextButton_Click(object sender, EventArgs e)
        {

        }
    }
}
