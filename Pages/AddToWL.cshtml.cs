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
        private readonly IConfiguration _config;        
        private readonly IStaffUserDataAsync _staffData;
        private readonly IClinicVenueDataAsync _clinicalVenueData;
        private readonly IPatientDataAsync _patientData;
        private readonly IWaitingListSqlServices _ss;
        private readonly IPriorityDataAsync _priorityData;
        private readonly IReferralDataAsync _referralData;
        
        public IEnumerable<ClinicVenue> clinicVenueList { get; set; }        
        public IEnumerable<StaffMember> staffMemberList { get; set; }
        public IEnumerable<Priority> priorityList { get; set; }
        public IEnumerable<Referral> referralList { get; set; }
        public Patient patient { get; set; }

        public AddToWLModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserDataAsync(_context);
            _clinicalVenueData = new ClinicVenueDataAsync(_context);
            _priorityData = new PriorityDataAsync(_context);
            _patientData = new PatientDataAsync(_context);
            _referralData = new ReferralDataAsync(_context);
            _ss = new WaitingListSqlServices(_config);
        }
        
        public async Task OnGet(string cgu)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinicVenueList = await _clinicalVenueData.GetVenueList();       
                staffMemberList = await _staffData.GetClinicalStaffList();
                priorityList = await _priorityData.GetPriorityList();
                

                if (cgu != null)
                {                    
                    patient = await _patientData.GetPatientDetailsByCGUNo(cgu);
                    if (patient == null)
                    {                        
                        Response.Redirect("CGUNumberNotFound");
                    }
                    else
                    {
                        referralList = await _referralData.GetActiveReferralsListForPatient(patient.MPI);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public async Task OnPost(int mpi, string clin, string ven, int priorityLevel, int linkedRef)
        {
            try
            {
                clinicVenueList = await _clinicalVenueData.GetVenueList();
                staffMemberList = await _staffData.GetClinicalStaffList();
                priorityList = await _priorityData.GetPriorityList();                
                string staffCode = await _staffData.GetStaffCode(User.Identity.Name);

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
