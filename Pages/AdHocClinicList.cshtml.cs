using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AdHocClinicListModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IStaffData _staffData;
        private readonly IAdHocClinicData _adHocClinicData;
        public AdHocClinicListModel(DataContext context)
        {
            _context = context;
            _staffData = new StaffData(_context);
            _adHocClinicData = new AdHocClinicData(_context);
        }
        
        public StaffMember clinician;
        public List<ClinicsAdded> adHocList { get; set; }
        public void OnGet(string staffCode)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinician = _staffData.GetStaffDetails(staffCode);
                adHocList = _adHocClinicData.GetAdHocList(staffCode).Where(c => c.ClinicDate >= DateTime.Now.Date).OrderByDescending(c => c.ClinicDate).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
    }
}
