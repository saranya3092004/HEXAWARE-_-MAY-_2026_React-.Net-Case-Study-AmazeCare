namespace AmazeCare.Server.Modules.DoctorModule.DTOs
{
    public class DoctorSearchRequest
    {
        public string? Name { get; set; }
        public string? Specialization { get; set; } = String.Empty;
    }
}
