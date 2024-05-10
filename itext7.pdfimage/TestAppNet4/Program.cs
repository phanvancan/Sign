 
using System;
using System.IO;
using itext.pdfimage.Extensions;
using System.Drawing.Imaging;
using System.Drawing;

namespace TestAppNet4
{
    class Program
    {
        static void Main(string[] args)
        {
            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HD_1C23TSN_120_1712826810.pdf"), FileMode.Open);

            var reader = new iText.Kernel.Pdf.PdfReader(pdf);
            var pdfDocument = new iText.Kernel.Pdf.PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();

            foreach (var bitmap in bitmaps)
            {
                bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
                bitmap.Dispose();
            }

            var page1 = pdfDocument.GetPage(1);
            var bitmap1 = page1.ConvertPageToBitmap();
            bitmap1.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-page1-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
            bitmap1.Dispose();
        }
    }
}
