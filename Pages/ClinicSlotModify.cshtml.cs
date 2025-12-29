using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;

namespace CPTest.Pages
{
    public class ClinicSlotModifyModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicSlotDataAsync _slotData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IClinicSlotSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public ClinicSlotModifyModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _slotData = new ClinicSlotDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _ss = new ClinicSlotSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }

        public StaffMember staffMember { get; set; }
        public ClinicSlot slot { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public int slotID { get; set; }
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public async Task OnGet(string sSlotID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                slotID = Int32.Parse(sSlotID);

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                slot = await _slotData.GetSlotDetails(slotID);
                clinicVenue = await _clinicVenueData.GetVenueDetails(slot.ClinicID);
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(slot.ClinicianID);

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Clinic Slot Modify", "SlotID=" + sSlotID, _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(int slotID, string sAction, string sSlotTime, string? wcDateString, string? clinicianSelected, string? clinicSelected, string? detail = "", 
            bool? isApplyClinic=false, string? comments = "")
        {
            try
            {
                slot = await _slotData.GetSlotDetails(slotID);
                clinicVenue = await _clinicVenueData.GetVenueDetails(slot.ClinicID);
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(slot.ClinicianID);
                string username = User.Identity.Name;
                string staffCode = await _staffData.GetStaffCode(username);

                if (sAction=="ForMeOnly")
                {                    
                    detail = staffCode;
                }

                if (sAction == "Delete")
                {
                    _ss.DeleteClinicSlot(slotID, staffCode);
                }
                else
                {
                    if (isApplyClinic.GetValueOrDefault())
                    {
                        List<ClinicSlot> slotsForDay = new List<ClinicSlot>();
                        slotsForDay = await _slotData.GetDaySlots(slot.SlotDate, slot.ClinicianID, slot.ClinicID);
                        foreach(var slot in slotsForDay)
                        {
                            _ss.ModifyClinicSlot(slot.SlotID, staffCode, sAction, detail, comments);
                        }
                    }
                    else
                    {
                        _ss.ModifyClinicSlot(slotID, staffCode, sAction, detail, comments);
                    }
                }

                if (sSlotTime != slot.SlotTime.ToString("HH:mm:ss.fff"))
                {
                    _ss.ChangeClinicSlotTime(slotID, sSlotTime);
                }

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
