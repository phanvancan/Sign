using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomInstall
{
    [RunInstaller(true)]
    public partial class CustomerInstaller : Installer
    {
        private const string URI_SCHEME = "newsign";
        private const string URI_KEY = "URL:NewScheme Protocol";
        private static string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private const string FolderApp = "InvoiceSignInvoice";
        private const string Asset = "Asset";
        private const string Release = "Release";
        private const string Temp = "Temp";
        public CustomerInstaller()
        {
            InitializeComponent();
        }

        public override void Uninstall(IDictionary savedState)
        {
            this.DeleteUserShemas();
            base.Uninstall(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            CreateFolderOfApp();
            CreateFolderRelease();
            CreateFolderTemp();
            CreateFolderAsset();
            CoppyFileAsset();
            this.CreateUserShemas();
            base.OnAfterInstall(savedState);
        }
        
        private void CreateUserShemas()
        {
            string str = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SignInvoice.exe");
            //CustomerInstaller.WriteLog(string.Format("appPath {0}", (object)str));

            if (!File.Exists(str))
                return;
            CustomerInstaller.RegisterUriScheme(str);
        }

        private void DeleteUserShemas()
        {
            try
            {
                CustomerInstaller.UnregisterUriScheme();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Failed!");
                Console.WriteLine("{0}: {1}", (object)ex.GetType().Name, (object)ex.Message);
            }
        }

        private static void RegisterUriScheme(string appPath)
        {
            CustomerInstaller.WriteLog(string.Format("RegisterUriScheme {0}", (object)appPath));
            using (RegistryKey subKey1 = Registry.ClassesRoot.CreateSubKey(URI_SCHEME))
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

        private static void WriteLog(string log)
        {
            string fullPathFolderApp = GetFullFolderApp();
            string fileLog = Path.Combine(fullPathFolderApp, "log.txt");
            using (StreamWriter text = File.CreateText(fileLog))
            {
                text.WriteLine(log);
            }
        }

        private static void CreateFolderOfApp()
        {
            string fullPathFolderApp = GetFullFolderApp();
            if (!Directory.Exists(fullPathFolderApp))
            {
                Directory.CreateDirectory(fullPathFolderApp);
            }
        }

        private static string GetFullFolderApp()
        {
            return Path.Combine(FolderPath, FolderApp);
        }

        private static void CreateFolderRelease()
        {
            string fullPathFolderApp = GetFullFolderApp();
            string folderRelease = Path.Combine(fullPathFolderApp, Release);
            if (!Directory.Exists(folderRelease))
            {
                Directory.CreateDirectory(folderRelease);
            }
        }
        private static void CreateFolderTemp()
        {
            string fullPathFolderApp = GetFullFolderApp();
            string folderTemp = Path.Combine(fullPathFolderApp, Temp);
            if (!Directory.Exists(folderTemp))
            {
                Directory.CreateDirectory(folderTemp);
            }
        }
        private static void CreateFolderAsset()
        {
            string fullPathFolderApp = GetFullFolderApp();
            string folderAsset = Path.Combine(fullPathFolderApp, Asset);
            if (!Directory.Exists(folderAsset))
            {
                Directory.CreateDirectory(folderAsset);
            }
        }
        private void CoppyFileAsset()
        {
            string temp_sign = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Asset, "temple.pdf");
            string temp_sign_dest = Path.Combine(FolderPath, FolderApp, Asset, "temple.pdf");
            
            FileInfo fileTempSign = new FileInfo(temp_sign);
            if (File.Exists(temp_sign_dest))
            {
                File.Delete(temp_sign_dest);
            }

            fileTempSign.MoveTo(temp_sign_dest);
        }
    }
}
