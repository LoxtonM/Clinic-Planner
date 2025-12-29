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
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicSlotDataAsync _slotData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IReferralDataAsync _referralData;
        private readonly IActivityTypeDataAsync _appTypeData;
        private readonly IOutcomeDataAsync _outcomeData;
        private readonly IClinicSlotSqlServices _ssSlot;
        private readonly IAppointmentSqlServices _ssAppt;
        private readonly IAuditSqlServices _audit;

        public PastApptModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _slotData = new ClinicSlotDataAsync(_context);
            _referralData = new ReferralDataAsync(_context);
            _appTypeData = new ActivityTypeDataAsync(_context);
            _outcomeData = new OutcomeDataAsync(_context);
            _ssSlot = new ClinicSlotSqlServices(_config);
            _ssAppt = new AppointmentSqlServices(_config);
            _audit = new AuditSqlServices(_config);
        }

        public StaffMember staffMember { get; set; }
        public List<StaffMember> clinicianList { get; set; }
        public List<ClinicVenue> clinicList { get; set; }
        public List<Referral> referralList { get; set; }
        public List<ActivityType> appTypeList { get; set; }
        public List<Outcome> outcomeList { get; set; }
               

        public async Task OnGet(int mpi)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinicianList = await _staffData.GetClinicalStaffList();
                clinicList = await _clinicVenueData.GetVenueList();
                appTypeList = await _appTypeData.GetApptTypes();
                referralList = await _referralData.GetReferralsList(mpi);
                outcomeList = await _outcomeData.GetOutcomeList();

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Create Past Appointment", "MPI=" + mpi, _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(string sSlotDate, string sSlotTime, string clinician, string clinic, int duration, int refID, string appType, 
            string? instructions, string outcome, string letterRequired, string arrivalTime, int patientsSeen, bool isClockStop)
        {
            try
            {                
                string username = User.Identity.Name;
                string staffCode = await _staffData.GetStaffCode(username);
                
                int mpi = 0;
                DateTime dSlotDate = DateTime.Parse(sSlotDate);
                DateTime dSlotTime = DateTime.Parse("1899-12-30 " + sSlotTime);

                clinicianList = await _staffData.GetClinicalStaffList();
                clinicList = await _clinicVenueData.GetVenueList();
                appTypeList = await _appTypeData.GetApptTypes();
                referralList = await _referralData.GetReferralsList(mpi);
                outcomeList = await _outcomeData.GetOutcomeList();

                Referral refer = await _referralData.GetReferralDetails(refID);
                mpi = refer.MPI;

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
