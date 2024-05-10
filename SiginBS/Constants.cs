
namespace SiginBS
{
    public static class Constants
    {
        public const string TRANSLATOR_RESOURCE = "Translator.App.Resources.StringResource";
        public const string OrderByDesc = "DESC";
        public const string OrderByAsc = "ASC";
        public const string CollectionTotalPages = "X-Collection-Total";
        public const string CollectionSkip = "X-Collection-Skip";
        public const string CollectionTake = "X-Collection-Take";
        public const string FolderRoot = "SignOffice";
        public const string FileSign = "Sign.bmp";
        public const string FolderRelease = "Release";
        public const string FolderSignFile = "Sign";
        public const string FolderAsset = "Asset";
    }

    public enum ResultSignInvoice
    {
        Sucessfull = 1,
        SignError,
        PrintError,
    }

    public static class MessageError
    {
        public const string SysErrorMessages = "Lỗi trong quá trình khởi tạo hệ thống, vui lòng liên hệ với nhà cung cấp dịch vụ";
        public const string ErrorMessagesProcessData = "Lỗi trong quá trình kết nối với hệ thống máy chủ, vui lòng liên hệ với nhà cung cấp dịch vụ";
        public const string ErrorMessagesNotFoundHardwer = "Không tìm thấy thiết bị chữ ký số, vui lòng kiểm tra thiết bị";
        public const string ErrorMessagesNotFoundInvoice = "Không có dữ liệu phát hành hóa đơn";
        public const string ErrorMessagesNotFoundSerialNumber = "Chữ ký số không hợp lệ với chứ số đã đăng ký với hệ thống vui lòng kiểm tra lại";
    }

}
