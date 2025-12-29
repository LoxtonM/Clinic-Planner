using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;

namespace CPTest.Pages
{
    public class AppModifyModel : PageModel
    {

        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IPatientDataAsync _patientData;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicVenueData;
        private readonly IActivityTypeDataAsync _appTypeData;
        private readonly IOutcomeDataAsync _outcomeData;
        private readonly ICancellationReasonDataAsync _cancelReasonData;
        private readonly IAppointmentDataAsync _appointmentData;        
        private readonly IAppointmentSqlServices _ss;

        public AppModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;            
            _ss = new AppointmentSqlServices(_config);
            _patientData = new PatientDataAsync(_context);
            _staffData = new StaffUserDataAsync(_context);
            _clinicVenueData = new ClinicVenueDataAsync(_context);
            _appTypeData = new ActivityTypeDataAsync(_context);
            _outcomeData = new OutcomeDataAsync(_context);
            _cancelReasonData = new CancellationReasonDataAsync(_cpxContext);
            _appointmentData = new AppointmentDataAsync(_context);
        }

        public Patient patient { get; set; }
        public List<Patient> familyMembers { get; set; }
        public List<Patient> familyMembersList { get; set; }
        public List<Patient> patientsList { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        public List<Outcome> outcomeList { get; set; }
        public List<CancellationReason> cancellationReasonsList { get; set; }
        public List<ActivityType> appTypeList { get; set; }
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentsList { get; set; }
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public async Task OnGet(int refID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                appointment = await _appointmentData.GetAppointmentDetails(refID);
                int mpi = appointment.MPI;
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(appointment.STAFF_CODE_1);
                clinicVenue = await _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                patient = await _patientData.GetPatientDetails(mpi);
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();
                outcomeList = await _outcomeData.GetOutcomeList();
                outcomeList = outcomeList.Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                cancellationReasonsList = await _cancelReasonData.GetCancellationReasonsList();
                appTypeList = await _appTypeData.GetApptTypes();
                
                appointmentsList = await _appointmentData.GetAppointmentsForWholeFamily(refID);
                patientsList = new List<Patient>();
                familyMembers = await _patientData.GetFamilyMembers(mpi);
                familyMembersList = new List<Patient>(); //we have to build the list first and add to it, we can't remove from an existing list

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

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

                //add to familyMembersList where appointment is not already added
                foreach (var f in familyMembers)
                {
                    if (!patientsList.Contains(f))
                    {
                        familyMembersList.Add(f);
                    }
                }                
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(int refID, DateTime dNewDate, DateTime dNewTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sInstructions, string sCancel, string? sCancelReason, string? wcDateString, string? clinicianSelected, string? clinicSelected, int? famMPI = 0, bool? isReturnToWL = false)
        {
            try
            {                
                string username = User.Identity.Name;
                appointment = await _appointmentData.GetAppointmentDetails(refID);
                int mpi = appointment.MPI;
                staffMember = await _staffData.GetStaffMemberDetailsByStaffCode(appointment.STAFF_CODE_1);
                clinicVenue = await _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                patient = await _patientData.GetPatientDetails(mpi);
                staffMemberList = await _staffData.GetClinicalStaffList();
                clinicVenueList = await _clinicVenueData.GetVenueList();
                outcomeList = await _outcomeData.GetOutcomeList();
                outcomeList = outcomeList.Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                cancellationReasonsList = await _cancelReasonData.GetCancellationReasonsList();
                appTypeList = await _appTypeData.GetApptTypes();

                //appointmentsList = _appointmentData.GetAppointmentsForWholeFamily(refID);
                patientsList = new List<Patient>();      //and we have to create the lists, even if we're not using them, or it'll throw a fit.          
                familyMembersList = new List<Patient>();

                string sNewTime = dNewTime.Hour.ToString() + ":" + dNewTime.Minute.ToString(); //for some reason, I can't just convert the time to a string!!!
                string sUser = await _staffData.GetStaffCode(username);

                _ss.ModifyAppointment(refID, dNewDate, sNewTime, appWith1, appWith2, appWith3, appLocation,
                appType, duration, sUser, sInstructions, sCancel, famMPI.GetValueOrDefault(), isReturnToWL.GetValueOrDefault(), sCancelReason);

                wcDateStr = HttpUtility.UrlEncode(wcDateString);
                clinicianSel = HttpUtility.UrlEncode(clinicianSelected);
                clinicSel = HttpUtility.UrlEncode(clinicSelected);

                string returnUrl = "Index?wcDt=" + wcDateStr;
                if (clinicianSel != null) { returnUrl = returnUrl + $"&clinician={clinicianSel}"; }
                if (clinicSel != null) { returnUrl = returnUrl + $"&clinic={clinicSel}"; }

                Response.Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
