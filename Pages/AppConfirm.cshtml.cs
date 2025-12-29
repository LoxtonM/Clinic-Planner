using CPTest.Connections;
//using CPTest.Data;
//using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;

namespace CPTest.Pages
{
    public class AppConfirmModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IPatientDataAsync _patientData;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IReferralDataAsync _referralData;
        private readonly IActivityTypeDataAsync _appTypeData;
        private readonly IClinicSlotDataAsync _clinicSlotData;
        private readonly IWaitingListDataAsync _waitingListData;
        private readonly IAppointmentSqlServices _ss;

        public AppConfirmModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;            
            _ss = new AppointmentSqlServices(_config);
            _patientData = new PatientDataAsync(_context);
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _referralData = new ReferralDataAsync(_context);
            _appTypeData = new ActivityTypeDataAsync(_context);
            _waitingListData = new WaitingListDataAsync(_context);
            _clinicSlotData = new ClinicSlotDataAsync(_context);
        }

        public Patient? patient { get; set; }
        public StaffMember? staffMember { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<Referral> linkedRefList { get; set; }
        //public List<AppType> appTypeList { get; set; }
        public List<ActivityType> appTypeList { get; set; }

        public DateTime appDate;
        public DateTime appTime;
        public int appDur;
        public string? appDateString;
        public string? appTimeString;
        public string? appTypeDef;
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;
        public int wlID;
        public int slID;
        
        //public void OnGet(string intIDString, string clin, string ven, string dat, string tim, string dur, string instructions)
        public async Task OnGet(string intIDString, string slotIDString, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                //int intID = Int32.Parse(intIDString);
                int WLID = Int32.Parse(intIDString);
                WaitingList wl = await _waitingListData.GetWaitingListEntryByID(WLID);
                int intID = wl.IntID;
                int slotID = Int32.Parse(slotIDString);
                int mpi = 0;
                
                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                ClinicSlot slot = await _clinicSlotData.GetSlotDetails(slotID);

                string clin = slot.ClinicianID;
                string ven = slot.ClinicID;

                patient = await _patientData.GetPatientDetailsByIntID(intID);

                if (patient == null)
                {
                    Response.Redirect("PatientNotFound?intID=" + intID.ToString() + "&clinicianID=" + clin + "&clinicID=" + ven, true);
                }
                else
                {
                    mpi = patient.MPI;
                }

                appTypeList = await _appTypeData.GetApptTypes();
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(clin);

                clinicVenue = await _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = await _referralData.GetReferralsList(mpi);
                                
                appDate = slot.SlotDate;
                appTime = slot.SlotTime;
                appDur = slot.duration;
                wlID = WLID;
                slID = slotID;

                if (staffMember.CLINIC_SCHEDULER_GROUPS == "GC")
                {
                    appTypeDef = "GC Only Appt";
                }
                else
                {
                    appTypeDef = "Cons. Appt";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(int wlID, int mpi, int refID, string clin, string ven, DateTime dat, string tim, int dur, string instructions, string type, int slotID,
            string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                string staffCode;
                string username = User.Identity.Name;

                patient = await _patientData.GetPatientDetails(mpi);
                appTypeList = await _appTypeData.GetApptTypes();
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(clin);
                staffCode = await _staffData.GetStaffNameFromStaffCode(username);                
                
                clinicVenue = await _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = await _referralData.GetReferralsList(mpi);
                string message="";
                bool isSuccess = false;
                int success = _ss.CreateAppointment(dat, tim, clin, null, null, ven, refID, mpi, type, dur, staffCode, instructions, wlID, slotID);
                if (success == 0)
                {
                    message = "Update failed.";                    
                    //Response.Redirect("Index?message='Update failed'&isSuccess=false");
                }
                else
                {
                    message = "Appointment booked.";
                    isSuccess = true;
                }

                wcDateStr = HttpUtility.UrlEncode(wcDateString);
                clinicianSel = HttpUtility.UrlEncode(clinicianSelected);
                clinicSel = HttpUtility.UrlEncode(clinicSelected);

                string returnUrl = "Index?wcDt=" + wcDateStr;
                if (clinicianSel != null) { returnUrl = returnUrl + $"&clinician={clinicianSel}"; }
                if (clinicSel != null) { returnUrl = returnUrl + $"&clinic={clinicSel}"; }
                //if(!isSuccess) { 
                returnUrl = returnUrl + $"&isSuccess={isSuccess}&message={message}"; //}

                Response.Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
