using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Models
{
    [Table("CLIN_OUTCOMES", Schema = "dbo")]
    public class Outcome
    {
        [Key]
        public string CLINIC_OUTCOME { get; set; }
        public string DEFAULT_CLINIC_STATUS { get; set; }
    }

    //Views rather than tables are used so we can have useful data available in the front end
    [Table("ViewPatientWaitingListDetails", Schema = "dbo")]
    [Keyless]
    public class WaitingList
    {
        public int MPI { get; set; }
        public int IntID { get; set; }
        public string? ClinicianID { get; set; }
        public string? ClinicID { get; set; }
        //public DateTime? AddedDate { get; set; }
        public string? CGU_No { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? Duration { get; set; }
    }

    [Table("ViewClinicSlots", Schema = "dbo")]
    public class ClinicSlot
    {
        [Key]
        public int SlotID { get; set; }
        public string ClinicianID { get; set; }
        public string ClinicID { get; set; }
        public string SlotStatus { get; set; }
        public string? PedNum { get; set; }
        [DataType(DataType.Date)]
        public DateTime SlotDate { get; set; }
        [DataType(DataType.Time)]
        public DateTime SlotTime { get; set; }
        public int duration { get; set; }
        public int StartHr { get; set; }
        public int StartMin {  get; set; }
        public string Clinician {  get; set; }
        public string Facility { get; set; }
    }

    [Table("CLIN_FACILITIES", Schema = "dbo")]
    public class ClinicVenue
    {
        [Key]
        public string FACILITY { get; set; }
        public string? NAME { get; set; }
        public Int16 NON_ACTIVE { get; set; }
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

    [Table("STAFF", Schema = "dbo")]
    public class  StaffMember
    {
        [Key]
        public string STAFF_CODE { get; set; }
        public string? EMPLOYEE_NUMBER { get; set; }
        public string? NAME { get; set; }
        public string CLINIC_SCHEDULER_GROUPS { get; set; }
        public bool InPost { get; set; }
        public bool Clinical { get; set; }

    }

    //Again, view used here so we can have CGU_No in the front end
    [Table("ViewPatientAppointmentDetails", Schema = "dbo")]
    public class Appointment
    {
        [Key]
        public int RefID { get; set; }
        public int MPI { get; set; }
        public string? CGU_No { get; set; }
        public DateTime? BOOKED_DATE { get; set; }
        public DateTime? BOOKED_TIME { get; set; }
        public string? STAFF_CODE_1 { get; set; }
        public string? STAFF_CODE_2 { get; set; }
        public string? STAFF_CODE_3 { get; set; }
        public string? FACILITY { get; set; }
        //public Int16? EST_DURATION_MINS { get; set; }
        public Int16? Duration { get; set; }
        //public string? COUNSELED { get; set; }
        public string? Attendance { get; set; }
        //public string? TYPE { get; set; }
        public string? AppType { get; set; }
        public string? Clinician { get; set; }
        public string? Clinic {  get; set; }
    }

    [Table("ViewPatientReferralDetails", Schema = "dbo")]
    public class Referral
    {
        [Key]
        public int RefID { get; set; }
        public int MPI { get; set; }
        public string? RefType { get; set; }
        public DateTime RefDate { get; set; }
        public string? COMPLETE { get; set; }
        public string? ReferringClinician {  get; set; }
        public string? ReferringFacility { get; set; }
        public bool logicaldelete { get; set; }
    }

    [Table("MasterPatientTable", Schema = "dbo")]
    public class Patient
    {
        [Key]
        public int MPI { get; set; }
        public int INTID { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? CGU_No { get; set; }
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
        public string Clinic { get; set; }
        public string StaffID { get; set; }
        public int DyOfWk { get; set; }
        public int WkOfMth { get; set; }
        public string? MthOfYr { get; set; }
        public int NumSlots { get; set; }
        public int StartHr { get; set; }
        public int StartMin {  get; set; }
        public int Duration {  get; set; }
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
        public DateTime HoldayDate { get; set; }
    }

    [Table("Notifications")]
    public class Notifications
    {
        [Key]
        public int ID { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }
}


