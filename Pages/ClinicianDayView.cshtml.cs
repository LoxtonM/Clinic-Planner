using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class ClinicianDayViewModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IAppointmentDataAsync _appointmentData;
        private readonly IAuditSqlServices _audit;

        public ClinicianDayViewModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _appointmentData = new AppointmentDataAsync(_context);
            _audit = new AuditSqlServices(_config);
        }

        public IEnumerable<Outcome> outcomes { get; set; }
        
        public IEnumerable<Appointment?> appointmentList { get; set; }

        public DateTime[] TimeArray = new DateTime[120];
        public string[] ClinicianArray;

        public DateTime dDate;
        public string clinician = new string("");
        public string clinic = new string("");

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

                
                dDate = dClinicDate;

                clinician = clinician;
                clinic = clinic;
                DateTime initTime = dClinicDate.Add(new TimeSpan(8, 0, 0));

                for (int i = 0; i < 120; i++)
                {
                    TimeArray[i] = initTime.AddMinutes(i * 5);
                }

                appointmentList = await _appointmentData.GetAppointmentsForADay(dClinicDate);

                List<string> clinicianList = new List<string>();

                foreach (var item in appointmentList)
                {
                    clinicianList.Add(item.STAFF_CODE_1);                    
                }                

                clinicianList = clinicianList.Distinct().ToList();
                ClinicianArray = clinicianList.ToArray();

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Clinician Day View", "", _ip.GetIPAddress());
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
