using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicPatternModel : PageModel
    {
        private readonly DataContext _context;
        private DataConnections dc;

        public ClinicPatternModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
        }

        public string clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public void OnGet(string sClinician)
        {
            clinician = dc.GetStaffDetails(sClinician).NAME;
            patternList = dc.GetPatternList(sClinician);
        }
    }
}
