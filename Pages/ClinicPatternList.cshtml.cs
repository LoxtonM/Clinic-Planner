using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CPTest.Document;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;

namespace CPTest.Pages
{
    public class ClinicPatternListModel : PageModel
    {
        private readonly ClinicalContext _context; 
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IStaffData _staffData;
        private readonly IPatternData _patternData;
        private readonly DocumentController _doc;
        public ClinicPatternListModel(ClinicalContext context, CPXContext cpxContext, DocumentContext documentContext)
        {
            _context = context;
            _cpxContext = cpxContext;
            _documentContext = documentContext;
            _staffData = new StaffData(_context);
            _patternData = new PatternData(_cpxContext);
            _doc = new DocumentController(_context, _cpxContext, _documentContext);
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
