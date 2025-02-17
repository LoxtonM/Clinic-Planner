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
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAppTypeData _appTypeData;
        private readonly IOutcomeData _outcomeData;
        private readonly ICancellationReasonData _cancelReasonData;
        private readonly IAppointmentData _appointmentData;        
        private readonly IAppointmentSqlServices _ss;

        public AppModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;            
            _ss = new AppointmentSqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _appTypeData = new AppTypeData(_cpxContext);
            _outcomeData = new OutcomeData(_context);
            _cancelReasonData = new CancellationReasonData(_cpxContext);
            _appointmentData = new AppointmentData(_context);
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
        public List<AppType> appTypeList { get; set; }
        public Appointment appointment { get; set; }
        public List<Appointment> appointmentsList { get; set; }
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public void OnGet(int refID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
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
                cancellationReasonsList = _cancelReasonData.GetCancellationReasonsList();
                appTypeList = _appTypeData.GetAppTypeList();
                
                appointmentsList = _appointmentData.GetAppointmentsForWholeFamily(refID);
                patientsList = new List<Patient>();
                familyMembers = _patientData.GetFamilyMembers(mpi);
                familyMembersList = new List<Patient>(); //we have to build the list first and add to it, we can't remove from an existing list

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

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

        public void OnPost(int refID, DateTime dNewDate, DateTime dNewTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sInstructions, string sCancel, string? sCancelReason, string? wcDateString, string? clinicianSelected, string? clinicSelected, int? famMPI = 0, bool? isReturnToWL = false)
        {
            try
            {                
                string username = User.Identity.Name;
                appointment = _appointmentData.GetAppointmentDetails(refID);
                int mpi = appointment.MPI;
                staffMember = _staffData.GetStaffDetails(appointment.STAFF_CODE_1);
                clinicVenue = _clinicVenueData.GetVenueDetails(appointment.FACILITY);
                patient = _patientData.GetPatientDetails(mpi);
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
                outcomeList = _outcomeData.GetOutcomeList().Where(o => o.CLINIC_OUTCOME.Contains("Cancelled")).ToList();
                cancellationReasonsList = _cancelReasonData.GetCancellationReasonsList();
                appTypeList = _appTypeData.GetAppTypeList();

                //appointmentsList = _appointmentData.GetAppointmentsForWholeFamily(refID);
                patientsList = new List<Patient>();      //and we have to create the lists, even if we're not using them, or it'll throw a fit.          
                familyMembersList = new List<Patient>();

                string sNewTime = dNewTime.Hour.ToString() + ":" + dNewTime.Minute.ToString(); //for some reason, I can't just convert the time to a string!!!
                string sUser = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;

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
