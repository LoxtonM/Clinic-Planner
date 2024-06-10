using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public ClinicSlotModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _slotData = new ClinicSlotData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _ss = new ClinicSlotSqlServices(_config);
        }

        public StaffMember staffMember { get; set; }
        public ClinicSlot slot { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public int slotID { get; set; }

        public void OnGet(string sSlotID)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                slotID = Int32.Parse(sSlotID);

                slot = _slotData.GetSlotDetails(slotID);
                clinicVenue = _clinicVenueData.GetVenueDetails(slot.ClinicID);
                staffMember = _staffData.GetStaffDetails(slot.ClinicianID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int slotID, string sAction, string? detail = "", bool? isApplyClinic=false)
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
                        slotsForDay = _slotData.GetDaySlots(slot.ClinicianID, slot.ClinicID, slot.SlotDate).ToList();
                        foreach(var slot in slotsForDay)
                        {
                            _ss.ModifyClinicSlot(slot.SlotID, staffCode, sAction, detail);
                        }
                    }
                    else
                    {
                        _ss.ModifyClinicSlot(slotID, staffCode, sAction, detail);
                    }
                }

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
