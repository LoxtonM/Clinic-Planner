using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AdHocClinicListModel : PageModel
    {
        private readonly DataContext _context;
        private readonly DataConnections _dc;
        public AdHocClinicListModel(DataContext context)
        {
            _context = context;
            _dc = new DataConnections(_context);
        }
        
        public string clinician;
        public List<ClinicsAdded> adHocList { get; set; }
        public void OnGet(string clinician)
        {
            try
            {
                clinician = _dc.GetStaffDetails(clinician).NAME;
                adHocList = _dc.GetAdHocList(clinician).Where(c => c.ClinicDate >= DateTime.Now.Date).OrderByDescending(c => c.ClinicDate).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
    }
}
