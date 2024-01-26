using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AppModifyModel : PageModel
    {

        private readonly DataContext _context;
        private readonly IConfiguration _config;
        DataConnections dc;
        SqlServices ss;

        public AppModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            dc = new DataConnections(_context);
            ss = new SqlServices(_config);       
        }

        public Patient Patient { get; set; }
        public StaffMember StaffMember { get; set; }
        public List<StaffMember> StaffMembers { get; set; }
        public ClinicVenue ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }
        public List<Outcome> Outcomes { get; set; }
        public List<AppType> AppTypes { get; set; }
        public Appointment Appointment { get; set; }

        
        public void OnGet(string sRefID)
        {
            try
            {
                int iRefID = Int32.Parse(sRefID);
                
                Appointment = dc.GetAppointmentDetails(iRefID);                
                StaffMember = dc.GetStaffDetails(Appointment.STAFF_CODE_1);                
                ClinicVenue = dc.GetVenueDetails(Appointment.FACILITY);                
                Patient = dc.GetPatientDetails(Appointment.MPI);                
                StaffMembers = dc.GetStaffMemberList();                
                ClinicVenues = dc.GetVenueList();
                Outcomes = dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                AppTypes = dc.GetAppTypeList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string sRefID, DateTime dNewDate, DateTime dNewTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int iDuration, string sInstructions, string sCancel)
        {
            try
            {
                int iRefID = Int32.Parse(sRefID);

                Appointment = dc.GetAppointmentDetails(iRefID);
                StaffMember = dc.GetStaffDetails(Appointment.STAFF_CODE_1);
                ClinicVenue = dc.GetVenueDetails(Appointment.FACILITY);
                Patient = dc.GetPatientDetails(Appointment.MPI);
                StaffMembers = dc.GetStaffMemberList();
                ClinicVenues = dc.GetVenueList();
                Outcomes = dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                AppTypes = dc.GetAppTypeList();

                string sNewTime = dNewTime.Hour.ToString() + ":" + dNewTime.Minute.ToString(); //for some reason, I can't just convert the time to a string!!!
                string sUser = dc.GetStaffDetailsByUsername("mnln").STAFF_CODE;

                ss.ModifyAppointment(iRefID, dNewDate, sNewTime, appWith1, appWith2, appWith3, appLocation,
                appType, iDuration, sUser, sInstructions, sCancel);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
