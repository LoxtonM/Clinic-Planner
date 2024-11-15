using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;


namespace CPTest.Pages
{
    public class PastApptModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicSlotData _slotData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IReferralData _referralData;
        private readonly IAppTypeData _appTypeData;
        private readonly IOutcomeData _outcomeData;
        private readonly IClinicSlotSqlServices _ssSlot;
        private readonly IAppointmentSqlServices _ssAppt;
        private readonly IAuditSqlServices _audit;

        public PastApptModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context, _cpxContext);
            _slotData = new ClinicSlotData(_context);
            _referralData = new ReferralData(_context);
            _appTypeData = new AppTypeData(_cpxContext);
            _outcomeData = new OutcomeData(_context);
            _ssSlot = new ClinicSlotSqlServices(_config);
            _ssAppt = new AppointmentSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }

        public StaffMember staffMember { get; set; }
        public List<StaffMember> clinicianList { get; set; }
        public List<ClinicVenue> clinicList { get; set; }
        public List<Referral> referralList { get; set; }
        public List<AppType> appTypeList { get; set; }
        public List<Outcome> outcomeList { get; set; }

        //public string? wcDateStr;
        //public string? clinicianSel;
        //public string? clinicSel;

        public void OnGet(int mpi)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinicianList = _staffData.GetStaffMemberList();
                clinicList = _clinicVenueData.GetVenueList();
                appTypeList = _appTypeData.GetAppTypeList();
                referralList = _referralData.GetReferralsList(mpi);
                outcomeList = _outcomeData.GetOutcomeList();
                
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Create Past Appointment", "MPI=" + mpi);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string sSlotDate, string sSlotTime, string clinician, string clinic, int duration, int refID, string appType, 
            string? instructions, string outcome, string letterRequired, string arrivalTime, int patientsSeen, bool isClockStop)
        {
            try
            {                
                string username = User.Identity.Name;
                string staffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;
                
                int mpi = 0;
                DateTime dSlotDate = DateTime.Parse(sSlotDate);
                DateTime dSlotTime = DateTime.Parse("1899-12-30 " + sSlotTime);

                clinicianList = _staffData.GetStaffMemberList();
                clinicList = _clinicVenueData.GetVenueList();
                appTypeList = _appTypeData.GetAppTypeList();
                referralList = _referralData.GetReferralsList(mpi);
                outcomeList = _outcomeData.GetOutcomeList();

                mpi = _referralData.GetReferralDetails(refID).MPI;

                int success = _ssAppt.CreatePastAppointment(dSlotDate, sSlotTime, clinician, clinic, refID, mpi, appType, duration, staffCode,
                    outcome, isClockStop, letterRequired, patientsSeen, arrivalTime);

                if (success == 0)
                {
                    Response.Redirect("Error?sError=Update failed");
                }

                //string returnUrl = "Index?wcDt=" + wcDateStr;
                //if (clinicianSel != null) { returnUrl = returnUrl + $"&clinician={clinicianSel}"; }
                //if (clinicSel != null) { returnUrl = returnUrl + $"&clinic={clinicSel}"; }

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
