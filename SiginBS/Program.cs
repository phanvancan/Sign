using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Org.BouncyCastle.Crypto.Parameters;
using SiginBS.Common;
using SiginBS.Models;

namespace SiginBS
{
    internal static class Program
    {
        private const string URI_SCHEME = "newsignvb";
        private const string URI_KEY = "URL:NewScheme Protocol";
        private static string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private const string FolderApp = "SignOffice";
        private const string Asset = "Asset";
        private const string Release = "Release";
        private const string Temp = "Temp";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {


            //if (args.Length > 0 && IsAdministrator())
            //{
            //    if (args[0] == "reg")
            //    {
            //        string fileFullName = Assembly.GetExecutingAssembly().Location;
            //        string str = Path.Combine(Path.GetDirectoryName(fileFullName), Path.GetFileName(fileFullName));
            //        Console.WriteLine(str);
            //        if (!File.Exists(str))
            //            return;
            //        RegisterUriScheme(str);
            //    }

            //}




            //  GetParameter(args);
            bool createdNew = false;
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    // Only allow one instance
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    STAApplicationContext context = new STAApplicationContext();
                    Application.Run(context);

                    //  Form1 form1 = new Form1();
                    // form1.Show();
                    //  Application.Run(form1);
                    //  form1.Hide();

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }



            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
        private static void GetParameter(string[] args)
        {
            ReleaseInfo releaseInfo = new ReleaseInfo(args);
            if (releaseInfo.ReleaseId < 1)
            {
               // MessageBox.Show(MessageError.SysErrorMessages);
              //  Logger.Default.Error(MessageError.SysErrorMessages);
                //Environment.Exit(0);
            }

            GlobalData.Instance.releaseInfo = releaseInfo;
        }
        private static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }


        private static void RegisterUriScheme(string appPath)
        {
            

            using (RegistryKey subKey1 = Registry.ClassesRoot.CreateSubKey(AppConfig.Instance.UrlSchemas))
            {
                subKey1.SetValue((string)null, (object)URI_KEY);
                subKey1.SetValue("URL Protocol", (object)string.Empty, RegistryValueKind.String);
                using (RegistryKey subKey2 = subKey1.CreateSubKey("DefaultIcon"))
                {
                    string str = string.Format("\"{0}\",0", (object)appPath);
                    subKey2.SetValue((string)null, (object)str);
                }
                using (RegistryKey subKey2 = subKey1.CreateSubKey("shell"))
                {
                    using (RegistryKey subKey3 = subKey2.CreateSubKey("open"))
                    {
                        using (RegistryKey subKey4 = subKey3.CreateSubKey("command"))
                        {
                            string str = string.Format("\"{0}\" \"%1\"", (object)appPath);
                            subKey4.SetValue((string)null, (object)str);
                        }
                    }
                }
            }
        }
        private static void UnregisterUriScheme()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(URI_SCHEME);
        }
    }
}
