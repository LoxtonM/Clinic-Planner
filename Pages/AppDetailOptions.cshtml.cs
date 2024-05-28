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
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;
        private readonly IClinicVenueData _clinicVenueData;

        public AppDetailOptionsModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);            
            _appointmentData = new AppointmentData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
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
                //int refID = 0;
                refID = Int32.Parse(sRefID);

                appointment = _appointmentData.GetAppointmentDetails(refID);
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                patient = _patientData.GetPatientDetails(appointment.MPI);
                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
