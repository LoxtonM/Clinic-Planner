using CPTest.Connections;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;
using CPTest.Data;

namespace CPTest.Pages
{
    public class ClinicLettersAndListsModel : PageModel
    {
        private readonly ClinicalContext _context;        
        private readonly CPXContext _cpxContext;
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;        
        private readonly IClinicVenueData _clinicVenueData;

        public ClinicLettersAndListsModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);
            _appointmentData = new AppointmentData(_context);
            _clinicVenueData = new ClinicVenueData(_context, _cpxContext);
        }

        public Patient patient { get; set; }        
        public StaffMember staffMember { get; set; }
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentList { get; set; }
        public List<Appointment> appointmentListForFamily { get; set; }
        public ClinicVenue clinicVenue { get; set; }

        public void OnGet(int refID)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                appointment = _appointmentData.GetAppointmentDetails(refID);
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                patient = _patientData.GetPatientDetails(appointment.MPI);
                appointmentList = _appointmentData.GetAppointmentsForADay(appointment.BOOKED_DATE.GetValueOrDefault(), appointment.STAFF_CODE_1, appointment.FACILITY);
                appointmentListForFamily = new List<Appointment>();

                foreach (Appointment appt in appointmentList)
                {
                    if(appt.STAFF_CODE_1 == appointment.STAFF_CODE_1 && appt.FACILITY == appointment.FACILITY && appt.BOOKED_TIME == appointment.BOOKED_TIME)
                    {
                        appointmentListForFamily.Add(appt);
                    }
                }

                appointmentListForFamily = appointmentListForFamily.Distinct().ToList();

                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
