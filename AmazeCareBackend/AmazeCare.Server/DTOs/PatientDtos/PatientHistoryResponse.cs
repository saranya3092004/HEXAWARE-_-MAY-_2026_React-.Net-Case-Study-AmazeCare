using AmazeCare.Server.Modules.PatientModule.DTOs;

namespace AmazeCare.Server.Modules.PatientModule.DTOs
{
    public class PatientHistoryResponse
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public List<ConsultationSummary> Consultations { get; set; } = new();
    }

}