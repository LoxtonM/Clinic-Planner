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
        private readonly IStaffUserData _staffData;
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
            _staffData = new StaffUserData(_context);
            _clinicalVenueData = new ClinicVenueData(_context);
            _waitingListData = new WaitingListData(_context);
            _priority = new PriorityData(_context);
            _audit = new AuditSqlServices(_config);
        }

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }     
        public WaitingList? waitingList { get; set; }
        public List<Priority> priorityList { get; set; }
        
        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;


        public void OnGet(int id, string clinicID, string clinicianID, string? wcDateString, string? clinicianSelected, string? clinicSelected)
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
                int intID=0;
                
                if (id != 0)
                {
                    waitingList = _waitingListData.GetWaitingListEntryByID(id);
                    intID = waitingList.IntID;
                }

                priorityList = _priority.GetPriorityList();

                if (clinicianID != null)
                {
                    staffMember = _staffData.GetStaffMemberDetailsByStaffCode(clinicianID);                    
                }

                if (clinicID != null)
                {
                    clinicVenue = _clinicalVenueData.GetVenueDetails(clinicID);
                }

                if (intID != 0)
                {
                    patient = _patientData.GetPatientDetailsByIntID(intID);
                }

                if (patient == null)
                {                    
                    Response.Redirect("PatientNotFound?intID=" + intID.ToString() + "&clinicianID=" + clinicianID + "&clinicID=" + clinicID, true);
                }

                staffMemberList = _staffData.GetStaffMemberList();

                clinicVenueList = _clinicalVenueData.GetVenueList();

                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                _audit.CreateAudit(_staffData.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE, "Waiting List Modify", "IntID=" + intID.ToString(), _ip.GetIPAddress());
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
        
        public void OnPost(int id, string clinicianID, string clinicID, int priorityLevel, bool isRemoval, string? wcDateString, string? clinicianSelected, string? clinicSelected)
        {
            try
            {
                if (clinicianID != null)
                {
                    staffMember = _staffData.GetStaffMemberDetailsByStaffCode(clinicianID);
                }

                if (clinicID != null)
                {
                    clinicVenue = _clinicalVenueData.GetVenueDetails(clinicID);
                }

                waitingList = _waitingListData.GetWaitingListEntryByID(id);

                if (waitingList != null)
                {
                    patient = _patientData.GetPatientDetails(waitingList.MPI);
                }

                string sStaffCode = _staffData.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE;
                int intID = patient.INTID;
                string oldClinicianID = waitingList.ClinicianID; //for auditing
                string oldClinicID = waitingList.ClinicID;
                int oldPriorityLevel = waitingList.PriorityLevel;

                priorityList = _priority.GetPriorityList();

                staffMemberList = _staffData.GetStaffMemberList();

                clinicVenueList = _clinicalVenueData.GetVenueList();

                _ss.ModifyWaitingListEntry(id, clinicianID, clinicID, priorityLevel, oldClinicianID, oldClinicID, oldPriorityLevel, sStaffCode, isRemoval);

 
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
