namespace MS_Application.Constants;

public static class OtpPurpose
{
    public const string ResetPassword = "RESET_PASSWORD";
    public const string VerifyEmail = "VERIFY_EMAIL";
    public const string ChangeEmail = "CHANGE_EMAIL";

    private static readonly string[] All =
    {
        ResetPassword,
        VerifyEmail,
        ChangeEmail
    };

    public static string Normalize(string? purpose)
        => string.IsNullOrWhiteSpace(purpose)
            ? string.Empty
            : purpose.Trim().ToUpper();

    public static bool IsValid(string purpose)
        => All.Contains(purpose);

    public static string GetEmailSubject(string purpose) => purpose switch
    {
        ResetPassword => "Mã xác nhận đặt lại mật khẩu - MusicEmolite",
        VerifyEmail => "Mã xác nhận email - MusicEmolite",
        ChangeEmail => "Mã xác nhận đổi email - MusicEmolite",
        _ => "Mã xác nhận OTP - MusicEmolite"
    };

    public static string GetEmailBody(string purpose, string code, int expiryMinutes)
    {
        var action = purpose switch
        {
            ResetPassword => "đặt lại mật khẩu",
            VerifyEmail => "xác thực email",
            ChangeEmail => "đổi email",
            _ => "xác thực"
        };

        return $@"
            <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto;padding:24px;"">
                <h2 style=""color:#7c3aed;"">MusicEmolite</h2>
                <p>Mã OTP để {action} của bạn là:</p>
                <p style=""font-size:32px;font-weight:bold;letter-spacing:6px;color:#111;"">{code}</p>
                <p>Mã có hiệu lực trong {expiryMinutes} phút. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                <p style=""color:#888;font-size:12px;"">Nếu bạn không yêu cầu mã này, hãy bỏ qua email này.</p>
            </div>";
    }
}
