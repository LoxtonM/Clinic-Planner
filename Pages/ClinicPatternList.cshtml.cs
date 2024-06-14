using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CPTest.Document;

namespace CPTest.Pages
{
    public class ClinicPatternListModel : PageModel
    {
        private readonly DataContext _context;        
        private readonly IStaffData _staffData;
        private readonly IPatternData _patternData;
        private readonly DocumentController _doc;
        public ClinicPatternListModel(DataContext context)
        {
            _context = context;
            _staffData = new StaffData(_context);
            _patternData = new PatternData(_context);
            _doc = new DocumentController(_context);
        }

        public StaffMember clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public void OnGet(string staffCode)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinician = _staffData.GetStaffDetails(staffCode);
                patternList = _patternData.GetPatternList(staffCode);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        
    }
}
