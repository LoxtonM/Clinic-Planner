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
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IAdHocClinicDataAsync _adHocClinicData;
        private readonly IAdHocClinicSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public AdHocClinicModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _adHocClinicData = new AdHocClinicDataAsync(_cpxContext);
            _ss = new AdHocClinicSqlServices(_context, _cpxContext, _config);
            _audit = new AuditSqlServices(_config);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public async Task OnGet(int id)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                adhocclinic = await _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(adhocclinic.ClinicianID);
                venue = await _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Ad Hoc Clinic Modify", "", _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(int id, int duration, int startHr, int startMin, int numSlots, DateTime dClinicDate)
        {
            try
            {
                adhocclinic = await _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(adhocclinic.ClinicianID);
                venue = await _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);
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
