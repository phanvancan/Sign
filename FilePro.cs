using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
 
using System.IO;
using System.Collections.Generic;
 
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using iTextSharp.text.pdf.parser;

namespace SiginBS
{



    public class FileUploadInfo
    {
        public HttpPostedFile File { get; set; }
        public string FileName { get; set; }
        public int Chunk { get; set; }
        public int TotalChunk { get; set; }

        public static FileUploadInfo FromRequest(HttpRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
            {
                return null;
            }

            return new FileUploadInfo
            {
                File = request.Files[0],
                FileName = request.Headers["name"],
                Chunk = Convert.ToInt32(request.Headers["chunk"]),
                TotalChunk = Convert.ToInt32(request.Headers["chunks"])
            };
        }
    }
    public enum ResultCode : int
    {
        #region Common error codes (000 ~ 099)

        NoError = 1,
        UnknownError,
        TokenInvalid,
        NotFoundResourceId,
        IdNotMatch,
        NotModified,
        WaitNextRequest,
        DataInvalid = 8,
        DataIsUsed = 9,
        FileLarge = 10,
        #endregion Common error codes

        #region System error codes (100 ~ 199)

        SystemConfigNotFound = 100,
        SystemConfigInvalid,
        FileNotFound,
        ReadWriteFileError,

        #endregion System error codes

        #region Client error codes (200 ~ 199)

        RequestDataInvalid = 200,
        NotEnoughPermission,

        #endregion Client error codes

        #region Noitice Invoice error codes (300 ~ 399)

        // Login, Session error codes
        UserIsDisabled = 300,
        UserNotFound,
        LoginFailed,
        SessionAlive,
        SessionEnded,

        // Insert, update data error codes
        ConflictResourceId,

        #endregion Noitice Invoice error codes

        #region Common error codes (1000 ~ 1999)
        NotAuthorized = 1000,
        NotPermisionData,
        TimeOut,

        #endregion User operation error codes

        #region Login error codes (2000 ~ 2009)
        LoginUserIdIsEmpty = 2000,
        LoginUserIdNotExist,
        LoginEmailInvalid,
        LoginEmailNotExist,
        LoginPasswordIsEmpty,
        LoginOldPasswordIncorrect,
        #endregion

        #region Invoice error codes (2010 ~ 2019)
        InvoiceReleasedNotDelete = 2010,
        InvoiceReleasedNotUpdate = 2011,
        NumberInvoiceOverloadRegister = 2012,
        InvoiceIsConverted = 2013,
        InvoiceDateInvalid = 2014,
        #endregion

        #region User Account Management error codes (2020 ~ 2039)
        UserAccountMgtUsernameExceedMaxLength = 2020,
        UserAccountMgtUsernameIsEmpty,
        UserAccountMgtEmailExceedMaxLength,
        UserAccountMgtPasswordExceedMaxLength,
        UserAccountMgtEmailIsEmpty,
        UserAccountMgtEmailInvalid,
        UserAccountMgtPasswordIsEmpty,
        UserAccountMgtConflictResourceUserId = 2027,
        UserAccountMgtUserIdIsEmpty = 2028,
        UserAccountMgtUserIdExceedMaxLength,
        UserAccountMgtConflictResourceEmail,
        UserAccountMgtCompanyIdNotFound = 2031,
        UserAccountMgtNotPermissionDelete,
        UserAccountMgtConflictResourceTaxCode = 2033,

        #endregion Project management error codes

        #region Client error codes (2040~2059)

        // English name exceeds maximum length
        ClientEnglishNameExceedsMaxLength = 2040,
        // Local name exceeds maximum length
        ClientLocaNameExceedsMaxLength,

        // English address exceeds maximum length
        ClientEnglishAddressExceedsMaxLength,
        // Local address exceeds maximum length
        ClientLocalAddressExceedsMaxLength,

        // Phone number exceeds maximum length
        ClientPhoneNumberExceedsMaxLength,
        // Fax number exceeds maximum length
        ClientFaxNumberExceedsMaxLength,

        // Contact person's name exceeds maximum length
        ClientContactNameExceedsMaxLength,
        // Email address exceeds maximum length
        ClientContactEmailExceedsMaxLength,

        // You have to enter at least a name of client
        ClientClientNameIsEmpty,
        // You have to enter at least a address of client
        ClientAddressIsEmpty,
        // Format of email is invalid
        ClientEmailInvalidFormat,

        #endregion

        #region Country admin account management error codes(2080 ~ 2999)

        CountryAdminMgtSearchTextExceedMaxLength = 2080,
        CountryAdminMgtUsernameExceedMaxLength,
        CountryAdminMgtFormatUserNameInvalid,
        CountryAdminMgtEmailExceedMaxLength,
        CountryAdminMgtEmailInvalid,
        CountryAdminMgtPasswordInvalid,
        CountryAdminMgtPasswordExceedMaxLength,
        CountryAdminMgtEmailIsEmpty,
        CountryAdminMgtPasswordIsEmpty,

        #endregion Country admin account management error codes

        #region Company error Code (3000 ~ 3029)

        CompanyNameBlank = 3000,
        CompanyAddressBlank,
        CompanyAdminNameBlank,
        CompanyAdminEmailBlank,
        CompanyTaxInvalid,
        CompanyEnglishCompanyNameMaxLength,
        CompanyLocalCompanyNameMaxLength,
        CompanyEnglishAddressMaxLength,
        CompanyLocalAddressMaxLength,
        CompanyFirstPhoneNumberMaxLength,
        CompanyPhoneNumberMaxLength = 3010,
        CompanyFaxNumberMaxLength = 3011,
        CompanyHomepageMaxLength,
        CompanyAdminNameMaxLength,
        CompanyAdminEmailMaxLength,
        CompanyAdminEmailInvalid,
        CompanyNameMaxLength = 3016,
        CompanyNameIsExitsTaxCode = 3017,
        CompanyNameIsExitsEmail = 3018,
        CompanyNameCannotCreated = 3019,

        #endregion

        #region Register Template error Code (3030 ~ 3060)
        TemplateInvoiceBeingused = 3030,
        TemplateCodeIdIsExisted = 3031,

        #endregion

        #region Noitice Invoice error codes (4000 ~ 4050)

        NoiticeUseInvoiceApprovedNotUpdate = 4000,
        NoiticeUseInvoiceApprovedNotDelete = 4001,
        NoticeDetailSymbolIsExisted = 4002,
        InvoiceApprovedNotHasCustomer = 4003,
        InvoiceApprovedNotHasItem = 4004,

        #endregion Noitice Invoice error codes

        #region Release Invoice error codes (4051 ~ 4100)

        ReleaseInvoiceApprovedNotUpdate = 4051,
        ReleaseUseInvoiceApprovedNotDelete = 4052,
        ReleaseDetailSymbolIsExisted = 4053,

        #endregion Noitice Invoice error codes

        #region Cancelling Invoice error codes (4101 ~ 4140)

        InvoiceIssuedNotUpdate = 4101,

        #endregion Cancelling Invoice error codes

        #region ImportData
        ImportDataSizeOfFileTooLarge = 5050,
        ImportFileFormatInvalid = 5051,
        ImportColumnIsNotExist,
        ImportDataIsEmpty = 5053,
        ImportDataExceedMaxLength,
        ImportDataFormatInvalid,
        ImportDataNotSuccess,
        ImportDataIsExisted = 5057,
        ImportDataIsNotNumberic = 5058,
        ImportDataIsNotDateTime,
        ImportDataDaeteOfInvoiceInvalid = 5060,
        #endregion

        #region PayMent
        ExceedAmountOfInvoice = 6000,

        #endregion

        #region Contract
        ContractNotYetApporved = 8000,
        ContractNoExisted,
        ContractNotExisted,
        ContractCustomerNotYetUse,
        ContractLoginUserIsEmpty = 8004,
        ContractDownloaded = 8005,
        ContractCustomerUsedInvoice,
        #endregion

        #region Agencies
        AgenciesHasContract = 9000,
        #endregion

        #region Customer
        CustomerHasContract = 9050,
        #endregion
    }
    public class UploadFileCommon
    {
        

        public UploadFileCommon() { }
        //public ResultCode SaveFile(FileUploadInfo fileUploadInfo, string filePath)
        //{
        //    var file = fileUploadInfo.File;
        //    if (file == null || file.InputStream == null || string.IsNullOrWhiteSpace(filePath))
        //    {
        //        return ResultCode.RequestDataInvalid;
        //    }

        //    ResultCode resultCode = ResultCode.WaitNextRequest;
        //    using (var fs = new FileStream(filePath, fileUploadInfo.Chunk == 0 ? FileMode.Create : FileMode.Append))
        //    {
        //        var buffer = new byte[Config.ApplicationSetting.Instance.MaxLengthBuffer];
        //        int bytesRead;
        //        while ((bytesRead = file.InputStream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            fs.Write(buffer, 0, bytesRead);
        //        }
        //    }

        //    if (fileUploadInfo.Chunk == (fileUploadInfo.TotalChunk - 1))
        //    {
        //        resultCode = ResultCode.NoError;
        //    }

        //    return resultCode;
        //}
        //How to create multiple directories from a single full path in C#?
        public static List<String> GetAllFiles(String directory)
        {
            if (!directory.EndsWith("\\"))
            {
                directory = directory + "\\";
            }

            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).ToList();
        }

        public string CreateMultiplePath(String path)
        {// Lay thu muc  LocalLow
            //  string  currentForlder = System.IO.Path.Combine(Environment.GetFolderPathEnvironment.SpecialFolder.ApplicationData), "template");
            //C:\Users\ccanpv\AppData\LocalLow\xmlvb
            //  string folderPath = HttpContext.Current.Server.MapPath(path);
            if(!path.EndsWith("\\"))
            {
                path = path + "\\";
            }
             
            String folderPath = path;
            try
            {
                string folder = System.IO.Path.GetDirectoryName(folderPath);
                if (!Directory.Exists(folder))
                {
                    try
                    {
                        // Try to create the directory.
                        DirectoryInfo di = Directory.CreateDirectory(folder);
                    }
                    catch { }
                }
            }
            catch (IOException ioex)
            {
               // Console.WriteLine(ioex.Message);
                return ioex.Message;
            }
            return path;
        }

        public bool CreateFilePathByDocId(string path)
        {
            // string path = System.IO.Path.Combine(Config.ApplicationSetting.Instance.FolderAssetOfCompany, companyId.ToString(), AssetSignInvoice.Release, AssetSignInvoice.TempSignFile, releaseId.ToString());

            string folderPath = HttpContext.Current.Server.MapPath(path);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return true;
            // return System.IO.Path.Combine(folderPath, fileUploadInfo.FileName);
        }


        public bool IsUploadedFile(FileUploadInfo fileUploadInfo)
        {
            return fileUploadInfo.Chunk == (fileUploadInfo.TotalChunk - 1);
        }

        //public string StandardFileUpload(string sourceFile)
        //{
        //    string newFilePath = string.Empty;
        //    try
        //    {
        //        FileInfo fileInfo = new FileInfo(sourceFile);

        //        var extension = fileInfo.Extension;
        //        var nameWithoutExtension = fileInfo.Name.Remove(fileInfo.Name.LastIndexOf(extension));

        //        string suffix = DateTime.Now.ToString(Formatter.DateTimeFormat);
        //        string newName = nameWithoutExtension + Characters.Underscore + suffix + extension;

        //        string folderPath = fileInfo.DirectoryName;
        //        string destFile = System.IO.Path.Combine(folderPath, newName);

        //        fileInfo.MoveTo(destFile);
        //        int companyId = GetCompanyIdOfUser();
        //        string path = System.IO.Path.Combine(Config.ApplicationSetting.Instance.FolderAssetOfCompany, companyId.ToString(), AssetSignInvoice.Release, AssetSignInvoice.SignFile);
        //        string tagetFullPath = HttpContext.Current.Server.MapPath(path);
        //        if (!Directory.Exists(tagetFullPath))
        //        {
        //            Directory.CreateDirectory(tagetFullPath);
        //        }
        //        FileProcess.ExtractFile(destFile, tagetFullPath);
        //        newFilePath = destFile;
        //        DeleteFolderTempl(folderPath);
        //    }
        //    catch
        //    {
        //        // Don't need handle exception. 
        //        // When exception, will set file path is source file
        //        // TODO: Maybe, write log to trace exception.
        //        newFilePath = string.Empty;
        //    }

        //    return newFilePath;

        //}
        //private void DeleteFolderTempl(string destFile)
        //{

        //    if (destFile.IsNotNullOrEmpty() && Directory.Exists(destFile))
        //    {
        //        Directory.Delete(destFile, true);
        //    }
        //}
        //public string StandardFileUpload(string sourceFile, int companyId)
        //{
        //    string newFilePath = string.Empty;
        //    try
        //    {
        //        FileInfo fileInfo = new FileInfo(sourceFile);

        //        var extension = fileInfo.Extension;
        //        var nameWithoutExtension = fileInfo.Name.Remove(fileInfo.Name.LastIndexOf(extension));

        //        string suffix = DateTime.Now.ToString(Formatter.DateTimeFormat);
        //        string newName = nameWithoutExtension + Characters.Underscore + suffix + extension;

        //        string folderPath = fileInfo.DirectoryName;
        //        string destFile = System.IO.Path.Combine(folderPath, newName);

        //        fileInfo.MoveTo(destFile);

        //        string path = System.IO.Path.Combine(Config.ApplicationSetting.Instance.FolderAssetOfCompany, companyId.ToString(), AssetSignInvoice.Release, AssetSignInvoice.SignFile);
        //        string tagetFullPath = HttpContext.Current.Server.MapPath(path);
        //        if (!Directory.Exists(tagetFullPath))
        //        {
        //            Directory.CreateDirectory(tagetFullPath);
        //        }
        //        FileProcess.ExtractFile(destFile, tagetFullPath);
        //        newFilePath = destFile;
        //        DeleteFolderTempl(folderPath);
        //    }
        //    catch
        //    {
        //        // Don't need handle exception. 
        //        // When exception, will set file path is source file
        //        // TODO: Maybe, write log to trace exception.
        //        newFilePath = string.Empty;
        //    }

        //    return newFilePath;

        //}
    }

    public class SendMail
    {
        public SendMail()
        {
        }
    //    public static async Task<bool> _send(string subject, string body, string[] MailTo)
    //{
    //    string MailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
    //    string MailFromPassword = System.Configuration.ConfigurationManager.AppSettings["MailFromPassword"];

    //        //string MailTo = System.Configuration.ConfigurationManager.AppSettings["MailTo"];
    //        // string MailTo2 = System.Configuration.ConfigurationManager.AppSettings["MailTo2"];
    //        //string MailTo3 = System.Configuration.ConfigurationManager.AppSettings["MailTo3"];

    //        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
    //    foreach (string s in MailTo)
    //    {

    //        var foo = new EmailAddressAttribute();
    //        bool ebar;
    //        ebar = foo.IsValid(s);

    //        if (ebar) mail.To.Add(s);// to mailfo
    //                                 //mail.To.Add(MailTo2);// to mail

    //    }// mail.To.Add(MailTo3);// to mail

    //    mail.From = new MailAddress(MailFrom, "Your Health");
    //    mail.Subject = subject;
    //    //DateTime dateLogin = DateTime.Now;
    //    mail.Body = body;

    //    //mail.Body = "Bạn đã đăng nhập thành công với tài khoản: " +  "<div><B> "+ model.Username + "</B></div>"  + " vào lúc " + DateTime.Now.ToString("hh:mm tt dd/MM/yyyy");



    //    mail.IsBodyHtml = true;
    //    SmtpClient smtp = new SmtpClient();
    //    smtp.Host = "smtp.gmail.com";
    //    smtp.Port = 587;
    //    smtp.UseDefaultCredentials = true;
    //    smtp.Credentials = new System.Net.NetworkCredential(MailFrom, MailFromPassword);
    //    smtp.EnableSsl = true;
    //    try
    //    {
    //        await smtp.SendMailAsync(mail);
    //        // tam thoi khong gui mail

    //        return true;
    //    }
    //    catch
    //    {

    //        // return false;
    //    }
    //    return false;
    //}
    
    }
    //public class eMail
    //{
    //    public void SendEmailWebMail(string subject, string body,
    //    string toAddress,
    //    string cc = null, string bcc = null,
    //    IEnumerable<string> attachmentFilePath = null,
    //    IDictionary<string, string> headers = null)
    //    {
    //        System.Web.Mail.MailMessage message = new System.Web.Mail.MailMessage();
    //        //from, to, reply to
    //        // message.From = new MailAddress(fromAddress, fromName);
    //        string fromAddress = ConfigurationManager.AppSettings["fromAddress"];
    //        message.From = fromAddress;

    //        //message.From = new MailAddress("info@caythuoc.net", "info@caythuoc.net");

    //        //message.To.Add(new MailAddress(toAddress, toName));

    //        message.To = toAddress; //"thanh1976.nl@gmail.com";
    //                                // message.Cc = cc;
    //                                //if (!String.IsNullOrEmpty(replyTo))
    //                                //{
    //                                //    message.ReplyToList = (new MailAddress(replyTo, replyToName));
    //                                //}

    //        //BCC
    //        if (bcc != null)
    //        {
    //            //foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
    //            //{
    //            message.Bcc = bcc;//(address.Trim());
    //            //}
    //        }

    //        //message.Bcc = "nguyenlethanh@baoviet.com.vn";

    //        //CC
    //        if (cc != null)
    //        {
    //            //  foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
    //            //  {
    //            message.Cc = cc;//(address.Trim());
    //            //}
    //        }

    //        message.Subject = subject;
    //        message.Body = body;
    //        message.BodyEncoding = System.Text.Encoding.UTF8;
    //        message.BodyFormat = MailFormat.Html;

    //        //LinkedResource logo = new LinkedResource("");
    //        //System.Net.Mail.Attachment attachment;
    //        //attachment = New System.Net.Mail.Attachment(Server.MapPath("~/App_Data/hello.pdf"));
    //        //mail.Attachments.Add(attachment);

    //        if (attachmentFilePath != null)
    //        {
    //            foreach (var strFile in attachmentFilePath.Where(attachmentFilePathValue => !String.IsNullOrWhiteSpace(attachmentFilePathValue)))
    //            {
    //                MailAttachment attachFile = new MailAttachment(strFile);

    //                message.Attachments.Add(attachFile);
    //            }

    //            // message.Attachments.GetType().
    //            //message.Attachments.Add(attachFile);

    //        }
    //        string smptServer = ConfigurationManager.AppSettings["smptServer"];
    //        string emailAccount = ConfigurationManager.AppSettings["emailAccount"];
    //        string emailPass = ConfigurationManager.AppSettings["emailPass"];
    //        string emailPort = ConfigurationManager.AppSettings["emailPort"];
    //        string smtpusessl = ConfigurationManager.AppSettings["smtpusessl"];

    //        string smtpconnectiontimeout = System.Configuration.ConfigurationManager.AppSettings["smtpconnectiontimeout"];
    //        SmtpMail.SmtpServer = smptServer;//

    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", emailAccount);
    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", emailPass);
    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", emailPort);
    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", smtpusessl);
    //        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", smtpconnectiontimeout);

    //        SmtpMail.Send(message);
    //        // Log log = new Log(); log.Logwrite(DateTime.Now.ToString("yyyyMMdd"), message.ToString());

    //    }
    //    /// <summary>
    //    /// Chuyen file anh về dang bytes
    //    /// </summary>
    //    /// <param name="path"></param>
    //    /// <returns></returns>
    //    public byte[] ImageBytes(string path)
    //    {
    //        byte[] imageBytes = null;
    //        Image a = new Bitmap(path);//Bitmap(@".../path/to/image.png");

    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            // Convert Image to byte[]
    //            a.Save(ms, a.RawFormat);
    //            imageBytes = ms.ToArray();
    //            //this.richTextBox1.Text = "<img src=\"data:image/png;base64," + Convert.ToBase64String( imageBytes ) + "\"/>";
    //        }
    //        return imageBytes;
    //    }
    //}
    public class Footer
    {
        public string Name { get; set; } = string.Empty;
        public string Chucvu { get; set; } = string.Empty;
        public string Phong { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public static class  fileCommon 
    {
        //picturebox1.Image = fileCommon.LoadBitmap("LOCATION");
        public static Bitmap LoadBitmap(string path)
        {
            if (File.Exists(path))
            {
                // open file in read only mode
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                // get a binary reader for the file stream
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // copy the content of the file into a memory stream
                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                    // make a new Bitmap object the owner of the MemoryStream
                    return new Bitmap(memoryStream);
                }
            }
            else
            {
                MessageBox.Show("Error Loading File.", "Error!", MessageBoxButtons.OK);
                return null;
            }
        }
    }
}