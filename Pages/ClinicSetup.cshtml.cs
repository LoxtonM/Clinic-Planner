using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicSetupModel : PageModel
    {

        private readonly DataContext _context;

        public ClinicSetupModel(DataContext context)
        {
            _context = context;
        }
        public List<StaffMember> StaffMembers { get; set; }
        public ClinicVenue ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }

        
        public void OnGet(string sRefID)
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
