using CPTest.Connections;
using CPTest.Data;
using CPTest.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicLetterPrintModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IDocumentController _doc;        
        private readonly IConfiguration _config;
        private readonly IClinicLetterSqlServices _letter;
        private readonly IStaffData _staffData;
        private readonly IAuditSqlServices _audit;
        public ClinicLetterPrintModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _doc = new DocumentController(_context);
            _staffData = new StaffData(_context);
            _letter = new ClinicLetterSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }
        public void OnGet(int refID, bool isEmailOnly)
        {
            try
            {
                if (_doc.ClinicLetter(refID) == 1)
                {
                    _letter.UpdateClinicLetter(refID, User.Identity.Name);
                    _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic Letter Print", "RefID=" + refID.ToString());
                    Response.Redirect(@Url.Content(@"~/letter.pdf"));
                }
                else
                {
                    string message = "Something went wrong and the letter didn't print for some reason.";
                    Response.Redirect("Error?sError=" + message);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
