using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Models
{    

    [Table("ListCancellationReason", Schema = "dbo")]
    public class CancellationReason
    {
        [Key]
        public int ID { get; set; }
        public string Reason { get; set; }
    }    

    //This view only exists so that the Clinics menu can filter by Clinician.
    [Table("ViewCliniciansClinics", Schema = "dbo")]
    public class CliniciansClinics
    {
        [Key]
        public string FACILITY { get; set; }
        public string? NAME { get; set; }
        public string? STAFF_CODE { get; set; }
    }           
    
    //For the "list of appointment types" menu
    [Table("APPTYPE", Schema = "dbo")]
    public class AppType
    {
        [Key]
        public string APP_TYPE { get; set; }
        public bool ISREFERRAL { get; set; }
        public bool ISAPPT { get; set; }
        public Int16 NON_ACTIVE { get; set; }
    }

    [Table("ClinicPattern", Schema = "dbo")]
    public class ClinicPattern
    {
        [Key]
        public int PatternID { get; set; }
        public string? Clinic { get; set; }
        public string? StaffID { get; set; }
        public int DyOfWk { get; set; }
        public int WkOfMth { get; set; }
        public string? MthOfYr { get; set; }
        public int NumSlots { get; set; }
        public int StartHr { get; set; }
        public int StartMin { get; set; }
        public int Duration { get; set; }
        public int EndHr { get; set; }
        public int EndMin { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    [Table("ClinicsAdded", Schema = "dbo")]
    [Keyless]
    public class ClinicsAdded
    {
        public string ClinicianID { get; set; }
        public string ClinicID { get; set; }
        public DateTime ClinicDate { get; set; }
        public int Duration { get; set; }
        public int StartHr { get; set; }
        public int StartMin { get; set; }
        public int EndHr { get; set; }
        public int EndMin { get; set; }
        public int NumSlots { get; set; }
        public int ID { get; set; }
    }

    [Table("Clinicians_Clinics", Schema = "dbo")]
    public class ClinicDetails
    {
        [Key]
        public string Facility { get; set; }
        public string? Addressee { get; set; }
        public string? Position { get; set; }
        public string? A_Address { get; set; }
        public string? A_Town { get; set; }
        public string? A_PostCode { get; set; }
        public string? A_Salutation { get; set; }
        public string? Preamble { get; set; }
        public string? Postlude { get; set; }
        public string? Copies_To { get; set; }
        public string? ClinicSite { get; set; }
        public string? TelNo { get; set; }
        public string? Initials { get; set; }
        public string? Secretary { get; set; }
    }    

    [Table("ListDates", Schema = "dbo")]
    [Keyless]
    public class DateList
    {        
        public DateTime Dt { get; set; }
        public string WeekDay { get; set; }
        public Int64 NumberOfThisWeekDayInMonth { get; set; }
    }

    [Table("ListNationalHolidays", Schema = "dbo")]
    public class NationalHolidays
    {
        [Key]
        public int ID { get; set; }
        public DateTime HolidayDate { get; set; }
    }    
}


