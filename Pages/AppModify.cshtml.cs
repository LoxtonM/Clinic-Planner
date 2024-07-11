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
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAppTypeData _appTypeData;
        private readonly IOutcomeData _outcomeData;
        private readonly IAppointmentData _appointmentData;        
        private readonly IAppointmentSqlServices _ss;

        public AppModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;            
            _ss = new AppointmentSqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _appTypeData = new AppTypeData(_context);
            _outcomeData = new OutcomeData(_context);
            _appointmentData = new AppointmentData(_context);
        }

        public Patient patient { get; set; }
        public List<Patient> familyMembers { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        public List<Outcome> outcomeList { get; set; }
        public List<AppType> appTypeList { get; set; }
        public Appointment appointment { get; set; }

        
        public void OnGet(int refID)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                appointment = _appointmentData.GetAppointmentDetails(refID);
                int mpi = appointment.MPI;
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                patient = _patientData.GetPatientDetails(mpi);
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
                outcomeList = _outcomeData.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = _appTypeData.GetAppTypeList();
                familyMembers = _patientData.GetFamilyMembers(mpi);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int refID, DateTime dNewDate, DateTime dNewTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sInstructions, string sCancel, int? famMPI = 0)
        {
            try
            {
                //int refID = Int32.Parse(sRefID);
                string username = User.Identity.Name;
                appointment = _appointmentData.GetAppointmentDetails(refID);
                int mpi = appointment.MPI;
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                patient = _patientData.GetPatientDetails(mpi);
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
                outcomeList = _outcomeData.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                appTypeList = _appTypeData.GetAppTypeList();
                familyMembers = _patientData.GetFamilyMembers(mpi);

                string sNewTime = dNewTime.Hour.ToString() + ":" + dNewTime.Minute.ToString(); //for some reason, I can't just convert the time to a string!!!
                string sUser = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;

                _ss.ModifyAppointment(refID, dNewDate, sNewTime, appWith1, appWith2, appWith3, appLocation,
                appType, duration, sUser, sInstructions, sCancel, famMPI.GetValueOrDefault());

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
