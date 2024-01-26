using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AddToWLModel : PageModel
    {
        private readonly DataContext _context;
        public IEnumerable<ClinicVenue> clinicVenueList { get; set; }        
        public IEnumerable<StaffMember> staffMemberList { get; set; }
        public Patient Patient { get; set; }

        public AddToWLModel(DataContext context)
        {
            _context = context;
        }
        
        public void OnGet(string sCGU)
        {
            try
            {
                //ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
                clinicVenueList = _context.ClinicVenues.OrderBy(v => v.NAME);
                staffMemberList = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);

                if (sCGU != null)
                {
                    Patient = _context.Patients.FirstOrDefault(p => p.CGU_No == sCGU);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost()
        {
            try
            {
                //do stuff
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
