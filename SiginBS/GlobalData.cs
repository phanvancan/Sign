 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiginBS.Models;

namespace SiginBS
{
   public class GlobalData
    {
       private static GlobalData _instance = new GlobalData();

       public static GlobalData Instance
       {
           get { return _instance; }
       }

       public ReleaseInfo releaseInfo { get; set; }

     //  public AccountInfo  currentUser { get; set; }

     //  public KeyStoreOfCompany keyStore { get; set; }
    }
}
