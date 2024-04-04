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
        private readonly DataConnections _dc;
        private readonly SqlServices _ss;

        public AppModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _ss = new SqlServices(_config);       
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
                int refID = Int32.Parse(sRefID);

                appointment = _dc.GetAppointmentDetails(refID);
                staffMember = _dc.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = _dc.GetVenueDetails(appointment.FACILITY);
                patient = _dc.GetPatientDetails(appointment.MPI);
                staffMemberList = _dc.GetStaffMemberList();
                clinicVenueList = _dc.GetVenueList();
                outcomeList = _dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = _dc.GetAppTypeList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string sRefID, DateTime dNewDate, DateTime dNewTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sInstructions, string sCancel)
        {
            try
            {
                int refID = Int32.Parse(sRefID);

                appointment = _dc.GetAppointmentDetails(refID);
                staffMember = _dc.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = _dc.GetVenueDetails(appointment.FACILITY);
                patient = _dc.GetPatientDetails(appointment.MPI);
                staffMemberList = _dc.GetStaffMemberList();
                clinicVenueList = _dc.GetVenueList();
                outcomeList = _dc.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = _dc.GetAppTypeList();

                string sNewTime = dNewTime.Hour.ToString() + ":" + dNewTime.Minute.ToString(); //for some reason, I can't just convert the time to a string!!!
                string sUser = _dc.GetStaffDetailsByUsername("mnln").STAFF_CODE;

                _ss.ModifyAppointment(refID, dNewDate, sNewTime, appWith1, appWith2, appWith3, appLocation,
                appType, duration, sUser, sInstructions, sCancel);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
