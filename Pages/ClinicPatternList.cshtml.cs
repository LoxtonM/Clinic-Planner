using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CPTest.Document;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Meta;

namespace CPTest.Pages
{
    public class ClinicPatternListModel : PageModel
    {
        private readonly ClinicalContext _context; 
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IPatternDataAsync _patternData;
        private readonly DocumentController _doc;
        public ClinicPatternListModel(ClinicalContext context, CPXContext cpxContext, DocumentContext documentContext)
        {
            _context = context;
            _cpxContext = cpxContext;
            _documentContext = documentContext;
            _staffData = new StaffUserDataAsync(_context);
            _patternData = new PatternDataAsync(_cpxContext);
            _doc = new DocumentController(_context, _cpxContext, _documentContext);
        }

        public StaffMember clinician;
        public List<ClinicPattern> patternList {  get; set; }
        public async Task OnGet(string staffCode)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(staffCode);
                patternList = await _patternData.GetPatternList(staffCode);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        
    }
}
