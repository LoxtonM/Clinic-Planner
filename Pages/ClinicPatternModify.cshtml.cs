using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;

namespace CPTest.Pages
{
    public class ClinicPatternModifyModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IPatternDataAsync _patternData;
        private readonly IClinicPatternSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public ClinicPatternModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;    
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _patternData = new PatternDataAsync(_cpxContext);
            _ss = new ClinicPatternSqlServices(_context, _cpxContext, _config);
            _audit = new AuditSqlServices(_config);
        }

        public ClinicPattern pattern { get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }

        public async Task OnGet(int id)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                pattern = await _patternData.GetPatternDetails(id);
                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(pattern.StaffID);
                venue = await _clinicVenueData.GetVenueDetails(pattern.Clinic);
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Clinic Pattern Modify", "", _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }        
        }

        public async Task OnPost(int id, int day, int week, string months, int dur,
            int startHr, int startMin, int numSlots, DateTime dStart, DateTime? dEnd)
        {
            try
            {
                pattern = await _patternData.GetPatternDetails(id);
                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(pattern.StaffID);
                venue = await _clinicVenueData.GetVenueDetails(pattern.Clinic);
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();

                if (months == null)
                {
                    months = "123456789abc";
                }
                string username = User.Identity.Name;

                _ss.UpdateClinicPattern(id, pattern.StaffID, pattern.Clinic, day, week, months, numSlots, dur, startHr, startMin,  dStart, dEnd, username);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
