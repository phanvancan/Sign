using System;
using System.Collections.Generic;

 
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.security;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
 
 
  
using itext.pdfimage.Extensions;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
 



namespace SiginBS
{
    public partial class Form1 : Form
    {
        public static IList<Org.BouncyCastle.X509.X509Certificate> chain = new List<Org.BouncyCastle.X509.X509Certificate>();
        public static X509Certificate2 pk;
        private const string keyGetCompany = "CN=";
        private static IOcspClient ocspClient;
        private static ITSAClient tsaClient;
        private static IList<ICrlClient> crlList;
        private static readonly string fullPathAppOfCurrentUser = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string templatepdfFile = "temple.pdf";
        private pdfMergefiles pdfMergefiles= new pdfMergefiles();
        public Form1()
        {
            InitializeComponent();
        }
        private void button3_Click(object sender, EventArgs e)
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
        void convertPdftoPNG(string src, string des)
        {
            var pdf = File.Open(src, FileMode.Open);

            var reader = new iText.Kernel.Pdf.PdfReader(pdf);
            var pdfDocument = new iText.Kernel.Pdf.PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();

            foreach (var bitmap in bitmaps)
            {
                bitmap.Save(des, ImageFormat.Png);
                bitmap.Dispose();
            }


        }
        private void Form1_Load(object sender, EventArgs e)
        {



            string pfxFilePath = @"C08-Cert.pfx";
            string pfxPassword = "Hanoi1";
            // Org.BouncyCastle.Pkcs.Pkcs12Store pfxKeyStore = new Org.BouncyCastle.Pkcs.Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());


            // X509Certificate2Collection collection = new X509Certificate2Collection();
            // collection.Import(pfxFilePath, pfxPassword, X509KeyStorageFlags.DefaultKeySet);


            
            textBox1.Text = "120";
            textBox2.Text = "120";
            int page = 1;

            X509Certificate2 cert = new X509Certificate2(pfxFilePath, pfxPassword, X509KeyStorageFlags.UserKeySet);
            
           
            //pk = cert;


            X509Store x509Store = new X509Store(StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)x509Store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Chọn thiết bị xác thực ", "Chọn một chứng chỉ từ danh sách dưới đây để nhận thông tin về chứng chỉ đó", X509SelectionFlag.MultiSelection);
            if (scollection.Count > 0)
            {
                pk = scollection[0];

            }
            else
            {
                MessageBox.Show("Looix");
            }
            //byte[] certData = pk.Export(X509ContentType.Pfx, "Hanoi1");
            // File.WriteAllBytes(@"C08-Cert.pfx", certData);

            X509Chain ch = new X509Chain();
            ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            ch.Build(pk);
            Console.WriteLine("Chain Information");
            Console.WriteLine("Chain revocation flag: {0}", ch.ChainPolicy.RevocationFlag);
            Console.WriteLine("Chain revocation mode: {0}", ch.ChainPolicy.RevocationMode);
            Console.WriteLine("Chain verification flag: {0}", ch.ChainPolicy.VerificationFlags);
            Console.WriteLine("Chain verification time: {0}", ch.ChainPolicy.VerificationTime);
            Console.WriteLine("Chain status length: {0}", ch.ChainStatus.Length);
            Console.WriteLine("Chain application policy count: {0}", ch.ChainPolicy.ApplicationPolicy.Count);
            Console.WriteLine("Chain certificate policy count: {0} {1}", ch.ChainPolicy.CertificatePolicy.Count, Environment.NewLine);

            //Output chain element information.
            Console.WriteLine("Chain Element Information");
            Console.WriteLine("Number of chain elements: {0}", ch.ChainElements.Count);
            Console.WriteLine("Chain elements synchronized? {0} {1}", ch.ChainElements.IsSynchronized, Environment.NewLine);
            foreach (X509ChainElement element in ch.ChainElements)
            {
                Console.WriteLine("Element issuer name: {0}", element.Certificate.Issuer);
                Console.WriteLine("Element certificate valid until: {0}", element.Certificate.NotAfter);
                Console.WriteLine("Element certificate is valid: {0}", element.Certificate.Verify());
                Console.WriteLine("Element error status length: {0}", element.ChainElementStatus.Length);
                Console.WriteLine("Element information: {0}", element.Information);
                Console.WriteLine("Number of element extensions: {0}{1}", element.Certificate.Extensions.Count, Environment.NewLine);

                if (ch.ChainStatus.Length > 1)
                {
                    for (int index = 0; index < element.ChainElementStatus.Length; index++)
                    {
                        Console.WriteLine(element.ChainElementStatus[index].Status);
                        Console.WriteLine(element.ChainElementStatus[index].StatusInformation);
                    }
                }
            }
            

        }



        private string SignFile(string fileToSign, string certname, int w, int h, float llx, float lly, int page)
        {
            string signedFile = Path.GetFileName(fileToSign).Replace(".pdf", $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss")}.signed.pdf");

             

            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(fileToSign))
            {
                int pages = pdfReader.NumberOfPages;
                var currentSignaturesCount = pdfReader.AcroFields.GetSignatureNames().Count();

                using (FileStream signedPdf = new FileStream(signedFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    string tempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".tempfiles");
                    Directory.CreateDirectory(tempDir);
                    string tempFileName = Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".pdf");
                    if (!File.Exists(tempFileName))
                        File.Create(tempFileName).Close();

                    using (PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0', tempFileName, true))  // Append mode
                    {
                        for (int i = 1; i <= pages; i++)
                        {
                            page = i;
                        // Add signature image
                        if (page <= pages && page > 0)
                        {
                            var pdfContentByte = pdfStamper.GetOverContent(page);

                            var pageSize = pdfReader.GetPageSize(1);
                            float pageWidth = pageSize.Width;//162
                            float pageHeight = pageSize.Height;//792

                            //img = GenerateStamp();
                            //bitmap2.Save("myfile.png", ImageFormat.Png);

                            string filename = @"img.png";

                            //var img2 = GenerateStamp();
                            //img2.Save(filename, ImageFormat.Png);

                            //System.IO.FileInfo file = new System.IO.FileInfo(filename);
                            //using (Bitmap iInfo = new Bitmap(filename))
                            //{
                            //   // llx = iInfo.Width;
                            //   // lly = iInfo.Height;

                            //}

                            var image = iTextSharp.text.Image.GetInstance(filename, true);


                            if (llx > pageWidth)
                            {
                                llx = pageWidth - w;
                            }

                            if (lly > pageHeight)
                            {
                                lly = pageHeight - h;
                            }

                            if (w > pageWidth)
                            {
                                w = (int)pageWidth;
                            }

                            if (h > pageHeight)
                            {
                                h = (int)pageHeight;
                            }

                            image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            image.ScaleToFit(w, h);

                            image.SetAbsolutePosition(llx, lly);


                            pdfContentByte.AddImage(image);

                            

                        

                            }
                        }



                        PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;

                        signatureAppearance.Reason ="Được ký bởi:\n" + pk.Subject.ToString();
                        signatureAppearance.SignDate = DateTime.Now;
                       // signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                        signatureAppearance.Acro6Layers = false;
                        float x = llx;
                        float y = lly;

                   
                         signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
                         w = 150;
                         h = 50;

                        signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + w, y + h), page, "signature");


             
                        try
                        {
                            X509Certificate2 cert = pk;
                            X509CertificateParser cp = new X509CertificateParser();
                            Org.BouncyCastle.X509.X509Certificate[] chain = new  Org.BouncyCastle.X509.X509Certificate[]
                           {
                               cp.ReadCertificate(pk.RawData) 
                           };
                            
                            //X509Chain ch = new X509Chain();
                            //ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                            //ch.Build(cert);


                            IExternalSignature externalSignature = new X509Certificate2Signature(pk, "SHA-256");
                            MakeSignature.SignDetached(signatureAppearance, externalSignature,chain
                                , null, null, null, 0, CryptoStandard.CMS);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                    }
                }
            }

            return signedFile;
        }

     

        Bitmap GenerateStamp()
        {
            System.Net.WebRequest request =
           System.Net.WebRequest.Create("https://daugiabienso.bocongan.gov.vn/assets/images/header/img.png");
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);

            return bitmap2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open pdf File";
            theDialog.Filter = "pdf files|*.pdf";
            theDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = templatepdfFile = theDialog.FileName;
                try
                {

                   // SignFile(templatepdfFile, "", int.Parse(textBox3.Text), int.Parse(textBox4.Text), float.Parse(textBox1.Text),
                //    float.Parse(textBox2.Text)
               //     , 1);
                }
                catch
                {
                    Console.WriteLine("Ko ky");
                }
               

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tachfile();


        }
        private void tachfile()
        {
          string  currentForlder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot);

            UploadFileCommon uploadFileCommon = new UploadFileCommon();

            uploadFileCommon.CreateMultiplePath(Path.Combine(currentForlder, "Fileprocessing"));

            string pdfFilePath = txtFile.Text;
            if (!File.Exists(pdfFilePath))
                return;

            // @"C:\temp\";
            string outputPath = Path.Combine(currentForlder, "Fileprocessing");
            int interval = 1;
            int pageNameSuffix = 0;
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdfFilePath);
            FileInfo file = new FileInfo(pdfFilePath);
            string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
            {
                pageNameSuffix++;
                string newPdfFileName = string.Format(pdfFileName + "{0}", pageNameSuffix.ToString().PadLeft(5, '0'));
                //if (txtName.Text!= "")
                // newPdfFileName = string.Format(txtName.Text  + "-{0}", pageNameSuffix);                
                Console.WriteLine($"Dang cat file 1 trang 1 file: {outputPath}\\{newPdfFileName}.pdf");

                pdfMergefiles.SplitAndSaveInterval(pdfFilePath, outputPath, pageNumber, interval, newPdfFileName);
                //Path.Combine(outputPath, pdfFileName + ".pdf")

                convertPdftoPNG(Path.Combine(outputPath, newPdfFileName + ".pdf"), Path.Combine(outputPath, newPdfFileName + ".png"));



            }
        }

       
    }
}
