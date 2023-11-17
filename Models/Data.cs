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

    //[Table("Clinicians_WaitingList", Schema = "dbo")]
    //[Keyless]
    //public class WaitingList
    //{
    //    public int IntID { get; set; }
    //    public string? ClinicianID { get; set; }
    //    public string? ClinicID { get; set; }
    //    public DateTime? AddedDate { get; set; }
    //}

    [Table("ViewPatientWaitingListDetails", Schema = "dbo")]
    [Keyless]
    public class WaitingList
    {
        public int MPI { get; set; }
        public string? ClinicianID { get; set; }
        public string? ClinicID { get; set; }
        //public DateTime? AddedDate { get; set; }
        public string CGU_No { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? Duration { get; set; }
    }

    [Table("ClinicSlotsAll", Schema = "dbo")]
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
    }

    [Table("CLIN_FACILITIES", Schema = "dbo")]
    public class ClinicVenue
    {
        [Key]
        public string FACILITY { get; set; }
        public string? NAME { get; set; }
        public Int16 NON_ACTIVE { get; set; }
    }

    [Table("STAFF", Schema = "dbo")]
    public class  StaffMember
    {
        [Key]
        public string STAFF_CODE { get; set; }
        public string? NAME { get; set; }
        public string CLINIC_SCHEDULER_GROUPS { get; set; }
        public bool InPost { get; set; }
        public bool Clinical { get; set; }

    }

    [Table("MasterActivityTable", Schema = "dbo")]
    public class Appointment
    {
        [Key]
        public int RefID { get; set; }
        public int MPI { get; set; }
        public DateTime? BOOKED_DATE { get; set; }
        public DateTime? BOOKED_TIME { get; set; }
        public string? STAFF_CODE_1 { get; set; }
        public string? FACILITY { get; set; }
        public Int16? EST_DURATION_MINS { get; set; }
        public string? COUNSELED { get; set; }
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
}
