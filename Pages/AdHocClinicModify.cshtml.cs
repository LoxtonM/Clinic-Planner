using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Meta;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Data;


namespace CPTest.Pages
{
    public class AdHocClinicModifyModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAdHocClinicData _adHocClinicData;
        private readonly IAdHocClinicSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public AdHocClinicModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _adHocClinicData = new AdHocClinicData(_cpxContext);
            _ss = new AdHocClinicSqlServices(_context, _cpxContext, _config);
            _audit = new AuditSqlServices(_config);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public void OnGet(int id)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                adhocclinic = _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = _staffData.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Ad Hoc Clinic Modify", "", _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int id, int duration, int startHr, int startMin, int numSlots, DateTime dClinicDate)
        {
            try
            {
                adhocclinic = _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = _staffData.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);
                string username = User.Identity.Name;

                _ss.UpdateAdHocClinic(id, adhocclinic.ClinicianID, adhocclinic.ClinicID, duration, startHr, startMin, numSlots, dClinicDate, username);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
