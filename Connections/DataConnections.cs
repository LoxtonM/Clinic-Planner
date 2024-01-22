using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace CPTest.Connections
{
    public class DataConnections
    {
        private readonly DataContext _context;
        public DataConnections(DataContext context)
        {
            _context = context;
        }

        public Patient GetPatientDetails(int iMPI)
        {
            var pt = _context.Patients.FirstOrDefault(p => p.MPI == iMPI);

            return pt;
        }

        public StaffMember GetStaffDetails(string sClin)
        {
            var staff = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == sClin);

            return staff;
        }

        public StaffMember GetStaffDetailsByUsername(string sUsername)
        {
            var staff = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == sUsername);

            return staff;
        }

        public List<StaffMember> GetStaffMemberList() 
        {            
            var stafflist = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);

            return stafflist.ToList();
        }

        public ClinicVenue GetVenueDetails(string sVen)
        {
            var clin = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == sVen);

            return clin;
        }

        public List<ClinicVenue> GetVenueList() 
        {
            var venuelist = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);

            return venuelist.ToList();
        }

        public List<Referral> GetReferralsList(int iMPI) 
        {
            var refs = _context.Referrals.Where(r => r.MPI == iMPI & r.logicaldelete == false & r.COMPLETE == "Active").OrderBy(r => r.RefDate).ToList();

            return refs;
        }

        public List<AppType> GetAppTypeList()
        {
            var at = _context.AppType.Where(t => t.NON_ACTIVE == 0 & t.ISAPPT == true).ToList();

            return at;
        }

        public IEnumerable<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? strClinician, string? strClinic)
        {
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE >= dFrom & a.BOOKED_DATE <= dTo & a.COUNSELED != "Declined" & a.COUNSELED != "Cancelled by professional" & a.COUNSELED != "Cancelled by patient").ToList();

            if (strClinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == strClinician).ToList();
            }
            if (strClinic != null)
            {
                appts = appts.Where(l => l.FACILITY == strClinic).ToList();
            }

            return appts;
        }

        public IEnumerable<WaitingList> GetWaitingList(string? strClinician, string? strClinic)
        {
            var wl = _context.WaitingList.ToList();

            if (strClinician != null)
            {
                wl = wl.Where(l => l.ClinicianID == strClinician).ToList();
            }
            if (strClinic != null)
            {
                wl = wl.Where(l => l.ClinicID == strClinic).ToList();
            }            

            return wl.OrderBy(l => l.AddedDate);
        }

        public IEnumerable<WaitingList> GetWaitingListByCGUNo(string sSearchTerm)
        {
            var wl = _context.WaitingList.Where(w => w.CGU_No.Contains(sSearchTerm));                       

            return wl.OrderBy(l => l.AddedDate);
        }

        public IEnumerable<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? strClinician, string? strClinic)
        {
            var slots = _context.ClinicSlots.Where(l => l.SlotDate >= dFrom & l.SlotDate <= dTo).ToList().OrderBy(l => l.SlotDate).ToList();
            
            if (strClinician != null)
            {
                slots = slots.Where(s => s.ClinicianID == strClinician).ToList();
            }
            if (strClinic != null)
            {
                slots = slots.Where(s => s.ClinicID == strClinic).ToList();
            }

            return slots;
        }

        public IEnumerable<ClinicSlot> GetOpenSlots(IEnumerable<ClinicSlot> clinicSlots)
        {
            var os  = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");

            return os;
        }
    }
}
