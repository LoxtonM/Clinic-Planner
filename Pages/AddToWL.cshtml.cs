using CPTest.Connections;
using CPTest.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;

namespace CPTest.Pages
{
    public class AddToWLModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IConfiguration _config;        
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicalVenueData;
        private readonly IPatientData _patientData;
        private readonly IWaitingListSqlServices _ss;
        private readonly IPriorityData _priorityData;
        private readonly IReferralData _referralData;
        
        public IEnumerable<ClinicVenue> clinicVenueList { get; set; }        
        public IEnumerable<StaffMember> staffMemberList { get; set; }
        public IEnumerable<Priority> priorityList { get; set; }
        public IEnumerable<Referral> referralList { get; set; }
        public Patient Patient { get; set; }

        public AddToWLModel(ClinicalContext context, CPXContext cpxContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicalVenueData = new ClinicVenueData(_context, _cpxContext);
            _priorityData = new PriorityData(_context);
            _patientData = new PatientData(_context);
            _referralData = new ReferralData(_context);
            _ss = new WaitingListSqlServices(_config);
        }
        
        public void OnGet(string cgu)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinicVenueList = _clinicalVenueData.GetVenueList();       
                staffMemberList = _staffData.GetStaffMemberList();
                priorityList = _priorityData.GetPriorityList();

                if (cgu != null)
                {                    
                    Patient = _patientData.GetPatientDetailsByCGUNo(cgu);
                    if (Patient == null)
                    {                        
                        Response.Redirect("CGUNumberNotFound");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int mpi, string clin, string ven, int priorityLevel, int linkedRef)
        {
            try
            {
                clinicVenueList = _clinicalVenueData.GetVenueList();
                staffMemberList = _staffData.GetStaffMemberList();
                priorityList = _priorityData.GetPriorityList();
                string username = User.Identity.Name;
                string staffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;

                _ss.CreateWaitingListEntry(mpi, clin, ven, staffCode, priorityLevel, linkedRef);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
