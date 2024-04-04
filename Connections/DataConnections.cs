using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    public class DataConnections
    {
        private readonly DataContext _context;
        public DataConnections(DataContext context)
        {
            _context = context;
        }

        public Patient GetPatientDetails(int mpi)
        {
            var pt = _context.Patients.FirstOrDefault(p => p.MPI == mpi);
            return pt;
        }

        public StaffMember GetStaffDetails(string clin)
        {
            var staff = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == clin);
            return staff;
        }

        public StaffMember GetStaffDetailsByUsername(string username)
        {
            var staff = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == username);
            return staff;
        }

        public List<StaffMember> GetStaffMemberList() 
        {            
            var stafflist = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);
            return stafflist.ToList();
        }

        public ClinicVenue GetVenueDetails(string ven)
        {
            var clin = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == ven);
            return clin;
        }

        public List<ClinicVenue> GetVenueList() 
        {
            var venuelist = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
            return venuelist.ToList();
        }

        public List<Referral> GetReferralsList(int mpi) 
        {
            var refs = _context.Referrals.Where(r => r.MPI == mpi & r.logicaldelete == false & r.COMPLETE == "Active").OrderBy(r => r.RefDate).ToList();
            return refs;
        }

        public List<AppType> GetAppTypeList()
        {
            var at = _context.AppType.Where(t => t.NON_ACTIVE == 0 & t.ISAPPT == true).ToList();
            return at;
        }

        public List<Outcome> GetOutcomeList() 
        {
            var oc = _context.Outcomes.Where(o => o.DEFAULT_CLINIC_STATUS == "Active").ToList();
            return oc;
        }

        public Appointment GetAppointmentDetails(int refID)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.RefID == refID);
            return appt;
        }

        public IEnumerable<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {            
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE >= dFrom & 
                    a.BOOKED_DATE <= dTo & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional" 
                    & a.Attendance != "Cancelled by patient").ToList();

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician).ToList();
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic).ToList();
            }
            
            return appts;
        }

        public IEnumerable<Appointment> GetAppointmentsForADay(DateTime clinicDate, string? clinician, string? clinic)
        {
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE == clinicDate 
            & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional"
                    & a.Attendance != "Cancelled by patient").ToList();

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician).ToList();
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic).ToList();
            }
            
            return appts;
        }

        public IEnumerable<WaitingList> GetWaitingList(string? clinician, string? clinic)
        {
            var wl = _context.WaitingList.ToList();

            if (clinician != null)
            {
                wl = wl.Where(l => l.ClinicianID == clinician).ToList();
            }
            if (clinic != null)
            {
                wl = wl.Where(l => l.ClinicID == clinic).ToList();
            }            

            return wl.OrderBy(l => l.AddedDate);
        }

        public IEnumerable<WaitingList> GetWaitingListByCGUNo(string searchTerm)
        {
            var wl = _context.WaitingList.Where(w => w.CGU_No.Contains(searchTerm));                       
            return wl.OrderBy(l => l.AddedDate);
        }

        public IEnumerable<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {
            var slots = _context.ClinicSlots.Where(l => l.SlotDate >= dFrom & l.SlotDate <= dTo).ToList().OrderBy(l => l.SlotDate).ToList();
            
            if (clinician != null)
            {
                slots = slots.Where(s => s.ClinicianID == clinician).ToList();
            }
            if (clinic != null)
            {
                slots = slots.Where(s => s.ClinicID == clinic).ToList();
            }

            return slots;
        }

        public List<CliniciansClinics> GetCliniciansClinics(string clinician)
        {
            var clinics = _context.CliniciansClinics.Where(c => c.STAFF_CODE == clinician).ToList();
            return clinics;
        }

        public IEnumerable<ClinicSlot> GetOpenSlots(IEnumerable<ClinicSlot> clinicSlots)
        {
            var os  = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
            return os;
        }

        public ClinicPattern GetPatternDetails(int patID) 
        {
            var pat = _context.ClinicPattern.FirstOrDefault(p => p.PatternID == patID);
            return pat;
        }

        public List<ClinicPattern> GetPatternList(string clinID)
        {
            var patterns = _context.ClinicPattern.Where(p => p.StaffID == clinID).ToList();
            return patterns;
        }
        public ClinicsAdded GetAdHocClinicDetails(int id)
        {
            var adhoc = _context.ClinicsAdded.FirstOrDefault(p => p.ID == id);
            return adhoc;
        }
        public List<ClinicsAdded> GetAdHocList(string clinID) 
        {
            var adhocs = _context.ClinicsAdded.Where(c => c.ClinicianID == clinID).ToList();
            return adhocs;
        }

        public DateTime GetFirstDateFromList(DateTime dateToCheck, string day) 
        {
            DateTime firstDate = _context.DateList.FirstOrDefault(d => d.Dt.Month == dateToCheck.Month && d.Dt.Year == dateToCheck.Year
                                            && d.NumberOfThisWeekDayInMonth == 1 && d.WeekDay == day).Dt;

            return firstDate;
        }

        public bool GetIsNationalHolday(DateTime date)
        {
            int holdays = _context.NationalHoldays.Where(d => d.HoldayDate == date).Count();

            if(holdays > 0)
            {
                return true;
            }
            return false;
        }
    }
}
