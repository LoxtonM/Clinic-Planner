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
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAlertData _alertData;
        private readonly IAuditSqlServices _audit;

        public AppDetailOptionsModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);            
            _appointmentData = new AppointmentData(_context);
            _clinicVenueData = new ClinicVenueData(_context, _cpxContext);
            _alertData = new AlertData(_context);
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

        public void OnGet(string sRefID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
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
                    patient = _patientData.GetPatientDetails(67066);    //and obviously it can't just redirect it, it has to resolve the entire page first!!!
                    appointmentsList = new List<Appointment>();         //So we have to give it junk data and use the page to resolve the if condition.
                    alertsList = new List<Alert>();
                    Response.Redirect("Error?sError=Appointment not found - you may have clicked on a slot instead. Appointments should be blue or purple, " +
                        "and slots should be green. If you see a red slot, it shouldn't be there, please note the S: number and report it to the IT team.");
                }
                else
                {
                    appointment = _appointmentData.GetAppointmentDetails(refID);
                    staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                    patient = _patientData.GetPatientDetails(appointment.MPI);
                    clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                    appointmentsList = _appointmentData.GetAppointmentsForWholeFamily(refID);
                    
                    alertsList = _alertData.GetAlertsList(patient.MPI);

                    if (appointmentsList.Count > 1)
                    {
                        foreach (Appointment a in appointmentsList)
                        {
                            Patient p = _patientData.GetPatientDetails(a.MPI);
                            if (p.MPI != patient.MPI)
                            {
                                patientsList.Add(p);
                            }
                        }
                    }
                }
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Appt Details", "RefID=" + sRefID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
