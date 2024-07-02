using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class WLModifyModel : PageModel
    {

        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicalVenueData;
        private readonly IWaitingListData _waitingListData;        
        private readonly IWaitingListSqlServices _ss;        
        private readonly IPriorityData _priority;
        private readonly IAuditSqlServices _audit;

        public WLModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _ss = new WaitingListSqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
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
        public WaitingList waitingList { get; set; }
        public List<Priority> priorityList { get; set; }

        
        public void OnGet(int intID, string clinicID, string clinicianID)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

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
        
        public void OnPost(int mpi, string clinicianID, string clinicID, string oldClinicianID, string oldClinicID, int priorityLevel, int oldPriorityLevel, bool isRemoval)
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

                waitingList = _waitingListData.GetWaitingListEntry(intID, clinicianID, clinicID);

                priorityList = _priority.GetPriorityList();

                staffMemberList = _staffData.GetStaffMemberList();

                clinicVenueList = _clinicalVenueData.GetVenueList();

                _ss.ModifyWaitingListEntry(intID, clinicianID, clinicID, priorityLevel, oldClinicianID, oldClinicID, oldPriorityLevel, sStaffCode, isRemoval);
                               

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
