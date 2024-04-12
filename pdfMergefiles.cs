using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Drawing;
using iTextSharp.text.pdf;
using iTextSharp.text;


namespace SiginBS
{
    internal class pdfMergefiles
    {

    
        

        public void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
        {

            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                Document document = new Document(PageSize.A4);
                PdfCopy copy = new PdfCopy(document, new FileStream(Path.Combine(outputPath, pdfFileName + ".pdf"), FileMode.Create));
                document.Open();

                //try
                //{
                int numberOfPages = reader.NumberOfPages;
                if (interval == 0)
                {
                    interval = numberOfPages - startPage + 1;
                }


                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                }
                //}
                //catch
                //{
                //   // document.Close();
                //}

                document.Close();
            }
        }
    }
}
