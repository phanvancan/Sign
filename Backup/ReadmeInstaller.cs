// This Code was created by Microgold Software Inc. for educational purposes
// Copyright Microgold Software Inc. Saturday, December 27, 2003

using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Configuration.Install;

namespace ReadMeDialogExtension
{


	// Set 'RunInstaller' attribute to true.
	[RunInstaller(true)]
	public class ReadmeInstaller: Installer
	{
		public ReadmeInstaller() :base()
		{
		// Attach the 'Committed' event.
		this.Committed += new InstallEventHandler(ReadmeInstaller_Committed);
		}

		// Event handler for 'Committed' event.
		private void ReadmeInstaller_Committed(object sender, InstallEventArgs e)
		{
		}


		// Override the 'Install' method.
		public override void Install(IDictionary savedState)
		{
			base.Install(savedState);

			// get readme file name from custom action parameter
		  string ProvidedName = this.Context.Parameters["file"];
		
		  string LinkName = this.Context.Parameters["link"];
		  string TheAssemblyPath = this.Context.Parameters["assemblypath"];
		  string MainDirectory = TheAssemblyPath.Substring(0, TheAssemblyPath.LastIndexOf("\\"));

			ReadmeForm MyReadMe = new ReadmeForm(ProvidedName, LinkName, MainDirectory);
			MyReadMe.ShowDialog();
		}




	}
}
