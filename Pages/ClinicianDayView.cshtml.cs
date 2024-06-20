using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicianDayViewModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAppointmentData _appointmentData;
        private readonly IClinicSlotData _slotData;
        private readonly ICliniciansClinicData _cliniciansClinicData;
        private readonly IAuditSqlServices _audit;

        public ClinicianDayViewModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _appointmentData = new AppointmentData(_context);
            _slotData = new ClinicSlotData(_context);
            _audit = new AuditSqlServices(_config);
        }

        public IEnumerable<Outcome> outcomes { get; set; }
        //public IEnumerable<WaitingList> waitingList { get; set; }
        //public ClinicVenue? clinicVenue { get; set; }
        //public StaffMember? staffMember { get; set; }
        //public IEnumerable<ClinicSlot> clinicSlotList { get; set; }
        //public IEnumerable<ClinicSlot> openSlotList { get; set; }
        //public IEnumerable<Patient> patientList { get; set; }
        public IEnumerable<Appointment?> appointmentList { get; set; }

        public DateTime[] TimeArray = new DateTime[120];
        public string[] ClinicianArray;

        public DateTime dDate;
        public string clinician = new string("");
        public string clinic = new string("");

        public void OnGet(DateTime dClinicDate)
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

                appointmentList = _appointmentData.GetAppointmentsForADay(dClinicDate);

                List<string> clinicianList = new List<string>();

                foreach (var item in appointmentList)
                {
                    clinicianList.Add(item.STAFF_CODE_1);                    
                }

                //clinicSlotList = _slotData.GetDaySlots(dClinicDate);

                //foreach (var item in clinicSlotList)
                //{
                //    clinicianList.Add(item.ClinicianID);
                //}

                clinicianList = clinicianList.Distinct().ToList();
                ClinicianArray = clinicianList.ToArray();

                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinician Day View", "");
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
