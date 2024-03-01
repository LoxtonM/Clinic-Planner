using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AdHocClinicListModel : PageModel
    {
        private readonly DataContext _context;
        DataConnections dc;
        public AdHocClinicListModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
        }
        
        public string clinician;
        public List<ClinicsAdded> adHocList { get; set; }
        public void OnGet(string sClinician)
        {
            try
            {
                clinician = dc.GetStaffDetails(sClinician).NAME;
                adHocList = dc.GetAdHocList(sClinician).Where(c => c.ClinicDate >= DateTime.Now.Date).OrderByDescending(c => c.ClinicDate).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
    }
}
