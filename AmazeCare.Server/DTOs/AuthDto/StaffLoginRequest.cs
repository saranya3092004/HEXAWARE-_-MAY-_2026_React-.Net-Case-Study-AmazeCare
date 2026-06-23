namespace AmazeCare.Server.Modules.Auth.DTOs
{
    public class StaffLoginRequest
    {
        public string StaffCode { get; set; } = string.Empty;   
        public string Password { get; set; } = string.Empty;
    }
}
