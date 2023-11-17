using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class WLModifyModel : PageModel
    {

        private readonly DataContext _context;

        public WLModifyModel(DataContext context)
        {
            _context = context;
        }
        public Patient Patient { get; set; }
        public StaffMember StaffMember { get; set; }
        public List<StaffMember> StaffMembers { get; set; }
        public ClinicVenue ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }        

        
        public void OnGet(int iMPI, string sClinicID, string sClinicianID)
        {
            if(sClinicianID != null)
            {
                StaffMember = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == sClinicianID);
            }

            if(sClinicID != null)
            {
                ClinicVenue = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == sClinicID);
            }

            if(iMPI != null)
            {
                Patient = _context.Patients.FirstOrDefault(p => p.MPI == iMPI);
            }

            StaffMembers = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).ToList();

            ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).ToList();
        }        
    }
}
