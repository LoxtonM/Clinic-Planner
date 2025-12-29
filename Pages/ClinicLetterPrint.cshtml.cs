using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using CPTest.Connections;
using CPTest.Data;
using CPTest.Document;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicLetterPrintModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IDocumentController _doc;        
        private readonly IConfiguration _config;
        private readonly IClinicLetterSqlServices _letter;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IAuditSqlServices _audit;
        public ClinicLetterPrintModel(ClinicalContext context, CPXContext cpxContext, DocumentContext documentContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _documentContext = documentContext;
            _config = config;
            _doc = new DocumentController(_context, _cpxContext, _documentContext);
            _staffData = new StaffUserDataAsync(_context);
            _letter = new ClinicLetterSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }
        public async Task OnGet(int refID, bool isEmailOnly)
        {
            try
            {
                if (_doc.ClinicLetter(refID, User.Identity.Name, isEmailOnly) == 1)
                {
                    _letter.UpdateClinicLetter(refID, User.Identity.Name);
                    IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                    _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Clinic Letter Print", "RefID=" + refID.ToString(), _ip.GetIPAddress());

                    if (isEmailOnly)
                    {
                        Response.Redirect(@Url.Content($"~/letter-{User.Identity.Name}.pdf"));
                    }
                    else
                    {
                        string message = "Letter successfully sent to Synertec";
                        Response.Redirect("ClinicLettersAndLists?refID=" + refID + "&sMessage=" + message + "&isSuccess=true");
                    }
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
