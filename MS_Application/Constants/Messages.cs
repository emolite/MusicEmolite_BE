namespace MS_Application.Constants;

public static class Messages
{
    public static class Action
    {
        public const string CreateSuccess = "Add {0} success.";
        public const string CreateFail = "Add {0} fail.";
        public const string GetSuccess = "Get {0} success.";
        public const string GetFail = "Get {0} fail.";
        public const string UpdateSuccess = "Update {0} success.";
        public const string UpdateFail = "Update {0} fail.";
        public const string DeleteSuccess = "Delete {0} success.";
        public const string DeleteFail = "Delete {0} fail.";
        public const string UploadSuccess = "Upload {0} success";
        public const string UploadFail = "Upload {0} fail";
    }

    public static class Validation
    {
        public const string Required = "{0} là bắt buộc.";
        public const string Exists = "{0} already existed.";
        public const string ExistsLike = "{0} already liked";
        public const string NotFound = "{0} not found";
        public const string MaxLength = "Trường này không được vượt quá {1} ký tự.";
        public const string MinLength = "Trường này phải có ít nhất {1} ký tự.";
        public const string Email = "Địa chỉ email không hợp lệ.";
        public const string Range = "Giá trị phải nằm trong khoảng từ {1} đến {2}.";
        public const string InvalidModel = "Dữ liệu nhập chưa chính xác";
        public const string InvalidValue = "{0} không hợp lệ.";
        public const string ValidValue = "{0} valid.";
        public const string TimeRangeErrorMessage = "Khoảng thời gian {0} đến {1} bị trùng với khoảng {2} đến {3} trong danh mục {4}";
        public const string LimitDatetimeErrorMessage = "Thời gian hiệu lực phải lớn hơn hoặc bằng thời gian hiện tại";
        public const string StatusErrorMessage = "Không thể cập nhật {0}: Không ở trạng thái Bản nháp hoặc Đang hoạt động.";
        public const string NoneGreaterOrEqualMessage = "{0} lớn hơn hoặc bằng {1}";
        public const string NoneLessMessage = "{0} không được nhỏ hơn {1}";
        public const string NotAllowDeleteMessage = "Không được phép xóa {0}";
        public const string OnlyDeleteMessage = "Chỉ được xoá {0}";
        public const string StartTimeDuplicateErrorMessage = "Thời gian bắt đầu {0} cho danh mục {1} bị trùng lặp.";
        public const string GreaterThan = "{0} phải lớn hơn {1}";
        public const string LessThan = "{0} phải bé hơn {1}";
        public const string InValidHeader = "Định dạng tiêu đề không chính xác.";
        public const string InValidContent = "Nội dung file nhập không chính xác.";
        public const string InValidFile = "File nhập không chính xác.";
        public const string NotInApproval = "{0} không trong trạng thái chờ duyệt.";
    }

    public static class PasswordReset
    {
        public const string ResetWait60Second = "Bạn phải chờ ít nhất 60 giây trước khi gửi lại yêu cầu mới";
        public const string ResetWait1Hour = "Bạn đã vượt quá số lần yêu cầu trong 1 giờ. Vui lòng thử lại sau.";
        public const string PasswordResetSendSuccess = "Mã đặt lại mật khẩu đã được tạo thành công";
        public const string TooManyRequests = "Vượt quá giới hạn gửi 5 lần 1 giờ";
        public const string Success = "Mã đặt lại mật khẩu đã được tạo thành công";
        public const string UserNotFound = "Không tìm thấy tài khoản";
        public const string PassInvalid = "Mật khẩu tạm thời không hợp lệ";
        public const string PassExpired = "Mật khẩu tạm thời hết hạn";
        public const string LoginSuccess = "Đăng nhập thành công";
        public const string UserInvalid = "Tài khoản không hợp lệ";
        public const string NotFoundEmail = "Không tìm thấy tài khoản Email";
        public const string NotFoundAccount = "Không tìm thấy tài khoản";
    }

    public static class Maill
    {
        public const string SubjectPassReset = "Mã xác nhận đặt lại mật khẩu";
        public const string MessagePassReset = "Mật khẩu tạm thời của bạn là: {0}";
    }

    public static class Login
    {
        public const string AccountNotFound = "Sai tài khoản hoặc mật khẩu";
        public const string WrongPassword = "Sai tài khoản hoặc mật khẩu";
        public const string LoginSuccess = "Đăng nhập thành công";
    }

    public static class Logout
    {
        public const string AccountNotFound = "Không tìm thấy tài khoản";
        public const string LogoutSuccess = "Đăng xuất thành công";
    }

    public static class Register
    {
        public const string RegisterSuccess = "Đăng ký thành công";
        public const string ConfirmError = "Mật khẩu xác nhận không đúng";
        public const string UserNameExistsEn = "User exists with same username";
        public const string UserNameExistsVi = "Tên tài khoản đã được sử dụng";
    }

    public static class System
    {
        public const string Error500 = "Lỗi Hệ Thống";
        public const string UnknowMessage = "Có lỗi xảy ra , vui lòng thử lại";
    }
}
