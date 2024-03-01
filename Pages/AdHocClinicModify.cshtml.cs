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
        DataConnections dc;
        SqlServices sql;

        public AdHocClinicModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            dc = new DataConnections(_context);
            sql = new SqlServices(_config);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public void OnGet(int ID)
        {
            try
            {
                adhocclinic = dc.GetAdHocClinicDetails(ID);
                clinician = dc.GetStaffDetails(adhocclinic.ClinicianID);
                venue = dc.GetVenueDetails(adhocclinic.ClinicID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
