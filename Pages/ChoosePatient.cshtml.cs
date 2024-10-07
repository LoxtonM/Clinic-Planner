using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ChoosePatientModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAuditSqlServices _audit;

        public ChoosePatientModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);
            _audit = new AuditSqlServices(_config);
        }

        public Patient patient { get; set; }

        public void OnGet(string? cguno)
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
                    patient = _patientData.GetPatientDetailsByCGUNo(cguno);
                }

                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Choose Patient", "CGU=" + cguno);
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
