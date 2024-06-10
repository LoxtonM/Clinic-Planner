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
        private readonly IAdHocClinicSqlServices _ss;

        public AdHocClinicModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _adHocClinicData = new AdHocClinicData(_context);
            _ss = new AdHocClinicSqlServices(_context, _config);
        }

        public ClinicsAdded adhocclinic {  get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }
        public void OnGet(int id)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                adhocclinic = _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = _staffData.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int id, int duration, int startHr, int startMin, int numSlots, DateTime dClinicDate)
        {
            try
            {
                adhocclinic = _adHocClinicData.GetAdHocClinicDetails(id);
                clinician = _staffData.GetStaffDetails(adhocclinic.ClinicianID);
                venue = _clinicVenueData.GetVenueDetails(adhocclinic.ClinicID);
                string username = User.Identity.Name;

                _ss.UpdateAdHocClinic(id, adhocclinic.ClinicianID, adhocclinic.ClinicID, duration, startHr, startMin, numSlots, dClinicDate, username);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
