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

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        public List<Outcome> outcomeList { get; set; }
        public List<AppType> appTypeList { get; set; }
        public Appointment appointment { get; set; }

        
        public void OnGet(string sRefID)
        {
            try
            {
                int iRefID = Int32.Parse(sRefID);

                appointment = dc.GetAppointmentDetails(iRefID);
                staffMember = dc.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = dc.GetVenueDetails(appointment.FACILITY);
                patient = dc.GetPatientDetails(appointment.MPI);
                staffMemberList = dc.GetStaffMemberList();
                clinicVenueList = dc.GetVenueList();
                outcomeList = dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = dc.GetAppTypeList();
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

                appointment = dc.GetAppointmentDetails(iRefID);
                staffMember = dc.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = dc.GetVenueDetails(appointment.FACILITY);
                patient = dc.GetPatientDetails(appointment.MPI);
                staffMemberList = dc.GetStaffMemberList();
                clinicVenueList = dc.GetVenueList();
                outcomeList = dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = dc.GetAppTypeList();

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
