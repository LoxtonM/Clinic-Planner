using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;

namespace CPTest.Pages
{
    public class IndexModel : PageModel
    {        
        private readonly DataContext _context;

        public IndexModel(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Outcome> Outcomes { get; set; }
        public IEnumerable<WaitingList> WaitingLists { get; set; }
        public ClinicVenue? ClinicVenue { get; set; }
        public IEnumerable<ClinicVenue> ClinicVenues { get; set; }
        public StaffMember? StaffMember { get; set; }
        public IEnumerable<StaffMember> StaffMembers { get; set; }
        public IEnumerable<ClinicSlot> ClinicSlots { get; set; }
        public IEnumerable<ClinicSlot> OpenSlots { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Appointment?> Appointments { get; set; }

        public DateTime[] DateArray = new DateTime[5];
        public DateTime[] TimeArray = new DateTime[120];
        
        public string wcDateString = new string("");
        public string sClinician = new string("");
        public string sClinic = new string("");


        public void OnGet(DateTime wcDt, string strClinician, string strClinic)
        {
            ClinicFormSetup(wcDt, strClinician, strClinic);
        }

        public void OnPost(DateTime wcDt, string strClinician, string strClinic)
        {
            ClinicFormSetup(wcDt, strClinician, strClinic);
        }

        private void ClinicFormSetup(DateTime wcDt, string strClinician, string strClinic)
        {
            ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
            StaffMembers = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);
            
            DateTime wcDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            string TheDay = wcDate.DayOfWeek.ToString();
            

            if (wcDt.ToString() != "01/01/0001 00:00:00")
            {
                wcDate = wcDt;
            }
            //we have to do it this way, because we can't simply pass the date to the HTML, for some reason!!!
            string dy = wcDate.ToString("dd");
            string mt = wcDate.ToString("MM");
            string yr = wcDate.ToString("yyyy");
            wcDateString = yr + "-" + mt + "-" + dy;
            sClinician = strClinician;
            sClinic = strClinic;

            DateTime initTime = wcDate.Add(new TimeSpan(8, 0, 0));

            for (int i = 0; i < 5; i++)
            {
                DateArray[i] = wcDate.AddDays(i);
            }

            for (int i = 0; i < 120; i++)
            {
                TimeArray[i] = initTime.AddMinutes(i * 5);
            }

            Appointments = _context.Appointments.Where(a => a.BOOKED_DATE >= DateArray[0] & a.BOOKED_DATE <= DateArray[4]);// & a.COUNSELED != "Declined" & a.COUNSELED != "Cancelled by professional" & a.COUNSELED != "Cancelled by patient");

            if (!strClinic.IsNullOrEmpty())
            {
                ClinicVenue = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == strClinic);
                if (!strClinician.IsNullOrEmpty())
                {
                    StaffMember = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == strClinician);
                    WaitingLists = _context.WaitingLists.Where(l => l.ClinicianID == strClinician & l.ClinicID == strClinic).ToList().OrderBy(l => l.AddedDate);
                    ClinicSlots = _context.ClinicSlots.Where(l => l.ClinicID == strClinic & l.ClinicianID == strClinician & l.SlotDate >= DateArray[0] & l.SlotDate <= DateArray[4]).ToList().OrderBy(l => l.SlotDate);
                    if (!Appointments.IsNullOrEmpty())
                    {
                        Appointments = Appointments.Where(a => a.FACILITY == strClinic & a.STAFF_CODE_1.ToUpper() == strClinician & a.BOOKED_DATE >= DateArray[0] & a.BOOKED_DATE <= DateArray[4]).ToList().OrderBy(a => a.BOOKED_DATE);
                    }
                }
                else
                {
                    WaitingLists = _context.WaitingLists.Where(l => l.ClinicID == strClinic).ToList().OrderBy(l => l.AddedDate);
                    ClinicSlots = _context.ClinicSlots.Where(l => l.ClinicID == strClinic & l.SlotDate >= DateArray[0] & l.SlotDate <= DateArray[4]).ToList().OrderBy(l => l.SlotDate);
                    if (!Appointments.IsNullOrEmpty())
                    {
                        Appointments = Appointments.Where(a => a.FACILITY == strClinic & a.BOOKED_DATE >= DateArray[0] & a.BOOKED_DATE <= DateArray[4]).ToList().OrderBy(a => a.BOOKED_DATE);
                    }
                }
            }
            else
            { 
                if (!strClinician.IsNullOrEmpty())
                {
                    StaffMember = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == strClinician);
                    WaitingLists = _context.WaitingLists.Where(l => l.ClinicianID == strClinician).ToList().OrderBy(l => l.AddedDate);
                    ClinicSlots = _context.ClinicSlots.Where(l => l.ClinicianID == strClinician & l.SlotDate >= DateArray[0] & l.SlotDate <= DateArray[4]).ToList().OrderBy(l => l.SlotDate);
                    if (!Appointments.IsNullOrEmpty())
                    {
                        Appointments = Appointments.Where(a => a.STAFF_CODE_1.ToUpper() == strClinician & a.BOOKED_DATE >= DateArray[0] & a.BOOKED_DATE <= DateArray[4]).ToList().OrderBy(a => a.BOOKED_DATE);
                    }
                }
                else
                {
                    WaitingLists = _context.WaitingLists.ToList().OrderBy(l => l.AddedDate);
                    ClinicSlots = _context.ClinicSlots.Where(l => l.SlotDate >= DateArray[0] & l.SlotDate <= DateArray[4]).ToList().OrderBy(l => l.SlotDate);
                }
            }



            //ClinicSlots = _context.ClinicSlots.Where(l => l.ClinicID == strClinic & l.SlotDate >= DateArray[0] & l.SlotDate <= DateArray[4]).ToList().OrderBy(l => l.SlotDate);
            
            OpenSlots = ClinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
        }
    }
}