using Microsoft.EntityFrameworkCore;
using CPTest.Models;

namespace CPTest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Outcome> Outcomes { get; set; }
        public DbSet<WaitingList> WaitingList { get; set; }
        public DbSet<ClinicSlot> ClinicSlots { get; set; }
        public DbSet<ClinicVenue> ClinicVenues { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<AppType> AppType { get; set; }
        public DbSet<CliniciansClinics> CliniciansClinics { get; set; }
        public DbSet<ClinicPattern> ClinicPattern { get; set; }
        public DbSet<ClinicsAdded> ClinicsAdded { get; set; }
        public DbSet<DateList> DateList { get; set; }
        public DbSet<NationalHolidays> NationalHolidays { get; set; }
        public DbSet<Priority> Priority { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Constants> Constants { get; set; }
        public DbSet<DocumentsContent> DocumentsContent { get; set; }
        public DbSet<ExternalClinician> ExternalClinician { get; set; }
        public DbSet<ClinicDetails> ClinicDetails { get; set; }
        public DbSet<Alerts> Alerts { get; set; }
    }
}
