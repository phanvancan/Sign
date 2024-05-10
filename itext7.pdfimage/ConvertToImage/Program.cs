using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using itext.pdfimage; 
namespace ConvertToImage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pdf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "base64.online.pdf");

            PdfToImageConverter cvimg = new PdfToImageConverter();
                
             var  img=  cvimg.ConvertToImages(pdf.ToString());

            foreach (var image in img)
            {
                image.Save( $"wave-{DateTime.Now.Ticks}.png", ImageFormat.Png);
                 image.Dispose();

            }

            //bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-{DateTime.Now.Ticks}.png"), ImageFormat.Png);

        }
    }
}
