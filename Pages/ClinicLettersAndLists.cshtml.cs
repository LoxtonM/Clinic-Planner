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
        private readonly DocumentContext _docContext;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IPatientDataAsync _patientData;
        private readonly IAppointmentDataAsync _appointmentData;        
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IConstantsDataAsync _constantsData;

        public ClinicLettersAndListsModel(ClinicalContext context, CPXContext cpxContext, DocumentContext docContext, IConfiguration config)
        {
            _context = context;
            _docContext = docContext;
            _staffData = new StaffUserDataAsync(_context);
            _patientData = new PatientDataAsync(_context);
            _appointmentData = new AppointmentDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _constantsData = new ConstantsDataAsync(_docContext);
        }

        public Patient patient { get; set; }        
        public StaffMember staffMember { get; set; }
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentList { get; set; }
        public List<Appointment> appointmentListForFamily { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public string clinicianSel { get; set; }
        public string clinicSel { get; set; }
        public string wcDateStr { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public bool synertecPrinterActive { get; set; }

        public async Task OnGet(int refID, string? sMessage, bool? isSuccess, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                appointment = await _appointmentData.GetAppointmentDetails(refID);
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(appointment.STAFF_CODE_1);
                patient = await _patientData.GetPatientDetails(appointment.MPI);
                appointmentList = await _appointmentData.GetAppointmentsForADay(appointment.BOOKED_DATE.GetValueOrDefault(), appointment.STAFF_CODE_1, appointment.FACILITY);
                appointmentListForFamily = new List<Appointment>();

                string cs = await _constantsData.GetConstant("SynertecPrinterName", 2);

                if (!cs.Contains("0")) //we need to be able to disable it in case something isn't working right
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

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                appointmentListForFamily = appointmentListForFamily.Distinct().ToList();

                clinicVenue = await _clinicVenueData.GetVenueDetails(appointment.FACILITY);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
