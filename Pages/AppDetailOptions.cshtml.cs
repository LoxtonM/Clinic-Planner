using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AppDetailOptionsModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAuditSqlServices _audit;

        public AppDetailOptionsModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);            
            _appointmentData = new AppointmentData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _audit = new AuditSqlServices(_config);
        }

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }        
        public Appointment appointment { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public int refID { get; set; }

        public void OnGet(string sRefID)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                
                refID = Int32.Parse(sRefID);

                appointment = _appointmentData.GetAppointmentDetails(refID);
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                patient = _patientData.GetPatientDetails(appointment.MPI);
                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Appt Details", "RefID=" + sRefID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
