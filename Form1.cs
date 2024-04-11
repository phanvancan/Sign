using System;
using System.Collections.Generic;

using System.Threading.Tasks;

 
using Org.BouncyCastle.Security;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.security;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using System.Runtime.ConstrainedExecution;
using Org.BouncyCastle.Crypto.Tls;
using System.Security.Claims;
using System.Security.Cryptography;
using Org.BouncyCastle.Ocsp;
using static System.Windows.Forms.AxHost;





namespace SiginBS
{
    public partial class Form1 : Form
    {
        public static IList<X509Certificate> chain = new List<X509Certificate>();
        public static X509Certificate2 pk;
        private const string keyGetCompany = "CN=";
        private static IOcspClient ocspClient;
        private static ITSAClient tsaClient;
        private static IList<ICrlClient> crlList;
        private static readonly string fullPathAppOfCurrentUser = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string templatepdfFile = "temple.pdf";

        public Form1()
        {
            InitializeComponent();
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

             

            using (PdfReader pdfReader = new PdfReader(fileToSign))
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


                  //  net-sign: "772be5e76efb47c69e3c79e24dac526d",71239

                        //Also tried like this:
                        //signatureAppearance.CertificationLevel = currentSignaturesCount == 0 ? PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS : PdfSignatureAppearance.NOT_CERTIFIED;
                        // with message: "There have been changes made to this document that invalidate the signature"

                        // sign document
                        try
                        {
                            X509Certificate2 cert = pk;
                            X509CertificateParser cp = new X509CertificateParser();
                            X509Certificate[] chain = new  X509Certificate[]
                           {
                               cp.ReadCertificate(pk.RawData) 
                           };
                            
                            //X509Chain ch = new X509Chain();
                            //ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                            //ch.Build(cert);


                            IExternalSignature externalSignature = new X509Certificate2Signature(pk, "SHA-256");
                            MakeSignature.SignDetached(signatureAppearance, externalSignature,
                                chain
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
                templatepdfFile = theDialog.FileName;

                SignFile(templatepdfFile, "", int.Parse(textBox3.Text), int.Parse(textBox4.Text), float.Parse(textBox1.Text),
                float.Parse(textBox2.Text)
                , 1);
            }
        }
    }
}
