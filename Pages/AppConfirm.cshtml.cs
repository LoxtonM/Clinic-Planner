using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace CPTest.Pages
{
    public class AppConfirmModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IReferralData _referralData;
        private readonly IAppTypeData _appTypeData;
        private readonly IClinicSlotData _clinicSlotData;
        private readonly IAppointmentSqlServices _ss;

        public AppConfirmModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;            
            _ss = new AppointmentSqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _referralData = new ReferralData(_context);
            _appTypeData = new AppTypeData(_context);
            _clinicSlotData = new ClinicSlotData(_context);
        }

        public Patient? patient { get; set; }
        public StaffMember? staffMember { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<Referral> linkedRefList { get; set; }
        public List<AppType> appTypeList { get; set; }

        public DateTime appDate;
        public DateTime appTime;
        public int appDur;
        public string? appDateString;
        public string? appTimeString;
        public string? appTypeDef;
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;
        
        //public void OnGet(string intIDString, string clin, string ven, string dat, string tim, string dur, string instructions)
        public void OnGet(string intIDString, string slotIDString, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                int intID = Int32.Parse(intIDString);
                int slotID = Int32.Parse(slotIDString);
                int mpi = 0;
                
                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                ClinicSlot slot = _clinicSlotData.GetSlotDetails(slotID);

                string clin = slot.ClinicianID;
                string ven = slot.ClinicID;

                patient = _patientData.GetPatientDetailsByIntID(intID);

                if (patient == null)
                {
                    Response.Redirect("PatientNotFound?intID=" + intID.ToString() + "&clinicianID=" + clin + "&clinicID=" + ven, true);
                }
                else
                {
                    mpi = patient.MPI;
                }

                appTypeList = _appTypeData.GetAppTypeList();
                staffMember = _staffData.GetStaffDetails(clin);

                clinicVenue = _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = _referralData.GetReferralsList(mpi);
                                
                appDate = slot.SlotDate;
                appTime = slot.SlotTime;
                appDur = slot.duration;

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

        public void OnPost(int mpi, int refID, string clin, string ven, DateTime dat, string tim, int dur, string instructions, string type, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                string staffCode;
                string username = User.Identity.Name;

                patient = _patientData.GetPatientDetails(mpi);
                appTypeList = _appTypeData.GetAppTypeList();
                staffMember = _staffData.GetStaffDetails(clin);
                staffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE; //placeholder - will replace when login screen available                
                
                clinicVenue = _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = _referralData.GetReferralsList(mpi);

                int success = _ss.CreateAppointment(dat, tim, clin, null, null, ven, refID, mpi, type, dur, staffCode, instructions);
                if (success == 0)
                {
                    Response.Redirect("Error?sError=Update failed");
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
