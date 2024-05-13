using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AdHocClinicModifyModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IAdHocClinicData _adHocClinicData;

        public AdHocClinicModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _adHocClinicData = new AdHocClinicData(_context);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public void OnGet(int id)
        {
            try
            {
                adhocclinic = _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = _staffData.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
