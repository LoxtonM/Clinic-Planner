using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Data;

namespace CPTest.Pages
{
    public class AdHocClinicListModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IStaffData _staffData;
        private readonly IAdHocClinicData _adHocClinicData;
        public AdHocClinicListModel(ClinicalContext context, CPXContext cpxContext)
        {
            _context = context;
            _cpxContext = cpxContext;
            _staffData = new StaffData(_context);
            _adHocClinicData = new AdHocClinicData(_cpxContext);
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
