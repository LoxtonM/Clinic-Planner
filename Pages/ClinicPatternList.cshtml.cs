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
        private DataConnections dc;

        public ClinicPatternListModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
        }

        public string clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public void OnGet(string sClinician)
        {
            try
            {
                clinician = dc.GetStaffDetails(sClinician).NAME;
                patternList = dc.GetPatternList(sClinician);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
