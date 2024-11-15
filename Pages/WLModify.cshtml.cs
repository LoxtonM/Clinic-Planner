using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class WLModifyModel : PageModel
    {

        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicalVenueData;
        private readonly IWaitingListData _waitingListData;        
        private readonly IWaitingListSqlServices _ss;        
        private readonly IPriorityData _priority;
        private readonly IAuditSqlServices _audit;

        public WLModifyModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _ss = new WaitingListSqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
            _clinicalVenueData = new ClinicVenueData(_context, _cpxContext);
            _waitingListData = new WaitingListData(_context);
            _priority = new PriorityData(_context);
            _audit = new AuditSqlServices(_config);
        }

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }     
        public WaitingList waitingList { get; set; }
        public List<Priority> priorityList { get; set; }
        
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;


        public void OnGet(int intID, string clinicID, string clinicianID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                wcDateStr = wcDateString;
                clinicianSel = clinicianSelected;
                clinicSel = clinicSelected;

                waitingList = _waitingListData.GetWaitingListEntry(intID, clinicianID, clinicID);

                priorityList = _priority.GetPriorityList();

                if (clinicianID != null)
                {
                    staffMember = _staffData.GetStaffDetails(clinicianID);
                }

                if (clinicID != null)
                {
                    clinicVenue = _clinicalVenueData.GetVenueDetails(clinicID);
                }

                if (intID != null)
                {
                    patient = _patientData.GetPatientDetailsByIntID(intID);
                }

                if (patient == null)
                {                    
                    Response.Redirect("PatientNotFound?intID=" + intID.ToString() + "&clinicianID=" + clinicianID + "&clinicID=" + clinicID, true);
                }

                staffMemberList = _staffData.GetStaffMemberList();

                clinicVenueList = _clinicalVenueData.GetVenueList();

                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Waiting List Modify", "IntID=" + intID.ToString());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
        
        public void OnPost(int mpi, string clinicianID, string clinicID, string oldClinicianID, string oldClinicID, int priorityLevel, int oldPriorityLevel, 
            bool isRemoval, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (clinicianID != null)
                {
                    staffMember = _staffData.GetStaffDetails(clinicianID);
                }

                if (clinicID != null)
                {
                    clinicVenue = _clinicalVenueData.GetVenueDetails(clinicID);
                }

                if (mpi != null)
                {
                    patient = _patientData.GetPatientDetails(mpi);
                }                

                string sStaffCode = _staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE;
                int intID = patient.INTID;                

                priorityList = _priority.GetPriorityList();

                staffMemberList = _staffData.GetStaffMemberList();

                clinicVenueList = _clinicalVenueData.GetVenueList();

                _ss.ModifyWaitingListEntry(intID, clinicianID, clinicID, priorityLevel, oldClinicianID, oldClinicID, oldPriorityLevel, sStaffCode, isRemoval);

                waitingList = _waitingListData.GetWaitingListEntry(intID, clinicianID, clinicID);

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
