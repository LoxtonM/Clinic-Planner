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
        private readonly DocumentContext _docContext;
        private readonly IStaffData _staffData;
        private readonly IPatientData _patientData;
        private readonly IAppointmentData _appointmentData;        
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IConstantsData _constantsData;

        public ClinicLettersAndListsModel(ClinicalContext context, CPXContext cpxContext, DocumentContext docContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _docContext = docContext;
            _staffData = new StaffData(_context);
            _patientData = new PatientData(_context);
            _appointmentData = new AppointmentData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _constantsData = new ConstantsData(_docContext);
        }

        public Patient patient { get; set; }        
        public StaffMember staffMember { get; set; }
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentList { get; set; }
        public List<Appointment> appointmentListForFamily { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public bool synertecPrinterActive { get; set; }

        public void OnGet(int refID, string? sMessage, bool? isSuccess)
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

                if(!_constantsData.GetConstant("SynertecPrinterName", 2).Contains("0")) //we need to be able to disable it in case something isn't working right
                {
                    synertecPrinterActive = true;
                }

                foreach (Appointment appt in appointmentList)
                {
                    if(appt.STAFF_CODE_1 == appointment.STAFF_CODE_1 && appt.FACILITY == appointment.FACILITY && appt.BOOKED_TIME == appointment.BOOKED_TIME)
                    {
                        appointmentListForFamily.Add(appt);
                    }
                }

                message = sMessage;
                success = isSuccess.GetValueOrDefault();

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
