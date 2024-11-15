using ClinicalXPDataConnections.Data;
using CPTest.Connections;
using CPTest.Data;
using CPTest.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CPTest.Pages
{
    public class ClinicListPrintModel : PageModel
    {
        private readonly DocumentController _doc;
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IAuditSqlServices _audit;

        public ClinicListPrintModel(ClinicalContext context, CPXContext cPXContext, DocumentContext documentContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cPXContext;
            _documentContext = documentContext;
            _config = config;
            _doc = new DocumentController(_context, _cpxContext, _documentContext);
            _staffData = new StaffData(_context);
            _audit = new AuditSqlServices(_config);
        }
        public void OnGet(int refID)
        {
            try
            {
                if (_doc.ClinicList(refID, User.Identity.Name) == 1)
                {
                    Response.Redirect(@Url.Content($"~/cliniclist-{User.Identity.Name}.pdf"));
                    _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic List Print", "RefID=" + refID.ToString());
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
