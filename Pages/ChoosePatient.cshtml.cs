using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class ChoosePatientModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IPatientDataAsync _patientData;
        private readonly IAuditSqlServices _audit;

        public ChoosePatientModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _patientData = new PatientDataAsync(_context);
            _audit = new AuditSqlServices(_config);
        }

        public Patient patient { get; set; }
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public async Task OnGet(string? cguno)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                patient = new Patient();

                if(cguno != null)
                {
                    patient = await _patientData.GetPatientDetailsByCGUNo(cguno);
                }

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Choose Patient", "CGU=" + cguno, _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string? cguno)
        {
            try
            {
                patient = new Patient();

                if (cguno != null)
                {
                    Response.Redirect("ChoosePatient?cguno=" + cguno);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
