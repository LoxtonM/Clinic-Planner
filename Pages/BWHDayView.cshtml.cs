using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using System.Threading.Tasks;

namespace CPTest.Pages
{
    public class BWHDayViewModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IAppointmentDataAsync _appointmentData;
        private readonly ICliniciansClinicDataAsync _cliniciansClinicData;
        private readonly IAuditSqlServices _audit;

        public BWHDayViewModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _appointmentData = new AppointmentDataAsync(_context);
            _cliniciansClinicData = new CliniciansClinicDataAsync(_cpxContext);
            _audit = new AuditSqlServices(_config);
        }

        public IEnumerable<Outcome> outcomes { get; set; }
        public IEnumerable<WaitingList> waitingList { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        public StaffMember? staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public IEnumerable<ClinicSlot> clinicSlotList { get; set; }
        public IEnumerable<ClinicSlot> openSlotList { get; set; }
        public IEnumerable<Patient> patientList { get; set; }
        public IEnumerable<Appointment?> appointmentList { get; set; }

        public DateTime[] TimeArray = new DateTime[120];
        public string[] ClinicArray;

        //public string wcDateString = new string("");
        public DateTime dDate;       

        public async Task OnGet(DateTime dClinicDate)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                
                if (dClinicDate.ToString() == "01/01/0001 00:00:00")
                {
                    dClinicDate = DateTime.Today;
                }

                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();

                dDate = dClinicDate;
               
                DateTime initTime = dClinicDate.Add(new TimeSpan(8, 0, 0));

                for (int i = 0; i < 120; i++)
                {
                    TimeArray[i] = initTime.AddMinutes(i * 5);
                }

                appointmentList = await _appointmentData.GetAppointmentsForBWH(dClinicDate);

                //ClinicArray = new string[appointmentList.Count()];
                List<string> clinicList = new List<string>();

                foreach (var item in appointmentList)
                {
                    //ClinicArray.Append(item.FACILITY);
                    clinicList.Add(item.FACILITY);
                }                

                clinicList = clinicList.Distinct().ToList();
                ClinicArray = clinicList.ToArray();

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "BWH Day View", "", _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(DateTime dClinicDate)
        {

        }
    }
}
