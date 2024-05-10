using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiginBS
{
    public class STAApplicationContext : ApplicationContext
    {
        public STAApplicationContext()
        {
            Form1 form1 = new Form1();
            form1.Show();

          //  form1.set();

        }



        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {

        }
    }
}
