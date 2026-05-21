using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Models;
using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;


namespace CPTest.Pages
{
    public class ListViewModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IApptsAndSlotsDataAsync _appSlotData;
        private readonly IWaitingListDataAsync _waitingListData;        
        private readonly ICliniciansClinicDataAsync _cliniciansClinicData;
        private readonly INotificationDataAsync _note;
        private readonly IAuditSqlServices _audit;
        private readonly INationalHolidayDataAsync _hols;
        private readonly IVersionData _versionData;

        public ListViewModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);            
            _waitingListData = new WaitingListDataAsync(_context);
            _appSlotData = new ApptsAndSlotsDataAsync(_cpxContext);
            _cliniciansClinicData = new CliniciansClinicDataAsync(_cpxContext);
            _versionData = new VersionData();
            _note = new NotificationDataAsync(_context);
            _hols = new NationalHolidayDataAsync(_cpxContext);
            _audit = new AuditSqlServices(config);
        }

        public List<Outcome> outcomes { get; set; }
        public List<WaitingList> waitingList { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        public StaffMember? staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public List<ClinicSlot> clinicSlotList { get; set; }
        public List<ClinicSlot> openSlotList { get; set; }
        public List<Patient> patientList { get; set; }
        public List<ApptsAndSlots> appointmentList { get; set; }        

        public string appVersion { get; set; }
        public string dllVersion { get; set; }
        public string notificationMessage { get; set; }
        public bool isLive { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
       

        //public string wcDateString = new string("");
        public DateTime theDate;
        public string clinician = new string("");
        public string clinic = new string("");
        public string userStaffCode { get; set; }

        public async Task OnGet(DateTime date, string clinician, string clinic, string searchTerm, string? message = "", bool? isSuccess = false)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    //userStaffCode = _staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE;
                    userStaffCode = await _staffData.GetStaffCode(User.Identity.Name);                    
                    notificationMessage = await _note.GetMessage("ClinicPlannerOutage");
                    isLive = bool.Parse(_config.GetValue("IsLive", ""));
                    await ClinicListViewSetup(date, clinician, clinic, searchTerm);
                    IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                    _audit.CreateAudit(userStaffCode, "Main Form", "", _ip.GetIPAddress());
                    appVersion = _config.GetValue("AppVersion", "");
                    dllVersion = _versionData.GetDLLVersion();

                    string TheDay = date.DayOfWeek.ToString();
                    this.clinician = clinician;
                    this.clinic = clinic;

                    if (message != "")
                    {
                        this.message = message;
                        this.success = isSuccess.GetValueOrDefault();
                    }                    
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        private async Task ClinicListViewSetup(DateTime date, string clinician, string clinic, string searchTerm)
        {
            try
            {
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();
                userStaffCode = await _staffData.GetStaffCode(User.Identity.Name);
                appVersion = _config.GetValue("AppVersion", "");

                if (date.ToString() != "01/01/0001 00:00:00")
                {
                    theDate = date;
                }
                else
                {
                    theDate = DateTime.Now;
                }

                var apptList = await _appSlotData.GetApptsAndSlotsList();

                if (searchTerm != null) //to search the waiting list for a CGU number
                {
                    waitingList = await _waitingListData.GetWaitingListByCGUNo(searchTerm);
                }
                else
                {
                    waitingList = await _waitingListData.GetWaitingList(clinician, clinic);
                }


                if (clinician != null)
                {
                    staffMember = await _staffData.GetStaffMemberDetails(clinician);
                    apptList = apptList.Where(a => a.ClinicianCode == clinician).ToList();

                    var Clinics = new List<CliniciansClinics>(); //because there are nulls
                    Clinics = await _cliniciansClinicData.GetCliniciansClinics(clinician);

                    clinicVenueList = clinicVenueList.Where(v => Clinics.Any(c => v.FACILITY == c.FACILITY)).ToList();
                }

                if (clinic != null)
                {
                    clinicVenue = await _clinicVenueData.GetVenueDetails(clinic);
                    apptList = apptList.Where(a => a.FacilityCode == clinic).ToList();
                }               

                appointmentList = apptList.Where(a => a.Date >= theDate).OrderBy(a => a.Time).OrderBy(a => a.Date).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
