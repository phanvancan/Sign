using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiginBS.Common;

namespace SiginBS.Models
{
    public class ReleaseInfo
    {
        public string Token { get; private set; }

        public int ReleaseId { get; private set; }

        public bool IsClientSign { get; private set; }


        public ReleaseInfo(string[] args)
        {

            SetData(args);
        }

        private void SetData(string[] args)
        {
            if (args.Length < 1 || args[0].Split(',').Length < 1)
            {
                return;
            }
            var parameter = args[0].Split(',');
            Token = parameter[0].Replace(AppConfig.Instance.UrlSchemas, "").DecodeUrl().ToAscii();
           if (parameter.Length>1)  ReleaseId = parameter[1].ToInt(0);
            if (parameter.Length > 2 && !parameter[2].IsNullOrEmpty())
            {
                IsClientSign = true;
            }
        }

    }
}
