namespace AmazeCare.Server.DTOs.AdminDtos
{
    public class AdminDashboardResponse
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TodaysAppointments { get; set; }
        public int PendingAppointments { get; set; }
    }
}
