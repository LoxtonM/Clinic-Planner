using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;


namespace CPTest.Pages
{
    public class IndexModel : PageModel
    {        
        private readonly DataContext _context;
        private readonly DataConnections _dc;

        public IndexModel(DataContext context)
        {
            _context = context;
            _dc = new DataConnections(_context);
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

        public DateTime[] DateArray = new DateTime[5];
        public DateTime[] TimeArray = new DateTime[120];

        //public string wcDateString = new string("");
        public DateTime wcDate;
        public string clinician = new string("");
        public string clinic = new string("");


        public void OnGet(DateTime wcDt, string clinician, string clinic, string searchTerm)
        {
            ClinicFormSetup(wcDt, clinician, clinic, searchTerm);
        }

        public void OnPost(DateTime wcDt, string clinician, string clinic, string searchTerm)
        {
            ClinicFormSetup(wcDt, clinician, clinic, searchTerm);
        }

        private void ClinicFormSetup(DateTime wcDt, string clinician, string clinic, string searchTerm)
        {
            try
            {                
                staffMemberList = _dc.GetStaffMemberList();
                clinicVenueList = _dc.GetVenueList();


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
                                
                appointmentList = _dc.GetAppointments(DateArray[0], DateArray[4], clinician, clinic);
                
                clinicSlotList = _dc.GetClinicSlots(DateArray[0], DateArray[4], clinician, clinic);

                if (searchTerm != null) //to search the waiting list for a CGU number
                {                    
                    waitingList = _dc.GetWaitingListByCGUNo(searchTerm);
                }
                else
                {
                    waitingList = _dc.GetWaitingList(clinician, clinic);
                }

                if (!clinic.IsNullOrEmpty())
                {
                    clinicVenue = _dc.GetVenueDetails(clinic);
                }

                if (!clinician.IsNullOrEmpty()) //if a clinician is selected as well
                {
                    staffMember = _dc.GetStaffDetails(clinician);
                    var Clinics = new List<CliniciansClinics>();
                    Clinics = _dc.GetCliniciansClinics(clinician);

                    clinicVenueList = clinicVenueList.Where(v => Clinics.Any(c => v.FACILITY == c.FACILITY)).ToList();
                }

                //openSlots = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
                openSlotList = _dc.GetOpenSlots(clinicSlotList);
            }
            catch (Exception ex) 
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}