namespace MS_Application.DataTransferObjects.Auth
{
    public class VerifyOtpRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }
}
