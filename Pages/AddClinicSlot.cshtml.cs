using CPTest.Connections;
//using CPTest.Data;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class AddClinicSlotModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        //private readonly IClinicSlotDataAsync _slotData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IClinicSlotSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public AddClinicSlotModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _ss = new ClinicSlotSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }

        public StaffMember staffMember { get; set; }
        public List<StaffMember> clinicianList { get; set; }
        public List<ClinicVenue> clinicList { get; set; }

        public StaffMember clinician {  get; set; }
        public ClinicVenue clinic { get; set; }
        public DateTime slotDate { get; set; }
        
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;



        public async Task OnGet(string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;
                clinicianList = await _staffData.GetClinicalStaffList();
                clinicList = await _clinicVenueData.GetVenueList();

                if (clinicianSelected != null)
                {
                    clinician = await _staffData.GetStaffMemberDetailsByStaffCode(clinicianSelected);
                }
                if (clinicSelected != null)
                {
                    clinic = await _clinicVenueData.GetVenueDetails(clinicSelected);
                }

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Add Clinic Slot", "", _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(string? wcDateString, string? clinicianSelected, string? clinicSelected, DateTime slotDate, string slotTime, int duration)
        {
            try
            {
                clinicianList = await _staffData.GetClinicalStaffList();
                clinicList = await _clinicVenueData.GetVenueList();                
                string staffCode = await _staffData.GetStaffCode(User.Identity.Name);
                int patternID = 0;
                DateTime dSlotTime = DateTime.Parse("1899-12-30 " + slotTime);

                _ss.CreateClinicSlot(slotDate, dSlotTime, clinicianSelected, clinicSelected, staffCode, duration, patternID);

                wcDateStr = HttpUtility.UrlEncode(wcDateString);
                clinicianSel = HttpUtility.UrlEncode(clinicianSelected);
                clinicSel = HttpUtility.UrlEncode(clinicSelected);

                string returnUrl = "Index?wcDt=" + wcDateStr;
                if (clinicianSel != null) { returnUrl = returnUrl + $"&clinician={clinicianSel}"; }
                if (clinicSel != null) { returnUrl = returnUrl + $"&clinic={clinicSel}"; }

                Response.Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
