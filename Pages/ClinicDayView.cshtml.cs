using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicDayViewModel : PageModel
    {
        private readonly DataContext _context;
        private readonly MiscData _dc;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAppointmentData _appointmentData;
        private readonly ICliniciansClinicData _cliniciansClinicData;

        public ClinicDayViewModel(DataContext context)
        {
            _context = context;
            _dc = new MiscData(_context);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _appointmentData = new AppointmentData(_context);
            _cliniciansClinicData = new CliniciansClinicData(_context);
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
        public string clinician = new string("");
        public string clinic = new string("");

        public void OnGet(string? clinician, string? clinic, DateTime dClinicDate)
        {
            try
            {
                if (dClinicDate.ToString() == "01/01/0001 00:00:00")
                {
                    dClinicDate = DateTime.Today;
                }

                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();

                dDate = dClinicDate;

                clinician = clinician;
                clinic = clinic;
                DateTime initTime = dClinicDate.Add(new TimeSpan(8, 0, 0));

                for (int i = 0; i < 120; i++)
                {
                    TimeArray[i] = initTime.AddMinutes(i * 5);
                }

                appointmentList = _appointmentData.GetAppointmentsForADay(dClinicDate, clinician, clinic);
                
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

                if (clinic != null)
                {
                    clinicVenue = _clinicVenueData.GetVenueDetails(clinic);
                }

                if (clinician != null)
                {
                    staffMember = _staffData.GetStaffDetails(clinician);
                    var Clinics = new List<CliniciansClinics>();
                    Clinics = _cliniciansClinicData.GetCliniciansClinics(clinician);

                    clinicVenueList = clinicVenueList.Where(v => Clinics.Any(c => v.FACILITY == c.FACILITY)).ToList();
                }

                //openSlots = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
                //openSlotList = _dc.GetOpenSlots(clinicSlotList);
                
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string? clinician, string? clinic, DateTime dClinicDate)
        {

        }
    }
}
