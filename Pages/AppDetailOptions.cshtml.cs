using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class AppDetailOptionsModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IPatientDataAsync _patientData;
        private readonly IAppointmentDataAsync _appointmentData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IAlertDataAsync _alertData;
        private readonly IAuditSqlServices _audit;

        public AppDetailOptionsModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _patientData = new PatientDataAsync(_context);            
            _appointmentData = new AppointmentDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _alertData = new AlertDataAsync(_context);
            _audit = new AuditSqlServices(_config);
        }
        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }        
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentsList { get; set; }
        public List<Patient> patientsList { get; set; }
        public List<Alert> alertsList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public int refID { get; set; }
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public async Task OnGet(string sRefID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                
                refID = Int32.Parse(sRefID);
                patientsList = new List<Patient>(); 
                
                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                if (refID == 0) //to catch those instances where the "booked" slot doesn't match an appointment
                {
                    patient = await _patientData.GetPatientDetails(67066);    //and obviously it can't just redirect it, it has to resolve the entire page first!!!
                    appointmentsList = new List<Appointment>();         //So we have to give it junk data and use the page to resolve the if condition.
                    alertsList = new List<Alert>();
                    Response.Redirect("Error?sError=Appointment not found - you may have clicked on a slot instead. Appointments should be blue or purple, " +
                        "and slots should be green. If you see a red slot, it shouldn't be there, please note the S: number and report it to the IT team.");
                }
                else
                {
                    appointment = await _appointmentData.GetAppointmentDetails(refID);
                    staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(appointment.STAFF_CODE_1);
                    patient = await _patientData.GetPatientDetails(appointment.MPI);
                    clinicVenue = await _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                    appointmentsList = await _appointmentData.GetAppointmentsForWholeFamily(refID);
                    
                    alertsList = await _alertData.GetAlertsList(patient.MPI);

                    if (appointmentsList.Count > 1)
                    {
                        foreach (Appointment a in appointmentsList)
                        {
                            Patient p = await _patientData.GetPatientDetails(a.MPI);
                            if (p.MPI != patient.MPI)
                            {
                                patientsList.Add(p);
                            }
                        }
                    }
                }
                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(await _staffData.GetStaffCode(User.Identity.Name), "Appt Details", "RefID=" + sRefID, _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
