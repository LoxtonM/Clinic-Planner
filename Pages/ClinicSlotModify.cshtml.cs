using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace CPTest.Pages
{
    public class ClinicSlotModifyModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicSlotData _slotData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IClinicSlotSqlServices _ss;
        private readonly IAuditSqlServices _audit;

        public ClinicSlotModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _slotData = new ClinicSlotData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
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

        public void OnGet(string sSlotID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
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

                slot = _slotData.GetSlotDetails(slotID);
                clinicVenue = _clinicVenueData.GetVenueDetails(slot.ClinicID);
                staffMember = _staffData.GetStaffDetails(slot.ClinicianID);

                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic Slot Modify", "SlotID=" + sSlotID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int slotID, string sAction, string sSlotTime, string? wcDateString, string? clinicianSelected, string? clinicSelected, string? detail = "", 
            bool? isApplyClinic=false, string? comments = "")
        {
            try
            {
                slot = _slotData.GetSlotDetails(slotID);
                clinicVenue = _clinicVenueData.GetVenueDetails(slot.ClinicID);
                staffMember = _staffData.GetStaffDetails(slot.ClinicianID);
                string username = User.Identity.Name;
                string staffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;

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
                        slotsForDay = _slotData.GetDaySlots(slot.SlotDate, slot.ClinicianID, slot.ClinicID).ToList();
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
