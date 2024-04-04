using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicPatternListModel : PageModel
    {
        private readonly DataContext _context;
        private readonly DataConnections _dc;

        public ClinicPatternListModel(DataContext context)
        {
            _context = context;
            _dc = new DataConnections(_context);
        }

        public string clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public void OnGet(string clinician)
        {
            try
            {
                clinician = _dc.GetStaffDetails(clinician).NAME;
                patternList = _dc.GetPatternList(clinician);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
