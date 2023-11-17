using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AddToWLModel : PageModel
    {
        private readonly DataContext _context;
        public IEnumerable<ClinicVenue> ClinicVenues { get; set; }        
        public IEnumerable<StaffMember> StaffMembers { get; set; }
        public Patient Patient { get; set; }

        public AddToWLModel(DataContext context)
        {
            _context = context;
        }
        
        public void OnGet(string sCGU)
        {
            ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
            StaffMembers = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);  
            
            if(sCGU != null)
            {
                Patient = _context.Patients.FirstOrDefault(p => p.CGU_No == sCGU);
            }
        }

        public void OnPost()
        {
            
        }
    }
}
