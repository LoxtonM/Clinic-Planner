using Microsoft.EntityFrameworkCore;
using CPTest.Models;

namespace CPTest.Data
{
    public class CPXContext : DbContext
    {
        public CPXContext(DbContextOptions<CPXContext> options) : base(options) { }        

        public DbSet<CancellationReason> CancellationReasons { get; set; }
                        
        public DbSet<AppType> AppType { get; set; }
        public DbSet<CliniciansClinics> CliniciansClinics { get; set; }
        public DbSet<ClinicPattern> ClinicPattern { get; set; }
        public DbSet<ClinicsAdded> ClinicsAdded { get; set; }
        public DbSet<DateList> DateList { get; set; }
        public DbSet<NationalHolidays> NationalHolidays { get; set; }        
        public DbSet<ClinicDetails> ClinicDetails { get; set; }        
    }
}
