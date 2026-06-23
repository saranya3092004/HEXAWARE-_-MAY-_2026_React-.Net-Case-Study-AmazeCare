namespace AmazeCare.Server.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public bool IsActive { get; set; } = true; 

        public ICollection<Doctor> DoctorSpecializations { get; set; } = new List<Doctor>();
    }
}
