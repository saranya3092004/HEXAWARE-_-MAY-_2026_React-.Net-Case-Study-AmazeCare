using Microsoft.EntityFrameworkCore;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class AmazeCareDBContext : DbContext // it is a class we are inheriting and we pass options from here to the parent constrictor so the EF Core will initialize the Connection to the db
    {
        public AmazeCareDBContext(DbContextOptions<AmazeCareDBContext> options) : base(options)
        {

        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Consultation> Consultations => Set<Consultation>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AmazeCareDBContext).Assembly);
            base.OnModelCreating(modelBuilder);

        }
    }
}
   
