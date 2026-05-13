using MS_Application.DataTransferObjects.Base;

namespace MS_Application.DataTransferObjects.User
{
    public class CrmUserResponseDto : BaseDto
    {
        public string? RefCode { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? RoleCode { get; set; }

    }
}
