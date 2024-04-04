using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AddToWLModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly DataConnections _dc;
        private readonly SqlServices _sql;
        public IEnumerable<ClinicVenue> clinicVenueList { get; set; }        
        public IEnumerable<StaffMember> staffMemberList { get; set; }
        public Patient Patient { get; set; }

        public AddToWLModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _sql = new SqlServices(_config);
        }
        
        public void OnGet(string cgu)
        {
            try
            {
                //ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
                clinicVenueList = _context.ClinicVenues.OrderBy(v => v.NAME);
                staffMemberList = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);

                if (cgu != null)
                {
                    Patient = _context.Patients.FirstOrDefault(p => p.CGU_No == cgu);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int mpi, string clinician, string clinic)
        {
            try
            {
                string staffCode = _dc.GetStaffDetailsByUsername("mnln").STAFF_CODE; //todo: change when login screen available

                //_sql.CreateWaitingListEntry(mpi, clinician, clinic, staffCode);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
