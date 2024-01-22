using Microsoft.EntityFrameworkCore;
using CPTest.Models;
using System.Collections.Generic;

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
    }
}
