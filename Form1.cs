using System;
using System.IO;
using System.Collections.Generic;

 
using iTextSharp.text.pdf;

using iTextSharp.text.pdf.security;

using System.Windows.Forms;

using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
 
 
  

using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;

using System.Drawing.Drawing2D;
using System.Net;
using System.Web.UI.WebControls;
using iTextSharp.text;
using System.Security.Policy;
using SiginBS.Common;
using System.Diagnostics;
using SiginBS.Properties;
using SiginBS.Models;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using static System.Net.Mime.MediaTypeNames;
using itext.pdfimage;
//using com.itextpdf.text.pdf;


//using iTextSharp.text.pdf.parser;
//using Org.BouncyCastle.Crypto;
//using iText.Kernel.Pdf;
//using iText.Signatures;


namespace SiginBS
{
    public partial class Form1 : Form
    {
        public static IList<Org.BouncyCastle.X509.X509Certificate> chain = new List<Org.BouncyCastle.X509.X509Certificate>();
        public static X509Certificate2 pk;
      //  private const string keyGetCompany = "CN=";
       // private static IOcspClient ocspClient;
        //private static ITSAClient tsaClient;
       // private static IList<ICrlClient> crlList;
       // private static readonly string fullPathAppOfCurrentUser = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
       // private string templatepdfFile = "temple.pdf";

        private pdfMergefiles pdfMergefiles= new pdfMergefiles();
        private UploadFileCommon uploadFileCommon = new UploadFileCommon();
        private string imagePath = "";
       // string imagePath = "";
        private bool SelectingArea = false;
        private Point StartPoint, EndPoint, StartPointTemp;
        private Point SaveStartPoint, SaveEndPoint;
        PictureBox picCanvas = new PictureBox();
        List<fileIndex> lstindex = new List<fileIndex>();
        private bool openSignOk = false;
        public Form1()
        {
            InitializeComponent();

            this.picCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picCanvas.TabIndex = 0;
            this.picCanvas.TabStop = false;
            this.picCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.picCanvas_Paint);
            this.picCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picCanvas_MouseDown);
            this.picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picCanvas_MouseMove);
            this.picCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picCanvas_MouseUp);
            this.Resize += Form1_Resize;

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Left = button3.Left + button3.Width+ listView1.Width + 30;
            panel1.Width = Width - (button3.Width + 100);
            panel1.Height = Height;

        }

        private void button3_Click(object sender, EventArgs e)
        {
           // picCanvas.Load(@"C:\Users\ccanpv\AppData\Roaming\SignOffice\1.jpg");
            LoadfileImg(txtFile.Text);


        }
        private void LoadfileImg(string filename)
        {
            //if (picCanvas!= null) picCanvas.Dispose();
            Thumbnails thumbnails = new Thumbnails();
            var pdf = filename;

            PdfToImageConverter cvimg = new PdfToImageConverter();

            var img = cvimg.ConvertToImages(pdf.ToString());

            System.Windows.Forms.Application.DoEvents();


            var pathtemp = uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot));
            pathtemp = uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot));

            imagePath = System.IO.Path.Combine(pathtemp, $"img-{DateTime.Now.Ticks}.jpg");



            #region xoa file anh 120
            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(System.IO.Path.Combine(pathtemp, "thumb120"));
                Empty(directory);
            }
            catch { }

            #endregion xoa file anh 120
            // System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(pathtemp);
            var dirdel = Directory.GetFiles(pathtemp, "*.*", SearchOption.AllDirectories)
         .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                     s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                     s.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

            foreach (var file in dirdel)
            {try
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(file);
                }
                catch ( Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            int i = 1;
            foreach (var image in img)
            {
                imagePath = System.IO.Path.Combine(pathtemp, $"img-{i}-{DateTime.Now.Ticks}.jpg");
                System.Windows.Forms.Application.DoEvents();
                image.Save(imagePath, ImageFormat.Jpeg);

                image.Dispose();
                thumbnails.GeneralThumb(imagePath, "thumb120", 120, 0);
                thumbnails.GeneralThumb(imagePath, "thumb", 842, 0);
                i++;
            }



            //if (rt!= "error")
            //{
            //    imagePath = rt;
            //}
            ImageList image2 = new ImageList();
            lstindex = new List<fileIndex>();
            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.Combine(pathtemp, "thumb120"));
            i = 1;
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {


                    image2.Images.Add(System.Drawing.Image.FromFile(file.FullName));

                    string result = System.IO.Path.GetFileName(file.FullName);
                    lstindex.Add(new fileIndex { index = i, filename = result });
                    i++;
                    Console.WriteLine(file.FullName);
                }
                catch
                {
                    Console.WriteLine("This is not an image file");
                }
            }
            this.listView1.Items.Clear();

            this.listView1.View = System.Windows.Forms.View.LargeIcon;
            image2.ImageSize = new Size(120, 169);

            this.listView1.LargeImageList = image2;


            foreach (var r in lstindex)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = r.index - 1;
                item.Text = $"{r.index}";
                this.listView1.Items.Add(item);
            }



            //  Panel MyPanel = new Panel();
            //PictureBox picCanvas = new PictureBox();
            imagePath = System.IO.Path.Combine(pathtemp, "thumb", lstindex.FirstOrDefault().filename);

            System.Drawing.Image image1 = System.Drawing.Image.FromFile(imagePath);

            picCanvas.Image = image1;
            picCanvas.Height = image1.Height;
            picCanvas.Width = image1.Width;

            panel1.Controls.Add(picCanvas);
            panel1.AutoScroll = true;

            //this.Controls.Add(panel1);

            //picCanvas.Load(imagePath);

            //picCanvas.Width = 1005;
            //picCanvas.Height = 985;

            //pictureBox1.Load(imagePath);
            int zoomRatio = 10;
            //Set the zoomed width and height
            int widthZoom = picCanvas.Width * zoomRatio / 100;
            int heightZoom = picCanvas.Height * zoomRatio / 100;
            //zoom = true --> zoom in
            //zoom = false --> zoom out

            //Add the width and height to the picture box dimensions
            picCanvas.Width += widthZoom;
            picCanvas.Height += heightZoom;
            //image1.Dispose();

        }
    
        private void Form1_Load(object sender, EventArgs e)
        {



            //  imagePath = @"D:\temp\imresizer-1713165935821.png";
            //picCanvas.Load(imagePath);

            txtFile.Text = @"C:\Users\ccanpv\AppData\Roaming\SignOffice\lab01.pdf";
            // Org.BouncyCastle.Pkcs.Pkcs12Store pfxKeyStore = new Org.BouncyCastle.Pkcs.Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());


            // X509Certificate2Collection collection = new X509Certificate2Collection();
            // collection.Import(pfxFilePath, pfxPassword, X509KeyStorageFlags.DefaultKeySet);


            
            textBox1.Text = "120";
            textBox2.Text = "120";
            //int page = 1;
           // string pfxFilePath = @"C08-Cert.pfx";
          //  string pfxPassword = "Hanoi1";
          //  X509Certificate2 cert = new X509Certificate2(pfxFilePath, pfxPassword, X509KeyStorageFlags.UserKeySet);


            //pk = cert;
            //MessageBox.Show("Looix");


           

        }

        private string SignV2(string fileToSign, string filedesc, string certname, int w,
           int h, float llx, float lly, int page1, string filenameChuKy = @"img.png", bool onepage = true)

        {
            // Step 2: Load PDF Document
            string pdfFilePath = fileToSign;
            string destinationPath = filedesc;
            PdfReader pdfReader = new PdfReader(pdfFilePath);

        
            // Step 4: Count the pages and add signature in each page
            int page = pdfReader.NumberOfPages;
            for (int i = 1; i <= page; i++)
            {
                if (i > 1)
                {
                    FileStream stremfile = new FileStream(destinationPath, FileMode.Open, FileAccess.Read);
                    pdfReader = new PdfReader(stremfile);
                    File.Delete(destinationPath);
                }

                // Step 4.1: Initialize the PDF Stamper And Creating the Signature Appearance
                FileStream signedPdf = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite);
                PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0', null, true);
                PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                float x = 360;
                float y = 130;

                var pdfContentByte = pdfStamper.GetOverContent(i);

                var pageSize = pdfReader.GetPageSize(i);
              //  float pageWidth = pageSize.Width;//162
              //  float pageHeight = pageSize.Height;//792                                  

                //float picW = picCanvas.Width;
               // float picH = picCanvas.Height;
                var image = iTextSharp.text.Image.GetInstance(filenameChuKy, true);


                image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                image.ScaleToFit(150, 50);
                image.SetAbsolutePosition(x, y);
                pdfContentByte.AddImage(image);
                
                signatureAppearance.Reason = "Digital Signature Reason";
                signatureAppearance.Location = "Digital Signature Location";
                signatureAppearance.Acro6Layers = false;

                
               
                signatureAppearance.Acro6Layers = false;
                signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
                signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + 150, y + 50), i, null);

 
                
                X509CertificateParser cp = new X509CertificateParser();

                Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[]
                         {
                               cp.ReadCertificate(pk.RawData)
                         };
                iTextSharp.text.pdf.security.IExternalSignature privateKey = new X509Certificate2Signature(pk, DigestAlgorithms.SHA256);
               
                

                MakeSignature.SignDetached(signatureAppearance, privateKey, chain, null, null, null, 0, CryptoStandard.CMS);

                
                pdfReader.Close();
                pdfStamper.Close();
            }
            Console.WriteLine("PDF signed successfully!");
            return "";
        }

            private string SignFile(string fileToSign, string filedesc, string certname, int w, 
            int h, float llx, float lly, int page, string filenameChuKy = @"img.png", bool onepage=true)

        {
            Console.WriteLine("PDF sign begin ");

            string signedFile = System.IO.Path.GetFileName(filedesc).Replace(".pdf", $"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss")}.signed.pdf");
            signedFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filedesc), signedFile);
            string tempDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SignOffice", "tempfiles");           
            Directory.CreateDirectory(tempDir);


            //if (File.Exists(System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy))))
            //{

            //    Console.WriteLine(System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy)));
            //}
            //else
            //{
            //    System.IO.File.Copy(filenameChuKy, System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy)), true);
            //    Console.WriteLine($"copy to desc {filenameChuKy}");
            //}

            
          var   filenameChuKyDesc = System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy));

            if (File.Exists(filenameChuKyDesc))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
             //   File.Delete(filenameChuKyDesc);
            }
            else
            {
                GenerateStamp(filenameChuKy).Save(filenameChuKyDesc, ImageFormat.Png);
            }



            float x =0;
            float y =0;
            float _llx = llx;// (pageHeight * llx) / picH;
            float _lly = lly;// (pageWidth * lly) / picW;
            w = (int) ((float)w - ((float) w * 0.35277723881255185));
            h = (int)((float)h - ((float)h * 0.35277723881255185));

         

            //float _mm2Pt = Convert.ToSingle(0.35277723881255185)

            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(fileToSign))
            {
                Console.WriteLine("PDF sign begin ");

                int pages = pdfReader.NumberOfPages;
                var currentSignaturesCount = pdfReader.AcroFields.GetSignatureNames().Count();

                FileStream signedPdf = new FileStream(signedFile, FileMode.Create, FileAccess.ReadWrite);



                    string tempFileName = System.IO.Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".pdf");
                    if (!File.Exists(tempFileName))
                        File.Create(tempFileName).Close();

                    using (PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0', tempFileName, true))  // Append mode
                    {


                        if (page <= pages && page > 0)
                        {
                            var pdfContentByte = pdfStamper.GetOverContent(page);

                            var pageSize = pdfReader.GetPageSize(page);
                            float pageWidth = pageSize.Width;//162
                            float pageHeight = pageSize.Height;//792                                  

                            float picW = picCanvas.Width;
                            float picH = picCanvas.Height;
                           


                            var image = iTextSharp.text.Image.GetInstance(filenameChuKyDesc, true);

                            _llx = (pageHeight * llx) / picH;
                            _lly = (pageWidth * lly) / picW;

                            if (_llx > pageWidth)
                            {
                                _llx = pageWidth - w;
                            }

                            if (_lly > pageHeight)
                            {
                                _lly = pageHeight - h;
                            }

                            if (w > pageWidth)
                            {
                                w = (int)pageWidth;
                            }

                            if (h > pageHeight)
                            {
                                h = (int)pageHeight;
                            } 

                            x = _llx;
                            float llytemp = pageHeight - _lly - h;
                            _lly = llytemp;
                            y = _lly;

                            image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            image.ScaleToFit(w, h);
                            image.SetAbsolutePosition(_llx, _lly);                           
                            pdfContentByte.AddImage(image);
                        }






                        iTextSharp.text.pdf.PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                        

                        signatureAppearance.Reason = "Được ký bởi:\n" + pk.Subject.ToString();
                        signatureAppearance.SignDate = DateTime.Now;
                        //  signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                        signatureAppearance.Acro6Layers = false;



                        signatureAppearance.Layer4Text = iTextSharp.text.pdf.PdfSignatureAppearance.questionMark;
                        w = 150;
                        h = 50;

                        signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + w, y + h), page, "signature");

                       


                        try
                        {
                            X509Certificate2 cert = pk;
                            X509CertificateParser cp = new X509CertificateParser();
                            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[]
                           {
                               cp.ReadCertificate(pk.RawData)
                           };

                            //X509Chain ch = new X509Chain();
                            //ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                            //ch.Build(cert);


                            iTextSharp.text.pdf.security.IExternalSignature externalSignature = new X509Certificate2Signature(pk, "SHA-256");
                            MakeSignature.SignDetached(signatureAppearance, externalSignature, chain
                                , null, null, null, 0, CryptoStandard.CMS);

                        }
                        
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());

                        }
                        finally
                        {
                            if (pdfStamper != null)
                            {
                                pdfStamper.Close();
                                pdfStamper.Dispose();
                            }

                        }

                    if (signedPdf != null)
                    {
                        signedPdf.Close();
                        signedPdf.Dispose();
                    }


                }
            }
            Console.WriteLine("PDF signed successfully!");
            return signedFile;
        }

     

        Bitmap GenerateStamp(string urlimg)
        {
            System.Net.WebRequest request =
           System.Net.WebRequest.Create(urlimg);
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
                txtFile.Text =  theDialog.FileName;
                try
                {

                    button3_Click(sender, e);   
                }
                catch
                {
                    Console.WriteLine("Ko ky");
                }
               

            }
        }

        private void ExtractJpeg(string file1)
        {
            var dir1 = System.IO.Path.GetDirectoryName(file1);
            var fn = System.IO.Path.GetFileNameWithoutExtension(file1);
            var dir2 = System.IO.Path.Combine(dir1, fn);
            if (!Directory.Exists(dir2)) Directory.CreateDirectory(dir2);

            var pdf = new PdfReader(file1);
            int n = pdf.NumberOfPages;
            for (int i = 1; i <= n; i++)
            { 
                var pg = pdf.GetPageN(i);
                var res = PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES)) as PdfDictionary;
                var xobj = PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)) as PdfDictionary;

              

                if (xobj == null) continue;

                var keys = xobj.Keys;
                if (keys.Count == 0) continue;

                var obj = xobj.Get(keys.ElementAt(0));
                if (!obj.IsIndirect()) continue;

                var tg = PdfReader.GetPdfObject(obj) as PdfDictionary;
                var type = PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)) as PdfName;
                if (!PdfName.IMAGE.Equals(type)) continue;

                int XrefIndex = (obj as PRIndirectReference).Number;
                var pdfStream = pdf.GetPdfObject(XrefIndex) as PRStream;
                var data = PdfReader.GetStreamBytesRaw(pdfStream);
                var jpeg = System.IO.Path.Combine(dir2, string.Format("{0:0000}.png", i));
                File.WriteAllBytes(jpeg, data);
            }
        }

 
        private void button2_Click(object sender, EventArgs e)
        {
            PDFHelper pDF = new PDFHelper();
           
            
            pDF.ExtractImages( txtFile.Text, "D:\\temp\\1\\1", progressBar1 );
            progressBar1.Visible = false;
           


        }

        private void tachfile()
        {
         string currentForlder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot);

            UploadFileCommon uploadFileCommon = new UploadFileCommon();

            uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(currentForlder, "Fileprocessing"));

            string pdfFilePath = txtFile.Text;
            if (!File.Exists(pdfFilePath))
                return;

            // @"C:\temp\";
            string outputPath = System.IO.Path.Combine(currentForlder, "Fileprocessing");
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
                //System.IO.Path.Combine(outputPath, pdfFileName + ".pdf")

        



            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!openSignOk)
            {
                try
                {

                    X509Store x509Store = new X509Store(StoreLocation.CurrentUser);
                    x509Store.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection collection = (X509Certificate2Collection)x509Store.Certificates;
                    X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                    X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Chọn thiết bị xác thực ", "Chọn một chứng chỉ từ danh sách dưới đây để nhận thông tin về chứng chỉ đó", X509SelectionFlag.MultiSelection);
                    if (scollection.Count > 0)
                    {
                        pk = scollection[0];
                        openSignOk = true;
                    }
                    else
                    {
                        openSignOk = false;
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
                catch (Exception ex)
                {
                    openSignOk = false;
                    MessageBox.Show(ex.Message);

                }
            }
            //llx = x;
            //lly = page.Rect.Height - y - height;
            //urx = x + width;
            //ury = page.Rect.Height - y;

            //float _mm2Pt = Convert.ToSingle(0.35277723881255185);
            //var pdfDocument = new Document(fs);

            //var currentPage = pdfDocument.Pages[pageNumber];
            //var pHeight = currentPage.Rect.Height;
            //var bX = (x / _mm2Pt);
            //var bY = pHeight - (y / _mm2Pt) - (h / _mm2Pt);
            //var bW = bX + (w / _mm2Pt);
            //var bH = bY + (h / _mm2Pt);

            //var appearance = new DefaultAppearance(font, fontSize, System.Drawing.Color.FromName(fontColor));
            //var rectangle = new Rectangle(bX, bY, bW, bH);

            int w = int.Parse(textBox3.Text) - int.Parse(textBox1.Text);
            int h = int.Parse(textBox4.Text) - int.Parse(textBox2.Text);
            float llx=0, lly = 0, pich = float.Parse( textBox6.Text), picw = float.Parse(textBox5.Text);
            
            //var pHeight = pich;

            //llx = (picw / _mm2Pt);

            //lly =  pHeight - (w / _mm2Pt) - (h / _mm2Pt);
             llx = int.Parse (textBox1.Text);
             lly = int.Parse(textBox2.Text);
            string currentForlder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot);

            UploadFileCommon uploadFileCommon = new UploadFileCommon();
            //System.IO.Path.Combine(currentForlder, "Fileprocessing"
            var pathtemp = uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(currentForlder, "Fileprocessing"));


            string filedesc =System.IO.Path.Combine(pathtemp, System.IO.Path.GetFileName(txtFile.Text));

            int page = 1;
            if (listView1.Tag!=null)  int.TryParse(listView1.Tag.ToString(), out  page);
            //SignV2
            var fileSign = SignFile(txtFile.Text, filedesc, "", w, h, llx, lly, page, "https://api.hoguomopera.vn/img.png", checkBox1.Checked);
           // var fileSign = SignV2(txtFile.Text, filedesc, "", w, h, llx, lly, page, "img.png", checkBox1.Checked);

            button5.Tag = fileSign;
            
            LoadfileImg(fileSign);
            
                


        }

        void vehinh()
        {
            string currentForlder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot);


            System.Drawing.Image image = System.Drawing.Image.FromFile(System.IO.Path.Combine(currentForlder, "thumb", imagePath));


            textBox5.Text= image.Width.ToString();
            textBox6.Text = image.Height.ToString();

            Graphics newGraphics = Graphics.FromImage(image);



            using (Pen pen = new Pen(Color.Yellow, 2))
            {
                newGraphics.DrawRectangle(pen, MakeRectangle(SaveStartPoint, SaveEndPoint));

                pen.Color = Color.Red;
                pen.DashPattern = new float[] { 5, 5 };
                newGraphics.DrawRectangle(pen, MakeRectangle(SaveStartPoint, SaveEndPoint));
            }
            // image.Save("xulys.jpg");
            StartPointTemp = new Point();
            StartPointTemp.X = SaveStartPoint.X - 30;
            StartPointTemp.Y = SaveStartPoint.Y - 20;
            newGraphics.DrawString($"Form ({SaveStartPoint.X},{SaveStartPoint.Y})", new System.Drawing.Font("Arial", 16F, FontStyle.Regular), Brushes.Red, StartPointTemp);

            newGraphics.DrawString($"To ({SaveEndPoint.X},{SaveEndPoint.Y})", new System.Drawing.Font("Arial", 16F, FontStyle.Regular), Brushes.Red, SaveEndPoint);
            
            textBox1.Text = SaveStartPoint.X.ToString();
            textBox2.Text = SaveStartPoint.Y.ToString();

            textBox3.Text = SaveEndPoint.X.ToString();
            textBox4.Text = SaveEndPoint.Y.ToString();


            picCanvas.Image = image;
        }

        private void picCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            StartPoint = e.Location;
            EndPoint = e.Location;
            SelectingArea = true;
            picCanvas.Cursor = Cursors.Cross;

            // Refresh.
            picCanvas.Refresh();
        }

        // Continue drawing.
        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Update the end point.
            EndPoint = e.Location;

            // Refresh.
            picCanvas.Refresh();
        }

        // Continue drawing.
        private void picCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            SelectingArea = false;
            picCanvas.Cursor = Cursors.Default;

            // Do something with the selection rectangle.
            //  Rectangle rect = MakeRectangle(StartPoint, EndPoint);


            SaveStartPoint = StartPoint;
            SaveEndPoint = EndPoint;
            vehinh();

            // Console.WriteLine(rect.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {   if (button5.Tag == null) return;
            Process p = new Process();
            p.StartInfo.FileName = button5.Tag.ToString();
            p.Start(); 
        }

        private void Empty(  System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
            {
                subDirectory.Delete(true);
            }

        }
        private void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            var pathtemp = uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot, "tempfiles"
));
        
            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(pathtemp);

                Empty(directory);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete file {ex.Message}");
            }


            pathtemp = uploadFileCommon.CreateMultiplePath(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot ));

            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(pathtemp);

                foreach (System.IO.FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete file {ex.Message}");
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0)
            {
                return;
            }
            int intselectedindex = listView1.SelectedIndices[0];
            
            if (intselectedindex >= 0)
            {
                string text = listView1.Items[intselectedindex].Text;
               //  Console.WriteLine(text);
                
                imagePath = lstindex.Where(x=>x.index==int.Parse(text)).FirstOrDefault().filename;
                listView1.Tag = text;
                string currentForlder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.FolderRoot);

                string filename = System.IO.Path.Combine(currentForlder, "thumb", imagePath);

                if (!File.Exists(filename))
                {
                    picCanvas.Image = null;
                    return;
                }
                System.Drawing.Image image1 = System.Drawing.Image.FromFile(filename);

                picCanvas.Image = image1;
                picCanvas.Height = image1.Height;
                picCanvas.Width = image1.Width;

                panel1.Controls.Add(picCanvas);
                panel1.AutoScroll = true;

                //this.Controls.Add(panel1);

                //picCanvas.Load(imagePath);

                //picCanvas.Width = 1005;
                //picCanvas.Height = 985;

                //pictureBox1.Load(imagePath);
                int zoomRatio = 10;
                //Set the zoomed width and height
                int widthZoom = picCanvas.Width * zoomRatio / 100;
                int heightZoom = picCanvas.Height * zoomRatio / 100;
                //zoom = true --> zoom in
                //zoom = false --> zoom out

                //Add the width and height to the picture box dimensions
                picCanvas.Width += widthZoom;
                picCanvas.Height += heightZoom;


            }

            //Console.WriteLine(listView1.SelectedItems[0].Text);



        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            //string tempDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SignOffice", "tempfiles");
            //Directory.CreateDirectory(tempDir);

            //string filenameChuKy = "img.png";
            //if (File.Exists(System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy))))
            //{

            //    Console.WriteLine(System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy)));
            //}
            //else
            //{
            //    System.IO.File.Copy(filenameChuKy, System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy)), true);
            //    Console.WriteLine($"copy to desc {filenameChuKy}");
            //}
            //filenameChuKy = System.IO.Path.Combine(tempDir, System.IO.Path.GetFileName(filenameChuKy));
        }

        // Draw the selection rectangle.
        private void picCanvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;



            if (SelectingArea)
            {
                using (Pen pen = new Pen(Color.Yellow, 2))
                {



                    e.Graphics.DrawRectangle(pen,
                        MakeRectangle(StartPoint, EndPoint));

                    pen.Color = Color.Red;
                    pen.DashPattern = new float[] { 5, 5 };
                    e.Graphics.DrawRectangle(pen,
                        MakeRectangle(StartPoint, EndPoint));
                }
            }
            else
            {


                // DrawSmiley(e.Graphics, EndPoint, 50);

            }
        }

        // Make a rectangle from two points.
        private System.Drawing.Rectangle MakeRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            return new System.Drawing.Rectangle(x, y, width, height);
        }

        ////////////
        ///

        private void DrawSmiley(Graphics gr, PointF center, float radius)
        {
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen thick_pen = new Pen(Color.Red, 2))
            {
                // Draw the face.
                RectangleF rect = new RectangleF(
                    center.X - radius,
                    center.Y - radius,
                    2 * radius, 2 * radius);
                gr.FillEllipse(Brushes.Yellow, rect);
                gr.DrawEllipse(thick_pen, rect);

                // Left eye.
                rect = new RectangleF(
                    center.X - 0.6f * radius,
                    center.Y - 0.6f * radius,
                    0.4f * radius,
                    0.6f * radius);
                gr.FillEllipse(Brushes.White, rect);
                gr.DrawEllipse(Pens.Black, rect);

                // Left pupil.
                rect = new RectangleF(
                    center.X - 0.4f * radius,
                    center.Y - 0.5f * radius,
                    0.2f * radius,
                    0.4f * radius);
                gr.FillEllipse(Brushes.Black, rect);

                // Right eye.
                rect = new RectangleF(
                    center.X + 0.2f * radius,
                    center.Y - 0.6f * radius,
                    0.4f * radius,
                    0.6f * radius);
                gr.FillEllipse(Brushes.White, rect);
                gr.DrawEllipse(Pens.Black, rect);

                // Right pupil.
                rect = new RectangleF(
                    center.X + 0.4f * radius,
                    center.Y - 0.5f * radius,
                    0.2f * radius,
                    0.4f * radius);
                gr.FillEllipse(Brushes.Black, rect);

                // Nose.
                rect = new RectangleF(
                    center.X - 0.15f * radius,
                    center.Y - 0.15f * radius,
                    0.3f * radius,
                    0.5f * radius);
                gr.FillEllipse(Brushes.LightBlue, rect);
                gr.DrawEllipse(Pens.Blue, rect);

                // Smile.
                float smile_radius = radius * 0.7f;
                rect = new RectangleF(
                    center.X - smile_radius,
                    center.Y - smile_radius,
                    2 * smile_radius,
                    2 * smile_radius);
                thick_pen.Color = Color.Green;
                thick_pen.Width = 3;
                gr.DrawArc(thick_pen, rect, 20, 140);



            }
        }


    }
}
