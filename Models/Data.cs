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
        public string? ClinicianName { get; set; }
        public string? ClinicID { get; set; }
        public string? ClinicName { get; set; }
        public string? CGU_No { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? Duration { get; set; }
        public int PriorityLevel { get; set; }
        public string Priority { get; set; }
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
        public int StartMin { get; set; }
        public string Clinician { get; set; }
        public string Facility { get; set; }
    }

    [Table("CLIN_FACILITIES", Schema = "dbo")]
    public class ClinicVenue
    {
        [Key]
        public string FACILITY { get; set; }
        public string? NAME { get; set; }
        public string? LOCATION { get; set; }
        public string? NOTES { get; set; }
        public Int16 NON_ACTIVE { get; set; }
        public bool HasQRCode { get; set; }
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
    public class StaffMember
    {
        [Key]
        public string STAFF_CODE { get; set; }
        public string? EMPLOYEE_NUMBER { get; set; }
        public string? NAME { get; set; }
        public string CLINIC_SCHEDULER_GROUPS { get; set; }
        public bool InPost { get; set; }
        public bool Clinical { get; set; }
        public string POSITION { get; set; }

    }

    //Again, view used here so we can have CGU_No in the front end
    [Table("ViewPatientAppointmentDetails", Schema = "dbo")]
    public class Appointment
    {
        [Key]
        public int RefID { get; set; }
        public int MPI { get; set; }
        public string? CGU_No { get; set; }
        public string? FamilyName { get; set; }
        public string FamilyNumber { get; set; }
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
        public string? Clinician2 { get; set; }
        public string? Clinician3 { get; set; }
        public string? Clinic { get; set; }
        public int ReferralRefID { get; set; }
        public string? LetterPrintedDate { get; set; }
        public string? PrimaryLanguage { get; set; }
        public bool? IsInterpreterReqd { get; set; }
        public bool ActiveAlerts { get; set; }
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
        public string? ReferringClinician { get; set; }
        public string? ReferrerCode { get; set; }
        public string? ReferringFacility { get; set; }
        public bool logicaldelete { get; set; }
        public string? LeadClinician { get; set; }
        public string? GC { get; set; }
    }

    [Table("ViewPatientDemographicDetails", Schema = "dbo")]
    public class Patient
    {
        [Key]
        public int MPI { get; set; }
        public int INTID { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? PtAKA { get; set; }
        public string? CGU_No { get; set; }
        public string PEDNO { get; set; }
        public string PtLetterAddressee { get; set; }
        public string? SALUTATION { get; set; }
        public string ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? ADDRESS3 { get; set; }
        public string? ADDRESS4 { get; set; }
        public string POSTCODE { get; set; }
        public string SOCIAL_SECURITY { get; set; }
        public DateTime DOB { get; set; }
        public string PrimaryLanguage { get; set; }
        public string IsInterpreterReqd { get; set; }
        public string? Ethnic { get; set; }
        public string? GP { get; set; }
        public string? GP_CODE { get; set; }
        public string? GP_Facility { get; set; }
        public string? GP_Facility_Code { get; set; }
    }

    [Table("MasterClinicianTable", Schema = "dbo")]
    public class ExternalClinician
    {
        [Key]
        public string MasterClinicianCode { get; set; }
        public string? TITLE { get; set; }
        public string? FIRST_NAME { get; set; }
        public string? NAME { get; set; }
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

    [Table("Alerts", Schema = "dbo")]
    public class Alerts
    {
        [Key]
        public int AlertID { get; set; }
        public int MPI {  get; set; }
        public string AlertType { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string? Comments { get; set; }
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

    [Table("CLIN_CLASS", Schema = "dbo")]
    public class Priority
    {
        [Key]
        public int PriorityLevel { get; set; }
        public string CLASS { get; set; }
        public string DESCRIPTION { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Notifications", Schema = "dbo")]
    public class Notifications
    {
        [Key]
        public int ID { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Constants", Schema = "dbo")]
    public class Constants
    {
        [Key]
        public string ConstantCode { get; set; }
        public string ConstantValue { get; set; }
    }

    [Table("ListDocumentsContent", Schema = "wmfacs_user")]
    public class DocumentsContent
    {
        [Key]
        public int DocContentID { get; set; }
        public string DocCode { get; set; }
        public string? Para1 { get; set; }
        public string? Para2 { get; set; }
        public string? Para3 { get; set; }
        public string? Para4 { get; set; }
        public string? Para5 { get; set; }
        public string? Para6 { get; set; }
        public string? Para7 { get; set; }
        public string? Para8 { get; set; }
        public string? Para9 { get; set; }
        public string? Para11 { get; set; }
        public string? Para12 { get; set; }
        public string? Para13 { get; set; }
    }

}


