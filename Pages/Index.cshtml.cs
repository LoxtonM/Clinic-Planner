using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Meta;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using ClinicalXPDataConnections.Data;


namespace CPTest.Pages
{
    public class IndexModel : PageModel
    {        
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;        
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IAppointmentDataAsync _appointmentData;
        private readonly IWaitingListDataAsync _waitingListData;
        private readonly IClinicSlotDataAsync _slotData;
        private readonly ICliniciansClinicDataAsync _cliniciansClinicData;
        private readonly INotificationDataAsync _note;
        private readonly IAuditSqlServices _audit;
        private readonly INationalHolidayDataAsync _hols;
        private readonly IVersionData _versionData;

        public IndexModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);            
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _appointmentData = new AppointmentDataAsync(_context);
            _waitingListData = new WaitingListDataAsync(_context);
            _slotData = new ClinicSlotDataAsync(_context);
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
        public List<Appointment?> appointmentList { get; set; }
        public List<NationalHolidays> holidays { get; set; }

        public string appVersion { get; set; }
        public string dllVersion { get; set; }
        public string notificationMessage { get; set; }
        public bool isLive { get; set; }
        public string message { get; set; }
        public bool success { get; set; }

        public DateTime[] DateArray = new DateTime[5];
        public DateTime[] TimeArray = new DateTime[120];

        //public string wcDateString = new string("");
        public DateTime wcDate;
        public string clinician = new string("");
        public string clinic = new string("");
        public string userStaffCode { get; set; }


        public async Task OnGet(DateTime wcDt, string clinician, string clinic, string searchTerm, string? message = "", bool? isSuccess = false)
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
                    holidays = await _hols.GetNationalHolidays();
                    notificationMessage = await _note.GetMessage("ClinicPlannerOutage");
                    isLive = bool.Parse(_config.GetValue("IsLive", ""));
                    await ClinicFormSetup(wcDt, clinician, clinic, searchTerm);
                    IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                    _audit.CreateAudit(userStaffCode, "Main Form", "", _ip.GetIPAddress());
                    appVersion = _config.GetValue("AppVersion", "");
                    dllVersion = _versionData.GetDLLVersion();

                    if(message != "")
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

        public async Task OnPost(DateTime wcDt, string clinician, string clinic, string searchTerm)
        {
            await ClinicFormSetup(wcDt, clinician, clinic, searchTerm);
        }

        private async Task ClinicFormSetup(DateTime wcDt, string clinician, string clinic, string searchTerm)
        {
            try
            {                
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();
                userStaffCode = await _staffData.GetStaffCode(User.Identity.Name);
                appVersion = _config.GetValue("AppVersion", "");

                if (wcDt.ToString() != "01/01/0001 00:00:00")
                {
                    wcDate = wcDt;
                }
                else
                {
                    wcDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                }
                
                string TheDay = wcDate.DayOfWeek.ToString();
                clinician = clinician;
                clinic = clinic;
                DateTime initTime = wcDate.Add(new TimeSpan(8, 0, 0));

                //DateArray is used by the JS to create the day/time slots
                for (int i = 0; i < 5; i++)
                {
                    DateArray[i] = wcDate.AddDays(i);
                }

                for (int i = 0; i < 120; i++)
                {
                    TimeArray[i] = initTime.AddMinutes(i * 5);
                }
                                
                appointmentList = await _appointmentData.GetAppointments(DateArray[0], DateArray[4], clinician, clinic);
                
                clinicSlotList = await _slotData.GetClinicSlots(DateArray[0], DateArray[4], clinician, clinic);

                if (searchTerm != null) //to search the waiting list for a CGU number
                {                    
                    waitingList = await _waitingListData.GetWaitingListByCGUNo(searchTerm);
                }
                else
                {
                    waitingList = await _waitingListData.GetWaitingList(clinician, clinic);
                }

                if (!clinic.IsNullOrEmpty())
                {
                    clinicVenue = await _clinicVenueData.GetVenueDetails(clinic);
                }

                if (!clinician.IsNullOrEmpty()) //if a clinician is selected as well
                {
                    staffMember = await _staffData.GetStaffMemberDetails(clinician);
                    var Clinics = new List<CliniciansClinics>(); //because there are nulls
                    Clinics = await _cliniciansClinicData.GetCliniciansClinics(clinician);

                    clinicVenueList = clinicVenueList.Where(v => Clinics.Any(c => v.FACILITY == c.FACILITY)).ToList();
                }

                //openSlots = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
                openSlotList = _slotData.GetOpenSlots(clinicSlotList); //won't go async
            }
            catch (Exception ex) 
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}