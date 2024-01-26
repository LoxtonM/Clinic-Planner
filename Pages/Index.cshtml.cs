﻿using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;


namespace CPTest.Pages
{
    public class IndexModel : PageModel
    {        
        private readonly DataContext _context;
        DataConnections dc;

        public IndexModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
        }
        public IEnumerable<Outcome> Outcomes { get; set; }
        public IEnumerable<WaitingList> waitingList { get; set; }
        public ClinicVenue? ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }
        public StaffMember? StaffMember { get; set; }
        public List<StaffMember> StaffMembers { get; set; }
        public IEnumerable<ClinicSlot> clinicSlots { get; set; }
        public IEnumerable<ClinicSlot> openSlots { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Appointment?> Appointments { get; set; }

        public DateTime[] DateArray = new DateTime[5];
        public DateTime[] TimeArray = new DateTime[120];

        //public string wcDateString = new string("");
        public DateTime wcDate;
        public string sClinician = new string("");
        public string sClinic = new string("");


        public void OnGet(DateTime wcDt, string strClinician, string strClinic, string sSearchTerm)
        {
            ClinicFormSetup(wcDt, strClinician, strClinic, sSearchTerm);
        }

        public void OnPost(DateTime wcDt, string strClinician, string strClinic, string sSearchTerm)
        {
            ClinicFormSetup(wcDt, strClinician, strClinic, sSearchTerm);
        }

        private void ClinicFormSetup(DateTime wcDt, string strClinician, string strClinic, string sSearchTerm)
        {
            try
            {                
                StaffMembers = dc.GetStaffMemberList();
                ClinicVenues = dc.GetVenueList();


                if (wcDt.ToString() != "01/01/0001 00:00:00")
                {
                    wcDate = wcDt;
                }
                else
                {
                    wcDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                }

                string TheDay = wcDate.DayOfWeek.ToString();
                sClinician = strClinician;
                sClinic = strClinic;
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
                                
                Appointments = dc.GetAppointments(DateArray[0], DateArray[4], strClinician, strClinic);                
                clinicSlots = dc.GetClinicSlots(DateArray[0], DateArray[4], strClinician, strClinic);

                if (sSearchTerm != null) //to search the waiting list for a CGU number
                {                    
                    waitingList = dc.GetWaitingListByCGUNo(sSearchTerm);
                }
                else
                {
                    waitingList = dc.GetWaitingList(strClinician, strClinic);
                }

                if (!strClinic.IsNullOrEmpty())
                {
                    ClinicVenue = dc.GetVenueDetails(strClinic);
                }

                if (!strClinician.IsNullOrEmpty()) //if a clinician is selected as well
                {
                    StaffMember = dc.GetStaffDetails(strClinician);
                    var Clinics = new List<CliniciansClinics>();
                    Clinics = dc.GetCliniciansClinics(strClinician);

                    ClinicVenues = ClinicVenues.Where(v => Clinics.Any(c => v.FACILITY == c.FACILITY)).ToList();
                }

                //openSlots = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
                openSlots = dc.GetOpenSlots(clinicSlots);
            }
            catch (Exception ex) 
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}