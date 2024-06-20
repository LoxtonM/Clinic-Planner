using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicDayViewModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAppointmentData _appointmentData;
        private readonly ICliniciansClinicData _cliniciansClinicData;
        private readonly IAuditSqlServices _audit;

        public ClinicDayViewModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _appointmentData = new AppointmentData(_context);
            _cliniciansClinicData = new CliniciansClinicData(_context);            
            _audit = new AuditSqlServices(_config);
        }

        public IEnumerable<Outcome> outcomes { get; set; }
        public IEnumerable<WaitingList> waitingList { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public StaffMember? staffMember { get; set; }
        public IEnumerable<ClinicSlot> clinicSlotList { get; set; }
        public IEnumerable<ClinicSlot> openSlotList { get; set; }
        public IEnumerable<Patient> patientList { get; set; }
        public IEnumerable<Appointment?> appointmentList { get; set; }

        public DateTime[] TimeArray = new DateTime[120];
        public string[] ClinicArray;

        //public string wcDateString = new string("");
        public DateTime dDate;
        

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
                                
                DateTime initTime = dClinicDate.Add(new TimeSpan(8, 0, 0));

                for (int i = 0; i < 120; i++)
                {
                    TimeArray[i] = initTime.AddMinutes(i * 5);
                }

                appointmentList = _appointmentData.GetAppointmentsForADay(dClinicDate);

                //ClinicArray = new string[appointmentList.Count()];
                List<string> clinicList = new List<string>();

                foreach (var item in appointmentList)
                {
                    //ClinicArray.Append(item.FACILITY);
                    clinicList.Add(item.FACILITY);
                }
                //clinicSlotList = _dc.GetClinicSlots(dClinicDate, , clinician, clinic);

                clinicList = clinicList.Distinct().ToList();
                ClinicArray = clinicList.ToArray();


                //openSlots = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
                //openSlotList = _dc.GetOpenSlots(clinicSlotList);              
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic Day View", "");
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
