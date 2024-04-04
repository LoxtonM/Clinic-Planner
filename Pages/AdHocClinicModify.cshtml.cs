using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace CPTest.Pages
{
    public class AdHocClinicModifyModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly DataConnections _dc;
        private readonly SqlServices _sql;

        public AdHocClinicModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _sql = new SqlServices(_config);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public void OnGet(int id)
        {
            try
            {
                adhocclinic = _dc.GetAdHocClinicDetails(id);
                clinician = _dc.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _dc.GetVenueDetails(adhocclinic.ClinicID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
