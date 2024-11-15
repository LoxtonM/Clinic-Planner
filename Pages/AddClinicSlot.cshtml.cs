using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class AddClinicSlotModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicSlotData _slotData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IClinicSlotSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public AddClinicSlotModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context, _cpxContext);
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



        public void OnGet(string? wcDateString, string? clinicianSelected, string? clinicSelected)
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
                clinicianList = _staffData.GetStaffMemberList();
                clinicList = _clinicVenueData.GetVenueList();

                if (clinicianSelected != null)
                {
                    clinician = _staffData.GetStaffDetails(clinicianSelected);
                }
                if (clinicSelected != null)
                {
                    clinic = _clinicVenueData.GetVenueDetails(clinicSelected);
                }


                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Add Clinic Slot", "");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string? wcDateString, string? clinicianSelected, string? clinicSelected, DateTime slotDate, string slotTime, int duration)
        {
            try
            {
                clinicianList = _staffData.GetStaffMemberList();
                clinicList = _clinicVenueData.GetVenueList();
                string username = User.Identity.Name;
                string staffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;
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
