using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicPatternListModel : PageModel
    {
        private readonly DataContext _context;        
        private readonly IStaffData _staffData;
        private readonly IPatternData _patternData;

        public ClinicPatternListModel(DataContext context)
        {
            _context = context;
            _staffData = new StaffData(_context);
            _patternData = new PatternData(_context);
        }

        public string clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public void OnGet(string clinician)
        {
            try
            {
                clinician = _staffData.GetStaffDetails(clinician).NAME;
                patternList = _patternData.GetPatternList(clinician);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
